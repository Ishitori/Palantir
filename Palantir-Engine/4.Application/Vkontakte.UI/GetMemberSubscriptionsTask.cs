namespace Ix.Palantir.Vkontakte.UI
{
    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.Vkontakte.API.Access;

    public class GetMemberSubscriptionsTask
    {
         public string Execute()
         {
             var dataProviderCreator = Factory.GetInstance<IVkConnectionBuilder>();
             var vkDataProvider = dataProviderCreator.GetVkDataProvider();
             var data = vkDataProvider.GetMemberSubscriptions("14448993", 0);
             return data.Feed;
         }
    }
}