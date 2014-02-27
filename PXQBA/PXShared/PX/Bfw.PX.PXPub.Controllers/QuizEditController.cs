using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Bfw.Agilix.Commands;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

using System.Text.RegularExpressions;
using System;
namespace Bfw.PX.PXPub.Controllers
{

    [PerfTraceFilter]
    public class QuizEditController : Controller
    {
        /// <summary>
        /// Access to the current business context information.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Access to an INavigationActions implementation.
        /// </summary>
        /// <value>
        /// The navigation actions.
        /// </value>
        protected BizSC.INavigationActions NavigationActions { get; set; }

        /// <summary>
        /// Access to an IContentActions implementation.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }

        /// <summary>
        /// Access to an IQuestionActions implementation.
        /// </summary>
        /// <value>
        /// The question actions.
        /// </value>
        protected BizSC.IQuestionActions QuestionActions { get; set; }

        /// <summary>
        /// Access to ISearchActions implementation
        /// </summary>
        protected BizSC.ISearchActions SearchActions { get; set; }

        /// <summary>
        /// The names of item types that should be displayed as quizzes.
        /// </summary>
        public static readonly string[] QuizTypeNames = new string[] { "assessment", "quiz", "homework" };

        /// <summary>
        /// Constructs a default TocWidgetController. Depends on a business context
        /// and user actions implementation.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="navigationActions">The navigation actions.</param>
        /// <param name="contentActions">The content actions.</param>
        public QuizEditController(BizSC.IBusinessContext context, BizSC.INavigationActions navigationActions, BizSC.IContentActions contentActions, BizSC.ISearchActions searchActions, BizSC.IQuestionActions questionActions)
        {
            Context = context;
            NavigationActions = navigationActions;
            ContentActions = contentActions;
            SearchActions = searchActions;
            QuestionActions = questionActions;
        }

        /// <summary>
        /// Returns the collection related to an item id
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public QuizCollectionType GetCollectionTypeFromItem(string itemId)
        {
            if (itemId == "PX_LOR")
            {
                return QuizCollectionType.ByPublisher;
            }
            else if (itemId == "PX_MY_QUESTIONS")
            {
                return QuizCollectionType.ByMe;
            }
            else if (itemId.StartsWith("MODULE_") || itemId == "PX_MULTIPART_LESSONS" || itemId == "PX_TOC")
            {
                return QuizCollectionType.InExistingAssessment;
            }
            else
            {
                return QuizCollectionType.All;
            }
        }


