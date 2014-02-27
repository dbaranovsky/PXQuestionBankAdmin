using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PXWebAPI.Mappers;
using Bfw.PXWebAPI.Models;
using Adc = Bfw.Agilix.DataContracts;

namespace Bfw.PXWebAPI.Helpers
{

	/// <summary>
	/// IApiUserActions
	/// </summary>
	public interface IApiUserActions
	{
		/// <summary>
		/// FindCoursesByInstructor
		/// </summary>
		/// <param name="application_type"></param>
		/// <param name="instructorId"></param>
		/// <param name="domainId"></param>
		/// <param name="academicTerm"></param>
		/// <returns></returns>
		IEnumerable<Course> FindCoursesByInstructor(string application_type, string instructorId, string domainId, string academicTerm);

		/// <summary>
		/// GetEnrollments
		/// </summary>
		/// <param name="id"></param>
		/// <param name="byreference"></param>
		/// <param name="domainId"></param>
		/// <returns></returns>
		List<Agilix.DataContracts.Enrollment> GetEnrollments(string id, bool byreference = false, string domainId = "");

		/// <summary>
		/// GetUserEnrollments
		/// </summary>
		/// <param name="userid"></param>
		/// <param name="courseid"></param>
		/// <returns></returns>
		List<Agilix.DataContracts.Enrollment> GetUserEnrollments(string userid, string courseid = "");


		/// <summary>
		/// GetUserEnrollmentsByQueryAndFlags
		/// </summary>
		/// <param name="userid"></param>
		/// <param name="query"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		List<Agilix.DataContracts.Enrollment> GetUserEnrollmentsByQueryAndFlags(string userid, string query, string flags);


		/// <summary>
		/// GetUsers
		/// </summary>
		/// <param name="rauserid"></param>
		/// <param name="domainid"></param>
		/// <param name="userid"></param>
		/// <returns></returns>
		List<Agilix.DataContracts.AgilixUser> GetUsers(string rauserid = "", string domainid = "", string userid = "");

		/// <summary>
		/// GetUsersFromAllDomains
		/// </summary>
		/// <param name="id"></param>
		/// <param name="byreference"></param>
		/// <returns></returns>
		List<Agilix.DataContracts.AgilixUser> GetUsersFromAllDomains(string id, bool byreference = false);


		/// <summary>
		/// ListInstructorsForDomain
		/// </summary>
		/// <param name="application_type"></param>
		/// <param name="domainId"></param>
		/// <param name="academicTerm"></param>
		/// <returns></returns>
		IEnumerable<Agilix.DataContracts.AgilixUser> ListInstructorsForDomain(string application_type, string domainId, string academicTerm);


		/// <summary>
		/// UpdateUsers
		/// </summary>
		/// <param name="id"></param>
		/// <param name="byreference"></param>
		/// <param name="userInput"></param>
		/// <returns></returns>
		bool UpdateUsers(string id, bool byreference, EditUser userInput);
	}

	/// <summary>
	/// ApiUserActions
	/// </summary>
	public class ApiUserActions : IApiUserActions
	{

		#region Properties

		protected ISessionManager SessionManager { get; set; }

        protected IUserActions UserActions { get; set; }

		protected IBusinessContext Context { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ApiUserActions"/> class.
		/// </summary>
		/// <param name="sessionManager">The session manager.</param>
        /// <param name="userActions">The User Actions.</param>
		/// <param name="context"> </param>
		public ApiUserActions(ISessionManager sessionManager, IUserActions userActions, PX.Biz.ServiceContracts.IBusinessContext context)
		{
			SessionManager = sessionManager;
            UserActions = userActions;
			Context = context;
		}

		#endregion
		/// <summary>
		/// GetUsers
		/// </summary>
		/// <param name="rauserid"></param>
		/// <param name="domainid"></param>
        /// <param name="userid"></param>
		/// <returns></returns>
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
				//Distinct is performed here temporarily to fix the issue of duplicate users in Dlap response(bug)
				ausrs = ausrs.GroupBy(i => i.Id, (key, group) => group.First()).ToList();
				return ausrs;
			}
			return new List<Adc.AgilixUser>();
		}

        /// <summary>
		/// GetUsersFromAllDomains
		/// </summary>
		/// <param name="id"></param>
		/// <param name="byreference">Flag to use RA UserID</param>
		/// <returns></returns>
		public List<Adc.AgilixUser> GetUsersFromAllDomains(string id, bool byreference = false)
        {
            return byreference ? GetUsers(rauserid: id) : GetUsers(userid: id);
        }

	    /// <summary>
		/// GetUserEnrollments
		/// </summary>
		/// <param name="userid"></param>
		/// <param name="courseid"></param>
		/// <returns></returns>
		public List<Adc.Enrollment> GetUserEnrollments(string userid, string courseid = "")
		{
			var cmd = new GetUserEnrollmentList
						{
							SearchParameters = new Adc.EntitySearch
												{
													UserId = userid,
													CourseId = courseid
												}
						};
			SessionManager.CurrentSession.Execute(cmd);
			var aenrollments = cmd.Enrollments;
			return aenrollments;
		}

