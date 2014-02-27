using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;

using Mhe.GrantAccess.DataContracts;
using Mhe.GrantAccess.ServiceContracts;
using Mhe.GrantAccess.Services;
using Mhe.GrantAccess.DAL.Models;
using Mhe.GrantAccess.DAL.Contracts;
using Mhe.GrantAccess.DAL;

using Bfw.Common;
using Bfw.Common.Logging;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.Dlap.Components.Session;

using GrantAccess.Models;

namespace GrantAccess.Controllers
{
    public class HomeController : Controller
    {
        protected IAccessService AccessService { get; set; }

        public HomeController()
        {
            var sessionManager = new WebSessionManager(new NullLogger(), new NullTraceManager());
            sessionManager.CurrentSession = sessionManager.StartAnnonymousSession();
            var databaseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["PXData"].ConnectionString);

            var userStore = new PxUserStore(sessionManager);
            var rightsStore = new PxDataRightsStore(databaseConnection);
            var courseStore = new PxCourseStore(sessionManager);

            AccessService = new AccessService(userStore, rightsStore, courseStore);
        }

        public ActionResult Index()
        {
            ViewData.Model = new GrantAccessRequest();

            return View();
        }

        [HttpPost]
        public ActionResult Index(GrantAccessRequest request)
        {
            var domainId = ConfigurationManager.AppSettings["SandBoxDomainId"];
            var accessResponse = AccessService.Grant(Access.AdminSandbox, to: request.UserEmail, forCourse: request.CourseId, inDomain: domainId);

            if (!accessResponse.Error)
            {
                ViewData.Model = new GrantAccessRequest
                {
                    Error = false,
                    Message = "Access granted successfully",
                    UserEmail = string.Empty,
                    CourseId = string.Empty
                };
            }
            else
            {
                ViewData.Model = new GrantAccessRequest
                {
                    Error = accessResponse.Error,
                    Message = accessResponse.Message
                };
            }

            return View();
        }
    }
}
