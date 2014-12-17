namespace Ix.Palantir.Infrastructure.Process
{
    using System;
    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.DataAccess;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Logging;
    using Ix.Palantir.Queueing.API.Command;

    public class MembersInOutUpdateProcess
    {
        private const string CONST_MembersDeltaQueueName = "MembersDeltaQueue";

        private readonly IUnitOfWorkProvider unitOfWorkProvider;
        private readonly IMembersDeltaUpdater deltaUpdater;
        private readonly IVkGroupRepository vkGroupRepository;
        private readonly ILog log;

        public MembersInOutUpdateProcess(IUnitOfWorkProvider unitOfWorkProvider, IMembersDeltaUpdater deltaUpdater, IVkGroupRepository vkGroupRepository, ILog log)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
            this.deltaUpdater = deltaUpdater;
            this.vkGroupRepository = vkGroupRepository;
            this.log = log;
        }
        
        public void Run()
        {
            using (ICommandReceiver receiver = Factory.GetInstance<ICommandReceiver>().Open(CONST_MembersDeltaQueueName))
            {
                while (true)
                {
                    UpdateMembersDeltaCommand command = null;

                    try
                    {
                        command = receiver.GetCommand<UpdateMembersDeltaCommand>();

                        if (command == null)
                        {
                            this.log.Debug("No export command found. Processing stopped.");
                            return;
                        }

                        using (this.unitOfWorkProvider.CreateUnitOfWork())
                        {
                            var state = this.vkGroupRepository.GetProcessingState(command.VkGroupId, DataFeedType.MembersInfo);

                            if (state == null)
                            {
                                this.log.WarnFormat("MembersInfo processing state is not found for VkGroupId = \"{0}\"", command.VkGroupId);
                                return;
                            }

                            if (command.Version < state.Version)
                            {
                                this.log.WarnFormat("Processing state of command is outdate. Command.Version = {0} and State.Version = {1}", command.Version, state.Version);
                                return;
                            }

                            this.log.DebugFormat("Processing member delta for vkgroup = \"{0}\" on \"{1}\" for version = {2}", command.VkGroupId, command.SendingDate, command.Version);
                            this.deltaUpdater.CalculateMembersDelta(command.VkGroupId, DateTime.UtcNow);
                        }
                    }
                    catch (Exception exc)
                    {
                        this.log.ErrorFormat("Exception is occured while updating members delta for the group with Id = {0}: {1}", command != null ? command.VkGroupId : 0, exc.ToString());
                    }
                    finally
                    {
                        if (command != null)
                        {
                            command.MarkAsCompleted();
                        }
                    }
                }
            }
        }
    }
}