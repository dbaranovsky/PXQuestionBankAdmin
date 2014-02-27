
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.Common.Pagination;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Controllers.Admin;
using Bfw.PX.PXPub.Controllers.Helpers;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizQuestion = Bfw.PX.Biz.DataContracts.Question;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using ContentItem = Bfw.PX.Biz.DataContracts.ContentItem;
using ModelQuestion = Bfw.PX.PXPub.Models.Question;
using Bfw.PX.Biz.Direct.Services;
using PxWebUser;
namespace Bfw.PX.PXPub.Controllers
{
    [PerfTraceFilter]
    public class QuestionAdminController : Controller
    {


        #region Variables Declaration:
        //***********************************************************************   
        private readonly string mainEntityId;

        /// <summary>
        /// Access to the current business context information.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Access to an IUserActions implementation.
        /// </summary>
        /// <value>
        /// The user actions.
        /// </value>
        protected BizSC.IUserActions UserActions { get; set; }

        protected BizSC.ICourseActions CourseActions { get; set; }

        protected BizSC.IQuestionAdminActions QuestionAdminActions { get; set; }

        protected BizSC.IContentActions ContentActions { get; set; }

        protected BizSC.IQuestionActions QuestionActions { get; set; }

        protected Dictionary<string, string> QuestionTypes { get; set; }

        /// <summary>
        /// Access to a content helper object.
        /// </summary>
        /// <value>
        /// The content helper.
        /// </value>
        protected ContentHelper ContentHelper { get; set; }
        /// <summary>
        /// Gets or sets the search actions.
        /// </summary>
        /// <value>
        /// The search actions.
        /// </value>
        protected BizSC.ISearchActions SearchActions { get; set; }

        protected string Cont_ChapterList = "chapterList";
        protected string Cont_QuizList = "quizList";
        protected string Cont_HTS = "HTS";
        protected string Cont_ECON_GRAPH = "FMA_GRAPH";

        #endregion


        /// <summary>
        /// Constructs a default AccountWidgetController. Depends on a business context
        /// and user actions implementation.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="questionAdminActions"> </param>
        /// <param name="searchActions"> </param>
        /// <param name="contentActions"> </param>
        public QuestionAdminController(BizSC.IBusinessContext context, BizSC.IQuestionAdminActions questionAdminActions, BizSC.ISearchActions searchActions, BizSC.IContentActions contentActions, BizSC.IQuestionActions questionActions, ContentHelper contentHelper)
        {
            Context = context;
            QuestionAdminActions = questionAdminActions;
            SearchActions = searchActions;
            ContentActions = contentActions;
            QuestionActions = questionActions;
            ContentHelper = contentHelper;
            mainEntityId = string.Empty;

            string questionCourseId = QuestionActions.GetQuestionRepositoryCourse(Context.EntityId);

            mainEntityId = !string.IsNullOrEmpty(questionCourseId) ? questionCourseId : Context.EntityId;

            QuestionTypes = Context.GetQuestionTypes();
        }


