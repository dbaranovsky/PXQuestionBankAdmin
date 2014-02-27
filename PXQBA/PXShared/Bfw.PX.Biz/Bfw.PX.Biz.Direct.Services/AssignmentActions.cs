using System;
using System.Collections.Generic;
using System.Linq;

using Bfw.Common.Collections;


using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.Common.Database;
using Microsoft.Practices.ServiceLocation;

namespace Bfw.PX.Biz.Direct.Services
{

    /// <summary>
	/// Provides an implementation of the IAssignmentActions.
	/// </summary>
	public class AssignmentActions : IAssignmentActions
	{
        private IDatabaseManager dbManager;

		#region Properties

		/// <summary>
		/// The IBusinessContext implementation to use.
		/// </summary>
		protected IBusinessContext Context { get; set; }

		/// <summary>
		/// The IContentActions implementation to use.
		/// </summary>
		protected IContentActions ContentActions { get; set; }

        protected IDatabaseManager DbManager
        {
            get 
            { 
                if (dbManager == null)
                    dbManager = ServiceLocator.Current.GetInstance<IDatabaseManager>();
                return dbManager;
            }
        }

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="AssignmentActions"/> class.
		/// </summary>
		/// <param name="context">The business context.</param>
		/// <param name="contentActions">The content actions.</param>
		public AssignmentActions(IBusinessContext context, IContentActions contentActions)
		{
			Context = context;
			ContentActions = contentActions;
		}

		#endregion

		#region IAssignmentActions Members

