namespace Ix.Palantir.DataAccess.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using DomainModel;
    using Framework.ObjectFactory;

    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.Localization.API;
    using Ix.Palantir.Queueing.API.Command;

    public class FeedRepository : IFeedRepository
    {
        private readonly IDateTimeHelper dateTimeHelper;

        public FeedRepository(IDateTimeHelper dateTimeHelper)
        {
            this.dateTimeHelper = dateTimeHelper;
        }

        public void AddVkGroupFeedFetchingToQueue(int vkGroupId)
        {
            var dataFeedTypes = Enum.GetValues(typeof(QueueItemType)).Cast<QueueItemType>().Where(t => t != QueueItemType.Undefined);
            var queueItems = dataFeedTypes.Select(t => new FeedQueueItem()
                                                           {
                                                               VkGroupId = vkGroupId,
                                                               QueueItemType = t,
                                                               CreationDate = this.dateTimeHelper.GetDateTimeNow(),
                                                           });

            this.PutItemsToQueue(queueItems);
            this.PutVkGroupToQueue(new GroupQueueItem(vkGroupId));
        }

        public void PutItemsToQueue(IEnumerable<FeedQueueItem> queueItems)
        {
            using (ICommandSender commandSender = Factory.GetInstance<ICommandSender>().Open("FeedJobQueue"))
            {
                foreach (var item in queueItems)
                {
                    commandSender.SendCommand(item);
                }
            }
        }

        public void PutVkGroupToQueue(GroupQueueItem groupQueueItem)
        {
            using (ICommandSender commandSender = Factory.GetInstance<ICommandSender>().Open("GroupJobQueue"))
            {
                commandSender.SendCommand(groupQueueItem);
            }
        }
    }
}