using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Microsoft.Practices.ServiceLocation;

namespace Bfw.PX.PXPub.Controllers.Helpers
{
    public class AssignmentHelper
    {
        
        /// <summary>
        /// Get an Assignment policy based on the Assignment type and Settings.
        /// </summary>
        /// <param name="assignment">The assignment.</param>
        /// <param name="contentActions">The content actions.</param>
        /// <returns></returns>
        public static IList<String> AssignmentPolicyFromSettings(BizDC.ContentItem assignment, IContentActions contentActions)
        {
            return assignment.AssignmentSettings.IsAssignable ?
                AssignmentPolicyFromSettings(contentActions, new Assignment { SubType = assignment.Subtype, }, assignment.AssignmentSettings) :
                new List<string>();
        }

        /// <summary>
        /// Get an Assignment policy based on the Assignment type and Settings.
        /// </summary>
        /// <param name="assignment">The assignment.</param>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        public static IList<String> AssignmentPolicyFromSettings(BizSC.IContentActions contentActions, Assignment assignment, AssignmentSettings settings, bool isItemLocked = false)
        {
            IList<String> results = new List<String>();

            //ASSIGNEMENT
            //mishka, show only for instructor
            //if assignment exists -> "Edit Assignment", if not -> "Assign this folder" links
            // Here all these Policies are not for FACEPLACE COURSE.
            if (contentActions.Context.Course.bfw_tab_settings.view_tab.show_assignment_details == true && contentActions.Context.Course.CourseType != Biz.DataContracts.CourseType.FACEPLATE.ToString())
            {
                if (contentActions != null)
                {
                    string assignmentMessage = "<h2>Assignment Details</h2> ";

                    var assignmentType = string.Empty;
                    switch (assignment.SubType.ToLowerInvariant())
                    {
                        case "eportfolio":
                            assignmentType = "folder";
                            break;
                        case "reflectionassignment":
                            assignmentType = "assignment";
                            break;
                    }
                    if (settings.DueDate.ToShortDateString() != DateTime.MinValue.ToShortDateString())
                    {
                        assignmentMessage += string.Format("<span id=\"assigndetails\">This {0} is due on {1} at {2}.</span>", assignmentType, settings.DueDate.ToString("MM/dd/yyyy"), settings.DueDate.ToString("hh:mm tt"));
                    }
                    else
                    {
                        assignmentMessage += string.Format("<div class='late-prompt'>This {0} has not been assigned yet.</div>", assignmentType);
                    }

                    if (settings.AllowLateSubmission)
                    {
                        assignmentMessage += "<span id=\"gradablemessage\">This assignment is gradable and will accept late submissions.</span>";
                    }

                    if (contentActions.Context.AccessLevel == AccessLevel.Instructor)
                    {
                        var assignTabLinkClass = isItemLocked ? "AssignTabLinkIdDisabled" : "AssignTabLinkId";
                        var assignTabLinkHref = isItemLocked ? string.Empty : "href='#'";
                        var assignTabLinkTitle = isItemLocked ? " title=\"This resource is locked by the template\" " : string.Empty;
                        if (settings.meta_bfw_Assigned != true)
                        {
                            assignmentMessage += String.Format("<a " + assignTabLinkHref + assignTabLinkTitle + " id='" + assignTabLinkClass + "' > Assign this {0}.</a>", assignmentType);
                        }
                        else if (contentActions.Context.Course.CourseType != Biz.DataContracts.CourseType.FACEPLATE.ToString())
                        {
                            assignmentMessage += String.Format("<a " + assignTabLinkHref + assignTabLinkTitle + " id='" + assignTabLinkClass + "' > Edit this {0}</a>", assignmentType);
                        }
                    }


                    assignmentMessage = "<div class='assignmentsection'>" + "<div class='icon'></div>" + assignmentMessage + "</div>";
                    results.Add(assignmentMessage);
                }
            }

            return results;
        }

