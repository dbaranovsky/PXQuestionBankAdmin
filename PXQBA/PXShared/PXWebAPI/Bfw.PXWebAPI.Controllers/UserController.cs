using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PXWebAPI.Helpers;
using Bfw.PXWebAPI.Helpers.Context;
using Bfw.PXWebAPI.Helpers.Services;
using Bfw.PXWebAPI.Mappers;
using Bfw.PXWebAPI.Models;
using Bfw.PXWebAPI.Models.DTO;
using Bfw.PXWebAPI.Models.Response;
using Adc = Bfw.Agilix.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Course = Bfw.PX.Biz.DataContracts.Course;
using Enrollment = Bfw.PXWebAPI.Models.Enrollment;

namespace Bfw.PXWebAPI.Controllers
{
	/// <summary>
	/// UserController
	/// </summary>
	public class UserController : ApiController
	{
		protected ISessionManager SessionManager { get; set; }

		protected IBusinessContext Context { get; set; }

		/// <summary>
		/// Gets  the Api Enrollment Actions.
		/// </summary>
		/// <value>
		/// The API Enrollment actions.
		/// </value>
		protected IApiEnrollmentActions ApiEnrollmentActions { get; set; }

        /// <summary>
        /// Gets  the Api Enrollment Actions.
        /// </summary>
        /// <value>
        /// The API Enrollment actions.
        /// </value>
        protected ICourseActions CourseActions { get; set; }

        /// <summary>
        /// Gets  the Api Enrollment Actions.
        /// </summary>
        /// <value>
        /// The API Enrollment actions.
        /// </value>
        protected IEnrollmentActions EnrollmentActions { get; set; }

		/// <summary>
		/// Gets  the Api User Actions.
		/// </summary>
		/// <value>
		/// The API User actions.
		/// </value>
		protected IApiUserActions ApiUserActions { get; set; }

        /// <summary>
        /// Gets  the Api Domain Actions.
        /// </summary>
        /// <value>
        /// The API Domain actions.
        /// </value>
        protected IApiDomainActions ApiDomainActions { get; set; }

		/// <summary>
		/// Gets  the Api Course Actions.
		/// </summary>
		/// <value>
		/// The API Course actions.
		/// </value>
		protected IApiCourseActions ApiCourseActions { get; set; }

        ///// <summary>
        ///// Wrapper for the HttpContext.Current object, so it can be mocked
        ///// </summary>
        protected IHttpContextAdapter HttpContextAdapter { get; set; }

		/// <summary>
		/// A const for that holds the instructor permission flags.
		/// </summary>
		private readonly string INSTRUCTOR_FLAGS = ConfigurationManager.AppSettings["InstructorPermissionFlags"];

	    /// <summary>
	    /// UserController
	    /// </summary>
	    /// <param name="sessionManager"></param>
	    /// <param name="context"> </param>
	    /// <param name="apiEnrollmentActions"> </param>
	    /// <param name="apiCourseActions"> </param>
	    /// <param name="apiUserActions"> </param>
	    public UserController(ISessionManager sessionManager, BizSC.IBusinessContext context, IApiEnrollmentActions apiEnrollmentActions,
	        IApiCourseActions apiCourseActions,IApiUserActions apiUserActions,IApiDomainActions apiDomainActions,
            IHttpContextAdapter httpContextAdapter, ICourseActions courseActions, IEnrollmentActions enrollmentActions)
	    {
	        SessionManager = sessionManager;
	        Context = context;
	        ApiCourseActions = apiCourseActions; //new ApiCourseActions(sessionManager, context);
	        ApiEnrollmentActions = apiEnrollmentActions; //new ApiEnrollmentActions(sessionManager, context);
	        ApiUserActions = apiUserActions;
	        ApiDomainActions = apiDomainActions;
	        HttpContextAdapter = httpContextAdapter;
	        CourseActions = courseActions;
	        EnrollmentActions = enrollmentActions;
	    }

	    //TODO: Put meaningful error messages and codes for all the responses. (SK)
		#region "Check and Create User and Enrollment"

