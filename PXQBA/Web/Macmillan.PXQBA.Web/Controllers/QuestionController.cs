using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using AutoMapper;
using Bfw.Common;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Business.Services;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Common.Logging;
using Macmillan.PXQBA.Web.Helpers;
using Macmillan.PXQBA.Web.ViewModels;
using Macmillan.PXQBA.Web.ViewModels.Versions;
using Newtonsoft.Json;
using EnumHelper = Macmillan.PXQBA.Common.Helpers.EnumHelper;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class QuestionController : MasterController
    {
        private readonly IQuestionManagementService questionManagementService;
        private readonly IQuestionMetadataService questionMetadataService;
        private readonly IProductCourseManagementService productCourseManagementService;
    
        public QuestionController(IQuestionManagementService questionManagementService,   IQuestionMetadataService questionMetadataService, IProductCourseManagementService productCourseManagementService, IUserManagementService userManagementService)
        {
            this.questionManagementService = questionManagementService;
            this.questionMetadataService = questionMetadataService;
            this.productCourseManagementService = productCourseManagementService;
        }

        [HttpPost]
        public ActionResult UpdateMetadataField(string questionId, string fieldName, string fieldValue)
        {
            if (fieldName == MetadataFieldNames.Flag)
            {
                if ((fieldValue == ((int) QuestionFlag.Flagged).ToString() && !UserCapabilitiesHelper.Capabilities.Contains(Capability.FlagQuestion)) ||
                    (fieldValue == ((int)QuestionFlag.NotFlagged).ToString() && !UserCapabilitiesHelper.Capabilities.Contains(Capability.UnflagQuestion)))
                {
                    return new HttpUnauthorizedResult();
                }
            }
            bool success = questionManagementService.UpdateQuestionField(CourseHelper.CurrentCourse, questionId, fieldName, fieldValue, UserCapabilitiesHelper.Capabilities);
            return JsonCamel(new { isError = !success });
        
        }

        [HttpPost]
        public ActionResult UpdateSharedMetadataField(string questionId, string fieldName, List<string> fieldValues)
        {
            if (!UserCapabilitiesHelper.Capabilities.Contains(Capability.EditSharedQuestionMetadata))
            {
                return new HttpUnauthorizedResult();
            }
            bool success = questionManagementService.UpdateSharedQuestionField(CourseHelper.CurrentCourse, questionId, fieldName, fieldValues);
            return JsonCamel(new { isError = !success });
        }

        [HttpPost]
        public ActionResult BulkUpdateMetadataField(string[] questionIds, string fieldName, string fieldValue)
        {
            bool success = questionManagementService.BulklUpdateQuestionField(CourseHelper.CurrentCourse, questionIds, fieldName, fieldValue, UserCapabilitiesHelper.Capabilities);
            return JsonCamel(new { isError = !success });

        }

        public ActionResult CreateQuestion(string questionType, string bank, string chapter)
        {
            if (!UserCapabilitiesHelper.Capabilities.Contains(Capability.CreateQuestion))
            {
                return new HttpUnauthorizedResult();
            }
            var question = questionManagementService.CreateQuestion(CourseHelper.CurrentCourse, questionType, bank, chapter);
            return JsonCamel(CreateQuestionViewModelForEditing(question));
        }

        [HttpPost]
        public ActionResult DuplicateQuestion(string questionId, string version = null)
        {
            if (!UserCapabilitiesHelper.Capabilities.Contains(Capability.DuplicateQuestion))
            {
                return new HttpUnauthorizedResult();
            }
            var question = questionManagementService.DuplicateQuestion(CourseHelper.CurrentCourse, questionId, version);
            return JsonCamel(CreateQuestionViewModelForEditing(question));

        }

        public ActionResult GetAvailibleMetadata()
        {
            return JsonCamel(questionMetadataService.GetAvailableFields(CourseHelper.CurrentCourse).Select(MetadataFieldsHelper.Convert).ToList());
        }

        public ActionResult GetAvailibleMetadataByCourseId(string courseId)
        {
            var course = productCourseManagementService.GetProductCourse(courseId);
            if (course != null)
            {
                return JsonCamel(questionMetadataService.GetAvailableFields(course).Select(MetadataFieldsHelper.Convert).ToList());
            }
            return JsonCamel(questionMetadataService.GetAvailableFields(CourseHelper.CurrentCourse).Select(MetadataFieldsHelper.Convert).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateQuestion(string questionJsonString)
        {
            // manual JSON deserialize is nessessary becauese of invalid model mapping on controller
            var questionViewModel = JsonConvert.DeserializeObject<QuestionViewModel>(questionJsonString);
            if ((!UserCapabilitiesHelper.Capabilities.Contains(Capability.EditAvailableQuestion) &&
                 questionViewModel.Status == ((int) QuestionStatus.AvailableToInstructors).ToString()) ||
                (!UserCapabilitiesHelper.Capabilities.Contains(Capability.EditInProgressQuestion) &&
                 questionViewModel.Status == ((int) QuestionStatus.InProgress).ToString()) ||
                (!UserCapabilitiesHelper.Capabilities.Contains(Capability.EditDeletedQuestion) &&
                 questionViewModel.Status == ((int) QuestionStatus.Deleted).ToString()))
            {
                return new HttpUnauthorizedResult();
            }

            if (!IsAuthorizedToOverrideOnMetadata(questionViewModel))
            {
                return new HttpUnauthorizedResult();
            }

            if (!IsAuthorizedToOverrideOffMetadata(questionViewModel))
            {
                return new HttpUnauthorizedResult();
            }

            var question = Mapper.Map<Question>(questionViewModel);

            try
            {
                questionManagementService.UpdateQuestion(CourseHelper.CurrentCourse, QuestionHelper.QuestionIdToEdit, question);
            }
            catch (Exception ex)
            {
                return JsonCamel(new { isError = true });
            }
            return JsonCamel(new { isError = false });
        }

        private bool IsAuthorizedToOverrideOffMetadata(QuestionViewModel questionViewModel)
        {
            if (!UserCapabilitiesHelper.Capabilities.Contains(Capability.RestoreLocalizedMetadataToSharedValue))
            {
                var initialViewModel = QuestionHelper.QuestionViewModelToEdit;
                if ((initialViewModel.DefaultSection.Title != initialViewModel.LocalSection.Title && questionViewModel.DefaultSection.Title == questionViewModel.LocalSection.Title) ||
                    (initialViewModel.DefaultSection.Bank != initialViewModel.LocalSection.Bank && questionViewModel.DefaultSection.Bank == questionViewModel.LocalSection.Bank) ||
                    (initialViewModel.DefaultSection.Chapter != initialViewModel.LocalSection.Chapter && questionViewModel.DefaultSection.Chapter == questionViewModel.LocalSection.Chapter))
                {
                    return false;
                }
                foreach (var defaultFieldFromInitial in initialViewModel.DefaultSection.DynamicValues.Where(v => initialViewModel.LocalSection.DynamicValues.ContainsKey(v.Key)))
                {
                    var localValuesFromInitial = initialViewModel.LocalSection.DynamicValues[defaultFieldFromInitial.Key];
                    var localValuesFromNew = initialViewModel.LocalSection.DynamicValues[defaultFieldFromInitial.Key];
                    var defaultValuesFromNew = initialViewModel.LocalSection.DynamicValues[defaultFieldFromInitial.Key];
                    if (defaultFieldFromInitial.Value.Intersect(localValuesFromInitial).Count() != Math.Max(defaultFieldFromInitial.Value.Count, localValuesFromInitial.Count) &&
                       defaultValuesFromNew.Intersect(localValuesFromNew).Count() == Math.Max(defaultValuesFromNew.Count, localValuesFromNew.Count))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool IsAuthorizedToOverrideOnMetadata(QuestionViewModel questionViewModel)
        {
            if (!UserCapabilitiesHelper.Capabilities.Contains(Capability.OverrideQuestionMetadata))
            {
                var initialViewModel = QuestionHelper.QuestionViewModelToEdit;
                if ((initialViewModel.DefaultSection.Title == initialViewModel.LocalSection.Title && questionViewModel.DefaultSection.Title != questionViewModel.LocalSection.Title) ||
                    (initialViewModel.DefaultSection.Bank == initialViewModel.LocalSection.Bank && questionViewModel.DefaultSection.Bank != questionViewModel.LocalSection.Bank) ||
                    (initialViewModel.DefaultSection.Chapter == initialViewModel.LocalSection.Chapter && questionViewModel.DefaultSection.Chapter != questionViewModel.LocalSection.Chapter))
                {
                    return false;
                }
                foreach (var defaultFieldFromInitial in initialViewModel.DefaultSection.DynamicValues.Where(v => initialViewModel.LocalSection.DynamicValues.ContainsKey(v.Key)))
                {
                    var localValuesFromInitial = initialViewModel.LocalSection.DynamicValues[defaultFieldFromInitial.Key];
                    var localValuesFromNew = initialViewModel.LocalSection.DynamicValues[defaultFieldFromInitial.Key];
                    var defaultValuesFromNew = initialViewModel.LocalSection.DynamicValues[defaultFieldFromInitial.Key];
                    if (defaultFieldFromInitial.Value.Intersect(localValuesFromInitial).Count() == Math.Max(defaultFieldFromInitial.Value.Count, localValuesFromInitial.Count) &&
                       defaultValuesFromNew.Intersect(localValuesFromNew).Count() != Math.Max(defaultValuesFromNew.Count, localValuesFromNew.Count))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public ActionResult GetQuestion(string questionId)
        {
            var question = questionManagementService.GetQuestion(CourseHelper.CurrentCourse, questionId);
            return JsonCamel(CreateQuestionViewModelForEditing(question));
        }

        private QuestionViewModel CreateQuestionViewModelForEditing(Question question)
        {
            QuestionHelper.QuestionIdToEdit = question.Id;
            var tempQuestion = questionManagementService.CreateTemporaryQuestion(CourseHelper.CurrentCourse, question.Id);
            var questionViewModel = Mapper.Map<Question, QuestionViewModel>(tempQuestion, opt => opt.Items.Add(CourseHelper.CurrentCourse.ProductCourseId, CourseHelper.CurrentCourse));
            questionViewModel.ActionPlayerUrl = String.Format(ConfigurationHelper.GetActionPlayerUrlTemplate(), questionViewModel.EntityId, questionViewModel.QuizId);

            questionViewModel.QuestionType = question.CustomUrl;
            questionViewModel.EditorUrl = QuestionPreviewHelper.GetEditorUrl(questionViewModel.QuestionType,
                                                                            questionViewModel.Id,
                                                                            questionViewModel.EntityId,
                                                                            questionViewModel.QuizId);          
           

            questionViewModel.GraphEditorHtml = QuestionPreviewHelper.GetGraphEditor(question.InteractionData,
                                                                                    questionViewModel.Id,
                                                                                    question.CustomUrl);
            if (questionViewModel.IsShared)
            {
                QuestionHelper.QuestionViewModelToEdit = questionViewModel;
            }
            UpdateCapabilities(questionViewModel);
            return questionViewModel;
        }

        private void UpdateCapabilities(QuestionViewModel questionViewModel)
        {
            var userCapabilities = UserCapabilitiesHelper.Capabilities;
            questionViewModel.CanTestQuestion = userCapabilities.Contains(Capability.TestQuestion);
            questionViewModel.CanTestQuestionVersion = userCapabilities.Contains(Capability.TestSpecificVersion);
            questionViewModel.CanOverrideMetadata = userCapabilities.Contains(Capability.OverrideQuestionMetadata);
            questionViewModel.CanRestoreMetadata = userCapabilities.Contains(Capability.RestoreLocalizedMetadataToSharedValue);
            questionViewModel.CanEditQuestion = (userCapabilities.Contains(Capability.EditAvailableQuestion) &&
                                                 questionViewModel.Status == ((int) QuestionStatus.AvailableToInstructors).ToString()) ||
                                                (userCapabilities.Contains(Capability.EditInProgressQuestion) &&
                                                 questionViewModel.Status == ((int) QuestionStatus.InProgress).ToString()) ||
                                                (userCapabilities.Contains(Capability.EditDeletedQuestion) &&
                                                 questionViewModel.Status == ((int) QuestionStatus.Deleted).ToString());
            questionViewModel.CanEditSharedQuestionContent = userCapabilities.Contains(Capability.EditSharedQuestionContent);
            questionViewModel.CanEditSharedQuestionMetadata = userCapabilities.Contains(Capability.EditSharedQuestionMetadata);
            questionViewModel.CanPublishDraft = userCapabilities.Contains(Capability.PublishDraft);
            questionViewModel.CanRestoreVersion = userCapabilities.Contains(Capability.RestoreOldVersion);
            questionViewModel.CanCreateDraftFromVersion = userCapabilities.Contains(Capability.CreateDraftFromOldVersion);
            questionViewModel.CanViewHistory = userCapabilities.Contains(Capability.ViewVersionHistory);
            if (!userCapabilities.Contains(Capability.EditSharedQuestionContent) && questionViewModel.IsShared)
            {
                questionViewModel.EditorUrl = string.Empty;
                questionViewModel.GraphEditorHtml = string.Empty;
            }
            if (!userCapabilities.Contains(Capability.TestQuestion))
            {
                questionViewModel.ActionPlayerUrl = string.Empty;
            }
        }


        /// <summary>
        /// Remove shared questions from current title 
        /// </summary>
        /// <param name="questionsId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveFromTitle(string[] questionsId)
        {
            bool isSuccess = questionManagementService.RemoveFromTitle(questionsId, CourseHelper.CurrentCourse);
            return JsonCamel(new { isError = !isSuccess });
        }

        /// <summary>
        /// Share questions with selected  title, bank and chapter
        /// </summary>
        /// <param name="questionsId"></param>
        /// <param name="courseId"></param>
        /// <param name="bank"></param>
        /// <param name="chapter"></param>
        /// <returns></returns>
        public ActionResult PublishToTitle(string[] questionsId, int courseId, string bank, string chapter)
        {
            if (!UserCapabilitiesHelper.Capabilities.Contains(Capability.PublishQuestionToAnotherTitle))
            {
                return new HttpUnauthorizedResult();
            }
            bool isSuccess = questionManagementService.PublishToTitle(questionsId, courseId, bank, chapter, CourseHelper.CurrentCourse);
            return JsonCamel(new { isError = !isSuccess });
        }


        /// <summary>
        /// Returns current question's vesrion history 
        /// </summary>
        /// <returns></returns>
        public ActionResult GetQuestionVersions()
        {
            if(!UserCapabilitiesHelper.Capabilities.Contains(Capability.ViewVersionHistory))
            {
                return new HttpUnauthorizedResult();
            }
            var versionHistory = Mapper.Map<QuestionHistoryViewModel>(questionManagementService.GetVersionHistory(CourseHelper.CurrentCourse, QuestionHelper.QuestionIdToEdit), opt => opt.Items.Add(CourseHelper.CurrentCourse.ProductCourseId, CourseHelper.CurrentCourse.ProductCourseId));
            return JsonCamel(versionHistory);
        }


        public ActionResult GetVersionPreviewLink(string version)
        {
            if (!UserCapabilitiesHelper.Capabilities.Contains(Capability.TestSpecificVersion))
            {
                return new HttpUnauthorizedResult();
            }
            var tempVersion = questionManagementService.GetTemporaryQuestionVersion(CourseHelper.CurrentCourse, QuestionHelper.QuestionIdToEdit, version);
            return JsonCamel(new { Url = String.Format(ConfigurationHelper.GetActionPlayerUrlTemplate(), tempVersion.EntityId, tempVersion.QuizId) });
        }

        public ActionResult PublishDraftToOriginal(string draftQuestionId)
        {
            if (!UserCapabilitiesHelper.Capabilities.Contains(Capability.PublishDraft))
            {
                return new HttpUnauthorizedResult();
            }
            var success = questionManagementService.PublishDraftToOriginal(CourseHelper.CurrentCourse, draftQuestionId);
            return JsonCamel(new {isError = !success});
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SaveAndPublishDraft(string questionJsonString)
        {
            var success = false;
            // manual JSON deserialize is nessessary becauese of invalid model mapping on controller
            var questionViewModel = JsonConvert.DeserializeObject<QuestionViewModel>(questionJsonString);
            var question = Mapper.Map<Question>(questionViewModel);

            try
            {
                question = questionManagementService.UpdateQuestion(CourseHelper.CurrentCourse, QuestionHelper.QuestionIdToEdit, question);
                success = questionManagementService.PublishDraftToOriginal(CourseHelper.CurrentCourse, question.Id);
            }
            catch (Exception ex)
            {
                return JsonCamel(new { isError = !success });
            }
            return JsonCamel(new { isError = !success });
        }

        public ActionResult CreateDraft(string questionId, string version = null, string questionStatus = null)
        {
            if (version == null && !UserCapabilitiesHelper.Capabilities.Contains(Capability.CreateDraftFromOldVersion))
            {
                return new HttpUnauthorizedResult();
            }
            if (questionStatus == EnumHelper.GetEnumDescription(QuestionStatus.AvailableToInstructors) &&
                !UserCapabilitiesHelper.Capabilities.Contains(Capability.CreateDraftFromAvailableQuestion))
            {
                return new HttpUnauthorizedResult();
            }
            var question = questionManagementService.CreateDraft(CourseHelper.CurrentCourse, string.IsNullOrEmpty(questionId) ? QuestionHelper.QuestionIdToEdit : questionId);
            return JsonCamel(CreateQuestionViewModelForEditing(question));

        }

        public ActionResult DeleteQuestion()
        {
            var isDeleted = questionManagementService.RemoveQuestion(CourseHelper.CurrentCourse,QuestionHelper.QuestionIdToEdit);
            return JsonCamel(new { ResetState = isDeleted });
        }

        public ActionResult RestoreVersion(string version)
        {
            if (!UserCapabilitiesHelper.Capabilities.Contains(Capability.RestoreOldVersion))
            {
                return new HttpUnauthorizedResult();
            }
            var questionVersion = questionManagementService.RestoreQuestionVersion(CourseHelper.CurrentCourse, QuestionHelper.QuestionIdToEdit, version);
            return JsonCamel(Mapper.Map<QuestionVersionViewModel>(questionVersion));
        }

        public ActionResult GetUpdatedGraphEditor(string interactionData)
        {
             return  JsonCamel(new { EditorHtml = QuestionPreviewHelper.GetGraphEditor(interactionData, QuestionHelper.QuestionIdToEdit, QuestionTypeHelper.GraphType)});
        }
	}
  
}