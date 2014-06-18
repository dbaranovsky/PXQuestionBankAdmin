using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Bfw.Common.Database;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Commands.Services.EntityFramework
{
    public class UserNotificationOperation : IUserNotificationOperation
    {
        private readonly IDatabaseManager databaseManager;
        private readonly IContext businessContext;

        public UserNotificationOperation(IDatabaseManager databaseManager, IContext businessContext)
        {

        #if DEBUG
            databaseManager = new DatabaseManager(@"TestPXData");
        #endif

            this.databaseManager = databaseManager;
            this.businessContext = businessContext;
        }

        public IEnumerable<UserNotShownNotification> GetNotShownNotification()
        {
            DbCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.GetUserNotShownNotifications";

            var userIdParam = new SqlParameter("@userId", businessContext.CurrentUser.Username);
            command.Parameters.Add(userIdParam);

            var dbRecords = databaseManager.Query(command);

            return GetNotificationsFromRecords(dbRecords);
        }

        private IEnumerable<UserNotShownNotification> GetNotificationsFromRecords(IEnumerable<DatabaseRecord> dbRecords)
        {
            return dbRecords.Select(databaseRecord => new UserNotShownNotification()
            {
                UserId = databaseRecord["UserId"].ToString(),
                NotificationType = (NotificationType)Enum.Parse(typeof(NotificationType), (string)databaseRecord["NotificationType"])
            }).ToList();

        }

        public void CreateNotShownNotification(NotificationType notificationType)
        {
            DbCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.CreateUserNotShownNotification";
           

            var userIdParam = new SqlParameter("@userId", businessContext.CurrentUser.Username);
            command.Parameters.Add(userIdParam);

            var text = new SqlParameter("@notificationType", notificationType);
            command.Parameters.Add(text);

            try
            {
                databaseManager.StartSession();
                databaseManager.ExecuteNonQuery(command);
            }
            finally 
            {
                databaseManager.EndSession();
            }
        }
    }
}