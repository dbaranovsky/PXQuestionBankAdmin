
//using AutoMapper;
//using Bfw.Agilix.Dlap.Components.Session;
//using Bfw.Agilix.Dlap.Session;
//using Bfw.Common.Database;
//using Bfw.Common.Logging;
//using Bfw.Common.Patterns.Logging;
//using Macmillan.PXQBA.Business.Commands.Services.EntityFramework;
//using Macmillan.PXQBA.Business.Contracts;
//using Macmillan.PXQBA.Business.Services.Automapper;
//using Macmillan.PXQBA.Common.Logging;
//using Macmillan.PXQBA.DataAccess.Data;
//using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macmillan.PXQBA.Business.Services.Tests
{
    [TestClass]
    public class QuestionManagementServiceTests
    {
        //private IUnityContainer container;
        //private IQuestionManagementService questionManagementService;

        [TestInitialize]
        public void TestInitialize()
        {
            InitializeUnity();

            //questionManagementService = container.Resolve<IQuestionManagementService>();
        }

        private void InitializeUnity()
        {
            
            //container = new UnityContainer();

            //container.RegisterTypes(AllClasses.FromAssemblies(typeof(QuestionFilterManagementService).Assembly), WithMappings.FromAllInterfaces, WithName.Default);
            //container.RegisterType<Profile, ModelProfile>();
            //container.RegisterType<ISessionManager, WebSessionManager>();
            //container.RegisterType<ITraceManager, MvcMiniProfilerTraceManager>();
            //container.RegisterType<ILogger, CommonLogger>();
            //container.RegisterTypes(AllClasses.FromAssemblies(typeof(Context).Assembly), WithMappings.FromAllInterfaces, WithName.Default);
            //container.RegisterTypes(AllClasses.FromAssemblies(typeof(QuestionCommands).Assembly), WithMappings.FromAllInterfaces, WithName.Default);

            //container.RegisterTypes(AllClasses.FromAssemblies(typeof (QuestionManagementService).Assembly),
            //    WithMappings.FromAllInterfaces, WithName.Default);
            //container.RegisterType<IDatabaseManager, DatabaseManager>(new InjectionConstructor("PXData"));

            //string connectionString =
            //    @"metadata=res://*/QBADummyModel.csdl|res://*/QBADummyModel.ssdl|res://*/QBADummyModel.msl;provider=System.Data.SqlClient;provider connection string=""data source=localhost;initial catalog=QBADummyData;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework""";
            //container.RegisterType<QBADummyModelContainer, QBADummyModelContainer>(new InjectionConstructor(connectionString));

        }


        [TestMethod]
        public void CopyQuestionTest()
        {
           //var question = questionManagementService.GetQuestion("123");
        }
    }
}
