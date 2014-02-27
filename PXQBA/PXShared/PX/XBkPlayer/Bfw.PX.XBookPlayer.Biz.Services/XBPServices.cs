using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Collections;
using Bfw.PX.Biz.Services.Mappers;
using Microsoft.Practices.ServiceLocation;

namespace Bfw.PX.XBkPlayer.Biz.Services
{
    public interface IXBPServices
    {
        /// <summary>
        /// Retrieves a question item from DLAP.
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        Bfw.PX.Biz.DataContracts.Question GetQuestion(string entityId, string questionId);
    }
    public class XBPServices:IXBPServices
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
        public XBPServices(ISessionManager sessionManager)
        {
            SessionManager = sessionManager;
        }

        #endregion

        #region IXBkServices Members

        /// <summary>
        /// Gets a question by its ID.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        /// <param name="questionId">The question ID.</param>
        /// <returns></returns>
        public Bfw.PX.Biz.DataContracts.Question GetQuestion(string entityId, string questionId)
        {
            Bfw.PX.Biz.DataContracts.Question result = null;

            if (result == null)
            {
                var questions = GetQuestions(entityId, new string[] { questionId });

                if (!questions.IsNullOrEmpty())
                {
                    result = questions.First();
                }
            }

            return result;
        }

        private static Bfw.Agilix.Dlap.Session.ISessionManager EstablishConnection()
        {
            var username = ConfigurationManager.AppSettings["user"];
            var password = ConfigurationManager.AppSettings["password"];
            var sm = ServiceLocator.Current.GetInstance<Bfw.Agilix.Dlap.Session.ISessionManager>();
            sm.CurrentSession = sm.StartNewSession(username, password, true, username);
            return sm;
        }

        /// <summary>
        /// Gets a list of questions by their IDs.
        /// </summary>
        /// <param name="entityId">Id of the entity in which the questions exist.</param>
        /// <param name="questionIds">Ids of the questions to get</param>
        /// <returns></returns>
        public IEnumerable<Bfw.PX.Biz.DataContracts.Question> GetQuestions(string entityId, IEnumerable<string> questionIds)
        {
            var options = new CommandOptions();
            var sm = EstablishConnection();
            var oSession = sm.CurrentSession;


            IEnumerable<Bfw.PX.Biz.DataContracts.Question> serviceResults = null;

            List<Bfw.PX.Biz.DataContracts.Question> cachedResults = new List<Bfw.PX.Biz.DataContracts.Question>();

            List<string> questionsToFetch = new List<string>();

            questionsToFetch.Add(questionIds.FirstOrDefault());

            if (!questionsToFetch.IsNullOrEmpty())
            {
                var cmd = new GetQuestions()
                {
                    SearchParameters = new Bfw.Agilix.DataContracts.QuestionSearch()
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
    }
    #endregion

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
