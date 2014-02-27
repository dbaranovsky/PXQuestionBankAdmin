using System;
using System.Collections.Generic;
using System.Configuration;

using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace Bfw.Common.Patterns.Unity
{
    /// <summary>
    /// Implements <see cref="Microsoft.Practices.ServiceLocation.ServiceLocatorImplBase"/> in order
    /// to provide a dependency injection hook backed by the Unity Framework
    /// </summary>
    public class UnityServiceLocator : ServiceLocatorImplBase
    {
        /// <summary>
        /// Handle to the container that stores the locator's type mappings
        /// </summary>
        public IUnityContainer Container { get; protected set; }

        /// <summary>
        /// Configures itself with the default unity container from the config file.
        /// By convention the configuration section name must be "unity".
        /// </summary>
        public UnityServiceLocator()
        {
            var unity = new UnityContainer();
            var unityConfig = ConfigurationManager.GetSection("unity") as UnityConfigurationSection;

            unityConfig.Configure(unity, "unity");

            Container = unity;
        }

        /// <summary>
        /// Constructor which sets the IUnityContainer
        /// </summary>
        /// <param name="container"></param>
        public UnityServiceLocator(IUnityContainer container)
        {
            Container = container;
        }

        #region ServiceLocatorImplBase

        /// <summary>
        ///  Return instances of all registered types requested.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return Container.ResolveAll(serviceType);
        }

        /// <summary>
        ///  Resolve an instance of the requested type with the given name from the container.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override object DoGetInstance(Type serviceType, string key)
        {

            return Container.Resolve(serviceType, key);
        }

        #endregion

    }
}
