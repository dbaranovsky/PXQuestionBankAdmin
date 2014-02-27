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
    public interface IHTSActions
    {
        /// <summary>
        /// Retrieves a question item from DLAP.
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        Bdc.Question GetQuestion(string entityId, string questionId);

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
        /// Stores a collection of questions back to DLAP.
        /// </summary>
        /// <param name="questions"></param>
        void StoreQuestions(IList<Bdc.Question> questions);
    }

    public class HTSActions : IHTSActions
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
        public HTSActions(ISessionManager sessionManager)
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


            IEnumerable<Bdc.Question> serviceResults = null;

            List<Bdc.Question> cachedResults = new List<Bdc.Question>();

            List<string> questionsToFetch = new List<string>();

            questionsToFetch.Add(questionIds.FirstOrDefault());

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

                Batch questionBatch = new Batch();
                questionBatch.Add("getquestions", cmd);

                oSession.ExecuteAsAdmin(questionBatch);

                cmd = questionBatch.CommandAs<GetQuestions>("getquestions");

                if (!cmd.Questions.IsNullOrEmpty())
                {
                    serviceResults = cmd.Questions.Map(q => q.ToQuestion());
                    //Context.CacheProvider.StoreQuestionsByCourse(serviceResults);
                    cachedResults.AddRange(serviceResults);
                }
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

        public Bdc.Question ConvertQuestion(string entityId, string questionId)
        {
            Bdc.Question question = null;
            question = GetQuestion(entityId, questionId);
            HTSData htsData = new HTSData();

            htsData.LoadQuestionXml(question.QuestionXml);
            string interactionData = htsData.ToXML();
            interactionData = HttpUtility.HtmlDecode(interactionData);

            question.InteractionData = interactionData;
            question.Body = "Advanced Question";
            question.CustomUrl = "HTS";

            //StoreQuestion(question);

            return question;
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
