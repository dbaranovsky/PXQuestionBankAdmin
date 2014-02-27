using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.Common.Logging;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using Adc = Bfw.Agilix.DataContracts;
using Bdc = Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.Direct.Services
{
    /// <summary>
    /// Implementation of the IGradeActions interface.
    /// </summary>
    public class GradeActions : IGradeActions
    {
        #region Properties

        /// <summary>
        /// The IBusinessContext implementation to use.
        /// </summary>
        protected IBusinessContext Context { get; set; }

        /// <summary>
        /// The SessionManager implementation to use.
        /// </summary>
        protected ISessionManager SessionManager { get; set; }

        /// <summary>
        /// The IDocumentConverter implementation to use.
        /// </summary>
        protected IDocumentConverter DocumentConverter { get; set; }

        /// <summary>
        /// The IContentActions implementation to use.
        /// </summary>
        protected IContentActions ContentActions { get; set; }

        /// <summary>
        /// The IEnrollmentActions implementation to use.
        /// </summary>
        protected IEnrollmentActions EnrollmentActions { get; set; }

        protected List<TeacherResponse> TeacherResponses;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GradeActions"/> class.
        /// </summary>
        /// <param name="context">The IBusinessContext implementation.</param>
        /// <param name="gradeService">The IGradeService implementation.</param>
        /// <param name="contentActions">The IContentActions implementation.</param>
        /// <param name="enrollmentActions">The IEnrollmentActions implementation.</param>
        public GradeActions(IBusinessContext context, ISessionManager sessionManager, IDocumentConverter docConverter, IContentActions contentActions, IEnrollmentActions enrollmentActions)
        {
            Context = context;
            SessionManager = sessionManager;
            DocumentConverter = docConverter;
            ContentActions = contentActions;
            EnrollmentActions = enrollmentActions;
        }

        #endregion

        #region IGradeActions Members

        /// <summary>
        /// Gets gradebook detail for the specified user.
        /// </summary>
        /// <param name="userId">ID of the user for which to get grades.</param>
        /// <param name="entityId">Optional entity ID by which to filter the returned data.</param>
        /// <param name="status">Graded item status by which to filter the returned data.</param>
        /// <param name="dueAfter">Date range by which to filter the returned data.</param>
        /// <param name="dueBefore">Date range by which to filter the returned data.</param>
        /// <returns></returns>
        public IEnumerable<DataContracts.Grade> GetGrades(string userId, string entityId, GradedItemStatus status, DateTime dueAfter, DateTime dueBefore)
        {
            IEnumerable<DataContracts.Grade> grades = null;

            using (Context.Tracer.StartTrace(string.Format("GradeActions.GetGrades(userId={0}, entityId={1}, status={2}, dueAfter={3}, dueBefore={4})", userId, entityId, status, dueAfter, dueBefore)))
            {
                var cmd = new GetGrades
                {
                    SearchParameters = new Adc.GradeSearch
                    {
                        UserId = userId,
                        EntityId = entityId
                    }
                };
                SessionManager.CurrentSession.Execute(cmd);

                if (!cmd.Enrollments.IsNullOrEmpty() && cmd.Enrollments.Count() == 1)
                {
                    grades = cmd.Enrollments.First().ItemGrades.Map(g => g.ToGrade());

                    if (status == GradedItemStatus.HasBeenGraded)
                    {
                        grades = grades.Where(g => g.ScoredDate.HasValue && g.ScoredDate.Value.Year > DateTime.MinValue.Year);
                    }
                    else if (status == GradedItemStatus.HasBeenSubmittedOrGraded)
                    {
                        grades = grades.Where(g => (g.SubmittedDate.HasValue && g.SubmittedDate.Value.Year > DateTime.MinValue.Year) || (g.ScoredDate.HasValue && g.ScoredDate.Value.Year > DateTime.MinValue.Year));
                    }

                    grades = grades.Where(g => g.GradedItem.AssignmentSettings.DueDate > dueAfter);
                    grades = grades.Where(g => g.GradedItem.AssignmentSettings.DueDate <= dueBefore);
                }
            }

            return grades;
        }
        /// <summary>
        /// Get grades for all students for a specific item
        /// </summary>
        /// <param name="entityId">Course that Owns the item</param>
        /// <param name="itemId">Id of the Item</param>
        /// <param name="userId">user Id</param>
        /// <returns>List of grades for all students for this item</returns>
        public IEnumerable<DataContracts.Grade> GetGrades(string entityId, List<string> itemId)
        {
            var results = new List<DataContracts.Grade>();

            using (Context.Tracer.DoTrace("GradeActions.GetGrades(entityId={0},itemId={1})", entityId, itemId))
            {
                var cmd = new GetGradebookSummary()
                {
                  EntityId = entityId,
                  ItemIds = itemId
                };

                SessionManager.CurrentSession.Execute(cmd);

                foreach (var enrollmentGrade in cmd.EnrollmentGrades)
                {
                    var grade = enrollmentGrade.Value.ToGrade();
                    grade.EnrollmentId = enrollmentGrade.Key;
                    grade.GradedItem.CourseId = entityId;
                    results.Add(grade);
                }
            }

            return results;
        }

        public IEnumerable<DataContracts.Grade> GetGradesByEnrollment(string enrollmentId, List<string> itemIds)
        {

            var results = new List<DataContracts.Grade>();
            using (Context.Tracer.DoTrace("GradeActions.GetGrades(entityId={0},itemIds={1})", enrollmentId, itemIds))
            {
                var checkItemIdsLength = new StringBuilder();
                var getGradeItemIds = new List<string> {"*"};
                if (itemIds != null)
                {
                    itemIds.ForEach(i => checkItemIdsLength.Append(i + "|"));
                    //Each '|' will encode into %7c so we need to calcualte the length of that too
                    getGradeItemIds = (checkItemIdsLength.Length + (itemIds.Count * 2)) > 1950 ? new List<string> { "*" } : itemIds;
                }

                var cmd = new GetGrades
                {
                    SearchParameters = new Adc.GradeSearch
                    {
                        EnrollmentId = enrollmentId,
                        ItemIds = getGradeItemIds
                    }
                };

                SessionManager.CurrentSession.Execute(cmd);

                foreach (var enrollment in cmd.Enrollments)
                {
                    results.AddRange(enrollment.ItemGrades.Map(g => g.ToGrade()));
                }
                if (itemIds != null)
                {
                    results = (from g in results
                        join i in itemIds on g.GradedItem.Id equals i
                        select g).ToList();
                }
            }
            return results;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enrollmentId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public DataContracts.GradeList GetGradeList(string enrollmentId, string itemId)
        {
            var results = new GradeList();
            if (string.IsNullOrWhiteSpace(enrollmentId) || string.IsNullOrWhiteSpace(itemId))
                return results;
            using (Context.Tracer.DoTrace("GradeActions.GetGradeList(enrollmentId={0},itemId={1})", enrollmentId, itemId))
            {
                var cmd = new GetGradeList()
                {
                    SearchParameters = new Adc.GradeListSearch()
                    {
                        EnrollmentId = enrollmentId,
                        ItemId = itemId
                    }
                };

                SessionManager.CurrentSession.Execute(cmd);
                results = cmd.GradeList.ToGradeList();
            }

            return results;
        }

        /// <summary>
        /// Gets the history of submissions for an item in the gradebook for the specified user enrollment.
        /// </summary>
        /// <param name="enrollmentId">Enrollment ID of user for which to get submission history.</param>
        /// <param name="itemId">ID of the item for which to get submission history.</param>
        /// <returns></returns>
        public IEnumerable<DataContracts.Submission> GetSubmissions(string enrollmentId, string itemId)
        {
            IEnumerable<DataContracts.Submission> submissions;

            using (Context.Tracer.StartTrace(string.Format("GradeActions.GetSubmissions(enrollmentId={0}, itemId={1})", enrollmentId, itemId)))
            {
                var cmd = new GetSubmissions
                {
                    SearchParameters = new Bfw.Agilix.DataContracts.SubmissionSearch
                    {
                        EnrollmentId = enrollmentId,
                        ItemId = itemId
                    }
                };
                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

                if (!cmd.Submissions.IsNullOrEmpty())
                {
                    foreach (var submission in cmd.Submissions)
                    {
                        submission.SubmittedDate = DateTimeConversion.UtcRelativeAdjustCommon(submission.SubmittedDate, Context.Course.CourseTimeZone);
                    }

                    submissions = cmd.Submissions.Map(s => s.ToSubmission());
                }
                else
                {
                    submissions = new List<DataContracts.Submission>();
                }

            }

            return submissions;
        }

        /// <summary>
        /// Adds a student submission for an activity to the server.
        /// </summary>
        /// <param name="entityId">ID of the entity associated with the submission.</param>
        /// <param name="submission">The submission info to add to the server.</param>
        public void AddStudentSubmission(string entityId, DataContracts.Submission submission)
        {
            using (Context.Tracer.StartTrace(string.Format("GradeActions.AddStudentSubmission(entityId={0}, submission={1})", entityId, submission)))
            {
                var cmd = new PutStudentSubmission
                {
                    EntityId = entityId,
                    Submission = submission.ToSubmission()
                };
                var enrollmentId = !String.IsNullOrEmpty(submission.EnrollmentId) ? submission.EnrollmentId : Context.EnrollmentId;
                cmd.Submission.EnrollmentId = enrollmentId;
                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            }
        }



        /// <summary>
        /// Gets a student's submission.
        /// </summary>
        /// <param name="enrollmentId">ID of the user's enrollment to which this student submission belongs.</param>
        /// <param name="itemId">ID of the item (in the course manifest) to which this student submission belongs.</param>
        /// <returns></returns>
        public DataContracts.Submission GetStudentSubmission(string enrollmentId, string itemId)
        {
            DataContracts.Submission submission = null;

            using (Context.Tracer.StartTrace(String.Format("GradeActions.GetStudentSubmission(enrollmentId={0}, itemId={1})", enrollmentId, itemId)))
            {
                var cmd = new GetStudentSubmission
                {
                    SearchParameters = new Agilix.DataContracts.SubmissionSearch
                    {
                        EnrollmentId = enrollmentId,
                        ItemId = itemId
                    }
                };

                try
                {
                    SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                }
                catch (BadDlapResponseException ex)
                {
                    // Getting a DoesNotExist is OK for Submissions. Example error:
                    // Message = "Submission for enrollment 110603, item 'e353f62788444f969e813a299a840cc8', version 0 does not exist."
                }

                submission = cmd.Submission != null ? cmd.Submission.ToSubmission() : new DataContracts.Submission();
            }

            var cmd2 = new GetTeacherResponse
            {
                SearchParameters = new Agilix.DataContracts.TeacherResponseSearch
                {
                    EnrollmentId = enrollmentId,
                    ItemId = itemId
                }
            };

            try
            {
                SessionManager.CurrentSession.Execute(cmd2);
                var tr = cmd2.TeacherResponse.ToTeacherResponse();

                TeacherResponses = new List<TeacherResponse> { };
                FlattenTeacherResponses(tr);

                foreach (var trR in TeacherResponses)
                {
                    foreach (var trQA in submission.QuestionAttempts)
                    {
                        foreach (var trQA2 in trQA.Value)
                        {
                            if (trQA2.PartId == trR.ForeignId)
                            {
                                trQA2.PointsComputed = trR.PointsComputed.ToString();
                                trQA2.PointsPossible = trR.PointsPossible.ToString();
                            }
                        }
                    }
                }
            }
            catch { }

            return submission;
        }


        public DataContracts.Submission GetStudentSubmission(string enrollmentId, string itemId, int version)
        {
            DataContracts.Submission submission = null;

            using (Context.Tracer.StartTrace(String.Format("GradeActions.GetStudentSubmission(enrollmentId={0}, itemId={1}, verison={2})", enrollmentId, itemId, version)))
            {
                var cmd = new GetStudentSubmission
                {
                    SearchParameters = new Agilix.DataContracts.SubmissionSearch
                    {
                        EnrollmentId = enrollmentId,
                        ItemId = itemId,
                        Version = version
                    }
                };
                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                submission = cmd.Submission != null ? cmd.Submission.ToSubmission() : new DataContracts.Submission();
            }

            var cmd2 = new GetTeacherResponse
            {
                SearchParameters = new Agilix.DataContracts.TeacherResponseSearch
                {
                    EnrollmentId = enrollmentId,
                    ItemId = itemId
                }
            };

            try
            {
                SessionManager.CurrentSession.Execute(cmd2);
                var tr = cmd2.TeacherResponse.ToTeacherResponse();

                TeacherResponses = new List<TeacherResponse> { };
                FlattenTeacherResponses(tr);

                foreach (var trR in TeacherResponses)
                {
                    foreach (var trQA in submission.QuestionAttempts)
                    {
                        foreach (var trQA2 in trQA.Value)
                        {
                            if (trQA2.PartId == trR.ForeignId)
                            {
                                trQA2.PointsComputed = trR.PointsComputed.ToString();
                                trQA2.PointsPossible = trR.PointsPossible.ToString();
                            }
                        }
                    }
                }
            }
            catch { }

            return submission;
        }
        /// <summary>
        /// flattens a TeacherResponse object to facilitate processing
        /// </summary>
        /// <param name="tr">The TeacherResponse response object to flatten</param>
        /// <returns></returns>
        protected void FlattenTeacherResponses(TeacherResponse tr)
        {
            TeacherResponses.Add(tr);
            foreach (var trChild in tr.Responses)
            {
                FlattenTeacherResponses(trChild);
            }
        }

        public IEnumerable<Submission> GetEntitySubmissions(string entityId, bool outstanding = true)
        {
            List<DataContracts.Submission> submissions = null;
            using (Context.Tracer.StartTrace(String.Format("GradeActions.GetEntitySubmission(entityId={0}, outstanding={1})", entityId, outstanding)))
            {
                var cmd = new GetEntityWork2
                {
                    SearchParameters = new Agilix.DataContracts.WorkSearch
                    {
                        EntityId = entityId,
                        OutStanding = outstanding
                    }
                };
                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                submissions = cmd.Works != null ? cmd.Works.Select(e=>new Submission() 
                { 
                    ItemId= e.ItemId, EnrollmentId = e.EnrollmentId, SubmissionStatus=SubmissionStatus.NotGraded, 
                    SubmittedDate = DateTime.Parse(e.SubmittedDate), Version=int.Parse(e.SubmittedVersion),
                    ItemName = e.Title
                }).ToList() : new List<Submission>();
            }

            return submissions;
        }

        public IEnumerable<DataContracts.Submission> GetStudentSubmissionInfo(string entityId, string itemId, string enrollmentId, IEnrollmentActions enrollmentActions)
        {
            return GetStudentSubmissionInfo(entityId, new List<string>() { itemId }, enrollmentId, enrollmentActions);
        }
        /// <summary>
        /// Retrieves information about one or more student submissions from the server.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="itemId">ID of the item (in the course manifest) to which this submission belongs.</param>
        /// <param name="enrollmentId">ID of the user’s enrollment to which this submission belongs.</param>
        /// <param name="enrollmentActions">The IEnrollmentActions implementation to use.</param>
        /// <returns></returns>
        [Obsolete("This method depends on an IEnrollmentActions object.", false)]
        public IEnumerable<DataContracts.Submission> GetStudentSubmissionInfo(string entityId, List<string> itemId, string enrollmentId, IEnrollmentActions enrollmentActions)
        {
            using (Context.Tracer.StartTrace(String.Format("GradeActions.GetStudentSubmissionInfo(entityId={0}, itemId={1}, enrollmentId={2})", entityId, string.Join(",", itemId), enrollmentId)))
            {
                var enrollments = enrollmentActions.GetEntityEnrollmentsAsAdmin(entityId);
                var userEnrollment = enrollments.Where(t => t.Id == enrollmentId).FirstOrDefault();

                List<DataContracts.Submission> submissions = new List<Submission>();
                foreach (var item in itemId)
                {
                    var submission = new DataContracts.Submission
                    {
                        ItemId = item,
                        EnrollmentId = enrollmentId
                    };
                    if (userEnrollment != null && userEnrollment.User != null)
                    {
                        submission.StudentFirstName = userEnrollment.User.FirstName;
                        submission.StudentLastName = userEnrollment.User.LastName;
                    }
                    submissions.Add(submission);
                }
                return GetStudentsSubmissionInfo(submissions);
            }
        }

        public IEnumerable<DataContracts.Submission> GetStudentsSubmissionInfo(string entityId, string itemId, IEnrollmentActions enrollmentActions)
        {
            return GetStudentsSubmissionInfo(entityId, new List<string>() { itemId }, enrollmentActions);
        }

        /// <summary>
        /// Retrieves information about one or more student submissions from the server.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="itemId">ID of the item (in the course manifest) to which this submission belongs.</param>
        /// <param name="enrollmentActions">The IEnrollmentActions implementation to use.</param>
        /// <returns></returns>
        [Obsolete("This method depends on an IEnrollmentActions object.", false)]
        public IEnumerable<DataContracts.Submission> GetStudentsSubmissionInfo(string entityId, List<string> itemId, IEnrollmentActions enrollmentActions)
        {
            using (Context.Tracer.StartTrace(String.Format("GradeActions.GetStudentSubmissionInfo(entityId={0}, itemId={1})", entityId, itemId)))
            {
                var submissions = new List<DataContracts.Submission>();
                var enrollments = enrollmentActions.GetEntityEnrollmentsAsAdmin(entityId);

                if (enrollments.Any())
                {
                    foreach (var enrollment in enrollments)
                    {
                        if (!enrollment.Flags.Contains("Participate")) continue;
                        foreach (var item in itemId)
                        {
                            var submission = new DataContracts.Submission
                            {
                                ItemId = item,
                                EnrollmentId = enrollment.Id,
                                StudentFirstName = enrollment.User.FirstName,
                                StudentLastName = enrollment.User.LastName
                            };
                            submissions.Add(submission);
                        }

                    }
                    return GetStudentsSubmissionInfo(submissions);
                }
                return null;
            }
        }

        /// <summary>
        /// Retrieves submission information for the specified submission collection.
        /// See http://gls.agilix.com/Docs/Command/PutTeacherResponse
        /// </summary>
        /// <param name="submissions">The submissions to get information for.</param>
        /// <returns></returns>
        public IEnumerable<DataContracts.Submission> GetStudentsSubmissionInfo(IEnumerable<DataContracts.Submission> submissions)
        {
            using (Context.Tracer.StartTrace(String.Format("GradeActions.GetStudentSubmissionInfo({0} submissions)", submissions.Count())))
            {
                var agxSubmissions = !submissions.IsNullOrEmpty()
                                         ? submissions.Map(s => s.ToSubmission())
                                         : new List<Adc.Submission>();

                var cmd = new GetStudentSubmissionInfo { Submissions = agxSubmissions.ToList() };
                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                var pxSubmissions = !cmd.Submissions.IsNullOrEmpty()
                           ? cmd.Submissions.Where(t => t.Submitted).Map(s => s.ToSubmission()).ToList()
                           : new List<DataContracts.Submission>();

                return SetSubmissionStatus(pxSubmissions);
            }
        }

        public IEnumerable<DataContracts.Submission> GetStudentsSubmissionInfoWithoutTeacherResponse(IEnumerable<DataContracts.Submission> submissions)
        {
            using (Context.Tracer.StartTrace(String.Format("GradeActions.GetStudentSubmissionInfo({0} submissions)", submissions.Count())))
            {
                var agxSubmissions = !submissions.IsNullOrEmpty()
                                         ? submissions.Map(s => s.ToSubmission())
                                         : new List<Adc.Submission>();

                var cmd = new GetStudentSubmissionInfo { Submissions = agxSubmissions.ToList() };
                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                var pxSubmissions = !cmd.Submissions.IsNullOrEmpty()
                           ? cmd.Submissions.Where(t => t.Submitted).Map(s => s.ToSubmission()).ToList()
                           : new List<DataContracts.Submission>();

                return pxSubmissions;
            }
        }

        /// <summary>
        /// Adds teacher response data including scores, comments, and grade status flags to the server.
        /// </summary>
        /// <param name="studentEnrollmentId">ID of the user's enrollment to which this teacher response belongs.</param>
        /// <param name="itemId">ID of the item (in the course manifest) to which this teacher response belongs.</param>
        /// <param name="teacherResponse">The teacher response business object.</param>
        public void AddTeacherResponse(string studentEnrollmentId, string itemId, DataContracts.TeacherResponse teacherResponse)
        {
            using (Context.Tracer.StartTrace(String.Format("GradeActions.AddTeacherResponse(enrollmentId={0}, itemId={1})", studentEnrollmentId, itemId)))
            {
                var cmd = new PutTeacherResponse
                {
                    StudentEnrollmentId = studentEnrollmentId,
                    ItemId = itemId,
                    TeacherResponse = teacherResponse.ToTeacherResponse()
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                if (!itemId.IsNullOrEmpty() && !Context.EntityId.IsNullOrEmpty())
                {
                    var report = new ItemReport()
                    {
                        ItemId = itemId,
                        EntityId = Context.EntityId
                    };

                    Context.CacheProvider.InvalidateItemAnalysisReport(report);
                }
                //SessionManager.CurrentSession.Execute(cmd);
            }
        }

        /// <summary>
        /// Gets a teacher's response to a student's submission.
        /// </summary>
        /// <param name="studentEnrollmentId">ID of the user's enrollment to which this teacher response belongs.</param>
        /// <param name="itemId">ID of the item (in the course manifest) to which this teacher response belongs.</param>
        /// <returns></returns>
        public DataContracts.TeacherResponse GetTeacherResponse(string studentEnrollmentId, string itemId)
        {
            using (Context.Tracer.StartTrace(String.Format("GradeActions.GetTeacherResponse(studentEnrollmentId={0}, itemId={1})", studentEnrollmentId, itemId)))
            {
                var cmd = new GetTeacherResponse
                {
                    SearchParameters = new Agilix.DataContracts.TeacherResponseSearch
                    {
                        EnrollmentId = studentEnrollmentId,
                        ItemId = itemId
                    }
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                return cmd.TeacherResponse.ToTeacherResponse();
            }
        }

        /// <summary>
        /// Gets a teacher's response to a student's submission.
        /// </summary>
        /// <param name="studentEnrollmentId">ID of the user's enrollment to which this teacher response belongs.</param>
        /// <param name="itemId">ID of the item (in the course manifest) to which this teacher response belongs.</param>
        /// <param name="version">Version of the teacher response to retrieve.</param>
        /// <returns></returns>
        public DataContracts.TeacherResponse GetTeacherResponse(string studentEnrollmentId, string itemId, int version)
        {
            using (Context.Tracer.StartTrace(String.Format("GradeActions.GetTeacherResponse(studentEnrollmentId={0}, itemId={1}, version={2})", studentEnrollmentId, itemId, version)))
            {
                var cmd = new GetTeacherResponse
                {
                    SearchParameters = new Agilix.DataContracts.TeacherResponseSearch
                    {
                        EnrollmentId = studentEnrollmentId,
                        ItemId = itemId,
                        Version = version
                    }
                };
                SessionManager.CurrentSession.Execute(cmd);
                return cmd.TeacherResponse.ToTeacherResponse();
            }
        }

        /// <summary>
        /// Gets teacher responses for itemids specified
        /// </summary>
        /// <param name="itemIds">List of Item Ids</param>
        public void GetTeacherResponseInfo(List<string> itemIds)
        {

            // get all the id's      

        }

        /// <summary>
        /// For a specified enrollment, gets the percent of graded items out of all gradable items.
        /// </summary>
        /// <param name="e">The enrollment.</param>
        /// <returns></returns>
        public double GetPercentGraded(DataContracts.Enrollment enrollment)
        {
            using (Context.Tracer.StartTrace(String.Format("GradeActions.GetPercentGraded(enrollment.Id={0})", enrollment.Id)))
            {
                var gradableItems = GetGrades(enrollment.User.Id, Context.EntityId, GradedItemStatus.Any,
                                              DateTime.MinValue, DateTime.MaxValue);

                var gradedItems = GetGrades(enrollment.User.Id, Context.EntityId, GradedItemStatus.HasBeenGraded, DateTime.MinValue, DateTime.MaxValue);
                return
                    (!gradableItems.IsNullOrEmpty() && !gradedItems.IsNullOrEmpty()) ?
                    ((double)gradedItems.Count()) / gradableItems.Count() :
                    0;
            }
        }

        /// <summary>
        /// Uploads a document submission.
        /// </summary>
        /// <param name="entityId">ID of the parent course or section.</param>
        /// <param name="itemId">ID of the item to attach the document to.</param>
        /// <param name="docTitle">Title of the document.</param>
        /// <param name="fileStream">Stream to the content of the document.</param>
        /// <param name="outputType">Output type for the document.</param>
        /// <param name="resourceMapActions">The IResourceMapActions implementation.</param>
        /// <returns></returns>
        [Obsolete("This method depends on an IResourceMapActions object and an IContentActions object.", false)]
        public string UploadDocument(string entityId, string itemId, string docTitle, System.IO.Stream fileStream, DataContracts.DocumentOutputType outputType, IResourceMapActions resourceMapActions)
        {
            using (Context.Tracer.StartTrace(String.Format("GradeActions.UploadDocument(entityId={0}, itemId={1}, docTitle={2}, outputType={3})", entityId, itemId, docTitle, outputType)))
            {
                var docStream = DocumentConverter.ConvertDocument(new DocumentConversion
                {
                    DataStream = fileStream,
                    FileName = docTitle,
                    OutputType = outputType
                });

                var wordCount = DocumentConverter.GetDocumentWordCount(fileStream);

                StreamReader docReader = null;
                if (docStream.Length > 0) docReader = new StreamReader(docStream);

                var resId = Guid.NewGuid().ToString("N");
                var url = string.Format("Templates/Data/XmlResources/Documents/{0}.pxres", resId);
                var resource = new XmlResource
                {
                    Status = ResourceStatus.Normal,
                    Url = url,
                    EntityId = entityId,
                    Title = docTitle,
                    Body = (docReader != null) ? docReader.ReadToEnd() : "",
                    ExtendedProperties = new Dictionary<string, string> { { ResourceExtendedProperty.Status.ToString(), "saved" }, { "WordCount", wordCount } }
                };

                ContentActions.StoreResources(new List<Resource> { resource });

                // Add resource relation to item.
                if (!string.IsNullOrEmpty(itemId))
                {
                    resourceMapActions.AddResourceMap(resource, itemId, "");
                }
                return resId;
            }
        }

        /// <summary>
        /// Gets a submission aspose document stream.
        /// </summary>
        /// <param name="resourcePaths">The resource paths.</param>
        /// <param name="entityId">The entity id.</param>
        /// <param name="outputType">Type of the output.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public System.IO.Stream GetDocumentsStreamFromResource(IEnumerable<string> resourcePaths, string entityId, DataContracts.DocumentOutputType outputType, out string fileName)
        {
            using (Context.Tracer.StartTrace(String.Format("GradeActions.GetDocumentsStreamFromResource(resourcePaths={0}, entityId={1}, outputType={2})", String.Join("|", resourcePaths), entityId, outputType)))
            {
                var resources = ContentActions.ListResources(resourcePaths, entityId);
                var docConversions = resources.Select(resource => new DocumentConversion
                {
                    DataStream = resource.GetStream(),
                    FileName = resource.Name,
                    OutputType = outputType
                }).ToList();

                // Set fileName if its only one doc, if > 1 then zip file name should be set by caller.
                fileName = docConversions.Count == 1 ? docConversions.First().FileName : "";
                return DocumentConverter.ConvertDocuments(docConversions);
            }
        }

        /// <summary>
        /// Returns submission aspose document stream if one document requested.
        /// Returns submission aspose document zipped stream if  more than one document requested.
        /// </summary>
        /// <param name="entityId">ID of the course.</param>
        /// <param name="enrollmentIds">Enrollment ID list for the submissions.</param>
        /// <param name="itemId">ID of the item to which submission made by student.</param>
        /// <param name="outputType">Download file format.</param>
        /// <param name="fileName">Output file name.</param>
        /// <returns></returns>
        public System.IO.Stream GetSubmissionsStream(string entityId, List<string> enrollmentIds, string itemId, DataContracts.DocumentOutputType outputType, out string fileName)
        {
            using (Context.Tracer.StartTrace(String.Format("GradeActions.GetSubmissionsStream(entityId={0}, enrollmentIds={1}, itemId={2}, outputType={3})", entityId, String.Join("|", enrollmentIds), itemId, outputType)))
            {
                var docConversions = new List<DocumentConversion>();
                var enrollments = EnrollmentActions.GetEntityEnrollments(entityId);
                foreach (var enrollmentId in enrollmentIds)
                {
                    var submission = GetStudentSubmission(enrollmentId, itemId);
                    if (submission.Body == null) submission.Body = "<div></div>";
                    var eId = enrollmentId;
                    var currentEnrollmentUser = enrollments.Where(e => e.Id == eId).FirstOrDefault();
                    var studentFullName = currentEnrollmentUser.User.LastName + ", " + currentEnrollmentUser.User.FirstName;
                    var subBodyStream = new MemoryStream();
                    var subBodyStreamSw = new StreamWriter(subBodyStream);
                    subBodyStreamSw.Write(System.Web.HttpUtility.HtmlDecode(submission.Body));
                    subBodyStreamSw.Flush();
                    subBodyStream.Seek(0, SeekOrigin.Begin);
                    docConversions.Add(new DocumentConversion { DataStream = subBodyStream, FileName = studentFullName, OutputType = outputType });
                }

                // Set fileName if its only one doc, if > 1 then zip file name should be set by caller.
                fileName = docConversions.Count == 1 ? docConversions.First().FileName : "";
                return DocumentConverter.ConvertDocuments(docConversions);
            }
        }

        /// <summary>
        /// Returns submission aspose document stream if one document requested.
        /// Returns submission aspose document zipped stream if  more than one document requested.
        /// </summary>
        /// <param name="entityId">ID of the course.</param>
        /// <param name="enrollmentIds">Enrollment ID list for the submissions.</param>
        /// <param name="itemId">ID of the item to which submission made by student.</param>
        /// <param name="outputType">Download file format.</param>
        /// <param name="fileName">Output file name.</param>
        /// <returns></returns>
        public System.IO.Stream GetDropboxSubmissionsStream(string entityId, string enrollmentId, string itemId, DataContracts.DocumentOutputType outputType)
        {
            using (Context.Tracer.StartTrace(String.Format("GradeActions.GetSubmissionsStream(entityId={0}, enrollmentIds={1}, itemId={2}, outputType={3})", entityId, String.Join("|", enrollmentId), itemId, outputType)))
            {
                var docConversions = new List<DocumentConversion>();
                var enrollments = EnrollmentActions.GetEntityEnrollments(entityId);
                var submission = GetStudentSubmission(enrollmentId, itemId);
                if (submission.Body == null) submission.Body = Convert.ToBase64String(System.Text.Encoding.Default.GetBytes("<div></div>"));
                Byte[] bytes = Convert.FromBase64String(submission.Body);
                Stream stream = new MemoryStream(bytes);
                return stream;
            }
        }


        /// <summary>
        /// Retrieves information about one or more student submissions from the server.
        /// </summary>
        /// <param name="entityId">ID of the course or section to which the submissions belong.</param>
        /// <param name="enrollmentId">ID of the user’s enrollment to which the submissions belong.</param>
        /// <param name="itemIds">ID of the items (in the course manifest).</param>
        /// <returns></returns>
        public IEnumerable<DataContracts.Submission> GetStudentSubmissionInfoList(string entityId, string enrollmentId, List<string> itemIds)
        {
            using (Context.Tracer.StartTrace(String.Format("GradeActions.GetStudentSubmissionInfoList(entityId={0}, enrollmentId={1}, itemIds={2})", entityId, enrollmentId, String.Join("|", itemIds))))
            {
                var submissions = new List<Submission>();

                if (itemIds.Any())
                {
                    foreach (var id in itemIds)
                    {
                        var submission = new Submission
                        {
                            ItemId = id,
                            EnrollmentId = enrollmentId
                        };
                        submissions.Add(submission);
                    }
                    return GetStudentsSubmissionInfo(submissions);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the item and category gradebook weights for the specified entity.
        /// See http://gls.agilix.com/Docs/Command/GetGradebookWeights.
        /// </summary>
        /// <param name="entityId">ID of the course or section to get weights for.</param>
        /// <returns></returns>
        public DataContracts.GradeBookWeights GetGradeBookWeights(string entityId)
        {
            using (Context.Tracer.StartTrace(String.Format("GradeActions.GetGradeBookWeights(entityId={0})", entityId)))
            {
                var cmd = new GetGradeBookWeights
                {
                    SearchParameters = new Adc.GradeBookWeightSearch
                    {
                        EntityId = entityId
                    }
                };

                //the user of a readonly course(ex: Publisher Template or Program Manager template accessed by non-owner) will not have access to the owner created items
                if (Context.IsCourseReadOnly)
                {
                    SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                }
                else
                {
                    SessionManager.CurrentSession.Execute(cmd);
                }

                return cmd.GradeBookWeights.ToGradeBookWeight();
            }
        }

        /// <summary>
        /// Gets all assignments/grades with for a student (won't work with impersonated instructor account)
        /// </summary>
        /// <param name="userId">Required User id (takes precedence over enrollment id if not null)</param>
        /// <param name="enrollmentId">Required Enrollment id</param>
        /// <param name="utcOffSet">Required the course time difference between GMT and local time, in minutes.</param>
        /// <param name="showCompleted">Optional flag to show completed assignments (false by default)</param>
        /// <param name="showPastDue">Optional flag to show past due assignments (false by default)(</param>
        /// <param name="days">Optional day range (2 weeks by default)</param>
        /// <returns></returns>
        public IEnumerable<DataContracts.Grade> GetDueSoonItemsWithGrades(string userId, string enrollmentId, int utcOffSet, bool showCompleted = false, bool showPastDue = false, int days = 14)
        {
            IEnumerable<DataContracts.Grade> grades = null;

            using (Context.Tracer.StartTrace(String.Format("GradeActions.GetDueSoonItemsWithGrades(userId={0}, enrollmentId={1}, utcOffSet={2}, showCompleted={3}, showPastDue={4}, days={5})", userId, enrollmentId, utcOffSet, showCompleted, showPastDue, days)))            
            {
                var cmd = new GetDueSoonList
                {
                    SearchParameters = new Adc.DueSoonSearch
                    {
                        UserId = userId,
                        EnrollmentId = enrollmentId,
                        UtcOffSet = utcOffSet,
                        ShowCompleted = showCompleted,
                        ShowPastDue = showPastDue,
                        Days = days
                    }
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

                grades = cmd.Grades.Map(g => g.ToGrade());
            }

            return grades;
        }

        /// <summary>
        /// Gets all assignments/grades for the enrollment for a student (won't work with impersonated instructor account)
        /// </summary>
        /// <param name="enrollmentId">Required Enrollment id</param>
        /// <param name="utcOffSet">Required the course time difference between GMT and local time, in minutes.</param>
        /// <returns></returns>
        public IEnumerable<Grade> GetDueSoonItemsWithGrades(string enrollmentId, int utcOffSet)
        {
            return GetDueSoonItemsWithGrades(null, enrollmentId, utcOffSet);
        }

        /// <summary>
        /// Determines whether the specified item has submissions.
        /// </summary>
        /// <param name="itemId">The item ID.</param>
        /// <returns>
        ///   <c>true</c> if the specified item has submissions; otherwise, <c>false</c>.
        /// </returns>
        public bool HasSubmissions(string itemId)
        {
            using (Context.Tracer.StartTrace("GradeActions.GradeActions.HasSubmissions"))
            {
                var cmd = new GetSubmissions
                {
                    SearchParameters = new Bfw.Agilix.DataContracts.SubmissionSearch
                        {
                            ItemId = itemId
                        }
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                return !cmd.Submissions.IsNullOrEmpty();
            }
        }

        /// <summary>
        /// Determines whether [has saved document] [the specified enrollment id].
        /// </summary>
        /// <param name="enrollmentId">The enrollment id.</param>
        /// <param name="itemId">The item id.</param>
        /// <param name="path">The path.</param>
        /// <returns>
        ///   <c>true</c> if [has saved document] [the specified enrollment id]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasSavedDocument(string enrollmentId, string itemId, string path)
        {

            using (Context.Tracer.StartTrace(String.Format("GradeActions.GetStudentSubmission(enrollmentId={0}, itemId={1})", enrollmentId, itemId)))
            {
                var cmd = new GetDocument
                {
                    SearchParameters = new Agilix.DataContracts.SubmissionSearch
                    {
                        EnrollmentId = enrollmentId,
                        ItemId = itemId,
                        FilePath = path,
                        PackageType = "xml"
                    }
                };
                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                return (cmd.SubmissionXml != null || cmd.Submission != null);

            }

        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Updates the submission status for a specified list of submissions based on teacher response.
        /// </summary>
        /// <param name="pxSubmissions">Submission list to update.</param>
        /// <returns></returns>
        private IEnumerable<DataContracts.Submission> SetSubmissionStatus(IEnumerable<Bdc.Submission> pxSubmissions)
        {
            using (Context.Tracer.StartTrace("GradeActions.SetSubmissionStatus"))
            {
                foreach (var submission in pxSubmissions)
                {
                    var response = GetTeacherResponse(submission.EnrollmentId, submission.ItemId);
                    submission.SubmissionStatus = DataContracts.SubmissionStatus.NotGraded;
                    if (submission.Grade != null)
                    {
                        submission.SubmissionStatus = DataContracts.SubmissionStatus.Graded;
                        if (submission.Grade.RawAchieved == 0 && submission.Grade.Achieved > 0)
                        {
                            submission.Grade.RawAchieved = submission.Grade.Achieved;
                        }
                        if (submission.Grade.RawPossible == 0 && submission.Grade.Possible > 0)
                        {
                            submission.Grade.RawPossible = submission.Grade.Possible;
                        }
                    }
                    else
                    {
                        submission.Grade = new DataContracts.Grade
                        {
                            RawAchieved = response.PointsAssigned,
                            RawPossible = response.PointsPossible,
                            Possible = response.PointsPossible,
                            Achieved = response.PointsAssigned
                        };

                        if (submission.Version > 0 && response.ScoredVersion == submission.Version)
                        {
                            submission.SubmissionStatus = DataContracts.SubmissionStatus.Graded;
                            if (response.Status == DataContracts.GradeStatus.AllowResubmission)
                            {
                                submission.SubmissionStatus = DataContracts.SubmissionStatus.Unsubmitted;
                            }
                        }
                    }
                    var submissionHist = GetSubmissions(submission.EnrollmentId, submission.ItemId);
                    var recentSub = submissionHist.Where(s => s.Version == submissionHist.Max(v => v.Version)).FirstOrDefault();
                    if (recentSub != null && recentSub.SubmittedDate != null)
                    {
                        submission.SubmittedDate = recentSub.SubmittedDate;
                    }
                }

                return pxSubmissions;
            }
        }

        #endregion
    }
}
