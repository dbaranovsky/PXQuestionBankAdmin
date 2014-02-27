using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Configuration;

using Bfw.PX.Abstractions;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;

using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

using BFW.RAg;

namespace Bfw.PX.PXPub.Controllers
{
    public class RALoginController : Controller
    {
        /// <summary>
        /// IUserActions implementation to use
        /// </summary>
        protected BizSC.IUserActions UserActions { get; set; }
        protected BizSC.IEnrollmentActions EnrollmentActions { get; set; }
        protected BizSC.ICourseActions CourseActions { get; set; }
        protected BizSC.IBusinessContext BusinessContext { get; set; }

        public RALoginController(BizSC.IUserActions userActions, BizSC.IEnrollmentActions enrollmentActions, BizSC.ICourseActions courseActions, BizSC.IBusinessContext bizContext)
        {
            string RABaseUrl = ConfigurationSettings.AppSettings["RABaseUrl"];
            //, "/BFWglobal/js/jquery/jquery-1.3.2.min.js"
            string[] scripts = new string[] { "/BFWglobal/js/json2.js", "/BFWglobal/js/jquery/jquery.cookie.js", "/BFWglobal/js/global.js", "/BFWglobal/js/BFW_LogError.js", "/BFWglobal/js/BFW_Error.js", "/BFWglobal/RAg/v2.0/RA.js", "/BFWglobal/RAg/v2.0/RAWS.js", "/BFWglobal/RAg/v2.0/RAif.js"};
            string raCss = ConfigurationSettings.AppSettings["RABaseUrl"] + "/BFWglobal/RAg/v2.0/css/RAif.css";

            for (int i = 0; i < scripts.Length; i++ )            
                scripts[i] = RABaseUrl + scripts[i];

            UserActions = userActions;
            EnrollmentActions = enrollmentActions;
            CourseActions = courseActions;
            BusinessContext = bizContext;

            // -- TODO ---- how to get it from quesry string 
            //ViewData["courseid"] = Request.QueryString["courseid"];

            ViewData["courseid"] = "74";
            ViewData["scripts"] = scripts;
            ViewData["RAcss"] = raCss;
        }

        public ActionResult RALogin()
        {
            var model = new RALogin();
            ViewData.Model = model;
            return View();
        }      

        public ActionResult RAStudent()
        {
            ActionResult aResult = null;
            BFW.RAg.Session ragSession = new Session();
            Boolean HaveSession = ragSession.Check();
        
            if (string.IsNullOrEmpty(ViewData["courseid"].ToString()))
            {
                var model = new RALogin();
                ViewData.Model = model;
                aResult = View();
            }
            else
            {                
                if (HaveSession)
                {
                    if (ragSession.CurrentUser.CurrentSiteLogin.LevelOfAccess == 20)
                    {
                        var model = new RALogin();
                        ViewData["accessType"] = ragSession.CurrentUser.CurrentSiteLogin.LevelOfAccess;
                        ViewData.Model = model;
                        aResult = View();
                    }
                    //Premiun
                    // check the levelofaccess for the premium student
                    else if (ragSession.CurrentUser.CurrentSiteLogin.LevelOfAccess == 40)
                    {
                        aResult = Redirect(ProcessStudentRequest(ragSession));
                        aResult = RedirectToAction("RAInstructor");
                    }
                    else
                    {
                        var model = new RALogin();
                        ViewData.Model = model;
                        aResult = View();
                    }
                }
            }
            return aResult;
        }

        public ActionResult Demo()
        {
            return View();
        }

        public ActionResult RAInstructor()
        {
            BFW.RAg.Session ragSession = new Session();
            Boolean HaveSession = ragSession.Check();
            var model = new RALogin();
            ViewData["accessType"] = ragSession.CurrentUser.CurrentSiteLogin.LevelOfAccess;
            ViewData.Model = model;

            return View();
        }

        private string ProcessStudentRequest(BFW.RAg.Session ragSession)
        {
            User ragUser = ragSession.CurrentUser;
            BizDC.UserInfo userInfo = null;
            BizDC.Course courseInfo = null;
            string url = "";
            string domainId = "";
            string flags = "2097153"; 

            // get into this to create Agilix User only if the Agilix UserId is not set, to avoid round trips.
            // TODO - need to check if email is unique, otherwise this logic shud be changed.
            if ((ragUser != null) && (BusinessContext.CurrentUser == null || BusinessContext.CurrentUser.Email != ragUser.Email)) 
            {                
                //Check if the user exits in Agilix, if not create the User                    
                userInfo = UserActions.GetUserbyRefId(ragUser.UID.ToString());                
                
                if (userInfo == null)
                {
                    // TO DO -- to get the domain name using the Course ID.
                    courseInfo = CourseActions.GetCourseByCourseId(BusinessContext.AgilixCourseID);                    
                    if (string.IsNullOrEmpty(courseInfo.DomainId)) domainId = "1";
                    else domainId = courseInfo.DomainId;
                         
                    //Create Component comes here.                    
                    UserActions.Login("root/administrator", "Password1");
                    userInfo = UserActions.CreateUser(ragUser.Email, ragUser.PWHint, "", "", ragUser.FirstName, ragUser.LastName, ragUser.Email, domainId, ragUser.UID.ToString());                    
                }
                ragSession.SetCurrentUserAgilixID(Convert.ToInt32(userInfo.Id));
            }
            if (string.IsNullOrEmpty(BusinessContext.EnrollmentId))
            {
                // TODO - 1. check if it shud be enrollment domain id / course domain id / user domain id
                //        2. how to get n assign flags

                var eid = EnrollmentActions.GetUserEnrollmentId(ragUser.AgilixUserID.ToString(), BusinessContext.AgilixCourseID);
               
                if (string.IsNullOrEmpty(eid))
                {
                    if (BusinessContext.CurrentUser.Id != "2")
                        UserActions.Login("root/administrator", "Password1");
                    IList<BizDC.Enrollment> newEnrollment = EnrollmentActions.CreateEnrollments(domainId, userInfo.Id, BusinessContext.AgilixCourseID, flags, "1", "2011-01-01T12:00:00.0Z", "2011-04-30T12:00:00.0Z", "", "");
                    if (newEnrollment != null) BusinessContext.EnrollmentId = newEnrollment[0].Id;
                    BusinessContext.Course = new Bfw.PX.Biz.DataContracts.Course() { Id = newEnrollment[0].Course.Id };
                }
                else
                {                        
                    BusinessContext.EnrollmentId = eid;
                }                
                url = BusinessContext.URL;               
            }
            if(BusinessContext.CurrentUser.Id != ragUser.AgilixUserID.ToString())
            {
                UserActions.Logout();
                UserActions.Login(ragUser.Email, ragUser.PWHint);
            }
            url = BusinessContext.URL;
            return url;
        }
    }
}