		/// <summary>
		/// GetUserEnrollmentsByQueryAndFlags
		/// </summary>
		/// <param name="userid"></param>
		/// <param name="query"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		public List<Adc.Enrollment> GetUserEnrollmentsByQueryAndFlags(string userid, string query, string flags)
		{
			var cmd = new GetUserEnrollmentList
						{
							SearchParameters = new Adc.EntitySearch
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



		/// <summary>
		/// GetEnrollments
		/// </summary>
		/// <param name="id"></param>
		/// <param name="byreference">Flag to use RA UserID</param>
		/// <param name="domainId"></param>
		/// <returns></returns>
		public List<Adc.Enrollment> GetEnrollments(string id, bool byreference = false, string domainId = "")
		{
			if (!byreference) return GetUserEnrollments(id);
			var users = GetUsers(id, domainId);
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

		/// <summary>
		/// UpdateUsers
		/// </summary>
		/// <param name="id"></param>
		/// <param name="byreference"></param>
		/// <param name="userInput"></param>
		/// <returns></returns>
		public bool UpdateUsers(string id, bool byreference, EditUser userInput)
		{
            var result = false;
            var list = new List<Bfw.PX.Biz.DataContracts.UserInfo>();
            var users = GetUsersFromAllDomains(id, byreference);

            if (users != null)
            {
                foreach (var user in users)
                {
                    list.Add(new PX.Biz.DataContracts.UserInfo()
                        {
                            Id = user.Id??"",
                            FirstName = userInput.FirstName,
                            LastName = userInput.LastName,
                            Email = userInput.Email,
                            DomainId = user.Domain==null?"":user.Domain.Id,
                            Username = user.UserName??""
                        });
                }
            }

            if (list.Count > 0)
            {
                result = UserActions.UpdateUsers(list);
            }

			return result;
		}

		/// <summary>
		/// ListInstructorsForDomain
		/// </summary>
		/// <param name="application_type"></param>
		/// <param name="domainId"></param>
		/// <param name="academicTerm"></param>
		/// <returns></returns>
		public IEnumerable<Adc.AgilixUser> ListInstructorsForDomain(String application_type, string domainId, string academicTerm)
		{
			var batch = new Batch();
			var instructors = new List<Adc.AgilixUser>();
			//get all the courses first for the domain/term selected
			string query;
			if (string.IsNullOrEmpty(application_type))
			{
				query = string.Format(@"/meta-bfw_academic_term='{0}'", academicTerm);
			}
			else
			{
				query = string.Format(@"/meta-bfw_academic_term='{0}' AND /meta-bfw_course_type = '{1}'",
									academicTerm, application_type);
			}
			var cmdCourses = new GetCourse
								{
									SearchParameters = new Adc.CourseSearch
														   {
															   DomainId = domainId,
															   Query = query
														   }
								};
			SessionManager.CurrentSession.ExecuteAsAdmin(cmdCourses);

			if (cmdCourses.Courses != null)
			{
				foreach (var c in cmdCourses.Courses.Filter(c => c.Domain.Id == domainId))
				{
					batch.Add(new GetEntityEnrollmentList
								{
									SearchParameters = new Adc.EntitySearch
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
							u => ( u.Flags.HasFlag(DlapRights.SubmitFinalGrade) && u.Domain.Id.Equals(domainId) )));
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

		/// <summary>
		/// FindCoursesByInstructor
		/// </summary>
		/// <param name="application_type"></param>
		/// <param name="instructorId"></param>
		/// <param name="domainId"></param>
		/// <param name="academicTerm"></param>
		/// <returns></returns>
		public IEnumerable<Course> FindCoursesByInstructor(String application_type, string instructorId, string domainId, string academicTerm)
		{
			string query;
			if (string.IsNullOrEmpty(application_type))
			{
				query = string.Format(@"/meta-bfw_academic_term='{0}'", academicTerm);
			}
			else
			{
				query = string.Format(@"/meta-bfw_academic_term='{0}' AND /meta-bfw_course_type = '{1}'",
									academicTerm, application_type);
			}

			//get all the courses for the domain/term selected
			var cmdCourses = new GetCourse
								{
									SearchParameters = new Adc.CourseSearch
														{
															DomainId = domainId,
															Query = query,
														}
								};
			SessionManager.CurrentSession.ExecuteAsAdmin(cmdCourses);

			//no get the enrollments
			var enrollmentsCmd = new GetUserEnrollmentList
									{
										SearchParameters = new Adc.EntitySearch
															{
																UserId = instructorId
															}
									};

			var courses = cmdCourses.Courses.Filter(c => c.Domain.Id == domainId);

			SessionManager.CurrentSession.ExecuteAsAdmin(enrollmentsCmd);

			var userEnrollments = enrollmentsCmd.Enrollments.Where(u => u.Flags.HasFlag(DlapRights.SubmitFinalGrade) & u.Domain.Id==domainId);

			var enrollableCourses = new List<Adc.Course>();

			foreach (var enrollment in userEnrollments.ToList())
			{
				var enrllmnt = enrollment;
				var foundCourses = courses.ToList().Where(course => course.Id == enrllmnt.Course.Id);
				if (foundCourses.Any())
				{
					enrollableCourses.AddRange(foundCourses);
				}

			}

			return enrollableCourses.Map(c => c.ToCourse());
		}

	}
}
