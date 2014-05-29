using System.Linq;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Common.Database;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Commands.Services.DLAP;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Services.Automapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;

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

        static void ExecuteAsAdminFillOneCourse(GetCourse getCourseCommand)
        {
            getCourseCommand.Courses = new List<Course>()
                          {
                              new Course()
                              {
                                  Id = getCourseCommand.SearchParameters.CourseId
                              }
                          };
        }

        static void ExecuteAsAdminFillGetItems(GetItems getItemsCommand)
        {
            getItemsCommand.Items = new List<Item>()
                                    {
                                        new Item()
                                        {
                                            Id = "1122",
                                        }
                                    };
        }

        [TestMethod]
        public void GetProductCourse_AnyCourseId_ReturnCourseIsNotNull()
        {
            const string courseId = "123";
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetCourse>(x => ExecuteAsAdminFillOneCourse((x))));

            var course = productCourseOperation.GetProductCourse(courseId);

            Assert.IsNotNull(course);
        }

        [TestMethod]
        public void GetProductCourse_SpecificCourseId_CorrenctInvokeGetCourseCommand()
        {
            const string courseId = "123";
            var invokeCount = 0;

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetCourse>(x => ExecuteAsAdminFillOneCourse((x))));
            context.SessionManager.CurrentSession.When(x => x.ExecuteAsAdmin(Arg.Is<GetCourse>(a => a.SearchParameters.CourseId == courseId)))
                                                  .Do(x => invokeCount++);

            productCourseOperation.GetProductCourse(courseId);

            Assert.IsTrue(invokeCount == 1);
        }

        [TestMethod]
        public void GetProductCourse_SpecificCourseIdAndRequiredQuestionBankRepositoryFlagOn_CorrenctInvokeGetItems()
        {
            const string courseId = "123";
            const string queryForGetItems = @"/bfw_meta/bfw_metadata@name=""AgilixDisciplineId""";

            var invokeCount = 0;

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetCourse>(x => ExecuteAsAdminFillOneCourse((x))));
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetItems>(x => ExecuteAsAdminFillGetItems((x))));
            context.SessionManager.CurrentSession.When(x => x.ExecuteAsAdmin(
                                        Arg.Is<Batch>(a => (
                                                             ((GetItems)a.Commands.ToList()[0]).SearchParameters.EntityId == courseId
                                                           )
                                                           &&
                                                           (
                                                            ((GetItems)a.Commands.ToList()[0]).SearchParameters.Query == queryForGetItems
                                                           )
                                                           ))).Do(x => invokeCount++);

            productCourseOperation.GetProductCourse(courseId, true);

            Assert.IsTrue(invokeCount == 1);
        }

        [TestMethod]
        public void GetProductCourse_AnyCourseIdAndRequiredQuestionBankRepositoryFlagOff_GetItemsNotInvoked()
        {
            const string courseId = "123";
 
            var invokeCount = 0;

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetCourse>(x => ExecuteAsAdminFillOneCourse((x))));
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetItems>(x => ExecuteAsAdminFillGetItems((x))));
            context.SessionManager.CurrentSession.When(x => x.ExecuteAsAdmin(
                                        Arg.Any<Batch>())).Do(x => invokeCount++);

            productCourseOperation.GetProductCourse(courseId, false);

            Assert.IsTrue(invokeCount == 0);
        }
    }
}
