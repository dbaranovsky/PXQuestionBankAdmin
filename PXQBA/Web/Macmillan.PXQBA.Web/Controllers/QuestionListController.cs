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
using System.Linq.Dynamic;
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
        public ActionResult GetQuestionData(QuestionListDataRequest request)
        {

            // uncomment this for real data
            //var questionList = questionListManagementService.GetQuestionList();
            //var questions = (IList<Question>) Mapper.Map<IEnumerable<Bfw.Agilix.DataContracts.Question>, IEnumerable<Question>>(questionList);
            //questions = SetMockTitles(questions);

            //For debug paging
            var questions =  GetFakeQuestionsFromXml();
            questions = ApplyFakeOrdering(questions, request.OrderType, request.OrderField);
            var model = new QuestionListDataRespons()
                        {
                            TotalPages = questions.Count / questionPerPage,
                            QuestionList = questions.ToList().Skip((request.PageNumber - 1) * questionPerPage).Take(questionPerPage),
                            PageNumber = request.PageNumber,
                            Order = new QuestionOrder()
                                    {
                                        OrderField = request.OrderField,
                                        OrderType = request.OrderType.ToString().ToLower()
                                    }
                        };
            return JsonCamel(model);
        }

#region debug
        /// <summary>
        /// For deubg ordering question list.
        /// </summary>
        /// <returns></returns>
        private IList<Question> ApplyFakeOrdering(IList<Question> questions, OrderType orderType, string fieldName)
        {
            switch (orderType)
            {
                case OrderType.Asc:
                  return questions.AsQueryable().OrderBy(MappingNameForFake(fieldName)).ToList();
                case OrderType.Desc:
                  return questions.AsQueryable().OrderBy(MappingNameForFake(fieldName) + " descending").ToList();
            }
 
            return questions;
        }

        /// <summary>
        /// For debug ordering question list.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        private string MappingNameForFake(string title)
        {
            switch (title)
            {
                case "Chapter":
                    return "EBookChapter";
                case "Bank":
                    return "QuestionBank";
                case "Seq":
                    return "QuestionSeq";
                case "Title":
                    return "Title";
                case "Format":
                    return "QuestionType";
            }

            return "QuestionSeq";
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
                foreach (XmlNode node in nodes)
                {
                    var priview = node.SelectSingleNode("Preview");
                    string priviewText = string.Empty;
                    if (priview != null)
                    {
                        priviewText = priview.InnerText;
                    }

                    questions.Add(new Question()
                                  {
                                      Title = node.SelectSingleNode("Title").InnerText,
                                      EBookChapter = node.SelectSingleNode("Chapter").InnerText,
                                      QuestionBank = node.SelectSingleNode("Bank").InnerText,
                                      QuestionSeq = node.SelectSingleNode("Seq").InnerText,
                                      QuestionType = node.SelectSingleNode("Format").InnerText,
                                      QuestionHtmlInlinePreview = priviewText
                                  });

                }

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

#endregion

    }

}
