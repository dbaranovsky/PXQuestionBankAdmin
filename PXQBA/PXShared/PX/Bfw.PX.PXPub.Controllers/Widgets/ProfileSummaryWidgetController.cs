using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Text;
using System.Configuration;
using System.Text.RegularExpressions;
using Bfw.PX.Abstractions;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.PXPub.Components;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Controllers.Contracts;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    [PerfTraceFilter]
	public class ProfileSummaryWidgetController : Controller, IPXWidget
	{
        /// <summary>
        /// Context
        /// </summary>
		private BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// User Actions
        /// </summary>
		private BizSC.IUserActions UserActions { get; set; }


        private BizSC.IEnrollmentActions EnrollmentActions { get; set; }        

        /// <summary>
        /// Exposes the related functions for 'Course Sharing' functionality across PX
        /// </summary>
        protected BizSC.ISharedCourseActions SharedCourseActions { get; set; }

        /// <summary>
        /// Gets or sets the content helper.
        /// </summary>
        /// <value>
        /// The content helper.
        /// </value>
        protected IContentHelper ContentHelper { get; set; }

		protected BizSC.IContentActions ContentActions { get; set; }

        public ProfileSummaryWidgetController(BizSC.IBusinessContext context, BizSC.IUserActions userActions, BizSC.IEnrollmentActions enrollmentActions,
            BizSC.ISharedCourseActions sharedCourseActions, IContentHelper helper, BizSC.IContentActions contentActions)
		{
			Context = context;
			UserActions = userActions;
            EnrollmentActions = enrollmentActions;            
            SharedCourseActions = sharedCourseActions;
            ContentHelper = helper;
			ContentActions = contentActions;
		}

		/// <summary>
		/// summary
		/// </summary>
		/// <returns></returns>
        public ActionResult Summary(Models.Widget widget)
		{
			var data = new ProfileSummaryWidget();
			data.ProfileToEdit = Context.CurrentUser.Id;

		    List<UserInfo> uiList = UserProfileMapper.ToUserProfile(UserActions.LoadUserProfile()).UsersSummaryList;

		    var enrollments = EnrollmentActions.GetEntityEnrollments(Context.EntityId);

		    foreach (var user in uiList)
		    {
		        var userEnrollment = enrollments.FirstOrDefault(i => i.User.Id == user.Id);
		        user.EnrollmentIdForCurrentCourse = userEnrollment.Id;
		    }

		    data.UserProfile = (from c in uiList where c.Id == Context.CurrentUser.Id select c).FirstOrDefault();

		    if (data.UserProfile == null)
            {
                var courseOwnerId = !string.IsNullOrWhiteSpace(Context.Course.CourseOwner) ? Context.Course.CourseOwner : Context.CurrentUser.Id; 
                data.UserProfile = UserActions.GetUser(courseOwnerId);
            }

            data.UserProfile.FirstName = data.UserProfile.FirstName.Replace("'", "^$#!").Replace(@"""", "^$#~");
            data.UserProfile.LastName = data.UserProfile.LastName.Replace("'", "^$#!").Replace(@"""", "^$#~");
            data.UserProfile.Email = data.UserProfile.Email.Replace("'", "^$#!").Replace(@"""", "^$#~");
			data.UserProfile.AvatarUrl = UserActions.GetUserAvatarUrl(data.UserProfile);

		    data.UsersSummaryList = null;
		    if (widget.ListStudents)
		    {
		        data.UsersSummaryList = uiList.FindAll(c => c.Id != Context.CurrentUser.Id);
		    }
            data.CourseType = Context.Course.CourseType;
		    data.AccessLevel = Context.AccessLevel;
			ViewData.Model = data;
            ViewData["CourseID"] = Context.EntityId;
            ViewData["DashboardID"] = Context.DashboardCourseId;
            ViewData["IsStudentView"] = Context.ImpersonateStudent.ToString().ToLowerInvariant();

            if (Context.AccessLevel == BizSC.AccessLevel.Student)
            {
                ViewData["DashboardID"] = Context.StudentDashboardId;
            }

            if (ViewData["DashboardID"] == null)
            { 
                if (Request["dID"] != "" && Request["dID"] != null) {
                    ViewData["DashboardID"] = Request["dID"].ToString();
                }  
            }

            ViewData["IsReadOnly"] = Context.IsCourseReadOnly;

			return View();
		}

		/// <summary>
		/// 	 on edit
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="userRefId"></param>
		/// <param name="firstName"></param>
		/// <param name="lastName"></param>
		/// <param name="email"></param>
		/// <param name="imageUrl"></param>
		/// <returns></returns>
		public ActionResult Edit(string userId, string userRefId, string firstName, string lastName, string email, string imageUrl) 
		{
			var data = new ProfileSummaryWidget();
		  	
			UserInfo userProfile = new UserInfo();
			userProfile.Id = userId;
			userProfile.ReferenceId = userRefId;
            userProfile.FirstName = firstName.Replace("^$#!", "'").Replace("^$#~", @"""");
            userProfile.LastName = lastName.Replace("^$#!", "'").Replace("^$#~", @"""");
            userProfile.Email = email.Replace("^$#!", "'").Replace("^$#~", @"""");
			userProfile.AvatarUrl = imageUrl;

			data.UserProfile = userProfile;
            data.AccessLevel = Context.AccessLevel;

			ViewData.Model = data;
			return View("Edit", data);
		}

		/// <summary>
		///  on save
		/// </summary>
		/// <returns></returns>
        public ActionResult Save(string userId, string userRefId)
		{
		    string currentUserName = Context.CurrentUser.Username;
            string email = Request.Form.Get("em");
            string firstname = Request.Form.Get("fn");
            string lastname = Request.Form.Get("ln");
		    Stream file = null;
            int profileErrorCode;

            if (Request.Files.Count > 0)
            {
				if (Request.Files["docFile"] != null && !Request.Files["docFile"].FileName.IsNullOrEmpty())
                {
                    string filetype = (Request.Files.Count > 0) ? Request.Files["docFile"].ContentType.Split('/')[1] : string.Empty;

                    file = Request.Files["docFile"].InputStream;

					//save the profile image as a Brainhoney resource
					var filePath = string.Format("{0}/{1}_profile_photo.{2}", ConfigurationManager.AppSettings["ProfileImageFolder"], Context.CurrentUser.Id, filetype);
					Resource profileResource = new Resource()
					{
						ContentType = filetype,
						EntityId = ConfigurationManager.AppSettings["ProfileImageEntity"],
						Url = filePath
					};
					var profileStream = profileResource.GetStream();
					file.CopyTo(profileStream);
					//store the image
					ContentActions.StoreResources(new List<Resource> { profileResource });
                }
            }

			UserProfileResponse  response = UserActions.SetUserProfileInfo(userId, userRefId, email, firstname, lastname);
            if (response.Error != null)
            {
                bool hasError = int.TryParse(response.Error.Code, out profileErrorCode);
                if (profileErrorCode != 0)
                {
                    if(hasError)
                    {
                        ViewData["Error"] = string.Format("Error Code: {0} {1}", response.Error.Code, response.Error.Message);
                    }
                }
            }

            var data = new ProfileSummaryWidget();

            data.ProfileToEdit = Context.CurrentUser.Id;
      
            List<UserInfo> uiList = UserProfileMapper.ToUserProfile(UserActions.LoadUserProfile()).UsersSummaryList;

            UserActions.invalidateUser(userRefId);

            data.UserProfile = (from c in uiList where c.Id == Context.CurrentUser.Id select c).FirstOrDefault();
            data.UsersSummaryList = uiList.FindAll(c => c.Id != Context.CurrentUser.Id);
            data.AccessLevel = Context.AccessLevel;
            ViewData.Model = data;
			data.UserProfile.AvatarUrl = UserActions.GetUserAvatarUrl(data.UserProfile);
		    return View("Summary");

		}

		/// <summary>
		/// default
		/// </summary>
		/// <returns></returns>
        public ActionResult ViewAll(Models.Widget model)
		{
			return View();
		}
	
        /// <summary>
        /// Returns the student profile widget
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public ActionResult StudentProfile(string category)
        {
            var userinfo = new UserInfo();
            int sharedItemCount = 0;
            var courseId = Context.CourseId;

            if (!string.IsNullOrEmpty(category))
            {
                var enrollmentId = Regex.Split(category, "enrollment_").ToList().Last();
                var userid = SharedCourseActions.GetUserId(enrollmentId, Context.EntityId);

                var userInfoList = UserProfileMapper.ToUserProfile(UserActions.LoadUserProfile()).UsersSummaryList;
                if (!userInfoList.IsNullOrEmpty() && userInfoList.Exists(u => u.Id == userid))
                {
                    userinfo = userInfoList.Find(u => u.Id == userid);
                }

                if (Context.IsSharedCourse)
                {
                    sharedItemCount = SharedCourseActions.getSharedItems(enrollmentId).Count;
                    var sharedCourseDefnition = SharedCourseActions.getSharedCourseDef(enrollmentId, Context.CourseId).ToSharedCourseDefinition();
                    if (!string.IsNullOrEmpty(sharedCourseDefnition.AnonyousName))
                    {
                        ViewData["anonymousName"] = sharedCourseDefnition.AnonyousName;
                    }
                }
                else
                {
                    sharedItemCount = GetEportfolioItemCount(enrollmentId, courseId);
                }
            }

            ViewData["category"] = category;
            ViewData["StudentItemCount"] = sharedItemCount;
            ViewData.Model = userinfo;
            
            return View();
        }

        /// <summary>
        /// Gets the count of eportfolio items tagged to a student
        /// </summary>
        /// <returns></returns>
        private int GetEportfolioItemCount(string enrollmentId, string courseId)
        {
            var root = "PX_COURSE_EPORTFOLIO_ROOT_ITEM";
            enrollmentId = string.Format("enrollment_{0}", enrollmentId);
            var tocs = ContentHelper.LoadTocWithAllChild(courseId, root, enrollmentId).ToList();
            return GetEportfolioItemList(tocs).Count();
        }

        /// <summary>
        /// Gives the flattened toc items list from a toc item tree
        /// </summary>
        /// <param name="tocs"></param>
        /// <returns></returns>
        private IEnumerable<TocItem> GetEportfolioItemList(IEnumerable<TocItem> tocs)
        {
            var flatToc = new List<TocItem>();
            flatToc = tocs.ToList();

            foreach (var toc in tocs)
            {
                flatToc.AddRange(GetEportfolioItemList(toc.Children.ToList()));
            }

            return flatToc;
        }
    }


}
