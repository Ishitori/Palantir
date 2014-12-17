namespace Ix.Palantir.Vkontakte.Workflows.Providers
{
    using System.Collections.Generic;
    using System.Linq;

    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Localization.API;
    using Ix.Palantir.Logging;
    using Ix.Palantir.Vkontakte.API;
    using Ix.Palantir.Vkontakte.API.Access;
    using Ix.Palantir.Vkontakte.API.Responses.MemberSubscription;

    public class MemberSubscriptionFeedProvider : IFeedProvider
    {
        private readonly ILog log;
        private readonly IListRepository listRepository;
        private readonly IVkResponseMapper responseMapper;
        private readonly IVkDataLimits dataLimits;
        private readonly IDateTimeHelper dateTimeHelper;

        public MemberSubscriptionFeedProvider(ILog log, IListRepository listRepository, IVkResponseMapper responseMapper, IVkDataLimits dataLimits, IDateTimeHelper dateTimeHelper)
        {
            this.log = log;
            this.listRepository = listRepository;
            this.responseMapper = responseMapper;
            this.dataLimits = dataLimits;
            this.dateTimeHelper = dateTimeHelper;
        }

        public QueueItemType SupportedFeedType
        {
            get { return QueueItemType.MemberSubscription; }
        }
        public DataFeedType ProvidedDataType
        {
            get { return DataFeedType.MemberSubscription; }
        }

        public IEnumerable<DataFeed> GetFeeds(IVkDataProvider dataProvider, VkGroup vkGroup)
        {
            IList<long> memberVkIds = this.listRepository.GetMemberVkIds(vkGroup.Id);

            foreach (var memberId in memberVkIds)
            {
                int offsetCounter = 0;
                IList<response> feedsPerOneMember = new List<response>();

                while (true)
                {
                    var subscriptionFeed = dataProvider.GetMemberSubscriptions(memberId.ToString(), offsetCounter);
                    this.log.DebugFormat("Member subscription feed is received: {0}", subscriptionFeed.Feed);

                    if (subscriptionFeed.group != null && subscriptionFeed.group.Length > 0)
                    {
                        feedsPerOneMember.Add(subscriptionFeed);
                    }

                    if (subscriptionFeed.group == null || subscriptionFeed.group.Length == 0)
                    {
                        var dataFeed = this.Join(vkGroup.Id, memberId, feedsPerOneMember);

                        if (dataFeed != null)
                        {
                            yield return dataFeed;
                        }

                        break;
                    }

                    if (subscriptionFeed.group.Length < this.dataLimits.MemberSubscriptionLimits)
                    {
                        var dataFeed = this.Join(vkGroup.Id, memberId, feedsPerOneMember);

                        if (dataFeed != null)
                        {
                            yield return dataFeed;
                        }

                        break;
                    }

                    offsetCounter += subscriptionFeed.group.Length;
                }
            }
        }

        private DataFeed Join(int vkGroupId, long vkMemberId, IList<response> feedsPerOneMember)
        {
            if (feedsPerOneMember == null || feedsPerOneMember.Count == 0)
            {
                return null;
            }

            int i = 0;
            var responseGroups = feedsPerOneMember.SelectMany(feedPerMember => feedPerMember.@group).ToList();

            var memberFeed = new response();
            memberFeed.group = new responseGroup[responseGroups.Count];
            memberFeed.list = true.ToString();

            foreach (var group in responseGroups)
            {
                memberFeed.group[i++] = group;
            }

            string subscriptionFeed = this.responseMapper.MapResponseObject(memberFeed);

            DataFeed dataFeed = new DataFeed
            {
                ReceivedAt = this.dateTimeHelper.GetDateTimeNow(),
                Feed = subscriptionFeed,
                VkGroupId = vkGroupId,
                RelatedObjectId = vkMemberId.ToString(),
                Type = DataFeedType.MemberSubscription
            };

            return dataFeed;
        }
    }
}