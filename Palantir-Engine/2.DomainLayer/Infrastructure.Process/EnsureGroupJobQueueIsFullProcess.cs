namespace Ix.Palantir.Infrastructure.Process
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Engine.Configuration;
    using Ix.Palantir.Logging;
    using Ix.Palantir.Utilities;

    public class EnsureGroupJobQueueIsFullProcess
    {
        private static readonly Regex GroupRegex = new Regex(@"IxVkGroupId=(\d+)", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        private readonly IVkGroupRepository groupRepository;
        private readonly IFeedRepository feedRepository;
        private readonly JmxClientConfiguration jmxConfiguration;

        private readonly ILog log;

        public EnsureGroupJobQueueIsFullProcess(IVkGroupRepository groupRepository, IFeedRepository feedRepository, JmxClientConfiguration jmxConfiguration, ILog log)
        {
            this.groupRepository = groupRepository;
            this.feedRepository = feedRepository;
            this.jmxConfiguration = jmxConfiguration;
            this.log = log;
        }

        public void Run()
        {
            this.log.Debug("Group job queue checking started");

            var vkGroups = this.groupRepository.GetGroups();
            var allPossibleQueueItems = this.GetPossibleQueueItems(vkGroups);
            var allFeedsInQueue = this.GetExistingQueueItems();

            foreach (var feedQueueItem in allFeedsInQueue)
            {
                var item = allPossibleQueueItems.FirstOrDefault(x => x.VkGroupId == feedQueueItem.VkGroupId);

                if (item != null)
                {
                    allPossibleQueueItems.Remove(item);
                }
            }

            if (allPossibleQueueItems.Count > 0)
            {
                this.log.WarnFormat("Following group items are missing: {0}. Adding them...", this.Serialize(allPossibleQueueItems));

                foreach (var item in allPossibleQueueItems)
                {
                    this.feedRepository.PutVkGroupToQueue(item);
                }

                this.log.Debug("Missing items are added");
            }
            else
            {
                this.log.Debug("Feed job queue is full. Everything is fine");
            }

            this.log.Debug("Group job queue checking finished");
        }

        private IList<GroupQueueItem> GetPossibleQueueItems(IEnumerable<VkGroup> vkGroups)
        {
            return vkGroups.Select(vkGroup => new GroupQueueItem(vkGroup.Id)).ToList();
        }

        private IEnumerable<GroupQueueItem> GetExistingQueueItems()
        {
            IList<GroupQueueItem> items = new List<GroupQueueItem>();
            java.util.List jobQueueItems = Client.getGroupJobQueueItems(this.jmxConfiguration.AmqJmxUrl, this.jmxConfiguration.BrokerJmxName, "GroupJobQueue");

            for (int i = 0; i < jobQueueItems.size(); i++)
            {
                var queueItem = jobQueueItems.get(i) as string;

                if (queueItem != null)
                {
                    var boxedGroupId = GroupRegex.Match(queueItem).Groups[1].Value;
                    var item = new GroupQueueItem(int.Parse(boxedGroupId));
                    items.Add(item);
                }
            }

            return items;
        }

        private string Serialize(IEnumerable<GroupQueueItem> items)
        {
            SeparatedStringBuilder stringBuilder = new SeparatedStringBuilder("; ");

            foreach (var item in items)
            {
                stringBuilder.AppendFormatWithSeparator("<{0}>", item.VkGroupId.ToString());
            }

            return stringBuilder.ToString();
        }    
    }
}