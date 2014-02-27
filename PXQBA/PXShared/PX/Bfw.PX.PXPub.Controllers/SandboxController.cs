using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.PX.Abstractions;
using Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using Widget = Bfw.PX.PXPub.Models.Widget;
using BizDC = Bfw.PX.Biz.DataContracts;
using System.Configuration;
using PxWebUser;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    [PerfTraceFilter]
    public class SandboxController : Controller
    {
        protected BizSC.IBusinessContext Context { get; set; }
        
        /// <summary>
        /// course actions
        /// </summary>
        protected BizSC.ICourseActions CourseActions { get; set; }

        /// <summary>
        /// Gets or sets the content actions.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }

        /// <summary>
        /// Enrollment actions
        /// </summary>
        protected BizSC.IEnrollmentActions EnrollmentActions { get; set; }

        /// <summary>
        /// User actions
        /// </summary>
        protected BizSC.IUserActions UserActions { get; set; }


        private PxWebUserRights WebRights;


        public SandboxController(BizSC.ICourseActions courseActions, BizSC.IBusinessContext context, BizSC.IContentActions content, BizSC.IEnrollmentActions enrollmentActions, BizSC.IUserActions userActions)
        {
            CourseActions = courseActions;
            Context = context;
            ContentActions = content;
            EnrollmentActions = enrollmentActions;
            UserActions = userActions;
            WebRights = Context.CurrentUser.WebRights;
        }

        public ActionResult Index()
        {
            //Context.CurrentUser.WebRights = new PxWebUserRights(Context.CurrentUser.Username, Context.Course.ProductCourseId);
            bool IsCourseUpdated = CourseActions.CourseUpdateFlag(Context.Course);
            ViewData["isSandboxCourse"] = Context.Course.IsSandboxCourse;
            ViewData["isCourseUpdated"] = IsCourseUpdated;
            return View("SandboxAlert");

        }
        public ActionResult PublishModal()
        {            
            ViewData["AllowPublish"] = WebRights.AdminTool.AllowPublishCourse;
            //ViewData["AllowPublish"] = true;
            return View("SandboxPublish");
        }
        public ActionResult RevertModal()
        {            
            ViewData["AllowRevert"] = WebRights.AdminTool.AllowRevertCourse;
            //ViewData["AllowRevert"] = true;
            return View("SandboxRevert");
        }


        public ActionResult PublishCourse()
        {
            bool AllowPublishCourse = WebRights.AdminTool.AllowPublishCourse;
            //bool AllowPublishCourse = true;
            if (AllowPublishCourse)
            {
                Context.Course.IsSandboxCourse = false;
                CourseActions.UpdateCourse(Context.Course);

                CourseActions.MergeCourses(Context.Course);

                var productCourseId = Context.Course.ProductCourseId;
                CourseActions.DeleteCourses(new List<BizDC.Course>() { Context.Course });
                Context.EnrollmentId = string.Empty;

                var sandboxCourse = CreateSandboxCourse(productCourseId);

                return RedirectToRoute("IndexDefault", new { courseid = sandboxCourse.Id });
            }
            else
                return RedirectToRoute("EcomEntitled");
        }

        public ActionResult RevertCourse()
        {
            bool AllowRevertCourse = WebRights.AdminTool.AllowRevertCourse;
            //bool AllowRevertCourse = true;
            if (AllowRevertCourse)
            {
                var productCourseId = Context.Course.ProductCourseId;
                CourseActions.DeleteCourses(new List<BizDC.Course>() { Context.Course });
                Context.EnrollmentId = string.Empty;
                var sandboxCourse = CreateSandboxCourse(productCourseId);


                return RedirectToRoute("IndexDefault", new { courseid = sandboxCourse.Id });
            }
            else
                return RedirectToRoute("EcomEntitled");
       }

        /// <summary>
        /// Finds / Creates the Sandbox course and redirect to the course 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditSandboxCourse()
        {
            bool AllowEditSandboxCourse = WebRights.AdminTool.AllowEditSandboxCourse;
            //PX-3928 - Product Course and SandBox Course Should have the same Domain
            if (Context.Product != null && Context.Product.Domain.Id != null)
            {
                if (AllowEditSandboxCourse)
                {
                    var productId = (Context.CourseIsProductCourse) ? Context.Course.Id : Context.ProductCourseId;
                    var sandboxCourse = CreateSandboxCourse(productId);
                    return RedirectToRoute("IndexDefault", new { courseid = sandboxCourse.Id });
                }
            }            
            return RedirectToRoute("EcomEntitled");
        }

        /// <summary>
        /// Create new Sandbox course
        /// </summary>
        /// <returns></returns>
        private BizDC.Course CreateSandboxCourse(string productId)
        {
            var sandboxDomainId = Context.Product.Domain.Id; // System.Configuration.ConfigurationManager.AppSettings["SandboxDomain"];
            BizDC.Course sandboxCourse = CourseActions.FindSandboxCourse((BizDC.CourseType)Enum.Parse(typeof(BizDC.CourseType), Context.Course.CourseType, true), sandboxDomainId, productId);
            string sandboxCourseid;
            if (sandboxCourse != null && !string.IsNullOrEmpty(sandboxCourse.Id) && sandboxCourse.Id != "0")
                sandboxCourseid = sandboxCourse.Id;
            else
            {
                IEnumerable<BizDC.Domain> userDomains = Context.GetRaUserDomains();

                //var content = BaseCourse(sandboxDomainId.ToString());
                var productCourse = CourseActions.GetCourseByCourseId(productId);
                sandboxCourse = CourseActions.CreateDerivedCourse(productCourse, sandboxDomainId);
                sandboxCourse.IsSandboxCourse = true;                                             
                sandboxCourse = CourseActions.UpdateCourses(new List<Bfw.PX.Biz.DataContracts.Course>() { sandboxCourse }).First();
                sandboxCourseid = sandboxCourse.Id;


                if ((userDomains.Where(d => d.Id == sandboxDomainId)).Count() == 0)
                {
                    var user = Context.GetNewUserData();
                    var userInfo2 = UserActions.CreateUser(user.Username, user.Password, user.PasswordQuestion, user.PasswordAnswer,
                        user.FirstName, user.LastName, user.Email, sandboxCourse.Domain.Id, sandboxCourse.Domain.Name, user.ReferenceId);
                }

            }

            /*
            if (string.IsNullOrEmpty(Context.EnrollmentId))
            {
                BizDC.UserInfo userInfo;
            
                if (string.IsNullOrEmpty(Context.CurrentUser.Id) && !string.IsNullOrEmpty(Context.CurrentUser.Username) && !string.IsNullOrEmpty(Context.CurrentUser.Email))
            
                {
                    IEnumerable<BizDC.Domain> userDomains = Context.GetRaUserDomains();
                    if ((userDomains.Where(d => d.Id == sandboxCourse.Domain.Id)).Count() == 0)
                    {
                        var user = Context.GetNewUserData();
                        userInfo = UserActions.CreateUser(user.Username, user.Password, user.PasswordQuestion, user.PasswordAnswer,
                            user.FirstName, user.LastName, user.Email, sandboxCourse.Domain.Id, sandboxCourse.Domain.Name, user.ReferenceId);
                    }
                    else
                        userInfo = UserActions.GetUserByReferenceAndDomainId(Context.CurrentUser.Username, sandboxCourse.Domain.Id);
                    if (userInfo != null)
                        Context.CurrentUser = userInfo;
                }

                if (!string.IsNullOrEmpty(sandboxCourseid) && !string.IsNullOrEmpty(Context.CurrentUser.Id))
                {
                    string enrollmentId = EnrollmentActions.GetUserEnrollmentId(Context.CurrentUser.Id, sandboxCourseid);
                    if (string.IsNullOrEmpty(enrollmentId))
                    {
                        String instructorPermissionFlags = ConfigurationManager.AppSettings["instructorPermissionFlags"];
                        var Enrollment = EnrollmentActions.CreateEnrollments(sandboxCourse.Domain.Id, Context.CurrentUser.Id, sandboxCourse.Id, instructorPermissionFlags, "1", DateTime.Now, DateTime.Now.AddYears(5), string.Empty, string.Empty);
                        enrollmentId = Enrollment.First().Id;
                    }
                    Context.EnrollmentId = enrollmentId;
                }                                
            }
              
             * */

            BizDC.UserInfo userInfo = null;
            BizDC.UserInfo SandBoxUserInfo = UserActions.GetUserByReferenceAndDomainId(Context.CurrentUser.Username, sandboxCourse.Domain.Id);
            if (SandBoxUserInfo == null)
            {
                var user = Context.GetNewUserData();
                userInfo = UserActions.CreateUser(user.Username, user.Password, user.PasswordQuestion, user.PasswordAnswer,
                    user.FirstName, user.LastName, user.Email, sandboxCourse.Domain.Id, sandboxCourse.Domain.Name, user.ReferenceId);
                SandBoxUserInfo = userInfo;
                
            }
            Context.CurrentUser = SandBoxUserInfo;

            string SandBoxCourse_enrollmentId = EnrollmentActions.GetUserEnrollmentId(Context.CurrentUser.Id, sandboxCourseid);
            if (string.IsNullOrEmpty(SandBoxCourse_enrollmentId))
            {
                String instructorPermissionFlags = ConfigurationManager.AppSettings["instructorPermissionFlags"];
                var Enrollment = EnrollmentActions.CreateEnrollments(sandboxCourse.Domain.Id, Context.CurrentUser.Id, sandboxCourse.Id, instructorPermissionFlags, "1", DateTime.Now, DateTime.Now.AddYears(5), string.Empty, string.Empty);
                Context.EnrollmentId = Enrollment.First().Id;
            }
            else
                Context.EnrollmentId = SandBoxCourse_enrollmentId;


            
            return sandboxCourse;
        }
    }
}
