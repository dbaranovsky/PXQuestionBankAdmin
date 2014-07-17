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

namespace Macmillan.PXQBA.Business.Commands.Services.SQLOperations
{
    public class ParsedFileOperation : IParsedFileOperation
    {
        private readonly IDatabaseManager databaseManager;

        public ParsedFileOperation(IDatabaseManager databaseManager)
        {

#if DEBUG
            databaseManager = new DatabaseManager(@"TestPXData");
#endif

            this.databaseManager = databaseManager;
        }

        public int AddParsedFile(string fileName, string questionsData)
        {
            DbCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.AddQBAParsedFile";


            var fileNameParam = new SqlParameter("@fileName", fileName);
            command.Parameters.Add(fileNameParam);
            var questionsDataParam = new SqlParameter("@questionsData", questionsData);
            command.Parameters.Add(questionsDataParam);

            try
            {
                var result = databaseManager.ExecuteScalar(command);
                return int.Parse(result.ToString());
            }
            catch (Exception ex)
            {
                StaticLogger.LogError(string.Format("ParsedFileOperation.AddParsedFile: fileName: {0}, parsedQuestions:{1} ", fileName, questionsData), ex);
                throw;
            }
        }

        public int SetParsedFileStatus(int id, ParsedFileStatus status)
        {
            DbCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.SetQBAParsedFile";


            var idParam = new SqlParameter("@id", id);
            command.Parameters.Add(idParam);
            var statusParam = new SqlParameter("@status", (int)status);
            command.Parameters.Add(statusParam);

            try
            {
                var result = databaseManager.ExecuteScalar(command);
                return int.Parse(result.ToString());
            }
            catch (Exception ex)
            {
                StaticLogger.LogError(string.Format("ParsedFileOperation.SetParsedFileStatus: id: {0} sn't updated", id), ex);
                throw;
            }
        }

        public string GetParsedFile(int id)
        {
            DbCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.GetQBAParsedFile";

            var idParam = new SqlParameter("@id", id);
            command.Parameters.Add(idParam);

            var dbRecords = databaseManager.Query(command).ToList();

            return "";// GetRoleCapabilitiesFromRecords(dbRecords);
        }

        //private Role GetRoleCapabilitiesFromRecords(IEnumerable<DatabaseRecord> dbRecords)
        //{
        //    var role = new Role();
        //    var firstRecord = dbRecords.FirstOrDefault();
        //    if (firstRecord != null)
        //    {
        //        role.Id = (int)firstRecord["Id"];
        //        role.Name = firstRecord["Name"].ToString();
        //    }
        //    foreach (var databaseRecord in dbRecords)
        //    {
        //        if (!string.IsNullOrEmpty(databaseRecord["CapabilityId"].ToString()))
        //        {
        //            role.Capabilities.Add(EnumHelper.Parse<Capability>(databaseRecord["CapabilityId"].ToString()));
        //        }
        //    }
        //    return role;
        //}
    }
}