namespace Ix.Palantir.Services
{
    using System;
    using System.IO;
    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.FileSystem.API;
    using Ix.Palantir.Querying.Common;
    using Ix.Palantir.Queueing.API.Command;
    using Ix.Palantir.Security;
    using Ix.Palantir.Security.API;
    using Ix.Palantir.Services.API.Export;

    public class ExportService : IExportService
    {
        private const string CONST_ExportQueueName = "ExportQueue";
        private const string CONST_ExportResultQueueName = "ExportResultQueue";

        private readonly IUnitOfWorkProvider unitOfWorkProvider;
        private readonly IProjectRepository projectRepository;
        private readonly ICurrentUserProvider currentUserProvider;

        public ExportService(IUnitOfWorkProvider unitOfWorkProvider, IProjectRepository projectRepository, ICurrentUserProvider currentUserProvider)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
            this.projectRepository = projectRepository;
            this.currentUserProvider = currentUserProvider;
        }

        public ExportSchedulingResult ScheduleExport(int projectId, DateRange dateRange)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                var vkGroup = this.projectRepository.GetVkGroup(projectId);
                var currentUser = this.currentUserProvider.GetCurrentUser();
                var ticketId = this.ScheduleExport(vkGroup, dateRange, currentUser.GetId());
                return new ExportSchedulingResult() { TicketId = ticketId };
            }
        }

        public ExportExecutionResult GetExportStatus(int projectId, string ticketId)
        {
            ExportResultCommand result = this.GetExportResult(ticketId);

            return result == null 
                ? new ExportExecutionResult() { TicketId = ticketId, IsFinished = false, FileUrl = string.Empty } 
                : new ExportExecutionResult() { TicketId = ticketId, IsFinished = true, IsSuccess = result.IsSuccess, FileUrl = result.FilePath };
        }

        public byte[] LoadExportFile(int projectId, string virtualFilePath)
        {
            var currentUser = this.currentUserProvider.GetCurrentUser();
            IFileSystem fileSystem = Factory.GetInstance<IFileSystemFactory>().CreateFileSystem(currentUser.GetId());
            
            using (Stream stream = fileSystem.LoadFile(virtualFilePath))
            {
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);
                return buffer;
            }
        }

        private string ScheduleExport(VkGroup vkGroup, DateRange dateRange, int initiatorUserId)
        {
            using (ICommandSender commandSender = Factory.GetInstance<ICommandSender>().Open(CONST_ExportQueueName))
            {
                string ticketId = Guid.NewGuid().ToString();
                ExportReportCommand command = new ExportReportCommand
                {
                    VkGroupId = vkGroup.Id,
                    DateRange = dateRange,
                    InitiatorUserId = initiatorUserId,
                    TicketId = ticketId
                };
                commandSender.SendCommand(command);
                return ticketId;
            }
        }
        private ExportResultCommand GetExportResult(string ticketId)
        {
            using (ICommandReceiver receiver = Factory.GetInstance<ICommandReceiver>().Open(CONST_ExportResultQueueName, string.Format("TicketId = '{0}'", ticketId)))
            {
                ExportResultCommand result = receiver.GetCommand<ExportResultCommand>();

                if (result != null)
                {
                    result.MarkAsCompleted();
                }

                return result;
            }
        }
    }
}