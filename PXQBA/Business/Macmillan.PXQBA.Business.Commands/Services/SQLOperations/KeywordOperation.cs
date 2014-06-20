using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Bfw.Common.Database;
using Macmillan.PXQBA.Business.Commands.Contracts;

namespace Macmillan.PXQBA.Business.Commands.Services.SQLOperations
{
    public class KeywordOperation : IKeywordOperation
    {
        private readonly IDatabaseManager databaseManager;

        public KeywordOperation(IDatabaseManager databaseManager)
        {

#if DEBUG
            databaseManager = new DatabaseManager(@"TestPXData");
#endif

            this.databaseManager = databaseManager;
        }

        public IEnumerable<string> GetKeywordList(string courseId, string fieldName)
        {
            DbCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.GetKeywordList";

            var courseIdParam = new SqlParameter("@courseId", courseId);
            command.Parameters.Add(courseIdParam);
            var fieldNameParam = new SqlParameter("@fieldName", fieldName);
            command.Parameters.Add(fieldNameParam);

            var dbRecords = databaseManager.Query(command);

            return GetKeywordsFromRecords(dbRecords);
        }

        private IEnumerable<string> GetKeywordsFromRecords(IEnumerable<DatabaseRecord> dbRecords)
        {
            return dbRecords.Select(databaseRecord => (string) databaseRecord["Keyword"]).ToList();
        }

        public void AddKeywords(string courseId, string fieldName, IEnumerable<string> keywords)
        {
            DbCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.AddKeyword";
           

            var courseIdParam = new SqlParameter("@courseId", courseId);
            command.Parameters.Add(courseIdParam);
            var fieldNameParam = new SqlParameter("@fieldName", fieldName);
            command.Parameters.Add(fieldNameParam);
           
            try
            {
                databaseManager.StartSession();
                foreach (var keyword in keywords)
                {
                    SqlParameter keywordParam;
                    if (!command.Parameters.Contains("@keyword"))
                    {
                        keywordParam = new SqlParameter("@keyword", keyword);
                        command.Parameters.Add(keywordParam);
                    }
                    keywordParam = (SqlParameter)command.Parameters["@keyword"];
                    keywordParam.Value = keyword;
                    databaseManager.ExecuteNonQuery(command);
                }
            }
            finally 
            {
                databaseManager.EndSession();
            }
        }
    }
}