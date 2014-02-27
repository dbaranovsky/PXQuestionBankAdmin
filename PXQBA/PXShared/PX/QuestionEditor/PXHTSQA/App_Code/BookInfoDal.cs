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
    /// This object represents the properties and methods of a BookInfo.
    /// </summary>
    public static class BookInfoDal
    {

        // uses a DataReader and our DB layer to return a list of data
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public static BookInfoList GetBookInfoList()
        {
            PageUtils.DebugTrace("DBRun", "Start Setup Params");
            using (Database db = new Database())
            {
                BookInfoList retval = null;

                db.StoredProcedure = "BookGetInfoList";

                PageUtils.DebugTrace("DBRun", "Start Get Reader");
                SqlDataReader rdr = db.GetDataReader();
                PageUtils.DebugTrace("DBRun", "Start Create/Fill Object From Reader");
                retval = new BookInfoList(rdr);
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
        public static BookInfo GetBookInfo(int book_id)
        {
            PageUtils.DebugTrace("DBRun", "Start Setup Params");
            using (Database db = new Database())
            {
                BookInfo retval = null;

                db.StoredProcedure = "BookGetInfo";
                db.AddParameter("@book_id", book_id, DbType.Int32);

                PageUtils.DebugTrace("DBRun", "Start Get Reader");
                SqlDataReader rdr = db.GetDataReader();
                PageUtils.DebugTrace("DBRun", "Start Create/Fill Object From Reader");
                if (rdr.Read())
                {
                    retval = new BookInfo(rdr);
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
        public static BookInfo GetBookInfoFromCode(string book_code)
        {
            PageUtils.DebugTrace("DBRun", "Start Setup Params");
            using (Database db = new Database())
            {
                BookInfo retval = null;

                db.StoredProcedure = "BookGetInfoFromCode";
                db.AddParameter("@book_code", book_code, DbType.String);


                PageUtils.DebugTrace("DBRun", "Start Get Reader");
                SqlDataReader rdr = db.GetDataReader();
                PageUtils.DebugTrace("DBRun", "Start Create/Fill Object From Reader");
                if (rdr.Read())
                {
                    retval = new BookInfo(rdr);
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