		/// <summary>
		/// Marks an item as assigned and sets all appropriate assignment settings
		/// such as due date and max points.
		/// </summary>
		/// <param name="assignItem">The assign item.</param>
		[Obsolete("Depends on an instance of IContentActions.", false)]
		public void Assign(AssignedItem assignItem)
		{
			using (Context.Tracer.StartTrace(String.Format("AssignmentActions.Assign(assignItem.Id={0})", assignItem.Id)))
			{
                var entityId = assignItem.GroupId.IsNullOrEmpty() ? Context.EntityId : assignItem.GroupId;

                var item = ContentActions.GetContent(entityId, assignItem.Id);

                item.CustomFields = assignItem.CustomFields;
                item.AssignmentSettings.IsAssignable = assignItem.IsGradeable;
				item.AssignmentSettings.IsMarkAsCompleteChecked = assignItem.IsMarkAsCompleteChecked;
				item.AssignmentSettings.AllowLateSubmission = assignItem.IsAllowLateSubmission;
			    item.AssignmentSettings.IsAllowLateGracePeriod = assignItem.IsAllowLateGracePeriod;
                item.AssignmentSettings.IsAllowExtraCredit = assignItem.IsAllowExtraCredit;
                item.AssignmentSettings.LateGraceDuration = assignItem.LateGraceDuration;
                item.AssignmentSettings.LateGraceDurationType = assignItem.LateGraceDurationType;

                item.Properties["bfw_assignment_instructions"] = new PropertyValue()
                {
                    Type = PropertyType.String,
                    Value = assignItem.Instructions
                };

                item.Properties["bfw_allow_extracredit"] = new PropertyValue()
                {
                    Type = PropertyType.Boolean,
                    Value = assignItem.IsAllowExtraCredit
                };

			    if (assignItem.IsAllowExtraCredit)
			    {
			        item.GradeFlags |= GradeFlags.ExtraCredit;
			    }
			    else
			    {
                    item.GradeFlags &= ~GradeFlags.ExtraCredit;
                }

				if (assignItem.IsAllowLateSubmission)
				{
					item.Properties["bfw_highlightlatesubmission"] = new PropertyValue()
					{
						Type = PropertyType.Boolean,
						Value = assignItem.IsHighlightLateSubmission
					};
					item.Properties["bfw_allowlatesubmissiongrace"] = new PropertyValue()
					{
						Type = PropertyType.Boolean,
						Value = assignItem.IsAllowLateGracePeriod
					};
					if (assignItem.IsAllowLateGracePeriod)
					{
						item.Properties["bfw_latesubmissiongraceduration"] = new PropertyValue()
						{
							Type = PropertyType.Integer,
							Value = assignItem.LateGraceDuration
						};
						item.Properties["bfw_latesubmissiongracedurationtype"] = new PropertyValue()
						{
							Type = PropertyType.String,
							Value = assignItem.LateGraceDurationType
						};
					}
					else
					{
						item.Properties.Remove("bfw_latesubmissiongraceduration");
						item.Properties.Remove("bfw_latesubmissiongracedurationtype");
					}
				}
                else
                {
                    item.Properties["bfw_allowlatesubmissiongrace"] = new PropertyValue()
                    {
                        Type = PropertyType.Boolean,
                        Value = false
                    };
                }

				item.Properties["bfw_SendReminder"] = new PropertyValue()
				{
					Type = PropertyType.Boolean,
					Value = assignItem.IsSendReminder
				};

				item.AssignmentSettings.IsSendReminder = assignItem.IsSendReminder;
				item.AssignmentSettings.Category = assignItem.Category;
				item.AssignmentSettings.CompletionTrigger = assignItem.CompletionTrigger;

                if (assignItem.CompletionTrigger == DataContracts.CompletionTrigger.Minutes)
                {
                    item.AssignmentSettings.TimeToComplete = assignItem.TimeToComplete;
                }

                if (assignItem.CompletionTrigger == DataContracts.CompletionTrigger.PassingScore)
                {
                    item.AssignmentSettings.PassingScore = assignItem.PassingScore;
                }

				item.AssignmentSettings.Points = assignItem.MaxPoints.HasValue ? assignItem.MaxPoints.Value : 0;
				item.AssignmentSettings.Rubric = assignItem.RubricPath;
				item.AssignmentSettings.GradeRule = assignItem.GradeRule;

                if (item.AssignmentSettings != null)
                {
                    item.AssignmentSettings.SubmissionGradeAction= assignItem.SubmissionGradeAction;
                }
                
                if (item.AssessmentSettings != null)
                {
                    item.AssessmentSettings.SubmissionGradeAction= assignItem.SubmissionGradeAction;
                }

				item.Properties["bfw_IncludeGbbScoreTrigger"] = new PropertyValue() { Type = Bfw.PX.Biz.DataContracts.PropertyType.Integer, Value = assignItem.IncludeGbbScoreTrigger };
                
                // If IncludeGbbScoreTrigger is zero then an unscored assignment should show as 0%. This setting is defined in the GradeFlags enum as ZeroUnscored
                //item.GradeFlags = (assignItem.IncludeGbbScoreTrigger == 0) ? Biz.DataContracts.GradeFlags.ZeroUnscored : Biz.DataContracts.GradeFlags.None;

				item.Properties["bfw_syllabusfilter"] = new PropertyValue() { Type = Bfw.PX.Biz.DataContracts.PropertyType.String, Value = assignItem.SyllabusFilter };
				item.Sequence = assignItem.Sequence;
				item.Properties["bfw_startdate"] = new PropertyValue()
				{
					Type = PropertyType.DateTime,
					Value = assignItem.StartDate
				};
				if (item.Type == "Resource")
				{
					item.Type = "Assignment";
				}
				item.AssignmentSettings.DueDate = assignItem.DueDate.HasValue ? assignItem.DueDate.Value : DateTime.MinValue;
				item.AssignmentSettings.meta_bfw_Assigned = true;
				if ((assignItem.StartDate.HasValue && assignItem.StartDate.Value.Year > 1900) || assignItem.DueDate.HasValue && assignItem.DueDate.Value.Year > 1900)
				{
					item.ParentId = "PX_MANIFEST";
				}

                ContentActions.StoreContent(item, assignItem.GroupId, false);

				if (assignItem.IsSendReminder)
				{
					AddReminder(assignItem.ReminderEmail);
				}
				else
				{
					CancelReminder(item.Id);
				}
			}
		}

		/// <summary>
		/// Makes an item not gradable and also sets all of its assignment
		/// properties back to their defaults.
		/// </summary>
		/// <param name="assignedItem"></param>
		[Obsolete("Depends on an instance of IContentActions.", false)]
		public void Unassign(ContentItem assignedItem)
		{
			using (Context.Tracer.StartTrace(String.Format("AssignmentActions.Unassign(assignedtem.Id={0})", assignedItem.Id)))
			{
				SetUnAssignProperty(assignedItem);
				ContentActions.StoreContent(assignedItem);

				CancelReminder(assignedItem.Id);
                Context.CacheProvider.InvalidateLaunchPadData(Context.CourseId);
			}
		}

		/// <summary>
		/// Adds a new assignment reminder 
		/// </summary>
		/// <param name="email"></param>
		private void AddReminder(ReminderEmail email)
		{
            using (Context.Tracer.StartTrace(String.Format("AssignmentActions.AddReminder(email={0}) - db (AddEmailTracking)", email)))
            {
                DbManager.ConfigureConnection("PXData");

                try
                {
                    //var message = string.Format("<email><subject>{0}</subject><body>{1}</body></email>", email.Subject, email.Body);
                    DateTime sendOn = this.CalculateSendOnDate(email);
                    
                    var templateId = System.Configuration.ConfigurationManager.AppSettings["AssignmentReminderEmailTemplate"];
                    var EventTypeId = System.Configuration.ConfigurationManager.AppSettings["AssignmentReminderEmailEventType"];

                    DbManager.StartSession();
                    DbManager.ExecuteNonQuery("AddEmailTracking @0, @1, @2, @3, @4, @5, @6, @7, @8, @9", Context.CourseId,
                        Context.ProductCourseId, Context.CurrentUser.Id, sendOn, "add", EventTypeId, email.AssignmentId,
                        email.Subject, email.Body, templateId);
                }
                finally
                {
                    DbManager.EndSession();
                }
            }
		}

