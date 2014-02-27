using System.Configuration;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.PX.PXPub.Components.Test.Helper
{
    public static class DlapConnectionHelper
    {
        /// <summary>
        /// Sets up a DlapConnection for admin access.
        /// </summary>
        /// <returns></returns>
        public static DlapConnection AdminConnection()
        {
            DlapConnection connection = null;

            string userId = null;
  
            userId = ConfigurationManager.AppSettings.Get("AdministratorUserId");        
            var config = ConfigurationManager.GetSection("agilixSessionManager") as Bfw.Agilix.Dlap.Configuration.SessionManagerSection;

            connection = ConnectionFactory.GetDlapConnection(config.Connection.Url);
            connection.TrustHeaderKey = config.Connection.SecretKey;
            connection.TrustHeaderUsername = userId;

            return connection;
        }

        public static void ExecuteDlapCommand(DlapCommand command)
        {
            var connection = AdminConnection();
            ExecuteDlapCommand(connection, command);
        }

        public static void ExecuteDlapCommand(DlapConnection connection, DlapCommand command)
        {
            DlapRequest request = null;
            DlapResponse response = null;

            request = command.ToRequest();

            response = connection.Send(request);
                    
            command.ParseResponse(response);

            if (response.Code != DlapResponseCode.OK && response.Code != DlapResponseCode.None)
            {
                throw new BadDlapResponseException(response.Message);
            }

        }
    }
}
