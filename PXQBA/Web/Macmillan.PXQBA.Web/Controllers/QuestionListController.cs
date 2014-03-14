using Macmillan.PXQBA.Business.Contracts;
using System.Collections.Generic;
using System.Web.Mvc;
using Macmillan.PXQBA.Business.Models;
using Question = Macmillan.PXQBA.Business.Models.Question;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class QuestionListController : MasterController
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
            return View();
        }

        [HttpPost]
        public ActionResult GetQuestionData(string query, int pageNumber, int pageSize)
        {
            //var questions = questionListManagementService.GetQuestionList();
            //var data = Mapper.Map<IEnumerable<Question>, IEnumerable<Question>>(questions);

            //For debug paging
            var model = new QuestionListDataResult()
                        {
                            TotalPages = 5,
                            QuestionList = GetFakeQuestions(pageSize, pageNumber),
                            PageNumber = pageNumber
                        };
            return JsonCamel(model);
        }


        private IEnumerable<Question> GetFakeQuestions(int count, int appender)
        {
            var questions = new List<Question>();

            for (int i = 0; i < count; i++)
            {
                questions.Add(new Question()
                              {
                                  Title = "title" + (i + appender),
                                  QuestionType = "questionType" + (i + appender),
                                  EBookChapter = "eBookChapter" + (i + appender),
                                  QuestionBank = "questionBank" + (i + appender),
                                  QuestionSeq = (i + appender).ToString()
                              });
            }

            return questions;
        }

    }
}
