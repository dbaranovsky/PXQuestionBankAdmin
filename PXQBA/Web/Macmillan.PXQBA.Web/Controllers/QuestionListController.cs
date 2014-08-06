using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc.Html;
using AutoMapper;
using System.Net;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;
using Macmillan.PXQBA.Common.Logging;
using Macmillan.PXQBA.Web.Helpers;
using Macmillan.PXQBA.Web.ViewModels;
using Macmillan.PXQBA.Web.ViewModels.Pages;
using EnumHelper = Macmillan.PXQBA.Common.Helpers.EnumHelper;
using Question = Macmillan.PXQBA.Business.Models.Question;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class QuestionListController : MasterController
    {
        private readonly IQuestionManagementService questionManagementService;
        private readonly IQuestionMetadataService questionMetadataService;
        private readonly INotesManagementService notesManagementService;
        private readonly IProductCourseManagementService productCourseManagementService;
        private readonly IUserManagementService userManagementService;
        private readonly UserCapabilitiesHelper userCapabilitiesHelper;
        private readonly CourseHelper courseHelper;

        private readonly int questionPerPage;

        public QuestionListController(IQuestionMetadataService questionMetadataService,
                                      IQuestionManagementService questionManagementService,
                                      INotesManagementService notesManagementService, IProductCourseManagementService productCourseManagementService, 
                                      IUserManagementService userManagementService,
                                      UserCapabilitiesHelper userCapabilitiesHelper,
                                      CourseHelper courseHelper)
        {
            this.questionManagementService = questionManagementService;
            this.questionPerPage = ConfigurationHelper.GetQuestionPerPage();
            this.questionMetadataService = questionMetadataService;
            this.notesManagementService = notesManagementService;
            this.productCourseManagementService = productCourseManagementService;
            this.userManagementService = userManagementService;
            this.userCapabilitiesHelper = userCapabilitiesHelper;
            this.courseHelper = courseHelper;

        }

        public ActionResult Index(string titleId , string chapterId)
        {
            QuestionListViewModel viewModel = new QuestionListViewModel()
                                              {
                                                  CourseId = titleId,
                                                  ChapterId = chapterId
                                              };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult GetQuestionData(QuestionListDataRequest request)
        {
            StaticLogger.LogDebug("GetQuestionData start: " + DateTime.Now);
            var currentCourseFilter = request.Filter.SingleOrDefault(x => x.Field == MetadataFieldNames.ProductCourse);

            Course currentCourse = courseHelper.GetCourse(currentCourseFilter.Values.First());
            if (!userCapabilitiesHelper.GetCapabilities(currentCourse.ProductCourseId).Contains(Capability.ViewQuestionList))
            {
                return new HttpUnauthorizedResult();
            }
            var sortCriterion = new SortCriterion {ColumnName = request.OrderField, SortType = request.OrderType};
            var questionList = questionManagementService.GetQuestionList(currentCourse, request.Filter, sortCriterion, 
                                                                          (request.PageNumber - 1) * questionPerPage,
                                                                          questionPerPage);


            var totalPages = GridHelper.GetTotalPages(questionList.TotalItems, questionPerPage);

            var response = new QuestionListDataResponse
                        {
                            Filter = request.Filter,
                            TotalPages = totalPages,
                            QuestionList = questionList.CollectionPage.Select(q => Mapper.Map<QuestionMetadata>(q, opt => opt.Items.Add(currentCourse.ProductCourseId, currentCourse))).ToList(),
                            PageNumber = request.PageNumber,
                            Columns = questionMetadataService.GetDataForFields(currentCourse, request.Columns).Select(MetadataFieldsHelper.Convert).ToList(),
                            AllAvailableColumns = questionMetadataService.GetAvailableFields(currentCourse).Select(MetadataFieldsHelper.Convert).ToList(),
                            Order = new QuestionOrder()
                                    {
                                        OrderField = request.OrderField,
                                        OrderType = request.OrderType.ToString().ToLower()
                                    },
                            QuestionCardLayout = questionMetadataService.GetQuestionCardLayout(currentCourse),
                            ProductTitle = currentCourse.Title,
                            ProductCourseId = currentCourse.ProductCourseId
                        };
            UpdateCapabilities(response);
            StaticLogger.LogDebug("GetQuestionData end: " + DateTime.Now);

            return JsonCamel(response);
        }

        private void UpdateCapabilities(QuestionListDataResponse response)
        {
            var userCapabilities = userCapabilitiesHelper.GetCapabilities(response.ProductCourseId).ToList();
            response.CanViewQuestionList = userCapabilities.Contains(Capability.ViewQuestionList);
            response.CanPreviewQuestion = userCapabilities.Contains(Capability.PreviewQuestion);
            response.CanCreateQuestion = userCapabilities.Contains(Capability.CreateQuestion);
            response.CanDuplicateQuestion = userCapabilities.Contains(Capability.DuplicateQuestion);
            response.CanFlagQuestion = userCapabilities.Contains(Capability.FlagQuestion);
            response.CanUnflagQuestion = userCapabilities.Contains(Capability.UnflagQuestion);
            response.CanAddNotesQuestion = userCapabilities.Contains(Capability.AddNoteToQuestion);
            response.CanRemoveNotesQuestion = userCapabilities.Contains(Capability.RemoveNoteFromQuestion);
            response.CanShareQuestion = userCapabilities.Contains(Capability.PublishQuestionToAnotherTitle);
            response.CanViewHistory = userCapabilities.Contains(Capability.ViewVersionHistory);
            response.CanCreateNewDraft = userCapabilities.Contains(Capability.CreateDraftFromAvailableQuestion);
            response.CanPublishDraft = userCapabilities.Contains(Capability.PublishDraft);
            response.CanChangeFromAvailibleToDeleted = userCapabilities.Contains(Capability.ChangeStatusFromAvailableToDeleted);
            response.CanChangeFromAvailibleToInProgress = userCapabilities.Contains(Capability.ChangeStatusFromAvailableToInProgress);
            response.CanChangeFromDeletedToAvailible = userCapabilities.Contains(Capability.ChangeStatusFromDeletedToAvailable);
            response.CanChangeFromDeletedToInProgress = userCapabilities.Contains(Capability.ChangeStatusFromDeletedToInProgress);
            response.CanChangeFromInProgressToAvailible = userCapabilities.Contains(Capability.ChangeStatusFromInProgressToAvailable);
            response.CanChangeFromInProgressToDeleted = userCapabilities.Contains(Capability.ChangeStatusFromInProgressToDeleted);

            foreach (var questionMetadata in response.QuestionList)
            {
                if (!response.CanPreviewQuestion && questionMetadata.Data.ContainsKey(MetadataFieldNames.InlinePreview))
                {
                    questionMetadata.Data[MetadataFieldNames.InlinePreview] = "You are not authorized to preview question.";
                }
                if (questionMetadata.Data.ContainsKey(MetadataFieldNames.QuestionStatus))
                {
                    var status = questionMetadata.Data[MetadataFieldNames.QuestionStatus];
                    questionMetadata.CanEditQuestion = (userCapabilities.Contains(Capability.EditAvailableQuestion) &&
                                                 status == EnumHelper.GetEnumDescription(QuestionStatus.AvailableToInstructors)) ||
                                                (userCapabilities.Contains(Capability.EditInProgressQuestion) &&
                                                 status == EnumHelper.GetEnumDescription(QuestionStatus.InProgress)) ||
                                                (userCapabilities.Contains(Capability.EditDeletedQuestion) &&
                                                 status == EnumHelper.GetEnumDescription(QuestionStatus.Deleted));
                    questionMetadata.CanCreateDraftFromAvailableQuestion =
                        userCapabilities.Contains(Capability.CreateDraftFromAvailableQuestion);
                }
                if (userCapabilities.Contains(Capability.ChangeDraftStatus) && questionMetadata.Data.ContainsKey(MetadataFieldNames.DraftFrom) &&
                    !string.IsNullOrEmpty(questionMetadata.Data[MetadataFieldNames.DraftFrom]))
                {
                    questionMetadata.CanChangeDraftStatus = true;
                }

            }
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
        /// <param name="courseId"></param>
        /// <param name="note"></param>
        [HttpPost]
        public ActionResult CreateQuestionNote(string courseId, Note note)
        {
            if (!userCapabilitiesHelper.GetCapabilities(courseId).Contains(Capability.AddNoteToQuestion))
            {
                return new HttpUnauthorizedResult();
            }
            
            return JsonCamel(notesManagementService.CreateNote(note));
        }

        /// <summary>
        /// Delete question note
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="note"></param>
        [HttpPost]
        public ActionResult DeleteQuestionNote(string courseId, Note note)
        {
            if (!userCapabilitiesHelper.GetCapabilities(courseId).Contains(Capability.RemoveNoteFromQuestion))
            {
                return new HttpUnauthorizedResult();
            }
            notesManagementService.DeleteNote(note);
            return JsonCamel(new { isError = false });
        }

        /// <summary>
        /// Save new question note
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="note"></param>
        [HttpPost]
        public ActionResult SaveQuestionNote(string courseId, Note note)
        {
            if (!userCapabilitiesHelper.GetCapabilities(courseId).Contains(Capability.AddNoteToQuestion))
            {
                return new HttpUnauthorizedResult();
            }
          notesManagementService.SaveNote(note);
          return JsonCamel(new { isError = false });
        }


        /// <summary>
        /// Gets metadata for course by its Id
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public ActionResult GetMetadataForCourse(string courseId)
        {
            Course course = null;
            if (courseHelper.GetChachedCourseId() == courseId)
            {
                course = courseHelper.GetCourse(courseId);
            }
            else
            {
                course = productCourseManagementService.GetProductCourse(courseId);
            }
             
            return JsonCamel(questionMetadataService.GetAvailableFields(course).Select(MetadataFieldsHelper.Convert));
        }

    }

}
