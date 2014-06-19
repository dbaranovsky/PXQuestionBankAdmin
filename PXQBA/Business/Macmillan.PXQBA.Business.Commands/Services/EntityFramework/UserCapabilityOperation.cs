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

namespace Macmillan.PXQBA.Business.Commands.Services.EntityFramework
{
    public class UserCapabilityOperation : IUserCapabilityOperation
    {
        private readonly IDatabaseManager databaseManager;

        public UserCapabilityOperation(IDatabaseManager databaseManager)
        {

#if DEBUG
            databaseManager = new DatabaseManager(@"TestPXData");
#endif

            this.databaseManager = databaseManager;
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

        public IEnumerable<Role> GetUserRoles(string userId, string courseId)
        {
            DbCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.GetQBARolesForUser";

            var userIdParam = new SqlParameter("@userId", userId);
            command.Parameters.Add(userIdParam);
            var courseIdParam = new SqlParameter("@courseId", courseId);
            command.Parameters.Add(courseIdParam);

            var dbRecords = databaseManager.Query(command);

            return GetRolesFromRecords(dbRecords);
        }

        public IEnumerable<CourseUser> GetUsersInCourse(string courseId)
        {
            DbCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.GetUsersInCourse";

            var courseIdParam = new SqlParameter("@courseId", courseId);
            command.Parameters.Add(courseIdParam);

            var dbRecords = databaseManager.Query(command);

            return GetUsersFromRecords(dbRecords);
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
                    var capIdParam = new SqlParameter("@capabilityIds", CreateDataTable(role.Capabilities));
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

        private static DataTable CreateDataTable(IEnumerable<Capability> capabilities)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            foreach (int id in capabilities.Select(src => (int)src))
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

        private IEnumerable<CourseUser> GetUsersFromRecords(IEnumerable<DatabaseRecord> dbRecords)
        {
            return dbRecords.Select(GetUserFromRecord).ToList();
        }

        private CourseUser GetUserFromRecord(DatabaseRecord databaseRecord)
        {
            var user = new CourseUser();
            if (databaseRecord["Id"] != null)
            {
                user.Id = databaseRecord["Id"].ToString();
            }
            if (databaseRecord["Count"] != null)
            {
                user.RolesCount = (int)databaseRecord["Count"];
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