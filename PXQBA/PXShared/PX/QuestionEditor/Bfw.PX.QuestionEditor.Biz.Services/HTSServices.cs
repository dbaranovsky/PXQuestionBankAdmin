using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;
using System.IO;

using Mono.Options;
using Microsoft.Practices.ServiceLocation;

using Bfw.Common;
using Bfw.Common.Logging;
using Bfw.Common.Collections;
using Bfw.Common.Database;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.Commands;

using Bdc = Bfw.PX.Biz.DataContracts;
using Adc = Bfw.Agilix.DataContracts;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.PX.QuestionEditor.Biz.DataContracts;


namespace Bfw.PX.QuestionEditor.Biz.Services
{
    public interface IHTSServices
    {
        /// <summary>
        /// Retrieves a question item from DLAP.
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        Bdc.Question GetQuestion(string entityId, string questionId);

        /// <summary>
        /// Retrieves a quiz item from dlap.
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="quizId"></param>
        /// <returns></returns>
        Bdc.ContentItem GetQuiz(string entityId, string quizId);

        /// <summary>
        /// Converts an existing DLAP item data based question to an advanced HTS question.
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns>Returns the converted question.</returns>
        Bdc.Question ConvertQuestion(string entityId, string questionId);

        /// <summary>
        /// Saves an updated question back to DLAP.
        /// </summary>
        /// <param name="question"></param>
        void StoreQuestion(Bdc.Question question);

        /// <summary>
        /// Saves an updated question back to DLAP.
        /// </summary>
        /// <param name="question"></param>
        void StoreQuestion(HTSData htsData);

        /// <summary>
        /// Saves a "preview" copy of the specified question data. A temporary Quiz and question are created with a fixed ID.
        /// </summary>
        /// <param name="htsData"></param>
        /// <returns>The ID of the generated quiz.</returns>        
        string StoreQuizPreview(HTSData htsData);

        /// <summary>
        /// Stores a collection of questions back to DLAP.
        /// </summary>
        /// <param name="questions"></param>
        void StoreQuestions(IList<Bdc.Question> questions);
        /// <summary>
        /// Loads HTSData from Dlap
        /// </summary>
        /// <param name="entityId">The entity id</param>
        /// <param name="questionId">The question id</param>
        /// <param name="playerUrl">The play url</param>
        /// <param name="isConvert">Is converted</param>
        /// <returns></returns>
        Biz.DataContracts.HTSData GetHtsData(string entityId, string questionId, string playerUrl, bool isConvert);
        
    }

    public class HTSServices : IHTSServices
    {
        #region Properties

        /// <summary>
        /// Extension used by all PX Resource files.
        /// </summary>
        public const string PXRES_EXTENSION = "pxres";

