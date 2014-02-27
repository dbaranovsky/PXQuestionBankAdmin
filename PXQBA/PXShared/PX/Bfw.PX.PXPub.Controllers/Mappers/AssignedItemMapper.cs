using System;
using Bfw.PX.PXPub.Models;
using AssignedItem = Bfw.PX.PXPub.Models.AssignedItem;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class AssignedItemMapper
    {
        /// <summary>
        /// Maps a ContentItem business object to an Assignment model.
        /// </summary>
        /// <param name="biz">Assignment business object.</param>
        /// <returns>
        /// Assignment model.
        /// </returns>
        public static AssignedItem ToAssignedItem(this Biz.DataContracts.ContentItem biz)
        {
            var model = new AssignedItem();

            if (null != biz)
            {
                model.Id = biz.Id;
                model.Title = biz.Title;
                if (biz.AssignmentSettings != null)
                {
                    model.StartDate = biz.AssignmentSettings.StartDate;
                    model.DueDate = biz.AssignmentSettings.DueDate;
                    model.RubricPath = biz.AssignmentSettings.Rubric;
                    model.Score = new Score
                    {
                        Possible = biz.AssignmentSettings.Points
                    };
                    model.CompletionTrigger = (CompletionTrigger)biz.AssignmentSettings.CompletionTrigger;
                    model.TimeToComplete = biz.AssignmentSettings.TimeToComplete;
                    model.PassingScore = biz.AssignmentSettings.PassingScore;
                    model.Category = biz.AssignmentSettings.Category;
                    model.IsGradeable = biz.AssignmentSettings.IsGradeable;
                    model.MaxPoints = biz.AssignmentSettings.Points;
                    model.IsAllowLateSubmission = biz.AssignmentSettings.AllowLateSubmission;
                    model.IsHighlightLateSubmission = biz.AssignmentSettings.IsHighlightLateSubmission;
                    model.IsAllowLateGracePeriod = biz.AssignmentSettings.IsAllowLateGracePeriod;
                    model.IsAllowExtraCredit = biz.AssignmentSettings.IsAllowExtraCredit;
                    model.LateGraceDuration = biz.AssignmentSettings.LateGraceDuration;
                    model.LateGraceDurationType = biz.AssignmentSettings.LateGraceDurationType;
                    model.IsSendReminder = biz.AssignmentSettings.IsSendReminder;
                    model.SubmissionGradeAction = (Models.SubmissionGradeAction)biz.AssignmentSettings.SubmissionGradeAction;

                    if (model.IsSendReminder && biz.AssignmentSettings.ReminderEmail != null)
                    {
                        model.ReminderEmail.AssignmentId = biz.Id;
                        model.ReminderEmail.DaysBefore = biz.AssignmentSettings.ReminderEmail.DaysBefore;
                        model.ReminderEmail.DurationType = biz.AssignmentSettings.ReminderEmail.DurationType;
                        model.ReminderEmail.Subject = biz.AssignmentSettings.ReminderEmail.Subject;
                        model.ReminderEmail.Body = biz.AssignmentSettings.ReminderEmail.Body;
                        model.ReminderEmail.AssignmentDate = biz.AssignmentSettings.DueDate;
                        model.GradeRule = (Models.GradeRule)biz.AssignmentSettings.GradeRule;
                        model.IsMarkAsCompleteChecked = biz.AssignmentSettings.IsMarkAsCompleteChecked;
                    }
                }
                
                model.Instructions = biz.Description;
                model.SubType = biz.Subtype;
                model.Type = biz.Type;
                
                model.SourceType = biz.ModelType();
                var includeGbbScoreTrigger = biz.Properties.ContainsKey("bfw_IncludeGbbScoreTrigger")
                                                ? biz.Properties["bfw_IncludeGbbScoreTrigger"].Value
                                                : 0;

                model.IncludeGbbScoreTrigger = Convert.ToInt32(includeGbbScoreTrigger);

                var syllabusFilter = biz.Properties.ContainsKey("bfw_syllabusfilter") ? biz.Properties["bfw_syllabusfilter"].Value : "";
                model.SyllabusFilter = (syllabusFilter == null) ? "" : syllabusFilter.ToString();
                model.Sequence = biz.Sequence;
                model.IsImportant = biz.IsImportant;
                model.DueDateDisplay = "";
                model.Sco = biz.Sco;
                model.SortIndex = biz.SortIndex;
                model.CustomFields = biz.CustomFields;

                PopulateGradeSettings(model, biz.ItemDataXml);
            }

            return model;
        }

        /// <summary>
        /// Takes the xml data from the content item and populates the availble settings for completion trigger and grade action.
        /// The XML in Dlap looks like this:
        /// <AssignmentSettings>
	    ///     <AvailableCompletionTriggers>0|1|2</AvailableCompletionTriggers>
        ///     <DefaultCompletionTrigger>0</DefaultCompletionTrigger>
	    ///     <AvailableSubmissionGradeActions>0|1|2</AvailableSubmissionGradeActions>
	    ///     <DefaultSubmissionGradeAction>1</DefaultSubmissionGradeAction>
        /// </AssignmentSettings>
        /// </summary>
        /// <param name="model"></param>
        /// <param name="itemDataXml"></param>
        public static void PopulateGradeSettings(this AssignedItem model, XElement itemDataXml)
        {
                model.AvailableCompletionTriggers = new List<CompletionTrigger>();
                model.AvailableGradeActions = new List<SubmissionGradeAction>();
                if (itemDataXml != null && itemDataXml.Element("AssignmentSettings") != null)
                {   
                    var settings = itemDataXml.Element("AssignmentSettings");
                    if (settings.Element("AvailableCompletionTriggers") != null)
                    {
                        if (settings.Element("AvailableCompletionTriggers") != null)
                        {
                            string[] available = settings.Element("AvailableCompletionTriggers").Value.Trim().Split('|');
                            foreach (string value in available)
                            {
                                switch (value.Trim())
                                {
                                    case "0":
                                        model.AvailableCompletionTriggers.Add(CompletionTrigger.Minutes);
                                        break;
                                    case "1":
                                        model.AvailableCompletionTriggers.Add(CompletionTrigger.Submission);
                                        break;
                                    case "2":
                                        model.AvailableCompletionTriggers.Add(CompletionTrigger.PassingScore);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }

                    if (settings.Element("AvailableSubmissionGradeActions") != null)
                    {
                        if (settings.Element("AvailableSubmissionGradeActions") != null)
                        {
                            string[] available = settings.Element("AvailableSubmissionGradeActions").Value.Trim().Split('|');
                            foreach (string value in available)
                            {
                                switch (value.Trim())
                                {
                                    case "0":
                                        model.AvailableGradeActions.Add(SubmissionGradeAction.Default);
                                        break;
                                    case "1":
                                        model.AvailableGradeActions.Add(SubmissionGradeAction.Manual);
                                        break;
                                    case "2":
                                        model.AvailableGradeActions.Add(SubmissionGradeAction.Full_Credit);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }

                    if (settings.Element("DefaultCompletionTrigger") != null)
                    {
                        model.DefaultCompletionCriterion = !string.IsNullOrEmpty(settings.Element("DefaultCompletionTrigger").Value) ? (CompletionTrigger)Enum.Parse(typeof(CompletionTrigger), settings.Element("DefaultCompletionTrigger").Value.Trim()) : 0;
                    }

                    if (settings.Element("DefaultSubmissionGradeAction") != null)
                    {
                        model.DefaultGradeAction = !string.IsNullOrEmpty(settings.Element("DefaultSubmissionGradeAction").Value) ? (SubmissionGradeAction)Enum.Parse(typeof(SubmissionGradeAction), settings.Element("DefaultSubmissionGradeAction").Value.Trim()) : 0;
                    }
                }
        }

        /// <summary>
        /// Convert to an assigned item.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Biz.DataContracts.AssignedItem ToAssignedItem(this AssignedItem model)
        {
            var biz = new Biz.DataContracts.AssignedItem();

            if (null != model)
            {
                biz.Id = model.Id;
                biz.Title = model.Title;
                biz.StartDate = model.StartDate;
                biz.DueDate = model.DueDate;
                biz.MaxPoints = model.Score.Possible;
                biz.SyllabusFilter = model.SyllabusFilter;
                biz.CompletionTrigger = (BizDC.CompletionTrigger)model.CompletionTrigger;
                biz.TimeToComplete = model.TimeToComplete;
                biz.PassingScore = model.PassingScore;
                biz.Category = model.Category;
                biz.Sequence = model.Sequence;
                biz.RubricPath = model.RubricPath;
                biz.IsImportant = model.IsImportant;
                biz.IsGradeable = model.IsGradeable;
                biz.IsMarkAsCompleteChecked = model.IsMarkAsCompleteChecked;
                biz.IncludeGbbScoreTrigger = model.IncludeGbbScoreTrigger;
                biz.GradeRule = (BizDC.GradeRule)model.GradeRule;
                biz.IsAllowLateSubmission = model.IsAllowLateSubmission;
                biz.IsHighlightLateSubmission = model.IsHighlightLateSubmission;
                biz.IsAllowLateGracePeriod = model.IsAllowLateGracePeriod;
                biz.IsAllowExtraCredit = model.IsAllowExtraCredit;
                biz.LateGraceDuration = model.LateGraceDuration;
                biz.LateGraceDurationType = model.LateGraceDurationType;
                biz.IsSendReminder = model.IsSendReminder;
                biz.Instructions = model.Instructions;
                biz.CustomFields = model.CustomFields;


                biz.SubmissionGradeAction = (BizDC.SubmissionGradeAction)model.SubmissionGradeAction;

                if (model.IsSendReminder)
                {
                    biz.ReminderEmail = new BizDC.ReminderEmail
                    {
                        AssignmentId = model.Id,
                        DaysBefore = model.ReminderEmail.DaysBefore,
                        DurationType = model.ReminderEmail.DurationType,
                        Subject = model.ReminderEmail.Subject,
                        Body = model.ReminderEmail.Body,
                        AssignmentDate = model.ReminderEmail.AssignmentDate
                    };
                }
            }

            return biz;
        }

        /// <summary>
        /// Maps an Assignment business object to an Assignment model, and uses enrollment information to populate the
        /// Assignment's score.
        /// </summary>
        /// <param name="biz">AssignedItem business object.</param>
        /// <param name="enrollment">The enrollment to which the AssignedItem belongs.</param>
        /// <returns>
        /// Assignment model.
        /// </returns>
        public static AssignedItem ToAssignmentWithScores(this BizDC.AssignedItem biz, BizDC.Enrollment enrollment)
        {
            var model = new AssignedItem();

            if (null != biz)
            {
                model.DueDate = biz.DueDate.HasValue ? biz.DueDate.Value : DateTime.MinValue;
                model.Id = biz.Id;
                model.Title = biz.Title;
                foreach (BizDC.Grade grade in enrollment.ItemGrades)
                {
                    if (grade.GradedItem.Id == model.Id)
                    {
                        Score s = grade.ToScore();

                        if (s.Date.Year > DateTime.MinValue.Year)
                        {
                            model.Score = s;
                        }
                        break;
                    }
                }
            }

            return model;
        }

        /// <summary>
        /// Convert to a BizDC.ContentItem from an Assignned Item.
        /// </summary>
        /// <param name="assignItem">The assign item.</param>
        /// <param name="courseId">The course id.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static BizDC.ContentItem ToContentItem(this AssignedItem assignItem, string courseId, BizSC.IContentActions content)
        {
            return assignItem.ToAssignedItem().ToContentItem(courseId, content);
        }

        /// <summary>
        /// Convert to a BizDC.ContentItem from an Assigned Item.
        /// </summary>
        /// <param name="assignItem">The assign item.</param>
        /// <param name="courseId">The course id.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static BizDC.ContentItem ToContentItem(this BizDC.AssignedItem assignItem, string courseId, BizSC.IContentActions content)
        {
            var item = content.GetContent(courseId, assignItem.Id);

            item.CustomFields = assignItem.CustomFields;
            item.AssignmentSettings.IsAssignable = true;
            item.AssignmentSettings.Category = assignItem.Category;
            item.AssignmentSettings.CompletionTrigger = assignItem.CompletionTrigger;
            item.AssignmentSettings.TimeToComplete = assignItem.TimeToComplete;
            item.AssignmentSettings.PassingScore = assignItem.PassingScore;
            item.AssignmentSettings.Points = assignItem.MaxPoints.HasValue ? assignItem.MaxPoints.Value : 0;
            item.AssignmentSettings.Rubric = assignItem.RubricPath;
            item.AssignmentSettings.IsGradeable = assignItem.IsGradeable;
            item.AssignmentSettings.IsMarkAsCompleteChecked = assignItem.IsMarkAsCompleteChecked;
            item.AssignmentSettings.IncludeGbbScoreTrigger = assignItem.IncludeGbbScoreTrigger;
            item.Description = assignItem.Instructions;

            if (item.AssignmentSettings != null)
            {
                item.AssignmentSettings.GradeRule = assignItem.GradeRule;
                item.AssignmentSettings.SubmissionGradeAction = assignItem.SubmissionGradeAction;
            }

            if (item.AssessmentSettings != null)
            {
                item.AssessmentSettings.GradeRule = assignItem.GradeRule;
                item.AssessmentSettings.SubmissionGradeAction = assignItem.SubmissionGradeAction;
            }

            item.AssignmentSettings.AllowLateSubmission = assignItem.IsAllowLateSubmission;
            item.AssignmentSettings.IsHighlightLateSubmission = assignItem.IsHighlightLateSubmission;
            item.AssignmentSettings.IsAllowLateGracePeriod = assignItem.IsAllowLateGracePeriod;
            item.AssignmentSettings.LateGraceDuration = assignItem.LateGraceDuration;
            item.AssignmentSettings.LateGraceDurationType = assignItem.LateGraceDurationType;
            item.AssignmentSettings.IsSendReminder = assignItem.IsSendReminder;
            item.AssignmentSettings.ReminderEmail = assignItem.ReminderEmail;
            item.AssignmentSettings.IsAllowExtraCredit = assignItem.IsAllowExtraCredit;

            item.IsImportant = assignItem.IsImportant;
            item.Properties["bfw_syllabusfilter"] = new BizDC.PropertyValue { Type = BizDC.PropertyType.String, Value = assignItem.SyllabusFilter };

            item.Properties["bfw_IncludeGbbScoreTrigger"] = new BizDC.PropertyValue
                                                                {
                                                                    Type = BizDC.PropertyType.Integer,
                                                                    Value = assignItem.IncludeGbbScoreTrigger
                                                                };

            item.Properties["bfw_startdate"] = new BizDC.PropertyValue
                                                {
                                                    Type = BizDC.PropertyType.DateTime,
                                                    Value = assignItem.StartDate
                                                };

            item.AssignmentSettings.DueDate = assignItem.DueDate.HasValue ? assignItem.DueDate.Value : DateTime.MinValue;
            item.ParentId = "PX_MANIFEST";
            return item;
        }
    }
}
