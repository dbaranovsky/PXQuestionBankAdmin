using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Collections;
using Bfw.PXWebAPI.Mappers;
using Adc = Bfw.Agilix.DataContracts;
using Bfw.PXWebAPI.Models;
using Bfw.PXWebAPI.Models.DTO;

namespace Bfw.PXWebAPI.Helpers
{
    public class UserActions
    {
        #region Properties
        protected ISessionManager SessionManager { get; set; }
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserActions"/> class.
        /// </summary>
        /// <param name="sessionManager">The session manager.</param>
        public UserActions(ISessionManager sessionManager)
        {
            SessionManager = sessionManager;
        }

        #endregion
        public List<Adc.AgilixUser> GetUsers(string rauserid = "", string domainid = "", string userid = "")
        {
            var cmd = new GetUsers
            {
                SearchParameters = new Adc.UserSearch
                {
                    ExternalId = rauserid,
                    DomainId = domainid,
                    Id = userid
                }
            };
            SessionManager.CurrentSession.Execute(cmd);
            var ausrs = cmd.Users;

            if (!ausrs.IsNullOrEmpty())
            {
                //Distinct is performed here temperorily to fix the issue of duplicate users in Dlap response(bug)
                ausrs = ausrs.GroupBy(i => i.Id, (key, group) => group.First()).ToList();
                return ausrs;
            }
            return new List<Adc.AgilixUser>();
        }

        public List<Adc.AgilixUser> GetUsersFromAllDomains(string id, bool byreference = false)
        {
            if (byreference)
                return GetUsers(rauserid: id);
            var users = GetUsers(userid: id);
            if (users != null && users.Any())
            {
                return GetUsers(rauserid: users.First().Reference);
            }
            return new List<Adc.AgilixUser>();
        }

        public List<Adc.Enrollment> GetUserEnrollments(string userid, string courseid = "")
        {
            var cmd = new GetUserEnrollmentList()
            {
                SearchParameters = new Adc.EntitySearch()
                {
                    UserId = userid,
                    CourseId = courseid
                }
            };
            SessionManager.CurrentSession.Execute(cmd);
            var aenrollments = cmd.Enrollments;
            return aenrollments;
        }

        public List<Adc.Enrollment> GetUserEnrollmentsByQueryAndFlags(string userid, string query, string flags)
        {
            var cmd = new GetUserEnrollmentList()
            {
                SearchParameters = new Adc.EntitySearch()
                {
                    UserId = userid,
                    Query = query,
                    Flags = flags

                }
            };
            SessionManager.CurrentSession.Execute(cmd);
            var aenrollments = cmd.Enrollments;
            return aenrollments;
        }



        public List<Adc.Enrollment> GetEnrollments(string id, bool byreference = false, string domainId = "")
        {
            if (!byreference)
                return GetUserEnrollments(userid: id);
            List<Adc.AgilixUser> users = GetUsers(rauserid: id, domainid: domainId);
            if (users == null || !users.Any())
            {
                return new List<Adc.Enrollment>();
            }
            var enrollmentList = new List<Adc.Enrollment>();
            foreach (var user in users)
            {
                var enrollments = GetUserEnrollments(user.Id);
                if (enrollments != null)
                    enrollmentList.AddRange(enrollments);
            }
            return enrollmentList;
        }

        public bool UpdateUsers(string id, bool byreference, EditUser userInput)
        {
            var users = GetUsersFromAllDomains(id, byreference);
            Adc.Credentials credentials = null;
            if (!string.IsNullOrEmpty(userInput.Email))
            {
                credentials = new Adc.Credentials() { Username = userInput.Email };
            }

            var cmd = new UpdateUsers();
            foreach (var result in users)
            {
                cmd.Add(new Adc.AgilixUser()
                {
                    Id = result.Id,
                    FirstName = userInput.FirstName,
                    LastName = userInput.LastName,
                    Email = userInput.Email,
                    Credentials = credentials,
                });
            }
            SessionManager.CurrentSession.Execute(cmd);
            return cmd.UpdateResult;
        }

