namespace Ix.Palantir.Vkontakte.UI
{
    using System;
    using Caching;
    using DomainModel;
    using Framework.ObjectFactory;
    using Ix.Palantir.Localization.API;

    public class CouchbaseTask
    {
        public void Execute()
        {
            var cacheStorage = Factory.GetInstance<ICacheStorage>();
            var dateTimeHelper = Factory.GetInstance<IDateTimeHelper>();

            DateTime start = dateTimeHelper.GetDateTimeNow();

            /*for (int i = 0; i < 100000; i++)
            {
                Post post = new Post()
                {
                    VkId = i.ToString(),
                    VkGroupId = 1,
                    Text = string.Format("Мой {0}-ый пост в сложном объекте", i)
                };

                cacheStorage.PutItem(post.FullVkId, post);
            }*/

            DateTime end = dateTimeHelper.GetDateTimeNow();
            Console.WriteLine("Put 100000 elements into cache for about {0} ms", (end - start).TotalMilliseconds);

            IEntityIdBuilder idBuilder = Factory.GetInstance<IEntityIdBuilder>();
            start = dateTimeHelper.GetDateTimeNow();

            for (int i = 0; i < 100000; i++)
            {
                cacheStorage.GetItem<Post>(idBuilder.CreateEntityId("post", 1.ToString(), i.ToString()));
            }

            end = dateTimeHelper.GetDateTimeNow();
            Console.WriteLine("Get 100000 elements into cache for about {0} ms", (end - start).TotalMilliseconds);
        }
    }
}