        #region Dina's Actions
        //***********************************************************************
        /// <summary/>
        /// Get Index Page View
        /// <param name="questionId"> </param>
        /// <param name="quizId"> </param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index(string questionId = null, string quizId = null)
        {
            if (Context.CurrentUser != null && Context.CurrentUser.WebRights == null)
            {
                //Added Web User Rights for this Course:
                Context.CurrentUser.WebRights = new PxWebUserRights(Context.CurrentUser.Username, Context.ProductCourseId);
            }

            if (Context.CurrentUser.WebRights != null && Context.CurrentUser.WebRights.QuestionBank != null
                && Context.CurrentUser.WebRights.QuestionBank.ShowQuestionBankManager)
            {
                ViewData["CourseTitle"] = Context.Course.Title;

                if (String.IsNullOrEmpty(questionId) && string.IsNullOrEmpty(quizId)) return View();

                QuestionAdminSearchPanel searchPanelModel = new QuestionAdminSearchPanel()
                {
                    SelectedQuestionId = questionId,
                    QuizSelectedValues = new List<string> { quizId },
                    SelectedQuizId = quizId
                };

                return View(searchPanelModel);
            }
            else
            {
                return RedirectToRoute("EcomEntitled");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchPanelModel"></param>
        /// <returns></returns>
        public ActionResult SearchPanel(QuestionAdminSearchPanel searchPanelModel)
        {
            if (searchPanelModel == null) searchPanelModel = new QuestionAdminSearchPanel();


            QuestionAdminHelper.PopulateFormatList(searchPanelModel, QuestionTypes);


            QuestionAdminHelper.PopulateChaptersList(searchPanelModel, Context.EntityId, QuestionActions);

            QuestionAdminHelper.PopulateQuizzesList(searchPanelModel, Context.EntityId, QuestionActions);


            Context.CacheProvider.InvalidateQBAQuestionData(Context.EntityId);
            var quizList = searchPanelModel.QuizSelectList.ToDictionary(item => item.Value, item => item.Text);
            var chapterList = searchPanelModel.ChapterSelectList.ToDictionary(item => item.Value, item => item.Text);
            Context.CacheProvider.StoreQBASelectionList(Context.EntityId, quizList, Cont_QuizList);
            Context.CacheProvider.StoreQBASelectionList(Context.EntityId, chapterList, Cont_ChapterList);


            QuestionAdminHelper.PopulateStatusList(searchPanelModel);


            if (string.IsNullOrEmpty(searchPanelModel.SearchKeyword)) searchPanelModel.SearchKeyword = "enter search keywords";

            return View(searchPanelModel);
        }


        /// <summary>
        /// Return Search Result
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult SearchResult(QuestionAdminSearchPanel searchModel)
        {
            if (QuestionAdminHelper.IsSearchConditionsSetUp(searchModel)) searchModel = GetQuestionsResult(searchModel);

            return View(searchModel);
        }


        /// <summary>
        /// Searches the questions.
        /// </summary>
        /// <param name="searchModel"> </param>
        /// <returns></returns>    	
        [HttpPost]
        [AjaxMethod]
        [ActionName("SearchResult")]
        public ActionResult SearchQuestions(QuestionAdminSearchPanel searchModel)
        {
            searchModel = GetQuestionsResult(searchModel);
            return View(searchModel);
        }

        [HttpGet]
        public ActionResult StyleQuestions()
        {

            return View();
        }

        [HttpPost]
        public ActionResult GetStyleQuestions(string id = "1", string query = "style")
        {
            int questionCount = 0;

            //check for filter inputs
            string queryQuestonsFilter = "";// QuestionAdminHelper.GetSearchQuestonsFilter(searchModel);

            var searchModel = new QuestionAdminSearchPanel();
            //searchModel.ChapterSelectedValues.Add(id);

            var chapters = QuestionAdminHelper.PopulateChaptersList(searchModel, Context.EntityId, QuestionActions);
            int chapterId = Convert.ToInt32(id);
            searchModel.ChapterSelectedValues.Clear();
            searchModel.ChapterSelectedValues.Add(chapters[chapterId].Id);

            List<ContentItem> quizzes =
                QuestionAdminHelper.GetQuizzes(searchModel, Context.EntityId, ContentActions, QuestionActions).ToList();


            IEnumerable<ModelQuestion> questionsWithQuizzes = null;

            questionsWithQuizzes = SearchByChapter(searchModel, quizzes);

            var styleQuestions = questionsWithQuizzes.Where(q => q.CustomUrl == "HTS" && !String.IsNullOrEmpty(q.InteractionData) && q.InteractionData.Contains("<style"));



            return View(styleQuestions);
        }


        private QuestionAdminSearchPanel GetQuestionsResult(QuestionAdminSearchPanel searchModel)
        {

            if (searchModel.FormatSelectedValues.Count > 0)
            {
                List<string> chk = new List<string>() { Cont_HTS, Cont_ECON_GRAPH };
                for (var i = 0; i < searchModel.FormatSelectedValues.Count; i++)
                {
                    if (chk.Contains(searchModel.FormatSelectedValues[i].ToUpperInvariant()))
                    {
                        searchModel.FormatSelectedValues[i] = "custom";
                    }
                }
                searchModel.FormatSelectedValues = searchModel.FormatSelectedValues.Distinct().ToList();
            }
            //check for filter inputs
            string queryQuestonsFilter = QuestionAdminHelper.GetSearchQuestonsFilter(searchModel);

            List<ContentItem> quizzes =
                QuestionAdminHelper.GetQuizzes(searchModel, Context.EntityId, ContentActions, QuestionActions).ToList();

            var quizIds = quizzes.Select(q => q.Id).ToList();
            IEnumerable<ModelQuestion> questionsWithQuizzes = null;

            if (searchModel.SearchKeyword.ToLower() == "enter search keywords")
                searchModel.SearchKeyword = string.Empty;

            //Different combinations of search Parameters drive different search scenarios:
            //Scenario #1 Quizzes Selected or Scenario #2 Chapters Selected
            if ((searchModel.QuizSelectedValues != null && searchModel.QuizSelectedValues.All(i => i != "0"))
              || (searchModel.ChapterSelectedValues != null && searchModel.ChapterSelectedValues.All(i => i != "0")))
            {
                questionsWithQuizzes = SearchByChapter(searchModel, quizzes);
            }
            //Scenario #3 Only Filters Selected / key words:
            else if (!String.IsNullOrEmpty(queryQuestonsFilter) || !string.IsNullOrEmpty(searchModel.SearchKeyword))
            {
                questionsWithQuizzes = SearchByKeyword(searchModel, quizzes);
            }

            if (questionsWithQuizzes == null || !questionsWithQuizzes.Any())
            {
                ViewData["NothingFound"] = true;
                ViewData["TotalCount"] = null;
                ViewData["SearchModel"] = searchModel;
                searchModel.Quiz = null;
            }
            else
            {
                int totalCount = questionsWithQuizzes.Count();

                questionsWithQuizzes = QuestionAdminHelper.CreatePaginationForItemsToView(searchModel, questionsWithQuizzes,
                                                                                          totalCount,
                                                                                          pageSubmitFunction:
                                                                                            "PxQuestionAdmin.submitPagination");

                Quiz combinedQuizResult = new Quiz { Questions = questionsWithQuizzes.ToList() };




                ViewData["TotalCount"] = totalCount;
                ViewData["SearchModel"] = searchModel;
                searchModel.Quiz = combinedQuizResult;

                //getting lists from the cache to Assign proper Names to Question Bank / ebook Chapter fields
                var QuizList = Context.CacheProvider.FetchQBAQuestions(Context.EntityId, Cont_QuizList) as Dictionary<string, string>;
                var chapterList = Context.CacheProvider.FetchQBAQuestions(Context.EntityId, Cont_ChapterList) as Dictionary<string, string>;

                if (QuizList == null || chapterList == null)
                {
                    var searchPanelModel = new QuestionAdminSearchPanel();
                    var temp = QuestionAdminHelper.PopulateChaptersList(searchPanelModel, Context.EntityId, QuestionActions);
                    chapterList = searchPanelModel.ChapterSelectList.ToDictionary(x => x.Value, x => x.Text);
                    QuestionAdminHelper.PopulateQuizzesList(searchPanelModel, Context.EntityId, QuestionActions);
                    QuizList = searchPanelModel.QuizSelectList.ToDictionary(x => x.Value, x => x.Text);
                    //re-insert into cache 
                    Context.CacheProvider.InvalidateQBAQuestionData(Context.EntityId);
                    Context.CacheProvider.StoreQBASelectionList(Context.EntityId, QuizList, Cont_QuizList);
                    Context.CacheProvider.StoreQBASelectionList(Context.EntityId, chapterList, Cont_ChapterList);
                }


                foreach (var Qt in searchModel.Quiz.Questions)
                {
                    foreach (var i in QuizList)
                    {

                        if (Qt.AssignedQuizes.Any(s => s.Id.Equals(i.Key.ToString())))
                            Qt.QuestionBank = string.Join(",", i.Value.ToString());
                    }

                    foreach (var k in chapterList)
                    {
                        if (Qt.AssignedChapter == k.Key)
                            Qt.AssignedChapter = k.Value;
                    }

                }

                //QuestionAdminHelper.PopulateFormatList(searchModel, QuestionTypes);

            }
            return searchModel;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ModelQuestion> SearchByChapter(QuestionAdminSearchPanel searchModel, List<ContentItem> quizzes)
        {
            IEnumerable<ModelQuestion> questionsWithQuizzes = null;

            var bizQuestionsWithQuizzes = QuestionAdminActions.GetQuestionsForSelectedQuizzes(mainEntityId, quizzes);
            var questions = bizQuestionsWithQuizzes.Select(q => q.ToQuestion());

            var distinctQuestions = questions.Distinct().ToList();

            questionsWithQuizzes = QuestionAdminHelper.ApplySearchFilter(searchModel, distinctQuestions, QuestionTypes);


            return questionsWithQuizzes;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ModelQuestion> SearchByKeyword(QuestionAdminSearchPanel searchModel, List<ContentItem> quizzes)
        {
            IEnumerable<ModelQuestion> questionsWithQuizzes = null;
            string queryQuestonsFilter = QuestionAdminHelper.GetSearchQuestonsFilter(searchModel);

            var AdvanceQuestions = new List<string>() { };


            using (Context.Tracer.StartTrace("QBA Keywork search"))
            {

                IEnumerable<QuestionAdmin> questionsAdminList = QuestionAdminHelper.GetQuestionsAdminList(quizzes);
                IEnumerable<string> questionIds = QuestionAdminHelper.KeywordSearch(searchModel, mainEntityId, SearchActions);

                //Debug.WriteLine(questionIds.Count());

                questionIds = (from q in questionsAdminList
                               where questionIds.Contains(q.QuestionId)
                               select q.QuestionId).ToList().Distinct();

                using (Context.Tracer.StartTrace(string.Format("QBA Keywork search - Results Found {0}", questionIds.Count())))
                {

                    if (questionIds.Any())
                    {
                        int rowCount = 1000;
                        if (System.Configuration.ConfigurationManager.AppSettings["QuestionSearchMaxRowCount"] != null)
                        {
                            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["QuestionSearchMaxRowCount"], out rowCount);
                        }
                        questionIds = questionIds.Take(rowCount);

                        IEnumerable<ModelQuestion> questionsToView = QuestionAdminHelper.GetQuestionsListToView(mainEntityId, questionIds, null, null, ContentActions, QuestionActions);

                        if (questionsToView != null)
                        {
                            questionsWithQuizzes = QuestionAdminHelper.AddAssignedQuizzes(questionsAdminList, questionsToView.ToList());
                            questionsWithQuizzes = questionsWithQuizzes.Where(q => q.AssignedQuizes.Any());
                        }

                        if (searchModel.SelectedCustomQuestions.Count > 0)
                        {
                            questionsWithQuizzes = questionsWithQuizzes.Where(item => item.CustomUrl == null || searchModel.SelectedCustomQuestions.Contains(item.CustomUrl));
                        }
                    }
                }
            }







            return questionsWithQuizzes;




        }
        /// <summary>
        /// Returns List of Quizzes for List of Selected Chapters
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [AjaxMethod]
        [ActionName("SelectChapter")]
        public ActionResult SelectChapter(object data)
        {
            IEnumerable<ContentItem> quizzes;
            List<string> selectedValues;

            string[] chapters = data as string[];

            if (data == null || chapters == null)
            {
                quizzes = QuestionActions.GetCourseQuizzes(Context.EntityId);
                selectedValues = null;
            }
            else
            {
                chapters = chapters.First().Split("&".ToCharArray());
                selectedValues = chapters.Select(ch => String.Format("{0}", ch.Replace("ChapterSelectedValues=", ""))).ToList();

                if (selectedValues[0] == "0")
                {
                    quizzes = QuestionActions.GetCourseQuizzes(Context.EntityId);
                    selectedValues = null;
                }
                else
                {
                    quizzes = QuestionActions.GetQuizzesForSelectedChapters(Context.EntityId, selectedValues);
                }
            }

            IEnumerable<ContentItem> selectedChapters = selectedValues == null ? QuestionActions.GetCourseChapters(Context.EntityId) :
                                                                               ContentActions.GetItems(Context.EntityId, selectedValues, includingShortCuts: true);

            var quizzesToDisplay = QuestionAdminHelper.GetQuizzesWithDescription(selectedChapters, quizzes.ToList());
            IEnumerable<SelectListItem> quizSelectList = null;
            if (quizzesToDisplay != null)
            {
                quizSelectList = quizzesToDisplay.Select(item => new SelectListItem { Text = item.Title, Value = item.Id, Selected = false });
                //quizSelectList = quizSelectList.Add("Any", "0");
            }
            QuestionAdminSearchPanel model = new QuestionAdminSearchPanel { ChapterSelectedValues = selectedValues, QuizSelectList = quizSelectList };


            return PartialView("QuizListDropDown", model);

        }

        /// <summary>
        /// QuizzesTab Shows All Quizzes where Selected Question exists:
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="quizId"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        public ActionResult QuizzesTab(string questionId, string quizId, string currentPage)
        {
            List<BizDC.ContentItem> allChapters = QuestionActions.GetCourseChapters(Context.EntityId);
            IEnumerable<ContentItem> allQuizzes = QuestionActions.GetCourseQuizzes(Context.EntityId);

            IEnumerable<ContentItem> quizzes = allQuizzes.Where(z => z.QuizQuestions.Any(q => q.QuestionId == questionId));

            var quizzesToView = QuestionAdminHelper.GetQuizzesWithDescription(allChapters, quizzes.ToList());

            QuestionAdminSearchPanel searchPanel = new QuestionAdminSearchPanel
            {
                Pagination = new Pagination { CurrentPage = (!String.IsNullOrEmpty(currentPage)) ? int.Parse(currentPage) : 1 }
            };

            IEnumerable<ContentItem> quizzesToDisplay = new List<ContentItem>();
            if (quizzesToView != null && quizzesToView.Count() > 0)
                quizzesToDisplay = QuestionAdminHelper.CreatePaginationForItemsToView(searchPanel, quizzesToView, quizzesToView.Count(),
                                                                                                                   pageSubmitFunction: "submitQuizPagination");
            searchPanel.SelectedQuestionId = questionId;

            searchPanel.SelectedQuizId = quizId;

            searchPanel.QuizSelectList = quizzesToDisplay.Select(item =>
                                                             new SelectListItem
                                                             {
                                                                 Selected = true,
                                                                 Text = item.Title,
                                                                 Value = item.Id
                                                             });

            searchPanel.QuizSelectedValues = quizzesToDisplay.Select(qz => qz.Id).ToList();

            return View(searchPanel);
        }

        public JsonResult DuplicateQuestion(string entityid, string questionId, string quizId)
        {
            BizDC.Question questionToCopy = new BizQuestion();
            questionToCopy = QuestionActions.GetQuestion(entityid, questionId);

            if (questionToCopy != null)
            {
                questionToCopy.Id = Guid.NewGuid().ToString();
                questionToCopy.Title = String.Format("{0} (copy)", questionToCopy.Title);
                questionToCopy.QuestionStatus = "0";
                if (questionToCopy.QuestionBank == null)
                {
                    questionToCopy.QuestionBank = quizId;
                }
                QuestionActions.StoreQuestion(questionToCopy);

                ModelQuestion modelQuestion = questionToCopy.ToQuestion();
                this.UpdateQuizWithNewQuestion(quizId: quizId, question: modelQuestion);

                return Json(new { questionToCopy });
            }
            return Json(new { status = "false" });
        }


        #endregion Dina's Actions


        #region Mitul's actions:

        /// <summary>
        /// Question Editor
        /// </summary>
        /// <param name="Id">questionId</param>
        /// <param name="quizId">quizId</param>       
        public ActionResult QuestionEditor(string id, string quizId, string chapter)
        {
            //add chapter id to the URL

            string questionId = id;
            ViewData["CourseTitle"] = Context.Course.Title;
            ViewData["questionId"] = questionId;
            ViewData["quizId"] = quizId;
            ViewData["chapter"] = chapter;
            BizQuestion question = QuestionActions.GetQuestion(mainEntityId, questionId);

            if (question.AdminFlag) ViewData["AdminFlag"] = question.AdminFlag;

            ViewData["questionType"] = question.InteractionType.ToString().ToLowerInvariant();
            ViewData["questionCustomUrl"] = question.CustomUrl;
            ViewData["questionEntityId"] = question.EntityId;
            ViewData["previewEntityId"] = Context.EntityId;

            return View();
        }

        /// <summary>
        /// Question Editor Tab
        /// </summary>
        /// <param name="questionId">questionId</param>
        /// <param name="quizId">quizId</param>
        public ActionResult QuestionEditorTab(string questionId, string quizId)
        {
            try
            {
                var mockQuizQuestionId = "PxTempQBAQuestion_" + Context.EnrollmentId;
                var mockQuizId = "PxTempQBAQuiz_" + Context.EnrollmentId;
                string changedQuestionId;

                var r = QuestionActions.StoreMockQuiz(
                    sourceEntityId: mainEntityId,
                    destinationEntityId: Context.EntityId,
                    sourceQuestionId: questionId,
                    mockQuizId: mockQuizId,
                    mockQuestionId: mockQuizQuestionId,
                    changedQuestionId: out changedQuestionId);

                BizQuestion question = QuestionActions.GetQuestion(mainEntityId, changedQuestionId);//(Context.EntityId, changedQuestionId);

                if (question == null) return Json(new { Status = "error" }, JsonRequestBehavior.AllowGet);


                string instructorPermissionFlags = System.Configuration.ConfigurationManager.AppSettings["InstructorPermissionFlags"];
                string enrollmentId = QuestionAdminActions.GetAnonymousUserEnrollmentId(instructorPermissionFlags, mainEntityId);

                QuestionEditor model = new QuestionEditor
                {
                    QuestionId = changedQuestionId,
                    QuizId = mockQuizId,
                    EnrollmentId = //Context.EnrollmentId,
                    !string.IsNullOrEmpty(enrollmentId) ? enrollmentId : Context.EnrollmentId,
                    EntityId = question.EntityId,
                    QuestionType = question.InteractionType.ToString().ToLower(),
                    ShowAdvanced = false,
                    ShowSave = false,
                    ShowCancel = false,
                    ShowProperties = false,
                    ShowFeedback = true,
                    CustomXML = question.InteractionData
                };

                if (question.InteractionType == BizDC.InteractionType.Custom)
                {
                    model.CustomUrl = question.CustomUrl.ToUpper();
                    if (question.CustomUrl.ToUpper() == "HTS")
                    {
                        model.HtsPlayerUrl = Context.Domain.CustomQuestionUrls["HTS"];
                        model.IsAdvancedConvert = false;
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return Json(new { Status = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Preview Tab
        /// </summary>
        /// <param name="questionId">questionId</param>
        /// <param name="quizId">quizId</param>
        public ActionResult PreviewTab(string questionId, string quizId)
        {
            try
            {
                if (string.IsNullOrEmpty(questionId)) return Json(new { Status = "error" }, JsonRequestBehavior.AllowGet);
                //remove the question from cache.
                var questions = new List<BizDC.Question>(){
                                                 new BizDC.Question()
                                                    {
                                                        Id = questionId,
                                                        EntityId = mainEntityId
                                                    }
                                            };
                QuestionActions.RemoveQuestionsFromCache(questions);
                Models.Question model = QuestionActions.GetQuestion(mainEntityId, questionId).ToQuestion();
                if (model == null) return Json(new { Status = "error" }, JsonRequestBehavior.AllowGet);
                return View(model);
            }
            catch (Exception ex)
            {
                return Json(new { Status = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Notes Tab
        /// </summary>
        /// <param name="questionId">questionId</param>
        /// <param name="quizId">quizId</param>
        public ActionResult NotesTab(string questionId, string quizId)
        {
            try
            {
                ViewData["questionId"] = questionId;
                ViewData["quizId"] = quizId;

                if (string.IsNullOrEmpty(questionId)) return Json(new { Status = "error" }, JsonRequestBehavior.AllowGet);

                var QuestionNoteList = QuestionAdminActions.GetQuestionNotes(questionId);
                QuestionNotes model = new QuestionNotes { NoteList = QuestionNoteList.Map(e => e.ToQuestionNote()).ToList() };

                return View(model);
            }
            catch (Exception ex)
            {
                return Json(new { Status = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// History Tab
        /// </summary>
        /// <param name="questionId">questionId</param>
        /// <param name="quizId">quizId</param>
        public ActionResult HistoryTab(string questionId, string quizId)
        {
            try
            {
                ViewData["questionId"] = questionId;
                ViewData["quizId"] = quizId;

                if (string.IsNullOrEmpty(questionId)) return Json(new { Status = "error" }, JsonRequestBehavior.AllowGet);
                var Qlogs = QuestionAdminActions.GetQuestionAllVersions(mainEntityId, questionId, true);
                var QuestionLogList = QuestionAdminActions.GetQuestionLogs(questionId);
                QuestionLogs model = new QuestionLogs { LogList = QuestionLogList.Map(e => e.ToQuestionNote()).ToList() };
                return View(model);
            }
            catch (Exception ex)
            {
                return Json(new { Status = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// History Tab
        /// </summary>
        /// <param name="questionId">questionId</param>
        /// <param name="quizId">quizId</param>
        public ActionResult MetadataTab(string questionId, string quizId, string chapter)
        {
            //try
            //{
            Models.Question question = QuestionActions.GetQuestion(mainEntityId, questionId).ToQuestion();
            QuestionMetadata model = new QuestionMetadata();
            List<string> selectedValues = new List<string>();
            if (question.QuestionBank == null)
            {
                selectedValues.Add(quizId);
            }
            else
            {
                selectedValues.Add(question.QuestionBank);

            }

            if (string.IsNullOrEmpty(questionId)) return Json(new { Status = "error" }, JsonRequestBehavior.AllowGet);
            //remove the question from cache.
            var questions = new List<BizDC.Question>(){
                                                 new BizDC.Question()
                                                    {  
                                                        Id = questionId,
                                                        EntityId = mainEntityId
                                                    }
                                            };
            QuestionActions.RemoveQuestionsFromCache(questions);


            List<ContentItem> currentQuiz = new List<ContentItem>();

            currentQuiz = ContentActions.GetItems(Context.EntityId, selectedValues).ToList();
            List<string> selectedChapter = new List<string>();
            selectedChapter.Add(currentQuiz.FirstOrDefault().ParentId);
            var quizzes = QuestionActions.GetQuizzesForSelectedChapters(Context.EntityId, selectedChapter);


            IEnumerable<ContentItem> selectedChapters = selectedChapter == null ? QuestionActions.GetCourseChapters(Context.EntityId) :
                                                                               ContentActions.GetItems(Context.EntityId, selectedChapter, includingShortCuts: true);

            var quizzesToDisplay = QuestionAdminHelper.GetQuizzesWithDescription(selectedChapters, quizzes.ToList());

            IEnumerable<SelectListItem> quizSelectList = quizzesToDisplay.Select(item => new SelectListItem { Text = item.Title, Value = item.Id });

            QuestionAdminSearchPanel searchPanelModel = new QuestionAdminSearchPanel()
            {
                SelectedQuestionId = questionId,
                SelectedQuizId = quizId,
                ChapterSelectedValues = selectedChapter,
                QuizSelectList = quizSelectList
            };
            model.Metadata = searchPanelModel;

            QuestionAdminHelper.PopulateFormatList(searchPanelModel, QuestionTypes);
            QuestionAdminHelper.PopulateChaptersList(searchPanelModel, Context.EntityId, QuestionActions);
            QuestionAdminHelper.PopulateStatusList(searchPanelModel);

            var quizList = searchPanelModel.QuizSelectList.ToDictionary(item => item.Value, item => item.Text);

            if (question == null) return Json(new { Status = "error" }, JsonRequestBehavior.AllowGet);

            model.QuestionData = question;
            if (Context.Product.QuestionCardData != null)
            {
                model.QuestionCardData = AddSelectListToQuestionCardData(Context.Product.QuestionCardData.Map(i => i.ToQuestionCard()).ToList(), model.QuestionData);
            }
            if (model.QuestionData != null && model.QuestionData.eBookChapter == null)
            {

                model.QuestionData.eBookChapter = chapter;

            }

            if (model.QuestionData != null && model.QuestionData.QuestionBank == null)
            {

                model.QuestionData.QuestionBank = quizId;

            }


            if (model.QuestionBankText == null && model.UsedIn == null)
            {
                model.QuestionBankText = quizList.Filter(x => x.Key == quizId).SingleOrDefault().Value.ToString();
            }


            model.SelectedLearningObjectives = CreateSelectList(question.SelectedLearningObjectives);
            model.SelectedSuggestUse = CreateSelectList(question.SelectedSuggestedUse);

            model.QuestionCardData = InitializeDropDowns(model.QuestionCardData);

            model.QuestionStatus = question.QuestionStatus;

            return View(model);
            //}
            //catch (Exception ex)
            //{
            //    return Json(new { Status = ex.Message }, JsonRequestBehavior.AllowGet);
            //}
        }

        private static List<Bfw.PX.PXPub.Models.QuestionCardData> InitializeDropDowns(List<Bfw.PX.PXPub.Models.QuestionCardData> QList)
        {
            var defaultText = " Please Select";
            foreach (var k in QList)
            {
                if (!k.QuestionValues.Contains(defaultText))
                {
                    k.QuestionValues.Add(defaultText);
                    k.SelectedQuestionValues = k.SelectedQuestionValues.Add(defaultText, "0", MvcSelectListHelper.ListPosition.First);
                }
            }
            return QList;
        }

        public static IEnumerable<SelectListItem> CreateSelectList(Dictionary<string, string> questioncard)
        {
            List<SelectListItem> newSelectList = new List<SelectListItem>();

            if (questioncard != null)
            {
                //newSelectList.Add(new SelectListItem() { Text = "Please Select", Value = "0" });
                foreach (KeyValuePair<string, string> kvp in questioncard)
                {
                    newSelectList.Add(new SelectListItem() { Text = kvp.Value, Value = kvp.Key });
                }
            }

            return newSelectList;
        }


        public static List<Models.QuestionCardData> AddSelectListToQuestionCardData(List<Models.QuestionCardData> questioncard, Models.Question questionData)
        {
            List<Models.QuestionCardData> questioncardList = new List<Models.QuestionCardData>();
            foreach (Models.QuestionCardData qcd in questioncard)
            {

                if (qcd.FriendlyName != null && (qcd.QuestionValues != null || qcd.QuestionValues.Count() > 0))
                {
                    qcd.SelectedQuestionValues = new List<SelectListItem>();

                    foreach (string s in qcd.QuestionValues)
                    {
                        if (s.Contains('|'))
                        {
                            var result = s.Split('|');
                            qcd.SelectedQuestionValues = qcd.SelectedQuestionValues.Add(result[1], result[0]);
                        }
                        else
                        {
                            qcd.SelectedQuestionValues = qcd.SelectedQuestionValues.Add(s, s);
                        }

                    }
                    questioncardList.Add(qcd);
                }

            }

            return questioncardList;
        }
        /// <summary>
        /// Update question metadata
        /// </summary>
        /// <param name="questionNote">QuestionNote Object</param>
        [HttpPost]
        [AjaxMethod]
        [ActionName("UpdateMetadata")]
        public ActionResult UpdateMetadata(Models.QuestionMetadata questionMetadata)
        {
            var q = QuestionActions.GetQuestion(questionMetadata.EntityId, questionMetadata.QuestionId);


            if (q != null)
            {
                if (questionMetadata.OldQuizId != null && questionMetadata.QuestionBank != questionMetadata.OldQuizId)
                {

                    var quizOldBiz = ContentActions.GetContent(Context.EntityId, questionMetadata.OldQuizId);


                    if (quizOldBiz != null)
                    {
                        Quiz oldQuiz = quizOldBiz.ToQuiz(ContentActions, QuestionActions, true);

                        List<BizDC.Question> listOfQuestions = new List<BizDC.Question>();

                        if (!oldQuiz.Questions.IsNullOrEmpty())
                        {

                            listOfQuestions = oldQuiz.Questions.Map(question => question.ToQuestion()).ToList();

                            var questionToMove = listOfQuestions.Find(i => i.Id == questionMetadata.QuestionId);
                            if (questionToMove != null)
                            {
                                listOfQuestions.Remove(questionToMove);

                                QuestionActions.UpdateQuestionList(quizOldBiz.ActualEntityid, quizOldBiz.Id, listOfQuestions, true);

                                var newQuizBiz = ContentActions.GetContent(Context.EntityId, questionMetadata.QuestionBank);


                                if (newQuizBiz != null)
                                {

                                    q = UpdateQuestion(questionMetadata, q);
                                    Quiz newQuiz = newQuizBiz.ToQuiz(ContentActions, QuestionActions, true);

                                    listOfQuestions = new List<BizDC.Question>();
                                    listOfQuestions = newQuiz.Questions.Map(i => i.ToQuestion()).ToList();



                                    listOfQuestions.Add(q);
                                    QuestionActions.UpdateQuestionList(newQuizBiz.ActualEntityid, newQuizBiz.Id, listOfQuestions, true);
                                }


                            }

                        }

                    }

                }
                else
                {
                    q = UpdateQuestion(questionMetadata, q);
                    QuestionActions.StoreQuestion(q);
                }
            }



            //Dictionary<string, string> additionalArguments = new Dictionary<string, string>();
            //if (!string.IsNullOrEmpty(questionMetadata.QuestionBankText))
            //    additionalArguments.Add(questionMetadata.QuestionBank, questionMetadata.QuestionBankText);
            //if (!string.IsNullOrEmpty(questionMetadata.eBookChapterText))
            //    additionalArguments.Add(questionMetadata.eBookChapter, questionMetadata.eBookChapterText);
            // Create Diff XML and Save to DB //
            CompareAndSaveModifiedQuestion(questionMetadata.QuestionId);

            return Json(new { Status = "success" });
        }

        private string CheckforDefaultValues(string str)
        {
            return str == null || str.Trim() == "0" ? string.Empty : str;
        }
        public BizDC.Question UpdateQuestion(Models.QuestionMetadata questionMetadata, BizDC.Question questionToUpdate)
        {
            questionToUpdate.Title = questionMetadata.Title;
            questionToUpdate.ExcerciseNo = questionMetadata.ExcerciseNo ?? string.Empty;
            questionToUpdate.QuestionBank = questionMetadata.QuestionBank;
            questionToUpdate.eBookChapter = questionMetadata.eBookChapter;
            questionToUpdate.Difficulty = CheckforDefaultValues(questionMetadata.Difficulty);
            questionToUpdate.CongnitiveLevel = CheckforDefaultValues(questionMetadata.CongnitiveLevel);
            questionToUpdate.BloomsDomain = CheckforDefaultValues(questionMetadata.BloomsDomain);
            questionToUpdate.Guidance = questionMetadata.Guidance;
            questionToUpdate.FreeResponseQuestion = questionMetadata.FreeResponseQuestion;
            questionToUpdate.EbookSectionText = questionMetadata.eBookChapterText;
            questionToUpdate.QuestionBankText = questionMetadata.QuestionBankText;

            questionToUpdate.QuestionStatus = questionMetadata.QuestionStatus;



            if (questionMetadata.SuggestedUse != null)
            {
                foreach (SelectListItem sli in questionMetadata.SuggestedUse)
                {
                    questionToUpdate.SuggestedUse.Add(sli.Text, sli.Value);

                }
            }

            if (questionMetadata.LearningObjectives != null)
            {
                foreach (SelectListItem sli in questionMetadata.LearningObjectives)
                {
                    questionToUpdate.LearningObjectives.Add(sli.Value, sli.Text);

                }


            }


            return questionToUpdate;

        }

        /// <summary>
        /// Add Note to Question
        /// </summary>
        /// <param name="questionNote">QuestionNote Object</param>
        [HttpPost]
        [AjaxMethod]
        [ActionName("AddNote")]
        public ActionResult AddNote(Models.QuestionNote questionNote)
        {
            questionNote.UserId = Context.CurrentUser.Id;
            questionNote.FirstName = Context.CurrentUser.FirstName;
            questionNote.LastName = Context.CurrentUser.LastName;
            questionNote.CourseId = Context.CourseId;

            BizDC.QuestionLog questionLog = new BizDC.QuestionLog
                                                {
                                                    QuestionId = questionNote.QuestionId,
                                                    EventType = QuestionLogEventType.NoteAdded,
                                                    UserId = Context.CurrentUser.Id,
                                                    FirstName = Context.CurrentUser.FirstName,
                                                    LastName = Context.CurrentUser.LastName,
                                                    CourseId = Context.CourseId
                                                };
            QuestionAdminActions.AddQuestionLog(questionLog);
            QuestionAdminActions.AddQuestionNote(questionNote.ToQuestionNote());

            //TODO Mitul should change it to avoid redirection:
            return RedirectToAction("NotesTab", new { questionId = questionNote.QuestionId, quizId = "" });
        }

        /// <summary>
        /// Add Flag to Question
        /// </summary>
        /// <param name="questionId">questionId</param>
        [HttpPost]
        [AjaxMethod]
        [ActionName("AddFlag")]
        public ActionResult AddFlag(Models.QuestionNote questionNote)
        {
            //remove the question from cache.
            var questions = new List<BizDC.Question>(){
                                                 new BizDC.Question()
                                                    {
                                                        Id = questionNote.QuestionId,
                                                        EntityId = mainEntityId
                                                    }
                                            };
            QuestionActions.RemoveQuestionsFromCache(questions);
            BizQuestion question = QuestionActions.GetQuestion(mainEntityId, questionNote.QuestionId);
            question.AdminFlag = true;
            QuestionActions.StoreQuestion(question);

            questionNote.UserId = Context.CurrentUser.Id;
            questionNote.FirstName = Context.CurrentUser.FirstName;
            questionNote.LastName = Context.CurrentUser.LastName;
            questionNote.CourseId = Context.CourseId;

            QuestionAdminActions.AddQuestionNote(questionNote.ToQuestionNote());

            BizDC.QuestionLog questionLog = new BizDC.QuestionLog();
            questionLog.QuestionId = questionNote.QuestionId;
            questionLog.EventType = QuestionLogEventType.Flagged;
            questionLog.UserId = Context.CurrentUser.Id;
            questionLog.FirstName = Context.CurrentUser.FirstName;
            questionLog.LastName = Context.CurrentUser.LastName;
            questionLog.CourseId = Context.CourseId;

            QuestionAdminActions.AddQuestionLog(questionLog);

            return Json(new { Status = "success" });
        }

        /// <summary>
        /// Remove Flag from Question
        /// </summary>
        /// <param name="questionId">questionId</param>
        public ActionResult RemoveFlag(string questionId)
        {
            if (string.IsNullOrEmpty(questionId)) return Json(new { Status = "Error" }, JsonRequestBehavior.AllowGet);

            //remove the question from cache.
            var questions = new List<BizDC.Question>(){
                                                 new BizDC.Question()
                                                    {
                                                        Id = questionId,
                                                        EntityId = mainEntityId
                                                    }
                                            };
            QuestionActions.RemoveQuestionsFromCache(questions);
            BizQuestion question = QuestionActions.GetQuestion(mainEntityId, questionId);
            question.AdminFlag = false;
            QuestionActions.StoreQuestion(question);

            BizDC.QuestionLog questionLog = new BizDC.QuestionLog();
            questionLog.QuestionId = questionId;
            questionLog.EventType = QuestionLogEventType.UnFlagged;
            questionLog.UserId = Context.CurrentUser.Id;
            questionLog.FirstName = Context.CurrentUser.FirstName;
            questionLog.LastName = Context.CurrentUser.LastName;
            questionLog.CourseId = Context.CourseId;

            QuestionAdminActions.AddQuestionLog(questionLog);

            return Json(new { Status = "Success" }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Adds new question to selected quiz
        /// </summary>
        /// <param name="questionType">Type of question to add</param>
        /// <param name="quizId">id of quiz where to add new question</param>
        public ActionResult AddNewQuestion(string questionType, string quizId)
        {
            string questionId;

            string createdBy = Context.CurrentUser.Id;
            const string userCreated = "true";
            var metaData = new Dictionary<string, string> {
                                                          {"createdBy", createdBy},
                                                          {"userCreated", userCreated},
                                                          {"totalUsed", "1"}, 
                                                          {"questionstatus", "0"}
                                                      };
            questionId = Guid.NewGuid().ToString();
            var question = new Models.Question()
            {
                ItemId = quizId,
                EnrollmentId = Context.EnrollmentId,
                Id = questionId,
                Type = Models.Question.QuestionTypeShortNameFromId(questionType),
                EntityId = mainEntityId,
                IsLast = true,
                Points = 1,
                IsNewQuestion = true,
                SearchableMetaData = metaData,
                Text = "",
                CustomUrl = ""
            };

            if (Models.Question.QuestionTypeShortNameFromId(questionType) == "HTS" || Models.Question.QuestionTypeShortNameFromId(questionType) == "CUSTOM")
            {
                question.Text = "Advanced Question";
                question.CustomUrl = "HTS";
                question.Type = "CUSTOM";
            }

            if (Models.Question.QuestionTypeShortNameFromId(questionType) == "FMA_GRAPH")
            {
                question.Text = "Graphing exercise";
                question.CustomUrl = "FMA_GRAPH";
                question.Type = "CUSTOM";
            }

            QuestionActions.StoreQuestion(question.ToQuestion());

            var quiz = this.UpdateQuizWithNewQuestion(quizId, question);

            question.QuizType = quiz.QuizType;
            question.Attempts = quiz.AttemptLimit.ToString();

            //question.HtsPlayerUrl = Context.Domain.CustomQuestionUrls["HTS"];
            if (Context.Domain.CustomQuestionUrls.ContainsKey("HTS"))
            {
                //question.HtsPlayerUrl = "http://localhost:49676/pxPlayer.ashx";
                question.HtsPlayerUrl = Context.Domain.CustomQuestionUrls["HTS"];
            }


            //ContentActions.RemoveQuestionsFromCache(questions);



            ViewData.Model = question;
            var url = Url.Action("QuestionEditor", new { Id = questionId, quizId = quizId });
            return Json(new { Status = "success", Url = url });
        }

        /// <summary>
        /// This new question can be added from a new question or duplicated
        /// </summary>
        /// <param name="quizId">Quiz Id in which new question will belong</param>
        /// <param name="question">Question Model information</param>
        /// <returns>Quiz Content Item</returns>
        private Quiz UpdateQuizWithNewQuestion(string quizId, ModelQuestion question)
        {
            BizDC.QuestionLog questionLog = new BizDC.QuestionLog
            {
                QuestionId = question.Id,
                EventType = QuestionLogEventType.Added,
                UserId = Context.CurrentUser.Id,
                FirstName = Context.CurrentUser.FirstName,
                LastName = Context.CurrentUser.LastName,
                CourseId = Context.CourseId
            };

            QuestionAdminActions.AddQuestionLog(questionLog);

            var item = this.ContentActions.GetContent(this.mainEntityId, quizId);
            var quiz = item.ToQuiz(this.ContentActions, QuestionActions, false);
            quiz.Questions =
                QuestionActions.GetQuestions(this.mainEntityId, item, null, null)
                    .Questions.Map(q => q.ToQuestion())
                    .ToList();
            quiz.Questions.Add(question);

            QuestionActions.UpdateQuestionList(this.mainEntityId, quizId, quiz.Questions.Map(q => q.ToQuestion()).ToList(), false);

            return quiz;
        }

        /// <summary>
        /// Logs the Question update event to Log table        
        /// </summary>
        /// <param name="questionId">questionId</param>
        public ActionResult QuestionModified(string questionId)
        {
            if (string.IsNullOrEmpty(questionId)) return Json(new { Status = "error" }, JsonRequestBehavior.AllowGet);
            try
            {
                CompareAndSaveModifiedQuestion(questionId);
            }
            catch (Exception ex)
            {
                return Json(new { Status = "error" });
            }
            return Json(new { Status = "success" });
        }

        private string GetQuestionStatusDescription(string code)
        {
            string Qstatus = string.Empty;
            switch (code)
            {
                case "0":
                    Qstatus = ExtensionMethods.GetEnumDescription(QuestionStatusType.InProgress);
                    break;
                case "1":
                    Qstatus = ExtensionMethods.GetEnumDescription(QuestionStatusType.AvailabletoInstructor);
                    break;
                case "2":
                    Qstatus = ExtensionMethods.GetEnumDescription(QuestionStatusType.Deleted);
                    break;
            }
            return Qstatus;
        }

        private void CompareAndSaveModifiedQuestion(string questionId)
        {
            var QuestionVersions = QuestionAdminActions.GetQuestionAllVersions(mainEntityId, questionId, true);
            var newQuestion = QuestionVersions.Take(1).SingleOrDefault();
            string latestVersion = newQuestion.QuestionVersion;
            var oldQuestion = QuestionVersions.Skip(1).Take(1).SingleOrDefault();
            // Question Status change detection //
            oldQuestion.QuestionStatus = GetQuestionStatusDescription(oldQuestion.QuestionStatus);
            newQuestion.QuestionStatus = GetQuestionStatusDescription(newQuestion.QuestionStatus);

            var xmlchanges = QuestionAdminActions.FindChangesInQuestions(oldQuestion, newQuestion);

            BizDC.QuestionLog questionLog = new BizDC.QuestionLog
            {
                QuestionId = questionId,
                EventType = QuestionLogEventType.Modified,
                UserId = Context.CurrentUser.Id,
                FirstName = Context.CurrentUser.FirstName,
                LastName = Context.CurrentUser.LastName,
                CourseId = Context.CourseId,
                Version = latestVersion,
                ChangesMadeXML = xmlchanges   //"<changes><change><field>QuestionTitle</field><orig>Who is the President</orig><new>Who is the Manager</new></change></changes>"
            };
            QuestionAdminActions.AddQuestionLog(questionLog);
            //remove the question from cache.
            var questions = new List<BizDC.Question>(){
                                                 new BizDC.Question()
                                                    {
                                                        Id = questionId,
                                                        EntityId = mainEntityId
                                                    }
                                            };
            QuestionActions.RemoveQuestionsFromCache(questions);
        }


        /// <summary>
        /// Bulk Edit functionality related
        /// </summary>
        /// <param name="search model"></param>
        /// <returns></returns>
        public ActionResult BulkEdit(string questionId)
        {

            ViewData["CourseTitle"] = Context.Course.Title;
            return View("BulkEdit");
        }

        public ActionResult BulkEditConfirm(QuestionAdminSearchPanel searchModel)
        {
            string questionCourseId = QuestionActions.GetQuestionRepositoryCourse(Context.EntityId);
            // question live in discipline course e.g. 6712 for stoneEcon not the current context.entityID (118514)
            var selectedQuestions = QuestionActions.GetQuestions(questionCourseId, searchModel.BulkEditSelectedQuestions).ToList();
            if (selectedQuestions.Count > 0)
            {
                selectedQuestions.ForEach(i => i.QuestionStatus = searchModel.BulkEditStatus);
                try
                {
                    QuestionActions.StoreQuestions(selectedQuestions);
                }
                catch (Exception ex)
                {
                    return Json(new { Status = "error" });
                }

                return Json(new { Status = "success" });
            }

            else
                return Json(new { Status = "No questions updated" });

        }
        //region end
    }
        #endregion Mitul's actions:
}

