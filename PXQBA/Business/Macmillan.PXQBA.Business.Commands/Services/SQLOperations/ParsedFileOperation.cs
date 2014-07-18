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

        public long AddParsedFile(string fileName, string questionsData)
        {
            DbCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.AddQBAParsedFile";


            var fileNameParam = new SqlParameter("@fileName", fileName);
            command.Parameters.Add(fileNameParam);
            var questionsDataParam = new SqlParameter("@questionsData", questionsData);
            command.Parameters.Add(questionsDataParam);
            var statusParam = new SqlParameter("@status", ParsedFileStatus.Validated);
            command.Parameters.Add(statusParam);

            try
            {
                var result = databaseManager.ExecuteScalar(command);
                return long.Parse(result.ToString());
            }
            catch (Exception ex)
            {
                StaticLogger.LogError(string.Format("ParsedFileOperation.AddParsedFile: fileName: {0}, parsedQuestions:{1} ", fileName, questionsData), ex);
                throw;
            }
        }

        public long SetParsedFileStatus(long id, ParsedFileStatus status)
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
                return long.Parse(result.ToString());
            }
            catch (Exception ex)
            {
                StaticLogger.LogError(string.Format("ParsedFileOperation.SetParsedFileStatus: id: {0} sn't updated", id), ex);
                throw;
            }
        }

        public ParsedFile GetParsedFile(long id)
        {
            DbCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.GetQBAParsedFile";

            var idParam = new SqlParameter("@id", id);
            command.Parameters.Add(idParam);

            var dbRecords = databaseManager.Query(command).ToList();

            return GetParsedFileFromRecords(dbRecords);
        }

        private ParsedFile GetParsedFileFromRecords(IEnumerable<DatabaseRecord> dbRecords)
        {
            var parsedFile = new ParsedFile();
            var firstRecord = dbRecords.FirstOrDefault();
            if (firstRecord != null)
            {
                parsedFile.Id = (long)firstRecord["Id"];
                parsedFile.FileName = firstRecord["FileName"].ToString();
                parsedFile.QuestionsData = firstRecord["QuestionsData"].ToString();
            }
            return parsedFile;
        }
    }
}