using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace Bfw.Common.Patterns.Unity
{
    public class UnityDependencyResolver : IDependencyResolver
    {
        #region Constructors

        /// <summary>
        /// Handle to the container that stores the locator's type mappings
        /// </summary>
        public IUnityContainer Container { get; protected set; }

        /// <summary>
        /// Configures itself with the default unity container from the config file.
        /// By convention the configuration section name must be "unity".
        /// </summary>
        public UnityDependencyResolver()
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
        public UnityDependencyResolver(IUnityContainer container)
        {
            Container = container;
        }

        #endregion

        #region IDependencyResolver Members

        public object GetService(Type serviceType)
        {
            object instance;

            try
            {
                instance = Container.Resolve(serviceType);
            }
            catch
            {
                // we swallow the exception here, because the interface demands we return null
                instance = null;
            }

            return instance;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            IEnumerable<object> instances = null;

            try
            {
                instances = Container.ResolveAll(serviceType);
            }
            catch
            {
                // we swallow the exception here, because the interface demands we return null
                instances = new List<object>();
            }

            return instances;
        }

        #endregion
    }
}
