using System.Collections.Generic;
using System.Web.Mvc;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Web.ViewModels.CompareTitles;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class CompareController : MasterController
    {
        //ToDo Use service instead commands
        private readonly IQuestionCommands commands;

        public CompareController(IQuestionCommands commands)
        {
            this.commands = commands;
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

            commands.GetComparedQuestionList("39768", new List<FilterFieldDescriptor>()
                                                 {
                                                     new FilterFieldDescriptor()
                                                     {
                                                         Field = MetadataFieldNames.ProductCourse,
                                                         Values = new List<string>()
                                                                  {
                                                                      "70295", "85256"
                                                                  } 
                                                     }
                                                 }, "70295", "85256", 0, 50);

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