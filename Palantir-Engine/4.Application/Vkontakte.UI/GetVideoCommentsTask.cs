namespace Ix.Palantir.Vkontakte.UI
{
    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.Vkontakte.API.Access;

    public class GetVideoCommentsTask
    {
        public string Execute()
        {
            var dataProviderCreator = Factory.GetInstance<IVkConnectionBuilder>();
            var vkDataProvider = dataProviderCreator.GetVkDataProvider();
            var data = vkDataProvider.GetVideoComments("14721677", "163117554", 0);
            return data.Feed;
        }
    }
}