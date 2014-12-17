namespace Ix.Palantir.Vkontakte.Workflows.FeedProcessor
{
    using API;
    using API.Responses.MemberInformation;
    using DataAccess.API.Repositories;
    using DataAccess.API.Repositories.Changes;

    using DomainModel;
    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.Queueing.API.Command;
    using Ix.Palantir.Vkontakte.Workflows.VkMappers;

    using Logging;

    public class MembersFeedProcessor : IFeedProcessor
    {
        private const string CONST_UpdateMembersDeltaProcessQueue = "MembersDeltaQueue";

        private readonly ILog log;
        private readonly IVkResponseMapper responseMapper;
        private readonly IMemberRepository memberRepository;
        private readonly IMemberMapper memberMapper;

        public MembersFeedProcessor(ILog log, IVkResponseMapper responseMapper, IMemberRepository memberRepository, IMemberMapper memberMapper)
        {
            this.log = log;
            this.responseMapper = responseMapper;
            this.memberRepository = memberRepository;
            this.memberMapper = memberMapper;
        }

        public void Process(DataFeed dataFeed, VkGroup group)
        {
            var feed = this.responseMapper.MapResponse<response>(dataFeed.Feed);

            foreach (var member in feed.user)
            {
                this.ProcessUser(member, group);
            }
        }
        public void ProcessTerminator(int vkGroupId, int feedTypeVersion)
        {
            using (ICommandSender commandSender = Factory.GetInstance<ICommandSender>().Open(CONST_UpdateMembersDeltaProcessQueue))
            {
                var command = new UpdateMembersDeltaCommand
                {
                    VkGroupId = vkGroupId,
                    Version = feedTypeVersion
                };

                commandSender.SendCommand(command);
            }
        }

        private void ProcessUser(responseUser memberData, VkGroup group)
        {
            var savedMember = this.memberRepository.GetMember(group.Id, memberData.uid);

            if (savedMember != null)
            {
                this.UpdateExistingMember(savedMember, memberData);
            }
            else
            {
                this.SaveNewMember(memberData, group);
            }
        }

        private void SaveNewMember(responseUser memberData, VkGroup @group)
        {
            Member member = this.memberMapper.CreateMember(memberData, @group);
            this.memberMapper.UpdateMemberFields(member, memberData);
            this.memberRepository.SaveMember(member);
        }

        private void UpdateExistingMember(Member savedMember, responseUser memberData)
        {
            this.log.DebugFormat("Member with VkId={0} is already in database", memberData.uid);
            MemberChangeContext context = this.memberMapper.UpdateMemberFields(savedMember, memberData);
            savedMember.IsDeleted = false;
            this.memberRepository.UpdateMember(savedMember, context);
        }
    }
}