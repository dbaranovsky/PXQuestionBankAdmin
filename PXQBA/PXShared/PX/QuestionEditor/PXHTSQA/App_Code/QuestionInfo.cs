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
    /// This object represents the properties and methods of QuestionInfoList.
    /// </summary>
    [Serializable]
    public class QuestionInfoList : System.Collections.Generic.List<QuestionInfo>
    {

        public QuestionInfoList()
        {
        }

        public QuestionInfoList(QuestionInfo[] values)
        {
            AddRange(values);
        }

        public QuestionInfoList(List<QuestionInfo> values)
        {
            AddRange(values);
        }

        public void AddRange(QuestionInfo[] values)
        {
            for (int i = 0; i < values.Length; i++)
                Add(values[i]);
        }

        public QuestionInfoList(IDataReader reader)
        {
            PageUtils.DebugTrace("DBRun", "Fill Start");
            Fill(reader);
            PageUtils.DebugTrace("DBRun", "Fill End");
        }

        public void Fill(IDataReader reader)
        {
            while (reader.Read())
            {
                QuestionInfo ar1 = new QuestionInfo(reader);
                this.Add(ar1);
            }
        }
    }
    /// <summary>
    /// Summary description for QuestionInfo
    /// </summary>
    [Serializable]
    public partial class QuestionInfo
    {
        #region Fields and Properties
        #region - From DB

        private int _QuestionID;
        public int QuestionID
        {
            get { return _QuestionID; }
            set { _QuestionID = value; }
        }


        private int _BookID;
        public int BookID
        {
            get { return _BookID; }
            set { _BookID = value; }
        }


        private string _QuestionCode;
        public string QuestionCode
        {
            get { return _QuestionCode; }
            set { _QuestionCode = value; }
        }

        private string _FileName;
        public string FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }

        #endregion

        #region - Calculated

        public string XmlPath
        {
            get
            {
                return ItemServer.BaseUrl + "Question.ashx?book_id=" + BookID + "&question_code=" + HttpUtility.HtmlEncode(QuestionCode);
            }
        }

        public string XmlFilePath
        {
            get
            {
                return ItemServer.QuestionDataPath + FileName.Replace("/", "\\");
            }
        }

        #endregion
        #endregion

        public QuestionInfo()
        {
        }

        public QuestionInfo(int questionID, int bookID, string questionCode, string fileName)
        {
            this._QuestionID = questionID;
            this._BookID = bookID;
            this._QuestionCode = questionCode;
            this._FileName = fileName;
        }



        public QuestionInfo(IDataRecord row)
        {
            Fill(row);
        }

        public void Fill(IDataRecord row)
        {
            this._QuestionID = Database.GetAnyInt(row, "question_id");
            this._BookID = Database.GetAnyInt(row, "book_id");
            this._QuestionCode = Database.GetString(row, "question_code");
            this._FileName = Database.GetString(row, "filename");
        }


        public string GetXml()
        {
            string retval = null;
            FileInfo f = new FileInfo(XmlFilePath);

            if (f.Exists)
            {
                string readFileName = f.FullName;

                if (ItemServer.UseEncryptedXml)
                {
                    FileInfo encryptedFile = new FileInfo(f.Directory + "\\" + f.Name + ".ser");
                    if (!encryptedFile.Exists || encryptedFile.LastWriteTime < f.LastWriteTime)
                    {
                        // refresh cache
                        using (StreamReader sr = IPUtil.StreamReader(f.FullName))
                        {
                            retval = sr.ReadToEnd();
                            sr.Close();
                        }
                        retval = ItemServer.Encrypt(retval, true);
                        using (StreamWriter sw = IPUtil.StreamWriter(encryptedFile.FullName))
                        {
                            sw.Write(retval);
                            sw.Flush();
                            sw.Close();
                        }
                        return retval;
                    }
                    else
                        readFileName = encryptedFile.FullName;
                }

                using (StreamReader sr = IPUtil.StreamReader(readFileName))
                {
                    retval = sr.ReadToEnd();
                    sr.Close();
                }
            }
            return retval;
        }
    }
}