using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Bfw.Common.Database;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Common.Logging;

namespace Macmillan.PXQBA.Business.Commands.Services.SQLOperations
{
    public class UserCapabilityOperation : IUserCapabilityOperation
    {
        private readonly IDatabaseManager databaseManager;
        private readonly IUserOperation userOperation;
        private readonly IProductCourseOperation productCourseOperation;

        private const string userFullNameFormat = "{0} {1}";

        public UserCapabilityOperation(IDatabaseManager databaseManager, IUserOperation userOperation, IProductCourseOperation productCourseOperation)
        {

#if DEBUG
            databaseManager = new DatabaseManager(@"TestPXData");
#endif

            this.databaseManager = databaseManager;
            this.userOperation = userOperation;
            this.productCourseOperation = productCourseOperation;
        }

        public IEnumerable<Role> GetRolesForCourse(string courseId)
        {
            DbCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.GetQBARolesForCourse";

            var courseIdParam = new SqlParameter("@courseId", courseId);
            command.Parameters.Add(courseIdParam);

            var dbRecords = databaseManager.Query(command);

            return GetRolesFromRecords(dbRecords);
        }

        public IEnumerable<QBAUser> GetQBAUsers(int startingRecordNumber, int recordsCount)
        {
            DbCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.GetQBAUsers";

            var dbRecords = databaseManager.Query(command);

            return FillUserNames(GetUsersFromRecords(dbRecords.Skip(startingRecordNumber).Take(recordsCount)));
        }

        private IEnumerable<QBAUser> FillUserNames(IEnumerable<QBAUser> qbaUsers)
        {
            var usersInfo = userOperation.GetUsers(qbaUsers.Select(u => u.Id));
            foreach (var qbaUser in qbaUsers)
            {
                var userInfo = usersInfo.FirstOrDefault(u => u.Username == qbaUser.Id);
                if (userInfo != null)
                {
                    qbaUser.FullName = string.Format(userFullNameFormat, userInfo.FirstName, userInfo.LastName);
                }
                else
                {
                    qbaUser.FullName = "(Unknown)";
                }
            }
            return qbaUsers;
        }

        public void UpdateRolesCapabilities(string courseId, IEnumerable<Role> roles)
        {
            foreach (var role in roles)
            {
                try
                {
                    databaseManager.StartSession();
                    var roleId = InsertOrUpdateRole(courseId, role);
                    DbCommand command = new SqlCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "dbo.UpdateQBARoleCapabilities";

                    var roleIdParam = new SqlParameter("@roleId", roleId);
                    command.Parameters.Add(roleIdParam);
                    var capIdParam = new SqlParameter("@capabilityIds", CreateDataTable(role.Capabilities.Select(src => (int)src)));
                    capIdParam.SqlDbType = SqlDbType.Structured;
                    command.Parameters.Add(capIdParam);
                    databaseManager.ExecuteNonQuery(command);
                }
                finally
                {
                    databaseManager.EndSession();
                }
            }
        }

        /// <summary>
        /// RA user id should be passed here
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public QBAUser GetUserWithRoles(string userId)
        {
            DbCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.GetQBAUserCoursesWithRoles";

            var userIdParam = new SqlParameter("@userId", userId);
            command.Parameters.Add(userIdParam);

            var dbRecords = databaseManager.Query(command);
            var userInfo = userOperation.GetUsers(new List<string> {userId}).FirstOrDefault();
            
            var user = FillUserFromRecords(dbRecords);
            user.Id = userId;
            if (userInfo != null)
            {
                user.FullName = string.Format(userFullNameFormat, userInfo.FirstName, userInfo.LastName);
            }
            return user;
        }

        public void UpdateUserRoles(QBAUser user)
        {
            try
            {
                databaseManager.StartSession();
                DbCommand command = new SqlCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "dbo.UpdateQBAUserRoles";

                var userIdParam = new SqlParameter("@userId", user.Id);
                command.Parameters.Add(userIdParam);
                var capIdParam = new SqlParameter("@roleIds", CreateDataTable(user.ProductCourses.Where(c => c.CurrentRole != null).Select(c => c.CurrentRole.Id)));
                capIdParam.SqlDbType = SqlDbType.Structured;
                command.Parameters.Add(capIdParam);
                databaseManager.ExecuteNonQuery(command);
            }
            finally
            {
                databaseManager.EndSession();
            }
        }

        private QBAUser FillUserFromRecords(IEnumerable<DatabaseRecord> dbRecords)
        {
            var user = new QBAUser();
            foreach (var databaseRecord in dbRecords)
            {
                if (!string.IsNullOrEmpty(databaseRecord["CourseId"].ToString()))
                {
                    var courseId = databaseRecord["CourseId"].ToString();
                    var userCourse = user.ProductCourses.FirstOrDefault(c => c.Id == courseId);
                    if (userCourse == null)
                    {
                        userCourse = new UserProductCourse(){Id = courseId};
                        user.ProductCourses.Add(userCourse);
                    }
                    if(!string.IsNullOrEmpty(databaseRecord["RoleId"].ToString()))
                    {
                        var roleId = (int)databaseRecord["RoleId"];
                        var courseRole = userCourse.AvailableRoles.FirstOrDefault(r => r.Id == roleId);
                        if (courseRole == null)
                        {
                            courseRole = new Role(){Id = roleId};
                            userCourse.AvailableRoles.Add(courseRole);
                        }
                        if (!string.IsNullOrEmpty(databaseRecord["RoleName"].ToString()))
                        {
                            courseRole.Name = databaseRecord["RoleName"].ToString();
                        }
                        if (!string.IsNullOrEmpty(databaseRecord["UserId"].ToString()))
                        {
                            userCourse.CurrentRole = courseRole;
                        }
                    }
                }
            }
            
            UpdateCourseNames(user);
            return user;
        }

