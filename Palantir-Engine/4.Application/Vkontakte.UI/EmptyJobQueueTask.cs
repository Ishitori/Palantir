namespace Ix.Palantir.Vkontakte.UI
{
    using DomainModel;
    using Framework.ObjectFactory;
    using Queueing.API.Command;

    public class EmptyJobQueueTask
    {
        public void Execute()
        {
            using (ICommandReceiver commandReceiver = Factory.GetInstance<ICommandReceiver>().Open("FeedJobQueue"))
            {
                while (true)
                {
                    var feedQueueItem = commandReceiver.GetCommand<FeedQueueItem>();

                    if (feedQueueItem == null)
                    {
                        break;
                    }

                    feedQueueItem.MarkAsCompleted();
                }
            }
        }
    }
}