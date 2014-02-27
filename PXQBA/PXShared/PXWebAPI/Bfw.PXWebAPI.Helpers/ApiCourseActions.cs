using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Caching;
using System.Xml.Linq;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Caching;
using Bfw.Common.Collections;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PXWebAPI.Mappers;
using Bfw.PXWebAPI.Models;
using Adc = Bfw.Agilix.DataContracts;
using Course = Bfw.PXWebAPI.Models.Course;

namespace Bfw.PXWebAPI.Helpers
{
	/// <summary>
	/// IApiCourseActions
	/// </summary>
	public interface IApiCourseActions
	{
		/// <summary>
		/// Add Derived Course to Parent Course
		/// </summary>
		/// <param name="parentCourse"></param>
		/// <param name="domainid"></param>
		/// <param name="copyMethod"></param>
		/// <param name="title"></param>
		/// <param name="coursetype"></param>
		/// <param name="academicterm"></param>
		/// <param name="courseNumber"></param>
		/// <param name="instructorName"></param>
		/// <param name="courseTimeZone"></param>
		/// <param name="sectionNumber"></param>
		/// <returns></returns>
		Bfw.PX.PXPub.Models.Course AddCopyDerivedCourse(PX.Biz.DataContracts.Course parentCourse, string domainid,
                                                               string copyMethod, Bfw.PX.Biz.DataContracts.UserInfo user,
															   string title = "", string coursetype = "",
															   string academicterm = "",
															   string courseNumber = "", string instructorName = "",
															   string courseTimeZone = "", string sectionNumber = "");

        /// <summary>
        /// CreateUserEnrollment
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="courseid"></param>
        /// <param name="domainid"></param>
        /// <param name="isinstructor"></param>
        /// <param name="enrollmentstatus"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        Adc.Enrollment CreateUserEnrollment(string userid, string courseid, string domainid, bool isinstructor = false, string enrollmentstatus = "", string startdate = "", string enddate = "");

        /// <summary>
		/// GetCourseByCourseId
		/// </summary>
		/// <param name="courseId"></param>
		/// <returns></returns>
		Agilix.DataContracts.Course GetCourseByCourseId(string courseId);

		/// <summary>
		/// GetCourseByQuery
		/// </summary>
		/// <param name="query"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		List<Agilix.DataContracts.Course> GetCourseByQuery(string query, string userId = "");

		/// <summary>
		/// GetCourseListInformation
		/// </summary>
		/// <param name="courses"></param>
		/// <returns></returns>
		IEnumerable<Agilix.DataContracts.Course> GetCourseListInformation(IEnumerable<Agilix.DataContracts.Course> courses);

		/// <summary>
		/// GetCourseListInformation
		/// </summary>
		/// <param name="courses"></param>
		/// <returns></returns>
		IEnumerable<Course> GetCourseListInformation(IEnumerable<Course> courses);

		/// <summary>
		/// GetEntityEnrollments
		/// </summary>
		/// <param name="userid"></param>
		/// <param name="courseid"></param>
		/// <param name="flags"></param>
		/// <param name="query"></param>
		/// <returns></returns>
		List<Agilix.DataContracts.Enrollment> GetEntityEnrollments(string userid, string courseid = "", string flags = "", string query = "");

		/// <summary>
		/// ListAcademicTerms
		/// </summary>
		/// <param name="domainId"></param>
		/// <returns></returns>
		IEnumerable<CourseAcademicTerm> ListAcademicTerms(string domainId);

		/// <summary>
		/// UpdateCourses
		/// </summary>
		/// <param name="courses"></param>
		/// <returns></returns>
		List<PX.PXPub.Models.Course> UpdateCourses(List<PX.PXPub.Models.Course> courses);

		/// <summary>
		/// UpdateCourse
		/// </summary>
		/// <param name="course"></param>
		/// <returns></returns>
		PX.PXPub.Models.Course UpdateCourse(PX.PXPub.Models.Course course);
	}


	/// <summary>
	/// Course Actions
	/// </summary>
	public class ApiCourseActions : IApiCourseActions
	{
		#region Properties

		protected ISessionManager SessionManager { get; set; }

        protected ICacheProvider CacheProvider { get; set; }
		protected IBusinessContext Context { get; set; }

