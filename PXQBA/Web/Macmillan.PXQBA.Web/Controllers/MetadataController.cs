using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using AutoMapper;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Web.ViewModels.MetadataConfig;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class MetadataController : MasterController
    {

        private readonly IProductCourseManagementService productCourseManagementService;

        public MetadataController(IProductCourseManagementService productCourseManagementService)
        {
            this.productCourseManagementService = productCourseManagementService;
        }

        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult GetAvailableCourses()
        {
            var courses = Mapper.Map<IEnumerable<ProductCourseViewModel>>(
                                            productCourseManagementService.GetAvailableCourses());
            return JsonCamel(courses);
        }


        [HttpPost]
        public ActionResult GetMetadataConfig(string courseId)
        {
            var sb1 = new StringBuilder();

            sb1.AppendLine("Chapter 1 " + courseId);
            sb1.AppendLine("Chapter 2 " + courseId);
            sb1.AppendLine("Chapter 3 " + courseId);

            var sb2 = new StringBuilder();

            sb2.AppendLine("Bank 1");
            sb2.AppendLine("Bank 2" + courseId);
            sb2.AppendLine("Bank 3");

            var metadataConfig = new MetadataConfigViewModel()
                                 {
                                     CourseId = courseId,
                                     Chapters = sb1.ToString(),
                                     Banks = sb2.ToString()
                                 };

            return JsonCamel(metadataConfig);
        }

        [HttpPost]
        public ActionResult SaveMetadataConfig(MetadataConfigViewModel metadataConfig)
        {

            return JsonCamel(new {IsError = false});
        }
	}
}