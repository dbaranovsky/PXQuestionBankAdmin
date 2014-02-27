using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

using Bdc = Bfw.PX.Biz.DataContracts;
using Adc = Bfw.Agilix.DataContracts;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.Commands;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.Common.Logging;
using Bfw.Agilix.DataContracts;
using AgxDC = Bfw.Agilix.DataContracts;

namespace Bfw.PX.Biz.Direct.Services
{
    /// <summary>
    /// Provides functionality to perform GradeBook actions.
    /// </summary>
    public class PxGradeBookActions : IPxGradeBookActions
    {
        #region Properties

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        protected IBusinessContext Context { get; set; }

        /// <summary>
        /// Gets or sets the enrollment actions.
        /// </summary>
        protected IEnrollmentActions EnrollmentActions { get; set; }

        /// <summary>
        /// Gets or sets the content actions.
        /// </summary>
        protected IContentActions ContentActions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IUserActions UserActions { get; set; }

        /// <summary>
        /// Gets or sets the session manager.
        /// </summary>
        /// <value>
        /// The session manager.
        /// </value>
        protected ISessionManager SessionManager { get; set; }

        /// <summary>
        /// Grade Actions
        /// </summary>
        protected IGradeActions GradeActions { get; set; }

        private List<Item> UnAssignedItem;

        #endregion

        #region Constructors

        /// <summary>
        /// instance constructor
        /// </summary>
        public PxGradeBookActions(IBusinessContext context, IEnrollmentActions enrollmentActions, IUserActions userActions, ISessionManager sessionManager,
                                IContentActions contentActions)
        {
            Context = context;
            EnrollmentActions = enrollmentActions;
            UserActions = userActions;
            SessionManager = sessionManager;
            ContentActions = contentActions;
            UnAssignedItem = new List<Item>();
        }

        #endregion

        #region Command Methods
        private Batch GetAssignmentItemsCommand(IEnumerable<Bdc.ContentItem> folders)
        {
            var cmdBatch = new Batch();

            //Perpare a batch command to retrieve all the assignment items
            foreach (var folder in folders)
            {
                var cmd = new GetItems()
                {
                    SearchParameters = new ItemSearch()
                    {
                        EntityId = Context.EntityId,
                        Query = string.Format(@"/meta-xbook-assignment-id = '{0}'", folder.Id)
                    }
                };

                cmdBatch.Add(folder.Id, cmd);
            }

            SessionManager.CurrentSession.ExecuteAsAdmin(cmdBatch);

            return cmdBatch;
        }
        /// <summary>
        /// Returns an executed GetGrades dlap command ready for parsing
        /// </summary>
        /// <param name="itemIds">List of items to query for </param>
        /// <param name="enrollmentId">Id of the entity which the items are associated</param>
        /// <returns>Already executed GetGrades dlap command ready for parsing</returns>
        private IEnumerable<Enrollment> GetGradesCommand(IEnumerable<String> itemIds = null, string enrollmentId = null)
        {
            if (itemIds.IsNullOrEmpty())
            {
                //Get the list of assignment folders in a course
                var assignmentFolders = ContentActions.GetAssignmentFolders();

                //Get the list of assigned items
                var assignedItems = GetAssignmentItems(assignmentFolders);

                itemIds = assignedItems.Map(ai => ai.Id);
            }

            var batch = new Batch();

            var cmd = new GetGrades
            {
                SearchParameters = new Adc.GradeSearch
                {
                    ItemIds = itemIds.ToList(),
                    EntityId = Context.EntityId,
                    EnrollmentId = enrollmentId
                }
            };

            batch.Add(cmd);

            SessionManager.CurrentSession.ExecuteAsAdmin(batch);

            return batch.Commands.Select(c => ((GetGrades)c).Enrollments).Aggregate((a, b) => a.Concat(b));
        }
        #endregion Command Methods

        #region Assignments

