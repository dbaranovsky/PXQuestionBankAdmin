using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace BFW
{
    /// <summary>
    /// Summary description for CQ
    /// </summary>
    public class CQ
    {
        private string _BookCode;
        public string BookCode
        {
            get { return _BookCode; }
        }

        private string _QuestionCode;
        public string QuestionCode
        {
            get { return _QuestionCode; }
        }

        public bool IsValid
        {
            get 
            {
                if (String.IsNullOrEmpty(BookCode) || String.IsNullOrEmpty(QuestionCode))
                    return false;
                else
                    return true;
            }
        }

        public CQ(string cqid)
        {
            // boocode_cqid (cqid may contain underscore characters; bookcode cannot)

            if (String.IsNullOrEmpty(cqid))
                return;

            // find first underscore
            int index = cqid.IndexOf("_");
            if (index == -1)
                return;

            _BookCode = cqid.Substring(0, index).ToLower();
            _QuestionCode = cqid.Substring(index + 1, cqid.Length - index - 1);
        }
    }

    

}