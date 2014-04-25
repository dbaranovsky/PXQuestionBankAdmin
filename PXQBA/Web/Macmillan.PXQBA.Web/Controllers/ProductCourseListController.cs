using System.Collections.Generic;
using System.Web.Mvc;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class ProductCourseListController : MasterController
    {
        private readonly IProductCourseManagementService productCourseManagementService;

        public ProductCourseListController(IProductCourseManagementService productCourseManagementService)
        {
            this.productCourseManagementService = productCourseManagementService;
        }

        public ActionResult Index()
        {
           return View();
            var courses = productCourseManagementService.GetAvailableCourses();
            return View();
        }


        public ActionResult GetTitleData()
        {
            IEnumerable<Course> courses = productCourseManagementService.GetAvailableCourses();
            return JsonCamel(courses);
        }
	}
}