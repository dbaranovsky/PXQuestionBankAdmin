using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Web.ViewModels.TiteList;

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
        }


        [HttpPost]
        public ActionResult GetTitleData()
        {
            var titles = Mapper.Map<IEnumerable<TitleViewModel>>(productCourseManagementService.GetAvailableCourses());
            return JsonCamel(new TitleListDataResponse
                             {
                                 Titles = titles
                             });
        }
	}
}