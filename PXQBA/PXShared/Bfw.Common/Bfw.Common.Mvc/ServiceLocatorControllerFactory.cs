using System;
using System.Web.Mvc;

using Microsoft.Practices.ServiceLocation;

namespace Bfw.Common.Mvc
{
    /// <summary>
    /// Provides an IControllerFactory that uses a ServiceLocator to provide dependency injection capability
    /// for routing requests to controllers in the ASP.NET MVC framework.
    /// </summary>
    public class ServiceLocatorControllerFactory : DefaultControllerFactory
    {
        /// <summary>
        /// Uses the current ServiceLocator to get an instance of the requested controller. If
        /// resolution of the controller fails through the ServiceLocator, then the DefaultControllerFactory
        /// is asked to resolve the controller instance.
        /// </summary>
        /// <param name="requestContext">request context in which the controller is being requested</param>
        /// <param name="controllerType">type of controller being requested</param>
        /// <returns>Instance of the requested controller type</returns>
        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            var controller = null as IController;

            try
            {
                controller = ServiceLocator.Current.GetInstance(controllerType) as IController;

                if (controller == null)
                    throw new Exception();
            }
            catch
            {
                controller = base.GetControllerInstance(requestContext, controllerType);
            }

            return controller;
        }
    }
}