        public static DateTime GetGraceDueDate(BizDC.AssignmentSettings assignmentSettings)
        {
            if (assignmentSettings == null)
                return DateTime.MinValue;

            return GetGraceDueDate(assignmentSettings.DueDate, assignmentSettings.LateGraceDuration, assignmentSettings.LateGraceDurationType);
        }

        public static DateTime GetGraceDueDate(DateTime dueDate, long graceDuration, string graceDurationType)
        {
            var graceDueDate = dueDate;

            if (graceDuration > 0 && !string.IsNullOrEmpty(graceDurationType))
            {
                LateGraceDurationType lateGraceDurationType;
                Enum.TryParse(graceDurationType, true, out lateGraceDurationType);
                switch (lateGraceDurationType)
                {
                    case LateGraceDurationType.Minute:
                        graceDueDate = graceDueDate.AddMinutes(graceDuration);
                        break;
                    case LateGraceDurationType.Hour:
                        graceDueDate = graceDueDate.AddHours(graceDuration);
                        break;
                    case LateGraceDurationType.Day:
                        graceDueDate = graceDueDate.AddDays(graceDuration);
                        break;
                    case LateGraceDurationType.Week:
                        graceDueDate = graceDueDate.AddDays(graceDuration * 7);
                        break;
                    case LateGraceDurationType.Infinite:
                        graceDueDate = DateTime.MaxValue;
                        break;
                }
            }
            return graceDueDate;
        }
        /// <summary>
        /// Given an assessment settings object, adds textual descriptions of all settings
        /// that are not in their default states.
        /// </summary>
        /// <param name="assessmentSettings">The assessment settings.</param>
        /// <param name="quizTypeName">Name of the quiz type.</param>
        /// <returns></returns>
        public static IList<String> PolicyDescriptionFromSettings(BizDC.AssessmentSettings assessmentSettings, string quizTypeName)
        {
            return PolicyDescriptionFromSettings(null, assessmentSettings, quizTypeName);
        }

        /// <summary>
        /// Given an assessment settings object, adds textual descriptions of all settings
        /// that are not in their default states.
        /// </summary>
        /// <param name="assignmentSettings">The assignment settings.</param>
        /// <param name="assessmentSettings">The assessment settings.</param>
        /// <param name="quizTypeName">Name of the quiz type.</param>
        /// <returns></returns>
        public static IList<String> PolicyDescriptionFromSettings(AssignmentSettings assignmentSettings,
            BizDC.AssessmentSettings assessmentSettings, string quizTypeName)
        {
            var context = ServiceLocator.Current.GetInstance<IBusinessContext>();
            return PolicyDescriptionFromSettings(context, assignmentSettings, assessmentSettings, quizTypeName);
        }