        /// <summary>
        /// Gets the list of assignments
        /// </summary>
        /// <returns></returns>
        private IList<Item> GetAssignmentItems(IEnumerable<Bdc.ContentItem> assignmentFolders)
        {
            var assignedtItems = new List<Item>();
            var cmdBatch = GetAssignmentItemsCommand(assignmentFolders);

            foreach (var assignmentFolder in assignmentFolders)
            {
                var returnCmd = cmdBatch.CommandAs<GetItems>(assignmentFolder.Id);

                if (!returnCmd.Items.IsNullOrEmpty())
                {
                    assignedtItems.AddRange(returnCmd.Items);
                }
            }

            return assignedtItems;
        }
        /// <summary>
        /// Gets a dictionary of folders with the items
        /// </summary>
        /// <returns>Dictionary[folder, list items under that folder]</returns>
        private IDictionary<Bdc.ContentItem, IList<Bdc.ContentItem>> GetAssignments(IEnumerable<Bdc.ContentItem> assignmentFolders)
        {
            var assignments = new Dictionary<Bdc.ContentItem, IList<Bdc.ContentItem>>();
            var cmdBatch = GetAssignmentItemsCommand(assignmentFolders);

            foreach (var assignmentFolder in assignmentFolders)
            {
                var returnCmd = cmdBatch.CommandAs<GetItems>(assignmentFolder.Id);

                if (!returnCmd.Items.IsNullOrEmpty())
                {
                    assignments.Add(assignmentFolder, returnCmd.Items.Map(item => item.ToContentItem()).ToList());
                }
            }
            return assignments;
        }

        /// <summary>
        /// Sets the grade items, since the items returned by GetGrades agilix method does not have all the properties
        /// </summary>
        /// <param name="?"></param>
        private IEnumerable<Bdc.Grade> SetGradeItems(IEnumerable<Bdc.Grade> grades, IEnumerable<Item> items)
        {
            foreach (var grade in grades)
            {
                var item = items.Where(i => i.Id == grade.ItemId).FirstOrDefault();
                grade.GradedItem = item != null ? item.ToContentItem() : null;
            }
            return grades;
        }

        #endregion Assignments

        #region IPxGradeBookActions Methods
        /// <summary>
        /// Gets the list of students for a gradebook
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Bdc.StudentProfile> GetStudentList()
        {
            //Get the list of students enrolled in a course
            var enrollments = EnrollmentActions.GetEntityEnrollments(Context.EntityId, Biz.DataContracts.UserType.Student);

            var studentProfiles = enrollments.Map(delegate(Bdc.Enrollment enrollment)
            {
                var studenProfile = new Bdc.StudentProfile()
                {
                    EnrollmentId = enrollment.Id,
                    UserId = enrollment.User.Id,
                    FirstName = enrollment.User.FirstName,
                    LastName = enrollment.User.LastName,
                    FormattedName = string.Format("{0}, {1}", enrollment.User.LastName, enrollment.User.FirstName),
                    Email = enrollment.User.Email
                };
                return studenProfile;
            });

            return studentProfiles;
        }

        /// <summary>
        /// Get Student Profile
        /// </summary>
        /// <param name="studentUserId"></param>
        /// <returns></returns>
        public Bdc.StudentProfile GetStudent(string studentUserId)
        {
            var enrollment = EnrollmentActions.GetEnrollment(studentUserId, Context.EntityId);
            var studenProfile = new Bdc.StudentProfile()
            {
                EnrollmentId = enrollment.Id,
                UserId = enrollment.User.Id,
                FirstName = enrollment.User.FirstName,
                LastName = enrollment.User.LastName,
                FormattedName = string.Format("{0}, {1}", enrollment.User.LastName, enrollment.User.FirstName),
                Email = enrollment.User.Email,
                LastLogin = enrollment.User.LastLogin
            };

            return studenProfile;
        }

