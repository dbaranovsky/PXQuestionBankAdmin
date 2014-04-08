﻿using System;
using AutoMapper;
using System.Net;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Business.Models.Web;
using Macmillan.PXQBA.Business.Services;
using Macmillan.PXQBA.Common.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;
using System.Xml;
using Macmillan.PXQBA.Web.Helpers;
using Mono.Options;
using Question = Macmillan.PXQBA.Business.Models.Question;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class QuestionListController : MasterController
    {
        private readonly IQuestionManagementService questionManagementService;
        private readonly IQuestionMetadataService questionMetadataService;
        private readonly INotesManagementService notesManagementService;

        private readonly int questionPerPage;

        public QuestionListController(IQuestionMetadataService questionMetadataService,
                                      IQuestionManagementService questionManagementService,
                                      INotesManagementService notesManagementService)
        {
            this.questionManagementService = questionManagementService;
            this.questionPerPage = ConfigurationHelper.GetQuestionPerPage();
            this.questionMetadataService = questionMetadataService;
            this.notesManagementService = notesManagementService;

            // \todo Setup current course when the user selects one from the list
            CourseHelper.CurrentCourse = new Course {ProductCourseId = "1", Title = "Sample Course"};
        }

        //
        // GET: /QuestionList/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetQuestionData(QuestionListDataRequest request)
        {
            var sortCriterion = new SortCriterion {ColumnName = request.OrderField, SortType = request.OrderType};
            var questionList = questionManagementService.GetQuestionList(CourseHelper.CurrentCourse,
                                                                          sortCriterion, 
                                                                          (request.PageNumber - 1) * questionPerPage,
                                                                          questionPerPage);

            var model = new QuestionListDataResponse
                        {
                            TotalPages = questionList.TotalItems / questionPerPage,
                            QuestionList = questionList.CollectionPage.Select(Mapper.Map<QuestionMetadata>),
                            PageNumber = request.PageNumber,
                            Columns = questionMetadataService.GetDataForFields(request.Columns).Select(QuestionFieldViewModel.Convert).ToList(),
                            AllAvailableColumns = questionMetadataService.GetAvailableFields().Select(QuestionFieldViewModel.Convert).ToList(),
                            Order = new QuestionOrder()
                                    {
                                        OrderField = request.OrderField,
                                        OrderType = request.OrderType.ToString().ToLower()
                                    }
                        };
            return JsonCamel(model);
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
        public void DeleteQuestionNote(Note note)
        {
            notesManagementService.DeleteNote(note);
        }

        /// <summary>
        /// Save new question note
        /// </summary>
        /// <param name="note"></param>
        [HttpPost]
        public void SaveQuestionNote(Note note)
        {
          notesManagementService.SaveNote(note);
        }


        #region debug
        /// <summary>
        /// For deubg ordering question list.
        /// </summary>
        /// <returns></returns>

        private IList<Question> ApplyFakeOrdering(IList<Question> questions, SortType orderType, string fieldName)
        {
            switch (orderType)
            {
                case SortType.Asc:
                    return questions.AsQueryable().OrderBy(MappingNameForFake(fieldName)).ToList();
                case SortType.Desc:
                    return questions.AsQueryable().OrderBy(MappingNameForFake(fieldName) + " descending").ToList();
            }

            return questions;
        }

        /// <summary>
        /// For debug ordering question list.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        private string MappingNameForFake(string title)
        {
            switch (title)
            {
                case "Chapter":
                    return "EBookChapter";
                case "Bank":
                    return "QuestionBank";
                case "Seq":
                    return "QuestionSeq";
                case "Title":
                    return "Title";
                case "Format":
                    return "QuestionType";
            }

            return "QuestionSeq";
        }
        #endregion

    }

}
