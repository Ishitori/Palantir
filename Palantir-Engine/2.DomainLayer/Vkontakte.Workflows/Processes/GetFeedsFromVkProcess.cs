namespace Ix.Palantir.Vkontakte.Workflows.Processes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataAccess.API.Repositories;
    using DomainModel;
    using Framework.ObjectFactory;
    using Ix.Palantir.Configuration;
    using Ix.Palantir.Configuration.API;
    using Ix.Palantir.Localization.API;
    using Ix.Palantir.Utilities;
    using Ix.Palantir.Vkontakte.API;
    using Ix.Palantir.Vkontakte.API.Access;
    using Ix.Palantir.Vkontakte.Workflows.FeedProcessor;

    using Logging;
    using Providers;
    using Queueing.API.Command;

    public class GetFeedsFromVkProcess
    {
        private const int CONST_BatchProcessingSize = 500;

        private readonly IVkConnectionBuilder vkConnectionBuilder;
        private readonly IVkGroupRepository groupRepository;
        private readonly IDictionary<QueueItemType, IFeedProvider> feedProviders;
        private readonly ILog log;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly IWebUtilities webUtilities;

        private readonly IConfigurationProvider configProvider;

        public GetFeedsFromVkProcess(IVkConnectionBuilder vkConnectionBuilder, IVkGroupRepository groupRepository, ILog log, IConfigurationProvider configProvider, IDateTimeHelper dateTimeHelper, IWebUtilities webUtilities)
        {
            this.log = log;
            this.configProvider = configProvider;
            this.dateTimeHelper = dateTimeHelper;
            this.webUtilities = webUtilities;
            this.vkConnectionBuilder = vkConnectionBuilder;
            this.groupRepository = groupRepository;
            IProcessingStrategy strategy = new ProcessingStrategy(configProvider, groupRepository, this.dateTimeHelper);
            this.feedProviders = new List<IFeedProvider>
            {
                new WallPostFeedProvider(this.log, this.dateTimeHelper, strategy),
                new WallPostCommentsFeedProvider(this.log, Factory.GetInstance<IPostRepository>(), this.dateTimeHelper, strategy),
                new PhotoFeedProvider(this.log, this.dateTimeHelper, strategy),
                new PhotoAlbumDetailsFeedProvider(Factory.GetInstance<IPhotoRepository>(), this.dateTimeHelper, this.log),
                new MembersFeedProvider(this.log, this.dateTimeHelper),
                new MembersCountFeedProvider(this.log, this.dateTimeHelper),
                new MemberLikesFeedProvider(this.log, this.dateTimeHelper, Factory.GetInstance<IListRepository>(), Factory.GetInstance<IVkResponseMapper>(), strategy),
                new MemberSharesFeedProvider(this.log, this.dateTimeHelper, Factory.GetInstance<IListRepository>(), Factory.GetInstance<IVkResponseMapper>(), strategy),
                new MemberSubscriptionFeedProvider(this.log, Factory.GetInstance<IListRepository>(), Factory.GetInstance<IVkResponseMapper>(), Factory.GetInstance<IVkDataLimits>(), this.dateTimeHelper),
                new VideoFeedProvider(this.log, this.dateTimeHelper, strategy),
                new VideoCommentFeedProvider(this.log, Factory.GetInstance<IVideoRepository>(), this.dateTimeHelper, strategy),
                new VideoLikesFeedProvider(this.log, Factory.GetInstance<IVideoRepository>(), Factory.GetInstance<IVkResponseMapper>(), this.dateTimeHelper, strategy),
                new AdminsFeedProvider(this.log, this.dateTimeHelper),
                new TopicFeedProvider(this.log, this.dateTimeHelper, strategy),
                new TopicCommentFeedProvider(this.log, Factory.GetInstance<ITopicRepository>(), this.dateTimeHelper, strategy)
            }.ToDictionary(x => x.SupportedFeedType);
        }

        public void ProcessNextQueueItem()
        {
            this.log.Info("GetFeedsFromVKAction process started");

            var processingConfig = this.configProvider.GetConfigurationSection<FeedProcessingConfig>();
            var selector = !string.IsNullOrWhiteSpace(processingConfig.FeedFilter) ? processingConfig.FeedFilter : null;

            using (ICommandReceiver commandReceiver = Factory.GetInstance<ICommandReceiver>().Open(processingConfig.InputQueueId, selector))
            {
                IVkDataProvider vkDataProvider = this.vkConnectionBuilder.GetVkDataProvider();

                for (int i = 0; i < CONST_BatchProcessingSize; i++)
                {
                    FeedQueueItem queueItem = null;

                    try
                    {
                        queueItem = commandReceiver.GetCommand<FeedQueueItem>();

                        if (queueItem == null)
                        {
                            this.log.Info("No items in queue found. Processing stopped.");
                            return;
                        }

                        this.ProcessQueueItem(queueItem, vkDataProvider, processingConfig);
                    }
                    finally
                    {
                        if (queueItem != null)
                        {
                            queueItem.MarkAsCompleted();

                            using (ICommandSender commandSender = Factory.GetInstance<ICommandSender>().Open(processingConfig.InputQueueId))
                            {
                                commandSender.SendCommand(queueItem.Copy());
                            }
                        }
                    }
                }
            }
        }

        private void ProcessQueueItem(FeedQueueItem queueItem, IVkDataProvider vkDataProvider, FeedProcessingConfig processingConfig)
        {
            try
            {
                if (!this.feedProviders.ContainsKey(queueItem.QueueItemType))
                {
                    throw new ArgumentException(string.Format("Unsupported feed type provided: \"{0}\"", queueItem.QueueItemType));
                }

                var feedProvider = this.feedProviders[queueItem.QueueItemType];
                var vkGroup = this.groupRepository.GetGroupById(queueItem.VkGroupId);

                if (vkGroup == null)
                {
                    this.log.InfoFormat("Group with Id = \"{0}\" not found. Processing stopped.", queueItem.VkGroupId);
                    return;
                }

                this.log.InfoFormat("Fetching of feed '{0}' for VkGroupId {1} is started", queueItem.QueueItemType, queueItem.VkGroupId);

                using (ICommandSender commandSender = Factory.GetInstance<ICommandSender>().Open(processingConfig.OutputQueueId))
                {
                    foreach (var dataFeed in feedProvider.GetFeeds(vkDataProvider, vkGroup))
                    {
                        dataFeed.TtlInMinutes = processingConfig.TtlInMinutes;
                        commandSender.SendCommand(dataFeed);
                    }

                    var terminator = new DataFeed
                                        {
                                            IsSequenceTerminator = true,
                                            ReceivedAt = this.dateTimeHelper.GetDateTimeNow(),
                                            SendingDate = this.dateTimeHelper.GetDateTimeNow(),
                                            VkGroupId = vkGroup.Id,
                                            Type = feedProvider.ProvidedDataType,
                                            FetchingServer = this.webUtilities.GetServerName(),
                                            FetchingProcess = this.webUtilities.GetApplicationPoolName()
                                        };

                    commandSender.SendCommand(terminator);
                }

                this.log.InfoFormat("Fetching of feed '{0}' for VkGroupId {1} is finished", queueItem.QueueItemType, queueItem.VkGroupId);
            }
            catch (Exception exc)
            {
                this.log.Error(string.Format("Fetching of feed '{0}' for VkGroupId {1} is FAILED. Reason: {2}", queueItem.QueueItemType, queueItem.VkGroupId, exc));
            }
        }
    }
}