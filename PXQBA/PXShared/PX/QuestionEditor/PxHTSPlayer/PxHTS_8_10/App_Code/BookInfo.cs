using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using IntelliClass.Utils;
using System.ComponentModel;
using System.Data.SqlClient;

namespace BFW
{

    /// <summary>
    /// This object represents the properties and methods of BookInfoList.
    /// </summary>
    [Serializable]
    public class BookInfoList : System.Collections.Generic.List<BookInfo>
    {

        public BookInfoList()
        {
        }

        public BookInfoList(BookInfo[] values)
        {
            AddRange(values);
        }

        public BookInfoList(List<BookInfo> values)
        {
            AddRange(values);
        }

        public void AddRange(BookInfo[] values)
        {
            for (int i = 0; i < values.Length; i++)
                Add(values[i]);
        }

        public BookInfoList(IDataReader reader)
        {
            PageUtils.DebugTrace("DBRun", "Fill Start");
            Fill(reader);
            PageUtils.DebugTrace("DBRun", "Fill End");
        }

        public void Fill(IDataReader reader)
        {
            while (reader.Read())
            {
                BookInfo ar1 = new BookInfo(reader);
                this.Add(ar1);
            }
        }
    }
    /// <summary>
    /// Summary description for BookInfo
    /// </summary>
    [Serializable]
    public partial class BookInfo
    {
        #region Fields and Properties
        #region - From DB

        private int _BookID;
        public int BookID
        {
            get { return _BookID; }
            set { _BookID = value; }
        }


        private string _BookCode;
        public string BookCode
        {
            get { return _BookCode; }
            set { _BookCode = value; }
        }


        private string _Title;
        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }


        private int _Status;
        public int Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        #endregion
        #region - Calculated
        public bool IsActive
        {
            get { return Status == 1; }
        }
        #endregion
        #endregion

        public BookInfo()
        {
        }

        public BookInfo(int bookID, string bookCode, string title,
                int status)
        {
            this._BookID = bookID;
            this._BookCode = bookCode;
            this._Title = title;
            this._Status = status;
        }



        public BookInfo(IDataRecord row)
        {
            Fill(row);
        }

        public void Fill(IDataRecord row)
        {
            this._BookID = Database.GetAnyInt(row, "book_id");
            this._BookCode = Database.GetString(row, "book_code");
            this._Title = Database.GetString(row, "title");
            this._Status = Database.GetAnyInt(row, "status");
        }
    }
}