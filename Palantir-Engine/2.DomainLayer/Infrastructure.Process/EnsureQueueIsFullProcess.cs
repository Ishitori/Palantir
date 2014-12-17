namespace Ix.Palantir.Infrastructure.Process
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using DataAccess.API.Repositories;
    using DomainModel;
    using Ix.Palantir.Engine.Configuration;
    using Ix.Palantir.Localization.API;
    using Ix.Palantir.Logging;
    using Ix.Palantir.Utilities;

    public class CheckFeedJobQueueProcess
    {
        private static readonly Regex GroupRegex = new Regex(@"IxVkGroupId=(\d+)", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex QueueItemTypeRegex = new Regex(@"IxDataFeedType=(\w+)", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        private readonly IVkGroupRepository groupRepository;
        private readonly IFeedRepository feedRepository;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly JmxClientConfiguration jmxConfiguration;
        private readonly ILog log;

        public CheckFeedJobQueueProcess(IVkGroupRepository groupRepository, IFeedRepository feedRepository, IDateTimeHelper dateTimeHelper, JmxClientConfiguration jmxConfiguration, ILog log)
        {
            this.groupRepository = groupRepository;
            this.feedRepository = feedRepository;
            this.dateTimeHelper = dateTimeHelper;
            this.jmxConfiguration = jmxConfiguration;
            this.log = log;
        }

        public void Run()
        {
            this.log.Debug("Feed job queue checking started");

            var vkGroups = this.groupRepository.GetGroups();
            var dataFeedTypes = Enum.GetValues(typeof(QueueItemType)).Cast<QueueItemType>().Where(t => t != QueueItemType.Undefined).ToList();
            var allPossibleQueueItems = this.GetPossibleQueueItems(vkGroups, dataFeedTypes);
            var allFeedsInQueue = this.GetExistingQueueItems();

            foreach (var feedQueueItem in allFeedsInQueue)
            {
                var item = allPossibleQueueItems.FirstOrDefault(x => x.VkGroupId == feedQueueItem.VkGroupId && x.QueueItemType == feedQueueItem.QueueItemType);

                if (item != null)
                {
                    allPossibleQueueItems.Remove(item);
                }
            }

            if (allPossibleQueueItems.Count > 0)
            {
                this.log.WarnFormat("Following feed items are missing: {0}. Adding them...", this.Serialize(allPossibleQueueItems));
                this.feedRepository.PutItemsToQueue(allPossibleQueueItems);
                this.log.Debug("Missing items are added");
            }
            else
            {
                this.log.Debug("Feed job queue is full. Everything is fine");
            }

            this.log.Debug("Feed job queue checking finished");
        }

        private IList<FeedQueueItem> GetPossibleQueueItems(IEnumerable<VkGroup> vkGroups, List<QueueItemType> dataFeedTypes)
        {
            IList<FeedQueueItem> possibleItems = new List<FeedQueueItem>();

            foreach (var vkGroup in vkGroups)
            {
                foreach (var dataFeedType in dataFeedTypes)
                {
                    FeedQueueItem item = new FeedQueueItem
                                         {
                                                 VkGroupId = vkGroup.Id,
                                                 QueueItemType = dataFeedType,
                                                 CreationDate = this.dateTimeHelper.GetDateTimeNow()
                                         };

                    possibleItems.Add(item);
                }
            }

            return possibleItems;
        }
        private IEnumerable<FeedQueueItem> GetExistingQueueItems()
        {
            IList<FeedQueueItem> items = new List<FeedQueueItem>();
            java.util.List jobQueueItems = Client.getFeedJobQueueItems(this.jmxConfiguration.AmqJmxUrl, this.jmxConfiguration.BrokerJmxName, "FeedJobQueue");

            for (int i = 0; i < jobQueueItems.size(); i++)
            {
                var queueItem = jobQueueItems.get(i) as string;

                if (queueItem != null)
                {
                    var boxedVkGroupId = GroupRegex.Match(queueItem).Groups[1].Value;
                    var boxedQueueItemType = QueueItemTypeRegex.Match(queueItem).Groups[1].Value;

                    var item = new FeedQueueItem
                                   {
                                       VkGroupId = int.Parse(boxedVkGroupId),
                                       QueueItemType = (QueueItemType)Enum.Parse(typeof(QueueItemType), boxedQueueItemType)
                                   };
                    items.Add(item);
                }
            }

            return items;
        }

        private string Serialize(IEnumerable<FeedQueueItem> items)
        {
            SeparatedStringBuilder stringBuilder = new SeparatedStringBuilder("; ");

            foreach (var item in items)
            {
                stringBuilder.AppendFormatWithSeparator("<{0}, {1}>", item.VkGroupId.ToString(), item.QueueItemType.ToString());
            }

            return stringBuilder.ToString();
        }
    }
}