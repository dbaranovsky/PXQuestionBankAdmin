using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Caching;
using Bfw.Common.Database;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Commands.Services.DLAP;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Services.Automapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Macmillan.PXQBA.Business.Services.Tests
{
    [TestClass]
    public class ProductCourseOperationTests
    {
        private IContext context;
        private IDatabaseManager databaseManager;
        private IProductCourseOperation productCourseOperation;
        private AutomapperConfigurator automapperConfigurator;
        private IModelProfileService modelProfileService;

        [TestInitialize]
        public void TestInitialize()
        {
            context = Substitute.For<IContext>();
            databaseManager = Substitute.For<IDatabaseManager>();
            modelProfileService = Substitute.For<IModelProfileService>();

            automapperConfigurator = new AutomapperConfigurator(new ModelProfile(modelProfileService));
            automapperConfigurator.Configure(); 
            

            productCourseOperation = new ProductCourseOperation(databaseManager, context);
        }

        static void ExecuteAsAdminFillOneCourse(GetCourse c)
        {
            c.Courses = new List<Course>()
                          {
                              new Course() {}
                          };
        }

        [TestMethod]
        public void GetProductCourse_ReturnCourse()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<DlapCommand>(x => ExecuteAsAdminFillOneCourse(((GetCourse)x))));

            var course = productCourseOperation.GetProductCourse("123");

            Assert.IsNotNull(course);
        }
    }
}
