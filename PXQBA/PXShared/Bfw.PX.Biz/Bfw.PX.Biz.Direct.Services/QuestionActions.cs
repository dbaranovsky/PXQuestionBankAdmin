using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Caching;
using Bfw.Common.Collections;
using Bfw.Common.Logging;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.Direct.Services.Helper;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using Adc = Bfw.Agilix.DataContracts;
using Bdc = Bfw.PX.Biz.DataContracts;


namespace Bfw.PX.Biz.Direct.Services
{

    /// <summary>
    /// Implements IQuestionActions using direct connection to DLAP.
    /// </summary>
    public class QuestionActions : IQuestionActions
    {
        #region Properties
        /// <summary>
        /// Gets or sets the session manager.
        /// </summary>
        /// <value>
        /// The session manager.
        /// </value>
        protected ISessionManager SessionManager { get; set; }

        /// <summary>
        /// ContentActions
        /// </summary>
        private IContentActions _contentActions;

        /// <summary>
        /// Gets or sets the content actions.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected IContentActions ContentActions { get; set; }

        /// <summary>
        /// Provides access to cache mechanism
        /// </summary>
        protected ICacheProvider CacheProvider { get; set; }

        // The Item Quesy Helper
        protected IItemQueryActions ItemQueryActions { get; set; }

