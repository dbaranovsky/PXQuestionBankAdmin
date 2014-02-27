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
    /// This object represents the properties and methods of a ResultInfo.
    /// </summary>
    public static class ResultInfoDal
    {

        // uses a DataReader and our DB layer to return a record of data
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public static ResultInfo GetResultInfo(int result_id)
        {
            PageUtils.DebugTrace("DBRun", "Start Setup Params");
            using (Database db = new Database())
            {
                ResultInfo retval = null;

                db.StoredProcedure = "ResultGetInfo";

                db.AddParameter("@result_id", result_id, DbType.Int32);


                PageUtils.DebugTrace("DBRun", "Start Get Reader");
                SqlDataReader rdr = db.GetDataReader();
                PageUtils.DebugTrace("DBRun", "Start Create/Fill Object From Reader");
                if (rdr.Read())
                    retval = new ResultInfo(rdr);
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
        public static ResultInfo GetResultInfoFromResponse(string responseCode, string qid)
        {
            PageUtils.DebugTrace("DBRun", "Start Setup Params");
            using (Database db = new Database())
            {
                ResultInfo retval = null;

                db.StoredProcedure = "ResultGetInfoFromResponse";

                db.AddParameter("@response_id", responseCode, DbType.String);
                db.AddParameter("@qid", qid, DbType.String);

                PageUtils.DebugTrace("DBRun", "Start Get Reader");
                SqlDataReader rdr = db.GetDataReader();
                PageUtils.DebugTrace("DBRun", "Start Create/Fill Object From Reader");
                if (rdr.Read())
                    retval = new ResultInfo(rdr);
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