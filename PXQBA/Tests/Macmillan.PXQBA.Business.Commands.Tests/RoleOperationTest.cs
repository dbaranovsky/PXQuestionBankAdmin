using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bfw.Common.Database;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Commands.Services.SQLOperations;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Macmillan.PXQBA.Business.Commands.Tests
{
    [TestClass]
    public class RoleOperationTest
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
            businessContext.CurrentUser = new UserInfo()
                                          {
                                              Username = "234818"
                                          };

            roleOperation = new RoleOperation(databaseManager, userOperation, productCourseOperation, businessContext);
        }


        [TestMethod]
        public void GetQBAUsers_RecordsCount3_Return3Records()
        {
            int totalRecordsCount = 10;
            int recordsCount = 3;
            databaseManager.Query(Arg.Is<DbCommand>(c => c.CommandText == "dbo.GetQBAUsers"))
                           .Returns(GetQBAUsersRecords(totalRecordsCount));

            userOperation.GetUsers(null).ReturnsForAnyArgs(new List<UserInfo>()
                                                                         {
                                                                             new UserInfo()
                                                                             {
                                                                                 FirstName = "Test",
                                                                                 LastName = "Test",
                                                                                 Username = "1"
                                                                             }
                                                                         });

            var users = roleOperation.GetQBAUsers(0, recordsCount);
            var usersArray = users.CollectionPage.ToArray();

            Assert.IsTrue(users.TotalItems == totalRecordsCount);
            Assert.IsTrue(users.CollectionPage.Count() == recordsCount);
            Assert.IsTrue(usersArray[0].Id == "0");
            Assert.IsTrue(usersArray[1].Id == "1");
            Assert.IsTrue(usersArray[2].Id == "2");
            Assert.IsTrue(usersArray[1].FullName== "Test Test");
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
              userOperation.GetUsers(null).ReturnsForAnyArgs(new List<UserInfo>()
                                                                         {
                                                                             new UserInfo()
                                                                             {
                                                                                 FirstName = "Test",
                                                                                 LastName = "Test",
                                                                                 Username = "1"
                                                                             }
                                                                         });
            productCourseOperation.GetCoursesByCourseIds(null).ReturnsForAnyArgs(new List<Course>()
                                                                                 {
                                                                                     new Course()
                                                                                   {
                                                                                       Title = "Test",
                                                                                       ProductCourseId = (index + 1000).ToString()
                                                                                   }
                                                                                 });

            var user = roleOperation.GetUserWithRoles(userId);
          

            Assert.IsTrue(user.Id==userId);
            Assert.IsTrue(user.ProductCourses[0].Id == (index + 1000).ToString());
            Assert.IsTrue(user.ProductCourses[0].AvailableRoles.Count == 1);
            Assert.IsTrue(user.ProductCourses[0].AvailableRoles[0].Id == index + 10);
            Assert.IsTrue(user.ProductCourses[0].AvailableRoles[0].Name == "Role" + index);
            Assert.IsTrue(user.ProductCourses[0].CurrentRole.Name == "Role" + index);
            Assert.IsTrue(user.FullName == "Test Test");
            Assert.IsTrue(user.ProductCourses[0].Name == "Test");
        }
 [TestMethod]
        public void GetUserWithRoles_UserIdNoCourses_FilledQBAUser()
        {
            string userId = "123";
            int index = 1;
            databaseManager.Query(Arg.Is<DbCommand>(c => c.CommandText == "dbo.GetQBAUserCoursesWithRoles"))
                         .Returns(new List<DatabaseRecord>() { GetQBAUserCoursesWithRolesecord(index) });
              userOperation.GetUsers(null).ReturnsForAnyArgs(new List<UserInfo>()
                                                                         {
                                                                             new UserInfo()
                                                                             {
                                                                                 FirstName = "Test",
                                                                                 LastName = "Test",
                                                                                 Username = "1"
                                                                             }
                                                                         });
            productCourseOperation.GetCoursesByCourseIds(null).ReturnsForAnyArgs(new List<Course>()
                                                                                 {
                                                                                     new Course()
                                                                                   {
                                                                                       Title = "Test",
                                                                                       ProductCourseId = (index + 1000).ToString()
                                                                                   }
                                                                                 });

            var user = roleOperation.GetUserWithRoles(userId);
          

            Assert.IsTrue(user.Id==userId);
            Assert.IsTrue(user.ProductCourses[0].Id == (index + 1000).ToString());
            Assert.IsTrue(user.ProductCourses[0].AvailableRoles.Count == 1);
            Assert.IsTrue(user.ProductCourses[0].AvailableRoles[0].Id == index + 10);
            Assert.IsTrue(user.ProductCourses[0].AvailableRoles[0].Name == "Role" + index);
            Assert.IsTrue(user.ProductCourses[0].CurrentRole.Name == "Role" + index);
            Assert.IsTrue(user.FullName == "Test Test");
            Assert.IsTrue(user.ProductCourses[0].Name == "Test");
        }


        [TestMethod]
        public void GetUserWithRoles_UserId_UserWithSettedCourseAndId()
        {
            string userId = "123";
            DatabaseRecord dataRecord1 = new DatabaseRecord();

            dataRecord1["CourseId"] = "123";
            dataRecord1["RoleId"] = 1;
            dataRecord1["RoleName"] = "Role";
            dataRecord1["UserId"] = "123";


            databaseManager.Query(Arg.Is<DbCommand>(c => c.CommandText == "dbo.GetQBAUserCoursesWithRoles"))
                         .Returns(new List<DatabaseRecord>() { dataRecord1, dataRecord1 });

          

            var user = roleOperation.GetUserWithRoles(userId);
            Assert.IsTrue(user.Id == "123");
            Assert.IsTrue(user.ProductCourses.Count() == 1);

        }

        [TestMethod]
        public void GetRoleWithCapabilities_RoleId_SqlInvokedWithRoleId()
        {
            const int roleId = 42;
            bool invokedWithRoleId = false;
            
            databaseManager.When(
                dm =>
                    dm.Query(Arg.Is<DbCommand>(c => c.CommandText == "dbo.GetQBARoleCapabilities" &&
                                                        c.Parameters[0].ParameterName == "@roleId" &&
                                                        (int)c.Parameters[0].Value == roleId)))
                        .Do(d=> { invokedWithRoleId = true; });

            roleOperation.GetRoleWithCapabilities(roleId);

            Assert.IsTrue(invokedWithRoleId);
        }


        [TestMethod]
        public void GetRoleWithCapabilities_RoleId_ReturnCorrectRole()
        {
            const int roleId = 42;

            databaseManager.Query(Arg.Is<DbCommand>(c => c.CommandText == "dbo.GetQBARoleCapabilities" &&
                                                         c.Parameters[0].ParameterName == "@roleId" &&
                                                         (int) c.Parameters[0].Value == roleId))
                           .Returns(GetQBARoleCapabilitiesRecords());
                        

            var role = roleOperation.GetRoleWithCapabilities(roleId);

            Assert.IsTrue(role.Id==1);
            Assert.IsTrue(role.Name == "RoleName1");
            Assert.IsTrue(role.Capabilities.Count == 2);
            Assert.IsTrue(role.Capabilities.ToArray()[0] == Capability.CreateDraftFromOldVersion);
            Assert.IsTrue(role.Capabilities.ToArray()[1] == Capability.ChangeDraftStatus);
        }

        [TestMethod]
        public void UpdateUserRoles_QBAUser_SqlInvokedWithUserId()
        {
            const string userId = "42";
            bool invokedWithUserId = false;

            databaseManager.When(
                dm =>
                    dm.ExecuteNonQuery(Arg.Is<DbCommand>(c => c.CommandText == "dbo.UpdateQBAUserRoles" &&
                                                        c.Parameters[0].ParameterName == "@userId" &&
                                                        c.Parameters[0].Value.ToString() == userId)))
                        .Do(d => { invokedWithUserId = true; });
            var user = new QBAUser()
                       {
                           Id = userId,
                           ProductCourses = new List<UserProductCourse>()
                                            {
                                                new UserProductCourse()
                                                {
                                                    CurrentRole = new Role(){ Id = 12}
                                                }
                                            }
                       };

            roleOperation.UpdateUserRoles(user);

            Assert.IsTrue(invokedWithUserId);
        }


    
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void UpdateRolesCapabilities_QBAUserWithNewRole_NullReferenseExpception()
        {
            const int roleId = 123;
            const string courseId = "courseId";

            databaseManager.ExecuteScalar(Arg.Is<DbCommand>(c => c.CommandText == "dbo.UpdateQBARole")).Returns(roleId);


            Role role = new Role()
                        {
                            Id = -1
                        };

            roleOperation.UpdateRolesCapabilities(courseId, new List<Role>() { role });
        }

        [TestMethod]
        public void UpdateRolesCapabilities_QBAUserWithNewRole_SqlInvokedWithRoleId()
        {
 
            const string courseId = "courseId";

            bool invokedWithRoleId = false;

            databaseManager.ExecuteScalar(Arg.Is<DbCommand>(c => c.CommandText == "dbo.AddQBARole")).Returns("123");

            databaseManager.When(
                dm =>
                    dm.ExecuteNonQuery(Arg.Is<DbCommand>(c => c.CommandText == "dbo.UpdateQBARoleCapabilities" &&
                                                        c.Parameters[0].ParameterName == "@roleId" &&
                                                        (int)c.Parameters[0].Value == 123)))
                        .Do(d => { invokedWithRoleId = true; });

            var role = new Role()
            {
                Id = -1,
                Capabilities = new List<Capability>()
                               {
                                  Capability.ChangeStatusFromAvailableToDeleted
                               }
            };

            roleOperation.UpdateRolesCapabilities(courseId, new List<Role>() { role });

            Assert.IsTrue(invokedWithRoleId);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void UpdateRolesCapabilities_QBAUserWithoutRoleId_NullReferenceException()
        {
       
            Role role = new Role()
            {
                Id = 123
            };

            roleOperation.UpdateRolesCapabilities("courseId", new List<Role>() { role });
        }


        [TestMethod]
        public void UpdateRolesCapabilities_QBAUser_SqlInvokedWithRoleId()
        {
            const int roleId = 123;
            const string courseId = "courseId";

            bool invokedWithRoleId = false;

            databaseManager.ExecuteScalar(Arg.Is<DbCommand>(c => c.CommandText == "dbo.UpdateQBARole")).Returns(roleId);

            databaseManager.When(
                dm =>
                    dm.ExecuteNonQuery(Arg.Is<DbCommand>(c => c.CommandText == "dbo.UpdateQBARoleCapabilities" &&
                                                        c.Parameters[0].ParameterName == "@roleId" &&
                                                        (int)c.Parameters[0].Value == roleId)))
                        .Do(d => { invokedWithRoleId = true; });

            var role = new Role()
            {
                Id = 1,
                Capabilities = new List<Capability>()
                               {
                                  Capability.ChangeStatusFromAvailableToDeleted
                               }
            };

            roleOperation.UpdateRolesCapabilities(courseId, new List<Role>() { role });

            Assert.IsTrue(invokedWithRoleId);
        }

        
        [TestMethod]
        public void GetUserCapabilities_CourseId_ReturnCapabilities()
        {
            const string courseId = "courseId";
            const string userName = "123";

            businessContext.CurrentUser.Returns(new UserInfo() { Username = userName });

            databaseManager.Query(Arg.Is<DbCommand>(c => c.CommandText == "dbo.GetQBAUserCapabilities" &&
                                                                 c.Parameters[0].ParameterName == "@userId" && 
                                                                 c.Parameters[0].Value.ToString() == userName &&
                                                                 c.Parameters[1].ParameterName == "@courseId" &&
                                                                 c.Parameters[1].Value.ToString() == courseId
                                                                 )).Returns(GetQBARoleCapabilitiesRecords());

           var capabilities = roleOperation.GetUserCapabilities(courseId);
           
           Assert.IsTrue(capabilities.Count()==2);
           Assert.IsTrue(capabilities.ToArray()[0] == Capability.CreateDraftFromOldVersion);
           Assert.IsTrue(capabilities.ToArray()[1] == Capability.ChangeDraftStatus);
        }

      [TestMethod]
        public void DeleteRole_RoleToDelete_SuccessRun()
        {
            var counter = 0;
            databaseManager.ExecuteNonQuery(Arg.Do<DbCommand>(x =>
            {
                Assert.IsTrue(x.CommandText == "dbo.DeleteQBARole");
                Assert.IsTrue(x.Parameters["@roleId"].Value.ToString() == "1");
               
                counter++;
            }));
            roleOperation.DeleteRole(1);
           Assert.IsTrue(counter == 1);
        }

        [TestMethod]
        public void GrantPredefinedRoleToCurrentUser_PredefinedRoleAndUserId_ProperlySettedRole()
        {

  
            databaseManager.Query(Arg.Is<DbCommand>(c => c.CommandText == "dbo.GetQBAUserCoursesWithRoles"))
                         .Returns(new List<DatabaseRecord>(){ GetQBAUserCoursesWithSettedRoleName("23")});

            databaseManager.When(m => m.ExecuteNonQuery(Arg.Do<DbCommand>(x =>
            {
                Assert.IsTrue(x.CommandText == "dbo.UpdateQBAUserRoles");
                Assert.IsTrue(x.Parameters["@roleId"].Value.ToString() == "11");
            })));

            roleOperation.GrantPredefinedRoleToCurrentUser(PredefinedRole.Author, "321");
          

        }

      #region private methods

      private IEnumerable<DatabaseRecord> GetQBARoleCapabilitiesRecords()
        {
            List<DatabaseRecord> records = new List<DatabaseRecord>();

            DatabaseRecord firstRecord = new DatabaseRecord();
            firstRecord["Id"] = 1;
            firstRecord["Name"] = "RoleName1";
            firstRecord["CapabilityId"] = Capability.CreateDraftFromOldVersion;
            records.Add(firstRecord);

            DatabaseRecord secondRecord = new DatabaseRecord();
            secondRecord["Id"] = null;
            secondRecord["Name"] = null;
            secondRecord["CapabilityId"] = Capability.ChangeDraftStatus;
            records.Add(secondRecord);
            return records;
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


        private DatabaseRecord GetQBAUserCoursesWithSettedRoleName(string roleName)
        {
            DatabaseRecord record = new DatabaseRecord();

            record["CourseId"] = "321";
            record["RoleId"] = 11;
            record["RoleName"] = roleName;
            record["UserId"] = "234818";

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

            if (index == 4)
            {
                record["Id"] = null;
            }
            else
            {
                record["Id"] = index;
            }
          
            record["Count"] = index * 2;

            return record;
        }

      #endregion

    }
}