        /// <summary>
        /// Gets or sets the session manager.
        /// </summary>
        /// <value>
        /// The session manager.
        /// </value>
        protected ISessionManager SessionManager { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentActions"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sessionManager">The session manager.</param>
        public HTSServices(ISessionManager sessionManager)
        {
            SessionManager = sessionManager;
        }

        #endregion

        #region IHTSActions Members

        /// <summary>
        /// Gets a question by its ID.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        /// <param name="questionId">The question ID.</param>
        /// <returns></returns>
        public Bdc.Question GetQuestion(string entityId, string questionId)
        {
            Bdc.Question result = null;

            //using (Context.Tracer.DoTrace("ContentActions.GetQuestion(entityId={0}, questionId={1})", entityId, questionId))
            //{
            //    result = Context.CacheProvider.FetchQuestion(entityId, questionId);

            if (result == null)
            {
                var questions = GetQuestions(entityId, new string[] { questionId });

                if (!questions.IsNullOrEmpty())
                {
                    result = questions.First();
                    //Context.CacheProvider.StoreQuestion(result);
                }
            }
            //}

            return result;
        }

        public Bdc.ContentItem GetQuiz(string entityId, string quizId)
        {
            Bdc.ContentItem ci = null;

            var itemList = FindItems(new Adc.ItemSearch()
            {
                EntityId = entityId,
                ItemId = quizId
            });

            var contentItems = itemList as IList<Bdc.ContentItem> ?? itemList.ToList();
            if (!contentItems.IsNullOrEmpty())
            {
                ci = contentItems.First();

            }

            return ci;
        }

       
        /// <summary>
        /// Loads HTSData from Dlap
        /// </summary>
        /// <param name="entityId">The entity id</param>
        /// <param name="questionId">The question id</param>
        /// <param name="playerUrl">The play url</param>
        /// <param name="isConvert">Is converted</param>
        /// <returns></returns>
        public HTSData GetHtsData(string entityId, string questionId, string playerUrl, bool isConvert)
        {
            string formulaEditorUrl;
            //TODO: remove temporary Ids

            formulaEditorUrl = playerUrl.ToLowerInvariant().Replace("pxplayer.ashx", "geteq.ashx");

            var question = GetQuestion(entityId, questionId);

            var htsData = new HTSData()
            {
                EntityId = entityId,
                QuestionId = questionId,
                PlayerUrl = playerUrl,
                FormulaEditorUrl = formulaEditorUrl,
                QuestionTitle = question.Body
            };

            if (isConvert)
            {
                htsData.LoadQuestionXml(question.QuestionXml);
            }
            else
            {
                var interactionData = question.InteractionData;
                htsData.LoadXML(interactionData);
            }

            return htsData;
        }
        /// <summary>
        /// Gets a list of questions by their IDs.
        /// </summary>
        /// <param name="entityId">Id of the entity in which the questions exist.</param>
        /// <param name="questionIds">Ids of the questions to get</param>
        /// <returns></returns>
        public IEnumerable<Bdc.Question> GetQuestions(string entityId, IEnumerable<string> questionIds)
        {
            var options = new CommandOptions();
            var sm = EstablishConnection();
            var oSession = sm.CurrentSession;


            var cachedResults = new List<Bdc.Question>();

            var questionsToFetch = new List<string> {questionIds.FirstOrDefault()};

            if (!questionsToFetch.IsNullOrEmpty())
            {
                var cmd = new GetQuestions()
                {
                    SearchParameters = new Adc.QuestionSearch()
                    {
                        EntityId = entityId,
                        QuestionIds = questionsToFetch
                    }
                };

                var questionBatch = new Batch();
                questionBatch.Add("getquestions", cmd);

                oSession.ExecuteAsAdmin(questionBatch);

                cmd = questionBatch.CommandAs<GetQuestions>("getquestions");

                if (!cmd.Questions.IsNullOrEmpty())
                {
                    IEnumerable<Bdc.Question> serviceResults = cmd.Questions.Map(q => q.ToQuestion());
                    cachedResults.AddRange(serviceResults);
                }
            }

            return cachedResults;
        }

        public IEnumerable<Bdc.ContentItem> FindItems(Adc.ItemSearch search, bool asAdmin = true, string categoryId = null)
        {
            InitializeSessionManager();

            var cachedResults = new List<Bdc.ContentItem>();

            var cmd = new GetItems()
            {
                SearchParameters = search
            };

            var itemsBatch = new Batch();
            itemsBatch.Add("getitemlist", cmd);

            SessionManager.CurrentSession.ExecuteAsAdmin(itemsBatch);

            cmd = itemsBatch.CommandAs<GetItems>("getitemlist");

            if (!cmd.Items.IsNullOrEmpty())
            {
                IEnumerable<Bdc.ContentItem> serviceResults = null;
                serviceResults = cmd.Items.Map(q => q.ToContentItem());
                cachedResults.AddRange(serviceResults);
            }

            return cachedResults;
        }


        /// <summary>
        /// Stores the specified question
        /// </summary>
        /// <param name="question">The question.</param>
        public void StoreQuestion(Bdc.Question question)
        {
            StoreQuestions(new List<Bdc.Question>() { question });
        }

        /// <summary>
        /// Stores the specified question
        /// </summary>
        /// <param name="question">The question.</param>
        public void StoreQuestion(HTSData htsData)
        {
            Bdc.Question question = null;
            // remove inject namespaces
            //interactionData = interactionData.Replace("xmlns=\"http://www.w3.org/1999/xhtml\"", "");                

            var interactionData = htsData.ToXML();
            //interactionData = Server.HtmlDecode(interactionData);

            if (string.IsNullOrEmpty(htsData.QuestionId))
            {
                //Dictionary<string, string> metaData = new Dictionary<string, string>();
                //metaData.Add("createdBy", htsData.UserId);
                //metaData.Add("userCreated", "true");
                //metaData.Add("totalUsed", "1");

                //question = new PxBizDC.Question()
                //{
                //    Body = "Advanced Question",
                //    Id = Guid.NewGuid().ToString(),
                //    EntityId = htsData.EntityId,
                //    InteractionType = PxBizDC.InteractionType.Custom,
                //    Points = 1,
                //    InteractionData = interactionData,
                //    SearchableMetaData = metaData
                //};
            }
            else
            {
                question = GetQuestion(htsData.EntityId, htsData.QuestionId);
                question.InteractionData = interactionData;
                question.Body = htsData.QuestionTitle;
                question.CustomUrl = "HTS";
                question.InteractionType = Bdc.InteractionType.Custom;
            }

            StoreQuestion(question);
        }

        /// <summary>
        /// Saves a "preview" copy of the specified question data. A temporary Quiz and question are created with a fixed ID.
        /// </summary>
        /// <param name="htsData"></param>
        /// <returns>The ID of the generated quiz.</returns>   
        public string StoreQuizPreview(HTSData htsData)
        {            
            // remove inject namespaces
            //interactionData = interactionData.Replace("xmlns=\"http://www.w3.org/1999/xhtml\"", "");                

            var mockQuestion = CreateMockQuestion(htsData);
            var mockQuiz = CreateMockQuiz(mockQuestion, htsData);

            var options = new CommandOptions();
            var sm = EstablishConnection();
            var oSession = sm.CurrentSession;

            var cmdBatch = new Batch();

            var cmdQuestion = new PutQuestions();
            cmdQuestion.Add(mockQuestion);

            var cmdQuiz = new PutItems();
            cmdQuiz.Items.Add(mockQuiz);

            cmdBatch.Add(cmdQuestion);
            cmdBatch.Add(cmdQuiz);

            oSession.Execute(cmdBatch);

            return mockQuiz.Id;
        }

        /// <summary>
        /// Stores the collection of questions.
        /// </summary>
        /// <param name="question">The question.</param>
        public void StoreQuestions(IList<Bdc.Question> questions)
        {
            var options = new CommandOptions();
            var sm = EstablishConnection();
            var oSession = sm.CurrentSession;

            var cmd = new PutQuestions();
            cmd.Add(questions.Map(q => q.ToQuestion()));

            oSession.Execute(cmd);
        }

        public Adc.Item CreateMockQuiz(Adc.Question q, HTSData htsData)
        {
            string previewQuizId = "PxQuizPreview_" + htsData.EnrollmentId;

            Bdc.ContentItem quiz = new Bdc.ContentItem()
            {
                Id = previewQuizId,
                CourseId = htsData.EntityId,
                Title = "Quiz Preview for enrollment:" + htsData.EnrollmentId,
                Description = "",
                ParentId = "PX_MANIFEST",
                Type = "Assessment",
                Href = "a",
                Sequence = "a"
            };

            Adc.Item quizItem = quiz.ToItem();

            var questionsElement = quizItem.Data.Element("questions");
            // If the questions element was not found then create it.
            if (questionsElement == null)
            {
                questionsElement = new XElement("questions");
                quizItem.Data.Add(questionsElement);
            }

            var questionElement = new XElement("question");
            questionElement.Add(new XAttribute("id", q.Id));
            questionsElement.Add(questionElement);

            var points = GetPoints(htsData);
            if (!string.IsNullOrEmpty(points))
            {
                questionElement.Add(new XAttribute("score", points));
            }
            return quizItem;
        }

        private string GetPoints(HTSData htsData)
        {
            var quiz = GetQuiz(htsData.EntityId, htsData.QuizId);
            string points = string.Empty;
            if (quiz != null)
            {
                points = (from c in quiz.QuizQuestions where c.QuestionId == htsData.QuestionId select c.Score).FirstOrDefault();
            }
            return points;
        }

        public Adc.Question CreateMockQuestion(HTSData htsData)
        {
            string previewQuizQuestionId = "PxQuestionPreview_" + htsData.EnrollmentId;
            Bdc.Question question = null;
            // remove inject namespaces
            //interactionData = interactionData.Replace("xmlns=\"http://www.w3.org/1999/xhtml\"", "");                

            var interactionData = htsData.ToXML();
            //interactionData = Server.HtmlDecode(interactionData);

            Dictionary<string, string> metaData = new Dictionary<string, string>();
            metaData.Add("createdBy", htsData.EnrollmentId);
            metaData.Add("userCreated", "true");
            metaData.Add("totalUsed", "1");

            question = new Bdc.Question()
            {
                Body = "Advanced Question Preview",
                Id = previewQuizQuestionId,
                EntityId = htsData.EntityId,
                InteractionType = Bdc.InteractionType.Custom,
                Points = 1,
                InteractionData = interactionData,
                SearchableMetaData = metaData,
                CustomUrl = "HTS"
            };


            return question.ToQuestion();
        }

        /// <summary>
        /// Converts a specified question to an advanced question.
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public Bdc.Question ConvertQuestion(string entityId, string questionId)
        {
            Bdc.Question question = null;
            question = GetQuestion(entityId, questionId);
            HTSData htsData = new HTSData();

            htsData.LoadQuestionXml(question.QuestionXml);
            string interactionData = htsData.ToXML();
            interactionData = HttpUtility.HtmlDecode(interactionData);

            question.InteractionData = interactionData;
            question.Body = "Untitled Advanced Question";
            question.CustomUrl = "HTS";

            //StoreQuestion(question);

            return question;
        }

        private void InitializeSessionManager()
        {
            var username = ConfigurationManager.AppSettings["user"];
            var password = ConfigurationManager.AppSettings["password"];
            
            if (SessionManager.CurrentSession == null)
            {
                var session = SessionManager.StartNewSession(username, password, true, username);
                SessionManager.CurrentSession = session;
            }
        }

        private static Bfw.Agilix.Dlap.Session.ISessionManager EstablishConnection()
        {
            var username = ConfigurationManager.AppSettings["user"];
            var password = ConfigurationManager.AppSettings["password"];
            var sm = ServiceLocator.Current.GetInstance<Bfw.Agilix.Dlap.Session.ISessionManager>();
            sm.CurrentSession = sm.StartNewSession(username, password, true, username);
            return sm;
        }

        private static OptionSet RegisterOptions(CommandOptions options)
        {
            var os = new OptionSet();
            os.Add("k:", k => options.Key = k);
            os.Add("v:", v => options.Value = v);
            os.Add("f:", f => options.FolderId = f);
            os.Add("c:", c => options.CourseId = c);
            os.Add("p:", p => options.ProductCourseId = p);
            os.Add("t:", t => options.Type = Bfw.PX.Biz.Services.Mappers.BizEntityExtensions.PropertyTypeFromString(t));
            os.Add("u:", u => options.UserId = u);
            os.Add("s:", s => options.Status = s);
            os.Add("a:", a => options.Action = a);
            os.Add("d:", d => options.DataFilePath = d);
            return os;
        }

        #endregion
    }


    public class CommandOptions
    {
        /// <summary>
        /// Metadata key to add or overwrite
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Value of the Metadata key to use
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Id of the folder whose items should be marked
        /// </summary>
        public string FolderId { get; set; }

        /// <summary>
        /// Id of the course the folder exists in
        /// </summary>
        public string CourseId { get; set; }

        public Bfw.PX.Biz.DataContracts.PropertyType Type { get; set; }

        public string Action { get; set; }

        public string UserId { get; set; }

        public string Status { get; set; }

        public string DataFilePath { get; set; }

        /// <summary>
        /// Id of the course to copy items to
        /// </summary>
        public string ProductCourseId { get; set; }
    }
}