		/// <summary>
		/// Check and Create User and Enrollment
		/// with following list of Post Parameters:
		/// rauserid, domainid, email, firstname, lastname, courseid, domainname, isinstructor, enrollmentstatus,  startdate, enddate,
		/// WHERE rauserid, domainid, email, firstname, lastname, courseid - Required.
		/// </summary>
		/// <returns>UserEnrollmentResponse</returns>
		[HttpPost]
		[ActionName("CheckandCreateUserandEnrollment")]
		public UserEnrollmentResponse CheckandCreateUserandEnrollment()
		{
			var response = new UserEnrollmentResponse();
			var context = HttpContext.Current;
			var postParams = Bfw.PXWebAPI.Helpers.ApiHelper.GetPostParameters(context.Request);
			//TODO: - Remove hardcoded text from here (SK)
			var rauserid = postParams.ContainsKey("rauserid") ? postParams["rauserid"] : "";
			var domainid = postParams.ContainsKey("domainid") ? postParams["domainid"] : "";
			var email = postParams.ContainsKey("email") ? postParams["email"] : "";
			var firstname = postParams.ContainsKey("firstname") ? postParams["firstname"] : "";
			var lastname = postParams.ContainsKey("lastname") ? postParams["lastname"] : "";
			var courseid = postParams.ContainsKey("courseid") ? postParams["courseid"] : "";
			if (String.IsNullOrEmpty(rauserid) || String.IsNullOrEmpty(domainid) || String.IsNullOrEmpty(email) || String.IsNullOrEmpty(firstname) || String.IsNullOrEmpty(lastname) || String.IsNullOrEmpty(courseid))
			{
				response.error_message = "Required parameters are missing";
				return response;
			}
			var domainname = postParams.ContainsKey("domainname") ? postParams["domainname"] : "";
			var isinstructor = postParams.ContainsKey("isinstructor") && Convert.ToBoolean(postParams["isinstructor"]);
			var enrollmentstatus = postParams.ContainsKey("enrollmentstatus") ? postParams["enrollmentstatus"] : "";
			var startdate = postParams.ContainsKey("startdate") ? postParams["startdate"] : "";
			var enddate = postParams.ContainsKey("enddate") ? postParams["enddate"] : "";

			var user = CheckandCreateUser(rauserid, domainid, email, firstname, lastname, domainname).results;
			if (user != null)
			{
				var enrollment = CheckandCreateUserEnrollment(user.Id, courseid, domainid, isinstructor,
																 enrollmentstatus,
																 startdate, enddate).results;

				if (enrollment != null)
				{
					var userenrollment = enrollment.ToUserEnrollment();
					userenrollment.User = user.ToBaseUser();
					response.results = userenrollment;
					return response;
				}
			}


			response.error_message = "No results found";
			return response;
		}
		#endregion

		#region "Check and Create User"
		/// <summary>
		/// Check and Create User by rauserid, domainid, email, firstname, lastname, domainname, where domainname is optional
		/// </summary>
		/// <param name="rauserid"></param>
		/// <param name="domainid"></param>
		/// <param name="email"></param>
		/// <param name="firstname"></param>
		/// <param name="lastname"></param>
		/// <param name="domainname"></param>
		/// <returns>UserResponse</returns>
		[HttpGet]
		[ActionName("CheckandCreateUser")]
		public UserResponse CheckandCreateUser(string rauserid, string domainid, string email, string firstname,
											 string lastname, string domainname = "")
		{
			var response = new UserResponse();

			var usrs = GetUsers(rauserid, domainid).results;
			if (usrs.IsNullOrEmpty())
				return CreateUser(rauserid, domainid, email, firstname, lastname, domainname);
			response.results = usrs.First();
			return response;

		}

		/// <summary>
		/// Get Users by rauserid, domainid, userid, where all parameters are optional
		/// </summary>
		/// <param name="rauserid"></param>
		/// <param name="domainid"></param>
		/// <param name="userid"></param>
		/// <returns>UserListResponse</returns>
		[HttpGet]
		[ActionName("Users")]
		public UserListResponse GetUsers(string rauserid = "", string domainid = "", string userid = "")
		{
			var response = new UserListResponse();
			
            rauserid = rauserid ?? "";
            domainid = domainid ?? "";
			userid = userid ?? "";

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
				var usrs = ausrs.Select(agilixUser => agilixUser.ToUser()).ToList();
				response.results = usrs;
				return response;
			}

			response.error_message = "No results found";
			return response;
		}