        /// <summary>
        /// Given an assessment settings object, adds textual descriptions of all settings
        /// that are not in their default states.
        /// </summary>
        /// <param name="context">Business Context</param>
        /// <param name="assignmentSettings">The assignment settings.</param>
        /// <param name="assessmentSettings">The assessment settings</param>
        /// <param name="quizTypeName">Name of the quiz type.</param>
        /// <returns></returns>
        public static IList<String> PolicyDescriptionFromSettings(IBusinessContext context, AssignmentSettings assignmentSettings, BizDC.AssessmentSettings assessmentSettings, string quizTypeName)
        {
            IList<String> results = new List<String>();

            if (assessmentSettings != null)
            {
                var accesslevel = "";

                if (context.AccessLevel == AccessLevel.Instructor)
                {
                    accesslevel = "Students";
                }
                else
                {
                    accesslevel = "You";
                }

                if (context.ImpersonateStudent)
                {
                    results.Add("<div style='background-color:#FFCCCC; border: 1px solid #333333; padding: 10px'>Quizzes are always accessible in Student View for testing purposes. Actual students cannot take a quiz if it is unassigned, if the due date has passed, or if they have exhausted all attempts.</div>");
                }
                if (context.AccessLevel == AccessLevel.Student && assessmentSettings.DueDate.Year <= 1)
                {
                    //item is not assigned, show nothing for students
                }
                else
                {
                    if (assessmentSettings.DueDate.Year != DateTime.MinValue.Year)
                    {
                        var graceDueDate = GetGraceDueDate(assignmentSettings);

                        if (assessmentSettings.DueDate < DateTime.Now.GetCourseDateTime() && graceDueDate <= assessmentSettings.DueDate)
                        {
                            results.Add("The due date for this assignment has expired.");
                        }
                        else
                        {
                            results.Add(String.Format("{0} must complete this assignment by {1}{2}.", accesslevel,
                                assessmentSettings.DueDate.GetCourseDateTime().ToShortDateString() + " " +
                                assessmentSettings.DueDate.GetCourseDateTime().ToShortTimeString() + " " +
                                context.Course.GetCourseTimeZoneAbbreviation(),
                                string.Empty));
                            //Remove grade period text (PX-8564)
                            // == assessmentSettings.DueDate ? string.Empty : " with grace period until " + graceDueDate.GetCourseDateTime().ToShortDateString() + " " + graceDueDate.GetCourseDateTime().ToShortTimeString() + " " + context.Course.GetCourseTimeZoneAbbreviation()));
                        }
                    }

                    if (assessmentSettings.QuizType == QuizType.Assessment.ToString())
                    {
                        // Attempt Limit
                        if (assessmentSettings.AttemptLimit == 1)
                        {
                            results.Add(String.Format("{0} may attempt the quiz once.", accesslevel));
                        }
                        else if (assessmentSettings.AttemptLimit == 0) // Unlimited
                        {
                            results.Add(String.Format("{0} may attempt the quiz an unlimited number of times before the due date.", accesslevel));
                        }
                        else if (assessmentSettings.AttemptLimit > 1)
                        {
                            results.Add(String.Format("{0} may attempt the quiz {1} times.", accesslevel, assessmentSettings.AttemptLimit));
                        }

                        //Scored attempt
                        UpdatePolicyForScoredAttempts(assessmentSettings, results, context);

                        // Time limit
                        if (assessmentSettings.TimeLimit == 0)
                        {
                            results.Add("There is no time limit for this quiz.");
                        }
                        else if (assessmentSettings.TimeLimit > 0)
                        {
                            results.Add(String.Format("There is a {0} minute time limit for this quiz.", assessmentSettings.TimeLimit));
                        }

                        // Question delivery
                        if (assessmentSettings.QuestionDelivery == BizDC.QuestionDelivery.OneNoBacktrack)
                        {
                            results.Add("Questions will be presented one at a time; backtracking is not allowed.");
                        }

                        // Save and continue
                        UpdatePolicyForSaveAndContinue(assessmentSettings, results, context);
                    }
                    else if (assessmentSettings.QuizType == QuizType.Homework.ToString())
                    {
                        //Scored attempt
                        UpdatePolicyForScoredAttempts(assessmentSettings, results, context);

                        // Save and continue
                        UpdatePolicyForSaveAndContinue(assessmentSettings, results, context);
                    }
                }
            }


            if (results.IsNullOrEmpty() && context.AccessLevel == AccessLevel.Instructor)
            {
                results = new List<String>(new String[] { String.Format("{0} is using all default settings", System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(quizTypeName)) });
            }

            return results;
        }

        /// <summary>
        /// Updates the policy for save and continue.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="results">The results.</param>
        /// <param name="context">The context.</param>
        private static void UpdatePolicyForSaveAndContinue(BizDC.AssessmentSettings settings, IList<string> results, IBusinessContext context)
        {
            string accesslevel = (context.AccessLevel == AccessLevel.Instructor) ? "Students" : "You";
            string your = (context.AccessLevel == AccessLevel.Instructor) ? "their" : "your";
            string quizType = (settings.QuizType == QuizType.Homework.ToString()) ? "a question" : "the quiz";

            if (settings.AllowSaveAndContinue == true)
            {
                results.Add(String.Format("{0} will be able to save {1} progress and return to {2} later after starting.", accesslevel, your.ToLower(), quizType));
            }
            else if (settings.AllowSaveAndContinue == false)
            {
                results.Add(String.Format("{0} will NOT be able to save {1} progress and return to {2} later after starting.", accesslevel, your.ToLower(), quizType));
            }
        }


