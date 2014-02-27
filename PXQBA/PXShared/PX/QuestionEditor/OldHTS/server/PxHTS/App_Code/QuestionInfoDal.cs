using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using IntelliClass.Utils;

namespace BFW
{

    [DataObject]
    /// <summary>
    /// This object represents the properties and methods of a QuestionInfo.
    /// </summary>
    public static class QuestionInfoDal
    {
        // uses a DataReader and our DB layer to return a list of data
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public static QuestionInfoList GetQuestionInfoList()
        {
            PageUtils.DebugTrace("DBRun", "Start Setup Params");
            using (Database db = new Database())
            {
                QuestionInfoList retval = new QuestionInfoList();

                db.StoredProcedure = "QuestionGetInfoList";

                PageUtils.DebugTrace("DBRun", "Start Get Reader");
                SqlDataReader rdr = db.GetDataReader();
                PageUtils.DebugTrace("DBRun", "Start Create/Fill Object From Reader");
                retval = new QuestionInfoList(rdr);
                PageUtils.DebugTrace("DBRun", "End Create/Fill Object From Reader");
                rdr.Close();
                PageUtils.DebugTrace("DBRun", "End");
                if (db.KeepStatistics)
                {
                    System.Collections.IDictionaryEnumerator e = db.GetSqlStatistics();
                    while (e.MoveNext())
                        PageUtils.DebugTrace("DBStats", e.Key.ToString() + ":\t" + e.Value.ToString());
                }
                return retval;
            }
        }

        // uses a DataReader and our DB layer to return a list of data
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public static QuestionInfoList GetQuestionInfoList(int bookID)
        {
            PageUtils.DebugTrace("DBRun", "Start Setup Params");
            using (Database db = new Database())
            {
                QuestionInfoList retval = new QuestionInfoList();

                db.StoredProcedure = "QuestionGetInfoListByBook";
                db.AddParameter("@bookID", bookID, DbType.Int32);

                PageUtils.DebugTrace("DBRun", "Start Get Reader");
                SqlDataReader rdr = db.GetDataReader();
                PageUtils.DebugTrace("DBRun", "Start Create/Fill Object From Reader");
                retval = new QuestionInfoList(rdr);
                PageUtils.DebugTrace("DBRun", "End Create/Fill Object From Reader");
                rdr.Close();
                PageUtils.DebugTrace("DBRun", "End");
                if (db.KeepStatistics)
                {
                    System.Collections.IDictionaryEnumerator e = db.GetSqlStatistics();
                    while (e.MoveNext())
                        PageUtils.DebugTrace("DBStats", e.Key.ToString() + ":\t" + e.Value.ToString());
                }
                return retval;
            }
        }

        // uses a DataReader and our DB layer to return a record of data
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public static QuestionInfo GetQuestionInfo(int question_id)
        {
            PageUtils.DebugTrace("DBRun", "Start Setup Params");
            using (Database db = new Database())
            {
                QuestionInfo retval = null;

                db.StoredProcedure = "QuestionGetInfo";

                db.AddParameter("@question_id", question_id, DbType.Int32);

                PageUtils.DebugTrace("DBRun", "Start Get Reader");
                SqlDataReader rdr = db.GetDataReader();
                PageUtils.DebugTrace("DBRun", "Start Create/Fill Object From Reader");
                if (rdr.Read())
                {
                    retval = new QuestionInfo(rdr);
                }
                PageUtils.DebugTrace("DBRun", "End Create/Fill Object From Reader");
                rdr.Close();
                PageUtils.DebugTrace("DBRun", "End");
                if (db.KeepStatistics)
                {
                    System.Collections.IDictionaryEnumerator e = db.GetSqlStatistics();
                    while (e.MoveNext())
                        PageUtils.DebugTrace("DBStats", e.Key.ToString() + ":\t" + e.Value.ToString());
                }
                return retval;
            }
        }

        // uses a DataReader and our DB layer to return a record of data
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public static QuestionInfo GetQuestionInfoFromCode(int bookID, string question_code)
        {
            PageUtils.DebugTrace("DBRun", "Start Setup Params");
            using (Database db = new Database())
            {
                QuestionInfo retval = null;

                db.StoredProcedure = "QuestionGetInfoFromCode";

                db.AddParameter("@book_id", bookID, DbType.Int32);
                db.AddParameter("@question_code", question_code, DbType.String);

                PageUtils.DebugTrace("DBRun", "Start Get Reader");
                SqlDataReader rdr = db.GetDataReader();
                PageUtils.DebugTrace("DBRun", "Start Create/Fill Object From Reader");
                if (rdr.Read())
                {
                    retval = new QuestionInfo(rdr);
                }
                PageUtils.DebugTrace("DBRun", "End Create/Fill Object From Reader");
                rdr.Close();
                PageUtils.DebugTrace("DBRun", "End");
                if (db.KeepStatistics)
                {
                    System.Collections.IDictionaryEnumerator e = db.GetSqlStatistics();
                    while (e.MoveNext())
                        PageUtils.DebugTrace("DBStats", e.Key.ToString() + ":\t" + e.Value.ToString());
                }
                return retval;
            }
        }
    }
}