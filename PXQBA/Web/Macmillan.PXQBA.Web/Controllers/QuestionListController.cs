using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Macmillan.PXQBA.Business.Contracts;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class QuestionListController : Controller
    {
        private readonly IQuestionListManagementService questionListManagementService;
        public QuestionListController(IQuestionListManagementService questionListManagementService)
        {
            this.questionListManagementService = questionListManagementService;
        }

        //
        // GET: /Default1/

        public ActionResult Index()
        {
            return View();
        }

    }
}
