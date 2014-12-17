namespace Ix.Palantir.Security
{
    using System.Security.Principal;
    using API;
    using Framework.ObjectFactory;

    public class PrincipalBuilder : IPrincipalBuilder
    {
        public IPrincipal CreatePrincipal(string identityName)
        {
            User user = Factory.GetInstance<IUserRepository>().GetUserByEmail(identityName);

            if (user == null)
            {
                return null;
            }

            var identity = new CustomIdentity(identityName);
            var principal = new CustomPrincipal(identity);

            return principal;
        }
    }
}