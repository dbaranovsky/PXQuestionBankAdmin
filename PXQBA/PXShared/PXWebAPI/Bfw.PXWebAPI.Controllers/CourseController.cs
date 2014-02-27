using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Collections;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PXWebAPI.Helpers;
using Bfw.PXWebAPI.Mappers;
using Bfw.PXWebAPI.Models;
using Bfw.PXWebAPI.Models.DTO;
using Bfw.PXWebAPI.Models.Response;
using CopyMethod = Bfw.PXWebAPI.Models.CopyMethod;
using Course = Bfw.PXWebAPI.Models.Course;
using CourseAcademicTerm = Bfw.PXWebAPI.Models.CourseAcademicTerm;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using BizDC = Bfw.PX.Biz.DataContracts;

namespace Bfw.PXWebAPI.Controllers
{
	/// <summary>
	/// CourseController
	/// </summary>
	public class CourseController : ApiController
	{
		protected ISessionManager SessionManager { get; set; }

		protected IBusinessContext Context { get; set; }

	    protected IEnrollmentActions PxEnrollmentActions { get; set; }

	    /// <summary>
		/// Gets  the Api User Actions.
		/// </summary>
		/// <value>
		/// The API User actions.
		/// </value>
		protected IApiUserActions ApiUserActions { get; set; }

        /// <summary>
        /// get api PageActions
        /// </summary>
	    protected IPageActions PageActions { get; set; }

	    /// <summary>
		/// Gets  the Api Course Actions.
		/// </summary>
		/// <value>
		/// The API Course actions.
		/// </value>
		protected IApiCourseActions ApiCourseActions { get; set; }

		/// <summary>
		/// CourseController
		/// </summary>
		/// <param name="sessionManager"></param>
		/// <param name="context"> </param>
		/// <param name="apiCourseActions"> </param>
		/// <param name="apiUserActions"> </param>
		public CourseController(ISessionManager sessionManager, PX.Biz.ServiceContracts.IBusinessContext context, IApiCourseActions apiCourseActions,
								IApiUserActions apiUserActions, IPageActions pageActions, IEnrollmentActions pxEnrollmentActions)
		{
			SessionManager = sessionManager;
			Context = context;
			ApiUserActions = apiUserActions; //new ApiUserActions(sessionManager, context);
			ApiCourseActions = apiCourseActions;
		    PageActions = pageActions;
		    PxEnrollmentActions = pxEnrollmentActions;
		}


		/// <summary>
		/// Get Course Details By CourseId 
		/// </summary>
		/// <param name="id"></param>
		/// <returns>CourseResponse</returns>
		[HttpGet]
		[ActionName("Details")]
		public CourseResponse Details(string id)
		{
			var response = new CourseResponse();

			var courseActions = ApiCourseActions;
			var course = courseActions.GetCourseByCourseId(id);
            var enrollments = PxEnrollmentActions.GetEntityEnrollments(id, BizDC.UserType.Instructor);
            
			if (course != null)
			{
				response.results = course.ToCourseDto();
			    response.results.Instructors = enrollments != null
			        ? enrollments.Select(
			            e => new Instructor() {Email = e.User.Email, Id = e.User.Id, Name = e.User.FormattedName}).ToList()
			        : null;
			    return response;
			}

			//TODO: Remove hardcoded text from here (SK)
			response.error_message = "No results found";
			return response;
		}


		/// <summary>
        /// <para>Function Add creates and saves child copy of Parent Course.</para>
        /// <para>The following POST Parameters expected:</para>
        /// <para>REQUIRED: parentcourseid, domainid, userid.</para>
        /// <para>OPTIONAL: title, type, academicterm, courseNumber, courseTimeZone, instructorName, sectionNumber</para>
		/// Example to test: Course/Add POST: parentcourseid=109554 + domainid=113354 + userid=113371 + title=dinatestForScott  
		/// </summary>        
        /// <returns>CourseResponse</returns>
		[HttpPost]
		[ActionName(Helper.ADD)]
		public ApiCourseResponse AddDerivativeCourse()
		{
            var response = new ApiCourseResponse();

			var parentcourseid = ApiHelper.GetFormRequestParameter("parentcourseid");
			var domainid = ApiHelper.GetFormRequestParameter("domainid");
			var userid = ApiHelper.GetFormRequestParameter("userid");
			var title = ApiHelper.GetFormRequestParameter("title");
			var coursetype = ApiHelper.GetFormRequestParameter("type");
			var academicterm = ApiHelper.GetFormRequestParameter("academicterm");
			var courseNumber = ApiHelper.GetFormRequestParameter("courseNumber");
			var courseTimeZone = ApiHelper.GetFormRequestParameter("courseTimeZone");
			var instructorName = ApiHelper.GetFormRequestParameter("instructorName");
			var sectionNumber = ApiHelper.GetFormRequestParameter("sectionNumber");

            if (parentcourseid.Trim().Equals(string.Empty) || domainid.Trim().Equals(string.Empty) || userid.Trim().Equals(string.Empty))
            {
                response.error_message = Helper.REQUIRED_PARAMS_MISSING;
            }
            else
            {
                Bfw.PX.Biz.DataContracts.Course parentCourse = Bfw.PX.Biz.Services.Mappers.BizEntityExtensions.ToCourse(ApiCourseActions.GetCourseByCourseId(parentcourseid));

                Context.CurrentUser = new UserInfo { Id = userid };
                Context.CurrentUser = Context.GetNewUserData();

                var course = ApiCourseActions.AddCopyDerivedCourse(parentCourse, domainid, CopyMethod.Derivative, Context.CurrentUser, title, coursetype, academicterm, courseNumber, instructorName, courseTimeZone, sectionNumber);

                if (course == null)
                {
                    response.error_message = Helper.NO_RESULTS;
                }
                else
                {
                    response.results = course;
                }
            }

			return response;
		}



