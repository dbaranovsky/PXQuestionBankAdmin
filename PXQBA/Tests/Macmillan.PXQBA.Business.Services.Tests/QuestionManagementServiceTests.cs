
using System.Linq;
using AutoMapper;
using Bfw.Agilix.Dlap.Components.Session;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Database;
using Bfw.Common.Logging;
using Bfw.Common.Patterns.Logging;
using Macmillan.PXQBA.Business.Commands.Services.DLAP;
using Macmillan.PXQBA.Business.Commands.Services.EntityFramework;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Business.Services.Automapper;
using Macmillan.PXQBA.Common.Logging;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macmillan.PXQBA.Business.Services.Tests
{
    [TestClass]
    public class QuestionManagementServiceTests
    {
        //private IUnityContainer container;
        //private IQuestionManagementService questionManagementService;
        //private AutomapperConfigurator automapperConfigurator;

        [TestInitialize]
        public void TestInitialize()
        {
            //InitializeUnity();
            //automapperConfigurator = container.Resolve<AutomapperConfigurator>();
            //automapperConfigurator.Configure();
            //var businessContext = container.Resolve<IContext>();
            //businessContext.Initialize("6670123");
            //questionManagementService = container.Resolve<IQuestionManagementService>();
        }

        private void InitializeUnity()
        {

            //container = new UnityContainer();

            //container.RegisterTypes(typeof(QuestionManagementService).Assembly.GetTypes().Where(t => t.Name.EndsWith("Service")), WithMappings.FromAllInterfaces, WithName.Default);
            //container.RegisterType<Profile, ModelProfile>();
            //container.RegisterType<ISessionManager, ThreadSessionManager>();
            //container.RegisterType<ITraceManager, MvcMiniProfilerTraceManager>();
            //container.RegisterType<ILogger, CommonLogger>();
            //container.RegisterTypes(AllClasses.FromAssemblies(typeof(Context).Assembly), WithMappings.FromAllInterfaces, WithName.Default);
            //container.RegisterTypes(AllClasses.FromAssemblies(typeof(QuestionCommands).Assembly), WithMappings.FromAllInterfaces, WithName.Default);

            //string connectionString =
            //    @"metadata=res://*/QBADummyModel.csdl|res://*/QBADummyModel.ssdl|res://*/QBADummyModel.msl;provider=System.Data.SqlClient;provider connection string=""data source=localhost;initial catalog=QBADummyData;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework""";
            ////container.RegisterType<QBADummyModelContainer, QBADummyModelContainer>(new InjectionConstructor(connectionString));

            //container.RegisterType<IDatabaseManager, DatabaseManager>(new InjectionConstructor("PXData"));
        }


        [TestMethod]
        public void CopyQuestionTest()
        {
            //var course = new Course()
            //{
            //    ProductCourseId = "70295",
            //    QuestionRepositoryCourseId = "39768"
            //};
            //var question = questionManagementService.GetQuestion(course, "10582cc2-2d64-443e-bdc5-0ca3b1453e76");
            //Assert.IsNotNull(question);
        }
    }
}
