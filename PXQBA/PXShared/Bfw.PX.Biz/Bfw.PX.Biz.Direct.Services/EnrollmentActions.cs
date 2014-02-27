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

namespace Bfw.PX.Biz.Direct.Services
{
    /// <summary>
    /// Implements IEnrollmentActions using direct connection to DLAP.
    /// </summary>
    public class EnrollmentActions : IEnrollmentActions
    {
        #region Properties

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected IBusinessContext Context { get; set; }

        /// <summary>
        /// Gets or sets the session manager.
        /// </summary>
        /// <value>
        /// The session manager.
        /// </value>
        protected ISessionManager SessionManager { get; set; }

        /// <summary>
        /// Gets or sets the note actions.
        /// </summary>
        /// <value>
        /// The note actions.
        /// </value>
        protected INoteActions NoteActions { get; set; }

        /// <summary>
        /// Gets or sets the user actions.
        /// </summary>
        /// <value>
        /// The user actions.
        /// </value>
        protected IUserActions UserActions { get; set; }

        /// <summary>
        /// Gets or sets the content actions.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected IContentActions ContentActions { get; set; }

        /// <summary>
        /// Gets or sets the enrollment settings path.
        /// </summary>
        protected string EnrollmentSettingsPath { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EnrollmentActions"/> class.
        /// Sets the default enrollment settings path naming convention based off current entity and enrollment ID.
        /// </summary>
        /// <param name="ctx">The IBusinessContext implementation.</param>
        /// <param name="sessionManager">The session manager.</param>
        /// <param name="noteActions">The INoteActions implementation.</param>
        /// <param name="userActions">The IUserActions implementation.</param>
        /// <param name="contentActions">The IContentActions implementation.</param>
        public EnrollmentActions(IBusinessContext ctx, ISessionManager sessionManager, INoteActions noteActions, IUserActions userActions, IContentActions contentActions)
        {
            Context = ctx;
            SessionManager = sessionManager;
            NoteActions = noteActions;
            UserActions = userActions;
            ContentActions = contentActions;
            EnrollmentSettingsPath = string.Format("Templates/Data/XmlResources/Settings/Enrollment/{0}/{1}.pxres", Context.EntityId, Context.EnrollmentId);
        }

        #endregion

        #region IEnrollmentActions Members

        /// <summary>
        /// Sets the enrollment of the user specified to inactive
        /// </summary>
        /// <param name="enrollment">enrollment object that will be deactivated</param>
        public void InactiveEnrollment(Bdc.Enrollment enrollment)
        {
            using (Context.Tracer.DoTrace("EnrollmentActions.InactiveEnrollment(enrollmentId)"))
            {
                var enrollments = new List<Adc.Enrollment>();
                enrollments.Add(new Enrollment()
                {
                    Id = enrollment.Id,
                    Status = ((int)EnrollmentStatus.Inactive).ToString(),
                    EndDate = enrollment.EndDate,
                    StartDate = enrollment.StartDate
                });

                var cmd = new UpdateEnrollments();
                cmd.Enrollments = enrollments;

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            }
        }

        /// <summary>
        /// Updates the enrollment of the user with new attributes
        /// </summary>
        /// <param name="enrollment">enrollment object that will be updated</param>
        /// <returns>Success or failure as true / false</returns>
        public bool UpdateEnrollment(Bdc.Enrollment enrollment)
        {
            using (Context.Tracer.DoTrace("EnrollmentActions.UpdateEnrollment(enrollment)"))
            {
                var enrollments = new List<Adc.Enrollment>();
                Bfw.Agilix.DataContracts.AgilixUser user = null;
                Bfw.Agilix.DataContracts.Domain domain = null;
                if (enrollment.User != null)
                {
                    user = new Bfw.Agilix.DataContracts.AgilixUser();
                    user.Id = enrollment.User.Id;
                    user.FirstName = enrollment.User.FirstName;
                    user.LastName = enrollment.User.LastName;
                    user.Reference = enrollment.User.ReferenceId;
                    user.UserName = enrollment.User.Username;
                    user.Email = enrollment.User.Email;

                    if (!enrollment.User.DomainId.IsNullOrEmpty() && !enrollment.User.DomainName.IsNullOrEmpty())
                    {
                        domain = new Bfw.Agilix.DataContracts.Domain();
                        domain.Id = enrollment.User.DomainId;
                        domain.Name = enrollment.User.DomainName;
                    }
                }

                enrollments.Add(new Enrollment()
                {
                    Id = enrollment.Id,
                    User = user,
                    CourseId = enrollment.CourseId,
                    Status = enrollment.Status,
                    EndDate = enrollment.EndDate,
                    StartDate = enrollment.StartDate,
                    Reference = enrollment.Reference,
                    Domain = domain
                });

                var cmd = new UpdateEnrollments();
                cmd.Enrollments = enrollments;

                try
                {
                    cmd.ParseResponse(SessionManager.CurrentSession.Send(cmd.ToRequest(), asAdmin: true));
                    Context.CacheProvider.InvalidateUserEnrollmentList(Context.CurrentUser.Username);
                    var courseId = Context.CourseIsProductCourse ? Context.CourseId : Context.ProductCourseId;
                    Context.CacheProvider.InvalidateUserEnrollmentList(Context.CurrentUser.Username, courseId);
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Updates a list of enrollment
        /// </summary>
        /// <param name="enrollments"></param>
        /// <returns>Success or failure as true / false</returns>
        public bool UpdateEnrollments(IEnumerable<Bdc.Enrollment> enrollments)
        {
            using (Context.Tracer.DoTrace("EnrollmentActions.UpdateEnrollments(enrollments)"))
            {
                var dlapEnrollments = new List<Adc.Enrollment>();
                Bfw.Agilix.DataContracts.AgilixUser user = null;
                Bfw.Agilix.DataContracts.Domain domain = null;
                foreach (var enrollment in enrollments)
                {
                    if (enrollment.User != null)
                    {
                        user = new Bfw.Agilix.DataContracts.AgilixUser();
                        user.Id = enrollment.User.Id;
                        user.FirstName = enrollment.User.FirstName;
                        user.LastName = enrollment.User.LastName;
                        user.Reference = enrollment.User.ReferenceId;
                        user.UserName = enrollment.User.Username;
                        user.Email = enrollment.User.Email;

                        if (!enrollment.User.DomainId.IsNullOrEmpty() && !enrollment.User.DomainName.IsNullOrEmpty())
                        {
                            domain = new Bfw.Agilix.DataContracts.Domain();
                            domain.Id = enrollment.User.DomainId;
                            domain.Name = enrollment.User.DomainName;
                        }
                    }

                    dlapEnrollments.Add(new Enrollment()
                    {
                        Id = enrollment.Id,
                        User = user,
                        CourseId = enrollment.CourseId,
                        Status = enrollment.Status,
                        EndDate = enrollment.EndDate,
                        StartDate = enrollment.StartDate,
                        Reference = enrollment.Reference,
                        Domain = domain
                    });
                }
                var cmd = new UpdateEnrollments();
                cmd.Enrollments = dlapEnrollments;

                try
                {
                    cmd.ParseResponse(SessionManager.CurrentSession.Send(cmd.ToRequest(), asAdmin: true));
                    Context.CacheProvider.InvalidateUserEnrollmentList(Context.CurrentUser.Username);
                    var courseId = Context.CourseIsProductCourse ? Context.CourseId : Context.ProductCourseId;
                    Context.CacheProvider.InvalidateUserEnrollmentList(Context.CurrentUser.Username, courseId);
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        public List<Bdc.Enrollment> LoadEnrollmentCourses(List<Bdc.Enrollment> enrollments, bool getEnrollmentCount = false)
        {
            using (Context.Tracer.DoTrace("EnrollmentActions.LoadEnrollmentCourses"))
            {
                if (enrollments != null)
                {
                    var batch = new Batch();
                    batch.RunAsync = false;

                    var courseIds = enrollments.Select(e => e.Course.Id).Distinct();
                    //build up your batch call
                    foreach (var courseId in courseIds)
                    {
                        GetCourse cmdGetCourse = new GetCourse()
                        {
                            SearchParameters = new Bfw.Agilix.DataContracts.CourseSearch()
                            {
                                CourseId = courseId
                            }
                        };

                        batch.Add(courseId, cmdGetCourse);
                    }

                    if (!batch.Commands.IsNullOrEmpty())
                    {
                        SessionManager.CurrentSession.ExecuteAsAdmin(batch);

                        foreach (Bdc.Enrollment enrollment in enrollments)
                        {
                            string courseId = enrollment.Course.Id;
                            var cmdCourse = batch.CommandAs<GetCourse>(courseId);
                            if (!cmdCourse.Courses.IsNullOrEmpty())
                            {
                                enrollment.Course = cmdCourse.Courses.First().ToCourse();
                            }
                        }

                        if (getEnrollmentCount)
                        {
                            var courses = enrollments.Map(e => e.Course);
                            GetStudentsEnrolledCount(courses);
                        }
                    }

                }
            }

            return enrollments;
        }

        /// <summary>
        /// Gets all stuident enrollments for a course
        /// </summary>
        /// <param name="course">Bdc.Course</param>
        /// <returns>int</returns>
        private void GetStudentsEnrolledCount(IEnumerable<Bdc.Course> courses)
        {
            using (Context.Tracer.DoTrace("EportfolioCourseActions.GetStudentsEnrolledCount(eportfolioCourses(eportfolioCourses)) eportfolioCourses.Id={0}",
                courses.Select(c => c.Id).Reduce((a, b) => a + "," + b, "")))
            {
                var batch = new Batch();
                foreach (var course in courses)
                {
                    Adc.EntitySearch searchParameters = new Adc.EntitySearch()
                    {
                        CourseId = course.Id
                    };

                    var cmd = new GetEntityEnrollmentList()
                    {
                        SearchParameters = searchParameters
                    };
                    batch.Add(cmd);
                }

                SessionManager.CurrentSession.ExecuteAsAdmin(batch);

                foreach (var dlapCommand in batch.Commands)
                {
                    var cmd = dlapCommand as GetEntityEnrollmentList;
                    var course = courses.FirstOrDefault(c => c.Id == cmd.SearchParameters.CourseId);
                    course.StudentEnrollmentCount = cmd.Enrollments.Map(i => i.ToEnrollment()).Where(e => e.Flags.Contains("Participate")).Count();
                }

            }
        }

        /// <summary>
        /// Gets a list of user enrollments from agilix based on the External/Reference ID across domains.
        /// </summary>
        /// <param name="referenceId"></param>
        /// <returns>List of Enrollment objects.</returns>
        public List<Bdc.Enrollment> ListEnrollments(string referenceId)
        {
            using (Context.Tracer.DoTrace(string.Format("EnrollmentActions.ListEnrollments({0})", referenceId)))
            {
                List<Bdc.Enrollment> enrollments = new List<Bdc.Enrollment>();
                enrollments = Context.CacheProvider.FetchUserEnrollmentList(Context.CurrentUser.Username) as List<Bdc.Enrollment>;
                if (enrollments.IsNullOrEmpty())
                {
                    enrollments = new List<Bdc.Enrollment>();
                    List<Bdc.UserInfo> userInfoList = GetUserInfoFromAllDomains(referenceId);

                    if (!userInfoList.IsNullOrEmpty())
                    {
                        Batch cmdEnrollmentBatch = new Batch();
                        userInfoList = userInfoList.GroupBy(h => h.Id).Select(grp => grp.First()).ToList();

                        cmdEnrollmentBatch.RunAsync = userInfoList.Count > 1;


                        foreach (Bdc.UserInfo ui in userInfoList)
                        {
                            //get the enrollments
                            var getEnrollmentsCmd = new GetUserEnrollmentList()
                            {
                                SearchParameters = new Adc.EntitySearch()
                                {
                                    UserId = ui.Id
                                }
                            };

                            cmdEnrollmentBatch.Add(ui.Id, getEnrollmentsCmd);
                        }

                        if (cmdEnrollmentBatch.Commands.Count() > 0)
                        {
                            SessionManager.CurrentSession.ExecuteAsAdmin(cmdEnrollmentBatch);
                        }

                        foreach (Bdc.UserInfo ui in userInfoList)
                        {
                            var userEnrollments = cmdEnrollmentBatch.CommandAs<GetUserEnrollmentList>(ui.Id).Enrollments;
                            if (!userEnrollments.IsNullOrEmpty())
                            {
                                enrollments.AddRange(userEnrollments.Map(e => e.ToEnrollment()));
                            }
                        }
                    }

                    if (!enrollments.IsNullOrEmpty())
                    {
                        LoadEnrollmentCourses(enrollments);
                    }

                    Context.CacheProvider.StoreUserEnrollmentList(enrollments, Context.CurrentUser.Username);
                }

                return enrollments;
            }
        }

        /// <summary>
        /// Gets a list of user enrollments from agilix that matches the query and reference id.
        /// The result is not cached.
        /// </summary>
        /// <param name="referenceId"></param>
        /// <returns>List of Enrollment objects.</returns>
        public List<Bdc.Enrollment> ListEnrollmentsMatchQuery(string referenceId, string query)
        {
            using (Context.Tracer.DoTrace(string.Format("EnrollmentActions.ListEnrollmentsMatchQuery(referenceId = {0}, query = {1})", referenceId, query)))
            {
                List<Bdc.Enrollment> enrollments = new List<Bdc.Enrollment>();
                List<Bdc.UserInfo> userInfoList = GetUserInfoFromAllDomains(referenceId);

                if (!userInfoList.IsNullOrEmpty())
                {
                    enrollments = ListUsersEnrollments(userInfoList.Select(u => u.Id), "", query);
                }

                return enrollments;
            }
        }

        /// <summary>
        /// Gets a list of enrollments from agilix that matches the query and user ids.
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="query">query used to filter result</param>
        /// <returns></returns>
        public List<Bdc.Enrollment> ListUsersEnrollments(IEnumerable<string> userIds, string flags, string query)
        {
            var enrollments = new List<Bdc.Enrollment>();
            Batch cmdEnrollmentBatch = new Batch();

            cmdEnrollmentBatch.RunAsync = userIds.Count() > 1;

            foreach (string id in userIds)
            {
                //get the enrollments
                var getEnrollmentsCmd = new GetUserEnrollmentList()
                {
                    SearchParameters = string.IsNullOrEmpty(flags) ?
                                       new Adc.EntitySearch { UserId = id, Query = query } :
                                       new Adc.EntitySearch { UserId = id, Query = query, Flags = flags }
                };

                cmdEnrollmentBatch.Add(id, getEnrollmentsCmd);
            }

            if (cmdEnrollmentBatch.Commands.Count() > 0)
            {
                SessionManager.CurrentSession.ExecuteAsAdmin(cmdEnrollmentBatch);
            }

            foreach (string id in userIds)
            {
                var userEnrollments = cmdEnrollmentBatch.CommandAs<GetUserEnrollmentList>(id).Enrollments;
                if (!userEnrollments.IsNullOrEmpty())
                {
                    enrollments.AddRange(userEnrollments.Map(e => e.ToEnrollment()));
                }
            }
            return enrollments;
        }

        public List<Bdc.Enrollment> ListEnrollments(string referenceId, string productCourseId, bool loadCourses = true, bool getEnrollmentCount = false)
        {
            var enrollments = Context.CacheProvider.FetchUserEnrollmentList(referenceId, productCourseId, loadCourses, getEnrollmentCount);

            if (enrollments.IsNullOrEmpty())
            {
                enrollments = new List<Bdc.Enrollment>();
                Batch batch = new Batch();

                var agilixUsers = Context.CacheProvider.FetchUsersByReference(referenceId);
                List<Bdc.UserInfo> userInfoList = new List<Bdc.UserInfo>();

                if (agilixUsers != null)
                {
                    userInfoList = agilixUsers.Map(au => au.ToUserInfo()).ToList();
                }
                else
                {
                    Bdc.UserInfo user = new Bdc.UserInfo();
                    user.Username = referenceId;
                    userInfoList = UserActions.ListUsersLike(user);
                    if (userInfoList != null)
                    {
                        Context.CacheProvider.StoreUsersByReference(referenceId,
                            userInfoList.Map(u => u.ToUserInfo()).ToList());
                    }
                }

               
                if (!userInfoList.IsNullOrEmpty())
                {
                    if (userInfoList.Count > 1)
                    {
                        batch.RunAsync = true;
                    }

                    foreach (Bdc.UserInfo userInfo in userInfoList)
                    {
                        batch.Add(new GetUserEnrollmentList()
                        {
                            SearchParameters = new Bfw.Agilix.DataContracts.EntitySearch()
                            {
                                UserId = userInfo.Id,
                                Query = "/meta-product-course-id=" + productCourseId
                            }
                        });
                    }

                    SessionManager.CurrentSession.ExecuteAsAdmin(batch);

                    IEnumerable<Bdc.Enrollment> cmdEnrollments = null;
                    GetUserEnrollmentList entityEnrollmentCmd = null;
                    foreach (var cmd in batch.Commands)
                    {
                        entityEnrollmentCmd = cmd as GetUserEnrollmentList;
                        cmdEnrollments = entityEnrollmentCmd.Enrollments.Map(e => e.ToEnrollment());
                        enrollments.AddRange(cmdEnrollments);
                    }

                    if (!enrollments.IsNullOrEmpty() && loadCourses)
                    {
                        LoadEnrollmentCourses(enrollments, getEnrollmentCount);
                    }
                    Context.CacheProvider.StoreUserEnrollmentList(enrollments, referenceId, productCourseId, loadCourses, getEnrollmentCount);
                }
            }

            return enrollments;
        }

        /// <summary>
        /// Gets a collection of enrollments for the requested entity/course ID.
        /// </summary>
        /// <param name="entityId">ID of the course or section for which to get the enrollment list.</param>
        /// <returns></returns>
        public IEnumerable<Bdc.Enrollment> GetEntityEnrollments(string entityId)
        {
            IEnumerable<Bdc.Enrollment> entityEnrollmentList = null;

            using (Context.Tracer.DoTrace("EnrollmentActions.GetEntityEnrollments(entityId={0})", entityId))
            {
                Adc.EntitySearch searchParameters = new Adc.EntitySearch() { CourseId = entityId };

                var cmd = new GetEntityEnrollmentList()
                {
                    SearchParameters = searchParameters
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

                if (cmd.Enrollments != null)
                {
                    entityEnrollmentList = cmd.Enrollments.Map(e => e.ToEnrollment());
                }
            }

            return entityEnrollmentList;
        }

        /// <summary>
        /// Gets a collection of enrollments with grades for the requested entity/course ID.
        /// </summary>
        /// <param name="entityId">ID of the course or section for which to get the enrollment list.</param>
        /// <returns></returns>
        public IEnumerable<DataContracts.Enrollment> GetEntityEnrollmentsWithGrades(string entityId)
        {
            var result = new List<DataContracts.Enrollment>();

            using (Context.Tracer.DoTrace("EntrollmentActions.GetEntityEnrollmentsWithGrades(entityId={0})", entityId))
            {
                var cmd = new GetGrades()
                {
                    SearchParameters = new Adc.GradeSearch()
                    {
                        EntityId = entityId
                    }
                };

                SessionManager.CurrentSession.Execute(cmd);

                if (!cmd.Enrollments.IsNullOrEmpty())
                {
                    result = cmd.Enrollments.Map(o => o.ToEnrollment()).ToList();
                }
            }

            return result;
        }

        public List<Bdc.Enrollment> SwitchUserEntityEnrollment(string oldUserId, string courseId, Bdc.UserInfo userInfo)
        {
            List<Bdc.Enrollment> result = new List<Bdc.Enrollment>();

            var originalEnrollments = GetEntityEnrollments(courseId);

            if (!originalEnrollments.IsNullOrEmpty())
            {
                foreach (Bdc.Enrollment e in originalEnrollments)
                {
                    if (e.User.Id == oldUserId)
                    {
                        e.User = userInfo;
                        UpdateEnrollment(e);
                        result.Add(e);
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// Gets the list of enrollments in the specified entity filtered by user type.
        /// </summary>
        /// <param name="entityId">ID of the course or section for which to get the enrollment list.</param>
        /// <param name="userType">Type of the user.</param>
        /// <returns>A list of enrollments.</returns>
        public IEnumerable<Bdc.Enrollment> GetEntityEnrollments(string entityId, Bdc.UserType userType)
        {
            IEnumerable<Bdc.Enrollment> result = null;
            using (Context.Tracer.DoTrace("EnrollmentActions.GetEntityEnrollments(entityId={0},userType={1})", entityId, userType))
            {
                var role = new Dictionary<Bdc.UserType, string>
                {
                { Bdc.UserType.Student, "Participate" },
                { Bdc.UserType.Instructor, "SubmitFinalGrade" },
                { Bdc.UserType.All, "" }
                }[userType];

                result = GetEntityEnrollmentsAsAdmin(entityId);
                if (result != null)
                {
                    result = result.Filter(e => e.Flags.Contains(role));
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the list of users enrolled in the specified entity via an Admin login.
        /// </summary>
        /// <param name="entityId">ID of the course or section for which to get the enrollment list.</param>
        /// <returns>List of Enrollments.</returns>
        public List<Bdc.Enrollment> GetEntityEnrollmentsAsAdmin(string entityId)
        {
            List<Bdc.Enrollment> entityEnrollmentList = null;

            using (Context.Tracer.DoTrace("EnrollmentActions.GetEntityEnrollmentsAsAdmin(entityId={0})", entityId))
            {
                Adc.EntitySearch searchParameters = new Adc.EntitySearch() { CourseId = entityId };

                var cmd = new GetEntityEnrollmentList()
                {
                    SearchParameters = searchParameters
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

                if (!cmd.Enrollments.IsNullOrEmpty())
                {
                    entityEnrollmentList = cmd.Enrollments.Map(e => e.ToEnrollment()).ToList();
                }
            }

            return entityEnrollmentList;
        }

        /// <summary>
        /// Gets the list of all users enrolled in the specified entity via an Admin login.
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public List<Bdc.Enrollment> GetAllEntityEnrollmentsAsAdmin(string entityId)
        {
            List<Bdc.Enrollment> entityEnrollmentList = null;

            using (Context.Tracer.DoTrace("EnrollmentActions.GetAllEntityEnrollmentsAsAdmin(entityId={0})", entityId))
            {
                Adc.EntitySearch searchParameters = new Adc.EntitySearch() { CourseId = entityId, AllStatus = true };

                var cmd = new GetEntityEnrollmentList()
                {
                    SearchParameters = searchParameters
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

                if (!cmd.Enrollments.IsNullOrEmpty())
                {
                    entityEnrollmentList = cmd.Enrollments.Map(e => e.ToEnrollment()).ToList();
                }
            }

            return entityEnrollmentList;
        }



        /// <summary>
        /// Gets the list of users enrolled in the specified entities via an Admin login.
        /// </summary>
        /// <param name="entityIds">List of course IDs</param>
        /// <param name="userType">User type</param>
        /// <returns></returns>
        public List<Bdc.Enrollment> GetEnrollmentsBatch(List<string> entityIds, Bdc.UserType userType = Bdc.UserType.All)
        {
            List<Bdc.Enrollment> result = null;
            if (!entityIds.IsNullOrEmpty())
            {
                using (Context.Tracer.DoTrace("EnrollmentActions.GetEnrollmentsBatch()"))
                {
                    Batch batch = new Batch { RunAsync = true };
                    foreach (var entityId in entityIds)
                    {
                        batch.Add(new GetEntityEnrollmentList()
                        {
                            SearchParameters = new Adc.EntitySearch()
                            {
                                CourseId = entityId
                            }
                        });
                    }
                    if (!batch.Commands.IsNullOrEmpty())
                    {
                        SessionManager.CurrentSession.ExecuteAsAdmin(batch);
                        var entityEnrollmentList = new List<Bdc.Enrollment>();
                        for (int i = 0; i < batch.Commands.Count(); i++)
                        {
                            var cmd = batch.CommandAs<GetEntityEnrollmentList>(i);
                            entityEnrollmentList.AddRange(batch.CommandAs<GetEntityEnrollmentList>(i).Enrollments.Map(e => e.ToEnrollment()).ToList());
                        }
                        var role = new Dictionary<Bdc.UserType, string>
                                                        {
                                                            { Bdc.UserType.Student, "Participate" },
                                                            { Bdc.UserType.Instructor, "SubmitFinalGrade" },
                                                            { Bdc.UserType.All, "" }
                                                        }[userType];
                        result = entityEnrollmentList.Filter(e => e.Flags.Contains(role)).ToList();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets an enrollment record.
        /// </summary>
        /// <param name="enrollmentId">enrollment id</param>
        /// <returns></returns>
        public Bdc.Enrollment GetEnrollment(string enrollmentId)
        {

            using (Context.Tracer.DoTrace("EnrollmentActions.GetEnrollment(enrollmentId={0})", enrollmentId))
            {
                var cmd = new GetEnrollment()
                {
                    EnrollmentId = enrollmentId
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

                if (!cmd.Enrollments.IsNullOrEmpty())
                {
                    return cmd.Enrollments.First().ToEnrollment();
                }
                else
                {
                    return new Bdc.Enrollment();
                }

            }

        }

        /// <summary>
        /// Gets an enrollment record.
        /// </summary>
        /// <param name="userId">ID of the user for which to get the enrollment.</param>
        /// <param name="entityId">ID of the course or section.</param>
        /// <returns>An enrollment record for the specified user and entity ID.</returns>
        public Bdc.Enrollment GetEnrollment(string userId, string entityId)
        {
            Bdc.Enrollment enrolled = null;

            using (Context.Tracer.DoTrace("EnrollmentActions.GetEnrollment(userId={0}, entityId={1})", userId, entityId))
            {
                enrolled = Context.CacheProvider.FetchEnrollment(userId, entityId);
                if (enrolled != null)
                {
                    Context.Logger.Log(string.Format("Enrollment loaded from cache - User:{0} - Course:{1}", userId, entityId), LogSeverity.Debug);
                }
                else
                {
                    IEnumerable<Bdc.Enrollment> entityEnrollments = new List<Bdc.Enrollment>();
                    entityEnrollments = GetEntityEnrollmentsAsAdmin(entityId);
                    if (entityEnrollments != null && entityEnrollments.Count() > 0)
                    {
                        entityEnrollments = entityEnrollments.Filter(e => e.User.Id == userId);
                        if (entityEnrollments != null && entityEnrollments.Count() > 0)
                        {
                            enrolled = entityEnrollments.First();
                            Context.CacheProvider.StoreEnrollment(enrolled);
                        }
                    }
                }
            }

            return enrolled;
        }


        /// <summary>
        ///  Get the inactive enrollment for the course.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="entityId"></param>
        /// <returns>An enrollment record for the specified user and entity ID.</returns>
        public Bdc.Enrollment GetInactiveEnrollment(string userId, string entityId)
        {
            Bdc.Enrollment enrolled = null;

            using (Context.Tracer.DoTrace("EnrollmentActions.GetInactiveEnrollment(userId={0}, entityId={1})", userId, entityId))
            {
                enrolled = Context.CacheProvider.FetchEnrollment(userId, entityId);
                if (enrolled != null)
                {
                    Context.Logger.Log(string.Format("Enrollment loaded from cache - User:{0} - Course:{1}", userId, entityId), LogSeverity.Debug);
                }
                else
                {
                    IEnumerable<Bdc.Enrollment> entityEnrollments = new List<Bdc.Enrollment>();
                    entityEnrollments = GetAllEntityEnrollmentsAsAdmin(entityId);
                    if (entityEnrollments != null && entityEnrollments.Count() > 0)
                    {
                        entityEnrollments = entityEnrollments.Filter(e => e.User.Id == userId);
                        if (entityEnrollments != null && entityEnrollments.Count() > 0)
                        {
                            enrolled = entityEnrollments.First();
                            Context.CacheProvider.StoreEnrollment(enrolled);
                        }
                    }
                }
            }

            return enrolled;
        }


        /// <summary>
        /// Creates the enrollments.
        /// See http://gls.agilix.com/Docs/Command/CreateEnrollments.
        /// </summary>
        /// <param name="domainId">The ID of the domain to create the enrollment in.</param>
        /// <param name="userId">The ID of the user to enroll.</param>
        /// <param name="entityId">The ID of the course or section in which to enroll the user.</param>
        /// <param name="flags">Bitwise OR of RightsFlags to grant to the user.</param>
        /// <param name="status">EnrollmentStatus for the user.</param>
        /// <param name="startDate">Date that the enrollment begins.</param>
        /// <param name="endDate">Date that the enrollment ends.</param>
        /// <param name="reference">Optional field reserved for any data the caller wishes to store. Used to store the RA ID.</param>
        /// <param name="schema">An optional parameter that specifies how to interpret flags in agilix.</param>
        /// <param name="disallowduplicates">This will disallow to create duplicate enrollment</param>
        /// <returns></returns>
        public List<Bdc.Enrollment> CreateEnrollments(string domainId, string userId, string entityId, string flags, string status, DateTime startDate, DateTime endDate, string reference, string schema, bool disallowduplicates = false)
        {
            var cmd = new CreateEnrollment();
            cmd.Disallowduplicates = disallowduplicates;
            List<Bdc.Enrollment> entityEnrollmentList = null;

            String traceMessage = string.Format("EnrollmentActions.CreateEnrollments(domainId={0}, userId={1}, entityId={2}, flags={3}, status={4}, startDate={5}, endDate={6}, reference={7})",
                domainId,
                userId,
                entityId,
                flags,
                status,
                startDate,
                endDate,
                reference,
                schema);

            using (Context.Tracer.StartTrace(traceMessage))
            {
                var agxEnrollments = new List<Adc.Enrollment>();
                cmd.Add(new Adc.Enrollment()
                {
                    Domain = new Adc.Domain() { Id = domainId },
                    User = new Adc.AgilixUser() { Id = userId },
                    Course = new Adc.Course() { Id = entityId },
                    Flags = (DlapRights)Enum.Parse(typeof(DlapRights), flags),
                    Status = status,
                    StartDate = startDate,
                    EndDate = endDate,
                    Reference = reference,
                    Schema = schema
                });

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

                if (!cmd.Enrollments.IsNullOrEmpty())
                {
                    entityEnrollmentList = cmd.Enrollments.Map(e => e.ToEnrollment()).ToList();
                }

                var fbits = long.Parse(flags);
                bool isInstructor = (fbits & 0x8000000000) > 0;
                var type = Bdc.UserType.Student;

                if (isInstructor)
                    type = Bdc.UserType.Instructor;

                var user = UserActions.GetUser(userId);
                NoteActions.InitializeUser(user, entityId, type);

                var productCourseId = Context.CourseIsProductCourse ? Context.CourseId : Context.ProductCourseId;
                Context.CacheProvider.InvalidateUserEnrollmentList(Context.CurrentUser.Username, productCourseId);
                //we also need to invalidate the instructor enrollments so that the number of enrollments is incremented on the dashboard
                if (Context.Course.CourseOwner != null && Context.Course.CourseOwner != Context.CurrentUser.Username)
                {
                    var instructor = UserActions.GetUser(Context.Course.CourseOwner);
                    if (instructor != null)
                    {
                        Context.CacheProvider.InvalidateUserEnrollmentList(instructor.Username, productCourseId);
                    }
                }
            }

            return entityEnrollmentList;
        }

        /// <summary>
        /// Gets a user enrollment ID.
        /// </summary>
        /// <param name="userId">ID of the user for which to get the enrollment ID.</param>
        /// <param name="entityId">ID of the course or section.</param>
        /// <returns></returns>
        public String GetUserEnrollmentId(string userId, string entityId, bool allStatus = false)
        {
            string agxEnrollmentId = string.Empty;

            using (Context.Tracer.DoTrace("EnrollmentActions.GetUserEnrollmentId(userId={0}, entityId={1})", userId, entityId))
            {
                Bdc.Enrollment enrolled = null;

                enrolled = Context.CacheProvider.FetchEnrollment(userId, entityId);
                if (enrolled != null)
                {
                    Context.Logger.Log(string.Format("Enrollment loaded from cache - user:{0} - course{1}", userId, entityId), LogSeverity.Debug);
                }
                else
                {
                    Adc.EntitySearch searchParameters = new Adc.EntitySearch() { CourseId = entityId, UserId = userId };

                    var cmd = new GetUserEnrollmentList()
                    {
                        AllStatus = allStatus,
                        SearchParameters = searchParameters
                    };

                    SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                    enrolled = cmd.Enrollments.FirstOrDefault().ToEnrollment();
                    Context.CacheProvider.StoreEnrollment(enrolled);
                }

                agxEnrollmentId = (enrolled != null) ? enrolled.Id : string.Empty;
            }

            return agxEnrollmentId;
        }

        /// <summary>
        /// Gets the enrollment settings resource file for the current user.
        /// Enrollment file is currently stored as a resource using the following naming/path convention:
        /// Templates/Data/XmlResources/Settings/Enrollment/{EntityId}/{EnrollmentId}.pxres
        /// </summary>
        [Obsolete("This method depends on an IContentActions object.", false)]
        public Bdc.Resource GetEnrollmentSettings()
        {
            Bdc.Resource result = null;
            using (Context.Tracer.StartTrace("EnrollmentActions.GetEnrollmentSettings"))
            {
                var settingsFile = ContentActions.ListResources(Context.EnrollmentId, EnrollmentSettingsPath, "");

                if (!settingsFile.Any())
                {
                    var xDoc = new XmlDocument();
                    XmlNode xRootNode = xDoc.CreateNode(XmlNodeType.Element, "EnrollmentSettings", "");
                    xDoc.AppendChild(xRootNode);
                    var resource = new Bdc.XmlResource
                    {
                        Status = Bdc.ResourceStatus.Normal,
                        Url = EnrollmentSettingsPath,
                        EntityId = Context.EnrollmentId,
                        Title = "EnrollmentSettings_" + Context.EnrollmentId,
                        Body = xDoc.InnerXml
                    };

                    ContentActions.StoreResources(new List<Bdc.Resource> { resource });
                    settingsFile = ContentActions.ListResources(Context.EnrollmentId, EnrollmentSettingsPath, "");
                }
                result = settingsFile.First();
            }
            return result;
        }

        /// <summary>
        /// Saves the enrollment settings resource file in the system for the current user.
        /// </summary>
        /// <param name="settingsFile">The settings resource file to save.</param>
        public void SaveEnrollmentSettings(Bdc.Resource settingsFile)
        {
            using (Context.Tracer.StartTrace("EnrollmentActions.SaveEnrollmentSettings(settingsFile)"))
            {
                ContentActions.StoreResources(new List<Bdc.Resource> { settingsFile });
            }
        }

        /// <summary>
        /// Retruns a list of all domains that a user can enroll in.
        /// </summary>
        /// <param name="userReferenceId">Reference ID of the user that wants to be enrolled.</param>
        /// <param name="parentDomainId">ID of the parent domain to restring domain list to.</param>
        /// <param name="forceIfMember">If populated with a list of domain ids, then these domains will be in the result if the user is a member of the domain.</param>
        /// <returns>Distinct list of domains that the user may enroll in.</returns>
        public IEnumerable<Bdc.Domain> GetEnrollableDomains(string userReferenceId, string parentDomainId, IEnumerable<string> forceIfMember = null)
        {
            var results = Context.CacheProvider.FetchDomainList(parentDomainId);

            if (results == null)
            {
                //first get the list of all child domains.
                var getDomains = new GetDomainList()
                {
                    SearchParameters = new Adc.Domain()
                    {
                        Id = parentDomainId
                    }
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(getDomains);

                if (!getDomains.Domains.IsNullOrEmpty())
                {
                    //remove the "parent domain" as DLAP for some reason always returns it...
                    results = getDomains.Domains.Filter(d => d.Id != parentDomainId).Map(d => d.ToDomain()).ToList();
                    Context.CacheProvider.StoreDomainList(parentDomainId, results);
                }

            }

            if (!forceIfMember.IsNullOrEmpty())
            {
                //since we have a forceIfMember list we need to make sure we add them if necessary...
                var userDomains = Context.GetRaUserDomains();

                foreach (var domain in userDomains)
                {
                    if (forceIfMember.Contains(domain.Id))
                    {
                        results.Add(domain);
                    }
                }
            }

            results = results.Distinct((a, b) => a.Id == b.Id).OrderBy(d => d.Name).ToList();

            return results;
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Get user info from all domains
        /// </summary>
        /// <param name="referenceId">user reference id</param>
        /// <returns></returns>
        private List<Bdc.UserInfo> GetUserInfoFromAllDomains(string referenceId)
        {
            var agilixUsers = Context.CacheProvider.FetchUsersByReference(referenceId);

            List<Bdc.UserInfo> userInfoList = new List<Bdc.UserInfo>();

            if (agilixUsers != null)
            {
                userInfoList = agilixUsers.Map(au => au.ToUserInfo()).ToList();
                Context.Logger.Log(string.Format("Agilix users loaded from cache for user: {0}", referenceId), LogSeverity.Debug);
            }
            else
            {
                var user = new Bdc.UserInfo();
                user.Username = Context.CurrentUser.Username;
                userInfoList = UserActions.ListUsersLike(user);
            }
            return userInfoList;
        }

        #endregion Implementation
    }
}
