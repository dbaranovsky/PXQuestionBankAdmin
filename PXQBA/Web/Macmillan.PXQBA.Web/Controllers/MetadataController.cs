using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using AutoMapper;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Web.Helpers;
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
            var course = Mapper.Map<MetadataConfigViewModel>(productCourseManagementService.GetProductCourse(courseId));
            return JsonCamel(course);
        }

        [HttpPost]
        public ActionResult SaveMetadataConfig(MetadataConfigViewModel metadataConfig)
        {
            var course = Mapper.Map<Course>(metadataConfig);
            productCourseManagementService.UpdateMetadataConfig(course);
            CourseHelper.CurrentCourse = null;
            return JsonCamel(new {IsError = false});
        }
	}
}