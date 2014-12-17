namespace Ix.Palantir.Vkontakte.Workflows.VkMappers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Ix.Palantir.DataAccess.API.Repositories.Changes;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Localization.API;
    using Ix.Palantir.Utilities;
    using Ix.Palantir.Vkontakte.API.Responses.MemberInformation;

    public class MemberMapper : IMemberMapper
    {
        private const int CONST_MaxInterestLength = 50;
        private static readonly Regex invalidInterestFormat = new Regex("(.)\\1{3,}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly IDateTimeHelper dateTimeHelper;

        public MemberMapper(IDateTimeHelper dateTimeHelper)
        {
            this.dateTimeHelper = dateTimeHelper;
        }

        public Member CreateMember(responseUser memberData, VkGroup group)
        {
            Member member = new Member()
                                {
                                    IsDeleted = false,
                                    VkGroupId = group.Id,
                                    VkMemberId = long.Parse(memberData.uid),
                                };

            return member;
        }
        public MemberChangeContext UpdateMemberFields(Member member, responseUser memberData)
        {
            if (memberData == null)
            {
                return null;
            }

            MemberChangeContext changeContext = new MemberChangeContext();
            Member newInstance = new Member()
                {
                    VkGroupId = member.VkGroupId,
                    VkMemberId = member.VkMemberId
                };

            newInstance.Name = this.GetMemberName(memberData);
            newInstance.Gender = this.GetGender(memberData.sex);
            this.AssignBirthDate(newInstance, memberData.bdate);
            newInstance.CityId = string.IsNullOrWhiteSpace(memberData.city) ? 0 : int.Parse(memberData.city);
            newInstance.CountryId = string.IsNullOrWhiteSpace(memberData.country) ? 0 : int.Parse(memberData.country);
            newInstance.Education = this.GetMemberEducation(memberData);
            newInstance.MaritalStatus = this.GetMaritalStatus(memberData.relation);
            newInstance.Status = this.GetMemberStatus(memberData);

            changeContext.IsMemberChanged = this.UpdateMemberInfo(member, newInstance);
            changeContext.IsMemberInterestsChanged = this.UpdateMemberInterests(member, memberData);

            return changeContext;
        }

        private bool UpdateMemberInfo(Member member, Member newInstance)
        {
            bool changesDetected = false;

            if (string.Compare(member.Name, newInstance.Name, StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                member.Name = newInstance.Name;
                changesDetected = true;
            }

            if (member.Gender != newInstance.Gender)
            {
                member.Gender = newInstance.Gender;
                changesDetected = true;
            }

            if (member.BirthDate != null && !member.BirthDate.Equals(newInstance.BirthDate))
            {
                member.BirthDate = newInstance.BirthDate;
                changesDetected = true;
            }

            if (member.CityId != newInstance.CityId)
            {
                member.CityId = newInstance.CityId;
                changesDetected = true;
            }

            if (member.CountryId != newInstance.CountryId)
            {
                member.CountryId = newInstance.CountryId;
                changesDetected = true;
            }

            if (member.Education != newInstance.Education)
            {
                member.Education = newInstance.Education;
                changesDetected = true;
            }

            if (member.MaritalStatus != newInstance.MaritalStatus)
            {
                member.MaritalStatus = newInstance.MaritalStatus;
                changesDetected = true;
            }

            if (member.Status != newInstance.Status)
            {
                member.Status = newInstance.Status;
                changesDetected = true;
            }

            return changesDetected;
        }

        private string GetMemberName(responseUser memberData)
        {
            int partsCount = 0;
            SeparatedStringBuilder name = new SeparatedStringBuilder(" ");

            if (!string.IsNullOrWhiteSpace(memberData.first_name))
            {
                name.AppendWithSeparator(memberData.first_name.Trim());
                partsCount++;
            }

            if (!string.IsNullOrWhiteSpace(memberData.last_name))
            {
                name.AppendWithSeparator(memberData.last_name.Trim());
                partsCount++;
            }

            if (partsCount == 2)
            {
                return name.ToString();
            }

            if (!string.IsNullOrWhiteSpace(memberData.screen_name))
            {
                return memberData.screen_name.Trim();
            }

            return name.ToString();
        }
        private Gender GetGender(string sex)
        {
            if (string.IsNullOrWhiteSpace(sex))
            {
                return Gender.Unknown;
            }

            if (sex == "1")
            {
                return Gender.Female;
            }

            if (sex == "2")
            {
                return Gender.Male;
            }

            return Gender.Unknown;
        }
        private void AssignBirthDate(Member member, string bdate)
        {
            if (string.IsNullOrWhiteSpace(bdate))
            {
                return;
            }

            var dateParts = bdate.Split('.');

            member.BirthDate.BirthDay = dateParts.Length > 0 ? int.Parse(dateParts[0]) : 0;
            member.BirthDate.BirthMonth = dateParts.Length > 1 ? int.Parse(dateParts[1]) : 0;
            member.BirthDate.BirthYear = dateParts.Length > 2 ? int.Parse(dateParts[2]) : 0;
        }
        private EducationLevel GetMemberEducation(responseUser memberData)
        {
            if (!string.IsNullOrWhiteSpace(memberData.university) && memberData.university != "0")
            {
                int graduationYear;

                if (int.TryParse(memberData.graduation, out graduationYear))
                {
                    return this.dateTimeHelper.GetDateTimeNow().Year >= graduationYear
                               ? EducationLevel.Higher
                               : EducationLevel.UncompletedHigher;
                }

                return EducationLevel.UncompletedHigher;
            }

            if (memberData.schools != null && memberData.schools.Length > 0)
            {
                return EducationLevel.Middle;
            }

            return EducationLevel.Unknown;
        }
        private MemberMaritalStatus GetMaritalStatus(string relation)
        {
            if (string.IsNullOrWhiteSpace(relation))
            {
                return MemberMaritalStatus.Unknown;
            }

            int relationCode;

            if (!int.TryParse(relation, out relationCode))
            {
                return MemberMaritalStatus.Unknown;
            }

            switch (relationCode)
            {
                case 4:
                    return MemberMaritalStatus.Married;

                case 1:
                case 2:
                case 3:
                case 5:
                case 6:
                case 7:
                    return MemberMaritalStatus.Single;

                default:
                    return MemberMaritalStatus.Unknown;
            }
        }
        private bool UpdateMemberInterests(Member member, responseUser memberData)
        {
            const string KeyTemplate = "{0}-{1}";
            bool changeDetected = false;

            IDictionary<string, MemberInterest> newInterests = this.GetNewInterests(member, memberData).ToDictionary(x => string.Format(KeyTemplate, x.Title.ToLower(), x.Type));
            IDictionary<string, MemberInterest> currentInterests = member.Interests.Distinct(new MemberInterestEqualityComparer()).ToDictionary(x => string.Format(KeyTemplate, x.Title.ToLower(), x.Type));

            foreach (var currentInterest in currentInterests)
            {
                if (!newInterests.ContainsKey(currentInterest.Key))
                {
                    member.Interests.Remove(currentInterest.Value);
                    changeDetected = true;
                }
            }

            foreach (var newInterest in newInterests)
            {
                if (!currentInterests.ContainsKey(newInterest.Key))
                {
                    member.Interests.Add(newInterest.Value);
                    changeDetected = true;
                }
            }

            return changeDetected;
        }

        private MemberStatus GetMemberStatus(responseUser memberData)
        {
            if (string.Compare(memberData.photo_rec, "http://vk.com/images/deactivated_c.gif", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                if (string.Compare(memberData.first_name, "no", StringComparison.InvariantCultureIgnoreCase) == 0 &&
                    string.Compare(memberData.last_name, "name", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    return MemberStatus.Deleted;
                }

                return MemberStatus.Blocked;
            }

            return MemberStatus.Active;
        }

        private IEnumerable<MemberInterest> GetNewInterests(Member member, responseUser memberData)
        {
            List<MemberInterest> currentInterests = new List<MemberInterest>();

            currentInterests.AddRange(this.GetInterestCollection(memberData.interests, MemberInterestType.General));
            currentInterests.AddRange(this.GetInterestCollection(memberData.movies, MemberInterestType.Movie));
            currentInterests.AddRange(this.GetInterestCollection(memberData.tv, MemberInterestType.TVShow));
            currentInterests.AddRange(this.GetInterestCollection(memberData.books, MemberInterestType.Book));
            currentInterests.AddRange(this.GetInterestCollection(memberData.games, MemberInterestType.VideoGame));

            currentInterests.ForEach(i => { i.VkMemberId = member.VkMemberId; i.VkGroupId = member.VkGroupId; });
            return currentInterests;
        }
        private IEnumerable<MemberInterest> GetInterestCollection(string interestsString, MemberInterestType type)
        {
            var interestsCollection = new Dictionary<string, MemberInterest>();

            if (string.IsNullOrWhiteSpace(interestsString))
            {
                return interestsCollection.Values;
            }

            foreach (var interest in interestsString.Split(','))
            {
                string key = interest.Trim().ToLower();

                if (interestsCollection.ContainsKey(key))
                {
                    continue;
                }

                if (invalidInterestFormat.IsMatch(key) || key.Length > CONST_MaxInterestLength)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                interestsCollection.Add(key, new MemberInterest { Title = interest.Trim(), Type = type });
            }

            return interestsCollection.Values;
        }
    }
}