﻿namespace Ix.Palantir.Vkontakte.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess.API;
    using DataAccess.API.Repositories;
    using DomainModel;
    using Framework.ObjectFactory;
    using Ix.Palantir.Localization.API;
    using Queueing.API.Command;

    public class FillJobQueueTask
    {
        private readonly IUnitOfWorkProvider unitOfWorkProvider;
        private readonly IVkGroupRepository groupRepository;
        private readonly IDateTimeHelper dateTimeHelper;

        public FillJobQueueTask(IUnitOfWorkProvider unitOfWorkProvider, IVkGroupRepository groupRepository, IDateTimeHelper dateTimeHelper)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
            this.groupRepository = groupRepository;
            this.dateTimeHelper = dateTimeHelper;
        }

        public void Execute()
         {
             using (this.unitOfWorkProvider.CreateUnitOfWork())
             {
                 var vkGroups = this.groupRepository.GetGroups();

                 // var vkGroups = new[] { new VkGroup() { Id = 26046814 } /*, new VkGroup() { Id = 29532168 }, new VkGroup() { Id = 14395935 }, new VkGroup() { Id = 29350491 }*/ }; //this.groupRepository.GetGroups();
                 var dataFeedTypes = Enum.GetValues(typeof(QueueItemType)).Cast<QueueItemType>().Where(t => t == QueueItemType.Members).ToList();

                 foreach (var vkGroup in vkGroups)
                 {
                     // this.AddTaskPerGroup(dataFeedType, vkGroups);
                     this.AddTasksPerGroup(vkGroup, dataFeedTypes);
                 }
             }
         }

        /*private void AddTaskPerGroup(QueueItemType dataFeedType, IEnumerable<VkGroup> vkGroups)
        {
            using (ICommandSender commandSender = Factory.GetInstance<ICommandSender>().Open("FeedJobQueue"))
            {
                foreach (var vkGroup in vkGroups)
                {
                    var item = this.GetQueueItem(vkGroup, dataFeedType);
                    commandSender.SendCommand(item);
                }
            }
        }*/

        private void AddTasksPerGroup(VkGroup vkGroup, IEnumerable<QueueItemType> dataFeedTypes)
        {
            var items = this.GetPossibleQueueItems(vkGroup, dataFeedTypes);

            using (ICommandSender commandSender = Factory.GetInstance<ICommandSender>().Open("GroupJobQueue"))
            {
                commandSender.SendCommand(new GroupQueueItem(vkGroup.Id));
            }

            using (ICommandSender commandSender = Factory.GetInstance<ICommandSender>().Open("FeedJobQueue"))
            {
                foreach (var item in items)
                {
                    commandSender.SendCommand(item);
                }
            }
        }

        private IEnumerable<FeedQueueItem> GetPossibleQueueItems(VkGroup group, IEnumerable<QueueItemType> dataFeedTypes)
        {
            IList<FeedQueueItem> possibleItems = new List<FeedQueueItem>();

            foreach (var dataFeedType in dataFeedTypes)
            {
                var item = this.GetQueueItem(@group, dataFeedType);
                possibleItems.Add(item);
            }

            return possibleItems;
        }

        private FeedQueueItem GetQueueItem(VkGroup @group, QueueItemType dataFeedType)
        {
            FeedQueueItem item = new FeedQueueItem()
            {
                VkGroupId = @group.Id,
                QueueItemType = dataFeedType,
                CreationDate = this.dateTimeHelper.GetDateTimeNow(),
            };

            return item;
        }
    }
}