        /// <summary>
        /// This method is used to get the assignment send on date for an assignment based on duration type
        /// </summary>
        /// <param name="email">Reminder Email model data with assignment date and days before. Here days before is actually the period and it can be min / hour / days / week.</param>
        /// <returns>Date time when the reminder service should be sent.</returns>
        public DateTime CalculateSendOnDate(ReminderEmail email)
        {
            DateTime sendOn;
            var durationType = email.DurationType;
            switch (durationType.ToLower())
            {
                case "day":
                case "days":
                    sendOn = email.AssignmentDate.AddDays(-email.DaysBefore);
                    break;
                case "week":
                case "weeks":
                    sendOn = email.AssignmentDate.AddDays(-7 * email.DaysBefore);
                    break;
                case "minute":
                case "minutes":
                    sendOn = email.AssignmentDate.AddMinutes(-email.DaysBefore);
                    break;
                case "hour":
                case "hours":
                    sendOn = email.AssignmentDate.AddHours(-email.DaysBefore);
                    break;
                default:
                    sendOn = email.AssignmentDate;
                    break;
            }
            return sendOn;
        }

        /// <summary>
		/// Cancels the assignment reminder based on item id
		/// </summary>
		/// <param name="itemId"></param>
		private void CancelReminder(string itemId)
		{
            using (Context.Tracer.StartTrace(String.Format("AssignmentActions.CancelReminder(itemId={0}) - db (CancelEmailTracking)", itemId)))
            {
                DbManager.ConfigureConnection("PXData");

                try
                {
                    DbManager.StartSession();
                    DbManager.ExecuteNonQuery("CancelEmailTracking @0, @1, @2 ", Context.CourseId, 1, itemId);
                }
                finally
                {
                    DbManager.EndSession();
                }
            }
		}

		/// <summary>
		/// Marks or unmarks an assignment as important
		/// </summary>
		/// <param name="assignmentIDs"></param>
		/// <param name="isImportant"></param>
		public void AssignmentImportant(List<string> assignmentIDs, bool isImportant)
		{
			if (assignmentIDs.IsNullOrEmpty())
			{
				return;
			}

			using (Context.Tracer.StartTrace(String.Format("AssignmentActions.AssignmentImportant(assignmentIDs={0},isImportant={1})",assignmentIDs.ToString(),isImportant.ToString())))
			{
				foreach (var assignmentID in assignmentIDs)
				{
					var item = ContentActions.GetContent(Context.EntityId, assignmentID);
					var category = System.Configuration.ConfigurationManager.AppSettings["IsImportant"];
					if (isImportant)
					{
						var importantCategory = new TocCategory() { Id = category, Text = category, ItemParentId = "", Sequence = "" };
						item.Categories.Add(importantCategory);
					}
					else
					{
						var importantCategory = item.Categories.FirstOrDefault(c => c.Id == category);
						item.Categories.Remove(importantCategory);
					}

					ContentActions.StoreContent(item);
				}
			}
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Makes an item not gradable and also sets all of its assignment
		/// properties back to their defaults.
		/// </summary>
		/// <param name="assignedItem">The assigned item.</param>
		private void SetUnAssignProperty(ContentItem assignedItem)
		{
			assignedItem.AssignmentSettings.IsAssignable = false;
			assignedItem.AssignmentSettings.meta_bfw_Assigned = false;
			assignedItem.Properties["bfw_startdate"] = new PropertyValue()
			{
				Type = PropertyType.DateTime,
				Value = DateTime.MinValue
			};
			assignedItem.AssignmentSettings.DueDate = DateTime.MinValue;
			assignedItem.AssignmentSettings.Points = 0;

			assignedItem.AssignmentSettings.AllowLateSubmission = false;
			assignedItem.Properties.Remove("bfw_SendReminder");
			assignedItem.Properties.Remove("bfw_highlightlatesubmission");
			assignedItem.Properties.Remove("bfw_allowlatesubmissiongrace");
			assignedItem.Properties.Remove("bfw_latesubmissiongraceduration");
			assignedItem.Properties.Remove("bfw_latesubmissiongracedurationtype");
            try
            {
                assignedItem.Properties.Remove("bfw_assignment_instructions");
            }
            catch { }

		}

		#endregion
	}
}
