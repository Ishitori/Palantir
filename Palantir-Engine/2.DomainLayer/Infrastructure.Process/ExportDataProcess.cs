namespace Ix.Palantir.Infrastructure.Process
{
    using System;
    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.DataAccess.API.Export;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.FileSystem.API;
    using Ix.Palantir.Logging;
    using Ix.Palantir.Querying.Common;
    using Ix.Palantir.Queueing.API.Command;

    public class ExportDataProcess : IExportDataProcess
    {
        private const string CONST_ExportQueueName = "ExportQueue";
        private const string CONST_ExportResultQueueName = "ExportResultQueue";
        private const string CONST_DateTimeFormat = "yyyy-MM-dd_HH";

        private readonly ILog log;
        private readonly IExportDataProvider dataProvider;
        private readonly IFileSystemFactory fileSystemFactory;
        private readonly IVkGroupRepository vkGroupRepository;

        public ExportDataProcess(IExportDataProvider dataProvider, IFileSystemFactory fileSystemFactory, IVkGroupRepository vkGroupRepository, ILog log)
        {
            this.dataProvider = dataProvider;
            this.fileSystemFactory = fileSystemFactory;
            this.vkGroupRepository = vkGroupRepository;
            this.log = log;
        }
        
        public string ScheduleExport(VkGroup vkGroup, DateRange dateRange, int initiatorUserId)
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
        public void ProcessExportQueue()
        {
            using (ICommandReceiver receiver = Factory.GetInstance<ICommandReceiver>().Open(CONST_ExportQueueName))
            {
                while (true)
                {
                    ExportReportCommand exportCommand = null;

                    try
                    {
                        exportCommand = receiver.GetCommand<ExportReportCommand>();

                        if (exportCommand == null)
                        {
                            this.log.Debug("No export command found. Processing stopped.");
                            return;
                        }

                        string fileName = this.GenerateFileName(exportCommand);
                        IFileSystem fileSystem = this.fileSystemFactory.CreateFileSystem(exportCommand.InitiatorUserId);

                        byte[] exportResult = this.dataProvider.ExportToXlsx(exportCommand.VkGroupId, exportCommand.DateRange);
                        string virtualFilePath = fileSystem.SaveToFile(fileName, exportResult);

                        this.SendExportFinished(exportCommand, true, virtualFilePath);
                    }
                    catch (Exception exc)
                    {
                        this.log.ErrorFormat("Exception is occured while processing a group Id = {0}, for userId = {1}: {2}", exportCommand != null ? exportCommand.VkGroupId : 0, exportCommand != null ? exportCommand.InitiatorUserId : 0, exc.ToString());

                        if (exportCommand != null)
                        {
                            this.SendExportFinished(exportCommand, isSuccess: false);
                        }
                    }
                    finally
                    {
                        if (exportCommand != null)
                        {
                            exportCommand.MarkAsCompleted();
                        }
                    }
                }
            }
        }

        public ExportResultCommand GetExportResult(string ticketId)
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

        private void SendExportFinished(ExportReportCommand exportCommand, bool isSuccess, string virtualFilePath = "")
        {
            using (ICommandSender commandSender = Factory.GetInstance<ICommandSender>().Open(CONST_ExportResultQueueName))
            {
                ExportResultCommand command = new ExportResultCommand
                {
                    VkGroupId = exportCommand.VkGroupId,
                    DateRange = exportCommand.DateRange,
                    InitiatorUserId = exportCommand.InitiatorUserId,
                    TicketId = exportCommand.TicketId,
                    IsSuccess = isSuccess,
                    FilePath = virtualFilePath
                };

                commandSender.SendCommand(command);
            }
        }
        private string GenerateFileName(ExportReportCommand exportCommand)
        {
            VkGroup vkGroup = this.vkGroupRepository.GetGroupById(exportCommand.VkGroupId);

            return !exportCommand.DateRange.IsSpecified 
                ? string.Format("export_of_{0}.xslx", vkGroup.Name) 
                : string.Format("export_of_{0}_from_{1}_to_{2}.xlsx", vkGroup.Name, exportCommand.DateRange.From.ToString(CONST_DateTimeFormat), exportCommand.DateRange.To.ToString(CONST_DateTimeFormat));
        }
    }
}