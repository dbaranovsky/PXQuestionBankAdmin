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

        [TestInitialize]
        public void TestInitialize()
        {
            context = Substitute.For<IContext>();
            databaseManager = Substitute.For<IDatabaseManager>();
            var courses = new List<Course>()
                          {
                              new Course() {}
                          };
            //businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            context.SessionManager.CurrentSession.When(c => c.ExecuteAsAdmin(Arg.Do<DlapCommand>(x => ((GetCourse)x).Courses = courses)));

            productCourseOperation = new ProductCourseOperation(databaseManager, context);

        }

        [TestMethod]
        public void GetProductCourse()
        {
            productCourseOperation.GetProductCourse("123");
        }
    }
}
