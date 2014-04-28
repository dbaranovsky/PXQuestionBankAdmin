using System;
using AutoMapper;
using System.Net;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;
using Macmillan.PXQBA.Web.Helpers;
using Macmillan.PXQBA.Web.ViewModels;
using Macmillan.PXQBA.Web.ViewModels.Pages;
using Question = Macmillan.PXQBA.Business.Models.Question;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class QuestionListController : MasterController
    {
        private readonly IQuestionManagementService questionManagementService;
        private readonly IQuestionMetadataService questionMetadataService;
        private readonly INotesManagementService notesManagementService;
        private readonly IProductCourseManagementService productCourseManagementService;

        private readonly int questionPerPage;

        public QuestionListController(IQuestionMetadataService questionMetadataService,
                                      IQuestionManagementService questionManagementService,
                                      INotesManagementService notesManagementService, IProductCourseManagementService productCourseManagementService)
        {
            this.questionManagementService = questionManagementService;
            this.questionPerPage = ConfigurationHelper.GetQuestionPerPage();
            this.questionMetadataService = questionMetadataService;
            this.notesManagementService = notesManagementService;
            this.productCourseManagementService = productCourseManagementService;
        }

        public ActionResult Index(string titleId , string chapterId)
        {
            QuestionListViewModel viewModel = new QuestionListViewModel()
                                              {
                                                  CourseId = titleId,
                                                  ChapterId = chapterId
                                              };
            CourseHelper.CurrentCourse = productCourseManagementService.GetProductCourse(titleId);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult GetQuestionData(QuestionListDataRequest request)
        {

            var currentCourseFilter = request.Filter.SingleOrDefault(x => x.Field == MetadataFieldNames.ProductCourse);
            ClearFilter(currentCourseFilter, request);
            CourseHelper.CurrentCourse = productCourseManagementService.GetProductCourse(currentCourseFilter.Values.First());

            var sortCriterion = new SortCriterion {ColumnName = request.OrderField, SortType = request.OrderType};
            var questionList = questionManagementService.GetQuestionList(CourseHelper.CurrentCourse, request.Filter, sortCriterion, 
                                                                          (request.PageNumber - 1) * questionPerPage,
                                                                          questionPerPage);
            var totalPages = (questionList.TotalItems + questionPerPage - (questionList.TotalItems % questionPerPage)) /
                             questionPerPage;

            var response = new QuestionListDataResponse
                        {
                            Filter = request.Filter,
                            TotalPages = totalPages,
                            QuestionList = questionList.CollectionPage.Select(Mapper.Map<QuestionMetadata>),
                            PageNumber = request.PageNumber,
                            Columns = questionMetadataService.GetDataForFields(CourseHelper.CurrentCourse, request.Columns).Select(MetadataFieldsHelper.Convert).ToList(),
                            AllAvailableColumns = questionMetadataService.GetAvailableFields(CourseHelper.CurrentCourse).Select(MetadataFieldsHelper.Convert).ToList(),
                            Order = new QuestionOrder()
                                    {
                                        OrderField = request.OrderField,
                                        OrderType = request.OrderType.ToString().ToLower()
                                    },
                            QuestionCardLayout = questionMetadataService.GetQuestionCardLayout(CourseHelper.CurrentCourse),
                            ProductTitle = CourseHelper.CurrentCourse.Title
                        };

            return JsonCamel(response);
        }

        /// <summary>
        /// Gets notes attached to the question by question id
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public ActionResult GetQuestionNotes(string questionId)
        {
            return JsonCamel(notesManagementService.GetQuestionNotes(questionId));
        }

        /// <summary>
        /// Save new question note
        /// </summary>
        /// <param name="note"></param>
        [HttpPost]
        public ActionResult CreateQuestionNote(Note note)
        {
              return JsonCamel(notesManagementService.CreateNote(note));
        }

        /// <summary>
        /// Delete question note
        /// </summary>
        /// <param name="note"></param>
        [HttpPost]
        public ActionResult DeleteQuestionNote(Note note)
        {
            notesManagementService.DeleteNote(note);
            return JsonCamel(new { isError = false });
        }

        /// <summary>
        /// Save new question note
        /// </summary>
        /// <param name="note"></param>
        [HttpPost]
        public ActionResult SaveQuestionNote(Note note)
        {
          notesManagementService.SaveNote(note);
          return JsonCamel(new { isError = false });
        }


        /// <summary>
        /// Check filtration and reset if title was changed
        /// </summary>
        /// <param name="courseFilterDescriptor"></param>
        /// <param name="request"></param>
        private void ClearFilter(FilterFieldDescriptor courseFilterDescriptor, QuestionListDataRequest request)
        {
            if (CourseHelper.IsResetFiltrationNeeded(courseFilterDescriptor))
            {
                foreach (var filterItem in request.Filter)
                {
                    if (filterItem.Field != courseFilterDescriptor.Field)
                    {
                        filterItem.Values = new List<string>();
                    }
                }
            }
        }
    }

}