        /// <summary>
        /// Returns the list of all grade book assignments in a course
        /// </summary>
        /// <returns>Dictionary[AssignmentFolder, List of Assignments] or if there are no assignment folders an empty dictionary</returns>
        public IDictionary<Bdc.ContentItem, IList<Bdc.ContentItem>> GetGradeBookAssignments()
        {
            IDictionary<Bdc.ContentItem, IList<Bdc.ContentItem>> retval = new Dictionary<Bdc.ContentItem, IList<Bdc.ContentItem>>();
            var assignmentFolders = ContentActions.GetAssignmentFolders();

            //Gets the dictionary containing the assignment folder and the corresponding assignment item list. Return an empty dictionary if no assignment 
            //folders exist.
            if (assignmentFolders.Count() > 0)
                retval = GetAssignments(assignmentFolders);

            return retval;
        }


        #region Grades
        /// <summary>
        /// Gets list of enrollments for all <paramref name="itemIds"/>
        /// </summary>
        /// <param name="itemIds">List of item id's to return enrollments for.  If null returns enrollments for all items
        /// associated with the active entity</param>
        /// <returns>List of enrollments associated with the id's of the items passed in</returns>
        public IEnumerable<Bdc.Enrollment> GetEnrollments(IEnumerable<String> itemIds = null)
        {
            var cmd = GetGradesCommand(itemIds);
            var gradeEnrollmentCollection = cmd.Map(e => e.ToEnrollment());

            return gradeEnrollmentCollection.ToList();
        }
        /// <summary>
        /// Get the Grades of a student given the student enrollment id
        /// </summary>
        /// <param name="enrollmentid"></param>
        /// <returns></returns>
        public IEnumerable<Bdc.Grade> GetGradesByEnrollment(string enrollmentid, bool assigned)
        {
            IEnumerable<Bdc.Grade> retval = new List<Bdc.Grade>();
            IEnumerable<Item> items = null;
            if (assigned)
            {
                var folders = ContentActions.GetAssignmentFolders();
                if (folders.Count() > 0)
                    items = GetAssignmentItems(folders);
            }
            else
            {
                items = GetUnAssignedGradableItems();
            }
            if (items != null && items.Count() > 0)
            {
                var cmd = GetGradesCommand(items.Map(ai => ai.Id), enrollmentid);
                var enrollment = cmd.Map(e => e.ToEnrollment()).FirstOrDefault();
                retval = SetGradeItems(enrollment.ItemGrades.ToList(), items);
            }
            return retval;
        }

