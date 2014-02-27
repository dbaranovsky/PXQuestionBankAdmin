using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Collections;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PXWebAPI.Mappers;
using Bfw.PXWebAPI.Models;
using Bfw.PXWebAPI.Models.Response;

namespace Bfw.PXWebAPI.Controllers
{
	/// <summary>
	/// EnrollmentController
	/// </summary>
	public class EnrollmentController : ApiController
	{
		protected ISessionManager SessionManager { get; set; }

		protected IBusinessContext Context { get; set; }

		protected Helpers.IApiEnrollmentActions ApiEnrollmentActions { get; set; }
        protected Helpers.IApiCourseActions ApiCourseActions { get; set; }
        protected Helpers.IApiUserActions ApiUserActions { get; set; }

		/// <summary>
		/// EnrollmentController
		/// </summary>
		/// <param name="sessionManager"></param>
		/// <param name="context"> </param>
		/// <param name="apiEnrollmentActions"> </param>
		public EnrollmentController(ISessionManager sessionManager, IBusinessContext context,
																	Helpers.IApiEnrollmentActions apiEnrollmentActions,
                                                                    Helpers.IApiCourseActions apiCourseActions,
                                                                    Helpers.IApiUserActions apiUserActions)
		{
			SessionManager = sessionManager;
			Context = context;
			ApiEnrollmentActions = apiEnrollmentActions;
		    ApiCourseActions = apiCourseActions;
		    ApiUserActions = apiUserActions;
		}

		/// <summary>
		/// Get Enrollable Domains by RaUserId, ParentDomainId and optional comma separated forceList of DomainIds 
		/// new route:      dev.px.bfwpub.com/api/Enrollment/GetEnrollableDomains?raUserId=9+parentDomainId=1 
		/// existing route: dev.px.bfwpub.com/api/Enrollment/EnrollableDomains?raUserId=9+parentDomainId=1	  
		/// </summary>
		/// <param name="raUserId"></param>
		/// <param name="parentDomainId"></param>
		/// <param name="forceList"></param>
		/// <returns>DomainListResponse</returns>
		[HttpGet]
		[ActionName("EnrollableDomains")]
		public DomainListResponse GetEnrollableDomains(string raUserId, string parentDomainId, string forceList = "")
		{
			var response = new DomainListResponse();

			var enrollmentActions = ApiEnrollmentActions;
			forceList = System.Web.HttpUtility.UrlDecode(forceList);
			IEnumerable<string> forceIfMember = forceList.Split(',');
			response.results = enrollmentActions.GetEnrollableDomains(raUserId, parentDomainId, forceIfMember).Map(
					x => x.ToDomainDto()).ToList();

			return response;
		}

		/// <summary>
		/// Get User EnrollmentId by UserId, EntityId and optional allStatus
		/// new route: dev.px.bfwpub.com/api/Enrollment/GetUserEnrollmentId?userId=47+entityId=97638	 	
		/// existing route: dev.px.bfwpub.com/api/Enrollment/UserEnrollmentId?userId=47+entityId=97638		 
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="entityId"></param>
		/// <param name="allStatus"></param>
		/// <returns>GenericResponse</returns>
		[HttpGet]
		[ActionName("UserEnrollmentId")]
		public GenericResponse GetUserEnrollmentId(string userId, string entityId, bool allStatus = false)
		{
			var genericResponse = new GenericResponse();
			var enrollmentActions = ApiEnrollmentActions;

			genericResponse.results = enrollmentActions.GetUserEnrollmentId(userId, entityId, allStatus);

			return genericResponse;
		}

		/// <summary>
		/// Get Enrollees by EntityId and userType, where user type one of the following: Instructor, Student, All
		/// new route: dev.px.bfwpub.com/api/Enrollment/GetEnrollees?entityId=97638+userType=0              
		/// existing route: dev.px.bfwpub.com/api/Enrollment/Enrollees?entityId=97638+userType=0                   
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="userType"></param>
		/// <returns>EnrolleeListResponse</returns>
		[HttpGet]
		[ActionName("Enrollees")]
		public EnrolleeListResponse GetEnrollees(string entityId, string userType)
		{
			var response = new EnrolleeListResponse();
			var enrollmentActions = ApiEnrollmentActions;

			var typeOfUser = (UserType)Enum.Parse(typeof(UserType), userType);
			response.results = enrollmentActions.GetEntityEnrollments(entityId, typeOfUser).Map(e => e.ToEnrollee()).ToList();

			return response;
		}

        /// <summary>
        /// Invalidate user enrollment from given course
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ClearEnrollment")]
        public BoolResponse ClearEnrollmentCache(string raUserid, string courseid)
        {
            var response = new BoolResponse();
            var courseActions = ApiCourseActions;
            var userActions = ApiUserActions;
            var enrollmentActions = ApiEnrollmentActions;

            Models.Course course = null;
            try
            {
                course = courseActions.GetCourseByCourseId(courseid).ToCourse();
            }
            catch (Exception)
            {
                response.status_code = -1;
                response.error_message = "Course not found.";
                return response;
            }

            if (course.Domain == null || String.IsNullOrWhiteSpace(course.Domain.Id))
            {
                response.status_code = -1;
                response.error_message = "Course does not contain domain information.";
                return response;
            }

            var userInfoList = userActions.GetUsers(rauserid: raUserid, domainid: course.Domain.Id);
            if (userInfoList.Count == 0)
            {
                response.status_code = -1;
                response.error_message = String.Format("Agilix user not found for course domain ({0}).", course.Domain.Id);
                return response;
            }
            var userInfo = userInfoList.First();
            Enrollment enrollment = enrollmentActions.GetEnrollment(userInfo.Id, courseid, true).ToEnrollment();
            if (enrollment == null)
            {
                response.status_code = -1;
                response.error_message = "User enrollment not found.";
                return response;
            }

            Context.CourseId = courseid;
            Context.ProductCourseId = course.ParentId;

            Context.CacheProvider.InvalidateEnrollment(userInfo.Id, enrollment.Id, course.Id);
            Context.CacheProvider.InvalidateUserEnrollmentList(userInfo.UserName);
            Context.CacheProvider.InvalidateUserEnrollmentList(userInfo.UserName, course.ProductCourseId ?? course.ParentId);

            response.results = true;
            return response;
        }
	}
}
