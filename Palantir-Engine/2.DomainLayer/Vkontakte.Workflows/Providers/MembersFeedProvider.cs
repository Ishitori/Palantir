namespace Ix.Palantir.Vkontakte.Workflows.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DomainModel;
    using Ix.Palantir.Localization.API;
    using Ix.Palantir.Vkontakte.API.Access;
    using Ix.Palantir.Vkontakte.Workflows.FeedProcessor;
    using Logging;

    public class MembersFeedProvider : IFeedProvider
    {
        private const int CONST_MembersToFetchAtOnce = 100;
        
        private readonly ILog log;
        private readonly IDateTimeHelper dateTimeHelper;

        public MembersFeedProvider(ILog log, IDateTimeHelper dateTimeHelper)
        {
            this.log = log;
            this.dateTimeHelper = dateTimeHelper;
        }

        public QueueItemType SupportedFeedType
        {
            get
            {
                return QueueItemType.Members;
            }
        }
        public DataFeedType ProvidedDataType
        {
            get
            {
                return DataFeedType.MembersInfo;
            }
        }
        
        public IEnumerable<DataFeed> GetFeeds(IVkDataProvider dataProvider, VkGroup vkGroup)
        {
            int offsetCounter = 0;

            while (true)
            {
                var memberFeed = dataProvider.GetGroupMemberIds(vkGroup.Id.ToString(), offsetCounter);
                this.log.DebugFormat("Users feed is received: {0}", memberFeed.Feed);

                if (memberFeed.users == null || memberFeed.users.Length == 0)
                {
                    break;
                }

                if (memberFeed.users[0].uid == null || memberFeed.users[0].uid.Length == 0)
                {
                    break;
                }

                var userIds = memberFeed.users[0].uid.Select(u => u.Value).ToList();
                var maxFetches = (int)Math.Floor((double)userIds.Count / CONST_MembersToFetchAtOnce);

                for (int i = 0; i < maxFetches; i++)
                {
                    List<string> ids = userIds.Skip(i * CONST_MembersToFetchAtOnce).Take(CONST_MembersToFetchAtOnce).ToList();

                    if (ids.Count == 0)
                    {
                        break;
                    }

                    var userProfiles = dataProvider.GetUserProfiles(ids);

                    DataFeed dataFeed = new DataFeed
                    {
                        ReceivedAt = this.dateTimeHelper.GetDateTimeNow(),
                        Feed = userProfiles.Feed,
                        VkGroupId = vkGroup.Id,
                        Type = DataFeedType.MembersInfo
                    };

                    yield return dataFeed;
                }

                offsetCounter += userIds.Count;
            }
        }
    }
}