		protected IContentActions PxContentActions { get; set; }
		protected ICourseActions PxCourseActions { get; set; }
		protected INoteActions PxNoteActions { get; set; }
		protected IDocumentConverter PxDocumentConverter { get; set; }
		protected IDomainActions PxDomainActions { get; set; }
        protected IUserActions PXUserActions { get; set; }        

		#endregion

		/// <summary>
		/// A const for that holds the instructor permission flags.
		/// </summary>
		private readonly DlapRights INSTRUCTOR_FLAGS = (DlapRights)Int64.Parse(System.Configuration.ConfigurationManager.AppSettings["InstructorPermissionFlags"]);

		/// <summary>
		/// A const for that holds the student permission flags.
		/// </summary>
		private readonly DlapRights STUDENT_FLAGS = (DlapRights)Int64.Parse(System.Configuration.ConfigurationManager.AppSettings["StudentPermissionFlags"]);


		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ApiCourseActions"/> class.
		/// </summary>
		/// <param name="sessionManager">The session manager.</param>
		/// <param name="context"> </param>
        /// <param name="courseActions"></param>
        public ApiCourseActions(ISessionManager sessionManager, PX.Biz.ServiceContracts.IBusinessContext context, ICourseActions courseActions, IItemQueryActions itemQueryActions, ICacheProvider cacheProvider)
		{
			SessionManager = sessionManager;
			Context = context;
		    CacheProvider = cacheProvider;

			PxDocumentConverter = new AsposeDocumentConverter();
            
            var databaseManager = new Bfw.Common.Database.DatabaseManager();
			
            PxContentActions = new ContentActions(Context, SessionManager, PxDocumentConverter, databaseManager, itemQueryActions);
			PxNoteActions = new NoteActions(Context, SessionManager);
			PxDomainActions = new DomainActions(Context, SessionManager, cacheProvider);
            PxCourseActions = courseActions;
            PXUserActions = new UserActions(Context, SessionManager, PxContentActions, PxDomainActions);
		}


	    #endregion


		/// <summary>
		/// Add Derived Course to Parent Course
		/// </summary>
		/// <param name="parentCourse"></param>
		/// <param name="domainid"></param>
		/// <param name="copyMethod"></param>
		/// <param name="title"></param>
		/// <param name="coursetype"></param>
		/// <param name="academicterm"></param>
		/// <param name="courseNumber"></param>
		/// <param name="instructorName"></param>
		/// <param name="courseTimeZone"></param>
		/// <param name="sectionNumber"></param>
		/// <returns></returns>
		public Bfw.PX.PXPub.Models.Course AddCopyDerivedCourse(PX.Biz.DataContracts.Course parentCourse, string domainid, string copyMethod, Bfw.PX.Biz.DataContracts.UserInfo user,
																string title = "", string coursetype = "", string academicterm = "",
																string courseNumber = "", string instructorName = "",
																string courseTimeZone = "", string sectionNumber = "")
		{
			PX.Biz.DataContracts.Course derivedCourse = PxCourseActions.CreateDerivedCourse(parentCourse, domainid, copyMethod, user.Id);

			derivedCourse.Title = title;
			derivedCourse.AcademicTerm = academicterm;
			derivedCourse.CourseType = coursetype;
            derivedCourse.CourseOwner = user.Id;
			derivedCourse.CourseNumber = courseNumber;
			derivedCourse.InstructorName = instructorName;
			derivedCourse.CourseTimeZone = courseTimeZone;
			derivedCourse.SectionNumber = sectionNumber;
			derivedCourse.ActivatedDate = DateTime.Now.ToString();

			derivedCourse = PxCourseActions.UpdateCourse(derivedCourse);

			return derivedCourse.ToCourse();
		}

		/// <summary>
		/// Gets a course by course ID.
		/// </summary>
		/// <param name="courseId">The course ID.</param>
		/// <returns></returns>
		public Adc.Course GetCourseByCourseId(string courseId)
		{
			var cmd = new GetCourse
						{
							SearchParameters = new CourseSearch
												  {
													  CourseId = courseId
												  }
						};

			SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
			Agilix.DataContracts.Course result = cmd.Courses.First();
			return result;
		}

