using System.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Unity;
using Bfw.Common.Patterns.Unity;

namespace PXWebAPI
{
	/// <summary>
	/// Use Bootstrapper to Initialize Unity for WebApi 
	/// </summary>
	public static class Bootstrapper
	{
		/// <summary>
		/// Initialise
		/// </summary>
        public static void Initialise()
        {
            var container = BuildBfwCommonPatternsUnityContainer();
            //var container = BuildUnityContainer();

            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

        }

        private static IUnityContainer BuildBfwCommonPatternsUnityContainer()
        {

            var unityServiceLocator = new Bfw.Common.Patterns.Unity.UnityServiceLocator();

            unityServiceLocator.Container.AddNewExtensionIfNotPresent<EnterpriseLibraryCoreExtension>();
            unityServiceLocator.Container.AddNewExtensionIfNotPresent<LoggingBlockExtension>();

            var container = unityServiceLocator.Container;

            var unityConfig = ConfigurationManager.GetSection("unity") as UnityConfigurationSection;
            unityConfig.Configure(container, "unity");


            ServiceLocator.SetLocatorProvider(() => unityServiceLocator);
            return container;
        }


		private static IUnityContainer BuildUnityContainer()
		{

			var unity = new UnityContainer();
			var unityConfig = ConfigurationManager.GetSection("unity") as UnityConfigurationSection;

			unityConfig.Configure(unity, "unity");

			// register all your components with the container here
			// e.g. container.RegisterType<ITestService, TestService>();            

			return unity;
		}
	}
}