		/// <summary>
		/// Get Px User Details by userId
		/// </summary>
		/// <param name="id"></param>
		/// <returns>UserResponse</returns>
		[HttpGet]
		[ActionName("PxUserDetails")]
		public UserResponse PxUserDetails(string id)
		{
			var response = new UserResponse();

			var users = GetUsers(userid: id);
			if (!users.results.IsNullOrEmpty())
			{
				response.results = users.results.First();
				return response;
			}

			response.error_message = "No results found";
			return response;
		}

		/// <summary>
		/// Get List of User From All Domains by userId and optionally byreference.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="byreference"></param>
		/// <returns>UserListResponse</returns>
		[HttpGet]
		[ActionName("Details")]
		public UserListResponse Details(string id, bool byreference = false)
		{
			var response = new UserListResponse();

			var userActions = ApiUserActions;
			var users = userActions.GetUsersFromAllDomains(id, byreference);
			response.results = users.Map(x => x.ToUser()).ToList();

			return response;
		}

		/// <summary>
		/// Create User by rauserid, domainid, email, firstname, lastname and optional domainname 
		/// </summary>
		/// <param name="rauserid"></param>
		/// <param name="domainid"></param>
		/// <param name="email"></param>
		/// <param name="firstname"></param>
		/// <param name="lastname"></param>
		/// <param name="domainname"></param>
		/// <returns>UserResponse</returns>
		[HttpGet]
		[ActionName("CreateUser")]
		public UserResponse CreateUser(string rauserid, string domainid, string email, string firstname, string lastname,
									 string domainname = "")
		{
            var response = new UserResponse();
            var cmd = new CreateUsers();

            Adc.Credentials credentials = null;
            credentials = new Adc.Credentials { Username = rauserid, Password = ConfigurationManager.AppSettings["BrainhoneyDefaultPassword"] };

            var domain = new Adc.Domain { Id = domainid, Name = String.IsNullOrEmpty(domainname) ? "root" : domainname };
            var pxUser = new Adc.AgilixUser
            {
                Domain = domain,
                Credentials = credentials,
                Email = email,
                FirstName = firstname,
                LastName = lastname,
                Reference = null
            };
            cmd.Add(pxUser);

            SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

            if (!cmd.Users.IsNullOrEmpty())
            {
                //Return only if the userid was updated and hence is not empty
                //Also, call the get user method to load all the data for the user to send it back.
                if (!string.IsNullOrEmpty(cmd.Users.First().Id))
                {
                    response.results = GetUsers(userid: cmd.Users.First().Id).results.First();
                    return response;
                }
            }

            response.error_message = "No results found";
            return response;
		}

		#endregion

		#region "Check and Create Enrollment"

	    /// <summary>
	    /// Drop user enrollment
	    /// </summary>
	    /// <param name="raUserid"></param>
	    /// <param name="courseid"></param>
	    /// <returns>EnrollmentResponse</returns>
	    [HttpPost]
	    [ActionName("DropUserEnrollment")]
	    public BoolResponse DropUserEnrollment(string raUserid, string courseid)
	    {
	        var response = new BoolResponse();
	        var courseActions = ApiCourseActions;
	        var userActions = ApiUserActions;
	        //var enrollmentActions = ApiEnrollmentActions;

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
	        var enrollments = ApiEnrollmentActions.GetEnrollments(userInfo.Id, courseid, true);
	        if (enrollments == null)
	        {
	            response.status_code = -1;
	            response.error_message = "User enrollment not found.";
	            return response;
	        }

            Context.CourseId = courseid;
            Context.ProductCourseId = course.ProductCourseId;

	        foreach (var enrollment in enrollments)
	        {
	            if (!ApiEnrollmentActions.DropEnrollment(enrollment.ToEnrollment()))
	            {
	                response.status_code = -1;
	                response.error_message = "Error dropping enrollment.";
	                return response;
	            }
	            else
	            {
                    //deactivate cache on dropping of enrollment
                    Context.CacheProvider.InvalidateEnrollment(userInfo.Id, enrollment.Id, course.Id);	                
	            }
	        }

	        //deactivate cache on dropping of enrollment
            Context.CacheProvider.InvalidateUserEnrollmentList(userInfo.UserName);
            Context.CacheProvider.InvalidateUserEnrollmentList(userInfo.UserName, course.ProductCourseId);

	        response.results = true;
            return response;
        }

