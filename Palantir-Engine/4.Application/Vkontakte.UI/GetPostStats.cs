namespace Ix.Palantir.Vkontakte.UI
{
    using System;
    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.Vkontakte.API.Access;

    public class GetPostStats
    {
        public string Execute()
        {
            var dataProviderCreator = Factory.GetInstance<IVkConnectionBuilder>();
            var vkDataProvider = dataProviderCreator.GetVkDataProvider();
            var data = vkDataProvider.GetPostStats("35453503", "20551", DateTime.Now.AddMonths(-3), DateTime.Now);
            return data;
        }
    }
}