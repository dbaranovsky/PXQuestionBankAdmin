using Bfw.Agilix.Dlap.Session;
using Macmillan.PXQBA.Business.Contracts;
using System.Web.Mvc;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class QuestionListController : Controller
    {

        private readonly IQuestionListManagementService questionListManagementService;

        private readonly ISessionManager sessionManager;
        public QuestionListController(IQuestionListManagementService questionListManagementService, ISessionManager sessionManager)
        {
            this.questionListManagementService = questionListManagementService;
            this.sessionManager = sessionManager;
        }

        //
        // GET: /QuestionList/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetQuestionData()
        {
            var data = new []
            {
                new { title = "title1", 
                    questionType = "questionType1",
                    eBookChapter = "eBookChapter1",
                    questionBank = "questionBank1", 
                    questionSeq="1"
                },

                new { title = "title2", 
                    questionType = "questionType2",
                    eBookChapter = "eBookChapter2",
                    questionBank = "questionBank2", 
                    questionSeq="2"
                }
            };
            return Json(data);
        }

    }
}
