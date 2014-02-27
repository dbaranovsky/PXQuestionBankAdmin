using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using IntelliClass.Utils;
using System.IO;

namespace BFW
{

    /// <summary>
    /// Summary description for QuestionManager
    /// </summary>
    public static class QuestionManager
    {
        /// <summary>
        /// Updates question - use this if we know that the question exists (otherwise just use CreateQuestion(...))
        /// </summary>
        /// <param name="q"></param>
        public static void UpdateQuestion(QuestionInfo q)
        {
            using (Database db = new Database())
            {
                db.StoredProcedure = "QuestionUpdate";
                db.AddParameter("@questionID", q.QuestionID, DbType.Int32);
                db.AddParameter("@questionCode", q.QuestionCode, DbType.String);
                db.AddParameter("@fileName", q.FileName, DbType.String);

                db.ExecuteSql();
            }
        }

        /// <summary>
        /// Create or Update a question
        /// </summary>
        /// <param name="questionCode"></param>
        /// <param name="fileName"></param>
        /// <param name="bookID"></param>
        /// <returns>true if this was a new question, false if this was an existing question</returns>
        public static bool CreateQuestion(string questionCode, string fileName, int bookID)
        {
            using (Database db = new Database())
            {
                db.StoredProcedure = "QuestionCreate";
                db.AddParameter("@questionCode", questionCode, DbType.String);
                db.AddParameter("@fileName", fileName, DbType.String);
                db.AddParameter("@bookID", bookID, DbType.Int32);

                db.AddReturnParameter();

                db.ExecuteSql();
                if (db.GetReturnParameter() == 0) // new question
                    return true;
                else // existing question
                    return false;
            }
        }

        /// <summary>
        /// Clear cached question data files. Used when encryption key is changed.
        /// </summary>
        public static void ClearCache()
        {
            DirectoryInfo qdata = new DirectoryInfo(ItemServer.QuestionDataPath);
            if (qdata.Exists)
            {
                FileInfo[] cacheFiles = qdata.GetFiles("*.xml.ser", SearchOption.AllDirectories);
                for (int i = 0; i < cacheFiles.Length; i++)
                {
                    IPUtil.DeleteFileNoException(cacheFiles[i].FullName);
                }
            }
        }
    }
}