        public IEnumerable<Adc.AgilixUser> ListInstructorsForDomain(String application_type, string domainId, string academicTerm)
        {
            var batch = new Batch();
            var instructors = new List<Adc.AgilixUser>();
            //get all the courses first for the domain/term selected
            string query = string.Empty;
            if (string.IsNullOrEmpty(application_type))
            {
                query = string.Format(@"/meta-bfw_academic_term='{0}'", academicTerm);
            }
            else
            {
                query = string.Format(@"/meta-bfw_academic_term='{0}' AND /meta-bfw_course_type = '{1}'",
                                    academicTerm, application_type);
            }
            var cmdCourses = new GetCourse()
                                 {
                                     SearchParameters = new Adc.CourseSearch()
                                                            {
                                                                DomainId = domainId,
                                                                Query = query
                                                            }
                                 };
            SessionManager.CurrentSession.ExecuteAsAdmin(cmdCourses);

            if (cmdCourses.Courses != null)
            {
                foreach (Adc.Course c in cmdCourses.Courses.Filter(c => c.Domain.Id == domainId))
                {
                    batch.Add(new GetEntityEnrollmentList()
                                  {
                                      SearchParameters = new Adc.EntitySearch()
                                                             {
                                                                 CourseId = c.Id
                                                             }
                                  });
                }
                if (!batch.Commands.IsNullOrEmpty())
                {
                    SessionManager.CurrentSession.ExecuteAsAdmin(batch);
                }
                var enrollmentUserId = new List<Adc.Enrollment>();
                for (int ord = 0; ord < batch.Commands.Count(); ord++)
                {
                    enrollmentUserId.AddRange(
                        batch.CommandAs<GetEntityEnrollmentList>(ord).Enrollments.Where(
                            u => (u.Flags.HasFlag(DlapRights.SubmitFinalGrade) && u.Domain.Id.Equals(domainId))));
                }

                if (!enrollmentUserId.IsNullOrEmpty())
                {
                    foreach (Adc.Enrollment e in enrollmentUserId)
                    {
                        if (!instructors.Exists(i => i.Id == e.User.Id))
                        {
                            instructors.Add(e.User);
                        }
                    }
                }
            }
            return instructors;
        }

        public IEnumerable<Course> FindCoursesByInstructor(String application_type, string instructorId, string domainId, string academicTerm)
        {
            string query = string.Empty;
            if (string.IsNullOrEmpty(application_type))
            {
                query = string.Format(@"/meta-bfw_academic_term='{0}'", academicTerm);
            }
            else
            {
                query = string.Format(@"/meta-bfw_academic_term='{0}' AND /meta-bfw_course_type = '{1}'",
                                    academicTerm, application_type);
            }

            //get all the coruses for the doamin/term selected
            var cmdCourses = new GetCourse()
            {
                SearchParameters = new Adc.CourseSearch()
                {
                    DomainId = domainId,
                    Query = query
                }
            };
            SessionManager.CurrentSession.ExecuteAsAdmin(cmdCourses);

            //no get the enrollments
            var enrollmentsCmd = new GetUserEnrollmentList()
            {
                SearchParameters = new Adc.EntitySearch()
                {
                    UserId = instructorId
                }
            };

            var courses = cmdCourses.Courses.Filter(c => c.Domain.Id == domainId);

            SessionManager.CurrentSession.ExecuteAsAdmin(enrollmentsCmd);

            var userEnrollments = enrollmentsCmd.Enrollments.Where(u => u.Flags.HasFlag(DlapRights.SubmitFinalGrade));
            var enrollmentCourseId = userEnrollments.Map(ue => ue.Course.Id);

            courses = courses.Filter(c => enrollmentCourseId.Contains(c.Id));
            //TODO
            //string checkId = (Context.CourseIsProductCourse) ? Context.Course.Id : Context.ProductCourseId;
            //courses = courses.Filter(c => c.ParentId == checkId);    // to filter courses by Products

            return courses.Map(c => c.ToCourse());
        }

    }
}
