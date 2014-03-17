using System.Linq;
using System.Web;
using AutoMapper;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using Macmillan.PXQBA.Common.Helpers;
using Question = Macmillan.PXQBA.Business.Models.Question;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class QuestionListController : MasterController
    {
        private readonly IQuestionListManagementService questionListManagementService;

        private readonly int questionPerPage;

        public QuestionListController(IQuestionListManagementService questionListManagementService)
        {
            this.questionListManagementService = questionListManagementService;
            this.questionPerPage = ConfigurationHelper.GetQuestionPerPage();
        }

        //
        // GET: /QuestionList/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetQuestionData(string query, int pageNumber)
        {

            // uncomment this for real data
            //var questionList = questionListManagementService.GetQuestionList();
            //var questions = (IList<Question>) Mapper.Map<IEnumerable<Bfw.Agilix.DataContracts.Question>, IEnumerable<Question>>(questionList);
            //questions = SetMockTitles(questions);

            //For debug paging
            var questions =  GetFakeQuestionsFromXml();
            var model = new QuestionListDataResult()
                        {
                            TotalPages = questions.Count / questionPerPage,
                            QuestionList = questions.ToList().Skip((pageNumber-1) * questionPerPage).Take(questionPerPage),
                            PageNumber = pageNumber
                        };
            return JsonCamel(model);
        }



        /// <summary>
        /// For deubg. Get list of questions from xml.
        /// </summary>
        /// <returns></returns>
        private IList<Question> GetFakeQuestionsFromXml()
        {
            var questions = new List<Question>();

            const string xmlFilePath = @"~\App_Data\dataMar-14-2014.xml";

            using (var xmlStream = System.IO.File.OpenRead(Server.MapPath(xmlFilePath)))
            {
                var document = new XmlDocument();
                document.Load(xmlStream);
                var nodes = document.SelectNodes("/records/record");
                questions.AddRange(from XmlNode node in nodes
                                   select new Question()
                                   {
                                       Title = node.SelectSingleNode("Title").InnerText,
                                       EBookChapter = node.SelectSingleNode("Chapter").InnerText,
                                       QuestionBank = node.SelectSingleNode("Bank").InnerText,
                                       QuestionSeq = node.SelectSingleNode("Seq").InnerText,
                                       QuestionType = node.SelectSingleNode("Format").InnerText
                                   });
            }

            return questions;
        }
        private IList<Question> SetMockTitles(IEnumerable<Question> questions)
        {

            foreach (var question in questions)
            {
                var words = question.Title.Split(' ');
                question.EBookChapter = string.Join(" ", words.Take(2));
                question.QuestionBank = string.Join(" ", words.Skip(2));
                question.QuestionSeq = "Consectetur";
                question.QuestionType = "Custom";
            }
            return (IList<Question>) questions;
        }
    }

}
