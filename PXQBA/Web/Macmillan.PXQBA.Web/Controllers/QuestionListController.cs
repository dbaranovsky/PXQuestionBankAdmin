using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bfw.Agilix.Dlap.Session;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Common.Helpers;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class QuestionListController : Controller
    {

        private readonly IQuestionListManagementService questionListManagementService;

        private readonly ISessionManager sessionManager;
        public QuestionListController(IQuestionListManagementService questionListManagementService, ISessionManager sessionManager)
        {
            this.questionListManagementService = questionListManagementService;
            this.sessionManager = sessionManager;
        }

        //
        // GET: /QuestionList/
        public ActionResult Index()
        {
            return View();
        }
	}
}