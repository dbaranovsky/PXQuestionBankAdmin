using AutoMapper;
using Bfw.Agilix.Dlap.Components.Session;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Logging;
using Bfw.Common.Patterns.Logging;
using Macmillan.PXQBA.Business;
using Macmillan.PXQBA.Business.Commands.Services.EntityFramework;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Services;
using Macmillan.PXQBA.Business.Services.Automapper;
using Macmillan.PXQBA.Common.Logging;
using Macmillan.PXQBA.DataAccess.Data;
using Microsoft.Practices.Unity;
using System;

namespace Macmillan.PXQBA.Web.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your types here
            // container.RegisterType<IProductRepository, ProductRepository>();
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();
            container.RegisterType<IQuestionFilterManagementService, QuestionFilterManagementService>(new PerRequestLifetimeManager());
            container.RegisterType<IQuestionManagementService, QuestionManagementService>(new PerRequestLifetimeManager());
            container.RegisterType<IQuestionMetadataService, QuestionMetadataService>(new PerRequestLifetimeManager());
            container.RegisterType<IModelProfileService, ModelProfileService>(new PerRequestLifetimeManager());
            container.RegisterType<Profile, ModelProfile>(new PerRequestLifetimeManager());
            container.RegisterType<QBADummyModelContainer, QBADummyModelContainer>(new PerRequestLifetimeManager());
            container.RegisterType<IQBAUow, QBAUow>();
            container.RegisterType<ISessionManager, WebSessionManager>(new PerRequestLifetimeManager());
            container.RegisterType<ITraceManager, MvcMiniProfilerTraceManager>(new PerRequestLifetimeManager());
            container.RegisterType<ILogger, CommonLogger>(new PerRequestLifetimeManager());
            container.RegisterTypes(AllClasses.FromAssemblies(typeof(Context).Assembly), WithMappings.FromAllInterfaces, WithName.Default, WithLifetime.Custom<PerRequestLifetimeManager>);
            container.RegisterTypes(AllClasses.FromAssemblies(typeof(QuestionCommands).Assembly), WithMappings.FromAllInterfaces, WithName.Default);
        }
    }
}
