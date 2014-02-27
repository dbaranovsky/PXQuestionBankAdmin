using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.HtsConversion;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Controllers.Contracts;
using Bfw.PX.PXPub.Controllers.Helpers;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

using System.Text.RegularExpressions;

namespace Bfw.PX.PXPub.Controllers.ContentTypes
{
    [PerfTraceFilter]
    public class QuizController : Controller
    {
        protected BizSC.IPageActions PageActions { get; set; }

        /// <summary>
        /// Access to the current business context information.
        /// </summary>
        /// <value>
        /// The context. 
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

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
        /// The content actions.
        /// </value>
        protected BizSC.IQuestionActions QuestionActions { get; set; }

        /// <summary>
        /// Actions for nav items.
        /// </summary>
        /// <value>
        /// The navigation actions. 
        /// </value>
        protected BizSC.INavigationActions NavigationActions { get; set; }

        protected BizSC.IGradeActions GradeActions { get; set; }

        /// <summary>
        /// Access to a content helper object
        /// </summary>
        /// <value>
        /// The content helper.
        /// </value>
        protected IContentHelper ContentHelper { get; set; }

        /// <summary>
        /// Access to QuizHelper
        /// </summary>
        protected IQuizHelper QuizHelper { get; set; }

        /// <summary>
        /// Gets or sets the assignment center helper.
        /// </summary>
        /// <value>
        /// The assignment center helper.
        /// </value>
        protected AssignmentCenterHelper AssignmentCenterHelper { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuizController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="contActions">The cont actions.</param>
        /// <param name="navigationActions">The navigation actions.</param>
        /// <param name="helper">The helper.</param>
        public QuizController(BizSC.IBusinessContext context, BizSC.IContentActions contActions, 
            BizSC.INavigationActions navigationActions, IContentHelper helper, IQuizHelper quizHelper, 
            AssignmentCenterHelper assignmentCenterHelper, BizSC.IPageActions pageActions, 
            BizSC.IQuestionActions questionActions, BizSC.IGradeActions gradeActions)
        {
            Context = context;
            NavigationActions = navigationActions;
            ContentActions = contActions;
            ContentHelper = helper;
            QuizHelper = quizHelper;
            AssignmentCenterHelper = assignmentCenterHelper;
            QuestionActions = questionActions;
            PageActions = pageActions;
            GradeActions = gradeActions;
        }

        /// <summary>
        /// Update a question pool.
        /// </summary>
        /// <param name="content">The content.<see cref="Quiz"/></param>
        /// <param name="editTitle">The edit title.</param>
        /// <param name="parentId">The parent id.</param>
        /// <param name="questionId">The question id.</param>
        /// <param name="poolCount">The pool count.</param>
        /// <param name="poolPoints">Number of points assigned to this pool in the quiz</param>
        public void EditPool(Quiz content, string editTitle, string parentId, string questionId, string poolCount, int poolPoints)
        {
            content.Id = questionId;
            content.Title = editTitle;
            content.ParentId = content.Id;
            ContentHelper.StoreQuiz(content);
            QuestionActions.EditQuestionList(Context.EntityId, parentId, questionId, poolCount, poolPoints);
        }

        /// <summary>
        /// Adds a new question pool.
        /// </summary>
        /// <param name="content">The content.<see cref="Quiz"/></param>
        /// <param name="parentId">The parent id.</param>
        /// <param name="poolCount">The pool count.</param>
        public JsonResult AddNewPool(Quiz content, string parentId, string poolCount)
        {
            content.ParentId = parentId;
            content.Hidden = true;
            ContentHelper.StoreQuiz(content);

            var question = new BizDC.Question
                               {
                                   Id = content.Id,
                                   BankUse = Convert.ToInt32(poolCount),
                                   EntityId = Context.EntityId,
                                   InteractionType = Biz.DataContracts.InteractionType.Bank
                               };

            QuestionActions.AddQuestionToQuestionList(Context.EntityId, parentId, content.Id, question);
            return Json(new { poolId = question.Id });
        }


        /// <summary>
        /// Returns a true or false value if the pool titles are the same for current quiz
        /// </summary>
        /// <param name="title">Current Title of the current pool</param>
        /// <param name="parentId">Quiz Id</param>
        /// <returns></returns>

        public ActionResult DoesQuestionPoolExist(string title, string parentId)
        {

            var allAuestionsInCurrentQuiz = ContentActions.GetContent(Context.EnrollmentId, parentId);
            var questionsCount = allAuestionsInCurrentQuiz.QuizQuestions.Count;
            var counter = 0;
            while (questionsCount > counter)
            {
                if (allAuestionsInCurrentQuiz.QuizQuestions[counter].Type == "2")
                {
                    //poolQuestionsIds.Add(allAuestionsInCurrentQuiz.QuizQuestions[counter].QuestionId);
                    var cItem = ContentActions.GetContent(Context.EntityId, allAuestionsInCurrentQuiz.QuizQuestions[counter].QuestionId);
                    if (cItem.Title.ToLowerInvariant() == title.ToLowerInvariant())
                    {
                        return new ContentResult() { Content = "True" };
                    }
                }
                counter++;
            }
            return new ContentResult() { Content = "False" };

        }



        /// <summary>
        /// Saves a quiz.
        /// </summary>
        /// <param name="content">The content.<see cref="Quiz"/></param>
        /// <param name="behavior">The behavior.</param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult SaveQuiz(Quiz content, string behavior, Assign assign, string toc = "syllabusfilter")
        {
            ActionResult result = null;
            ContentView model = null;

            switch (behavior.ToLowerInvariant())
            {
                case "cancel":
                    var idToLoad = string.IsNullOrEmpty(content.Id) ? content.ParentId : content.Id;
                    model = ContentHelper.LoadContentView(idToLoad, ContentViewMode.Preview, false, toc);
                    result = View("DisplayItem", model);
                    break;

                case "save":

                    if (ModelState.IsValid)
                    {
                        ContentHelper.StoreQuiz(content);
                        if (assign != null && assign.DueDate.Year > DateTime.MinValue.Year)
                        {
                            assign.Id = content.Id;
                            AssignmentCenterHelper.AssignItem(assign, toc);
                        }
                        model = ContentHelper.LoadContentView(content.Id, ContentViewMode.Preview, false, toc, false);
                        ((Quiz)model.Content).IsProductCourse = false;
                        ((Quiz)model.Content).Display = Quiz.DisplayType.Instructor;
                        ((Quiz)model.Content).ShowContentView = true;

                        result = View("DisplayItem", model);
                    }
                    else
                    {
                        content.EnvironmentUrl = Context.EnvironmentUrl;
                        content.CourseInfo = Context.Course.ToCourse();
                        content.EnrollmentId = Context.EnrollmentId;
                        content.Status = string.IsNullOrEmpty(content.Id) ? ContentStatus.New : ContentStatus.Existing;
                        content.Description = HttpUtility.HtmlDecode(content.Description);
                        result = View("CreateContent", content);
                    }
                    break;

                default:
                    result = View();
                    break;
            }

            return result;
        }

        /// <summary>
        /// Deletes the questions.
        /// </summary>
        /// <param name="quizQuestions">The quiz questions.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteQuestions(QuizQuestions quizQuestions)
        {
            var questionIds = new List<string>(quizQuestions.QuestionIds.Split(','));
            QuestionActions.DeleteQuestions(Context.EntityId, questionIds);
            return Json(new { status = "success" });
        }

       


        /// <summary>
        /// Attempts to retrieve the title for a custom question. Grabs the first 100 characters.
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        private string GetCustomQuestionTitle(BizDC.Question q)
        {
            string title = "Custom Question";
            string type = q.CustomUrl;

            if (type == "FMA_GRAPH")
            {
                if (!q.InteractionData.IsNullOrEmpty())
                {
                    XDocument myXml = XDocument.Parse(q.InteractionData);

                    if (myXml.Element("question") != null && myXml.Element("question").Element("question") != null)
                    {
                        title = myXml.Element("question").Element("question").Value; // CDATA property loaded as text.
                    }
                }
            }

            return title;
        }

        /// <summary>
        /// Updates the previous question.
        /// </summary>
        /// <param name="questionId">The question id.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdatePreviousQuestion(string questionId, string customQuestionXML, string courseId, string title)
        {
            string entityId = string.IsNullOrEmpty(courseId) ? Context.EntityId : courseId;
            var question = questionId.IsNullOrEmpty() ? null : QuestionActions.GetQuestion(entityId, questionId);
            if (question != null)
            {
                //update the question meta data.
                bool toUpdateQuestion = false;
                if (question.SearchableMetaData == null)
                {
                    question.SearchableMetaData = new Dictionary<string, string>();
                }
                if (!question.SearchableMetaData.Keys.Contains(BizDC.QuestionMetaDataFields.PublisherEdited.GetDescription()))
                {
                    question.SearchableMetaData.Add(BizDC.QuestionMetaDataFields.PublisherEdited.GetDescription(), "true");
                    toUpdateQuestion = true;
                }
                if (!question.SearchableMetaData.Keys.Contains(BizDC.QuestionMetaDataFields.ModifiedBy.GetDescription()))
                {
                    question.SearchableMetaData.Add(BizDC.QuestionMetaDataFields.ModifiedBy.GetDescription(), Context.CurrentUser.Id);
                    toUpdateQuestion = true;
                }
                else
                {
                    var lastModifiedBy = question.SearchableMetaData[BizDC.QuestionMetaDataFields.ModifiedBy.GetDescription()].Trim();
                    if (lastModifiedBy != Context.CurrentUser.Id)
                    {
                        question.SearchableMetaData[BizDC.QuestionMetaDataFields.ModifiedBy.GetDescription()] = Context.CurrentUser.Id;
                        toUpdateQuestion = true;
                    }
                }

                if (!customQuestionXML.IsNullOrEmpty())
                {
                    question.InteractionData = HttpUtility.HtmlDecode(customQuestionXML);
                    question.Body = GetCustomQuestionTitle(question);
                    question.Title = string.IsNullOrEmpty(title)?"":title;
                    toUpdateQuestion = true;
                }


                if (toUpdateQuestion)
                {
                    QuestionActions.StoreQuestion(question);
                }

            }
            return Json(new { status = "success" });
        }



        /// <summary>
        /// Updates the questions meta data.
        /// </summary>
        /// <param name="quizQuestions">The quiz questions.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateQuestionsMetaData(QuizQuestions quizQuestions, string removeFrom = "", string addTo = "")
        {
            var questionIds = new List<string>(quizQuestions.QuestionIds.Split(','));
            var questions = (quizQuestions.QuestionIds != null) ? QuestionActions.GetQuestions(Context.EntityId, questionIds).ToList() : null;
            //we don't have to update publisher edited questions.
            //only update user created questions.
            var userUpdatedquestions = (from c in questions where c.SearchableMetaData != null select c).ToList();
            foreach (var question in userUpdatedquestions)
            {
                if (question.SearchableMetaData == null)
                {
                    question.SearchableMetaData = new Dictionary<string, string>();
                }
                if (question.SearchableMetaData.Keys.Contains(BizDC.QuestionMetaDataFields.TotalUsed.GetDescription()))
                {
                    question.SearchableMetaData[BizDC.QuestionMetaDataFields.TotalUsed.GetDescription()] = (Convert.ToInt32(question.SearchableMetaData[BizDC.QuestionMetaDataFields.TotalUsed.GetDescription()]) - 1).ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    question.SearchableMetaData.Add(BizDC.QuestionMetaDataFields.TotalUsed.GetDescription(), "0");
                }
            }
            //Write code to save the questions(userUpdatedquestions) now in a batch.
            if (!userUpdatedquestions.IsNullOrEmpty())
            {
                QuestionActions.StoreQuestions(userUpdatedquestions);
            }
            return Json(new { status = "success" });
        }

        /// <summary>
        /// Add a list of questions (but not question banks) by ID to the end of a quiz.
        /// </summary>
        /// <param name="quizQuestions">The quiz questions.<see cref="QuizQuestions"/></param>
        /// <param name="mainQuizId"></param>
        /// <returns>
        /// A success JSON object.
        /// </returns>
        [HttpPost]
        public ActionResult AddQuestionsToQuiz(QuizQuestions quizQuestions, string mainQuizId = "")
        {
            if (!mainQuizId.IsNullOrEmpty() && quizQuestions.QuizId.IsNullOrEmpty())
            {
                var currentQuiz = ContentActions.GetContent(Context.EntityId, mainQuizId);
                quizQuestions.QuizId = currentQuiz.QuizQuestions.Last().QuestionId;
            }
            var questions = QuizHelper.UpdateQuizFromQuizQuestions(quizQuestions, ContentActions, QuestionActions, Context);


            ViewData.Model = ContentActions.GetContent(Context.EntityId, quizQuestions.QuizId).ToQuiz(ContentActions, QuestionActions, false);
            ((Quiz)ViewData.Model).Questions = questions.Map(q => q.ToQuestion()).ToList();

            ViewData["RunQuizInit"] = false;


            //ViewData.Model = ContentActions.GetContent(Context.EntityId, mainQuizId).ToQuiz(ContentActions, true);
            return View("DisplayTemplates/QuizPartials/Questions");

            //return Json(new { status = "success", poolId = quizQuestions.QuizId });
        }

        /// <summary>
        /// Adds a question by ID to quiz.
        /// </summary>
        /// <param name="quizQuestionId">id of the question that needs to be added<see cref="QuizQuestions"/></param>
        /// <param name="targetQuizId">id of the quiz in which we want to ass the new question<see cref="QuizQuestions"/></param>
        /// <returns>
        /// status message
        /// </returns>
        [HttpPost]
        public JsonResult AddQuestionToQuiz(string quizQuestionId, string targetQuizId)
        {
            if (quizQuestionId.IsNullOrEmpty() || targetQuizId.IsNullOrEmpty())
            {
                throw new ArgumentException("Question was not added to the assessment to missing question/quiz id.");
            }
            var questionDisciplineId = QuestionActions.GetQuestionRepositoryCourse(Context.EntityId);
            var questionContentItem = QuestionActions.GetQuestion(Context.EntityId, quizQuestionId);
            var currentQuiz = ContentActions.GetContent(Context.EntityId, targetQuizId);
            // question does not exist in our course entity
            if (questionContentItem == null)
            {
                // checking for duplicate question
                if (currentQuiz.QuizQuestions.Where(i => i.QuestionId == quizQuestionId).Count() == 0)
                {
                    var questionFromCourse = QuestionActions.GetQuestion(questionDisciplineId, quizQuestionId);
                    questionFromCourse.EntityId = Context.EntityId;
                    
                    QuestionActions.StoreQuestion(questionFromCourse);
                    var questions = new List<BizDC.Question>();
                    questions.Add(questionFromCourse);
                    QuestionActions.AppendQuestionList(Context.EntityId, targetQuizId, questions);
                }
                else
                {
                    return Json(new { message = "This question has already been added to \"" + currentQuiz.Title + "\".", success = false });
                }
            }
            else
            {
                // checking for duplicate question
                if (currentQuiz.QuizQuestions.Where(i => i.QuestionId == quizQuestionId).Count() == 0)
                {
                    var questions = new List<BizDC.Question>();
                    BizDC.Question question = new BizDC.Question()
                     {
                         EntityId = Context.EntityId,
                         Id = questionContentItem.Id,
                         Points = questionContentItem.Points
                     };
                    questions.Add(question);
                    QuestionActions.AppendQuestionList(Context.EntityId, targetQuizId, questions);

                    QuestionActions.StoreQuestion(questionContentItem);
                }
                else
                {
                    return Json(new { message = "This question has already been added to \"" + currentQuiz.Title + "\".", success = false });
                }
            }

            return Json(new { message = "Question was added to \"" + currentQuiz.Title + "\".", success = true });
        }

        /// <summary>
        /// Used to update the list of questions assosicaited with a quiz.  This allows for
        /// changing the order as well as removing questions.
        /// </summary>
        /// <param name="quizQuestions">The quiz questions.<see cref="QuizQuestions"/></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveQuestionList(QuizQuestions quizQuestions)
        {
            var questions = QuizHelper.UpdateQuizFromQuizQuestions(quizQuestions, ContentActions, QuestionActions, Context);

            ViewData.Model = ContentActions.GetContent(Context.EntityId, quizQuestions.QuizId).ToQuiz(ContentActions, QuestionActions, false);
            ((Quiz)ViewData.Model).Questions = questions.Map(q => q.ToQuestion()).ToList();

            ViewData["RunQuizInit"] = false;
            return View("DisplayTemplates/QuizPartials/Questions");

        }

        /// <summary>
        /// Saves the points for a Quiz.
        /// </summary>
        /// <param name="quizId">The quiz id.</param>
        /// <param name="questionId">The question id.</param>
        /// <param name="points">The points.</param>
        public void SavePoints(string quizId, string questionId, int? points)
        {
            var quiz = ContentActions.GetContent(Context.EntityId, quizId).ToQuiz(ContentActions, QuestionActions, true);

            var questionFound = quiz.Questions.First(q => q.Id == questionId);

            var questionSettings = new BizDC.QuestionSettings
            {
                Id = questionFound.Id,
                QuizId = quizId,
                EntityId = questionFound.EntityId,
                Points = points.GetValueOrDefault()
            };

            QuestionActions.UpdateQuestionSettings(Context.EntityId, questionSettings);
        }

        /// <summary>
        /// Saves the Quiz attempts.
        /// </summary>
        /// <param name="questionId">The question id.</param>
        /// <param name="quizId">The quiz id.</param>
        /// <param name="attempts">The attempts.</param>
        public void SaveAttempts(string questionId, string quizId, string attempts, string entityId)
        {
            var question = QuestionActions.GetQuestion(entityId, questionId);
            var quiz = ContentActions.GetContent(Context.EntityId, quizId);
            PX.Biz.DataContracts.AssessmentGroup assessmentGroup = null;

            if (quiz.AssessmentGroups != null)
            {
                var quizQuestionName = quizId + "_" + questionId;
                foreach (var quizGrp in quiz.AssessmentGroups)
                {
                    if (quizQuestionName == quizGrp.Name)
                    {
                        quizGrp.Attempts = attempts;
                        assessmentGroup = quizGrp;
                    }
                }

                if (assessmentGroup == null)
                {
                    var groupName = quizId + "_" + questionId;
                    question.AssessmentGroups.Add(groupName);
                    QuestionActions.StoreQuestion(question);

                    if (quiz.AssessmentGroups == null)
                    {
                        quiz.AssessmentGroups = new List<PX.Biz.DataContracts.AssessmentGroup>();
                    }

                    var agrp = new PX.Biz.DataContracts.AssessmentGroup
                                {
                                    Name = groupName,
                                    Attempts = attempts
                                };
                    quiz.AssessmentGroups.Add(agrp);
                }
            }

            ContentActions.StoreContent(quiz);
        }

        /// <summary>
        /// Saves the time limits fror a given Quiz.
        /// </summary>
        /// <param name="questionId">The question id.</param>
        /// <param name="quizId">The quiz id.</param>
        /// <param name="timelimits">The timelimits.</param>
        public void SaveTimeLimits(string questionId, string quizId, string timelimits)
        {
            var question = QuestionActions.GetQuestion(Context.EntityId, questionId);
            var quiz = ContentActions.GetContent(Context.EntityId, quizId);
            PX.Biz.DataContracts.AssessmentGroup assessmentGroup = null;

            if (question.AssessmentGroups != null)
            {
                foreach (var grp in question.AssessmentGroups)
                {
                    foreach (var quizGrp in quiz.AssessmentGroups)
                    {
                        if (grp == quizGrp.Name)
                        {
                            quizGrp.TimeLimit = timelimits;
                            assessmentGroup = quizGrp;
                        }
                    }
                }

                if (assessmentGroup == null)
                {
                    var groupName = quizId + "_" + questionId;
                    question.AssessmentGroups.Add(groupName);
                    QuestionActions.StoreQuestion(question);

                    if (quiz.AssessmentGroups == null)
                    {
                        quiz.AssessmentGroups = new List<PX.Biz.DataContracts.AssessmentGroup>();
                    }

                    var agrp = new PX.Biz.DataContracts.AssessmentGroup { Name = groupName, TimeLimit = timelimits };
                    quiz.AssessmentGroups.Add(agrp);
                }
            }

            ContentActions.StoreContent(quiz);
        }

        /// <summary>
        /// Saves the score.
        /// </summary>
        /// <param name="questionId">The question id.</param>
        /// <param name="quizId">The quiz id.</param>
        /// <param name="score">The score.</param>
        public void SaveScore(string questionId, string quizId, string score)
        {
            var question = QuestionActions.GetQuestion(Context.EntityId, questionId);
            var quiz = ContentActions.GetContent(Context.EntityId, quizId);
            PX.Biz.DataContracts.AssessmentGroup assessmentGroup = null;

            if (question.AssessmentGroups != null)
            {
                foreach (var grp in question.AssessmentGroups)
                {
                    foreach (var quizGrp in quiz.AssessmentGroups)
                    {
                        if (grp == quizGrp.Name)
                        {
                            var scr = (PX.Biz.DataContracts.SubmissionGradeAction)Enum.Parse(typeof(PX.Biz.DataContracts.SubmissionGradeAction), score, true);
                            quizGrp.SubmissionGradeAction = scr;
                            assessmentGroup = quizGrp;
                        }
                    }
                }

                if (assessmentGroup == null)
                {
                    var groupName = quizId + "_" + questionId;
                    question.AssessmentGroups.Add(groupName);
                    QuestionActions.StoreQuestion(question);

                    if (quiz.AssessmentGroups == null)
                    {
                        quiz.AssessmentGroups = new List<PX.Biz.DataContracts.AssessmentGroup>();
                    }

                    var agrp = new PX.Biz.DataContracts.AssessmentGroup { Name = groupName };
                    var scr = (PX.Biz.DataContracts.SubmissionGradeAction)Enum.Parse(typeof(PX.Biz.DataContracts.SubmissionGradeAction), score, true);
                    agrp.SubmissionGradeAction = scr;
                    quiz.AssessmentGroups.Add(agrp);
                }
            }

            ContentActions.StoreContent(quiz);
        }

        /// <summary>
        /// Saves the scrambled code for an AssessmentGroup.
        /// </summary>
        /// <param name="questionId">The question id.</param>
        /// <param name="quizId">The quiz id.</param>
        /// <param name="scrambled">The scrambled.</param>
        public void SaveScrambled(string questionId, string quizId, string scrambled)
        {
            var question = QuestionActions.GetQuestion(Context.EntityId, questionId);
            var quiz = ContentActions.GetContent(Context.EntityId, quizId);
            PX.Biz.DataContracts.AssessmentGroup assessmentGroup = null;

            if (question.AssessmentGroups != null)
            {
                foreach (var grp in question.AssessmentGroups)
                {
                    foreach (var quizGrp in quiz.AssessmentGroups)
                    {
                        if (grp == quizGrp.Name)
                        {
                            quizGrp.Scrambled = scrambled;
                            assessmentGroup = quizGrp;
                        }
                    }
                }

                if (assessmentGroup == null)
                {
                    var groupName = quizId + "_" + questionId;
                    question.AssessmentGroups.Add(groupName);
                    QuestionActions.StoreQuestion(question);

                    if (quiz.AssessmentGroups == null)
                    {
                        quiz.AssessmentGroups = new List<PX.Biz.DataContracts.AssessmentGroup>();
                    }

                    PX.Biz.DataContracts.AssessmentGroup agrp = new PX.Biz.DataContracts.AssessmentGroup();
                    agrp.Name = groupName;
                    agrp.Scrambled = scrambled;
                    quiz.AssessmentGroups.Add(agrp);
                }
            }

            ContentActions.StoreContent(quiz);
        }

        /// <summary>
        /// Saves the Question hints.
        /// </summary>
        /// <param name="questionId">The question id.</param>
        /// <param name="quizId">The quiz id.</param>
        /// <param name="hints">The hints.</param>
        public void SaveHints(string questionId, string quizId, string hints)
        {
            var question = QuestionActions.GetQuestion(Context.EntityId, questionId);
            var quiz = ContentActions.GetContent(Context.EntityId, quizId);
            PX.Biz.DataContracts.AssessmentGroup assessmentGroup = null;

            if (question.AssessmentGroups != null)
            {
                foreach (var grp in question.AssessmentGroups)
                {
                    foreach (var quizGrp in quiz.AssessmentGroups)
                    {
                        if (grp == quizGrp.Name)
                        {
                            quizGrp.Hints = hints;
                            assessmentGroup = quizGrp;
                        }
                    }
                }

                if (assessmentGroup == null)
                {
                    var groupName = quizId + "_" + questionId;
                    question.AssessmentGroups.Add(groupName);
                    QuestionActions.StoreQuestion(question);

                    if (quiz.AssessmentGroups == null)
                    {
                        quiz.AssessmentGroups = new List<PX.Biz.DataContracts.AssessmentGroup>();
                    }

                    var agrp = new PX.Biz.DataContracts.AssessmentGroup { Name = groupName, Hints = hints };
                    quiz.AssessmentGroups.Add(agrp);
                }
            }

            ContentActions.StoreContent(quiz);
        }

        /// <summary>
        /// Saves the review of a Quiz.
        /// </summary>
        /// <param name="questionId">The question id.</param>
        /// <param name="quizId">The quiz id.</param>
        /// <param name="review">The review.</param>
        public void SaveReview(string questionId, string quizId, string review)
        {
            var question = QuestionActions.GetQuestion(Context.EntityId, questionId);
            var quiz = ContentActions.GetContent(Context.EntityId, quizId);
            PX.Biz.DataContracts.AssessmentGroup assessmentGroup = null;

            if (question.AssessmentGroups != null)
            {
                foreach (var grp in question.AssessmentGroups)
                {
                    foreach (var quizGrp in quiz.AssessmentGroups)
                    {
                        if (grp == quizGrp.Name)
                        {
                            var rev = (PX.Biz.DataContracts.HomeworkGroupFlags)Enum.Parse(typeof(PX.Biz.DataContracts.HomeworkGroupFlags), review, true);
                            quizGrp.Review = rev;
                            assessmentGroup = quizGrp;
                        }
                    }
                }

                if (assessmentGroup == null)
                {
                    var groupName = quizId + "_" + questionId;
                    question.AssessmentGroups.Add(groupName);
                    QuestionActions.StoreQuestion(question);

                    if (quiz.AssessmentGroups == null)
                    {
                        quiz.AssessmentGroups = new List<PX.Biz.DataContracts.AssessmentGroup>();
                    }

                    var agrp = new PX.Biz.DataContracts.AssessmentGroup { Name = groupName };
                    var rev = (PX.Biz.DataContracts.HomeworkGroupFlags)Enum.Parse(typeof(PX.Biz.DataContracts.HomeworkGroupFlags), review, true);
                    agrp.Review = rev;
                    quiz.AssessmentGroups.Add(agrp);
                }
            }

            ContentActions.StoreContent(quiz);
        }

        /// <summary>
        /// Display the view that represents a Quiz.
        /// </summary>
        /// <param name="q">The q.<see cref="Quiz"/></param>
        /// <param name="ordinal">The question that should be displayed first.</param>
        /// <returns></returns>
        public ActionResult ShowQuiz(Quiz q, string extEntityId, int? ordinal, string toc = "syllabusfilter")
        {
            var contentView = ContentHelper.LoadContentView(q.Id, extEntityId, ContentViewMode.Preview, toc);
            var quiz = (Quiz)contentView.Content;
            quiz.EnrollmentId = Context.EnrollmentId;
            quiz.UserId = Context.CurrentUser.Id;
            quiz.CourseInfo = Context.Course.ToCourse();
            quiz.QuestionId = q.QuestionId;

            // If this is a quiz, we always need to start at the first question.
            if (quiz.QuizType == QuizType.Assessment)
            {
                ordinal = 0;
            }

            if (Context.ImpersonateStudent)
            {
                MakeQuizTakableByInstructor(quiz);                
                 
            }
            // For xbook, if we have overridden the due date requirement to take the quiz, make sure the quiz is gradable
            if (quiz.OverrideDueDateReq)
            {
                MakeQuizGradable(quiz.Id);
            }

            // We need to figure out where to start. If we were given an explicit place to start, use that; otherwise,
            // we should use the first question available that still has attempts remaining.
            if (!ordinal.HasValue)
            {
                bool isAnyQuestionWithNoAttempt = false;
                for (ordinal = 0; ordinal < quiz.Questions.Count; ordinal++)
                {
                    var questionAttemptsTaken = quiz.QuestionAttempts == null || !quiz.QuestionAttempts.ContainsKey(quiz.Questions[ordinal.Value].Id) ? 0 : quiz.QuestionAttempts[quiz.Questions[ordinal.Value].Id].Count;
                    if (questionAttemptsTaken == 0)
                    {
                        isAnyQuestionWithNoAttempt = true;
                        break;
                    }
                    else if (questionAttemptsTaken == 1)
                    {
                        if (quiz.QuestionAttempts[quiz.Questions[ordinal.Value].Id][0].PointsPossible == null)
                        {
                            isAnyQuestionWithNoAttempt = true;
                            break;
                        }
                    }
                }

                if (!isAnyQuestionWithNoAttempt)
                {
                    for (ordinal = 0; ordinal < quiz.Questions.Count; ordinal++)
                    {
                        var questionAttemptsTaken = quiz.QuestionAttempts == null ? 0 : quiz.QuestionAttempts[quiz.Questions[ordinal.Value].Id].Count;
                        var attemptsAvailable = !String.IsNullOrEmpty(quiz.Questions[ordinal.Value].Attempts) ? Int32.Parse(quiz.Questions[ordinal.Value].Attempts) : quiz.AttemptLimit;
                        if (attemptsAvailable == 0)
                        {
                            break;
                        }
                        var questionAttemptsRemaining = attemptsAvailable - questionAttemptsTaken;
                        if (questionAttemptsRemaining > 0)
                        {
                            break;
                        }
                    }
                }
            }
            
            ViewData["fromQBA"] = HttpContext.Request.UrlReferrer.ToString().ToLower().Contains("questionadmin");
            
            ViewData["ordinal"] = ordinal.Value;

            ViewData.Model = quiz;

            //Remove the Cache
            var questions = quiz.Questions.Map(quest => quest.ToQuestion()).ToList();
            QuestionActions.RemoveQuestionsFromCache(questions);

            return View("DisplayTemplates/QuizPartials/ShowQuiz");
        }

        [ValidateInput(false)]
        public ActionResult StoreQuizData(string customXML, string customURL, string entityId)
        {
            var quizId = QuestionActions.StoreQuizPreview(customXML, customURL, entityId);
            return Content(quizId);
        }

        /// <summary>
        /// Display the view that allows for editing of a Quiz.
        /// </summary>
        /// <param name="q">The q.</param>
        /// <returns></returns>
        public ActionResult EditQuiz(Quiz q)
        {
            var item = ContentActions.GetContent(Context.EntityId, q.Id);

            if (item.Subtype != null && item.Subtype.ToLower() == "learningcurveactivity")
            {
                q = item.ToLearningCurveActivity(ContentActions, QuestionActions, true, false);
            }
            else
            {
                q = item.ToQuiz(ContentActions, QuestionActions, true);
            }
            ViewData.Model = q;
            return View("DisplayTemplates/QuizPartials/EditQuiz");
        }

        /// <summary>
        /// Displays a view that show the Submission history for a Quizs
        /// </summary>
        /// <param name="quizId">The quiz id.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        public ActionResult SubmissionHistory(string quizId, int version)
        {
            ViewData.Model = new Submission() { QuizId = quizId, EnrollmentId = Context.EnrollmentId, Version = version };
            return View("DisplayTemplates/QuizPartials/SubmissionHistory");
        }

        /// <summary>
        /// Displays the view for Editing a question.
        /// </summary>
        /// <param name="questionId">The question id.</param>
        /// <param name="quizId">The quiz id.</param>
        /// <param name="isLast">if set to <c>true</c> [is last].</param>
        /// <param name="isAdvancedConvert">if set to <c>true</c> converts question to advanced before displaying.</param>
        /// <returns></returns>
        public ActionResult EditQuestion(string questionId, string quizId, bool isLast, bool isPoolQuestion = false, bool isAdvancedConvert = false, bool isFromLearningCurve = false)
        {
            if (questionId == "new_question")
            {
                return CreateQuestion(quizId, "choice", true);
            }

            var item = ContentActions.GetContent(Context.EntityId, quizId);
            if (null == item)
            {
                return new EmptyResult();
            }
            Quiz quiz;
            if (item.Subtype.ToLower() == "learningcurveactivity")
            {
                quiz = item.ToLearningCurveActivity(ContentActions, QuestionActions, true, false);
            }
            else
            {
                quiz = item.ToQuiz(ContentActions, QuestionActions, true);
            }

            //remove the question from cache.
            QuestionActions.RemoveQuestionsFromCache(new List<BizDC.Question>()
            {
                new BizDC.Question()
                {
                    Id = questionId,
                    EntityId = Context.EntityId
                }
            });

            var question = quiz.Questions.Filter(q => q.Id == questionId).First();
            if (null == question)
            {
                return new EmptyResult();
            }
            // The EntityId exists for the Context and the Question
            // Copies the question from its linked source to quiz (no longer a linked question)
            if (!(String.IsNullOrEmpty(Context.EntityId) || String.IsNullOrEmpty(question.EntityId)) &&
                Context.EntityId != question.EntityId)
            {


                QuizHelper.UpdateQuizFromQuizItem(quiz, ContentActions, QuestionActions, Context); //copies questions to current entity, saves the quiz
                
            }

            question.EnrollmentId = Context.EnrollmentId;
            question.ItemId = quizId;
            question.IsLast = isLast;

            question.QuizType = quiz.QuizType;
            if (Context.Domain.CustomQuestionUrls.ContainsKey("HTS"))
            {
                //question.HtsPlayerUrl = "http://localhost:49676/pxplayer.ashx";
                question.HtsPlayerUrl = Context.Domain.CustomQuestionUrls["HTS"];
            }
            question.IsAdvancedConvert = isAdvancedConvert;
            if (isAdvancedConvert)
            {
                question.CustomUrl = "HTS";
                question.Type = "CUSTOM";
            }

            // If some question setting are null, default to quiz/homework settings
            if (question.Attempts == null)
            {
                question.Attempts = quiz.AttemptLimit.ToString();
            }
            if (isFromLearningCurve)
            {
                question.LearningCurveQuestionSettings = new List<LearningCurveQuestionSettings>() { GetLearningCurveQuestionSettings(quizId, questionId) };
            }

            ViewData.Model = question;
            ViewData["isFromLearningCurve"] = isFromLearningCurve;
            ViewData["isPoolQuestion"] = isPoolQuestion;
            return View("DisplayTemplates/QuizPartials/EditQuestion");
        }

        //TODO:
        public ActionResult RemoveQuestionFromCache(string questionId, string courseId)
        {
            
            var questions = new List<BizDC.Question>(){
                                                 new BizDC.Question()
                                                    {
                                                        Id = questionId,
                                                        EntityId = string.IsNullOrEmpty(courseId) ? Context.EntityId : courseId
                                                    }
                                            };
            QuestionActions.RemoveQuestionsFromCache(questions);
            return Json(new { status = "success" });
        }

        /// <summary>
        /// Displays the view for Creating a question.
        /// </summary>
        /// <param name="quizId">The quiz id.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public ActionResult CreateQuestion(string quizId, string type, bool loadComponent)
        {
            //createdby, modifiedby, usercreated, totalUsed.

            string createdBy = Context.CurrentUser.Id;
            const string userCreated = "true";
            var metaData = new Dictionary<string, string>
                                                      {
                                                          {"createdBy", createdBy},
                                                          {"userCreated", userCreated},
                                                          {"totalUsed", "1"}
                                                          //{"questionstatus", "1"} // question status = 1 "available to instructor"
                                                      };
            var questionId = Guid.NewGuid().ToString();

            //removed dashes for custom questions type javascript references.
            questionId = questionId.Replace("-", "_");


            var question = new Question()
                        {
                            ItemId = quizId,
                            EnrollmentId = Context.EnrollmentId,
                            Id = questionId,
                            Type = Question.QuestionTypeShortNameFromId(type),
                            EntityId = Context.EntityId,
                            IsLast = true,
                            Points = 1,
                            IsNewQuestion = true,
                            SearchableMetaData = metaData,
                            Text = "",
                            CustomUrl = ""
                        };

            UpdateTitle(question, type);
            if (Question.QuestionTypeShortNameFromId(type) == "HTS")
            {
                question.Text = "Untitled Advanced Question";
                question.CustomUrl = "HTS";
                question.Type = "CUSTOM";
            }

            if (Question.QuestionTypeShortNameFromId(type) == "FMA_GRAPH")
            {
                question.Text = "Untitled Graphing exercise";
                question.CustomUrl = "FMA_GRAPH";
                question.Type = "CUSTOM";
            }


            QuestionActions.StoreQuestion(question.ToQuestion());

            var quiz = ContentActions.GetContent(Context.EntityId, quizId).ToQuiz(ContentActions, QuestionActions, true);
            quiz.Questions.Add(question);

            //Ensure that all the questions which belong to other entityid are copied.
            QuizHelper.UpdateQuizFromQuizItem(quiz, ContentActions, QuestionActions, Context);

            //ContentActions.UpdateQuestionList(Context.EntityId, quizId, quiz.Questions.Map(q => q.ToQuestion()).ToList(), false);
            if (!loadComponent)
            {
                ViewData.Model = ContentActions.GetContent(Context.EntityId, quizId).ToQuiz(ContentActions, QuestionActions, true);
                return View("DisplayTemplates/QuizPartials/Questions");
            }

            question.QuizType = quiz.QuizType;
            question.Attempts = quiz.AttemptLimit.ToString();
            if (Context.Domain.CustomQuestionUrls.ContainsKey("HTS"))
            {
                //question.HtsPlayerUrl = "http://localhost:49676/pxPlayer.ashx";
                question.HtsPlayerUrl = Context.Domain.CustomQuestionUrls["HTS"];
            }

            ViewData.Model = question;
            return View("DisplayTemplates/QuizPartials/EditQuestion");
        }


        private void UpdateTitle(Question question, string type)
        {
            switch (Question.QuestionTypeShortNameFromId(type))
            {
                case "A":
                    question.Title = "Untitled Multiple Answer Question";
                    break;
                case "MC":
                    question.Title = "Untitled Multiple Choice Question";
                    break;
                case "HTS":
                    question.Title = "Untitled Advanced Question";
                    break;
                case "FMA_GRAPH":
                    question.Title = "Untitled Graphing Question";
                    break;
                case "E":
                    question.Title = "Untitled Essay Question";
                    break;
                case "MT":
                    question.Title = "Untitled Matching Question";
                    break;
                case "TXT":
                    question.Title = "Untitled Short Answer Question";
                    break;
            };
        }

        /// <summary>
        /// Converts a question into an HTS Question.
        /// </summary>
        /// <param name="questionId">The question id.</param>
        /// <param name="questionXml">The question XML.</param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult MakeHtsQuestion(string questionId, string quizId, string questionXml)
        {
            var content = ContentActions.GetContent(Context.EntityId, quizId);
            string resourceEntityId = content.ResourceEntityId;

            string strResponse = "";
            try
            {
                HTSQuestion htsQuestion = new HTSQuestion("Section Heading", questionXml);
                XDocument xDoc = htsQuestion.GetHtsQuestion();
                strResponse = xDoc.ToString();
            }
            catch
            {
                // if there is a problem with the conversion then use the empty question structure.
                strResponse = ConfigurationManager.AppSettings["EmptyHtsQuestionData"];
            }

            strResponse = strResponse.Replace("<", "&lt;");
            strResponse = strResponse.Replace(">", "&gt;");
            strResponse = "<question questionid=\"" + questionId + "\" version=\"\" resourceentityid=\"" + resourceEntityId + "\" creationdate=\"\" creationby=\"\" modifieddate=\"\" modifiedby=\"\" flags=\"\" schema=\"\" score=\"\" partial=\"\"><answer><value /></answer><body /><interaction type=\"custom\"><data>" + strResponse + "</data></interaction></question>";
            strResponse = strResponse.Replace("\r\n", "");
            strResponse = strResponse.Replace("\\", "");
            var result = new ContentResult() { Content = strResponse };
            return result;
        }

        /// <summary>
        /// Displays the View for a Question.
        /// </summary>
        /// <param name="quizId">The quiz id.</param>
        /// <returns></returns>
        public ActionResult Questions(string quizId)
        {
            ViewData.Model = ContentActions.GetContent(Context.EntityId, quizId).ToQuiz(ContentActions, QuestionActions, true);
            return View("DisplayTemplates/QuizPartials/Questions");
        }

        /// <summary>
        /// Corrects invalid question bank totals
        /// </summary>
        /// <param name="quizId">The quiz id.</param>
        /// <returns></returns>
        public void UpdateQuestionBankTotals(string quizId)
        {
            if (!string.IsNullOrEmpty(quizId))
            {
                var quiz = ContentActions.GetContent(Context.EntityId, quizId).ToQuiz(ContentActions, QuestionActions, true);
                foreach (var question in quiz.Questions.Where(x => x.Type == "BANK"))
                {
                    if (question.BankUse > question.BankCount)
                    {
                        question.BankUse = question.BankCount;
                        QuestionActions.EditQuestionList(Context.EntityId, quizId, question.Id, question.BankUse.ToString(), null);
                    }
                }
            }
        }

        /// <summary>
        /// Runs validation for a Question list.
        /// </summary>
        /// <param name="quizId">The quiz id.</param>
        /// <returns></returns>
        public void ValidateQuestions(string quizId)
        {
            if (string.IsNullOrEmpty(quizId))
                return;

            var quiz = ContentActions.GetContent(Context.EntityId, quizId).ToQuiz(ContentActions, QuestionActions, true);
            var questionIds = new List<string>();
            var questionBankIds = new List<string>();

            foreach (var question in quiz.Questions.Where(x => x.Type != "BANK"))
            {
                var doc = new XmlDocument();
                doc.LoadXml(question.QuestionXml);
                XmlNodeList body = doc.GetElementsByTagName("body");
                bool deleteQuestion = true;

                if (question.Type == "CUSTOM")
                {
                    deleteQuestion = question.InteractionData.IsNullOrEmpty();
                }
                else
                {
                    foreach (XmlNode bodyText in body)
                    {
                        if (bodyText.InnerText.Length > 0 && bodyText.InnerText.Trim().ToLower() != "<paragraph><textinteraction /></paragraph>")
                        {
                            deleteQuestion = false;
                        }
                    }
                }

                if (deleteQuestion)
                {
                    questionIds.Add(question.Id);
                }
            }

            foreach (var bank in quiz.Questions.Where(x => x.Type.ToUpper().Trim() == "BANK"))
            {
                if (bank.BankCount == 0)
                {
                    //store the ids to delete.
                    questionBankIds.Add(bank.Id);
                }
            }

            if (!questionIds.IsNullOrEmpty())
            {
                var msg = new Common.Logging.LogMessage() { Application = "PX", Message = "QuestionIds: " + string.Join(",", questionIds.ToArray()) };
                Context.Logger.Log(msg);

                QuestionActions.DeleteQuestions(Context.EntityId, questionIds);
            }
            if (!questionIds.IsNullOrEmpty() || !questionBankIds.IsNullOrEmpty())
            {
                var questions = (from c in quiz.Questions where !(questionIds.Contains(c.Id) || (questionBankIds.Contains(c.Id))) select c.ToQuestion()).ToList();
                //also update the question list.
                QuestionActions.UpdateQuestionList(Context.EntityId, quizId, questions, true);
            }

        }

        /// <summary>
        /// Diplays the View for a list of questions.
        /// </summary>
        /// <param name="quizId">The quiz id.</param>
        /// <returns></returns>
        public ActionResult QuestionList(string quizId, string mainQuizId = "", bool isQuestionOverview = false)
        {
            Quiz questionPool = ContentActions.GetContent(Context.EntityId, quizId).ToQuiz(ContentActions, QuestionActions, true);
            //var analysis = ContentActions.GetQuestionAnalysis(Context.EntityId, mainQuizId);
            //if (!questionPool.Questions.IsNullOrEmpty())
            //{
            //    foreach (var question in questionPool.Questions)
            //    {
            //        var matched = (from c in analysis where c.QuestionId == question.Id select c);
            //        if (null != matched && matched.Count() > 0)
            //        {
            //            question.Analysis = matched.OrderByDescending(o => o.Version).First().ToQuestionAnalysis();
            //        }
            //    }
            //}

            ViewData["isPoolQuestion"] = true;
            ViewData["isQuestionOverview"] = isQuestionOverview;
            ViewData["mainQuizId"] = mainQuizId;
            ViewData["RunQuizInit"] = false;

            return View("DisplayTemplates/QuizPartials/QuestionList", questionPool);
        }

        public ActionResult DisplayQuestionList(string quizId)
        {
            ViewData.Model = ContentActions.GetContent(Context.EntityId, quizId).ToQuiz(ContentActions, QuestionActions, true);
            return View("DisplayTemplates/QuizPartials/DisplayQuestionList");
        }

        /// <summary>
        /// Gets the HTS question HTML from data being passed ins.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="questionId">The question id.</param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult GetHtsQuestionHtml(string data, string questionId)
        {
            string str1 = QuestionActions.GetCustomQuestionPreview(Context.Domain.CustomQuestionUrls["HTS"], data, questionId);
            return Content(str1);
        }

        /// <summary>
        /// Gets the HTS question HTML by id.
        /// </summary>
        /// <param name="questionId">The question id.</param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult GetCustomInlinePreview(string questionId, string customUrl, string entityId)
        {
            string customQuestionResponse = "";

            var qList = new List<BizDC.Question>
                                             {
                                                 new BizDC.Question()
                                                     {
                                                         EntityId = Context.EntityId,
                                                         Id = questionId
                                                     }
                                             };
            QuestionActions.RemoveQuestionsFromCache(qList);

            entityId = string.IsNullOrEmpty(entityId) ? Context.EntityId : entityId;

            var question = QuestionActions.GetQuestion(entityId, questionId);
            
            if (string.IsNullOrEmpty(customUrl) && question.CustomUrl != null)
                customUrl = question.CustomUrl.ToUpper();

            customQuestionResponse = QuestionActions.GetCustomQuestionPreview(customUrl, question.InteractionData, questionId);
            
            return Content(customQuestionResponse);
        }


        /// <summary>
        /// Gets the HTS question HTML by id.
        /// </summary>
        /// <param name="questionId">The question id.</param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult GetCustomInlinePreviewGeneric(string entityId, string questionId, string customUrl)
        {
            var qList = new List<BizDC.Question>
                                             {
                                                 new BizDC.Question()
                                                     {
                                                         EntityId = entityId,
                                                         Id = questionId
                                                     }
                                             };
            QuestionActions.RemoveQuestionsFromCache(qList);
            var question = QuestionActions.GetQuestion(entityId, questionId);
            
            if (string.IsNullOrEmpty(customUrl) && question.CustomUrl != null)
                customUrl = question.CustomUrl.ToUpper();

            string customQuestionResponse = "";

            customQuestionResponse = QuestionActions.GetCustomQuestionPreview(customUrl, question.InteractionData, questionId);
            
            return Content(customQuestionResponse);
        }

        string RemoveCData(string sXML)
        {
            string outputXML = "";

            if (!sXML.IsNullOrEmpty())
            {
                XDocument doc = XDocument.Parse(sXML);
                var cDataNodes = doc.DescendantNodes().OfType<XCData>().ToArray();
                foreach (var cDataNode in cDataNodes)
                    cDataNode.ReplaceWith(new XText(cDataNode));

                outputXML = doc.ToString();
            }

            return outputXML;
        }


        /// <summary>
        /// Generates previeq for a GRAPH Question.
        /// </summary>
        /// <param name="editorURL">The s URL.</param>
        /// <param name="customXML">The s XML.</param>
        /// <param name="questionId">The question id.</param>
        /// <returns></returns>
        public ActionResult EditCustomQuestion(string customXML, string questionId, string questionType, string title)
        {
            string sResponse = "";
            customXML = RemoveCData(HttpUtility.HtmlDecode(customXML));
            //customXML = HttpUtility.HtmlEncode(customXML);

            string sQuestionData = customXML;

            string editorURL = "test";

            if (Context.Domain.CustomQuestionUrls.ContainsKey(questionType))
            {
                editorURL = Context.Domain.CustomQuestionUrls[questionType];

                try
                {
                    customXML = "<info id=\"0\" mode=\"Review\"><question schema=\"2\" partial=\"false\"><answer /><body></body><customurl>HTS</customurl><interaction type=\"custom\"><data><![CDATA["
                        + customXML + "]]></data></interaction></question></info>";
                    byte[] buffer = Encoding.UTF8.GetBytes(customXML);

                    HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(editorURL);

                    WebReq.Method = "POST";
                    WebReq.ContentLength = buffer.Length;
                    Stream ReqStream = WebReq.GetRequestStream();
                    ReqStream.Write(buffer, 0, buffer.Length);
                    ReqStream.Close();
                    using (WebResponse WebRes = WebReq.GetResponse())
                    {
                        using (StreamReader ResReader = new StreamReader(WebRes.GetResponseStream()))
                        {
                            sResponse = ResReader.ReadToEnd();
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

                    questionId = questionId.Replace("-", "_");

                    sResponse = myXml.Element("custom").Element("body").Value; // CDATA property loaded as text.
                    sResponse = sResponse.Replace("$QUESTIONID$", questionId); // just use placeholder value.

                    string divId = "custom_preview_" + questionId;
                    string version = "1";
                    string mode = "editing";
                    string body = "";
                    string data = ser.Serialize(sQuestionData);
                    string pointsPossible = "1";
                    string pointsAssigned = "NaN";


                    string cqScript = string.Format(
                        QuestionActions.CQScriptString,
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
                    sResponse = sResponse.Replace("questionInfo.divId", "questionInfo.divId" + questionId + "_edit");

                    //sResponse = sResponse.Replace("[~]", "/BrainHoney/Resource/" + entityId);
                }
                catch (Exception ex2)
                {
                    sResponse = "There was an error: " + ex2.ToString();
                }
                ViewData["metatitle"] = string.IsNullOrEmpty(title)?"":title;
                ViewData["response"] = sResponse;
            }

            return View("CustomQuestionEditorComponent");
        }

        /// <summary>
        /// For a list of objects, and a property, if the property is set the same for all objects,
        /// return the set value. Otherwise return null.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="pi">The property we will inspect.</param>
        /// <param name="questions">The list of objects.</param>
        /// <returns>The value, if all objetcs have the same value for that property; otherwise null.</returns>
        private static T CommonOrBlank<T>(System.Reflection.PropertyInfo pi, IEnumerable<object> objects)
        {
            // If any of the objects in the list is null, return blank, since we
            // wouldn't be able to get the property from it.
            if (objects.Where(o => o == null).Count() > 0)
            {
                return default(T);
            }

            // Now iterate through the values of the property for all of the objects, and
            // make sure they all match the values of the property for the first object.
            T commonValue = (T)pi.GetValue(objects.First(), null);
            foreach (var o in objects)
            {
                var value = ((T)pi.GetValue(o, null));

                // If the value is null, but the common value is not null, return blank.
                if (value == null && commonValue != null)
                {
                    return default(T);
                }

                // If the value is present and doesn't match the common value, return blank.
                if (value != null && !value.Equals(commonValue))
                {
                    return default(T);
                }
            }
            return commonValue;
        }

        /// <summary>
        /// Displays the question settings
        /// </summary>
        /// <param name="quizId">Id of the quiz including the question</param>
        /// <param name="questionId">Id of the question</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult QuestionSettings(string quizId, string questionId)
        {
            QuestionSettings model;

            // We need to get the question objects from the IDs. This means getting the quiz and matching.
            var ci = ContentActions.GetContent(Context.EntityId, quizId);
            if (null == ci)
            {
                return Json(false);
            }

            var quiz = ci.ToQuiz(ContentActions, QuestionActions, true);
            var questionIds = questionId.Split(',');
            var questions = quiz.Questions.Where(q => questionIds.Contains(q.Id));

            // If not all the question IDs matched, let's throw an exception, since that should never happen.
            if (questionIds.Count() != questions.Count())
            {
                throw new ArgumentException("QuestionSettings was passed a question ID that does not exist in the quiz.", "questionId");
            }

            // Are we getting a list of question IDs? If so, we need to handle that separately.
            if (questions.Count() > 1)
            {
                // If all of the questions have blank settings, then return the defaults.
                var allBlank = true;
                const int defaultPoints = 1;
                foreach (var q in questions)
                {
                    if (!q.AssessmentGroups.IsNullOrEmpty() || q.Points != defaultPoints || !String.IsNullOrEmpty(q.TimeLimit))
                    {
                        allBlank = false;
                        break;
                    }
                }
                if (allBlank)
                {
                    model = new QuestionSettings
                    {
                        Id = questionId,
                        QuizId = quizId,
                        EntityId = Context.EntityId,

                        AttemptLimit = new NumberOfAttempts
                        {
                            Attempts = ci.AssessmentSettings.AttemptLimit
                        },
                        TimeLimit = ci.AssessmentSettings.TimeLimit,
                        Points = defaultPoints,

                        // Review Settings
                        ShowScoreAfter = (ReviewSetting)ci.AssessmentSettings.ShowScoreAfter,
                        ShowQuestionsAnswers = (ReviewSetting)ci.AssessmentSettings.ShowQuestionsAnswers,
                        ShowRightWrong = (ReviewSetting)ci.AssessmentSettings.ShowRightWrong,
                        ShowAnswers = (ReviewSetting)ci.AssessmentSettings.ShowAnswers,
                        ShowFeedbackAndRemarks = (ReviewSetting)ci.AssessmentSettings.ShowFeedbackAndRemarks,
                        ShowSolutions = (ReviewSetting)ci.AssessmentSettings.ShowSolutions
                    };
                }
                else
                {
                    // For each property, if it is set to the same value for all questions, then
                    // use that value; otherwise use null (blank).
                    var assignmentGroups = questions.Map(q => AssessmentGroupForQuestion(ci, q.Id));
                    var commonAttempts = CommonOrBlank<String>(assignmentGroups.First().GetType().GetProperty("Attempts"), assignmentGroups);
                    var commonTimeLimitString = CommonOrBlank<string>(typeof(Question).GetProperty("TimeLimit"), questions);
                    int? commonTimeLimit = null;
                    if (!commonTimeLimitString.IsNullOrEmpty())
                    {
                        commonTimeLimit = Int32.Parse(commonTimeLimitString);
                    }
                    var reviewSettings = questions.Map(q => q.ReviewSettings);

                    model = new QuestionSettings()
                    {
                        Id = questionId,
                        AttemptLimit = (commonAttempts == null) ? new NumberOfAttempts { Attempts = -1 } : new NumberOfAttempts { Attempts = Int32.Parse(commonAttempts.ToString()) },
                        Points = (double?)CommonOrBlank<double?>(typeof(Question).GetProperty("Points"), questions),
                        TimeLimit = commonTimeLimit,
                        ShowScoreAfter = CommonOrBlank<ReviewSetting?>(typeof(ReviewSettings).GetProperty("ShowScoreAfter"), reviewSettings),
                        ShowQuestionsAnswers = CommonOrBlank<ReviewSetting?>(typeof(ReviewSettings).GetProperty("ShowQuestionsAnswers"), reviewSettings),
                        ShowRightWrong = CommonOrBlank<ReviewSetting?>(typeof(ReviewSettings).GetProperty("ShowRightWrong"), reviewSettings),
                        ShowSolutions = CommonOrBlank<ReviewSetting?>(typeof(ReviewSettings).GetProperty("ShowSolutions"), reviewSettings),
                        ShowFeedbackAndRemarks = CommonOrBlank<ReviewSetting?>(typeof(ReviewSettings).GetProperty("ShowFeedbackAndRemarks"), reviewSettings),
                        ShowAnswers = CommonOrBlank<ReviewSetting?>(typeof(ReviewSettings).GetProperty("ShowAnswers"), reviewSettings)
                    };
                }
                return View("DisplayTemplates/QuizPartials/QuestionSettings", model);
            }

            // Get the question object
            Bfw.PX.PXPub.Models.Question question = questions.First();
            if (!ci.AssessmentGroups.IsNullOrEmpty())
            {
                var ag = AssessmentGroupForQuestion(ci, questionId);
                // Build the view model
                model = new Bfw.PX.PXPub.Models.QuestionSettings
                {
                    Id = questionId,
                    QuizId = quizId,
                    EntityId = question.EntityId,
                    AttemptLimit = new NumberOfAttempts
                    {
                        Attempts = Int32.Parse(ag.Attempts)
                    },
                    Points = question.Points,
                    TimeLimit = int.Parse(ag.TimeLimit),
                };

                if (null != question.ReviewSettings)
                {
                    model.ShowScoreAfter = question.ReviewSettings.ShowScoreAfter;
                    model.ShowQuestionsAnswers = question.ReviewSettings.ShowQuestionsAnswers;
                    model.ShowRightWrong = question.ReviewSettings.ShowRightWrong;
                    model.ShowAnswers = question.ReviewSettings.ShowAnswers;
                    model.ShowFeedbackAndRemarks = question.ReviewSettings.ShowFeedbackAndRemarks;
                    model.ShowSolutions = question.ReviewSettings.ShowSolutions;
                }
                else if (null != ag.ReviewSettings)
                {
                    //model.ShowScoreAfter = (ReviewSetting)ag.ReviewSettings.ShowScoreAfter;
                    model.ShowQuestionsAnswers = (ReviewSetting)ag.ReviewSettings.ShowQuestionsAnswers;
                    model.ShowRightWrong = (ReviewSetting)ag.ReviewSettings.ShowRightWrong;
                    model.ShowAnswers = (ReviewSetting)ag.ReviewSettings.ShowAnswers;
                    model.ShowFeedbackAndRemarks = (ReviewSetting)ag.ReviewSettings.ShowFeedbackAndRemarks;
                    model.ShowSolutions = (ReviewSetting)ag.ReviewSettings.ShowSolutions;
                }

                // Render the question settings UI
                return View("DisplayTemplates/QuizPartials/QuestionSettings", model);
            }
            else
            {
                // Get the Homework settings
                BizDC.ContentItem homework = ContentActions.GetContent(Context.EntityId, quizId);
                model = new QuestionSettings
                {
                    Id = questionId,
                    QuizId = quizId,
                    EntityId = question.EntityId,

                    AttemptLimit = new NumberOfAttempts
                    {
                        Attempts = homework.AssessmentSettings.AttemptLimit
                    },
                    Points = question.Points,
                    TimeLimit = homework.AssessmentSettings.TimeLimit,

                    // Review Settings
                    ShowScoreAfter = (ReviewSetting)homework.AssessmentSettings.ShowScoreAfter,
                    ShowQuestionsAnswers = (ReviewSetting)homework.AssessmentSettings.ShowQuestionsAnswers,
                    ShowRightWrong = (ReviewSetting)homework.AssessmentSettings.ShowRightWrong,
                    ShowAnswers = (ReviewSetting)homework.AssessmentSettings.ShowAnswers,
                    ShowFeedbackAndRemarks = (ReviewSetting)homework.AssessmentSettings.ShowFeedbackAndRemarks,
                    ShowSolutions = (ReviewSetting)homework.AssessmentSettings.ShowSolutions
                };

                // Pass the settings to the view model
                return View("DisplayTemplates/QuizPartials/QuestionSettings", model);
            }
        }

        /// <summary>
        /// Gets the Learning curve question settings.
        /// </summary>
        /// <param name="quizId">The quiz id.</param>
        /// <param name="questionId">The question id.</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult LearningCurveQuestionSettings(string quizId, string questionId)
        {
            var model = GetLearningCurveQuestionSettings(quizId, questionId);
            return View("DisplayTemplates/LearningCurvePartials/QuestionSettings", model);
        }

        private LearningCurveQuestionSettings GetLearningCurveQuestionSettings(string quizId, string questionId)
        {
            LearningCurveQuestionSettings model = null;

            // We need to get the question objects from the IDs. This means getting the quiz and matching.
            var quiz = ContentActions.GetContent(Context.EntityId, quizId).ToQuiz(ContentActions, QuestionActions, true); ;


            var questionIds = questionId.Split(',');
            var questions = quiz.Questions.Where(q => questionIds.Contains(q.Id));

            // If not all the question IDs matched, let's throw an exception, since that should never happen.
            if (questionIds.Count() != questions.Count())
            {
                throw new ArgumentException("QuestionSettings was passed a question ID that does not exist in the quiz.", "questionId");
            }
            var question = questions.FirstOrDefault();
            var settings = question.LearningCurveQuestionSettings;
            if (settings != null)
            {
                model = (from c in settings where c.QuizId == quizId select c).FirstOrDefault();
                model.Id = question.Id;
                model.QuestionType = question.Type;
            }
            else
            {
                model = new LearningCurveQuestionSettings()
                {
                    QuizId = quizId,
                    DifficultyLevel = "1",
                    NeverScrambleAnswers = true,
                    PrimaryQuestion = false,
                    QuestionType = question.Type,
                    Id = question.Id

                };
            }
            return model;
        }

        private static BizDC.AssessmentGroup AssessmentGroupForQuestion(BizDC.ContentItem ci, string questionId)
        {
            var quizQuestionName = ci.Id + "_" + questionId;
            var ag = new BizDC.AssessmentGroup()
            {
                Attempts = "3",
                TimeLimit = "0"
            };

            foreach (var quizGrp in ci.AssessmentGroups)
            {
                if (quizQuestionName == quizGrp.Name)
                {
                    ag = quizGrp;
                }
            }
            return ag;
        }

        /// <summary>
        /// Updates the settings of a given question
        /// </summary>
        /// <param name="settings">The question settings</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult QuestionSettings(QuestionSettings settings)
        {
            // If we have a list of questions, handle that.
            var questionIds = settings.Id.Split(',');

            foreach (var questionId in questionIds)
            {
                var ci = ContentActions.GetContent(Context.EntityId, settings.QuizId);
                var quiz = ci.ToQuiz(ContentActions, QuestionActions, true);
                Bfw.PX.PXPub.Models.Question question = quiz.Questions.SingleOrDefault(x => x.Id == questionId);

                var questionSettings = new BizDC.QuestionSettings
                {
                    Id = question.Id,
                    QuizId = settings.QuizId,
                    EntityId = question.EntityId,
                    Points = settings.Points,
                    AttemptLimit = int.Parse(Request.Form["NumberOfAttempts.Attempts"]),
                    TimeLimit = settings.TimeLimit,
                    ReviewSettings = new BizDC.ReviewSettings
                    {
                        ShowScoreAfter = (BizDC.ReviewSetting)Enum.Parse(typeof(BizDC.ReviewSetting), Request.Form["AssessmentSettings.ShowScoreAfter"]),
                        ShowQuestionsAnswers = (BizDC.ReviewSetting)Enum.Parse(typeof(BizDC.ReviewSetting), Request.Form["AssessmentSettings.ShowQuestionsAnswers"]),
                        ShowRightWrong = (BizDC.ReviewSetting)Enum.Parse(typeof(BizDC.ReviewSetting), Request.Form["AssessmentSettings.ShowRightWrong"]),
                        ShowAnswers = (BizDC.ReviewSetting)Enum.Parse(typeof(BizDC.ReviewSetting), Request.Form["AssessmentSettings.ShowAnswers"]),
                        ShowFeedbackAndRemarks = (BizDC.ReviewSetting)Enum.Parse(typeof(BizDC.ReviewSetting), Request.Form["AssessmentSettings.ShowFeedbackAndRemarks"]),
                        ShowSolutions = (BizDC.ReviewSetting)Enum.Parse(typeof(BizDC.ReviewSetting), Request.Form["AssessmentSettings.ShowSolutions"])
                    }
                };

                QuestionActions.UpdateQuestionSettings(Context.EntityId, questionSettings);
            }

            return Json(true);
        }

        /// <summary>
        /// Learnings the curve question settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LearningCurveQuestionSettings(LearningCurveQuestionSettings settings)
        {
            if (!string.IsNullOrEmpty(settings.QuizId))
            {
                var questionSettings = new BizDC.LearningCurveQuestionSettings
                {
                    Id = settings.QuizId,
                    QuestionId = settings.Id,
                    PrimaryQuestion = settings.PrimaryQuestion,
                    NeverScrambleAnswers = settings.NeverScrambleAnswers,
                    DifficultyLevel = settings.DifficultyLevel
                };

                QuestionActions.UpdateLearningCurveQuestionSettings(Context.EntityId, questionSettings);
            }
            return Json(settings);
        }
        /// <summary>
        /// Updates the settings of a given question
        /// </summary>
        /// <param name="settings">The question settings</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult QuizQuestionSettings(QuizQuestionSettings settings)
        {
            // If we have a list of questions, handle that.
            var questionIds = settings.Id.Split(',');

            List<object> result = new List<object>();
            var ci = ContentActions.GetContent(Context.EntityId, settings.QuizId);
            if (ci == null)
                return Json(false);
            var quiz = ci.ToQuiz(ContentActions, QuestionActions, true);

            foreach (var questionId in questionIds)
            {
                Bfw.PX.PXPub.Models.Question question = quiz.Questions.SingleOrDefault(x => x.Id == questionId);

                if (null == question)
                {
                    result.Add(Json(new {Id = questionId, Success = false}));
                    continue;
                }

                var questionSettings = new BizDC.QuestionSettings
                {
                    Id = question.Id,
                    QuizId = settings.QuizId,
                    EntityId = question.EntityId,
                    Points = settings.Points,
                };

                QuestionActions.UpdateQuestionSettings(Context.EntityId, questionSettings);
            }

            return result.IsNullOrEmpty() ? Json(true) : Json(result);
        }

        /// <summary>
        /// Displays the quiz question settings
        /// </summary>
        /// <param name="quizId">Id of the quiz including the question</param>
        /// <param name="questionId">Id of the question</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult QuizQuestionSettings(string quizId, string questionId)
        {
            QuizQuestionSettings model;

            // We need to get the question objects from the IDs. This means getting the quiz and matching.
            var ci = ContentActions.GetContent(Context.EntityId, quizId);
            var quiz = ci.ToQuiz(ContentActions, QuestionActions, true);
            var questionIds = questionId.Split(',');
            var questions = quiz.Questions.Where(q => questionIds.Contains(q.Id));

            // If not all the question IDs matched, let's throw an exception, since that should never happen.
            if (questionIds.Count() != questions.Count())
            {
                throw new ArgumentException("QuestionSettings was passed a question ID that does not exist in the quiz.", "questionId");
            }

            // Are we getting a list of question IDs? If so, we need to handle that separately.
            if (questions.Count() > 1)
            {
                // If all of the questions have blank settings, then return the defaults.
                var allBlank = true;
                const int defaultPoints = 1;
                foreach (var q in questions)
                {
                    if (!q.AssessmentGroups.IsNullOrEmpty() || q.Points != defaultPoints || !String.IsNullOrEmpty(q.TimeLimit))
                    {
                        allBlank = false;
                        break;
                    }
                }
                if (allBlank)
                {
                    model = new QuizQuestionSettings
                    {
                        Id = questionId,
                        QuizId = quizId,
                        EntityId = Context.EntityId,

                        Points = defaultPoints
                    };
                }
                else
                {
                    // For each property, if it is set to the same value for all questions, then
                    // use that value; otherwise use null (blank).
                    var assignmentGroups = questions.Map(q => AssessmentGroupForQuestion(ci, q.Id));
                    model = new QuizQuestionSettings()
                    {
                        Id = questionId,
                        Points = (double?)CommonOrBlank<double?>(typeof(Question).GetProperty("Points"), questions)
                    };
                }
                return View("~/Views/Shared/DisplayTemplates/QuizPartials/QuizQuestionSettings.ascx", model);
            }

            // Get the question object
            Bfw.PX.PXPub.Models.Question question = questions.First();
            if (!ci.AssessmentGroups.IsNullOrEmpty())
            {
                var ag = AssessmentGroupForQuestion(ci, questionId);
                // Build the view model
                model = new QuizQuestionSettings
                {
                    Id = questionId,
                    QuizId = quizId,
                    EntityId = question.EntityId,
                    Points = question.Points
                };

                // Render the question settings UI
                return View("~/Views/Shared/DisplayTemplates/QuizPartials/QuizQuestionSettings.ascx", model);
            }
            else
            {
                // Get the quiz settings
                BizDC.ContentItem quiz2 = ContentActions.GetContent(Context.EntityId, quizId);
                model = new QuizQuestionSettings
                {
                    Id = questionId,
                    QuizId = quizId,
                    EntityId = question.EntityId,
                    Points = question.Points
                };

                // Pass the settings to the view model
                return View("~/Views/Shared/DisplayTemplates/QuizPartials/QuizQuestionSettings.ascx", model);
            }
        }

        /// <summary>
        /// Gets the content of the related.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>        
        public ActionResult GetRelatedContent(string itemId)
        {
            var model = GetRelatedContentModel(itemId);
            return View("~/Views/Shared/DisplayTemplates/QuizPartials/RelatedContent.ascx", model);
        }

        /// <summary>
        /// Gets the related content model.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <returns></returns>
        public RelatedContent GetRelatedContentModel(string itemId)
        {
            string relatedContentId = ContentHelper.GetRelatedContentID(itemId);
            RelatedContent model = null;
            if (!string.IsNullOrEmpty(Context.EntityId) && !string.IsNullOrEmpty(relatedContentId))
            {
                //TODO - changed the below dummy id.
                var ci = ContentActions.GetContent(Context.EntityId, relatedContentId).ToRelatedContent(ContentActions, true);
                model = ci;
            }
            return model;
        }


        /// <summary>
        /// Saves the content of the related.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveRelatedContent(string itemId, string title)
        {
            //get the related content
            string relatedContentId = ContentHelper.GetRelatedContentID(itemId);
            if (!string.IsNullOrEmpty(Context.EntityId) && !string.IsNullOrEmpty(relatedContentId))
            {
                var ci = ContentActions.GetContent(Context.EntityId, relatedContentId);
                RelatedContent model = null;
                if (ci == null)
                {
                    model = ContentHelper.CreateNewRelatedContent(relatedContentId, string.Empty);
                }
                if (ci != null)
                {
                    ci.Title = title;
                    ContentActions.StoreContent(ci);
                    return Json(new { status = "success" });
                }
            }
            return Json(new { status = "failed" });
        }

        /// <summary>
        /// Removes the content of the related.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveRelatedContent(string itemId, string itemIdToRemove)
        {
            //Get the RelatedContent
            string relatedContentId = ContentHelper.GetRelatedContentID(itemId);
            RelatedContent model = null;
            if (!string.IsNullOrEmpty(Context.EntityId) && !string.IsNullOrEmpty(relatedContentId))
            {
                var ci = ContentActions.GetContent(Context.EntityId, relatedContentId);
                if (ci != null)
                {
                    var itemToRemove = (from c in ci.RelatedContents where c.Id == itemIdToRemove select c).FirstOrDefault();
                    if (itemToRemove != null)
                    {
                        ci.RelatedContents.Remove(itemToRemove);
                        ContentActions.StoreContent(ci);
                    }
                    model = ci.ToRelatedContent(ContentActions, true);
                }
            }
            return View("~/Views/Shared/DisplayTemplates/QuizPartials/RelatedContent.ascx", model);
        }

        /// <summary>
        /// Gets the content of the related.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddRelatedContent(string itemId, string itemIdToAdd)
        {
            string relatedContentId = ContentHelper.GetRelatedContentID(itemId);
            RelatedContent model = null;
            if (!string.IsNullOrEmpty(Context.EntityId) && !string.IsNullOrEmpty(relatedContentId))
            {
                var ci = ContentActions.GetContent(Context.EntityId, relatedContentId);
                if (ci == null)
                {
                    //Create a new related Content ID.
                    model = ContentHelper.CreateNewRelatedContent(relatedContentId, itemIdToAdd);
                }
                if (ci != null)
                {
                    if (!string.IsNullOrEmpty(itemIdToAdd))
                    {
                        var itemToAdd = new RelatedContent()
                        {
                            Id = itemIdToAdd
                        }.ToRelatedContent();

                        if (ci.RelatedContents == null)
                        {
                            ci.RelatedContents = new List<BizDC.RelatedContent>();
                        }
                        ci.RelatedContents.Add(itemToAdd);
                        ContentActions.StoreContent(ci);
                    }
                    model = ci.ToRelatedContent(ContentActions, true);
                }
            }
            return View("~/Views/Shared/DisplayTemplates/QuizPartials/RelatedContent.ascx", model);
        }

        /// <summary>
        /// Gets the learning curve individual question preview URL.
        /// </summary>
        /// <param name="questionId">The question id.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetLearningCurveIndividualQuestionPreviewUrl(string questionId)
        {
            string url = string.Empty;
            var model = new LearningCurveActivity()
            {
                EnrollmentId = Context.EnrollmentId,
                DefaultQuestionId = questionId
            };
            url = LearningCurveHelper.GetPlayerUrl(model, string.Empty);

            return Json(new { url = url });
        }

        /// <summary>
        /// Returns quiz's template id
        /// </summary>
        /// <returns></returns>
        public ActionResult QuizTemplateId()
        {

            var templates = ContentActions.GetAllTemplates();
            var id = templates.Where(i => i.Title == "Quiz" || i.Title == "quiz").FirstOrDefault().Id;
            ViewData["templateId"] = id;

            return View("~/Views/Shared/DisplayTemplates/QuizPartials/QuizTemplateId.ascx");

        }

        
        /// <summary>
        /// Gets a single question by question id.
        /// </summary>
        /// <param name="id">The requested question's id.</param>
        /// <param name="singleQuestion"></param>
        /// <returns></returns>
        public ActionResult GetSingleQuestion( SingleQuestion singleQuestion)
        {
            if (CheckForNull(singleQuestion))
            {
                return new EmptyResult();
            }

            var ci = ContentActions.GetContent(singleQuestion.EntityId, singleQuestion.QuizId);

            var quiz = ci.ToQuiz(ContentActions, QuestionActions, false) ?? new Quiz()
            {
                Id = singleQuestion.QuizId,
                Questions = new List<Question>()
            };

            var question = QuestionActions.GetQuestion(Context.EntityId, singleQuestion.QuestionId);
            if (question == null)
            {
                question = QuestionActions.GetQuestion(singleQuestion.EntityId, singleQuestion.QuestionId);
            }
            else
            {
                singleQuestion.EntityId = Context.EntityId;
            }
            var quizQuestion = quiz.Questions.FirstOrDefault(q => q.Id == question.Id);
            if (quizQuestion != null)
            {
                double points = 1.0;
                Double.TryParse(quizQuestion.Score, out points);
                question.Points = points;
                question.BankUse = quizQuestion.BankUse;
            }
            ViewData["question"] = question.ToQuestion();
            quiz.ShowReused = singleQuestion.ShowReused.HasValue && singleQuestion.ShowReused.Value;

            InitializeViewDataValues(singleQuestion);
            quiz.CourseInfo = Context.Course.ToCourse();

            return View("~/Views/Shared/DisplayTemplates/QuizPartials/SingleQuestion.ascx", quiz);
            
        }

        private void InitializeViewDataValues(SingleQuestion singleQuestion)
        {
            ViewData["allowSelection"] = singleQuestion.AllowSelection;
            ViewData["allowDrag"] = singleQuestion.AllowDrag;
            ViewData["showExpand"] = singleQuestion.ShowExpand;
            ViewData["showAddLink"] = singleQuestion.ShowAddLink;
            ViewData["showPoints"] = singleQuestion.ShowPoints;
            ViewData["extraClass"] = singleQuestion.ExtraClass;
            ViewData["isOdd"] = singleQuestion.IsOdd;
            ViewData["isReused"] = singleQuestion.IsReused;
            ViewData["questionEditedType"] = singleQuestion.QuestionEditedType;
            ViewData["isPrimary"] = singleQuestion.IsPrimary;
            ViewData["mode"] = singleQuestion.Mode;
            ViewData["isQuestionOverview"] = singleQuestion.IsQuestionOverview;
            ViewData["mainQuizId"] = singleQuestion.MainQuizId??string.Empty;
            ViewData["isPoolQuestion"] = singleQuestion.IsPoolQuestion;
            ViewData["questionNumber"] = singleQuestion.QuestionNumber??string.Empty;

        }

        private bool CheckForNull(SingleQuestion singleQuestion)
        {
            if (singleQuestion.QuizId == null || singleQuestion.QuestionId == null || singleQuestion.EntityId == null ||
                singleQuestion.AllowSelection == null ||
                singleQuestion.AllowDrag == null || singleQuestion.ShowExpand == null ||
                singleQuestion.ShowAddLink == null ||
                singleQuestion.ShowPoints == null || singleQuestion.ExtraClass == null || singleQuestion.IsOdd == null ||
                singleQuestion.IsReused == null ||
                singleQuestion.QuestionEditedType == null || singleQuestion.IsPrimary == null ||
                singleQuestion.Mode == null ||
                singleQuestion.IsQuestionOverview == null || singleQuestion.IsPoolQuestion == null)
            {
                return true;
            }
            return false;
        }

        public ActionResult ShowAddToNewAssessment(string questionId)
        {
            var assignedWidget = this.PageActions.GetWidget("PX_LAUNCHPAD_MOVECOPY_WIDGET").ToWidgetItem();

            var templates = (from s in ContentActions.GetAllTemplates()
                             where s.Title.Equals("Quiz") || s.Title.Equals("Homework") || s.Title.Equals("Learning Curve")
                             select new SelectListItem() { Text = s.Title, Value = s.Id }).ToList();

            ViewData["templates"] = templates;
            ViewData["questionId"] = questionId;

            return View(new List<Widget> { assignedWidget });
        }

        public JsonResult QuestionFilter()
        {
            if (Context != null && Context.Course != null)
            {
                var course = Context.Course.ToCourse();
                if (course != null && course.QuestionFilter != null)
                {
                    var questionFilter = course.QuestionFilter;

                    List<QuestionFilterMetadata> model = new List<QuestionFilterMetadata>();

                    if (questionFilter != null && questionFilter.FilterMetadata != null &&
                        questionFilter.FilterMetadata.Count() > 0)
                    {
                        model = questionFilter.FilterMetadata.ToList();
                    }

                    return Json(new { FilterMetadata = model }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(String.Empty);
        }

        /// <summary>
        /// We want to make sure that the item has gradable set to true and there is a weight so that
        /// it will show up in the BH gradebook. If there is an existing non 0 weight, it won't be changed
        /// </summary>
        /// <param name="itemId">ID of the quiz we want to make sure is gradeable</param>
        /// <returns>True if it was successful, false otherwise.</returns>
        public JsonResult MakeQuizGradable(string itemId)
        {
            return Json(QuizHelper.MakeQuizGradable(itemId, ContentActions, Context));
        }

        public JsonResult MakeQuizGradableIfSubmitted(string itemId)
        {
            var result = false;
            var gradelist = GradeActions.GetGradeList(Context.EnrollmentId, itemId );
            if (!string.IsNullOrWhiteSpace(gradelist.Status))
            {
                result = QuizHelper.MakeQuizGradable(itemId, ContentActions, Context);
            }
            return Json(result);
        }

        /// <summary>
        /// Change the quiz's due date and attempt limit so that it can be taken by instructor in student view
        /// </summary>
        /// <param name="quiz"></param>
        private void MakeQuizTakableByInstructor(Quiz quiz)
        {
            //If the quiz is not assigned, we don't need to worry about the quiz past due.
            if (quiz.DueDate.Year == DateTime.MinValue.Year)
                return;

            var item = ContentActions.GetContent(Context.EnrollmentId, quiz.Id);
            if (item.AssignmentSettings == null)
            {
                item.AssignmentSettings = new BizDC.AssignmentSettings {DueDate = DateTime.MinValue};
            }
            else
            {
                if (item.AssignmentSettings.DueDate.Year == DateTime.MinValue.Year)
                {
                    return;
                }
                else
                {
                    item.AssignmentSettings.DueDate = DateTime.MinValue;
                }
            }
            if (item.AssessmentSettings == null)
            {
                item.AssessmentSettings = new BizDC.AssessmentSettings {AttemptLimit = 0};
            }
            else
            {
                item.AssessmentSettings.AttemptLimit = 0;
            }

            ContentActions.StoreContent(item);
            
        }
    }
}