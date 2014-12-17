namespace Ix.Palantir.Vkontakte.Workflows.FeedProcessor
{
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Logging;
    using Ix.Palantir.Vkontakte.API;
    using Ix.Palantir.Vkontakte.API.Responses.LikeShareFeed;

    public class MemberLikesFeedProcessor : IFeedProcessor
    {
        private readonly IMemberLikeSharesRepository memberLikesRepository;
        private readonly ILog log;
        private readonly IVkResponseMapper responseMapper;

        public MemberLikesFeedProcessor(IMemberLikeSharesRepository memberLikesRepository, IVkResponseMapper responseMapper, ILog log)
        {
            this.memberLikesRepository = memberLikesRepository;
            this.log = log;
            this.responseMapper = responseMapper;
        }

        public void Process(DataFeed dataFeed, VkGroup group)
        {
            var feed = this.responseMapper.MapResponse<response>(dataFeed.Feed);
            this.ProcessLike(feed, group);
        }
        public void ProcessTerminator(int vkGroupId, int feedTypeVersion)
        {
        }

        private void ProcessLike(response feed, VkGroup vkGroup)
        {
            if (feed.users[0].uid == null || feed.users[0].uid.Length == 0)
            {
                return;
            }

            foreach (var memberId in feed.users[0].uid)
            {
                var like = this.memberLikesRepository.GetLike(vkGroup.Id, memberId.Value, int.Parse(feed.ParentObjectId), (LikeShareType)feed.LikeShareType);

                if (like != null)
                {
                    this.log.DebugFormat("MemberLike for MemberId={0}, ItemId={1}, ItemType={2} is already in database", memberId.Value, feed.ParentObjectId, feed.LikeShareType);
                    continue;
                }
                
                like = new MemberLike
                {
                    ItemId = int.Parse(feed.ParentObjectId),
                    VkGroupId = vkGroup.Id,
                    ItemType = (LikeShareType)feed.LikeShareType,
                    VkId = memberId.Value
                };

                this.memberLikesRepository.SaveLike(like);
            }
        }
    }
}