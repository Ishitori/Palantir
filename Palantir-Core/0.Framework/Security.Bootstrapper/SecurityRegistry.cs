namespace Ix.Palantir.Security.Bootstrapper
{
    using System;
    using API;
    using Framework.ObjectFactory;
    using ObjectFactory.StructureMapExtension;

    public class SecurityRegistry : IRegistry
    {
        public void InstantiateIn(IObjectResolver objectResolver)
        {
            if (objectResolver is StructureMapObjectResolver)
            {
                this.InstantiateInStructureMap();
            }
            else
            {
                throw new ArgumentException("Unsupported type of object resolver is provided", "objectResolver");
            }
        }
        private void InstantiateInStructureMap()
        {
            StructureMap.ObjectFactory.Configure(x =>
            {
                x.For<IUserRepository>().Use<UserRepository>();
                x.For<IAccountRepository>().Use<AccountRepository>();
                x.For<ICurrentUserProvider>().Use<CurrentUserProvider>();
                x.For<IPrincipalBuilder>().Use<PrincipalBuilder>();
            });
        }
    }
}