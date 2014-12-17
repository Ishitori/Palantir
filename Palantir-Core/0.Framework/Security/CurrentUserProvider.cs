namespace Ix.Palantir.Security
{
    using System.Security.Principal;
    using System.Threading;
    using System.Web;
    using API;

    public class CurrentUserProvider : ICurrentUserProvider
    {
        public IPrincipal GetCurrentUser()
        {
            var currentPrincipal = HttpContext.Current != null ? HttpContext.Current.User : null;

            if (currentPrincipal == null && Thread.CurrentPrincipal != null)
            {
                currentPrincipal = Thread.CurrentPrincipal;
            }

            return currentPrincipal;
        }

        public void SetCurrentUser(IPrincipal user)
        {
            Thread.CurrentPrincipal = user;

            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = user;
            }
        }

        public int GetIdOfCurrentUser()
        {
            var user = this.GetCurrentUser();
            return user.GetId();
        }

        public IAccount GetAccountOfCurrentUser()
        {
            var user = this.GetCurrentUser();
            return user.GetAccount();
        }
    }
}