        /// <summary>
        /// Updates the policy for scored attempts.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="results">The results.</param>
        /// <param name="context">The context.</param>
        private static void UpdatePolicyForScoredAttempts(BizDC.AssessmentSettings settings, IList<string> results, IBusinessContext context)
        {
            //GradeRule                    
            string gradePolicyText = string.Empty;
            var level = string.Empty;
            var isHomework = settings.QuizType == QuizType.Homework.ToString();

            if (settings.GradeRule == BizDC.GradeRule.Average)
            {
                gradePolicyText = context.AccessLevel == AccessLevel.Instructor ? 
                    String.Format("The average of each student's attempts will be counted as their score{0}.", isHomework ? " for that question" : string.Empty) :
                    String.Format("The average of your attempts will be counted as your score{0}.", isHomework ? " for that question" : string.Empty);
            }
            else
            {
                switch (settings.GradeRule)
                {
                    case BizDC.GradeRule.Highest:
                        level = "highest";
                        break;

                    case BizDC.GradeRule.First:
                        level = "first";
                        break;

                    case BizDC.GradeRule.Last:
                        level = "last";
                        break;

                    case BizDC.GradeRule.Lowest:
                        level = "lowest";
                        break;
                }

                gradePolicyText = context.AccessLevel == AccessLevel.Instructor ?
                    String.Format("Each student's {0} attempt will be counted as their score{1}.", level, isHomework ? " for that question" : string.Empty) :
                    String.Format("Your {0} attempt will be counted as your score{1}.", level, isHomework ? " for that question" : string.Empty);
            }

            if (!string.IsNullOrEmpty(gradePolicyText))
            {
                results.Add(gradePolicyText);
            }
        }

        
        /// <summary>
        /// Sets the assignment settings.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        internal static Models.ContentItem SetAssignmentSettings(Models.ContentItem item, BizDC.AssignmentSettings settings)
        {
            if (settings != null)
            {
                item.DueDate = settings.DueDate;
                item.DueDateTZ = settings.DueDateTZ;
                item.StartDateTZ = settings.StartDateTZ;
                item.MaxPoints = settings.Points;                
                item.CategorySequence = settings.CategorySequence;
                item.GradeBookWeightCategoryId = settings.Category;
                item.GradeReleaseDate = settings.GradeReleaseDate;
                item.IsGradable = settings.IsAssignable;
            }

            return item;
        }

        public static UrlHelper GetUrlHelper()
        {
            var httpContext = new HttpContextWrapper(HttpContext.Current);
            return new UrlHelper(new RequestContext(httpContext, CurrentRoute(httpContext)));
        }

        public static RouteData CurrentRoute(HttpContextWrapper httpContext)
        {
            return RouteTable.Routes.GetRouteData(httpContext);
        }

        #region Assignment

        /// <summary>
        /// Gets the list of assignment units.
        /// </summary>
        /// <param name="contentActions">The content actions.</param>
        /// <param name="entityId">The entity unique identifier.</param>
        /// <param name="containerId">The container unique identifier.</param>
        /// <param name="subcontainerId">The subcontainer unique identifier.</param>
        /// <param name="toc">The toc.</param>
        /// <param name="selectedId">The selected unique identifier.</param>
        /// <returns></returns>
        public static List<AssignmentUnit> GetAssignmentUnits(BizSC.IContentActions contentActions, string entityId, 
                                                              string containerId, string subcontainerId, string toc, string selectedId = "")
        {
            var listUnits = new List<AssignmentUnit>();

            var listItems = contentActions.GetContainerItems(entityId, containerId, subcontainerId, toc);
            if (listItems == null)
            {
                return listUnits;
            }

            var contentItems = listItems as BizDC.ContentItem[] ?? listItems.ToArray();
            if (contentItems.Any())
            {
                listUnits = contentItems.Map(biz => biz.ToAssignmentUnit())
                                        .ToList();
                var selTitle = listUnits.FirstOrDefault(u => !String.IsNullOrEmpty(u.Id)
                                                             && u.Id == selectedId);
                if (selTitle != null)
                {
                    selTitle.Selected = true;
                }
            }

            return listUnits;
        }