        /// <summary>
        /// Function Copy creates and saves Sibling Copy of the Source Course.
        /// The following POST Parameters expected:
        /// REQUIRED: sourcecourseid, domainid, userid
        /// OPTIONAL: title, type, term, courseNumber,courseTimeZone,instructorName,sectionNumber
        /// Example to test: Course/Copy POST: sourcecourseid=109554 + domainid=113354 + userid=113371 + title=dinatestForScott 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Copy")]
        public ApiCourseResponse DerivativeSiblingCopy()
        {

            var response = new ApiCourseResponse();

            var sourcecourseid = ApiHelper.GetFormRequestParameter("sourcecourseid");
            var domainid = ApiHelper.GetFormRequestParameter("domainid");
            var userid = ApiHelper.GetFormRequestParameter("userid");
            var title = ApiHelper.GetFormRequestParameter("title");
            var coursetype = ApiHelper.GetFormRequestParameter("type");
            var academicterm = ApiHelper.GetFormRequestParameter("term");

            var courseNumber = ApiHelper.GetFormRequestParameter("courseNumber");
            var courseTimeZone = ApiHelper.GetFormRequestParameter("courseTimeZone");
            var instructorName = ApiHelper.GetFormRequestParameter("instructorName");
            var sectionNumber = ApiHelper.GetFormRequestParameter("sectionNumber");

            if (sourcecourseid.Trim().Equals(string.Empty) || domainid.Trim().Equals(string.Empty) ||
                userid.Trim().Equals(string.Empty))
            {
                response.error_message = "Required parameters are missing";
                return response;
            }

            Bfw.PX.Biz.DataContracts.Course sourceCourse = Bfw.PX.Biz.Services.Mappers.BizEntityExtensions.ToCourse(ApiCourseActions.GetCourseByCourseId(sourcecourseid));

            Context.CurrentUser = new UserInfo { Id = userid };
            Context.CurrentUser = Context.GetNewUserData();

            var course = ApiCourseActions.AddCopyDerivedCourse(sourceCourse, domainid, CopyMethod.DerivativeSiblingCopy, Context.CurrentUser, title, coursetype, academicterm,
                                                               courseNumber, instructorName, courseTimeZone, sectionNumber);

            if (course != null)
            {
                Context.CourseId = course.Id;
                Context.ProductCourseId = course.IsProductCourse?course.Id:course.ProductCourseId;

                var widget = GetInstructorConsoleLaunchPadSettings().ToDcItem();

                widget.CollapsePastDue = false;
                widget.ShowCollapsePastDue = false;

                this.PageActions.UpdateWidget(widget);
                this.PageActions.EmptySettingsCache(widget.Id);

                response.results = course;
                return response;
            }

            //TODO: Remove hardcoded text from here (SK)
            response.error_message = "No results found";
            return response;
        }

        private Bfw.PX.PXPub.Models.LaunchPadSettings GetInstructorConsoleLaunchPadSettings()
        {
            var settingsDC = this.PageActions.GetInstructorConsoleLaunchPadSettings();
            var model = new Bfw.PX.PXPub.Models.LaunchPadSettings();
            model.ToModelItem(settingsDC);
            return model;
        }

		/// <summary>
		/// Get List of Course Academic Terms by domainId
		/// </summary>
		/// <param name="domainId"></param>
		/// <returns>IEnumerable of CourseAcademicTerms</returns>
		[HttpGet]
		[ActionName("Terms")]
		public IEnumerable<CourseAcademicTerm> GetTerms(string domainId)
		{
			var courseActions = ApiCourseActions;
			var terms = courseActions.ListAcademicTerms(domainId);
			return terms;
		}


		/// <summary>
		/// Get List of Instructors by domainId and termId
		/// </summary>
		/// <param name="domainId"></param>
		/// <param name="termId"></param>
		/// <returns>IEnumerable of Users</returns>
		[HttpGet]
		[ActionName("Instructors")]
		public IEnumerable<User> GetInstructors(string domainId, string termId)
		{
			var userActions = ApiUserActions;
			//String application_type = ApiCourseActions.getApplicationType(Context.Course.CourseType);
			var instructors = userActions.ListInstructorsForDomain(null, domainId, termId);
			return instructors.Map(x => x.ToUser());
		}

        /// <summary>
		/// Get List of Active Courses for Instructor by userId, domainId and termId
		/// </summary>
		/// <param name="domainId"></param>
		/// <param name="termId"></param>
		/// <param name="userId"></param>
		/// <returns>IEnumerable of Courses</returns>
		[HttpGet]
		[ActionName("InstructorCourseList")]
		public IEnumerable<Course> GetInstructorCourseList(string domainId, string termId, string userId)
		{
			string application_type = ""; //ApiApiCourseActions.getApplicationType(Context.Course.CourseType);
			// gets all courses the instructor is teaching for that term
			var courseActions = ApiCourseActions;
			var userActions = ApiUserActions;
			IEnumerable<Course> courses = userActions.FindCoursesByInstructor(application_type, userId, domainId, termId);
			// gets the course from dlap (currently does not contain all information)
			var fullCourses = courseActions.GetCourseListInformation(courses);
			// make sure all courses are activated
			//Course.IsActivated  = !String.IsNullOrEmpty(ActivatedDate) && ActivatedDate != DateTime.MaxValue.ToShortDateString();
			var activeCourses = fullCourses.OrderByDescending(c => c.Id);
			var courseList = activeCourses.ToArray().Take(500);
			return courseList;
		}

	}
}
