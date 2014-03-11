using Macmillan.PXQBA.Business.Contracts;
using System.Web.Mvc;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class QuestionListController : Controller
    {
        private readonly IQuestionListManagementService questionListManagementService;

        public QuestionListController(IQuestionListManagementService questionListManagementService)
        {
            this.questionListManagementService = questionListManagementService;
        }

        //
        // GET: /QuestionList/
        public ActionResult Index()
        {
            //var questions = questionListManagementService.GetQuestionList();
            return View();
        }

        [HttpPost]
        public ActionResult GetQuestionData()
        {
            //Fake data
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
                },

                new { title = "title3", 
                    questionType = "questionType3",
                    eBookChapter = "eBookChapter3",
                    questionBank = "questionBank3", 
                    questionSeq="3"
                },

                new { title = "title2", 
                    questionType = "questionType2",
                    eBookChapter = "eBookChapter2",
                    questionBank = "questionBank2", 
                    questionSeq="2"
                },

               new { title = "title2", 
                    questionType = "questionType2",
                    eBookChapter = "eBookChapter2",
                    questionBank = "questionBank2", 
                    questionSeq="2"
                },

               new { title = "title2", 
                    questionType = "questionType2",
                    eBookChapter = "eBookChapter2",
                    questionBank = "questionBank2", 
                    questionSeq="2"
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
