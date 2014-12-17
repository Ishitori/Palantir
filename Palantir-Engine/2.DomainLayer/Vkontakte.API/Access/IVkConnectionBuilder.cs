namespace Ix.Palantir.Vkontakte.API.Access
{
    public interface IVkConnectionBuilder
    {
        IVkDataProvider GetVkDataProvider();
        IVkCommandExecuter GetVkCommandExecuter();
    }
}