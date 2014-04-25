using System.Web.Mvc;
using Macmillan.PXQBA.Business.Contracts;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class ProductCourseListController : MasterController
    {
        private readonly IProductCourseManagementService productCourseManagementService;

        public ProductCourseListController(IProductCourseManagementService productCourseManagementService)
        {
            this.productCourseManagementService = productCourseManagementService;
        }
        //
        // GET: /TitleList/
        public ActionResult Index()
        {
            var courses = productCourseManagementService.GetAvailableCourses();
            return View();
        }
	}
}