namespace Ix.Palantir.Security.API
{
    using System.Security.Principal;

    public interface IPrincipalBuilder
    {
        IPrincipal CreatePrincipal(string identity);
    }
}