        /// <summary>
        /// Gets a collection of grades aggregated by content item
        /// </summary>
        /// <param name="assigned">If true returns assigned items.  Else returns unassigned items</param>
        /// <returns>Collection of aggregated assigned or unassigned items</returns>
        public IEnumerable<DataContracts.Grade> ItemAggregatedGrades(bool assigned)
        {
            var retval = new List<DataContracts.Grade>();

            //Get the list of assignment folders in a course
            var items = assigned ? GetAssignmentItems(ContentActions.GetAssignmentFolders()) : GetUnAssignedGradableItems();
            var cmd = GetGradesCommand(items.Map(ai => ai.Id));

            if (cmd != null)
            {
                var grades =
                    cmd.SelectMany(e => e.ItemGrades)
                        .Where(g => assigned || (!assigned && g.SubmittedDate != DateTime.MinValue)).Map(g => g.ToGrade()).ToList();

                items = items.Join(grades, item => item.Id, grade => grade.ItemId, (item, grade) => item).Distinct().ToList();

                foreach (var item in items)
                {
                    var relatedGrades = (from grade in grades where grade.ItemId == item.Id select grade);
                    var aggregateGrade = new DataContracts.Grade()
                    {
                        ItemId = item.Id,
                        ItemName = item.Title,
                        GradedItem = item.ToContentItem()
                    };

                    //add only if item has grade assigned
                    if (relatedGrades.Count() > 0)
                    {
                        foreach (var rg in relatedGrades)
                        {
                            //Only contrubiute to score if you have been graded.
                            if (rg.ScoredDate.HasValue && rg.ScoredDate.Value.Year > DateTime.MinValue.Year)
                            {
                                aggregateGrade.Possible += rg == null ? 0 : rg.Possible;
                                aggregateGrade.Achieved += rg == null ? 0 : rg.Achieved;
                                aggregateGrade.RawPossible += rg == null ? 0 : rg.RawPossible;
                                aggregateGrade.RawAchieved += rg == null ? 0 : rg.RawAchieved;
                            }
                            aggregateGrade.Rule = rg.Rule;
                            if (rg.SubmittedDate > aggregateGrade.SubmittedDate || aggregateGrade.SubmittedDate == null)
                                aggregateGrade.SubmittedDate = rg.SubmittedDate;
                        }
                        retval.Add(aggregateGrade);
                    }
                }
            }

            return retval;
        }
        /// <summary>
        /// Gets collections of grades (either assigned or unassigned) for a <paramref name="enrollmentId"/>
        /// </summary>
        /// <param name="enrollmentId">EnrollmentId to return grades for</param>
        /// <param name="assigned">If true returns assigned items.  Else unassigned</param>
        /// <returns>Collection of grades for <paramref name="enrollmentId"/></returns>
        public IEnumerable<DataContracts.Grade> ItemGradeDetails(string enrollmentId, bool assigned)
        {
            var retval = new List<DataContracts.Grade>();

            //Get the list of unassigned or assigned items based on what is passed in
            var items = assigned ? GetAssignmentItems(ContentActions.GetAssignmentFolders()) : GetUnAssignedGradableItems();
            var cmd = GetGradesCommand(items.Map(ai => ai.Id), enrollmentId);

            if (cmd != null)
            {
                var grades = cmd.SelectMany(e => e.ItemGrades).Map(g => g.ToGrade()).ToList();
                //set the enrollment ids for the grades
                grades.ForEach(g => { g.EnrollmentId = enrollmentId; });

                //Only return grades that have been submitted for unassiged items
                foreach (var grade in grades.Where(g => assigned || (!assigned && g.SubmittedDate != DateTime.MinValue)))
                {
                    //fill up attempts only if single student drill down
                    if (grades.Count() > 0)
                    {
                        grade.Submissions = GetAttemptsByStudent(grade.ItemId, enrollmentId).OrderBy(sub => sub.AttemptNo);
                    }

                    retval.Add(grade);
                }
            }

            return SetGradeItems(retval, items);
        }
        /// <summary>
        /// Returns a collection of assigned or unassiced grades aggregated by student
        /// </summary>
        /// <param name="assigned">If true returns assigned grades, else unassigned</param>
        /// <returns>Collection of assigned or unassigned grades aggregated by student</returns>
        public IEnumerable<DataContracts.Grade> StudentAggregatedGrades(bool assigned)
        {
            var retval = new List<DataContracts.Grade>();
            var items = assigned ? GetAssignmentItems(ContentActions.GetAssignmentFolders()) : GetUnAssignedGradableItems();

            //Get reply for grades for items passed in
            var enrollments = GetGradesCommand(items.Select(i => i.Id));

            foreach (var enrollment in enrollments)
            {
                //Create aggregate grade object for each enrollment
                var aggregateGrade = new DataContracts.Grade()
                {
                    EnrollmentId = enrollment.Id,
                    EnrollmentName = enrollment.User.FirstName + " " + enrollment.User.LastName
                };
                var itemid = "";
                var grades = enrollment.ItemGrades.Where(g => assigned || (!assigned && g.SubmittedDate != DateTime.MinValue)).Map(grade => grade.ToGrade());
                foreach (var grade in grades)
                {
                    itemid = grade.ItemId;
                    aggregateGrade.Possible += grade == null ? 0 : grade.Possible;
                    aggregateGrade.Achieved += grade == null ? 0 : grade.Achieved;
                    aggregateGrade.RawPossible += grade == null ? 0 : grade.RawPossible;
                    aggregateGrade.RawAchieved += grade == null ? 0 : grade.RawAchieved;

                    //Return the latest submission for the Last Submission field in the aggregate grade.
                    if (grade.SubmittedDate > aggregateGrade.SubmittedDate || aggregateGrade.SubmittedDate == null)
                        aggregateGrade.SubmittedDate = grade.SubmittedDate;
                    aggregateGrade.Rule = grade.Rule;
                }

                if (grades != null && grades.Count() > 0)
                    aggregateGrade.Submissions = GetAttemptsByStudent(itemid, enrollment.Id);

                retval.Add(aggregateGrade);
            }

            return retval;
        }
        /// <summary>
        /// Returns collection of student grades associated with <paramref name="itemiD" />
        /// </summary>
        /// <param name="itemId">Id to retreive student grades for</param>
        /// <param name="assigned">If the item passed in is an assigned item</param>
        /// <returns>Collection of grade details per student for <paramref name="itemId"/></returns>
        public IEnumerable<DataContracts.Grade> StudentGradeDetails(string itemId, bool assigned)
        {
            var retval = new List<DataContracts.Grade>();

            //Get the list of unassigned or assigned items based on what is passed in
            var cmd = GetGradesCommand(new List<string>() { itemId });

            foreach (var enrollment in cmd.Map(e => e.ToEnrollment()))
            {
                //Only return grades that have been submitted for unassiged items
                foreach (var grade in enrollment.ItemGrades.Where(g => assigned || (!assigned && g.SubmittedDate != DateTime.MinValue)))
                {
                    grade.EnrollmentId = enrollment.Id;
                    grade.EnrollmentName = enrollment.User.FirstName + " " + enrollment.User.LastName;
                    grade.Submissions = GetAttemptsByStudent(itemId, enrollment.Id);
                    grade.GradedItem = ContentActions.GetContent(Context.EntityId, itemId);
                    retval.Add(grade);
                }
            }
            return retval;
        }
        #endregion Grades

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ItemId"></param>
        /// <param name="EnrollmentId"></param>
        /// <returns></returns>
        public IEnumerable<Bdc.SubmissionLog> GetAttemptsByStudent(string ItemId, string EnrollmentId)
        {
            var cmd2 = new GetSubmissions
            {
                SearchParameters = new Adc.SubmissionSearch
                {
                    ItemId = ItemId,
                    EnrollmentId = EnrollmentId
                }
            };

            SessionManager.CurrentSession.ExecuteAsAdmin(cmd2);

            var attempts = new List<Bdc.SubmissionLog>();
            if (cmd2.Submissions != null)
            {
                foreach (var item in cmd2.Submissions)
                {
                    var submission = new Bdc.SubmissionLog()
                    {
                        AttemptNo = item.Version,
                        SubmittedDate = item.SubmittedDate,
                        Grade = item.Grade != null ? (Math.Round(item.Grade.RawAchieved / (item.Grade.RawPossible == 0 ? 1 : item.Grade.RawPossible) * 100, 4)) : 0,
                        RawAchieved = item.Grade != null ? item.Grade.RawAchieved : 0,
                        RawPossible = item.Grade != null ? item.Grade.RawPossible : 0,
                        Achieved = item.Grade != null ? item.Grade.Achieved : 0,
                        Possible = item.Grade != null ? item.Grade.Possible : 0
                    };

                    attempts.Add(submission);
                }
            }

            return attempts;
        }
        #endregion IPxGradeBookActions Methods

        /// <summary>
        /// Gets the list of unassigned gradable items
        /// </summary>
        /// <returns></returns>
        protected IList<AgxDC.Item> GetUnAssignedGradableItems()
        {
            if (UnAssignedItem.IsNullOrEmpty())
            {
                var cmd = new GetItems()
                {
                    SearchParameters = new ItemSearch()
                    {
                        EntityId = Context.EntityId,
                        Query = @"/gradable = 'true' AND /parent = 'PX_MANIFEST' AND NOT /meta-xbook-assignment-id<>''"
                    }
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                UnAssignedItem = !cmd.Items.IsNullOrEmpty() ? cmd.Items : new List<AgxDC.Item>();
            }

            return UnAssignedItem;
        }

    }
}
