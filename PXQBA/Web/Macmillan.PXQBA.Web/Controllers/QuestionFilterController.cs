using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Common.Helpers;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class QuestionFilterController : MasterController
    {
        private readonly IQuestionFilterManagementService questionFilterManagement;

        public QuestionFilterController(IQuestionFilterManagementService questionFilterManagement)
        {
            this.questionFilterManagement = questionFilterManagement;
        }

        public ActionResult GetQuestionTypeList()
        {
            return JsonCamel(questionFilterManagement.GetQuestionTypeList());  
        }
    }
}