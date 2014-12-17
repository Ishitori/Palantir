namespace Ix.Palantir.Vkontakte.Workflows.FeedProcessor
{
    using API;
    using API.Responses.GroupMembers;
    using DomainModel;
    using Ix.Palantir.DataAccess.API.Repositories;

    public class MembersCountFeedProcessor : IFeedProcessor
    {
        private readonly IVkResponseMapper responseMapper;
        private readonly IMemberRepository memberRepository;

        public MembersCountFeedProcessor(IVkResponseMapper responseMapper, IMemberRepository memberRepository)
        {
            this.responseMapper = responseMapper;
            this.memberRepository = memberRepository;
        }

        public void Process(DataFeed dataFeed, VkGroup group)
        {
            var feed = this.responseMapper.MapResponse<response>(dataFeed.Feed);
            var usersMeta = new MembersMetaInfo
            {
                PostedDate = dataFeed.ReceivedAt,
                VkGroupId = group.Id,
                Count = feed.Count
            };

            this.memberRepository.SaveMembersCount(usersMeta);
        }
        public void ProcessTerminator(int vkGroupId, int feedTypeVersion)
        {
        }
    }
}