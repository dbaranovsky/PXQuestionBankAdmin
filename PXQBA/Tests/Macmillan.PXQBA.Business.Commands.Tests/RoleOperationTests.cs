using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bfw.Common.Database;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Commands.Services.SQLOperations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Macmillan.PXQBA.Business.Commands.Tests
{
    [TestClass]
    public class RoleOperationTests
    {
        private IDatabaseManager databaseManager;
        private IUserOperation userOperation;
        private IProductCourseOperation productCourseOperation;
        private IContext businessContext;

        private IRoleOperation roleOperation;


        [TestInitialize]
        public void TestInitialize()
        {
            databaseManager = Substitute.For<IDatabaseManager>();
            userOperation = Substitute.For<IUserOperation>();
            productCourseOperation = Substitute.For<IProductCourseOperation>();
            businessContext = Substitute.For<IContext>();

            roleOperation = new RoleOperation(databaseManager, userOperation, productCourseOperation, businessContext);
        }


        [TestMethod]
        public void GetQBAUsers_RecordsCount3_Return3Records()
        {
            int totalRecordsCount = 10;
            int recordsCount = 3;
            databaseManager.Query(Arg.Is<DbCommand>(c => c.CommandText == "dbo.GetQBAUsers"))
                           .Returns(GetQBAUsersRecords(totalRecordsCount));

            var users = roleOperation.GetQBAUsers(0, recordsCount);
            var usersArray = users.CollectionPage.ToArray();

            Assert.IsTrue(users.TotalItems == totalRecordsCount);
            Assert.IsTrue(users.CollectionPage.Count() == recordsCount);
            Assert.IsTrue(usersArray[0].Id == "0");
            Assert.IsTrue(usersArray[1].Id == "1");
            Assert.IsTrue(usersArray[2].Id == "2");
        }


        [TestMethod]
        public void GetQBAUsers_Starting5RecordsCount5_Return5RecordsSkipped5()
        {
            int totalRecordsCount = 12;
            int recordsCount = 5;
            int skipped = 5;
            databaseManager.Query(Arg.Is<DbCommand>(c => c.CommandText == "dbo.GetQBAUsers"))
                           .Returns(GetQBAUsersRecords(totalRecordsCount));

            var users = roleOperation.GetQBAUsers(skipped, recordsCount);
            var usersArray = users.CollectionPage.ToArray();

            Assert.IsTrue(users.TotalItems == totalRecordsCount);
            Assert.IsTrue(users.CollectionPage.Count() == recordsCount);
            Assert.IsTrue(usersArray[0].Id == "5");
            Assert.IsTrue(usersArray[1].Id == "6");
            Assert.IsTrue(usersArray[2].Id == "7");
            Assert.IsTrue(usersArray[3].Id == "8");
            Assert.IsTrue(usersArray[4].Id == "9");
        }


        [TestMethod]
        public void GetRolesForCourse_CourseId_ReturnAllRoles()
        {
            string cousreId = "123";
            int totalRecordsCount = 6;
            databaseManager.Query(Arg.Is<DbCommand>(c => c.CommandText == "dbo.GetQBARolesForCourse"))
                              .Returns(GetRoleRecords(totalRecordsCount));

            var roles = roleOperation.GetRolesForCourse(cousreId);
            var rolesArray = roles.ToArray();

            Assert.IsTrue(rolesArray.Length == totalRecordsCount);
            Assert.IsTrue(rolesArray[0].Id == 0);
            Assert.IsTrue(rolesArray[0].Name == "Name_0");
            Assert.IsTrue(rolesArray[5].Id == 5);
            Assert.IsTrue(rolesArray[5].Name == "Name_5");
        }

        [TestMethod]
        public void GetRolesForCourse_CourseId_CourseIdParameterReceivedToSql()
        {
            string cousreId = "123";
            bool received = false;

            databaseManager.When(d => d.Query(Arg.Is<DbCommand>(c => c.CommandText == "dbo.GetQBARolesForCourse"
                                                    && c.Parameters[0].ParameterName == "@courseId"
                                                    && c.Parameters[0].Value.ToString() == cousreId)))
                                    .Do(d => { received = true; });

            roleOperation.GetRolesForCourse(cousreId);

            Assert.IsTrue(received);
        }


        [TestMethod]
        public void GetUserWithRoles_UserId_FilledQBAUser()
        {
            string userId = "123";
            int index = 1;
            databaseManager.Query(Arg.Is<DbCommand>(c => c.CommandText == "dbo.GetQBAUserCoursesWithRoles"))
                         .Returns(new List<DatabaseRecord>() { GetQBAUserCoursesWithRolesecord(index) });


            var user = roleOperation.GetUserWithRoles(userId);

            Assert.IsTrue(user.Id==userId);
            Assert.IsTrue(user.ProductCourses[0].Id == (index + 1000).ToString());
            Assert.IsTrue(user.ProductCourses[0].AvailableRoles.Count == 1);
            Assert.IsTrue(user.ProductCourses[0].AvailableRoles[0].Id == index + 10);
            Assert.IsTrue(user.ProductCourses[0].AvailableRoles[0].Name == "Role" + index);
            Assert.IsTrue(user.ProductCourses[0].CurrentRole.Name == "Role" + index);
        }

        private DatabaseRecord GetQBAUserCoursesWithRolesecord(int index)
        {
            DatabaseRecord record = new DatabaseRecord();

            record["CourseId"] = index + 1000;
            record["RoleId"] = index + 10;
            record["RoleName"] = "Role" + index;
            record["UserId"] = "123";

            return record;
        }


        private IEnumerable<DatabaseRecord> GetRoleRecords(int count)
        {
            List<DatabaseRecord> records = new List<DatabaseRecord>();

            for (int i = 0; i < count; i++)
            {
                records.Add(GetRoleRecord(i));
            }

            return records;
        }


        private DatabaseRecord GetRoleRecord(int index)
        {
            DatabaseRecord record = new DatabaseRecord();
            record["Id"] = index;
            record["Name"] = "Name_" + index;
            record["CanEdit"] = true;
            record["Count"] = index + 1;

            return record;
        }


        private IEnumerable<DatabaseRecord> GetQBAUsersRecords(int count)
        {
            List<DatabaseRecord> records = new List<DatabaseRecord>();

            for (int i = 0; i < count; i++)
            {
                records.Add(GetQBAUsersRecord(i));
            }

            return records;
        }

        private DatabaseRecord GetQBAUsersRecord(int index)
        {
            DatabaseRecord record = new DatabaseRecord();
            record["Id"] = index;
            record["Count"] = index * 2;

            return record;
        }


    }
}