	    /// <summary>
		/// Change Course Enrollment by userid, courseid and switchEnrollFromCourse
		/// </summary>
		/// <param name="userid"></param>
		/// <param name="courseid"></param>
		/// <param name="switchEnrollFromCourse"></param>
		/// <returns>EnrollmentResponse</returns>
		[HttpGet]
		[ActionName("ChangeCourseEnrollment")]
		public EnrollmentResponse ChangeCourseEnrollment(string userid, string courseid, string switchEnrollFromCourse)
		{
			var response = new EnrollmentResponse();
			var courseActions = ApiCourseActions;
			var userActions = ApiUserActions;
			//var enrollmentActions = ApiEnrollmentActions;
			if (!string.IsNullOrEmpty(switchEnrollFromCourse))
			{
				var originalCourse = courseActions.GetCourseByCourseId(switchEnrollFromCourse);
				var newUserInfo = userActions.GetUsers(userid: userid).First();
				var userToSwitch = userActions.GetUsers(newUserInfo.Reference, originalCourse.Domain.Id).First();
				Enrollment enrollment = null;

				if (userToSwitch != null)
				{
					Enrollment originalEnrollment = ApiEnrollmentActions.GetEnrollment(userToSwitch.Id, switchEnrollFromCourse).ToEnrollment();
					enrollment = new Enrollment
									{
										Id = originalEnrollment.Id,
										User = newUserInfo.ToUser(),
										CourseId = courseid, // new course id
										Flags = originalEnrollment.Flags,
										StartDate = DateTime.Now, //originalEnrollment.StartDate,
										EndDate = DateTime.Now.AddYears(1), //originalEnrollment.EndDate,
										OverallGrade = originalEnrollment.OverallGrade,
										PercentGraded = originalEnrollment.PercentGraded,
										Status = originalEnrollment.Status,
										Reference = originalEnrollment.Reference
									};
				}

				if (!ApiEnrollmentActions.UpdateEnrollment(enrollment))
				{
					response.status_code = -1;
					response.error_message = "Error updating enrollment.";
				}

				if (enrollment != null) response.results = ApiEnrollmentActions.GetEnrollment(enrollment.Id).ToEnrollment();
			}
			return response;
		}

		/// <summary>
		/// Check and Create User Enrollment by userid, courseid, domainid 
		/// and optional parameters isInstructor,  enrollmentstatus, startdate, enddate
		/// </summary>
		/// <param name="userid"></param>
		/// <param name="courseid"></param>
		/// <param name="domainid"></param>
		/// <param name="isInstructor"></param>
		/// <param name="enrollmentstatus"></param>
		/// <param name="startdate"></param>
		/// <param name="enddate"></param>
		/// <returns>EnrollmentResponse</returns>
		[HttpGet]
		[ActionName("CheckandCreateUserEnrollment")]
		public EnrollmentResponse CheckandCreateUserEnrollment(string userid, string courseid, string domainid,
													   bool isInstructor = false, string enrollmentstatus = "",
													   string startdate = "", string enddate = "")
		{
			var response = new EnrollmentResponse();

			var crseEnrollments = GetUserEnrollments(userid, courseid).results;
			if (crseEnrollments.IsNullOrEmpty())
				return CreateUserEnrollment(userid, courseid, domainid, isInstructor, enrollmentstatus, startdate,
											enddate);
			response.results = crseEnrollments.First();
			return response;

		}

		/// <summary>
		/// Get User Enrollments by userid and optional courseid 
		/// </summary>
		/// <param name="userid"></param>
		/// <param name="courseid"></param>
		/// <returns>EnrollmentListResponse</returns>
		[HttpGet]
		[ActionName("GetUserEnrollments")]
		public EnrollmentListResponse GetUserEnrollments(string userid, string courseid = "")
		{
			var response = new EnrollmentListResponse();

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
			if (!aenrollments.IsNullOrEmpty())
			{
				var enrollments = aenrollments.Select(enrollment => enrollment.ToEnrollment()).ToList();
				response.results = enrollments;
				return response;
			}

			response.error_message = "No results found";
			return response;
		}

