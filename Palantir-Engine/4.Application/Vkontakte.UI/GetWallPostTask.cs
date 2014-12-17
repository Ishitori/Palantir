namespace Ix.Palantir.Vkontakte.UI
{
    using DataAccess.API;
    using Framework.ObjectFactory;

    using Ix.Palantir.Vkontakte.API.Access;

    public class GetWallPostTask
    {
        public string Execute()
        {
            var dataProviderCreator = Factory.GetInstance<IVkConnectionBuilder>();
            var vkDataProvider = dataProviderCreator.GetVkDataProvider();
            var data = vkDataProvider.GetWallPosts("14721677", 0);
            return data.Feed;
        }
    }
}