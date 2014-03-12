using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Web.Controllers.Infrastructure;
using System.Collections.Generic;
using System.Web.Mvc;
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
        public ActionResult GetQuestionData()
        {
            var questions = questionListManagementService.GetQuestionList();
            //var data = Mapper.Map<IEnumerable<Question>, IEnumerable<Question>>(questions);

            //For debug paging
            var data = GetFakeQuestions(15);
            return JsonCamel(data);
        }


        private IEnumerable<Question> GetFakeQuestions(int count)
        {
            var questions = new List<Question>();

            for (int i = 0; i < count; i++)
            {
                questions.Add(new Question()
                              {
                                  Title = "title"+i,
                                  QuestionType = "questionType"+i,
                                  EBookChapter = "eBookChapter"+i,
                                  QuestionBank = "questionBank"+i,
                                  QuestionSeq = i.ToString()
                              });
            }

            return questions;
        }

    }
}