		/// <summary>
		/// Gets a course by query.
		/// </summary>
		/// <param name="query">The course ID.</param>
		/// <param name="userId"> </param>
		/// <returns></returns>
		public List<Adc.Course> GetCourseByQuery(string query, string userId = "")
		{
			var cmd = new GetCourse
						{
							SearchParameters = new CourseSearch
												{


													Query = query
												}
						};

			SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
			List<Agilix.DataContracts.Course> result = cmd.Courses.ToList();
			return result;
		}

		public IEnumerable<Adc.Course> GetCourseListInformation(IEnumerable<Adc.Course> courses)
		{
			var list = new List<Adc.Course>();
			var batch = new Batch();
			foreach (var c in courses)
			{
				batch.Add(new GetCourse
							{
								SearchParameters = new CourseSearch
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
		/// Updates a set of courses.
		/// </summary>
		/// <param name="courses">The courses.</param>
		/// <returns></returns>
		public List<Bfw.PX.PXPub.Models.Course> UpdateCourses(List<Bfw.PX.PXPub.Models.Course> courses)
		{
			List<Bfw.PX.Biz.DataContracts.Course> bizList = courses.Map(c => c.ToCourse()).ToList();

			bizList = PxCourseActions.UpdateCourses(bizList);

			courses = bizList.Map(c => c.ToCourse()).ToList();

			return courses;
		}




		/// <summary>
		/// Updates course.
		/// </summary>
		/// <param name="course">The courses.</param>
		/// <returns></returns>
		public Bfw.PX.PXPub.Models.Course UpdateCourse(Bfw.PX.PXPub.Models.Course course)
		{
			Bfw.PX.Biz.DataContracts.Course biz = course.ToCourse();
			biz = PxCourseActions.UpdateCourse(biz);
			course = biz.ToCourse();
			return course;
		}


		/// <summary>
		/// Copies a set of courses to a new domain.
		/// </summary>
		/// <param name="courses">The courses.</param>
		/// <param name="newDomainId">The new domain ID.</param>
		/// <param name="method">The method.</param>
		/// <param name="courseOwner">Course owner to enroll.</param>
		/// <returns></returns>
		private List<Course> ApiCopyCourses(IEnumerable<Course> courses, string newDomainId, string method, string courseOwner)
		{
			var cmd = new CopyCourses
						{
							DomainId = newDomainId,
							Method = method
						};
			cmd.Add(courses.Map(c => c.ToAgxCourse()).ToList());
			SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

			var result = cmd.Courses.Map(c => c.ToCourse()).ToList();
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
				cmd.Add(new Adc.Enrollment
							{
								User = new AgilixUser { Id = courseOwner },
								Course = course.ToAgxCourse(),
								Status = "1",
								StartDate = DateTime.Now,
								EndDate = DateTime.MaxValue,
								Flags = INSTRUCTOR_FLAGS
							});

				cmd.Add(new Adc.Enrollment
							{
								User = new AgilixUser { Id = courseOwner },
								Course = course.ToAgxCourse(),
								Status = "10",
								StartDate = DateTime.Now,
								EndDate = DateTime.MaxValue,
								Flags = STUDENT_FLAGS
							});


				SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
			}
		}

		public Adc.Enrollment CreateUserEnrollment(string userid, string courseid, string domainid,
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
				User = new AgilixUser { Id = userid },
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
            if (!cmd.Enrollments.IsNullOrEmpty())
            {
                return cmd.Enrollments.First();
            }
            return null;
		}

		public List<Adc.Enrollment> GetEntityEnrollments(string userid, string courseid = "", string flags = "", string query = "")
		{
			var cmd = new GetEntityEnrollmentList
						{
							SearchParameters = new EntitySearch
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
			var contentActions = PxContentActions;
			var resource = contentActions.GetResource(domainId, "PX/academicterms.xml");

			if (resource != null && resource.ContentType != null)
			{
				result = XDocument.Load(resource.GetStream());
			}
			else
			{
				result = XDocument.Parse("<academic_terms/>");
			}

			return result.Root.Elements("academic_term").Select(export => export.ToCourseAcademicTerm());
		}

		public IEnumerable<Course> GetCourseListInformation(IEnumerable<Course> courses)
		{
			var fullcourses = new List<Course>();
			var batch = new Batch();
			foreach (var c in courses)
			{
				batch.Add(new GetCourse
							{
								SearchParameters = new CourseSearch
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
				fullcourses.AddRange(list.Select(c => c.ToCourse()));
			}
			return fullcourses;
		}
	}
}
