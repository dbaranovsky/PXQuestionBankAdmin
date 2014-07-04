using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Web.ViewModels.CompareTitles;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class CompareController : MasterController
    {
        private readonly IQuestionManagementService questionManagementService;
        private readonly IProductCourseManagementService productCourseManagementService;
        private readonly IQuestionMetadataService questionMetadataService;

        private readonly int questionPerPage;

        public CompareController(IQuestionManagementService questionManagementService,
                                 IProductCourseManagementService productCourseManagementService,
                                 IQuestionMetadataService questionMetadataService)
        {
            this.questionManagementService = questionManagementService;
            this.productCourseManagementService = productCourseManagementService;
            this.questionMetadataService = questionMetadataService;

            this.questionPerPage = ConfigurationHelper.GetQuestionPerPage();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetComparedData(CompareTitlesRequest request)
        {
            var response = new CompareTitlesResponse();

            //for debug:
            //request.FirstCourse = "70295";
            //request.SecondCourse = "85256";

            response.Page = request.Page;

            if (request.FirstCourse == request.SecondCourse)
            {
                response.OneQuestionRepositrory = false;
                return JsonCamel(response);
            }

            var firstCourse = productCourseManagementService.GetProductCourse(request.FirstCourse, true);
            var secondCourse = productCourseManagementService.GetProductCourse(request.SecondCourse, true);

            if (firstCourse.QuestionRepositoryCourseId != secondCourse.QuestionRepositoryCourseId)
            {
                response.OneQuestionRepositrory = false;
                return JsonCamel(response);
            }

            response.OneQuestionRepositrory = true;

            var qeustions = questionManagementService.GetComparedQuestionList(firstCourse.QuestionRepositoryCourseId, 
                                                                              request.FirstCourse,
                                                                              request.SecondCourse,
                                                                              (request.Page - 1) * questionPerPage,
                                                                              questionPerPage);

            response.TotalPages = (qeustions.TotalItems + questionPerPage - (qeustions.TotalItems % questionPerPage)) / questionPerPage; 
            response.Questions = qeustions.CollectionPage.Select(Mapper.Map<ComparedQuestionViewModel>).ToList();
            response.FirstCourseQuestionCardLayout = questionMetadataService.GetQuestionCardLayout(firstCourse);
            response.SecondCourseQuestionCardLayout = questionMetadataService.GetQuestionCardLayout(secondCourse);

            return JsonCamel(response);
        }
	}
}