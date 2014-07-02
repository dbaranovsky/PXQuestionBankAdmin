using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Web.ViewModels.CompareTitles;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class CompareController : MasterController
    {
        //ToDo Use service instead commands
        private readonly IQuestionCommands commands;
        private readonly IProductCourseManagementService productCourseManagementService;
        private readonly IQuestionMetadataService questionMetadataService;

        public CompareController(IQuestionCommands commands,
                                 IProductCourseManagementService productCourseManagementService,
                                 IQuestionMetadataService questionMetadataService)
        {
            this.commands = commands;
            this.productCourseManagementService = productCourseManagementService;
            this.questionMetadataService = questionMetadataService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetComparedData(CompareTitlesRequest request)
        {
            var response = new CompareTitlesResponse();

            response.Page = request.Page;

            if (request.FirstCourse == request.SecondCourse)
            {
                response.OneQuestionRepositrory = false;
                return JsonCamel(response);
            }
            response.TotalPages = 5;
            response.OneQuestionRepositrory = true;

            var qeustions = commands.GetComparedQuestionList("39768", "70295", "85256", 0, 50);
            var firstCourse = productCourseManagementService.GetProductCourse("70295", true);
            var secondCourse = productCourseManagementService.GetProductCourse("85256", true);

            response.Questions = qeustions.CollectionPage.Select(Mapper.Map<ComparedQuestionViewModel>).ToList();
            response.FirstCourseQuestionCardLayout = questionMetadataService.GetQuestionCardLayout(firstCourse);
            response.SecondCourseQuestionCardLayout = questionMetadataService.GetQuestionCardLayout(secondCourse);

            return JsonCamel(response);
        }
	}
}