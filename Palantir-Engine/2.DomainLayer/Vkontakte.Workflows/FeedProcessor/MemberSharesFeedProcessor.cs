namespace Ix.Palantir.Vkontakte.Workflows.FeedProcessor
{
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Logging;
    using Ix.Palantir.Vkontakte.API;
    using Ix.Palantir.Vkontakte.API.Responses.LikeShareFeed;

    public class MemberSharesFeedProcessor : IFeedProcessor
    {
        private readonly IMemberLikeSharesRepository memberLikeSharesRepository;
        private readonly ILog log;
        private readonly IVkResponseMapper responseMapper;

        public MemberSharesFeedProcessor(IMemberLikeSharesRepository memberLikeSharesRepository, IVkResponseMapper responseMapper, ILog log)
        {
            this.memberLikeSharesRepository = memberLikeSharesRepository;
            this.log = log;
            this.responseMapper = responseMapper;
        }

        public void Process(DataFeed dataFeed, VkGroup group)
        {
            var feed = this.responseMapper.MapResponse<response>(dataFeed.Feed);
            this.ProcessShare(feed, group);
        }
        public void ProcessTerminator(int vkGroupId, int feedTypeVersion)
        {
        }

        private void ProcessShare(response feed, VkGroup vkGroup)
        {
            if (feed.users[0].uid == null || feed.users[0].uid.Length == 0)
            {
                return;
            }

            foreach (var memberId in feed.users[0].uid)
            {
                var share = this.memberLikeSharesRepository.GetShare(vkGroup.Id, memberId.Value, int.Parse(feed.ParentObjectId), (LikeShareType)feed.LikeShareType);

                if (share != null)
                {
                    this.log.DebugFormat("MemberShare for MemberId={0}, ItemId={1}, ItemType={2} is already in database", memberId.Value, feed.ParentObjectId, feed.LikeShareType);
                    continue;
                }

                share = new MemberShare
                {
                    ItemId = int.Parse(feed.ParentObjectId),
                    VkGroupId = vkGroup.Id,
                    ItemType = (LikeShareType)feed.LikeShareType,
                    VkId = memberId.Value
                };

                this.memberLikeSharesRepository.SaveShare(share);
            }
        }
    }
}