using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using System.Xml.Serialization;
using System.Xml.Linq;

using Bfw.PXAP.ServiceContracts;
using Bfw.PXAP.Models;
using Dapper;

namespace Bfw.PXAP.Services
{
    public class ProgressService :IProgressService
    {
        
        /// <summary>
        /// To return the process progress information from the database for the supplied process Id.
        /// </summary>
        /// <param name="processId">big int id of the long running process</param>
        /// <returns></returns>
        public ProgressModel GetProgress(Int64 processId)
        {
            string connString = ConfigurationManager.ConnectionStrings["PXAP"].ConnectionString;
            IDbConnection objConnection = new SqlConnection(connString);
            int connectionTimeOut = objConnection.ConnectionTimeout;
            objConnection.Open();

            ProgressModel process = new ProgressModel();

            string sql = "EXEC sp_GetProgressStatus @ProcessId";

            List<ProgressModel> processes = objConnection.Query<ProgressModel>(sql, new { ProcessId = processId }, null, true, connectionTimeOut, null).ToList();
            if (processes.Count > 0)
            {
                process = processes[0];
            }

            return process;
        }

        /// <summary>
        /// Add /Update the Process's progress in the database
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public Int64 AddUpdateProcess(ProgressModel process, out string Message)
        {
            string connString = ConfigurationManager.ConnectionStrings["PXAP"].ConnectionString;
            IDbConnection objConnection = new SqlConnection(connString);
            int connectionTimeOut = objConnection.ConnectionTimeout;
            objConnection.Open();

            Int64 iProcess = 0;
            Message = string.Empty;
            

            string sql = "EXEC sp_AddUpdateProgressStatus @ProcessId, @Percentage";

            List<ProgressModel> processes = objConnection.Query<ProgressModel>(sql, new { ProcessId = process.ProcessId, Percentage = process.Percentage }, null, true, connectionTimeOut, null).ToList();
            if (processes.Count > 0)
            {
                iProcess = processes[0].ProcessId;
                Message = processes[0].Status;
            }

            return iProcess;
        }
    }
}
