using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Unity;

using Bfw.PXAP.Components;
using Bfw.PXAP.ServiceContracts;
using Bfw.PXAP.Models;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Patterns.Unity;
using Bfw.Common.Collections;
using System.Configuration;


namespace Bfw.PXAP.Services
{
    public class ContentService : IContentService
    {

        private ISession Session { get; set; }
        private IApplicationContext Context { get; set; }

        public ContentService(IApplicationContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Copy Content from one Entity to another
        /// </summary>
        /// <param name="entityId">CourseId</param>
        /// <param name="parentId">Parent</param>
        /// <param name="category">TOC Category</param>
        /// <param name="contentType">BFW_TYPE</param>
        /// <param name="contentSubType">BFW_SUBTYPE</param>        
        /// <param name="moveToEntityId">Move to CourseId</param>        
        /// <param name="moveToParent">Move to Parent Course Flag</param>        
        /// <returns></returns>
        public void CopyContent(string entityId, string parentId, string category, string contentType, string contentSubType,string moveToEntityId, bool moveToParent, Int64 processId)
        {
            // Confiqure the Service
            ConfigureServiceLocator();
            var sm = EstablishConnection();
            Session = sm.CurrentSession;
            
            // Setup progress bar information
            int percentageDone = 0;
            IProgressService progress = new ProgressService();
            String message = "";
            ProgressModel progModel = new ProgressModel() { ProcessId = processId, Percentage = 0, Status = "Processing" };

            // Find To Course ID
            string destinationCourseId = string.Empty;            
            if (!string.IsNullOrEmpty(entityId) || entityId != "0")
            {
                if (moveToParent)
                {
                    var cmdCourse = new GetCourse()
                    {
                        SearchParameters = new CourseSearch()
                        {
                            CourseId = entityId
                        }
                    };
                    Session.ExecuteAsAdmin(cmdCourse);
                    Course result = cmdCourse.Courses.First();
                    if (result != null)
                        destinationCourseId = result.ParentId;
                }
                else if (!string.IsNullOrEmpty(moveToEntityId) || moveToEntityId != "0")
                {
                    destinationCourseId = moveToEntityId;
                }
            }


            if (!string.IsNullOrEmpty(destinationCourseId))
            {
                // Build query to Search items
                var cmd = BuildCmd(entityId, parentId, category, contentType, contentSubType);

                Session.ExecuteAsAdmin(Context.Environment, cmd);
                List<Item> items = cmd.Items;                
                
                if (items.Count > 20)
                {
                    int skip = 0;
                    //now set up the progress component
                    progress.AddUpdateProcess(progModel, out message);

                    List<Item> records = new List<Item>();
                    do
                    {
                        //putting group: " + skip
                        records = items.Skip(skip).Take(20).ToList();

                        if (records.Count > 0)
                        {
                            percentageDone = Convert.ToInt32(skip * 100 / items.Count);
                            progModel.Percentage = percentageDone;
                            progress.AddUpdateProcess(progModel, out message);
                            StoreItems(entityId,destinationCourseId, records);
                        }
                        skip += 20;

                    } while (records.Count >= 20);

                }
                else
                    StoreItems(entityId,destinationCourseId, items);

                List<string> questionsToCopy = FindQuestionToCopy(items);
                if(questionsToCopy.Count > 0)
                {
                    IEnumerable<Question> questions = GetQuestionList(entityId,questionsToCopy);
                    if (questions.Count() > 0)
                        PutQuestionList(destinationCourseId, questions.ToList());
                }
            }

            //update the progress to 100%
            progModel.Percentage = 100;
            progress.AddUpdateProcess(progModel, out message);
        }

        /// <summary>
        /// Build query for get items command
        /// </summary>
        /// <param name="entityId">CourseId</param>
        /// <param name="parentId">Parent</param>
        /// <param name="category">TOC Category</param>
        /// <param name="contentType">BFW_TYPE</param>
        /// <param name="contentSubType">BFW_SUBTYPE</param>        
        /// <returns></returns>
        private GetItems BuildCmd(string entityId, string parentId, string category, string contentType, string contentSubType)
        {
            Dictionary<string, string> query = new Dictionary<string, string>();
            
            if (!string.IsNullOrEmpty(parentId))
            {
                if (string.IsNullOrEmpty(category))
                    query.Add("parent", parentId);
                else
                    query.Add(string.Format(@"bfw_tocs/{0}@parentId", category), parentId);                    
            }
            if (!string.IsNullOrEmpty(contentType))
            {
                query.Add("bfw_type", contentType);
            }
            if (!string.IsNullOrEmpty(contentSubType))
            {
                query.Add("bfw_subtype", contentSubType);
            }        
            var queryCmd = new GetItems()
            {
                SearchParameters = BuildItemSearchQuery(entityId, query, "AND")
            };
            return queryCmd;
        }

        /// <summary>
        /// Build query for custom Agilix get items command
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="queryParams"></param>
        /// <param name="userId"></param>
        /// <param name="op">logical operator to use in query</param>
        /// <returns></returns>
        private ItemSearch BuildItemSearchQuery(string entityId, Dictionary<string, string> queryParams, string op)
        {
            var search = new ItemSearch()
            {
                EntityId = entityId,
                Query = ""
            };
            List<string> queryExpressions = new List<string>();
            foreach (string key in queryParams.Keys)
            {
                queryExpressions.Add(string.Format(@"/{0}='{1}'", key, queryParams[key]));
            }
            search.Query = string.Join(" " + op + " ", queryExpressions);
            return search;
        }

        /// <summary>
        /// Finds Questions that modified at the course level
        /// </summary>
        /// <param name="items">Item list</param>
        /// <returns></returns>
        private List<string> FindQuestionToCopy(List<Item> items)
        {
            List<string> questionList = new List<string>();
            foreach (Item item in items)
            {
                var questionsEl = item.Data.XPathSelectElement("//questions");
                if (questionsEl != null)
                {
                    var questionEls = questionsEl.XPathSelectElements("//question");
                    foreach (var questionEl in questionEls)
                    {
                        var entityId = questionEl.Attribute("entityid");
                        var questionIdEl = questionEl.Attribute("id");
                        var questionId = questionIdEl != null ? questionIdEl.Value : null;
                        if (entityId == null && questionId != null)
                        {
                           if(!questionList.Contains(questionId))
                                questionList.Add(questionId.ToString());
                        }
                    }
                }
            }
            return questionList.Distinct().ToList();     
        }

        /// <summary>
        /// Get question list from question ids
        /// </summary>
        /// <param name="entityId">course id</param>
        /// <param name="questionIds">list of questionids</param>
        /// <returns></returns>
        private IEnumerable<Question> GetQuestionList(string entityId, List<string> questionIds)
        {
            var cmd = new GetQuestions();
            cmd.SearchParameters = new QuestionSearch();
            cmd.SearchParameters.EntityId = entityId;
            cmd.SearchParameters.QuestionIds = questionIds;            
            Session.ExecuteAsAdmin(Context.Environment, cmd);
            return cmd.Questions;            
        }


        /// <summary>
        /// put question list to dlap
        /// </summary>
        /// <param name="entityId">course id</param>
        /// <param name="questionIds">list of questions to create</param>
        /// <returns></returns>
        private void PutQuestionList(string entityId, List<Question> questions)
        {
            questions.ForEach(i => i.EntityId = entityId);  

            if (questions.Count == 0)
            {
                Console.WriteLine("No questions to process for : " + entityId);
                return;
            }

            var cmd = new PutQuestions();
            cmd.Add(questions);
            
            Session.ExecuteAsAdmin(Context.Environment, cmd);
            if (!cmd.Failures.IsNullOrEmpty())
            {
                foreach (var failure in cmd.Failures)
                {
                    Console.WriteLine("Failed putting question with reason {0}", failure.Reason);
                }
            }
        }

        /// <summary>
        /// Store Item to Course
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="items">Items to store</param>
        /// <returns></returns>
        private void StoreItems(string entityId,string destinationCourseId, List<Item> items)
        {
            items.ForEach(i => i.EntityId = destinationCourseId);
            if (items.Count == 0)
            {
                Console.WriteLine("No items to process for : " + destinationCourseId);
                return;
            }
            var cmd = new PutItems();
            cmd.Add(items);
            Session.ExecuteAsAdmin(Context.Environment, cmd);
            if (!cmd.Failures.IsNullOrEmpty())
            {
                foreach (var failure in cmd.Failures)
                {
                    Console.WriteLine("Failed putting item {0} with reason {1}", failure.ItemId, failure.Reason);
                }
            }
        }

        private static void ConfigureServiceLocator()
        {
            var locator = new UnityServiceLocator();
            locator.Container.AddNewExtensionIfNotPresent<EnterpriseLibraryCoreExtension>();
            locator.Container.AddNewExtensionIfNotPresent<LoggingBlockExtension>();
            ServiceLocator.SetLocatorProvider(() => locator);
        }

        private static ISessionManager EstablishConnection()
        {            
            var username = ConfigurationManager.AppSettings["user"];
            var userid = ConfigurationManager.AppSettings["userid"];
            var password = ConfigurationManager.AppSettings["password"];
            var sm = ServiceLocator.Current.GetInstance<ISessionManager>();
            sm.CurrentSession = sm.StartNewSession(username, password, true, string.Empty);
            return sm;
        }
    }
}
