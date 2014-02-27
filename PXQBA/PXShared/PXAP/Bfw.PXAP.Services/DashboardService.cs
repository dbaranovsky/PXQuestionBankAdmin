using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web.Mvc;
using Bfw.PXAP.Models;
using Bfw.PXAP.ServiceContracts;

using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using Dapper;


namespace Bfw.PXAP.Services
{
    public class DashboardService : IDashboardService
    {
        public List<LogSummaryModel> GetLogSummary(string source)
        {
            string connstring = ConfigurationManager.ConnectionStrings["Logging"].ConnectionString;
            IDbConnection objConnection = new SqlConnection(connstring);
            int connectionTimeOut = objConnection.ConnectionTimeout;
            objConnection.Open();

            string sql = "EXEC SP_PXAP_GetLogSummary @Source";

            //Dapper is not automatically picking up the connection time-out from the SQLConnection
            var logSummary = objConnection.Query<LogSummaryModel>(sql, new { Source = source }, null, true, connectionTimeOut, null).ToList();

            return logSummary;

        }

        public bool ClearLogs(string severity)
        {
            string connstring = ConfigurationManager.ConnectionStrings["Logging"].ConnectionString;
            IDbConnection objConnection = new SqlConnection(connstring);
            int connectionTimeOut = objConnection.ConnectionTimeout;
            objConnection.Open();

            string sql = "EXEC dbo.[SP_PXAP_ClearLogs] @Severity";

            //Dapper is not automatically picking up the connection time-out from the SQLConnection
            objConnection.Execute(sql, new { Severity = severity }, null, 0, null);            

            return true;

        }
    }
}
