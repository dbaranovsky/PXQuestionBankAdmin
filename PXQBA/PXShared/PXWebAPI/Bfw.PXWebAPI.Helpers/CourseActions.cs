using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Xml.Linq;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Collections;
using Bfw.PXWebAPI.Mappers;
using Bfw.PXWebAPI.Models;
using Bfw.PXWebAPI.Models.DTO;
using Adc = Bfw.Agilix.DataContracts;
using Course = Bfw.PXWebAPI.Models.Course;

namespace Bfw.PXWebAPI.Helpers
{
    public class CourseActions
    {
        #region Properties

        protected ISessionManager SessionManager { get; set; }

        #endregion

        // <summary>
        /// A const for that holds the instructor persmission flags.
        /// </summary>
        private readonly DlapRights INSTRUCTOR_FLAGS = (DlapRights)Int64.Parse(System.Configuration.ConfigurationManager.AppSettings["InstructorPermissionFlags"]);

        /// <summary>
        /// A const for that holds the student persmission flags.
        /// </summary>
        private readonly DlapRights STUDENT_FLAGS = (DlapRights)Int64.Parse(System.Configuration.ConfigurationManager.AppSettings["StudentPermissionFlags"]);


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CourseActions"/> class.
        /// </summary>
        /// <param name="sessionManager">The session manager.</param>
        public CourseActions(ISessionManager sessionManager)
        {
            SessionManager = sessionManager;
        }
        #endregion

        /// <summary>
        /// Gets a course by course ID.
        /// </summary>
        /// <param name="courseId">The course ID.</param>
        /// <returns></returns>
        public Adc.Course GetCourseByCourseId(string courseId)
        {
            Adc.Course result = null;
            var cmd = new GetCourse()
                          {
                              SearchParameters = new Adc.CourseSearch()
                                                     {
                                                         CourseId = courseId
                                                     }
                          };

            SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            result = cmd.Courses.First();
            return result;
        }

        /// <summary>
        /// Gets a course by query.
        /// </summary>
        /// <param name="query">The course ID.</param>
        /// <returns></returns>
        public List<Adc.Course> GetCourseByQuery(string query, string userId = "")
        {
            List<Adc.Course> result = null;
            var cmd = new GetCourse()
            {
                SearchParameters = new Adc.CourseSearch()
                {


                    Query = query
                }
            };

            SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            result = cmd.Courses.ToList();
            return result;
        }

        public IEnumerable<Adc.Course> GetCourseListInformation(IEnumerable<Adc.Course> courses)
        {
            var list = new List<Adc.Course>();
            Batch batch = new Batch();
            foreach (Adc.Course c in courses)
            {
                batch.Add(new GetCourse()
                              {
                                  SearchParameters = new Adc.CourseSearch()
                                                         {
                                                             CourseId = c.Id
                                                         }
                              });
            }

            if (!batch.Commands.IsNullOrEmpty())
            {
                SessionManager.CurrentSession.ExecuteAsAdmin(batch);
            }

            for (int ord = 0; ord < batch.Commands.Count(); ord++)
            {
                if (batch.CommandAs<GetCourse>(ord).Courses != null)
                {
                    list.AddRange(batch.CommandAs<GetCourse>(ord).Courses);
                }
            }
            return list;
        }

        /// <summary>
        /// Creates a derived course.
        /// </summary>
        /// <param name="deriveFrom">The course to derive from.</param>
        /// <param name="newDomainId">The domain ID for the new course.</param>
        /// <param name="copyMethod">The method.</param>
        /// <param name="courseOwner">Course owner to enroll.</param>
        /// <returns></returns>
        public Course CreateDerivedCourse(Course deriveFrom, string newDomainId, string courseOwner, string copyMethod)
        {
            Course result = null;

            var copies = CopyCourses(new List<Course>() { deriveFrom }, newDomainId, copyMethod, courseOwner);
            if (copies.Count != 1)
            {
                throw new Exception(string.Format("Got wrong number of copies ({0} when there should be 1)", copies.Count));
            }
            result = GetCourseByCourseId(copies.First().Id).ToCourse();
            var productId = deriveFrom.Id;
            if (!string.IsNullOrEmpty(deriveFrom.ProductCourseId))
            {
                productId = deriveFrom.ProductCourseId;
            }
            result.ProductCourseId = productId;
            result.DashboardCourseId = deriveFrom.DashboardCourseId;
            result.Isbn13 = deriveFrom.Isbn13;
            result.CourseProductName = deriveFrom.Title;
            result.CourseOwner = courseOwner;
            result.CourseTemplate = productId;
            result.ProductCourseId = productId;

            //ToDo: Discuss this with PX team to decide who the instrcutor is going to be and if we should set the below values
            //result.ActivatedDate = DateTime.MaxValue.ToShortDateString();
            //result.InstructorName = content.CourseUserName;

            UpdateCourses(new List<Course>() { result });
            //return the result and not course because while updating, DLAP is not including all the properties like parent id 
            return result;
        }

        /// <summary>
        /// Updates a set of courses.
        /// </summary>
        /// <param name="courses">The courses.</param>
        /// <returns></returns>
        public List<Course> UpdateCourses(List<Course> courses)
        {
            List<Course> result = null;

            var cmd = new UpdateCourses();
            cmd.Add(courses.Map(c => c.ToCourse()).ToList());

            SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            result = cmd.Courses.Map(c => c.ToCourse()).ToList();
            return result;
        }

