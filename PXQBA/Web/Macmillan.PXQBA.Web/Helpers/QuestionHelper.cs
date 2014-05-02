using System.Web;

namespace Macmillan.PXQBA.Web.Helpers
{
    public class QuestionHelper
    {
        private const string QuestionIdParamName = "question_id_to_edit";

        public static string QuestionIdToEdit
        {
            get { return HttpContext.Current.Session[QuestionIdParamName].ToString(); }
            set { HttpContext.Current.Session[QuestionIdParamName] = value; }
        }
    }
}