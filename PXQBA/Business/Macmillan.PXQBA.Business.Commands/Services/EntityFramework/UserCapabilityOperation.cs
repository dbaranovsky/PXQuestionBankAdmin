using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Bfw.Common.Database;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Models;
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
                    var roleId = role.Id <= 0 ? AddRole(role.Name, role.CanDelete) : role.Id;
                    DbCommand command = new SqlCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "dbo.UpdateQBARoleCapabilities";

                    var courseIdParam = new SqlParameter("@courseId", courseId);
                    command.Parameters.Add(courseIdParam);
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

        private int AddRole(string roleName, bool canDelete)
        {
            var addRoleCommand = new SqlCommand();
            addRoleCommand.CommandType = CommandType.StoredProcedure;
            addRoleCommand.CommandText = "dbo.AddQBARole";
            var roleNameParam = new SqlParameter("@roleName", roleName);
            addRoleCommand.Parameters.Add(roleNameParam);
            var canDeleteParam = new SqlParameter("@canDelete", canDelete);
            addRoleCommand.Parameters.Add(canDeleteParam);
            try
            {
                var result = databaseManager.ExecuteScalar(addRoleCommand);
                return int.Parse(result.ToString());
            }
            catch (Exception ex)
            {
                StaticLogger.LogError(string.Format("AddRole: rolename: {0} wasn't created", roleName), ex);
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
                role.Id = (long) databaseRecord["Id"];
            }
            if (databaseRecord["Name"] != null)
            {
                role.Name = databaseRecord["Name"].ToString();
            }
            if (databaseRecord["Count"] != null)
            {
                role.CapabilitiesCount = (int) databaseRecord["Count"];
            }
            return role;
        }
    }
}