        public string CQScriptString
        {
            get
            {
                return
                    "if(typeof CQ ==='undefined')CQ = window.parent.CQ; CQ.questionInfoList['{0}'] = {{ divId: '{1}', version: '{2}', mode: '{3}', question: {{ body: '{4}', data: {5}}}, response: {{ pointspossible: '{6}', pointsassigned: '{7}'}} }}";
            }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentActions"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sessionManager">The session manager.</param>
        /// <param name="contentActions">Content actions</param>
        public QuestionActions(IBusinessContext context, ISessionManager sessionManager, IContentActions contentActions, ICacheProvider cacheProvider, IItemQueryActions itemQueryActions)
        {
            Context = context;
            SessionManager = sessionManager;
            ContentActions = contentActions;
            CacheProvider = cacheProvider;
            ItemQueryActions = itemQueryActions;
        }

        #endregion

        #region 
        /// <summary>
        /// Provides access to the BusinessContext the service is running under.
        /// </summary>
        public IBusinessContext Context { get; private set; }

        /// <summary>
        /// Finds Question Course Id from Course
        /// </summary>
        /// <param name="entityId">Entity Id of Course</param>
        /// <returns>Returns Question Course Id</returns>
        public string GetQuestionRepositoryCourse(string entityId)
        {
            var cacheKey = "QUESTION_COURSE";
            string questionCourseId = string.Empty;
            var questionCourse = CacheProvider.FetchCourseItem(entityId, cacheKey);

            if (questionCourse != null)
            {
                questionCourseId = questionCourse.ToString();
            }
            else
            {
                if (!Context.Course.QuestionBankRepositoryCourse.IsNullOrEmpty())
                {
                    questionCourseId = Context.Course.QuestionBankRepositoryCourse;
                }
                else
                {
                    List<ContentItem> items = GetCourseChapters(entityId);
                    IEnumerable<string> selectedChaptersList = items.Map(item => item.Id);
                    List<ContentItem> quizzes = GetQuizzesForSelectedChapters(entityId, selectedChaptersList);

                    foreach (ContentItem quiz in quizzes)
                    {
                        foreach (QuizQuestion quizQuestion in quiz.QuizQuestions)
                        {
                            if (quizQuestion != null && !string.IsNullOrEmpty(quizQuestion.EntityId) && quizQuestion.EntityId != entityId)
                            {
                                questionCourseId = quizQuestion.EntityId;
                                break;
                            }
                        }

                        if (!string.IsNullOrEmpty(questionCourseId))
                        {
                            break;
                        }
                    }
                }

                CacheProvider.StoreCourseItem(cacheKey, entityId, questionCourseId);
            }

            return questionCourseId;
        }

        /// <summary>
        /// Search for a set of questions and remove ones that are blank
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="questionId">The question to do this for</param>
        /// <returns></returns>
        public void DeleteInvalidQuizQuestion(string entityId, string questionId)
        {
            using (Context.Tracer.DoTrace("ContentActions.DeleteInvalidQuizQuestion(entityId={0}, questionId={1})", entityId, questionId))
            {
                XElement questionToDelete = new XElement("question",
                    new XAttribute("entityid", entityId),
                    new XAttribute("questionid", questionId)
                );

                var deleteCmd = new DeleteQuestions()
                {
                    Questions = new List<XElement>()
                    {
                        new XElement(questionToDelete)
                    }
                };

                var batch = new Batch();
                batch.Add(deleteCmd);
                SessionManager.CurrentSession.ExecuteAsAdmin(batch);
            }
        }

        /// <summary>
        /// Gets a question by its ID.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        /// <param name="questionId">The question ID.</param>
        /// <returns></returns>
        public Bdc.Question GetQuestion(string entityId, string questionId)
        {
            Bdc.Question result = null;

            using (Context.Tracer.DoTrace("ContentActions.GetQuestion(entityId={0}, questionId={1})", entityId, questionId))
            {
                result = Context.CacheProvider.FetchQuestion(entityId, questionId);

                if (result == null)
                {
                    var questions = GetQuestions(entityId, new string[] { questionId });

                    if (!questions.IsNullOrEmpty())
                    {
                        result = questions.First();
                        Context.CacheProvider.StoreQuestion(result);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the questions for an assessment.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        /// <param name="item">The item.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="lastIndex">The last index.</param>
        /// <returns></returns>
        public Bdc.QuestionResultSet GetQuestions(string entityId, Bdc.ContentItem item, string startIndex, string lastIndex)
        {
            return GetQuestions(entityId, item, false, entityId, startIndex, lastIndex);
        }

        /// <summary>
        /// Gets the questions for an assessment.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        /// <param name="itemId">The item ID.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="lastIndex">The last index.</param>
        /// <returns></returns>
        public Bdc.QuestionResultSet GetQuestions(string entityId, string itemId, string startIndex, string lastIndex)
        {
            return GetQuestions(entityId, ContentActions.GetContent(entityId, itemId), false, entityId, startIndex, lastIndex);
        }

        /// <summary>
        /// Search for a set of questions for the given entity and item ID.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="item">The item.</param>
        /// <param name="ignoreBanks">if set to <c>true</c> ignores question banks.</param>
        /// <param name="questionEntityId">The question entity id.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns></returns>
        public Bdc.QuestionResultSet GetQuestions(string entityId, Bdc.ContentItem item, bool ignoreBanks, string questionEntityId, string start, string end)
        {
            var usePaging = !(start == null || end == null);
            var resultSet = new Bdc.QuestionResultSet();
            var results = new List<Bdc.Question>();

            Bdc.QuestionQuery query = new Bdc.QuestionQuery
            {
                EntityId = entityId,
                ItemId = item.Id,
                IgnoreBanks = ignoreBanks,
                QuestionEntityId = questionEntityId,
                Start = start,
                End = end
            };

            using (Context.Tracer.StartTrace("ContentActions.GetQuestions Load Item"))
            {
                results = ProcessItem(query, usePaging, resultSet, results, item.QuizQuestions);
            }
            resultSet.Questions = results;
            resultSet.TotalCount = item.QuizQuestions.Count;
            return resultSet;
        }

        /// <summary>
        /// Gets a list of questions by their IDs.
        /// </summary>
        /// <param name="entityId">Id of the entity in which the questions exist.</param>
        /// <param name="questionIds">Ids of the questions to get</param>
        /// <returns></returns>
        public IEnumerable<Bdc.Question> GetQuestions(string entityId, IEnumerable<string> questionIds)
        {
            List<Bdc.Question> cachedResults = new List<Bdc.Question>();
            
            using (Context.Tracer.DoTrace("ContentActions.GetQuestions(entityId={0}, questionIds)", entityId))
            {
                var cachedObjects = Context.CacheProvider.FetchQuestions(entityId, questionIds.ToList());
                if (cachedObjects != null)
                {
                    cachedResults.AddRange(cachedObjects);
                }
                List<string> questionsToFetch;
                if (!cachedResults.IsNullOrEmpty())
                {
                    questionsToFetch = questionIds.Where(q => cachedResults.All(c => c.Id != q)).ToList();
                }
                else
                {
                    questionsToFetch = questionIds.ToList();
                }

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

                    SessionManager.CurrentSession.ExecuteAsAdmin(questionBatch);

                    cmd = questionBatch.CommandAs<GetQuestions>("getquestions");

                    if (!cmd.Questions.IsNullOrEmpty())
                    {
                        IEnumerable<Bdc.Question> serviceResults = cmd.Questions.Map(q => q.ToQuestion());
                        Context.CacheProvider.StoreQuestionsByCourse(serviceResults);
                        cachedResults.AddRange(serviceResults);
                    }
                }
            }

            return cachedResults;
        }

        /// <summary>
        /// Get Custom Question Preview
        /// </summary>
        /// <param name="customUrl"></param>
        /// <param name="questionData"></param>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public string GetCustomQuestionPreview(string customUrl, string questionData, string questionId)
        {
            string questionPreview = String.Empty;
            using (Context.Tracer.DoTrace("ContentActions.GetCustomQuestionPreview(customUrl={0}, questionId={1}, questionData={2})", customUrl, questionId, questionData))
            {
                if (!string.IsNullOrEmpty(customUrl))
                {
                    if (customUrl == "HTS")
                    {
                        questionPreview = GetHTSQuestionPreview(Context.Domain.CustomQuestionUrls[customUrl], questionData, questionId);
                    }
                    else if (customUrl == "FMA_GRAPH")
                    {
                        questionPreview = GetGraphQuestionPreview(Context.Domain.CustomQuestionUrls[customUrl], questionData, questionId);
                    }
                }
            }

            return questionPreview;
        }

        /// <summary>
        /// Get HTS Question Preview
        /// </summary>
        /// <param name="sUrl"></param>
        /// <param name="sXml"></param>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public string GetHTSQuestionPreview(string sUrl, string sXml, string questionId)
        {
            bool isBlankQuestion = sXml.IsNullOrEmpty();
            string sResponse = "";
            sXml = isBlankQuestion ? sXml : HtmlXmlHelper.CleanupHtmlString(sXml);
            string sQuestionData = sXml;
            questionId = questionId.Replace("-", "_");
            using (Context.Tracer.DoTrace("ContentActions.GetHTSQuestionPreview(customUrl={0}, questionId={1}, questionData={2})", sUrl, questionId, sXml))
            {
                try
                {
                    sXml = "<info id=\"0\" mode=\"PrintKey\"><question schema=\"2\" partial=\"false\"><answer /><body></body><customurl>HTS</customurl><interaction type=\"custom\"><data><![CDATA[" + sXml + "]]></data></interaction></question></info>";
                    byte[] buffer = Encoding.UTF8.GetBytes(sXml);

                    var webReq = WebRequest.Create(sUrl);

                    webReq.Method = "POST";
                    webReq.ContentLength = buffer.Length;
                    Stream reqStream = webReq.GetRequestStream();
                    reqStream.Write(buffer, 0, buffer.Length);
                    reqStream.Close();
                    using (WebResponse webRes = webReq.GetResponse())
                    {
                        using (var resReader = new StreamReader(webRes.GetResponseStream()))
                        {
                            sResponse = resReader.ReadToEnd();
                        }
                    }
                }
                catch (Exception ex)
                {
                    sResponse = ex.ToString();
                }

                try
                {
                    //parse xml response and grab custom question service markup to inject
                    XDocument myXml = XDocument.Parse(sResponse);
                    sResponse = myXml.Element("custom").Element("body").Value; // CDATA property loaded as text.

                    //replace token strings with actual question IDs, this allows for unique JS events
                    sResponse = sResponse.Replace("$QUESTIONID$", questionId); // just use placeholder value.

                    //add questionInfo object to client side array
                    string divId = "custom_preview_" + questionId;
                    string version = "1";
                    string mode = "Review";
                    string body = "";
                    var ser = new JavaScriptSerializer();
                    string data = ser.Serialize(sQuestionData);
                    string pointsPossible = "1";
                    string pointsAssigned = "NaN";

                    string cqScript = string.Format(
                        CQScriptString,
                        questionId,
                        divId,
                        version,
                        mode,
                        body,
                        data,
                        pointsPossible,
                        pointsAssigned);

                    cqScript = string.Format("<script type='text/javascript'>{0}</script>", cqScript);

                    sResponse = cqScript + sResponse;

                    //=== required if rendering question in iframe          
                    string cqPatch = "<script type='text/javascript' src='/Scripts/Quiz/CQ.js' />";
                    sResponse = cqPatch + sResponse;
                    //===

                    //CQ questionIds are assumed to be numeric, PX allows alphanumeric so fix script.
                    string questionIdNumeric = "(" + questionId;
                    string questionIdAlpha = "('" + questionId + "'";
                    sResponse = sResponse.Replace(questionIdNumeric, questionIdAlpha);

                    //if question data exist start render else inject a default message
                    if (sResponse.Length == 0 || isBlankQuestion)
                    {
                        sResponse = "<br /><div id=\"info.divId\">The question does not contain any data.</div>";
                    }
                    else
                    {
                        sResponse = "<br /><div id=\"info.divId\">Generating preview...</div>" + sResponse;
                    }
                    sResponse = sResponse.Replace("getElementById(info.divId)", "getElementById('info.divId')");
                    sResponse = sResponse.Replace("info.divId", "info.divId" + questionId);

                    //sResponse = sResponse.Replace("[~]", "/BrainHoney/Resource/" + Context.EntityId);
                }
                catch (Exception ex2)
                {
                    sResponse = "There was an error: " + ex2.ToString();
                }
            }
            return sResponse;

        }

        /// <summary>
        /// Get Graph Question Preview
        /// </summary>
        /// <param name="sUrl"></param>
        /// <param name="sXml"></param>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public string GetGraphQuestionPreview(string sUrl, string sXml, string questionId)
        {
            string sResponse = "";
            string sQuestionData = sXml;
            using (Context.Tracer.DoTrace("ContentActions.GetHTSQuestionPreview(customUrl={0}, questionId={1}, questionData={2})", sUrl, questionId, sXml))
            {
                try
                {
                    sXml = "<info id=\"0\" mode=\"Active\"><question schema=\"2\" partial=\"false\"><answer /><body></body><customurl>HTS</customurl><interaction type=\"custom\"><data><![CDATA["
                        + sXml + "]]></data></interaction></question></info>";
                    byte[] buffer = Encoding.UTF8.GetBytes(sXml);

                    HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(sUrl);

                    webReq.Method = "POST";
                    webReq.ContentLength = buffer.Length;
                    Stream reqStream = webReq.GetRequestStream();
                    reqStream.Write(buffer, 0, buffer.Length);
                    reqStream.Close();

                    using (WebResponse webRes = webReq.GetResponse())
                    {
                        using (StreamReader resReader = new StreamReader(webRes.GetResponseStream()))
                        {
                            sResponse = resReader.ReadToEnd();
                        }
                    }
                }
                catch (Exception ex)
                {
                    sResponse = ex.ToString();
                }

                try
                {
                    JavaScriptSerializer ser = new JavaScriptSerializer();
                    XDocument myXml = XDocument.Parse(sResponse);
                    sResponse = myXml.Element("custom").Element("body").Value; // CDATA property loaded as text.
                    sResponse = sResponse.Replace("$QUESTIONID$", questionId); // just use placeholder value.

                    string divId = "custom_preview_" + questionId;
                    string version = "1";
                    string mode = "Active";
                    string body = "";
                    string data = ser.Serialize(sQuestionData);
                    string pointsPossible = "1";
                    string pointsAssigned = "NaN";

                    string cqScript = string.Format(
                        CQScriptString,
                        questionId,
                        divId,
                        version,
                        mode,
                        body,
                        data,
                        pointsPossible,
                        pointsAssigned);

                    cqScript = string.Format("<script type='text/javascript'>{0}</script>", cqScript);

                    sResponse = cqScript + sResponse;

                    //CQ questionIds are assumed to be numeric, PX allows alphanumeric so fix script.
                    string questionIdNumeric = "(" + questionId;
                    string questionIdAlpha = "('" + questionId + "'";
                    sResponse = sResponse.Replace(questionIdNumeric, questionIdAlpha);

                    if (sResponse.Length == 0)
                    {
                        sResponse = "<br /><div id=\"questionInfo.divId\">The question does not contain any data.</div>";
                    }
                    else
                    {
                        sResponse = "<br /><div id=\"questionInfo.divId\">Generating graph preview...</div>" + sResponse;
                    }
                    sResponse = sResponse.Replace("getElementById(questionInfo.divId)", "getElementById('questionInfo.divId')");
                    sResponse = sResponse.Replace("questionInfo.divId", "questionInfo.divId" + questionId);

                    //sResponse = sResponse.Replace("[~]", "/BrainHoney/Resource/" + entityId);
                }
                catch (Exception ex2)
                {
                    sResponse = "There was an error: " + ex2.ToString();
                }

            }
            return sResponse;
        }

        /// <summary>
        /// Get the question analysis by entity ID.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        /// <param name="itemId">The item ID.</param>
        /// <returns></returns>
        public IList<Bdc.QuestionAnalysis> GetQuestionAnalysis(string entityId, string itemId)
        {
            List<Bdc.QuestionAnalysis> result = new List<Bdc.QuestionAnalysis>();

            using (Context.Tracer.DoTrace("ContentActions.GetQuestionAnalysis(entityId={0}, itemId={1}", entityId, itemId))
            {
                var cmd = new GetQuestionAnalysis()
                {
                    EntityId = entityId,
                    ItemId = itemId
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

                if (!cmd.QuestionAnalysis.IsNullOrEmpty())
                {
                    result = cmd.QuestionAnalysis.Map(qa => qa.ToQuestionAnalysis()).ToList();
                }
            }

            return result;
        }

        /// <summary>
        /// Update the list of question ids associated with a quiz
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        /// <param name="itemId">The item ID.</param>
        /// <param name="questions">The questions.</param>
        /// <param name="savePrevPoints">Save previous points.</param>
        /// <param name="mainQuizId">
        /// When we add questions to a question pool, the main quiz id will be different from the item we're adding the questions to.
        /// The main quiz id should be used when adding the "usedin" attribute.
        /// </param>
        /// <param name="updateUsedInMetadata"></param>
        public void UpdateQuestionList(string entityId, string itemId, IList<Bdc.Question> questions, bool savePrevPoints, string mainQuizId = "")
        {
            using (Context.Tracer.DoTrace("ContentActions.UpdateQuestionList(entityId={0}, itemId={1}, questions, savePrevPoints={2})", entityId, itemId, savePrevPoints))
            {
                var item = ContentActions.GetRawItem(entityId, itemId);

                if (item != null)
                {
                    List<Bdc.Question> questionsToAdd = new List<Bdc.Question>();
                    using (Context.Tracer.DoTrace("Update XML"))
                    {
                        var data = item.Root.Element("data");
                        var questionsElement = data.Element("questions");

                        // If the questions element was not found, but we have questions to add,
                        // then create it; otherwise, leave it as it was.
                        if (questionsElement == null && !questions.IsNullOrEmpty())
                        {
                            questionsElement = new XElement("questions");
                            data.Add(questionsElement);
                        }
                        Dictionary<string, double> PrevQuestionPoints = new Dictionary<string, double>();
                        if ((savePrevPoints) && (questionsElement != null))
                        {
                            foreach (var question in questionsElement.Elements("question"))
                            {
                                string questionId = question.Attribute("id").Value;
                                double points = 1.0;//default to 1.
                                if (question.Attribute("score") != null)
                                {
                                    try
                                    {
                                        points = Convert.ToDouble(question.Attribute("score").Value);
                                    }
                                    catch (System.FormatException) { }
                                }
                                if (!(questionId.IsNullOrEmpty() || PrevQuestionPoints.ContainsKey(questionId)))
                                {
                                    PrevQuestionPoints.Add(questionId, points);
                                }
                            }
                        }

                        if (questionsElement != null)
                        {
                            questionsElement.RemoveAll();
                            foreach (var question in questions)
                            {
                                var entityIdIsEmpty = question.EntityId.IsNullOrEmpty();
                                var entityIdIsContextId = question.EntityId == Context.EntityId;
                                //default Point Value to 1
                                if (!entityIdIsEmpty && !entityIdIsContextId && question.Points == 0)
                                {
                                    question.Points = 1;//Fix-PLATX-6449
                                }

                                // Fix for PX-3756
                                // If "question" is really a question pool, question.Id can actually
                                // be the entityId, plus a colon, plus the question pool's Id, like this:
                                // 11465:90f74adc1af2444abeb8d4c5f64ad95b. The entity Id and colon
                                // need to be removed in order to prevent errors.
                                int index = question.Id.IndexOf(":");

                                string cleanedQuestionId = (index < 0)
                                    ? question.Id
                                    : question.Id.Substring(index + 1, question.Id.Length - (index + 1));

                                if (savePrevPoints && PrevQuestionPoints.ContainsKey(cleanedQuestionId))
                                {
                                    question.Points = PrevQuestionPoints[cleanedQuestionId];
                                }

                                var questionElement = new XElement("question");
                                if (question.EntityId != Context.EntityId)
                                {
                                    questionElement.Add(new XAttribute("entityid", question.EntityId));
                                }
                                questionElement.Add(new XAttribute("id", cleanedQuestionId));
                                questionElement.Add(new XAttribute("score", question.Points));

                                if (question.EntityId != null && question.EntityId != Context.EntityId && question.EntityId.Trim().ToLower() != "null")
                                {
                                    questionElement.Add(new XAttribute("type", 1));
                                }

                                if (question.InteractionType == Bdc.InteractionType.Bank)
                                {
                                    if (questionElement.Attribute("type") != null)
                                    {
                                        questionElement.Attribute("type").Value = "2";
                                    }
                                    else
                                    {
                                        questionElement.Add(new XAttribute("type", 2));
                                    }
                                    if (question.BankUse > -1)
                                    {
                                        if (questionElement.Attribute("count") != null)
                                        {
                                            questionElement.Attribute("count").Value = question.BankUse.ToString();
                                        }
                                        else
                                        {
                                            questionElement.Add(new XAttribute("count", question.BankUse));
                                        }
                                    }
                                }
                                // Update the questions that are being added to the quiz
                                // ensure we use the real quiz id if adding to pool
                                if (string.IsNullOrWhiteSpace(mainQuizId))
                                {
                                    mainQuizId = itemId;
                                }

                                questionsElement.Add(questionElement);
                            }
                        }
                    }

                    if (questionsToAdd.Any())
                    {
                        StoreQuestions(questionsToAdd);
                    }

                    HtmlXmlHelper.SwitchAttributeName(item.Root, "id", "itemid", itemId);
                    HtmlXmlHelper.SwitchAttributeName(item.Root, "actualentityid", "entityid", entityId);
                    var cmd = new PutRawItem()
                    {
                        ItemDoc = item
                    };

                    SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                }
            }
        }

        /// <summary>
        /// Append a list of questions to the existing question list.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        /// <param name="itemId">The item ID.</param>
        /// <param name="questions">The questions.</param>
        /// <param name="mainQuizId">The main quiz ID.</param>
        public void AppendQuestionList(string entityId, string itemId, IList<Bdc.Question> questions, string mainQuizId = "")
        {
            using (Context.Tracer.DoTrace("ContentActions.AppendQuestionList(entityId={0}, itemId={1}, questions)", entityId, itemId))
            {
                var existingQuestions = GetQuestions(entityId, itemId, null, null).Questions.ToList();

                // Append this list of questions to the existing list
                var combinedList = existingQuestions;
                combinedList.AddRange(questions);

                UpdateQuestionList(entityId, itemId, combinedList, false, mainQuizId);
            }
        }

        /// <summary>
        /// Add a question pool to the QuestionList.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        /// <param name="parentId">The parent ID.</param>
        /// <param name="itemId">The item ID.</param>
        /// <param name="question">The question.</param>
        public void AddQuestionToQuestionList(string entityId, string parentId, string itemId, Bdc.Question question)
        {
            using (Context.Tracer.DoTrace("ContentActions.AddQuestionToQuestionList(entityId={0}, parentId={1}, itemId={2}, question)", entityId, parentId, itemId))
            {
                var item = ContentActions.GetRawItem(entityId, parentId);

                if (item != null)
                {
                    using (Context.Tracer.DoTrace("Update XML"))
                    {
                        var data = item.Element("data") ?? item.Root.Element("data");
                        var questionsElement = data.Element("questions");

                        // If the questions element was not found, but we have questions to add,
                        // then create it; otherwise, leave it as it was.
                        if (questionsElement == null)
                        {
                            questionsElement = new XElement("questions");
                            data.Add(questionsElement);
                        }

                        if (questionsElement != null)
                        {
                            var questionElement = new XElement("question");
                            questionElement.Add(new XAttribute("id", question.Id));

                            if (question.InteractionType == Bdc.InteractionType.Bank)
                            {
                                questionElement.Add(new XAttribute("type", 2));
                                if (question.BankUse > -1)
                                {
                                    questionElement.Add(new XAttribute("count", question.BankUse));
                                }
                            }
                            questionsElement.Add(questionElement);
                        }

                        //Also update the target score if the item is learning curve.
                        if (data.Element("bfw_type") != null && data.Element("bfw_type").Value.ToLower() == "learningcurveactivity")
                        {
                            var learningCurveSettings = data.Element("bfw_learning_curve");
                            if (learningCurveSettings != null)
                            {
                                var autotargetScore = learningCurveSettings.Element("autotargetscore");
                                if (autotargetScore != null && autotargetScore.Value == "true")
                                {
                                    //find the total question pools
                                    var poolElements = questionsElement.Elements("question").Distinct();
                                    var poolCount = (from c in poolElements where (c.Attribute("type") != null && c.Attribute("type").Value == "2") select c).Distinct().Count();
                                    var defaultMultiplier = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["LearningCurveDefaultTargetMultiplier"]);
                                    learningCurveSettings.Element("targetscore").Value = (poolCount * defaultMultiplier).ToString();
                                }
                            }
                        }
                    }

                    HtmlXmlHelper.SwitchAttributeName(item.Root, "id", "itemid", parentId);
                    HtmlXmlHelper.SwitchAttributeName(item.Root, "actualentityid", "entityid", entityId);
                    var cmd = new PutRawItem()
                    {
                        ItemDoc = item
                    };

                    SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                }
            }
        }

        /// <summary>
        /// Edits the question list.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="parentId">The parent id.</param>
        /// <param name="questionId">The question id.</param>
        /// <param name="poolCount">The pool count.</param>
        /// <param name="points"></param>
        public void EditQuestionList(string entityId, string parentId, string questionId, string poolCount, int? points)
        {
            using (Context.Tracer.DoTrace("ContentActions.EditQuestionList(entityId={0}, parentId={1}, questionId={2}, poolCount={3}", entityId, parentId, questionId, poolCount))
            {
                var item = ContentActions.GetRawItem(entityId, parentId);

                if (item != null)
                {
                    using (Context.Tracer.DoTrace("Update XML"))
                    {
                        var data = item.Element("data") ?? item.Root.Element("data"); ;
                        var questionsElement = data.Element("questions");

                        // If the questions element was not found, but we have questions to add,
                        // then create it; otherwise, leave it as it was.
                        if (questionsElement == null)
                        {
                            questionsElement = new XElement("questions");
                            data.Add(questionsElement);
                        }

                        if (questionsElement != null)
                        {
                            foreach (var element in questionsElement.Elements())
                            {
                                if (element.Attribute("id").Value == questionId)
                                {
                                    if (element.Attribute("count") == null)
                                    {
                                        element.Add(new XAttribute("count", poolCount));
                                    }
                                    else
                                    {
                                        element.Attribute("count").Value = poolCount;
                                    }
                                    if (points != null)
                                    {
                                        if (element.Attribute("score") == null)
                                        {
                                            element.Add(new XAttribute("score", points.ToString()));
                                        }
                                        else
                                        {
                                            element.Attribute("score").Value = points.ToString();
                                        }
                                    }

                                    break;
                                }
                            }
                            var questionElement = new XElement("question");
                            questionElement.Add(new XAttribute("id", questionId));
                        }
                    }

                    HtmlXmlHelper.SwitchAttributeName(item.Root, "id", "itemid", parentId);
                    HtmlXmlHelper.SwitchAttributeName(item.Root, "actualentityid", "entityid", entityId);
                    var cmd = new PutRawItem()
                    {
                        ItemDoc = item
                    };

                    SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                }
            }
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
        /// <param name="questions">The questions.</param>
        public void StoreQuestions(IList<Bdc.Question> questions)
        {
            using (Context.Tracer.DoTrace("ContentActions.StoreQuestions(questions)"))
            {
                var cmd = new PutQuestions();
                cmd.Add(questions.Map(q => q.ToQuestion()));

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                Context.CacheProvider.InvalidateQuestions(questions);
            }
        }

        /// <summary>
        /// Store Quiz Preview
        /// </summary>
        /// <param name="customXml"></param>
        /// <param name="customUrl"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public string StoreQuizPreview(string customXml, string customUrl, string entityId)
        {
            customUrl = (string.IsNullOrEmpty(customUrl)) ? "FMA_GRAPH" : customUrl;
            entityId = (string.IsNullOrEmpty(entityId)) ? Context.EntityId : entityId;

            var mockQuizQuestionId = "PxQuestionPreview_" + Context.EnrollmentId;
            var mockQuizId = "PxQuizPreview_" + Context.EnrollmentId;
            var mockQuizTitle = "Quiz Preview for enrollment: " + Context.EnrollmentId;

            var mockQuestion = CreateMockQuestion(
                customXml: customXml,
                customUrl: customUrl,
                sourceEntityId: entityId,
                destinationEntityId: entityId,
                mockQuizQuestionId: mockQuizQuestionId,
                sourceQuestionId: null);

            var mockQuiz = CreateMockQuiz(question: mockQuestion, entityId: entityId, mockQuizId: mockQuizId, mockQuizTitle: mockQuizTitle);

            var cmdBatch = new Batch();

            var cmdQuestion = new PutQuestions();
            cmdQuestion.Add(mockQuestion);

            var cmdQuiz = new PutItems();
            cmdQuiz.Items.Add(mockQuiz);

            cmdBatch.Add(cmdQuestion);
            cmdBatch.Add(cmdQuiz);

            SessionManager.CurrentSession.ExecuteAsAdmin(cmdBatch);

            return mockQuiz.Id;
        }

        /// <summary>
        /// Store Mock Quiz
        /// </summary>
        /// <param name="sourceEntityId"></param>
        /// <param name="destinationEntityId"></param>
        /// <param name="sourceQuestionId"></param>
        /// <param name="mockQuestionId"></param>
        /// <param name="mockQuizId"></param>
        /// <param name="changedQuestionId"></param>
        /// <returns></returns>
        public string StoreMockQuiz(string sourceEntityId, string destinationEntityId, string sourceQuestionId, string mockQuestionId, string mockQuizId, out string changedQuestionId)
        {
            changedQuestionId = string.Empty;

            var mockQuizTitle = "Mock Quiz for enrollment: " + Context.EnrollmentId;
            var mockQuestion = CreateMockQuestion(
                customXml: null,
                customUrl: null,
                sourceEntityId: sourceEntityId,
                destinationEntityId: destinationEntityId,
                mockQuizQuestionId: mockQuestionId,
                sourceQuestionId: sourceQuestionId);

            changedQuestionId = mockQuestion.Id;

            var mockQuiz = CreateMockQuiz(question: mockQuestion, entityId: sourceEntityId, mockQuizId: mockQuizId, mockQuizTitle: mockQuizTitle);

            var cmdBatch = new Batch();
            var cmdQuestion = new PutQuestions();
            cmdQuestion.Add(mockQuestion);
            var cmdQuiz = new PutItems();
            cmdQuiz.Items.Add(mockQuiz);
            cmdBatch.Add(cmdQuiz);
            SessionManager.CurrentSession.ExecuteAsAdmin(cmdBatch);

            return mockQuiz.Id;
        }

        /// <summary>
        /// Updates the questions settings
        /// </summary>
        /// <param name="entityId">Id of the entity including the question</param>
        /// <param name="settings">The question settings</param>
        public void UpdateQuestionSettings(string entityId, Bdc.QuestionSettings settings)
        {
            using (Context.Tracer.DoTrace("ContentActions.UpdateQuestionSettings(entity={0}, settings)", entityId))
            {
                if (settings.Points.HasValue)
                {
                    var quiz = ContentActions.GetContent(entityId, settings.QuizId);
                    var quesitonList = new List<Bdc.Question>();
                    
                    foreach (var matchQuestion in quiz.QuizQuestions)
                    {
                        if (matchQuestion.QuestionId == settings.Id)
                        {
                            if (settings.Points.HasValue)
                            {
                                matchQuestion.Score = settings.Points.ToString();
                            }
                        }
                        double points;
                        if (!double.TryParse(matchQuestion.Score, out points))
                        {
                            points = 1.0; //fix without checking the value was assigned earlier.
                        }

                        int bankUse;

                        if (!Int32.TryParse(matchQuestion.Count, out bankUse))
                        {
                            bankUse = -1;
                        }

                        Bdc.Question bizQuestion = new Bdc.Question()
                        {
                            BankUse = bankUse,
                            EntityId = matchQuestion.EntityId,
                            Id = matchQuestion.QuestionId,
                            Points = points,
                            InteractionType = (!matchQuestion.Type.IsNullOrEmpty() && matchQuestion.Type == "2") ? InteractionType.Bank : InteractionType.NotBank
                        };

                        quesitonList.Add(bizQuestion);
                    }

                    UpdateQuestionList(entityId, settings.QuizId, quesitonList, savePrevPoints: false);
                }

                // If we have settings that need to be saved on a group, then do that here.
                if (settings.AttemptLimit.HasValue || settings.TimeLimit.HasValue || settings.ReviewSettings != null)
                {
                    var quizId = settings.QuizId;
                    var questionId = settings.Id;
                    var question = GetQuestion(settings.EntityId, questionId);
                    var quiz = ContentActions.GetContent(Context.EntityId, quizId);

                    if (quiz.AssessmentGroups != null)
                    {
                        // Figure out what the group name is, and try to find an existing one.
                        var groupName = quizId + "_" + questionId;
                        Bdc.AssessmentGroup group = null;
                        foreach (var quizGrp in quiz.AssessmentGroups)
                        {
                            if (groupName == quizGrp.Name)
                            {
                                group = quizGrp;
                            }
                        }

                        // If we've not found an existing group, create a new one.
                        if (group == null)
                        {
                            group = new Bdc.AssessmentGroup()
                            {
                                Name = groupName
                            };
                        }

                        if (settings.AttemptLimit.HasValue)
                        {
                            group.Attempts = settings.AttemptLimit.ToString();
                        }
                        if (settings.TimeLimit.HasValue)
                        {
                            group.TimeLimit = settings.TimeLimit.ToString();
                        }
                        if (settings.ReviewSettings != null)
                        {
                            group.ReviewSettings = settings.ReviewSettings;
                        }

                        question.AssessmentGroups.Add(groupName);
                        StoreQuestion(question);

                        if (quiz.AssessmentGroups == null)
                        {
                            quiz.AssessmentGroups = new List<PX.Biz.DataContracts.AssessmentGroup>();
                        }

                        if (settings.AttemptLimit.HasValue)
                        {
                            group.Attempts = settings.AttemptLimit.ToString();
                        }
                        if (settings.AttemptLimit.HasValue)
                        {
                            group.TimeLimit = settings.TimeLimit.ToString();
                        }

                        // If there is an existing group with the same name, remove it before adding this one.
                        quiz.AssessmentGroups = quiz.AssessmentGroups.Filter(ag => ag.Name != group.Name).ToList();
                        quiz.AssessmentGroups.Add(group);

                        ContentActions.StoreContent(quiz);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the learning curve question settings.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="settings">The settings.</param>
        public void UpdateLearningCurveQuestionSettings(string entityId, Bdc.LearningCurveQuestionSettings settings)
        {
            using (Context.Tracer.DoTrace("ContentActions.UpdateLearningCurveQuestionSettings(entity={0}, settings)", entityId))
            {
                if (!settings.QuestionId.IsNullOrEmpty())
                {
                    //get the quiz first 
                    var quiz = ContentActions.GetContent(entityId, settings.Id);

                    //means we have the question id so get the question.
                    var question = (from c in quiz.QuizQuestions where c.QuestionId == settings.QuestionId select c).FirstOrDefault();
                    if (question != null)
                    {
                        var questionBody = GetQuestion(entityId, question.QuestionId);
                        var prevSettings = questionBody.LearningCurveQuestionSettings;

                        if (prevSettings.IsNullOrEmpty())
                        {
                            prevSettings = new List<Bdc.LearningCurveQuestionSettings>();
                            questionBody.LearningCurveQuestionSettings = prevSettings;
                        }

                        //if the settings exists just change it else will need to create new settings for the question.
                        var settingsExist = (from c in prevSettings where c.Id == settings.Id select c).FirstOrDefault();
                        if (settingsExist == null)
                        {
                            //it means it is a new settings for the question.
                            var newsettings = new Biz.DataContracts.LearningCurveQuestionSettings()
                            {
                                Id = settings.Id,
                                NeverScrambleAnswers = settings.NeverScrambleAnswers,
                                DifficultyLevel = settings.DifficultyLevel,
                                PrimaryQuestion = settings.PrimaryQuestion
                            };
                            prevSettings.Add(newsettings);
                        }
                        else
                        {
                            settingsExist.Id = settings.Id;
                            settingsExist.NeverScrambleAnswers = settings.NeverScrambleAnswers;
                            settingsExist.DifficultyLevel = settings.DifficultyLevel;
                            settingsExist.PrimaryQuestion = settings.PrimaryQuestion;
                        }
                        StoreQuestion(questionBody);
                    }

                }
            }
        }      

        /// <summary>
        /// Deletes the questions permanently.
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="questionIds">The question ids.</param>
        public void DeleteQuestions(string entityId, IEnumerable<string> questionIds)
        {
            using (Context.Tracer.DoTrace("ContentActions.DeleteQuestions(entityId={0}, questionIds)", entityId))
            {
                List<XElement> questions = new List<XElement>();
                List<Bdc.Question> bizQuestions = new List<Bdc.Question>();
                foreach (var questionId in questionIds)
                {
                    XElement questionToDelete = new XElement("question",
                        new XAttribute("entityid", entityId),
                        new XAttribute("questionid", questionId)
                    );
                    Bdc.Question question = new Bdc.Question()
                    {
                        Id = questionId,
                        EntityId = entityId
                    };
                    questions.Add(questionToDelete);
                    bizQuestions.Add(question);
                }
                var deleteCmd = new DeleteQuestions()
                {
                    Questions = questions
                };
                var batch = new Batch();
                batch.Add(deleteCmd);
                SessionManager.CurrentSession.ExecuteAsAdmin(batch);
                if (!bizQuestions.IsNullOrEmpty())
                {
                    Context.CacheProvider.InvalidateQuestions(bizQuestions);
                }
            }
        }

        /// <summary>
        /// Removes the question from cache.
        /// </summary>
        /// <param name="questions">The questions.</param>
        public void RemoveQuestionsFromCache(List<Bdc.Question> questions)
        {
            using (Context.Tracer.StartTrace("ContentActions.RemoveQuestionsFromCache(questions)"))
            {
                if (!questions.IsNullOrEmpty())
                {
                    Context.CacheProvider.InvalidateQuestions(questions);
                }
            }
        }

        /// <summary>
        /// Gets a list of questions by their IDs .
        /// </summary>
        /// <param name="entityId">Id of the entity in which the questions exist.</param>
        /// <param name="questionIds">Ids of the questions to get</param>
        /// <param name="searchQuestionsFilter"> </param>
        /// <param name="count"> </param>
        /// <returns></returns>
        public IEnumerable<Bdc.Question> GetQuestions(string entityId, IEnumerable<string> questionIds, string searchQuestionsFilter, int? count)
        {

            List<Bdc.Question> cachedResults = new List<Bdc.Question>();

            string questionIdToLog = "";

            bool isQuestionsListExists = false;
            if (questionIds != null && questionIds.Any())
            {
                questionIdToLog = questionIds.Fold("|");
                isQuestionsListExists = true;
            }

            using (Context.Tracer.DoTrace("ContentActions.GetQuestions(entityId={0}, questionIds={1}, searchQuestionsFilter={2})",
                                                                           entityId, questionIdToLog, searchQuestionsFilter))
            {
                List<string> questionsToFetch = null;
                if (isQuestionsListExists)
                {
                    var cachedObjects = Context.CacheProvider.FetchQuestions(entityId, questionIds.ToList());
                    if (cachedObjects != null)
                    {
                        cachedResults.AddRange(cachedObjects);
                    }
                    if (!cachedResults.IsNullOrEmpty())
                    {
                        questionsToFetch = questionIds.Where(q => cachedResults.All(c => c.Id != q)).ToList();
                    }
                    else
                    {
                        questionsToFetch = questionIds.ToList();
                    }
                }

                if (questionsToFetch.Count() > 0)
                {
                    Adc.QuestionAdminSearch searchParameters = new Adc.QuestionAdminSearch
                    {
                        EntityId = entityId,
                        QuestionIds = questionsToFetch,
                        QuestionsFilter = searchQuestionsFilter,
                        Count = count.GetValueOrDefault()
                    };


                    GetQuestionsAdmin cmd = new GetQuestionsAdmin { SearchParameters = searchParameters };

                    Batch questionBatch = new Batch();
                    questionBatch.Add("getquestions", cmd);

                    SessionManager.CurrentSession.ExecuteAsAdmin(questionBatch);

                    cmd = questionBatch.CommandAs<GetQuestionsAdmin>("getquestions");

                    if (!cmd.Questions.IsNullOrEmpty())
                    {
                        IEnumerable<Bdc.Question> serviceResults = cmd.Questions.Map(q => q.ToQuestion());
                        if (serviceResults != null)
                        {
                            Context.CacheProvider.StoreQuestionsByCourse(serviceResults);
                            cachedResults.AddRange(serviceResults);
                        }
                    }
                }
            }

            return cachedResults;
        }
      
        /// <summary>
        /// Gets a list all versions of that question
        /// </summary>
        /// <param name="entityId">Id of the entity in which the questions exist.</param>
        /// <param name="questionId">Ids of the questions to get</param>
        /// <param name="version"></param>
        /// <returns></returns>
        public IEnumerable<Bdc.Question> GetQuestionsList(string entityId, string questionId, bool version = false)
        {
            List<Bdc.Question> qryResults = new List<Bdc.Question>();
            if (!string.IsNullOrEmpty(entityId) && !string.IsNullOrEmpty(questionId))
            {
                using (Context.Tracer.DoTrace("ContentActions.GetQuestionsList(entityId={0}, questionIds,version)", entityId))
                {
                    List<string> questionsToFetch = new List<string> { questionId };

                    Adc.QuestionAdminSearch searchParameters = new Adc.QuestionAdminSearch
                    {
                        EntityId = entityId,
                        QuestionIds = questionsToFetch,
                        version = true
                    };

                    GetQuestionsAdmin cmd = new GetQuestionsAdmin { SearchParameters = searchParameters };

                    SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

                    if (!cmd.Questions.IsNullOrEmpty())
                    {
                        IEnumerable<Bdc.Question> serviceResults = cmd.Questions.Map(q => q.ToQuestion());
                        if (serviceResults != null)
                        {
                            qryResults = serviceResults.ToList();
                        }
                    }
                }
            }
            return qryResults;
        }
        #endregion
        #region Implementation
        /// <summary>
        /// Processes the item.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="usePaging">if set to <c>true</c> [use paging].</param>
        /// <param name="resultSet">The result set.</param>
        /// <param name="results">The results.</param>
        /// <param name="quizQuestions">Quiz Questions</param>
        /// <returns></returns>
        private List<Bdc.Question> ProcessItem(Bdc.QuestionQuery query, bool usePaging, Bdc.QuestionResultSet resultSet, List<Bdc.Question> results, IList<Bdc.QuizQuestion> quizQuestions)
        {
            using (Context.Tracer.DoTrace("ContentActions.ProcessItem(query, usePaging={0}, resultSet, results, quizQuestions)", usePaging))
            {
                // Some questions can be question banks (item ids pointing to other quizzes), 
                // and they won't return from the GetQuestionsForItem query.  We need to keep
                // the original list (to retain order) and repopulate the test bank questions.
                if (!(string.IsNullOrEmpty(query.Start) || string.IsNullOrEmpty(query.End)))
                {
                    resultSet.TotalCount = quizQuestions.Count();
                    int startIndex = Convert.ToInt32(query.Start);
                    int count = Convert.ToInt32(query.End) - startIndex;
                    int limit = (resultSet.TotalCount - startIndex);
                    count = (count > limit) ? limit : count;
                    if ((startIndex < resultSet.TotalCount) && (count < resultSet.TotalCount))
                        quizQuestions = quizQuestions.ToList().GetRange(startIndex, count);
                }


                // Each question could potentially be requested from a specific entity, but we can
                // only search for questions from one entity at a time, so we to keep a mapping of 
                // entities to the list of question ids we should look for therein, then recombine
                // the results of that search.
                var questionIdsByEntity = new Dictionary<string, List<string>>();
                //When we add a question to a quiz, we copy it to the current course
                //We need to search the course for all questions before looking for them in the Discipline course (where they are originally from)
                if (!questionIdsByEntity.ContainsKey(Context.EntityId))
                {
                    questionIdsByEntity.Add(Context.EntityId, new List<string>());
                }
                foreach (var quizQuestion in quizQuestions)
                {
                    var key = query.QuestionEntityId;
                    if (!string.IsNullOrEmpty(quizQuestion.EntityId))
                    {
                        key = quizQuestion.EntityId;
                    }

                    if (!questionIdsByEntity.ContainsKey(key))
                    {
                        questionIdsByEntity[key] = new List<string>();
                    }
                    if (key != Context.EntityId)
                    {
                        questionIdsByEntity[key].Add(quizQuestion.QuestionId);
                    }
                    questionIdsByEntity[Context.EntityId].Add(quizQuestion.QuestionId);
                }

                var detailResults = LoadQuestionDetails(questionIdsByEntity);


                using (Context.Tracer.DoTrace("Populating Result List"))
                {
                    foreach (var quizQuestion in quizQuestions)
                    {
                        var id = quizQuestion.QuestionId;

                        if (quizQuestion.Type == "2")
                        {
                            // Because this question has type 2, it is a link.  (TODO: is the previous statement correct?)
                            // Make a new request to get the item it refers to, from the entity it refers to, and plop those
                            // questions in its place.
                            results = ProcessQuestionBank(results, quizQuestion, id, query, resultSet);
                        }
                        else
                        {
                            // If it is not a question bank, then add it from the list of
                            // natural questions found for the quiz.
                            results = ProcessQuestionList(results, detailResults, quizQuestion, id);
                        }
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// Processes the question list.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <param name="detailResults">The detail results.</param>
        /// <param name="quizQuestion">The quiz question.</param>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        private List<Bdc.Question> ProcessQuestionList(List<Bdc.Question> results, List<Bdc.Question> detailResults, Bdc.QuizQuestion quizQuestion, string id)
        {
            using (Context.Tracer.DoTrace("ContentActions.ProcessQuestionList(results, detailResults, quizQuestion, id={0})", id))
            {
                foreach (var detailResult in detailResults)
                {
                    if (detailResult.Id == id)
                    {
                        if (!(quizQuestion.Score.IsNullOrEmpty()))
                        {
                            detailResult.Points = Convert.ToDouble(quizQuestion.Score);
                        }
                        results.Add(detailResult);
                        break;
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// Processes the question bank.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <param name="quizQuestion"></param>
        /// <param name="id">The id.</param>
        /// <param name="query">The query.</param>
        /// <param name="resultSet"></param>
        /// <returns></returns>
        private List<Bdc.Question> ProcessQuestionBank(List<Bdc.Question> results, Bdc.QuizQuestion quizQuestion, string id, Bdc.QuestionQuery query, Bdc.QuestionResultSet resultSet)
        {
            using (Context.Tracer.DoTrace("ContentActions.ProcessQuestionBank(results, quizQuestion, id={0}, query, resultSet)", id))
            {
                //if (!string.IsNullOrEmpty(quizQuestion.EntityId) && quizQuestion.EntityId != Context.EntityId && quizQuestion.EntityId.Trim().ToLower() != "null")
                //{
                //    var bank = GetQuestions(quizQuestion.EntityId, id, true, Context.EntityId, query.Start, query.End);
                //    resultSet.TotalCount = bank.TotalCount;

                //    foreach (var bankQuestion in bank.Questions)
                //    {
                //        results.Add(bankQuestion);
                //    }
                //}
                //else 
                if (!query.IgnoreBanks)
                {
                    results.Add(CreateQuestionBankObject(quizQuestion, query.EntityId));
                }
                else
                {
                    results.Add(CreateQuestionBankObject(quizQuestion, query.EntityId, false));
                }
            }
            return results;
        }

        /// <summary>
        /// Loads the question details.
        /// </summary>
        /// <param name="questionIdsByEntity">The question ids by entity.</param>
        /// <returns></returns>
        private List<Bdc.Question> LoadQuestionDetails(Dictionary<string, List<string>> questionIdsByEntity)
        {
            var serviceResults = new List<Bdc.Question>();
            var cachedResults = new List<Bdc.Question>();

            if (questionIdsByEntity.IsNullOrEmpty())
            {
                return serviceResults;
            }

            var detailsBatch = new Batch();
            using (Context.Tracer.DoTrace("ContentActions.LoadQuestionDetails(questionIdsByEntity)"))
            {
                foreach (var searchEntityId in questionIdsByEntity.Keys)
                {
                    var questionIds = questionIdsByEntity[searchEntityId].Distinct();

                    var cachedObjects = Context.CacheProvider.FetchQuestions(searchEntityId, questionIds.ToList());
                    if (cachedObjects != null)
                    {
                        cachedResults.AddRange(cachedObjects);
                    }
                    List<string> questionsNotInCache;
                    if (!cachedResults.IsNullOrEmpty())
                    {
                        questionsNotInCache = questionIds.Where(q => cachedResults.All(c => c.Id != q)).ToList();
                    }
                    else
                    {
                        questionsNotInCache = questionIds.ToList();
                    }

                    if (!questionsNotInCache.IsNullOrEmpty())
                    {
                        Adc.QuestionSearch search = new Adc.QuestionSearch()
                        {
                            EntityId = (String.IsNullOrEmpty(searchEntityId) || searchEntityId.ToLower().Trim() == "null") ? Context.EntityId : searchEntityId,
                            QuestionIds = questionIdsByEntity[searchEntityId]
                        };
                        detailsBatch.Add(new GetQuestions()
                        {
                            SearchParameters = search,
                            RunAsync = true
                        });
                    }
                }
                if (detailsBatch.Commands.Count() > 0)
                {
                    var tasks = new List<Task<int>>();
                    foreach (var cmd in detailsBatch.Commands)
                    {
                        DlapCommand command = cmd;
                        var task =
                            System.Threading.Tasks.Task.Factory.StartNew<int>(
                                () =>
                                {

                                    SessionManager.StartNewSession(null, null, false, Context.CurrentUser.Id).ExecuteAsAdmin(command);
                                    return 0;
                                });
                        tasks.Add(task);
                    }
                    System.Threading.Tasks.Task.WaitAll(tasks.ToArray());

                    // Join all the questions returned from the Batch command.
                    detailsBatch.Commands.Map(cmd => cmd as GetQuestions).Map(cmd => cmd.Questions).Reduce(
                            (qs, details) =>
                            {                  
                                if (qs != null && details != null)
                                {
                                    foreach (var question in qs)
                                    {
                                        if (details.Any(q => q.Id == question.Id) && question.EntityId == Context.EntityId)
                                        {
                                            details.Remove(details.First(q => q.Id == question.Id));
                                        }

                                        details.Add(question.ToQuestion());
                                    }

                                    return details;
                                }
                                else return null;
                            }, serviceResults);

                }

                // Cache the questions returned from service.
                if (!serviceResults.IsNullOrEmpty())
                {
                    Context.CacheProvider.StoreQuestionsByCourse(serviceResults);
                    cachedResults.AddRange(serviceResults);
                }
            }

            return cachedResults;
        }

        /// <summary>
        /// Creates a question bank business object based off of an XML response.
        /// </summary>
        /// <param name="quizQuestion"></param>
        /// <param name="entityId"></param>
        /// <param name="loadQuestions"></param>
        /// <returns></returns>
        private Bdc.Question CreateQuestionBankObject(Bdc.QuizQuestion quizQuestion, string entityId, bool loadQuestions = true)
        {
            using (Context.Tracer.DoTrace("ContentActions.CreateQuestionBankObject(quizQuestion, entityId={0})", entityId))
            {
                int use = -1;
                if (!String.IsNullOrEmpty(quizQuestion.Count))
                {
                    Int32.TryParse(quizQuestion.Count, out use);
                }

                // Need to get the actual quiz item, so that we can get some properties (name, how many questions it has) from it.
                //var item = GetRawItem(entityId, quizQuestion.QuestionId);
                var contentItem = ContentActions.GetContent(entityId, quizQuestion.QuestionId, false);

                // Need to get the quiz's questions -- but ignoring sub-question banks.
                QuestionResultSet questions = null;
                if (loadQuestions && contentItem != null)
                {
                    questions = GetQuestions(entityId, contentItem, true, entityId, null, null);

                }

                double points;

                if (!Double.TryParse(quizQuestion.Score, out points))
                {
                    points = 1;
                }

                return new Bdc.Question()
                {
                    Id = quizQuestion.QuestionId,
                    EntityId = entityId,
                    InteractionType = Bdc.InteractionType.Bank,
                    BankCount = (contentItem != null && contentItem.QuizQuestions != null) ? contentItem.QuizQuestions.Count : 0,
                    BankUse = use,
                    Body = (contentItem != null) ? contentItem.Title : String.Empty,//.XPathSelectElement("//data/title").Value,
                    Questions = questions != null ? questions.Questions : null,
                    Points = points
                };
            }
        }

        /// <summary>
        /// Search for a set of questions for the given entity and item ID.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="itemId">The item id.</param>
        /// <param name="ignoreBanks">if set to <c>true</c> ignore question banks.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="lastIndex">The last index.</param>
        /// <returns></returns>
        private Bdc.QuestionResultSet GetQuestions(string entityId, string itemId, bool ignoreBanks, string startIndex, string lastIndex)
        {
            return GetQuestions(entityId, itemId, ignoreBanks, entityId, startIndex, lastIndex);
        }

        /// <summary>
        /// Search for a set of questions for the given entity and item ID.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="itemId">The item id.</param>
        /// <param name="ignoreBanks">if set to <c>true</c> ignores question banks.</param>
        /// <param name="questionEntityId">The question entity id.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns></returns>
        private Bdc.QuestionResultSet GetQuestions(string entityId, string itemId, bool ignoreBanks, string questionEntityId, string start, string end)
        {
            return GetQuestions(entityId, ContentActions.GetContent(entityId, itemId), ignoreBanks, questionEntityId, start, end);
        }

        private Adc.Question CreateMockQuestion(string customXml, string customUrl, string sourceEntityId, string destinationEntityId, string mockQuizQuestionId, string sourceQuestionId)
        {
            Bdc.Question question = null;

            if (string.IsNullOrEmpty(sourceQuestionId))
            {
                var interactionData = customXml;

                Dictionary<string, string> metaData = new Dictionary<string, string>();
                metaData.Add("createdBy", Context.EnrollmentId);
                metaData.Add("userCreated", "true");
                metaData.Add("totalUsed", "1");

                question = new Bdc.Question()
                {
                    Body = "Advanced Question Preview",
                    Id = mockQuizQuestionId,
                    EntityId = sourceEntityId,
                    InteractionType = Bdc.InteractionType.Custom,
                    Points = 1,
                    InteractionData = interactionData,
                    SearchableMetaData = metaData,
                    CustomUrl = customUrl
                };

                try
                {
                    if (customUrl.Equals("FMA_GRAPH", StringComparison.OrdinalIgnoreCase)
                        && !question.InteractionData.IsNullOrEmpty())
                    {
                        XDocument myXml = XDocument.Parse(question.InteractionData);
                        var questionNode = myXml.Element("question");

                        if (questionNode != null)
                        {
                            var xElement = questionNode.Element("question");
                            if (xElement != null)
                            {
                                question.Body = xElement.Value;
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            else
            {
                question = GetQuestion(sourceEntityId, sourceQuestionId);
                question.EntityId = sourceEntityId;
            }
            return question.ToQuestion();
        }

        private Adc.Item CreateMockQuiz(Adc.Question question, string entityId, string mockQuizId, string mockQuizTitle)
        {
            string parentId = ConfigurationManager.AppSettings["QBADummyQuizParentId"] != null
                ? ConfigurationManager.AppSettings["QBADummyQuizParentId"].ToString()
                : "PX_MANIFEST";

            Bdc.ContentItem quiz = new Bdc.ContentItem()
            {
                Id = mockQuizId,
                CourseId = entityId,
                Title = mockQuizTitle,
                Description = "",
                ParentId = parentId,
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
            questionElement.Add(new XAttribute("id", question.Id));
            //questionElement.Add(new XAttribute("entityid", question.EntityId));
            questionElement.Add(new XAttribute("score", question.Score));
            //questionElement.Add(new XAttribute("type", "1"));
            questionsElement.Add(questionElement);

            return quizItem;
        }

        /// <summary>
        /// GetQuizzesForSelectedChapters
        /// </summary>
        /// <param name="entityId"> </param>
        /// <param name="selectedChaptersList"> </param>
        /// "cmd=getitemlist&entityid=65131&query=/bfw_subtype='QUIZ' 
        /// AND (/parent='chapterId' OR /parent='chapterId')"
        /// <returns></returns>
        public List<ContentItem> GetQuizzesForSelectedChapters(string entityId, IEnumerable<string> selectedChaptersList)
        {
            string selectedChapters = selectedChaptersList.Select(ch => String.Format("/parent='{0}'", ch.Replace("ChapterSelectedValues=", ""))).Fold(" OR ");

            string query = string.Format("/bfw_subtype='QUIZ' AND ( {0} )", selectedChapters);

            List<Bdc.ContentItem> itemsToReturn = null;

            using (Context.Tracer.DoTrace("QuestionAdminActions.GetCourseQuizzesForSelectedChapters()"))
            {
                Bfw.Agilix.DataContracts.ItemSearch itemsSearchQuery = new Bfw.Agilix.DataContracts.ItemSearch()
                {
                    EntityId = entityId,
                    Query = query
                };

                var items = ContentActions.FindContentItems(itemsSearchQuery);

                if (items != null) itemsToReturn = items.OrderBy(itm => itm.Title).Distinct().ToList();
            }

            return itemsToReturn;
        }

        public List<ContentItem> GetCourseChapters(string entityId)
        {
            using (Context.Tracer.StartTrace("QuestionActions.GetCourseChapters"))
            {
                const string parentId = "PX_LOR";
                const string tempCategory = "";
                var SearchParameters = ItemQueryActions.BuildListChildrenQuery(entityId, parentId, 1, tempCategory, Context.CurrentUser.Id);
                List<ContentItem> items = ContentActions.FindContentItems(SearchParameters).ToList();
                return items;
            }
        }

        public List<ContentItem> GetCourseQuizzes(string entityId)
        {
            List<ContentItem> items = GetCourseChapters(entityId);
            IEnumerable<string> selectedChaptersList = items.Map(item => item.Id);
            return GetQuizzesForSelectedChapters(entityId, selectedChaptersList);
        }

        #endregion
    }
}
