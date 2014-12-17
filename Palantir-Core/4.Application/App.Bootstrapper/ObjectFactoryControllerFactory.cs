namespace Ix.Palantir.App.Bootstrapper
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Ix.Framework.ObjectFactory;

    public class ObjectFactoryControllerFactory : DefaultControllerFactory
    {
        public ObjectFactoryControllerFactory()
        {
            Factory.SetResolver(new AppResolver());
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                return null;
            }

            return (IController)Factory.GetInstance(controllerType);
        }
    }
}