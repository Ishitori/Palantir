namespace Ix.Palantir.Vkontakte.UI
{
    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.Vkontakte.API.Access;

    public class GetPhotoAlbumDetailsTask
    {
        public string Execute()
        {
            var dataProviderCreator = Factory.GetInstance<IVkConnectionBuilder>();
            var vkDataProvider = dataProviderCreator.GetVkDataProvider();
            var data = vkDataProvider.GetPhotoAlbumDetails("14721677", "157784793", 0);
            return data.Feed;
        }
    }
}