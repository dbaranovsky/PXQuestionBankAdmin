using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using AutoMapper;
using Bfw.Agilix.Dlap.Components.Session;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Caching;
using Bfw.Common.Database;
using Bfw.Common.Logging;
using Bfw.Common.Patterns.Caching;
using Bfw.Common.Patterns.Logging;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.Biz.ServiceContracts;
using Macmillan.PXQBA.Business;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Commands.Services.DLAP;
using Macmillan.PXQBA.Business.Commands.Services.SQLOperations;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Services;
using Macmillan.PXQBA.Business.Services.Automapper;
using Macmillan.PXQBA.Common.Logging;
using Microsoft.Practices.Unity;
using System;
using System.Configuration;

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
            //container.RegisterTypes((AllClasses.FromAssemblies(typeof(QuestionFilterManagementService).Assembly)), WithMappings.FromAllInterfaces, WithName.Default, WithLifetime.Custom<PerRequestLifetimeManager>);
            container.RegisterTypes(typeof(QuestionManagementService).Assembly.GetTypes().Where(t => t.Name.EndsWith("Service")), WithMappings.FromAllInterfaces, WithName.Default, WithLifetime.Custom<PerRequestLifetimeManager>);
            container.RegisterType<Profile, ModelProfile>(new PerRequestLifetimeManager());
            container.RegisterType<ISessionManager, WebSessionManager>(new PerRequestLifetimeManager());
            container.RegisterType<ITraceManager, MvcMiniProfilerTraceManager>(new PerRequestLifetimeManager());
            container.RegisterType<ICacheProvider, AppFabricCacheProvider>(new PerRequestLifetimeManager());
            container.RegisterType<IDataCacheFactoryProvider, HttpApplicationDataCacheFactoryProvider>(new PerRequestLifetimeManager());
            container.RegisterType<ILogger, CommonLogger>(new PerRequestLifetimeManager());
            container.RegisterType<IRAServices, RAServices>(new PerRequestLifetimeManager());
            container.RegisterTypes(AllClasses.FromAssemblies(typeof(Context).Assembly), WithMappings.FromAllInterfaces, WithName.Default, WithLifetime.Custom<PerRequestLifetimeManager>);
            container.RegisterTypes(AllClasses.FromAssemblies(typeof(QuestionCommands).Assembly), WithMappings.FromAllInterfaces, WithName.Default);

            var cs = ConfigurationManager.ConnectionStrings["QBADummyModelContainer"];

            container.RegisterType<IDatabaseManager, DatabaseManager>(new InjectionConstructor("PXData"));

#if DEBUG
                 container.RegisterType<INoteCommands, NoteCommands>(new InjectionConstructor(new DatabaseManager("TestPXData")));
                 container.RegisterType<IUserNotificationOperation, UserNotificationOperation>(
                     new InjectionConstructor(new DatabaseManager("TestPXData"), typeof(IContext)));
                 container.RegisterType<IRoleOperation, RoleOperation>(
                     new InjectionConstructor(new DatabaseManager("TestPXData"),
                         typeof(IUserOperation),
                         typeof(IProductCourseOperation),
                         typeof(IContext)
                     ));
                 container.RegisterType<IKeywordOperation, KeywordOperation>(new InjectionConstructor(new DatabaseManager("TestPXData")));
                 container.RegisterType<IParsedFileOperation, ParsedFileOperation>(new InjectionConstructor(new DatabaseManager("TestPXData")));
                 container.RegisterType<IProductCourseOperation, ProductCourseOperation>(new InjectionConstructor(new DatabaseManager("TestPXData"), typeof(IContext)));
#endif
        }
    }
}
