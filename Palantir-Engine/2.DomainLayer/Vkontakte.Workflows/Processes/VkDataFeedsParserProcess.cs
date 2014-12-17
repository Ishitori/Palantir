namespace Ix.Palantir.Vkontakte.Workflows.Processes
{
    using System;
    using Configuration;
    using DataAccess.API.Repositories;
    using DomainModel;
    using FeedProcessor;
    using Framework.ObjectFactory;
    using Ix.Palantir.Configuration.API;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.Localization.API;
    using Ix.Palantir.Utilities;

    using Logging;
    using Queueing.API.Command;

    public class VkDataFeedsParserProcess
    {
        private readonly ILog log;
        private readonly IVkGroupRepository groupRepository;
        private readonly IUnitOfWorkProvider transactionProvider;
        private readonly IProcessorFactory processorFactory;
        private readonly IConfigurationProvider configProvider;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly IWebUtilities webUtilities;

        public VkDataFeedsParserProcess(IVkGroupRepository groupRepository, IUnitOfWorkProvider transactionProvider, IProcessorFactory processorFactory, IConfigurationProvider configProvider, IDateTimeHelper dateTimeHelper, IWebUtilities webUtilities, ILog log)
        {
            this.log = log;
            this.processorFactory = processorFactory;
            this.configProvider = configProvider;
            this.dateTimeHelper = dateTimeHelper;
            this.webUtilities = webUtilities;
            this.groupRepository = groupRepository;
            this.transactionProvider = transactionProvider;
        }

        public void ProcessAllFeeds()
        {
            this.log.Info("ProcessVkFeedsAction process started");

            var processingConfig = this.configProvider.GetConfigurationSection<FeedProcessingConfig>();

            if (processingConfig.UseGroupFilter)
            {
                this.log.Info("Use group filter is enabled in the configuration. Starting processing groups one by one");
                this.ProcessSpecificGroupFeeds(processingConfig);
            }
            else
            {
                this.log.Info("Use group filter is disabled in the configuration. Starting processing all messages using defined filter");
                this.ProcessGroup();
            }
        }

        private void ProcessSpecificGroupFeeds(FeedProcessingConfig processingConfig)
        {
            using (ICommandReceiver groupReceiver = Factory.GetInstance<ICommandReceiver>().Open(processingConfig.GroupQueueId))
            {
                while (true)
                {
                    GroupQueueItem commandMessage = null;

                    try
                    {
                        commandMessage = groupReceiver.GetCommand<GroupQueueItem>();

                        if (commandMessage == null)
                        {
                            this.log.Error("No groups found to process. Processing stopped.");
                            return;
                        }

                        this.ProcessGroup(commandMessage.VkGroupId);
                    }
                    catch (Exception exc)
                    {
                        this.log.ErrorFormat("Exception is occured while processing a group Id = {0}: {1}", commandMessage != null ? commandMessage.VkGroupId : 0, exc.ToString());
                        return;
                    }
                    finally
                    {
                        if (commandMessage != null)
                        {
                            commandMessage.MarkAsCompleted();

                            using (ICommandSender groupSender = Factory.GetInstance<ICommandSender>().Open(processingConfig.GroupQueueId))
                            {
                                groupSender.SendCommand(new GroupQueueItem(commandMessage.VkGroupId));
                            }
                        }
                    }
                }
            }
        }

        private void ProcessGroup(int vkGroupId = 0)
        {
            string groupSelector = string.Empty;

            try
            {
                SelectorBuilder selectorBuilder = new SelectorBuilder();
                groupSelector = selectorBuilder.BuildSelector(vkGroupId);
                DataFeed firstCommand = this.ProcessFeedItem(groupSelector);

                if (firstCommand == null || firstCommand.IsSequenceTerminator)
                {
                    this.log.InfoFormat("No items for selector = {0} found in the queue. Processing stopped.", groupSelector);
                    return;
                }

                if (vkGroupId == 0)
                {
                    vkGroupId = firstCommand.VkGroupId;
                }

                groupSelector = selectorBuilder.BuildSelector(vkGroupId, firstCommand.Type.ToString());
                DataFeed processFeedItem = this.ProcessFeedItem(groupSelector, true);

                if (processFeedItem == null)
                {
                    this.log.InfoFormat("No items for selector = {0} found in the queue. Processing stopped.", groupSelector);
                }
            }
            catch (Exception exc)
            {
                exc.Data.Add("Selector", groupSelector);
                throw;
            }
        }
        private DataFeed ProcessFeedItem(string groupSelector, bool infiniteLoop = false)
        {
            var config = this.configProvider.GetConfigurationSection<FeedProcessingConfig>();

            using (ICommandReceiver receiver = Factory.GetInstance<ICommandReceiver>().Open(config.InputQueueId, groupSelector))
            {
                DataFeed command;

                do
                {
                    command = receiver.GetCommand<DataFeed>();

                    if (command == null)
                    {
                        return null;
                    }

                    if (command.IsSequenceTerminator)
                    {
                        this.ProcessTerminator(command);
                    }
                    else
                    {
                        this.ProcessFeed(command);
                    }
                }
                while (infiniteLoop);

                return command;
            }
        }

        private void ProcessFeed(DataFeed dataFeed)
        {
            VkGroup group = this.groupRepository.GetGroupById(dataFeed.VkGroupId);
            IFeedProcessor processor = this.processorFactory.Create(dataFeed.Type);

            using (var transaction = this.transactionProvider.CreateTransaction().Begin())
            {
                this.log.InfoFormat("Processing data feed with VkGroupId={0} ({1}) is started", dataFeed.VkGroupId, dataFeed.Type);

                try
                {
                    processor.Process(dataFeed, group);
                    transaction.Commit();
                    this.log.InfoFormat("Processing data feed with VkGroupId={0} ({1}) is finished", dataFeed.VkGroupId, dataFeed.Type);
                }
                catch (Exception exc)
                {
                    transaction.Rollback();
                    this.log.ErrorFormat("Data feed with VkGroupId={0} ({1}) is failed to be processed. Reason: {2}", dataFeed.VkGroupId, dataFeed.Type, exc.ToString());
                    throw;
                }
                finally
                {
                    dataFeed.MarkAsCompleted();
                    this.log.InfoFormat("Data feed with Id={0} ({1}) is deleted", dataFeed.VkGroupId, dataFeed.Type);
                }
            }
        }
        private void ProcessTerminator(DataFeed dataFeed)
        {
            VkGroupProcessingHistoryItem item = new VkGroupProcessingHistoryItem
                                                    {
                                                        FeedType = dataFeed.Type,
                                                        VkGroupId = dataFeed.VkGroupId,
                                                        FetchingDate = dataFeed.SendingDate,
                                                        FetchingServer = dataFeed.FetchingServer,
                                                        FetchingProcess = dataFeed.FetchingProcess,
                                                        ProcessingDate = this.dateTimeHelper.GetDateTimeNow(),
                                                        ProcessingServer = this.webUtilities.GetServerName(),
                                                        ProcessingProcess = this.webUtilities.GetApplicationPoolName()
                                                    };

            using (var transaction = this.transactionProvider.CreateTransaction().Begin())
            {
                try
                {
                    int version = this.groupRepository.SaveGroupProcessingHistoryItem(item);
                    transaction.Commit();
                    
                    IFeedProcessor processor = this.processorFactory.Create(dataFeed.Type);
                    processor.ProcessTerminator(dataFeed.VkGroupId, version);
                }
                catch (Exception exc)
                {
                    transaction.Rollback();
                    this.log.ErrorFormat("Terminator feed with VkGroupId={0} ({1}) is failed to be processed. Reason: {2}", dataFeed.VkGroupId, dataFeed.Type, exc.ToString());
                    throw;
                }
                finally
                {
                    dataFeed.MarkAsCompleted();
                }
            }
        }
    }
}