		/// <summary>
		/// Get Enrollments by id and optionally byreference and domainId
		/// </summary>
		/// <param name="id"></param>
		/// <param name="byreference"></param>
		/// <param name="domainId"></param>
		/// <returns>UserEnrollmentListResponse</returns>
		[HttpGet]
		[ActionName("Enrollments")]
		public UserEnrollmentListResponse Enrollments(string id, bool byreference = false, string domainId = "")
		{
			var response = new UserEnrollmentListResponse();

			var userActions = ApiUserActions;
			var enrollments = userActions.GetEnrollments(id, byreference, domainId);

			if (enrollments != null)
			{
				var courseActions = ApiCourseActions;
				foreach (var enrollment in enrollments)
				{
					enrollment.Course = courseActions.GetCourseByCourseId(enrollment.Course.Id);
				}
				response.results = enrollments.Map(x => x.ToUserEnrollmentDto()).ToList();
			}
			return response;

		}

		/// <summary>
		/// Get Enrollment by id
		/// new route: dev.px.bfwpub.com/api/user/enrollment/details/97640     
		/// existing route: dev.px.bfwpub.com/api/user/enrollment/97640                    
		/// </summary>
		/// <param name="id"></param>
		/// <returns>UserEnrollmentDetailResponse</returns>
		[HttpGet]
		[ActionName("Enrollment")]
		public UserEnrollmentDetailResponse Enrollment(string id)
		{
			var response = new UserEnrollmentDetailResponse();

			//var actions = ApiEnrollmentActions;
			var enrollment = ApiEnrollmentActions.GetEnrollment(id);
			if (enrollment != null)
			{
				var userActions = ApiUserActions;
				enrollment.User = userActions.GetUsers(userid: enrollment.User.Id).First();
                // This is to get Domain name, by default we don't get domain name with user object.
			    if (enrollment.User.Domain != null && !string.IsNullOrWhiteSpace(enrollment.User.Domain.Id))
			    {
                    enrollment.User.Domain = ApiDomainActions.GetDomainById(enrollment.User.Domain.Id);
			    }
				var courseActions = ApiCourseActions;
				enrollment.Course = courseActions.GetCourseByCourseId(enrollment.Course.Id);
				response.results = enrollment.ToUserEnrollmentDto();
			}
			return response;

		}

	    /// <summary>
	    ///  Create User Enrollment by userid, courseid, domainid and optional parameters:
	    ///  isinstructor,  enrollmentstatus, startdate , enddate 
	    /// </summary>
	    /// <param name="userid"></param>
	    /// <param name="courseid"></param>
	    /// <param name="domainid"></param>
	    /// <param name="isinstructor"></param>
	    /// <param name="enrollmentstatus"></param>
	    /// <param name="startdate"></param>
	    /// <param name="enddate"></param>
	    /// <returns>EnrollmentResponse</returns>
	    [HttpGet]
	    [ActionName("CreateUserEnrollment")]
	    public EnrollmentResponse CreateUserEnrollment(string userid, string courseid, string domainid,
	        bool isinstructor = false, string enrollmentstatus = "",
	        string startdate = "", string enddate = "")
	    {
	        var response = new EnrollmentResponse();
	        
            var strStartDate = String.IsNullOrEmpty(startdate)? DateTime.Now.ToString(CultureInfo.InvariantCulture): startdate;
	        var strEndDate = String.IsNullOrEmpty(enddate)? DateTime.Now.AddYears(1).ToString(CultureInfo.InvariantCulture): enddate;
	        var strEnrollmentStatus = String.IsNullOrEmpty(enrollmentstatus)? "1": enrollmentstatus;
	        var course = CourseActions.GetCourseByCourseId(courseid);
            var user = ApiUserActions.GetUsers(userid:userid).FirstOrDefault();

	        Context.Course = course;
	        Context.CurrentUser = new UserInfo()
	        {
	            Id = courseid,
	            FirstName = user.FirstName,
	            LastName = user.LastName,
	            Username = user.UserName,
	            ReferenceId = user.Reference,
	            Email = user.Email

	        };

	        Context.CourseId = course.Id;
	        Context.ProductCourseId = course.ToCourse().ProductCourseId;

	        if (isinstructor)
	        {
	            CourseActions.EnrollCourses(new List<Course>() {new Course() {Id = courseid}}, userid);
	        }
	        else
	        {
	            EnrollmentActions.CreateEnrollments(domainid, userid, courseid, ConfigurationManager.AppSettings["StudentPermissionFlags"],
	                                                strEnrollmentStatus, DateTime.Parse(strStartDate), DateTime.Parse(strEndDate), string.Empty, 
                                                    string.Empty, true);
	        }

	        var enrollments = GetUserEnrollments(userid, courseid);
	        if (enrollments != null && enrollments.results != null)
	        {
	            response.results = enrollments.results.FirstOrDefault();
	            return response;
	        }
            
            response.error_message = "No results found";
            return response;
		}

