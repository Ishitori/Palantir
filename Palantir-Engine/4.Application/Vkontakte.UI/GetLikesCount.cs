namespace Ix.Palantir.Vkontakte.UI
{
    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Vkontakte.API.Access;

    public class GetLikesCount
    {
        public string Execute()
        {
            var dataProviderCreator = Factory.GetInstance<IVkConnectionBuilder>();
            var vkDataProvider = dataProviderCreator.GetVkDataProvider();
            var data = vkDataProvider.GetLikes("14721677", "163117554", LikeShareType.Video, 0);
            return data.Feed;
        }
    }
}