        /// <summary>
        /// Copies a set of courses to a new domain.
        /// </summary>
        /// <param name="courses">The courses.</param>
        /// <param name="newDomainId">The new domain ID.</param>
        /// <param name="method">The method.</param>
        /// <param name="courseOwner">Course owner to enroll.</param>
        /// <returns></returns>
        private List<Course> CopyCourses(List<Course> courses, string newDomainId, string method, string courseOwner)
        {
            List<Course> result = null;
            var cmd = new CopyCourses()
            {
                DomainId = newDomainId,
                Method = method
            };
            cmd.Add(courses.Map(c => c.ToCourse()).ToList());
            SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

            result = cmd.Courses.Map(c => c.ToCourse()).ToList();
            SetCourseEnrollments(result, courseOwner);
            return result;
        }



        /// <summary>
        /// Sets up the currently logged in user as the instructor. This fixes the problem were the admin
        /// user is set as the instructor. Also, set up the instructor's shadow student user.
        /// </summary>
        /// <param name="courses">The courses.</param>
        /// <param name="courseOwner">Owner of the course.</param>
        protected void SetCourseEnrollments(IEnumerable<Course> courses, string courseOwner)
        {

            var cmd = new CreateEnrollment();

            foreach (var course in courses)
            {
                cmd.Add(new Adc.Enrollment()
                {
                    User = new Adc.AgilixUser() { Id = courseOwner },
                    Course = course.ToCourse(),
                    Status = "1",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.MaxValue,
                    Flags = INSTRUCTOR_FLAGS
                });

                cmd.Add(new Adc.Enrollment()
                {
                    User = new Adc.AgilixUser() { Id = courseOwner },
                    Course = course.ToCourse(),
                    Status = "10",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.MaxValue,
                    Flags = STUDENT_FLAGS
                });


                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            }
        }

        public void CreateUserEnrollment(string userid, string courseid, string domainid,
                                               bool isinstructor = false, string enrollmentstatus = "",
                                               string startdate = "", string enddate = "")
        {

            var studentPermissionFlags = STUDENT_FLAGS.ToString();
            var instructorPermissionFlags = INSTRUCTOR_FLAGS.ToString();
            studentPermissionFlags = !string.IsNullOrEmpty(studentPermissionFlags) ? studentPermissionFlags : "131073";
            instructorPermissionFlags = !string.IsNullOrEmpty(instructorPermissionFlags)
                                            ? instructorPermissionFlags
                                            : "552155348992";
            var flags = isinstructor ? instructorPermissionFlags : studentPermissionFlags;
            var strStartDate = startdate == string.Empty
                                   ? DateTime.Now.ToString(CultureInfo.InvariantCulture)
                                   : startdate;
            var strEndDate = enddate == string.Empty
                                 ? DateTime.Now.AddYears(1).ToString(CultureInfo.InvariantCulture)
                                 : enddate;
            var strEnrollmentStatus = enrollmentstatus == string.Empty
                                          ? "1"
                                          : enrollmentstatus;

            var cmd = new CreateEnrollment();
            var crseEnrollment = new Adc.Enrollment
            {
                Domain = new Adc.Domain { Id = domainid },
                User = new Adc.AgilixUser { Id = userid },
                Course = new Adc.Course { Id = courseid },
                Flags = (DlapRights)Enum.Parse(typeof(DlapRights), flags),
                Status = strEnrollmentStatus,
                StartDate = Convert.ToDateTime(strStartDate),
                EndDate = Convert.ToDateTime(strEndDate),
                Schema = string.Empty,
                Reference = string.Empty
            };
            cmd.Add(crseEnrollment);
            SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
        }

        public List<Adc.Enrollment> GetEntityEnrollments(string userid, string courseid = "", string flags = "", string query = "")
        {
            var cmd = new GetEntityEnrollmentList()
            {
                SearchParameters = new EntitySearch()
                {
                    UserId = userid,
                    CourseId = courseid,
                    Flags = flags,
                    Query = query
                }
            };
            SessionManager.CurrentSession.Execute(cmd);
            var aenrollments = cmd.Enrollments;
            return aenrollments;
        }

        /// <summary>
        /// Gets the list of academic terms
        /// </summary>
        /// <param name="domainId">domain id</param>
        /// <returns></returns>
        public IEnumerable<CourseAcademicTerm> ListAcademicTerms(string domainId)
        {
            XDocument result;
            var contentActions = new ContentActions(SessionManager);
            var resource = contentActions.GetResource(domainId, "PX/academicterms.xml");

            if (resource != null && resource.ContentType != null)
            {
                result = XDocument.Load(resource.GetStream());
            }
            else
            {
                result = XDocument.Parse("<academic_terms/>");
            }

            foreach (var export in result.Root.Elements("academic_term"))
            {
                yield return export.ToCourseAcademicTerm();
            }
        }

        public IEnumerable<Course> GetCourseListInformation(IEnumerable<Course> courses)
        {
            var fullcourses = new List<Course>();
            Batch batch = new Batch();
            foreach (Course c in courses)
            {
                batch.Add(new GetCourse()
                              {
                                  SearchParameters = new Adc.CourseSearch()
                                                         {
                                                             CourseId = c.Id
                                                         }
                              });
            }

            if (!batch.Commands.IsNullOrEmpty())
            {
                SessionManager.CurrentSession.ExecuteAsAdmin(batch);
            }

            var list = new List<Adc.Course>();
            for (int ord = 0; ord < batch.Commands.Count(); ord++)
            {
                if (batch.CommandAs<GetCourse>(ord).Courses != null)
                {
                    list.AddRange(batch.CommandAs<GetCourse>(ord).Courses);
                }
            }

            if (!list.IsNullOrEmpty())
            {
                foreach (Adc.Course c in list)
                {
                    fullcourses.Add(c.ToCourse());
                }
            }
            return fullcourses;
        }
    }
}