        /// <summary>
        /// Browses the main landing page of FNE.
        /// </summary>
        /// <returns></returns>
        public ActionResult BrowseMainPage(string collectionType, QuizBrowserMode mode = QuizBrowserMode.QuestionPicker)
        {
            var listCollection = new List<QuizCollection>();
            QuizCollectionType collectiontype = QuizCollectionType.All;
            Enum.TryParse(collectionType, true, out collectiontype);

            //Load LOR.
            if (collectiontype == QuizCollectionType.All || collectiontype == QuizCollectionType.ByPublisher)
            {
                var publisherCollection = GetCollection("PX_LOR", QuizCollectionType.ByPublisher, "");
                listCollection.Add(publisherCollection);
            }

            //Load My Question Library.            
            if (collectiontype == QuizCollectionType.All || collectiontype == QuizCollectionType.InExistingAssessment)
            {
                var navigationItem = NavigationActions.LoadNavigation(Context.EntityId, "PX_TOC", "");
                QuizCollection questionLibraryCollection =
                    GetCollection(navigationItem.Children.Count > 0 ? navigationItem.Children[0].Id : string.Empty
                                  , QuizCollectionType.InExistingAssessment, "PX_MY_QUESTIONS");

                //Func<string, object> convert = str =>
                //    {
                //        try
                //        {
                //            return int.Parse(str);
                //        }
                //        catch
                //        {
                //            return str;
                //        }
                //    };

                //questionLibraryCollection.Items =
                //    questionLibraryCollection.Items.OrderBy(
                //        i => Regex.Split(i.Title.Replace(" ", ""), "([0-9]+)").Select(convert),
                //        new EnumerableComparer<object>()).ToList();
                listCollection.Add(questionLibraryCollection);
            }
            if (collectiontype == QuizCollectionType.ByMe)
            {
                var myquestions = GetCollection("PX_MY_QUESTIONS", QuizCollectionType.ByMe, "");
                listCollection.Add(myquestions);
            }
            ViewData["mode"] = mode;
            ViewData["Title"] = string.Empty;
            if (Context.Course.QuestionFilter != null && Context.Course.QuestionFilter.FilterMetadata != null)
            {
                ViewData["CourseFilterMetadata"] = Context.Course.ToCourse().QuestionFilter.FilterMetadata;
            }
            return View("~/Views/Shared/DisplayTemplates/QuizPartials/BrowserPage.ascx", listCollection);
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="quizCollectionType">Type of the quiz collection.</param>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        private QuizCollection GetCollection(string itemId, QuizCollectionType quizCollectionType, string category)
        {
            IList<TocItem> toc = new List<TocItem>();
            if (!string.IsNullOrEmpty(itemId))
            {
                var navigationItem = NavigationActions.LoadNavigation(Context.EntityId, itemId, category);

                if (!navigationItem.Children.IsNullOrEmpty())
                {
                    foreach (BizDC.NavigationItem ni in navigationItem.Children)
                    {
                        TocItem tocItem = ni.ToTocItem();
                        tocItem.IsActive = true;
                        toc.Add(tocItem);
                    }
                }
            }

            QuizCollection collection = new QuizCollection();
            collection.CollectionType = quizCollectionType;
            collection.Items = toc;

            return collection;
        }
        /// <summary>
        /// Returns a list of questions created or edited by the user
        /// </summary>
        /// <returns></returns>
        public ActionResult UserCreatedOrEdited( QuizBrowserMode mode = QuizBrowserMode.Resources)
        {
            var searchResults = new AdvancedSearchResults();
            var query = new SearchQuery()
                {
                    ExactQuery = "(userCreated:(\"true\") OR publisherEdited:(\"true\"))",
                    ClassType = "question",
                    EntityId = Context.EntityId
                };

            SearchActions.DoProductSearch(false);
            var srs = SearchActions.DoSearch(query.ToSearchQuery(), Url, true);
            searchResults.Results = srs.ToSearchResults();

            var results = searchResults.Results;
            var questionIds = (results.docs.Count > 0) ? results.docs.Map(q => q.dlap_id.Split('|')[2]) : null;
            var questions = (questionIds != null) ? QuestionActions.GetQuestions(query.EntityId, questionIds).ToList() : null;
            var totalCount = questions == null ? 0 : questions.Count;
            const string startIndex = "0";
            var endIndex = totalCount.ToString();
            ViewData.Model = new Quiz()
                {
                    Questions = (questions != null) ? questions.Map(q => q.ToQuestion()).ToList() : null,
                    ShowReused = true,
                    CourseInfo = Context.Course.ToCourse(),
                    Title = "Questions I've created or edited",
                    QuizPaging =
                        new Paging()
                            {
                                StartIndex = startIndex,
                                LastIndex = totalCount.ToString(),
                                TotalCount = totalCount
                            }
                };

            
            
            SetBreadcrumbData("", startIndex, endIndex, mode, new BizDC.NavigationItem() { Name = "Questions I've created or edited", Id = "PX_MY_QUESTIONS", ParentId = "PX_ROOT" });

            return View("~/Views/Shared/DisplayTemplates/QuizPartials/QuestionList.ascx");
        }

        /// <summary>
        /// Returns a view that represents an expanded section comprised of the children of itemId.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="category">The category.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="lastIndex">The last index.</param>
        /// <returns></returns>
        public ActionResult ExpandSection(string id, string category, string startIndex, string lastIndex, QuizBrowserMode mode = QuizBrowserMode.QuestionPicker, string mainQuizId = "", string query = "")
        {
            if (category == null)
            {
                category = string.Empty;
            }
            ViewData["mainQuizId"] = mainQuizId;
            BizDC.NavigationItem navigationItem = null;
            if (!id.IsNullOrEmpty())
            {
                navigationItem = NavigationActions.LoadNavigation(Context.EntityId, id, category);
            }

            SetBreadcrumbData(category, startIndex, lastIndex, mode, navigationItem);
            if (navigationItem != null && QuizTypeNames.Contains(navigationItem.Type.ToLower()))
            {
                return GetQuizView(id, category, startIndex, lastIndex, mode, navigationItem);
            }
            else if (!query.IsNullOrEmpty())
            {
                return GetSearchResultsView(query, startIndex, lastIndex);
            }
            else
            {
                return GetQuestionBankView(id, category, startIndex, lastIndex, mode, navigationItem);
            }
           
        }

        private ActionResult GetQuestionBankView(string id, string category, string startIndex, string lastIndex,
            QuizBrowserMode mode, BizDC.NavigationItem navigationItem)
        {
            IList<TocItem> tempModel;
            if (id == "PX_TOC")
            {
                QuizCollection questionLibraryCollection =
                    GetCollection(navigationItem.Children.Count > 0 ? navigationItem.Children[0].Id : string.Empty
                        , QuizCollectionType.InExistingAssessment, "PX_MY_QUESTIONS");
                tempModel = questionLibraryCollection.Items;
            }
            else
            {
                IList<TocItem> toc = new List<TocItem>();
                if (!navigationItem.Children.IsNullOrEmpty())
                {
                    foreach (BizDC.NavigationItem ni in navigationItem.Children)
                    {
                        TocItem tocItem = ni.ToTocItem();
                        tocItem.IsActive = true;
                        toc.Add(tocItem);
                    }
                }

                tempModel = toc;
            }
            Func<string, object> convert = str =>
            {
                try
                {
                    return int.Parse(str);
                }
                catch
                {
                    return str;
                }
            };
            ViewData.Model = tempModel.OrderBy(i => Regex.Split(i.Title.Replace(" ", ""), "([0-9]+)").Select(convert),
                new EnumerableComparer<object>());

            return View("~/Views/Shared/DisplayTemplates/QuizPartials/ExpandSection.ascx");
        }

        private ActionResult GetQuizView(string id, string category, string startIndex, string lastIndex, QuizBrowserMode mode,
            BizDC.NavigationItem navigationItem)
        {
            BizDC.QuestionResultSet questionSet = QuestionActions.GetQuestions(Context.EntityId, navigationItem.Id, startIndex,
                lastIndex);

            //PX-3451 only show question with Status "available to instructor"
            // QuestionStatus = 1 means "Available to instructor" // Enum QuestionStatusType.AvailabletoInstructor
            //questionSet.Questions = questionSet.Questions.Filter(q => q.QuestionStatus == "1").ToList();
            //questionSet.TotalCount = questionSet.Questions.Count; 

            BizDC.QuestionResultSet newQuestionSet = new BizDC.QuestionResultSet();

            if (category.Trim() != "question-pool")
            {
                newQuestionSet.Questions = new List<BizDC.Question>();
                foreach (BizDC.Question question in questionSet.Questions)
                {
                    if (question.InteractionType != BizDC.InteractionType.Bank)
                    {
                        if (question.QuestionStatus.IsNullOrEmpty() || question.QuestionStatus == "1")
                        {
                            newQuestionSet.Questions.Add(question);
                        }
                    }
                    else
                    {
                        //we already have loaded the questions, why do we need to load again?
                        List<BizDC.Question> quizItemQuestions = null;
                        if (!question.Questions.IsNullOrEmpty())
                        {
                            quizItemQuestions = question.Questions.ToList();
                        }
                        if (quizItemQuestions.IsNullOrEmpty())
                        {
                            BizDC.ContentItem content = ContentActions.GetContent(Context.EntityId, question.Id);
                            quizItemQuestions =
                                QuestionActions.GetQuestions(Context.EntityId, content, true, Context.EntityId, startIndex, lastIndex)
                                    .Questions.ToList();
                        }

                        foreach (BizDC.Question q in quizItemQuestions)
                        {
                            if (q.QuestionStatus.IsNullOrEmpty() || q.QuestionStatus == "1")
                                newQuestionSet.Questions.Add(q);
                        }
                    }
                }
            }
            else
            {
                newQuestionSet.Questions =
                    (IList<BizDC.Question>)
                        questionSet.Questions.Where(q => q.QuestionStatus.IsNullOrEmpty() || q.QuestionStatus == "1")
                            .ToList();
            }
            newQuestionSet.TotalCount = questionSet.TotalCount;

           
            
            ViewData.Model = new Quiz()
            {
                Questions = newQuestionSet.Questions.Map(q => q.ToQuestion()).ToList(),
                //.OrderBy(i => Regex.Split(i.PreviewText.Replace(" ", ""), "([0-9]+)").Select(convert), new EnumerableComparer<object>())
                Title = navigationItem.Name,
                ShowReused = category.Trim() == "question-pool" ? false : true,
                QuizPaging = category.Trim() == "question-pool"
                    ? null
                    : new Paging() {StartIndex = startIndex, LastIndex = lastIndex, TotalCount = newQuestionSet.TotalCount},
                CourseInfo = Context.Course.ToCourse(),
                Id = id
            };
            
            return View("~/Views/Shared/DisplayTemplates/QuizPartials/QuestionList.ascx");
        }

        private ActionResult GetSearchResultsView(string queryString, string startIndex, string lastIndex)
        {
            int start, end = 0;
            int.TryParse(startIndex, out start);
            int.TryParse(lastIndex, out end);
            var rows = Math.Max(end - start, 0);

            var query = new BizDC.SearchQuery()
            {
                IncludeWords = queryString,
                Start = start,
                Rows = rows,
            };
            var entityId = QuestionActions.GetQuestionRepositoryCourse(Context.EntityId);

            int numFound = 0;
            var searchResults = SearchActions.DoQuestionSearch(query, entityId, out numFound);

            ViewData.Model = new QuizSearchResults()
            {
                Query = query.ToSearchQuery(),
                Quiz = new Quiz()
                {
                    Questions = (searchResults != null) ? searchResults.Map(q => q.ToQuestion()).ToList() : null,
                    ShowReused = true,
                    CourseInfo = Context.Course.ToCourse(),
                    QuizPaging =
                        new Paging()
                        {
                            StartIndex = query.Start.ToString(),
                            LastIndex = query.Rows.ToString(),
                            TotalCount = numFound
                        }
                }
            };
            
            return View("DisplayTemplates/QuizPartials/SearchQuiz");
        }

        private void SetBreadcrumbData(string category, string startIndex, string lastIndex, QuizBrowserMode mode,
                                       BizDC.NavigationItem navigationItem, bool isRecursive = false)
        {
            ViewData["mode"] = mode;
            if (navigationItem == null)
            {
                return;
            }
            var itemCollection = GetCollectionTypeFromItem(navigationItem.Id);
            
            if (!isRecursive)
            {
                ViewData["Title"] = navigationItem.Name;
            }
            if (itemCollection != QuizCollectionType.All)
            {
                if (!isRecursive)
                {
                    ViewData["Title"] = itemCollection.GetDescription();

                }
                return;
            }


            if (navigationItem.ParentId.StartsWith("MODULE_"))
            {
                navigationItem.ParentId = "PX_TOC";
            }
            if (navigationItem.ParentId != "PX_CONTENT" && navigationItem.ParentId != "PX_ROOT")
            {

                var parent = NavigationActions.LoadNavigation(Context.EntityId, navigationItem.ParentId, category);
                if (parent != null && parent.Id != navigationItem.Id)
                {
                    SetBreadcrumbData(category, startIndex, lastIndex, mode, parent, true);
                    var collectionType = GetCollectionTypeFromItem(parent.Id);
                    var parentTitle = collectionType != QuizCollectionType.All
                                          ? collectionType.GetDescription()
                                          : parent.Name;
                    if (ViewData["BreadcrumbData"] == null)
                    {
                        ViewData["BreadcrumbData"] = new List<BreadcrumbData>();
                    }
                    ((IList<BreadcrumbData>)ViewData["BreadcrumbData"]).Add(new BreadcrumbData
                        {
                            Title = parentTitle,
                            Action = "ExpandSection",
                            Controller = "QuizEdit",
                            RouteValues = new RouteValueDictionary(new { parent.Id, category, startIndex, lastIndex, mode })
                        });

                    //ViewData["BreadcrumbParent"] = parentTitle;
                    //ViewData["BreadcrumbAction"] = "ExpandSection";
                    //ViewData["BreadcrumbController"] = "QuizEdit";

                    //ViewData["BreadcrumbRoute"] =
                    //    new RouteValueDictionary(new { parent.Id, category, startIndex, lastIndex, mode });
                }
            }
        }

        private class LowercaseEqualityComparer : IEqualityComparer<string>
        {
            #region IEqualityComparer<string> Members

            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            /// <param name="x">The first object of type <paramref name="T"/> to compare.</param>
            /// <param name="y">The second object of type <paramref name="T"/> to compare.</param>
            /// <returns>
            /// true if the specified objects are equal; otherwise, false.
            /// </returns>
            public bool Equals(string x, string y)
            {
                return x.ToLower().Equals(y.ToLower());
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <param name="s">The s.</param>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public int GetHashCode(string s)
            {
                return s.ToLower().GetHashCode();
            }

            #endregion
        }
    }
}