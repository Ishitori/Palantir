namespace Ix.Palantir.Vkontakte.UI
{
    using System;
    using System.IO;
    using Framework.ObjectFactory;
    using Ix.Palantir.Engine.Bootstrapper;

    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            Factory.SetResolver(new EngineResolver());

            /*var task = Factory.GetInstance<SavePlacesTask>();
            task.Execute();*/

            EmptyJobQueueTask task = Factory.GetInstance<EmptyJobQueueTask>();
            task.Execute();

            FillJobQueueTask task2 = Factory.GetInstance<FillJobQueueTask>();
            task2.Execute();

            /*GetMemberSubscriptionsTask task = new GetMemberSubscriptionsTask();
            string feed = task.Execute();

            using (StreamWriter streamWriter = File.CreateText(@"d:\member-subscriptions.xml"))
            {
                streamWriter.WriteLine(feed);
            }*/

            /*FixDateTimeTask task = new FixDateTimeTask();
            task.Execute();*/

            /*var couchbaseTask = new CouchbaseTask();
            couchbaseTask.Execute();*/

            /*var task = new GetLikesCount();
            string feed = task.Execute();

            using (StreamWriter streamWriter = File.CreateText(@"c:\likes.txt"))
            {
                streamWriter.WriteLine(feed);
            }*/

            Console.WriteLine("Execution succeed");
        }
    }
}