        #endregion

        /// <summary>
        /// Makes and item an assigned item.
        /// </summary>
        /// <param name="itemId">The item unique identifier.</param>
        /// <param name="dueYear">The due year.</param>
        /// <param name="dueMonth">The due month.</param>
        /// <param name="dueDay">The due day.</param>
        /// <param name="dueHour">The due hour.</param>
        /// <param name="dueMinute">The due minute.</param>
        /// <param name="dueAmpm">The due am/pm.</param>
        /// <param name="behavior">The behavior.</param>
        /// <param name="isMultipartLessons">Whether this request is being made from multipart lessons.</param>
        /// <param name="completionTrigger">The completion trigger.</param>
        /// <param name="gradebookCategory">The gradebook category.</param>
        /// <param name="syllabusFilter">The syllabus filter.</param>
        /// <param name="points">The points.</param>
        /// <param name="rubricId">The rubric unique identifier.</param>
        /// <param name="isGradeable">if set to <c>true</c> [is gradeable].</param>
        /// <param name="isAllowLateSubmission">if set to <c>true</c> [is allow late submission].</param>
        /// <param name="isSendReminder">if set to <c>true</c> [is send reminder].</param>
        /// <param name="reminderDurationCount">The reminder duration count.</param>
        /// <param name="reminderDurationType">Type of the reminder duration.</param>
        /// <param name="reminderSubject">The reminder subject.</param>
        /// <param name="reminderBody">The reminder body.</param>
        /// <param name="IncludeGbbScoreTrigger">The include GBB score trigger.</param>
        /// <param name="isHighlightLateSubmission">if set to <c>true</c> [is highlight late submission].</param>
        /// <param name="isAllowLateGracePeriod">if set to <c>true</c> [is allow late grace period].</param>
        /// <param name="lateGraceDuration">Duration of the late grace.</param>
        /// <param name="lateGraceDurationType">Type of the late grace duration.</param>
        /// <param name="CalculationTypeTrigger">The calculation type trigger.</param>
        /// <param name="contentActions">The content actions.</param>
        /// <param name="rubricActions">The rubric actions.</param>
        /// <param name="entityId">The entity unique identifier.</param>
        /// <param name="isAllowExtraCredit">if set to <c>true</c> [is allow extra credit].</param>
        /// <returns></returns>
        public static Models.AssignedItem AssignItem(string itemId, int dueYear, int dueMonth, int dueDay, int dueHour, int dueMinute, string dueAmpm, string behavior, bool? isMultipartLessons,
            string completionTrigger, string gradebookCategory, string syllabusFilter, int points, string rubricId, bool isGradeable, bool isAllowLateSubmission, bool isSendReminder,
            int reminderDurationCount, string reminderDurationType, string reminderSubject, string reminderBody, int IncludeGbbScoreTrigger, bool isHighlightLateSubmission, bool isAllowLateGracePeriod,
            long lateGraceDuration, string lateGraceDurationType, string CalculationTypeTrigger, IContentActions contentActions, IRubricActions rubricActions, string entityId, bool isAllowExtraCredit)
        {
            var result = new JsonResult();

            var ai = contentActions.GetContent(entityId, itemId).ToAssignedItem();

            switch (behavior.ToLower())
            {
                case "assign":
                    //if (ai.Score == null) ai.Score = new Score { Correct = 0.0, Possible = 0.0, Date = DateTime.Today };
                    ai.Score = new Score() { Possible = points };
                    if (ai.Score.Possible < 0 || ai.Score.Possible > 100)
                    {
                        result.Data = new Dictionary<string, string> { { "error", "Points should be between 0 and 100" } };
                        break;
                    }

                    dueHour %= 12;
                    dueHour = (dueAmpm.ToLower() == "pm") ? dueHour + 12 : dueHour;
                    ai.DueDate = new DateTime(dueYear, dueMonth,
                                              dueDay, dueHour, dueMinute, 0);
                    //PRODUCTION CHANGE - allow due dates in the past    
                    //if (ai.DueDate < DateTime.Now.GetCourseDateTime())
                    //{
                    //    result.Data = new Dictionary<string, string> { { "error", "Due date/time cannot be lesser than current date/time." } };
                    //    break;
                    //}

                    ai.CompletionTrigger = (Models.CompletionTrigger) int.Parse(completionTrigger); //Enum.Parse(typeof(Models.CompletionTrigger), completionTrigger, true);
                    ai.Category = gradebookCategory;
                    ai.SyllabusFilter = syllabusFilter;
                    ai.IsGradeable = isGradeable;
                    ai.IsAllowLateSubmission = isAllowLateSubmission;
                    ai.IsHighlightLateSubmission = isHighlightLateSubmission;
                    ai.IsAllowLateGracePeriod = isAllowLateGracePeriod;
                    ai.IsAllowExtraCredit = isAllowExtraCredit;
                    ai.LateGraceDuration = lateGraceDuration;
                    ai.LateGraceDurationType = lateGraceDurationType;
                    ai.IsSendReminder = isSendReminder;

                    // ai.GradeRule = !string.IsNullOrEmpty(SubmissionGradeAction) ? (ScoredAttempt)Enum.Parse(typeof(ScoredAttempt), SubmissionGradeAction) : ScoredAttempt.Highest;

                    if (isSendReminder)
                    {
                        // ai.ReminderEmail = new ReminderEmail { AssignmentId = itemId, Body = reminderBody, Subject = reminderSubject, DaysBefore = reminderDurationCount, DurationType = reminderDurationType, AssignmentDate = DateTime.Now };
                    }

                    ai.IncludeGbbScoreTrigger = (completionTrigger == "1") ? IncludeGbbScoreTrigger : 0;


                    //return ai;

                    break;

                case "unassign":
                    //AssignmentActions.Unassign(ai.ToContentItem(Context.EntityId, ContentActions));
                    //ai.StartDate = DateTime.MinValue;
                    //ai.DueDate = DateTime.MinValue;
                    //ViewData["message"] = "unassign successful";
                    //ai.IsImportant = false;
                    //ai.IncludeGbbScoreTrigger = 0;
                    break;
            }

            return ai;
        }

