namespace Ix.Palantir.Engine.Bootstrapper
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Ix.Framework.ObjectFactory;

    public class ObjectFactoryControllerFactory : DefaultControllerFactory
    {
        public ObjectFactoryControllerFactory()
        {
            Factory.SetResolver(new EngineResolver());
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