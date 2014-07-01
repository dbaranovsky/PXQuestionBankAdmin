using System.Collections.Generic;
using System.Web.Mvc;
using Macmillan.PXQBA.Web.ViewModels.CompareTitles;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class CompareController : MasterController
    {
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

            response.Questions = new List<ComparedQuestionViewModel>();

            response.Questions.Add(new ComparedQuestionViewModel()
                                   {
                                       Title = "Title1",
                                       CompareLocation = CompareLocationType.OnlyFirstCourse
                                   });

            response.Questions.Add(new ComparedQuestionViewModel()
                                    {
                                        Title = "Title2",
                                        CompareLocation = CompareLocationType.OnlySecondCourse
                                    });

            response.Questions.Add(new ComparedQuestionViewModel()
                                    {
                                        Title = "Title3",
                                        CompareLocation = CompareLocationType.BothCourses
                                    });

            return JsonCamel(response);
        }
	}
}