        private void UpdateCourseNames(QBAUser user)
        {
            var courses = productCourseOperation.GetCoursesByCourseIds(user.ProductCourses.Select(c => c.Id));
            foreach (var course in user.ProductCourses)
            {
                var match = courses.FirstOrDefault(c => c.ProductCourseId == course.Id);
                if (match != null)
                {
                    course.Name = match.Title;
                }
            }
        }

        public void DeleteRole(int roleId)
        {
            DbCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.DeleteQBARole";

            var roleIdParam = new SqlParameter("@roleId", roleId);
            command.Parameters.Add(roleIdParam);

            databaseManager.ExecuteNonQuery(command);
        }

        public Role GetRoleWithCapabilities(int roleId)
        {
            DbCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.GetQBARoleCapabilities";

            var roleIdParam = new SqlParameter("@roleId", roleId);
            command.Parameters.Add(roleIdParam);

            var dbRecords = databaseManager.Query(command).ToList();

            return GetRoleCapabilitiesFromRecords(dbRecords);
        }

        private Role GetRoleCapabilitiesFromRecords(IEnumerable<DatabaseRecord> dbRecords)
        {
            var role = new Role();
            var firstRecord = dbRecords.FirstOrDefault();
            if (firstRecord != null)
            {
                role.Id = (int) firstRecord["Id"];
                role.Name = firstRecord["Name"].ToString();
            }
            foreach (var databaseRecord in dbRecords)
            {
                if (!string.IsNullOrEmpty(databaseRecord["CapabilityId"].ToString()))
                {
                    role.Capabilities.Add(EnumHelper.Parse<Capability>(databaseRecord["CapabilityId"].ToString()));
                }
            }
            return role;
        }

        private static DataTable CreateDataTable(IEnumerable<int> ids)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            foreach (int id in ids)
            {
                table.Rows.Add(id);
            }
            return table;
        }

        private int InsertOrUpdateRole(string courseId, Role role)
        {
            if (role.Id <= 0)
            {
                return AddRole(courseId, role);
            }
            return UpdateRole(role.Id, role.Name);
        }

        private int AddRole(string courseId, Role role)
        {
            var command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.AddQBARole";
            var roleNameParam = new SqlParameter("@roleName", role.Name);
            command.Parameters.Add(roleNameParam);
            var courseIdParam = new SqlParameter("@courseId", courseId);
            command.Parameters.Add(courseIdParam);
            var canEditParam = new SqlParameter("@canEdit", role.CanEdit);
            command.Parameters.Add(canEditParam);
            try
            {
                var result = databaseManager.ExecuteScalar(command);
                return int.Parse(result.ToString());
            }
            catch (Exception ex)
            {
                StaticLogger.LogError(string.Format("AddRole: rolename: {0}, courseId:{1} wasn't created", role.Name, courseId), ex);
                throw;
            }
        }

        private int UpdateRole(int roleId, string roleName)
        {
            var command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.UpdateQBARole";
            var roleIdParam = new SqlParameter("@roleId", roleId);
            command.Parameters.Add(roleIdParam);
            var roleNameParam = new SqlParameter("@roleName", roleName);
            command.Parameters.Add(roleNameParam);
            try
            {
                var result = databaseManager.ExecuteScalar(command);
                return int.Parse(result.ToString());
            }
            catch (Exception ex)
            {
                StaticLogger.LogError(string.Format("UpdateRole: roleId: {0} wasn't updated", roleId), ex);
                throw;
            }
        }

        private IEnumerable<QBAUser> GetUsersFromRecords(IEnumerable<DatabaseRecord> dbRecords)
        {
            return dbRecords.Select(GetUserFromRecord).ToList();
        }

        private QBAUser GetUserFromRecord(DatabaseRecord databaseRecord)
        {
            var user = new QBAUser();
            if (databaseRecord["Id"] != null)
            {
                user.Id = databaseRecord["Id"].ToString();
            }
            if (databaseRecord["Count"] != null)
            {
                user.ProductCoursesCount = (int)databaseRecord["Count"];
            }
            return user;
        }

        private IEnumerable<Role> GetRolesFromRecords(IEnumerable<DatabaseRecord> dbRecords)
        {
            return dbRecords.Select(GetRoleFromRecord).ToList();
        }

        private Role GetRoleFromRecord(DatabaseRecord databaseRecord)
        {
            var role = new Role();
            if (databaseRecord["Id"] != null)
            {
                role.Id = (int) databaseRecord["Id"];
            }
            if (databaseRecord["Name"] != null)
            {
                role.Name = databaseRecord["Name"].ToString();
            }
            if (databaseRecord["CanEdit"] != null)
            {
                role.CanEdit = bool.Parse(databaseRecord["CanEdit"].ToString());
            }
            if (databaseRecord["Count"] != null)
            {
                role.CapabilitiesCount = (int) databaseRecord["Count"];
            }
            return role;
        }
    }
}