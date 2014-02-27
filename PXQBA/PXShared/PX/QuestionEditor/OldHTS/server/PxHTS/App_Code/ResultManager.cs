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
    /// Summary description for ResultManager
    /// </summary>
    public static class ResultManager
    {

        /// <summary>
        /// Returns a ResultInfo for the result matching this question and response code. Creates a new one if necessary, otherwise returns the existing one if found.
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="questionId">Id of the question being answered. Must exist or null is returned.</param>
        /// <param name="responseCode">Angel response ID identifying a student's response for a particual question</param>
        /// <returns></returns>
        public static ResultInfo CreateResult(int bookID, int questionID, string responseCode, string qid)
        {
            int resultID = 0;
            using (Database db = new Database())
            {
                db.StoredProcedure = "ResultCreate";
                db.AddParameter("@response_id", responseCode, DbType.String);
                db.AddParameter("@question_id", questionID, DbType.Int32);
                db.AddParameter("@qid", qid, DbType.String);

                db.AddOutputParameter("@result_id", DbType.Int32);
                db.ExecuteSql();
                resultID = db.GetParameterValueAnyInt("@result_id");
            }

            if (resultID <= 0)
                return null; // question does not exist
            else
                return ResultInfoDal.GetResultInfo(resultID);
        }

        public static void UpdateDateTime(int resultID)
        {
            using (Database db = new Database())
            {
                db.StoredProcedure = "ResultUpdateDateTime";
                db.AddParameter("@result_id", resultID, DbType.Int32);
                db.ExecuteSql();
            }
        }
    }
}