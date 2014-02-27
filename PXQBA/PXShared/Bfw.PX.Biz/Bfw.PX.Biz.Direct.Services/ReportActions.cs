// -----------------------------------------------------------------------
// <copyright file="RubricReport.cs" company="Macmillan">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Bfw.PX.Biz.Direct.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Bfw.Agilix.Commands;
    using Bfw.Agilix.Dlap.Session;
    using Bfw.Common.Collections;
    using Bfw.Common.Logging;
    using Bfw.PX.Biz.ServiceContracts;
    using Bfw.PX.Biz.Services.Mappers;
    using Adc = Bfw.Agilix.DataContracts;
    using Bdc = Bfw.PX.Biz.DataContracts;
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ReportActions : IReportActions
    {

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected IBusinessContext Context { get; set; }


        /// <summary>
        /// IEnrollmentActions implementation to use
        /// </summary>
        protected IEnrollmentActions EnrollmentActions { get; set; }

        /// <summary>
        /// Gets or sets the rubric actions.
        /// </summary>
        /// <value>
        /// The rubric actions.
        /// </value>
        protected IRubricActions RubricActions { get; set; }


        /// <summary>
        /// Gets or sets the dashboard actions.
        /// </summary>
        /// <value>
        /// The dashboard actions.
        /// </value>
        protected IDashboardActions DashboardActions { get; set; }

        /// <summary>
        /// Gets or sets the session manager.
        /// </summary>
        /// <value>
        /// The session manager.
        /// </value>
        protected ISessionManager SessionManager { get; set; }


        /// <summary>
        /// Gets or sets the content actions.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected IContentActions ContentActions { get; set; }

        /// <summary>
        /// Gets or sets the grade actions.
        /// </summary>
        /// <value>
        /// The grade actions.
        /// </value>
        protected IGradeActions GradeActions { get; set; }

        /// <summary>
        /// Gets or sets the course actions.
        /// </summary>
        /// <value>
        /// The course actions.
        /// </value>
        protected ICourseActions CourseActions { get; set; }

        /// <summary>
        /// Gets or sets the user actions.
        /// </summary>
        /// <value>
        /// The user actions.
        /// </value>
        protected IUserActions UserActions { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="ReportActions"/> class.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        /// <param name="enrollmentActions">The enrollment actions.</param>
        /// <param name="rubricActions">The rubric actions.</param>
        public ReportActions(IBusinessContext context,
            IEnrollmentActions enrollmentActions,
            IRubricActions rubricActions,
            IDashboardActions dashboardActions,
            ISessionManager sessionManager,
            IContentActions contentActions,
            IGradeActions gradeActions,            
            ICourseActions courseActions,
            IUserActions userActions)
        {
            Context = context;
            EnrollmentActions = enrollmentActions;
            RubricActions = rubricActions;
            DashboardActions = dashboardActions;
            SessionManager = sessionManager;
            ContentActions = contentActions;
            GradeActions = gradeActions;            
            CourseActions = courseActions;
            UserActions = userActions;
        }

        /// <summary>
        /// Gets the content of the rubric.
        /// </summary>
        /// <param name="alignedContents">The aligned contents.</param>
        /// <param name="rubricId">The rubric id.</param>
        /// <returns></returns>
        private Bdc.ContentItem GetRubricContent(string rubricId)
        {

            var content = ContentActions.GetContent(Context.Course.ProductCourseId, rubricId, true);
            if (content == null)
            {
                //this could be users rubric, try finding in current course.
                content = ContentActions.GetContent(Context.Course.Id, rubricId, true);
            }
            return content;
        }


        /// <summary>
        /// Gets all alligned contents for courses.
        /// </summary>
        /// <param name="dashboardCourses">The dashboard courses.</param>
        /// <param name="rubricId">The rubric id.</param>
        /// <returns></returns>
        private List<Bdc.ContentItem> GetAllAllignedContentsForCourses(IList<Bdc.Course> dashboardCourses, string rubricId)
        {
            var courseIds = (from c in dashboardCourses where !string.IsNullOrEmpty(c.Id) select c.Id).ToList();
            var alignedContent = RubricActions.ListAlignedContent(courseIds, new List<string> { rubricId }).ToList();
            return alignedContent;
        }

        /// <summary>
        /// Batches the calls to get rubric report.
        /// </summary>
        /// <param name="alignedContents">The aligned contents.</param>
        /// <returns></returns>
        private List<Bdc.RubricsReport> BatchCallsToGetRubricReport(List<Bdc.ContentItem> alignedContents)
        {
            var rubricReport = GetBatchResultForRubricReport(alignedContents);
            return rubricReport;
        }

        /// <summary>
        /// Updates the course and student information.
        /// </summary>
        /// <param name="dashboardCourses">The dashboard courses.</param>
        /// <param name="rubricReport">The rubric report.</param>
        private Bdc.RubricsReportContainer GenerateCompleteReport(IList<Bdc.Course> dashboardCourses, List<Bdc.RubricsReport> rubricReport, List<Bdc.ContentItem> alignedContents, string rubricId)
        {
            if (rubricReport.IsNullOrEmpty())
            {
                return new Bdc.RubricsReportContainer
                {
                    RubricsReport = new List<Bdc.RubricsReport>(),
                    RubricContentItem = null,
                    AlignedContentItems = null,
                    Enrollments = null
                };
            }
            var allCourseIds = rubricReport.Select(c => c.EntityId).ToList();
            //we have to get all the enrollment information too.
            var enrollments = EnrollmentActions.GetEnrollmentsBatch(allCourseIds, Bdc.UserType.Student);

            foreach (var report in rubricReport)
            {
                var entityId = report.EntityId;
                var courseName = (from c in dashboardCourses where c.Id == entityId select c.Title).FirstOrDefault();
                var instructorName = (from c in dashboardCourses where c.Id == entityId select c.InstructorName).FirstOrDefault();
                report.CourseName = courseName;
                report.InstructorName = instructorName;

                foreach (var student in report.StudentReports)
                {                    
                    var enrol = enrollments.Where(i => i.Id == student.EnrollmentId).FirstOrDefault();
                    if (enrol != null && enrol.Id == student.EnrollmentId)
                    {
                        var studentName = (from e in enrollments where e.Id == student.EnrollmentId select e.User.FormattedName).FirstOrDefault();
                        student.StudentName = studentName;
                    }
                    else
                    {
                        student.IsStudentDeleted = true;
                    }
                    
                }
                report.StudentReports.RemoveAll(i => i.IsStudentDeleted);
            }

            UpdateSubmissionsInformation(rubricReport, alignedContents);
            UpdateReviewedSubmissionsInfo(rubricReport, alignedContents);
            UpdateOwnerAndTemplateData(rubricReport);

            var rubricsReport = new Bdc.RubricsReportContainer
            {
                RubricsReport = rubricReport,
                AlignedContentItems = alignedContents,
                Enrollments = enrollments
            };

            return rubricsReport;
        }

        /// <summary>
        /// Updates the owner and template data.
        /// </summary>
        /// <param name="rubricReport">The rubric report.</param>
        private void UpdateOwnerAndTemplateData(List<Bdc.RubricsReport> rubricReport)
        {
            var list = new List<Bdc.Course>();
            var instructorList = new List<string>();
            if (!rubricReport.IsNullOrEmpty())
            {
                var distinctEntityIds = rubricReport.Select(i => i.EntityId).Distinct().ToList();
                distinctEntityIds.ForEach(c => list.Add(new Bdc.Course { Id = c }));                
                var theCourses = CourseActions.GetCourseListInformation(list);
                foreach (var c in theCourses)
                {
                    var current = rubricReport.SingleOrDefault(e => e.EntityId == c.Id);
                    if (current != null)
                    {
                        current.InstructorId = c.CourseOwner;
                        if (!instructorList.Contains(c.CourseOwner))
                        {
                            instructorList.Add(c.CourseOwner);
                        }
                        //current.InstructorName = GetInstructorName(c.CourseOwner);
                        current.TemplateId = c.CourseTemplate;
                    }
                }

                UpdateInstructorName(rubricReport, instructorList);
                //get template names
                list.Clear();

                var distinctTemplateIds = rubricReport.Select(i => i.TemplateId).Distinct().ToList();
                distinctTemplateIds.ForEach(c => list.Add(new Bdc.Course { Id = c }));
                
                var templates = CourseActions.GetCourseListInformation(list);
                foreach (var template in templates)
                {
                    var current = (from c in rubricReport where c.TemplateId == template.Id select c).FirstOrDefault();
                    if (current != null)
                    {
                        current.TemplateName = template.Title;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the instructor names.
        /// </summary>
        /// <param name="rubricReport">The rubric report.</param>
        /// <param name="instructorIds">The instructor ids.</param>
        private void UpdateInstructorName(List<Bdc.RubricsReport> rubricReport, List<string> instructorIds)
        {
            var userInfos = UserActions.ListUsers(instructorIds);
            if (userInfos.Count() > 0)
            {
                foreach (var report in rubricReport)
                {

                    var userInfo = (from c in userInfos where c.Id == report.InstructorId select c).FirstOrDefault();
                    if (userInfo != null && !string.IsNullOrEmpty(userInfo.FormattedName))
                    {
                        report.InstructorName = userInfo.FormattedName;
                    }
                }

            }
        }


        /// <summary>
        /// Updates the submissions information.
        /// </summary>
        /// <param name="rubricReport">The rubric report.</param>
        /// <param name="alignedContents">The aligned contents.</param>
        private void UpdateSubmissionsInformation(List<Bdc.RubricsReport> rubricReport, List<Bdc.ContentItem> alignedContents)
        {
            var submissions = new List<Bdc.Submission>();
            foreach (var report in rubricReport)
            {
                foreach (var item in report.StudentReports)
                {
                    foreach (var content in alignedContents)
                    {
                        submissions.Add(new Bdc.Submission
                        {
                            ItemId = content.Id,
                            EnrollmentId = item.EnrollmentId
                        });
                    }
                }
            }

            //Create batch call for all submissions.
            //var batchForSubmissions = CreateBatchCallForAllSubmissions(submissions);
            // var results = GetBatchReportsForAllSubmissions(batchForSubmissions);
            var submissionsResponse = GradeActions.GetStudentsSubmissionInfoWithoutTeacherResponse(submissions);

            foreach (var report in rubricReport)
            {
                foreach (var studentReport in report.StudentReports)
                {
                    var submissionsCount = (from c in submissionsResponse where c.EnrollmentId == studentReport.EnrollmentId select c).Count();
                    studentReport.AssignmentsCompleted = submissionsCount;
                }
            }

        }


        /// <summary>
        /// Updates the reviewed submissions info.
        /// </summary>
        /// <param name="rubricReport">The rubric report.</param>
        /// <param name="alignedContents">The aligned contents.</param>
        private void UpdateReviewedSubmissionsInfo(List<Bdc.RubricsReport> rubricReport, List<Bdc.ContentItem> alignedContents)
        {
            foreach (var report in rubricReport)
            {
                var entityId = report.EntityId;
                foreach (var studentReport in report.StudentReports)
                {
                    var allItemsCreatedByStudent = GetStudentItems(studentReport.EnrollmentId);
                    foreach (var studentItem in allItemsCreatedByStudent)
                    {
                        int totalReviewdSubmissions = (from c in alignedContents where c.Id == studentItem.ParentId select c).Count();
                        studentReport.ReviewedSubmissions += totalReviewdSubmissions;
                    }
                }
            }

        }

        /// <summary>
        /// Gets the student items.
        /// </summary>
        /// <param name="enrollmentId">The enrollment id.</param>
        /// <returns></returns>
        public IEnumerable<Bdc.ContentItem> GetStudentItems(string enrollmentId)
        {
            var studentResourceDoc = ContentActions.GetResource(enrollmentId, ContentActions.GetStudentResourceId(enrollmentId));
            var allItemsOfCurrentStudent = new List<Bdc.ContentItem>();

            if (studentResourceDoc != null)
            {
                var stream = studentResourceDoc.GetStream();
                XDocument doc = stream != null && stream.Length > 0
                                    ? XDocument.Load(stream)
                                    : ContentActions.GetEmptyStudentDoc();

                foreach (var itemElement in doc.Root.Elements("item"))
                {
                    var item = new Adc.Item();
                    item.ParseEntity(itemElement);
                    allItemsOfCurrentStudent.Add(item.ToContentItem());
                }
            }

            return allItemsOfCurrentStudent;
        }




        /// <summary>
        /// Creates the batch call for all submissions.
        /// </summary>
        /// <param name="submissions">The submissions.</param>
        /// <returns></returns>
        private Batch CreateBatchCallForAllSubmissions(List<Bdc.Submission> submissions)
        {
            var groupedSubmissions = submissions.GroupBy(s => s.EnrollmentId);

            var batch = new Batch();
            batch.RunAsync = true;
            foreach (var group in groupedSubmissions)
            {
                IList<Bdc.Submission> groupedSubmission = group.ToList();
                using (Context.Tracer.DoTrace("ReportActions.GetStudentSubmissionInfo(submissions)"))
                {
                    var agxSubmissions = !groupedSubmission.IsNullOrEmpty()
                                         ? groupedSubmission.Map(s => s.ToSubmission())
                                         : new List<Adc.Submission>();

                    //get the analysis report
                    batch.Add(new GetStudentSubmissionInfo()
                    {
                        Submissions = agxSubmissions.ToList()
                    });
                }
            }
            return batch;
        }


        private List<Bdc.ItemReport> GetBatchReportsForAllSubmissions(Batch batch)
        {
            SessionManager.CurrentSession.ExecuteAsAdmin(batch);
            var itemReports = new List<Bdc.ItemReport>();
            for (int index = 0; index < batch.Commands.Count(); index++)
            {
                if (!batch.CommandAs<GetStudentSubmissionInfo>(index).Submissions.IsNullOrEmpty())
                {
                    var cmd = batch.CommandAs<GetStudentSubmissionInfo>(index);
                    var pxSubmissions = !cmd.Submissions.IsNullOrEmpty()
                          ? cmd.Submissions.Where(t => t.Submitted).Map(s => s.ToSubmission()).ToList()
                          : new List<DataContracts.Submission>();

                    var enrollmentId = batch.CommandAs<GetStudentSubmissionInfo>(index).SearchParameters.EnrollmentId;

                }
            }
            return itemReports;
        }



        /// <summary>
        /// Generates the batch call.
        /// </summary>
        /// <param name="alignedContent">Content of the aligned.</param>
        /// <returns></returns>
        private Batch CreateBatchCallForRubricReport(List<Bdc.ContentItem> alignedContent)
        {
            var batch = new Batch();
            batch.RunAsync = true;
            foreach (var content in alignedContent)
            {
                using (Context.Tracer.DoTrace("ReportActions.GetRubricReport(entityId={0})", content.Id))
                {
                    //get the analysis report
                    batch.Add(new GetItemAnalysis()
                    {
                        SearchParameters = new Adc.ItemAnalysisSearch()
                        {
                            ItemId = content.Id,
                            EntityId = content.CourseId,
                            Verbose = true
                        }
                    });
                }
            }
            return batch;
        }


        /// <summary>
        /// Gets the batch result.
        /// </summary>
        /// <param name="batch">The batch.</param>
        /// <returns></returns>
        private List<Bdc.RubricsReport> GetBatchResultForRubricReport(List<Bdc.ContentItem> alignedContent)
        {
            List<Bdc.ItemReport> cachedResults = new List<Bdc.ItemReport>();
            List<Bdc.RubricsReport> courseSpecificReports = null;
            if (alignedContent != null && alignedContent.Count() > 0)
            {
                List<Bdc.ContentItem> reportsToFetchForContents = new List<Bdc.ContentItem>(); ;

                foreach (var content in alignedContent)
                {
                    //try loading from the cache first.
                    var qResult = Context.CacheProvider.FetchItemAnalysisReport(content.Id, content.CourseId);
                    if (qResult != null)
                    {
                        cachedResults.Add(qResult);
                    }
                    else
                    {
                        reportsToFetchForContents.Add(content);
                    }
                }

                if (reportsToFetchForContents.Count() > 0)
                {
                    var batch = new Batch();
                    batch.RunAsync = true;
                    foreach (var content in reportsToFetchForContents)
                    {
                        using (Context.Tracer.DoTrace("ReportActions.GetRubricReport(entityId={0})", content.Id))
                        {
                            //get the analysis report
                            batch.Add(new GetItemAnalysis()
                            {
                                SearchParameters = new Adc.ItemAnalysisSearch()
                                {
                                    ItemId = content.Id,
                                    EntityId = content.CourseId,
                                    Verbose = true
                                }
                            });
                        }
                    }


                    SessionManager.CurrentSession.ExecuteAsAdmin(batch);
                    var itemReports = new List<Bdc.ItemReport>();
                    for (int index = 0; index < batch.Commands.Count(); index++)
                    {
                        if (!batch.CommandAs<GetItemAnalysis>(index).ItemAnalysis.IsNullOrEmpty())
                        {
                            var entityId = batch.CommandAs<GetItemAnalysis>(index).SearchParameters.EntityId;
                            var itemId = batch.CommandAs<GetItemAnalysis>(index).SearchParameters.ItemId;

                            var rubricGradeAnalysis = batch.CommandAs<GetItemAnalysis>(index).ItemAnalysis;
                            var itemReport = new Bdc.ItemReport()
                            {
                                EntityId = entityId,
                                ItemId = itemId
                            };
                            if (!rubricGradeAnalysis.IsNullOrEmpty())
                            {
                                itemReport.RubricGrades = new List<Bdc.RubricGrade>();
                                foreach (var itemAnalysis in rubricGradeAnalysis)
                                {
                                    //the number of rubric details tell us the columns of the rubric and the scores.
                                    var rubricDetailResults = itemAnalysis.Grades;
                                    if (rubricDetailResults.Count > 0 && itemAnalysis.RubricRule != null)
                                    {
                                        var rubricGrades = new List<Bdc.RubricGrade>();
                                        var rubricRuleId = itemAnalysis.RubricRule.Id;
                                        foreach (var rubricColumnGrade in rubricDetailResults)
                                        {
                                            var grade = new Bdc.RubricGrade()
                                            {
                                                RubricRuleId = rubricRuleId,
                                                EnrollmentId = rubricColumnGrade.EnrollmentId,
                                                Achieved = rubricColumnGrade.Achieved,
                                                Possible = rubricColumnGrade.Possible
                                            };
                                            rubricGrades.Add(grade);
                                        }
                                        itemReport.RubricGrades.AddRange(rubricGrades);
                                    }
                                }
                                Context.CacheProvider.StoreItemAnalysisReport(itemReport);
                                itemReports.Add(itemReport);
                            }
                        }
                    }
                    if (itemReports.Count() > 0)
                    {
                        cachedResults.AddRange(itemReports);
                    }

                }
            }

            courseSpecificReports = GetCourseSpecificReports(cachedResults);
            return courseSpecificReports;
        }

        /// <summary>
        /// Gets the item reports for all courses.
        /// </summary>
        /// <param name="batch">The batch.</param>
        /// <returns></returns>
        private List<Bdc.ItemReport> GetItemReportsForAllCourses(Batch batch)
        {
            SessionManager.CurrentSession.ExecuteAsAdmin(batch);
            var itemReports = new List<Bdc.ItemReport>();
            for (int index = 0; index < batch.Commands.Count(); index++)
            {
                if (!batch.CommandAs<GetItemAnalysis>(index).ItemAnalysis.IsNullOrEmpty())
                {
                    var entityId = batch.CommandAs<GetItemAnalysis>(index).SearchParameters.EntityId;
                    var itemId = batch.CommandAs<GetItemAnalysis>(index).SearchParameters.ItemId;

                    var rubricGradeAnalysis = batch.CommandAs<GetItemAnalysis>(index).ItemAnalysis;
                    var itemReport = new Bdc.ItemReport()
                    {
                        EntityId = entityId,
                        ItemId = itemId
                    };
                    if (!rubricGradeAnalysis.IsNullOrEmpty())
                    {
                        itemReport.RubricGrades = new List<Bdc.RubricGrade>();
                        foreach (var itemAnalysis in rubricGradeAnalysis)
                        {
                            //the number of rubric details tell us the columns of the rubric and the scores.
                            var rubricDetailResults = itemAnalysis.Grades;
                            if (rubricDetailResults.Count > 0 && itemAnalysis.RubricRule != null)
                            {
                                var rubricGrades = new List<Bdc.RubricGrade>();
                                var rubricRuleId = itemAnalysis.RubricRule.Id;
                                foreach (var rubricColumnGrade in rubricDetailResults)
                                {
                                    var grade = new Bdc.RubricGrade()
                                    {
                                        RubricRuleId = rubricRuleId,
                                        EnrollmentId = rubricColumnGrade.EnrollmentId,
                                        Achieved = rubricColumnGrade.Achieved,
                                        Possible = rubricColumnGrade.Possible
                                    };
                                    rubricGrades.Add(grade);
                                }
                                itemReport.RubricGrades.AddRange(rubricGrades);
                            }
                        }
                        itemReports.Add(itemReport);
                    }
                }
            }
            return itemReports;
        }

        /// <summary>
        /// Gets the course specific reports.
        /// </summary>
        /// <param name="itemSpecificReports">The item specific reports.</param>
        /// <returns></returns>
        private List<Bdc.RubricsReport> GetCourseSpecificReports(List<Bdc.ItemReport> itemSpecificReports)
        {
            var rubricReports = new List<Bdc.RubricsReport>();

            var entitySpecificReports = GetEntitySpecificReports(itemSpecificReports);

            //loop per entityid
            foreach (var entityId in entitySpecificReports.Keys)
            {
                var itemReports = entitySpecificReports[entityId];
                var rubricReport = new Bdc.RubricsReport()
                {
                    EntityId = entityId,
                    StudentReports = new List<Bdc.StudentRubricReport>()
                };

                var rubricGradesPerEnrollment = GetEnrollmentSpecificReport(itemReports);

                //once we have full grades per enrollment let's compute the total per rubric rules
                foreach (var enrollmentId in rubricGradesPerEnrollment.Keys)
                {
                    var rubricGradesCompleteListPerEntityId = rubricGradesPerEnrollment[enrollmentId];
                    var rubricGradePerRubricRuleId = from c in rubricGradesCompleteListPerEntityId group c by c.RubricRuleId into p select p;

                    var rubricResults = GetRubricRuleSpecificReports(rubricGradePerRubricRuleId);
                    var perEnrollmentReport = new Bdc.StudentRubricReport()
                    {
                        EnrollmentId = enrollmentId,
                        RubricResults = rubricResults,
                    };

                    rubricReport.StudentReports.Add(perEnrollmentReport);
                }
                rubricReports.Add(rubricReport);
            }
            return rubricReports;
        }

        /// <summary>
        /// Gets the rubric rule specific reports.
        /// </summary>
        /// <param name="rubricGradePerRubricRuleId">The rubric grade per rubric rule id.</param>
        /// <returns></returns>
        private Dictionary<string, Bdc.RubricGrade> GetRubricRuleSpecificReports(IEnumerable<IGrouping<string, Bdc.RubricGrade>> rubricGradePerRubricRuleId)
        {
            var rubricResults = new Dictionary<string, Bdc.RubricGrade>();
            foreach (var group in rubricGradePerRubricRuleId)
            {
                var rubricGradesListPerRule = group.ToList();
                if (!rubricGradesListPerRule.IsNullOrEmpty())
                {
                    var rubricResultCombined = new Bdc.RubricGrade();
                    var totalAchieved = (from c in rubricGradesListPerRule select c.Achieved).Sum();
                    var totalPossible = (from c in rubricGradesListPerRule select c.Possible).Sum();
                    rubricResultCombined.Achieved = totalAchieved;
                    rubricResultCombined.Possible = totalPossible;
                    rubricResultCombined.RubricRuleId = group.Key;

                    rubricResults.Add(group.Key, rubricResultCombined);
                }
            }
            return rubricResults;
        }


        /// <summary>
        /// Gets the rubric grade per enrollment.
        /// </summary>
        /// <param name="itemReports">The item reports.</param>
        /// <returns></returns>
        private Dictionary<string, List<Bdc.RubricGrade>> GetEnrollmentSpecificReport(List<Bdc.ItemReport> itemReports)
        {
            var rubricGradesPerEnrollment = new Dictionary<string, List<Bdc.RubricGrade>>();

            foreach (var itemReport in itemReports)
            {
                var rubricGrades = itemReport.RubricGrades;

                foreach (var rubricGrade in rubricGrades)
                {
                    if (rubricGradesPerEnrollment.ContainsKey(rubricGrade.EnrollmentId))
                    {
                        rubricGradesPerEnrollment[rubricGrade.EnrollmentId].Add(rubricGrade);
                    }
                    else
                    {
                        rubricGradesPerEnrollment.Add(rubricGrade.EnrollmentId, new List<Bdc.RubricGrade> { rubricGrade });
                    }
                }
            }
            return rubricGradesPerEnrollment;
        }


        /// <summary>
        /// Gets the entity specific reports.
        /// </summary>
        /// <param name="itemSpecificReports">The item specific reports.</param>
        /// <returns></returns>
        private Dictionary<string, List<Bdc.ItemReport>> GetEntitySpecificReports(List<Bdc.ItemReport> itemSpecificReports)
        {
            var reportsPerEntityId = from c in itemSpecificReports group c by c.EntityId into g select g;

            //per entityId
            var entitySpecificReports = new Dictionary<string, List<Bdc.ItemReport>>();
            foreach (var group in reportsPerEntityId)
            {
                entitySpecificReports.Add(group.Key, group.ToList());
            }
            return entitySpecificReports;
        }

    }
}