        #region Submission
        
        /// <summary>
        /// Unsubmit Student Grade
        /// </summary>
        /// <param name="assignmentId"></param>
        /// <param name="studentId"></param>
        public static ActionResult UnsubmitGrade(BizSC.IGradeActions gradeActions, string assignmentId, string studentId)
        {
            var teacherResponse = gradeActions.GetTeacherResponse(studentId, assignmentId);
            teacherResponse.Status = BizDC.GradeStatus.NeedsGrading;
            gradeActions.AddTeacherResponse(studentId, assignmentId, teacherResponse);
            return new EmptyResult();
        }

        public static List<TeacherResponse> SaveRubricScoreIntoTeacherResponse(string rubricScore)
        {
            string[] rubricScores = rubricScore.Split(',');
            List<TeacherResponse> rubricScoreList = new List<TeacherResponse>();
            for (int i = 0; i < rubricScores.Length; i = i + 2)
            {
                if (!string.IsNullOrEmpty(rubricScores[i]) && !string.IsNullOrEmpty(rubricScores[i + 1]))
                {
                    TeacherResponse rubricData = new TeacherResponse { ForeignId = rubricScores[i], PointsAssigned = double.Parse(rubricScores[i + 1]), TeacherResponseType = TeacherResponseType.RubricRow };
                    rubricScoreList.Add(rubricData);
                }
            }
            return rubricScoreList;
        }
        #endregion 
    }
}
