using System.Collections.Generic;
using System.Web.Mvc;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Web.Helpers;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class QuestionFilterController : MasterController
    {
        private readonly IQuestionManagementService questionManagementService;

        public QuestionFilterController(IQuestionManagementService questionManagementService)
        {
            this.questionManagementService = questionManagementService;
        }

        public ActionResult GetQuestionTypeList()
        {
            var types = questionManagementService.GetQuestionTypesForCourse(CourseHelper.CurrentCourse);
            var typesViewModel = new List<dynamic>();
            foreach (var type in types)
            {
                var value = EnumHelper.GetEnumDescription(type);
                var key = type;
                typesViewModel.Add(new { key, value});
            }

            return JsonCamel(typesViewModel);  
        }
    }
}