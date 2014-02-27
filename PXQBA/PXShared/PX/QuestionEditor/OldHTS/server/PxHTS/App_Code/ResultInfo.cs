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
using System.IO;

namespace BFW
{


    /// <summary>
    /// This object represents the properties and methods of ResultInfoList.
    /// </summary>
    public class ResultInfoList : System.Collections.Generic.List<ResultInfo>
    {

        public ResultInfoList()
        {
        }

        public ResultInfoList(ResultInfo[] values)
        {
            AddRange(values);
        }

        public ResultInfoList(List<ResultInfo> values)
        {
            AddRange(values);
        }

        public void AddRange(ResultInfo[] values)
        {
            for (int i = 0; i < values.Length; i++)
                Add(values[i]);
        }

        public ResultInfoList(IDataReader reader)
        {
            PageUtils.DebugTrace("DBRun", "Fill Start");
            Fill(reader);
            PageUtils.DebugTrace("DBRun", "Fill End");
        }

        public void Fill(IDataReader reader)
        {
            while (reader.Read())
            {
                ResultInfo ar1 = new ResultInfo(reader);
                this.Add(ar1);
            }
        }
    }
    /// <summary>
    /// Summary description for ResultInfo
    /// </summary>
    public partial class ResultInfo
    {
        #region Fields and Properties
        #region - From DB

        private int _ResultID;
        public int ResultID
        {
            get { return _ResultID; }
            set { _ResultID = value; }
        }


        private string _ResponseID;
        public string ResponseID
        {
            get { return _ResponseID; }
            set { _ResponseID = value; }
        }

        private int _BookID;
        public int BookID
        {
            get { return _BookID; }
            set { _BookID = value; }
        }


        private int _QuestionID;
        public int QuestionID
        {
            get { return _QuestionID; }
            set { _QuestionID = value; }
        }


        private DateTime _Datecreated;
        public DateTime Datecreated
        {
            get { return _Datecreated; }
            set { _Datecreated = value; }
        }

        #endregion
        #region - Calculated
    #endregion
        #endregion

        public ResultInfo()
        {
        }

        public ResultInfo(int questionID, int bookID, int resultID, string responseID,
                DateTime datecreated)
        {
            this._ResultID = resultID;
            this._ResponseID = responseID;
            this._QuestionID = questionID;
            this._BookID = bookID;
            this._Datecreated = datecreated;
        }



        public ResultInfo(IDataRecord row)
        {
            Fill(row);
        }

        public void Fill(IDataRecord row)
        {
            this._ResultID = Database.GetAnyInt(row, "result_id");
            this._ResponseID = Database.GetString(row, "response_id");
            this._QuestionID = Database.GetAnyInt(row, "question_id");
            this._BookID = Database.GetAnyInt(row, "book_id");
            this._Datecreated = Database.GetDateTime(row, "datecreated");
        }

        public string GetXmlPath()
        {
            return ItemServer.ResultsDataPath + BookID + "\\" + QuestionID + "\\" + ResultID + ".xml";
        }

        public string GetXml()
        {
            string retval = null;
            FileInfo f = new FileInfo(GetXmlPath());
            if (f.Exists)
            {
                using (StreamReader sr = IPUtil.StreamReader(f.FullName))
                {
                    retval = sr.ReadToEnd();
                    sr.Close();
                }
            }
            return retval;
        }
    }
}