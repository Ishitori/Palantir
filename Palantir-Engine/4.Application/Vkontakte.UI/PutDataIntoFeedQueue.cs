namespace Ix.Palantir.Vkontakte.UI
{
    using System.IO;
    using DataAccess.API;
    using DomainModel;
    using Framework.ObjectFactory;
    using Ix.Palantir.Localization.API;
    using Queueing.API.Command;

    public class PutDataIntoFeedQueue
    {
        private readonly IUnitOfWorkProvider unitOfWorkProvider;
        private readonly IDateTimeHelper dateTimeHelper;

        public PutDataIntoFeedQueue(IUnitOfWorkProvider unitOfWorkProvider, IDateTimeHelper dateTimeHelper)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
            this.dateTimeHelper = dateTimeHelper;
        }

        public void Execute()
         {
             using (this.unitOfWorkProvider.CreateUnitOfWork())
             {
                 DataFeed feed = this.CreateDataFeed();

                 using (ICommandSender commandSender = Factory.GetInstance<ICommandSender>().Open("FeedDataQueue"))
                 {
                     commandSender.SendCommand(feed);
                 }
             }
         }

        private DataFeed CreateDataFeed()
        {
            string data;

            using (StreamReader streamReader = File.OpenText(@"C:\1.xml"))
            {
                data = streamReader.ReadToEnd();
            }

            DataFeed dataFeed = new DataFeed()
                                    {
                                        ReceivedAt = this.dateTimeHelper.GetDateTimeNow(),
                                        Feed = data,
                                        VkGroupId = 14721677,
                                        Type = DataFeedType.WallPosts
                                    };

            return dataFeed;
        }
    }
}