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
    public class LogSearchService : ILogSearchService
    {
        public List<LogModel> GetLogs(string sSeverity, string sCategoryName, string sSource, string sMessage, string dtStartDate, string dtEndDate)
        {            
            string connstring = ConfigurationManager.ConnectionStrings["Logging"].ConnectionString;
            IDbConnection objConnection = new SqlConnection(connstring);
            int connectionTimeOut = objConnection.ConnectionTimeout;
            objConnection.Open();

            string sql = "EXEC SP_PXAP_GetLogs @Severity, @CategoryName, @Source, @Message, @StartDate, @EndDate";

            //Dapper is not automatically picking up the connection time-out from the SQLConnection
            List<LogModel> lstLog = objConnection.Query<LogModel>(sql, new { Severity = sSeverity, CategoryName = sCategoryName, Source = sSource, Message = sMessage, StartDate = dtStartDate, EndDate = dtEndDate }, null, true, connectionTimeOut, null).ToList();

            return lstLog;
        }

        public LogSearchModel GetLogSearchModel(PXEnvironment currentEnv)
        {
            LogSearchModel logSearchModel = new LogSearchModel();
            List<SelectListItem> sevOptions = new List<SelectListItem>();
            sevOptions.Add(new SelectListItem() { Selected = true, Text = "All", Value = "All" });
            sevOptions.Add(new SelectListItem() { Selected = false, Text = "Error", Value = "Error" });
            sevOptions.Add(new SelectListItem() { Selected = false, Text = "Warning", Value = "Warning" });
            sevOptions.Add(new SelectListItem() { Selected = false, Text = "Information", Value = "Information" });
            sevOptions.Add(new SelectListItem() { Selected = false, Text = "Debug", Value = "Debug" });
            logSearchModel.SeverityOptions = sevOptions;

            List<SelectListItem> catOptions = new List<SelectListItem>();
            catOptions.Add(new SelectListItem() { Selected = true, Text = "All", Value = "All" });
            catOptions.Add(new SelectListItem() { Selected = false, Text = "Application Status", Value = "Application Status" });
            catOptions.Add(new SelectListItem() { Selected = false, Text = "Debug", Value = "Debug" });
            catOptions.Add(new SelectListItem() { Selected = false, Text = "Error", Value = "Error" });
            catOptions.Add(new SelectListItem() { Selected = false, Text = "Information", Value = "Information" });
            catOptions.Add(new SelectListItem() { Selected = false, Text = "Unhandled Exception", Value = "Unhandled Exception" });
            catOptions.Add(new SelectListItem() { Selected = false, Text = "Warning", Value = "Warning" });
            logSearchModel.CategoryOptions = catOptions;

            if (currentEnv != null && currentEnv.Sources != null && currentEnv.Sources.Count > 0 )
            {
                List<SelectListItem> sourceOptions = new List<SelectListItem>();
                sourceOptions.Add(new SelectListItem() { Selected = true, Text = "All", Value = "All" });
                foreach (var s in currentEnv.Sources)
                {
                    sourceOptions.Add(new SelectListItem() { Selected = false, Text = s, Value = s });
                }
                logSearchModel.SourceOptions = sourceOptions;
                //currentEnv.Sources;
            }

            return logSearchModel;
        }

        public string GetErrorMessage(int logID)
        {
            string connstring = ConfigurationManager.ConnectionStrings["Logging"].ConnectionString;
            IDbConnection objConnection = new SqlConnection(connstring);
            int connectionTimeOut = objConnection.ConnectionTimeout;
            objConnection.Open();

            string sErrorMessage = string.Empty;

            string sql = "EXEC SP_PXAP_SelectMessage @LogID";

            List<string> messages = objConnection.Query<string>(sql, new { LogId = logID }, null, true, connectionTimeOut, null).ToList();
            
            if(messages.Count > 0)
                sErrorMessage = messages[0];

            return sErrorMessage;
        }

    }
}
