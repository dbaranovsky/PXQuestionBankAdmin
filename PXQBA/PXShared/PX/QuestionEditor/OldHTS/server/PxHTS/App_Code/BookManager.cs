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

namespace BFW
{
    /// <summary>
    /// Summary description for BookManager
    /// </summary>
    public static class BookManager
    {
        /// <summary>
        /// Updates book
        /// </summary>
        /// <param name="q"></param>
        public static void UpdateBook(BookInfo b)
        {
            using (Database db = new Database())
            {
                db.StoredProcedure = "BookUpdate";
                db.AddParameter("@bookID", b.BookID, DbType.Int32);
                db.AddParameter("@title", b.Title, DbType.String);
                db.AddParameter("@bookCode", b.BookCode, DbType.String);
                db.AddParameter("@status", b.Status, DbType.Byte);

                db.ExecuteSql();
            }
        }

        /// <summary>
        /// Create new book
        /// </summary>
        /// <param name="questionCode"></param>
        /// <param name="fileName"></param>
        /// <param name="bookID"></param>
        /// <returns>true if sucessfully created book, false if book code already exists</returns>
        public static bool CreateBook(string title, string bookCode, int status)
        {
            using (Database db = new Database())
            {
                db.StoredProcedure = "BookCreate";
                db.AddParameter("@title", title, DbType.String);
                db.AddParameter("@bookCode", bookCode, DbType.String);
                db.AddParameter("@status", status, DbType.Byte);

                db.AddReturnParameter();

                db.ExecuteSql();
                if (db.GetReturnParameter() == 0) // new book
                    return true;
                else // existing book
                    return false;
            }
        }
    }
}