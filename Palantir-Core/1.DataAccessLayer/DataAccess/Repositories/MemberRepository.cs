namespace Ix.Palantir.DataAccess.Repositories
{
    using System;
    using System.Linq;

    using Dapper;
    using DomainModel;

    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DataAccess.API.Repositories.Changes;

    public class MemberRepository : IMemberRepository
    {
        private readonly IDataGatewayProvider dataGatewayProvider;

        public MemberRepository(IDataGatewayProvider dataGatewayProvider)
        {
            this.dataGatewayProvider = dataGatewayProvider;
        }

        public void SaveMembersCount(MembersMetaInfo membersMeta)
        {
            if (!membersMeta.IsTransient())
            {
                return;
            }

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                dataGateway.SaveEntity(membersMeta);
            }
        }
        public void SaveMember(Member member)
        {
            if (!member.IsTransient())
            {
                return;
            }

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                member.Id = dataGateway.Connection.Query<int>(@"insert into member(vkgroupid, name, gender, maritalstatus, birthday, birthmonth, birthyear, cityid, countryid, education, vkmemberid, status, isdeleted) values (@VkGroupId, @Name, @Gender, @MaritalStatus, @BirthDay, @BirthMonth, @BirthYear, @CityId, @CountryId, @Education, @VkMemberId, @Status, @IsDeleted) RETURNING id", this.Flatten(member)).First();

                if (member.Interests != null && member.Interests.Count > 0)
                {
                    foreach (var interest in member.Interests)
                    {
                        this.InsertInterest(interest, dataGateway);
                    }
                }

                var update = new MemberUpdate
                {
                    VkMemberId = member.VkMemberId,
                    VkGroupId = member.VkGroupId,
                    CreatedDate = DateTime.UtcNow,
                    IsNew = true
                };

                update.Id = dataGateway.Connection.Query<int>(@"INSERT INTO memberupdate(vkgroupid, vkmemberid, createddate, isnew) VALUES (@vkGroupId, @vkMemberId, @createdDate, @isNew) RETURNING id", update).First();
            }
        }
        public void UpdateMember(Member member, MemberChangeContext context = null)
        {
            if (member == null)
            {
                return;
            }

            MemberChangeContext changeContext = context ?? MemberChangeContext.Default;

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                if (changeContext.IsMemberChanged)
                {
                    dataGateway.Connection.Execute(@"update member set vkgroupid = @VkGroupId, name = @Name, gender = @Gender, maritalstatus = @MaritalStatus, birthday = @BirthDay, birthmonth = @BirthMonth, birthyear = @BirthYear, cityid = @CityId, countryid = @CountryId, education = @Education, vkmemberid = @VkMemberId, status = @Status, isdeleted = @IsDeleted where id = @Id", this.Flatten(member));
                }

                var update = new MemberUpdate
                {
                    VkMemberId = member.VkMemberId,
                    VkGroupId = member.VkGroupId,
                    CreatedDate = DateTime.UtcNow,
                    IsNew = false
                };

                update.Id = dataGateway.Connection.Query<int>(@"INSERT INTO memberupdate(vkgroupid, vkmemberid, createddate, isnew) VALUES (@vkGroupId, @vkMemberId, @createdDate, @isNew) RETURNING id", update).First();

                if (!changeContext.IsMemberInterestsChanged)
                {
                    return;
                }

                if (member.Interests != null && member.Interests.Count > 0)
                {
                    foreach (var interest in member.Interests)
                    {
                        if (interest.IsTransient())
                        {
                            this.InsertInterest(interest, dataGateway);
                        }
                        else
                        {
                            this.UpdateInterest(interest, dataGateway);
                        }
                    }
                }
            }
        }

        public Member GetMember(int groupId, string memberId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                const string QueryMember = @"select id, vkgroupid, name, gender, maritalstatus, cityid, countryid, education, vkmemberid, status, birthday, birthmonth, birthyear from member where vkgroupid = @vkgroupid and vkmemberid = @vkmemberid;";
                const string QueryInterests = @"select * from memberinterest where vkgroupid = @vkgroupid and vkmemberid = @vkmemberid";

                var result = dataGateway.Connection.Query<Member, BirthDate, Member>(
                    QueryMember,
                    (member, birthdate) => { member.BirthDate = birthdate; return member; },
                    new { vkgroupid = groupId, vkmemberid = long.Parse(memberId) },
                    splitOn: "birthday").FirstOrDefault();

                if (result == null)
                {
                    return null;
                }

                var interests = dataGateway.Connection.Query<MemberInterest>(QueryInterests, new { vkgroupid = groupId, vkmemberid = long.Parse(memberId) }).ToList();

                foreach (var interest in interests)
                {
                    result.Interests.Add(interest);
                }

                return result;
            }
        }

        private void InsertInterest(MemberInterest interest, IDataGateway dataGateway)
        {
            interest.Id = dataGateway.Connection.Query<int>(@"insert into memberinterest(vkgroupid, vkmemberid, title, type) values (@VkGroupId, @VkMemberId, @Title, @Type) RETURNING id", interest).First();
        }
        private void UpdateInterest(MemberInterest interest, IDataGateway dataGateway)
        {
            dataGateway.Connection.Execute(@"update memberinterest set vkgroupid = @VkGroupId, vkmemberid = @VkMemberId, title = @Title, type = @Type where id = @Id", interest);
        }

        private object Flatten(Member member)
        {
            return new
            {
                member.Id,
                member.VkGroupId,
                member.Name,
                member.Gender,
                member.MaritalStatus,
                member.BirthDate.BirthDay,
                member.BirthDate.BirthMonth,
                member.BirthDate.BirthYear,
                member.CityId,
                member.CountryId,
                member.Education,
                member.VkMemberId,
                member.Status,
                IsDeleted = false
            };
        }
    }
}