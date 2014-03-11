using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using AutoMapper;
using Bfw.Agilix.DataContracts;
using Macmillan.PXQBA.Business.Contracts;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Question = Macmillan.PXQBA.Business.Models.Question;

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
            return View();
        }

        [HttpPost]
        public ActionResult GetQuestionData()
        {
            var questions = questionListManagementService.GetQuestionList();
            //var data = Mapper.Map<IEnumerable<Question>, IEnumerable<Question>>(questions);

            //For debug paging
            var data = GetFakeQuestions(15);
            return Json(data);
        }


        private IEnumerable<Question> GetFakeQuestions(int count)
        {
            var questions = new List<Question>();

            for (int i = 0; i < count; i++)
            {
                questions.Add(new Question()
                              {
                                  title = "title"+i,
                                  questionType = "questionType"+i,
                                  eBookChapter = "eBookChapter"+i,
                                  questionBank = "questionBank"+i,
                                  questionSeq = i.ToString()
                              });
            }

            return questions;
        }

    }
}
