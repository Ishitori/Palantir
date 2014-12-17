namespace Ix.Palantir.Security.API
{
    using System.Security.Principal;

    public interface ICurrentUserProvider
    {
        IPrincipal GetCurrentUser();
        void SetCurrentUser(IPrincipal user);

        int GetIdOfCurrentUser();
        IAccount GetAccountOfCurrentUser();
    }
}