		#endregion

		#region Update User

		/// <summary>
		/// Update User with the following parameters:
		/// [FromUri] id, [FromUri] byreference, [FromBody]EditUser user(FirstName, LastName, Email)
		/// </summary>
		/// <param name="id"></param>
		/// <param name="byreference"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		[HttpPost]
		public BoolResponse Update([FromUri]string id, [FromUri]bool byreference, [FromBody]EditUser user)
		{
            var response = new BoolResponse()
            {
                error_message = "Error",
                status_code = -1,
                results = false
            };

            if (ApiUserActions.UpdateUsers(id, byreference, user))
            {
                response = new BoolResponse { results = true };
            }

		    return response;
		}

		#endregion

		#region "Get Courses for an Instructor"

		/// <summary>
		/// Get Courses for an Instructor by id and list of productcourseids(separated with '|') as Post Parameter
		/// Example to test: User/CoursesForInstructor/113371  POST:productcourseids=109554
		/// </summary>
		/// <param name="id"></param>
		/// <returns>CourseListResponse</returns>
		[HttpPost]
		[ActionName("CoursesForInstructor")]
        public CourseListResponse GetCoursesForInstructor(string id)
		{
			var response = new CourseListResponse();
            var productcourseids = HttpContextAdapter.Current.Request.Form["productcourseids"];
			if (productcourseids.Trim().Equals(string.Empty))
			{
				response.error_message = "Required parameters are missing";
				return response;
			}
			var productcourseidlist = productcourseids.Split('|');
			var sbQuery = new StringBuilder();
			const string orOperator = " OR ";
			//Get the parent course id field to filter against
			var productCourseIdSearchField = System.Configuration.ConfigurationManager.AppSettings["ProductCourseIdSearchField"];
			productCourseIdSearchField = !string.IsNullOrEmpty(productCourseIdSearchField) ? productCourseIdSearchField : "SOURCEID";
			foreach (var productcourseid in productcourseidlist)
			{
				sbQuery.Append(orOperator + "/" + productCourseIdSearchField + "='" + productcourseid
							   + "'");
			}
			if (sbQuery.Length > 0)
			{
				var finalQuery = sbQuery.ToString().Substring(orOperator.Length);
				var courseActions = ApiCourseActions;
				var userActions = ApiUserActions;
				//Get the enrollments for this user where this user has the course owner rights (INSTRUCTOR) and the courses sourceid is in the product course ids sent
				var userenrollments = userActions.GetUserEnrollmentsByQueryAndFlags(id, finalQuery, INSTRUCTOR_FLAGS);
				var resultcourses = new List<CourseDto>();
				if (!userenrollments.IsNullOrEmpty())
				{
					resultcourses.AddRange(from userenrollment in userenrollments select courseActions.GetCourseByCourseId(userenrollment.Course.Id) into enrcourse where enrcourse != null select enrcourse.ToCourseDto());

					if (!resultcourses.IsNullOrEmpty())
					{
						response.results = resultcourses;
						return response;
					}
				}
			}

			//TODO: Remove hardcoded text from here (SK)
			response.error_message = "No results found";
			return response;
		}
		#endregion

		#region "Get User packages"

		/// <summary>
		/// Get User packages by  rauserid and raemail
		/// </summary>
		/// <param name="rauserid"></param>
		/// <param name="raemail"></param>
		/// <returns>UserPackagesResponse</returns>
		[HttpGet]
		[ActionName("Packages")]
		public UserPackagesResponse GetPackages(string rauserid, string raemail)
		{
			CoreServices coreServices = new CoreServices();
			UserPackagesResponse response = new UserPackagesResponse();

			var packageListResponse = coreServices.GetUserPacakges(rauserid, raemail);
			if (packageListResponse != null)
			{
				if (packageListResponse.Error.Code == "0")
				{
					if (!packageListResponse.PackageSiteInfoList.PackageSiteInfoL.IsNullOrEmpty())
					{
						response.results = packageListResponse.PackageSiteInfoList.PackageSiteInfoL;
						return response;
					}
				}
			}

			//TODO: Remove hardcoded text from here (SK)
			response.error_message = "No results found";
			return response;
		}

		#endregion
	}
}
