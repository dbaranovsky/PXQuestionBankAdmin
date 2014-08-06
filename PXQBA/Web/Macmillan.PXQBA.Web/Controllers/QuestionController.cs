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
        private readonly UserCapabilitiesHelper userCapabilitiesHelper;
        private readonly CourseHelper courseHelper;

        public QuestionController(IQuestionManagementService questionManagementService, IQuestionMetadataService questionMetadataService, IProductCourseManagementService productCourseManagementService, IUserManagementService userManagementService, UserCapabilitiesHelper userCapabilitiesHelper, CourseHelper courseHelper)
        {
            this.questionManagementService = questionManagementService;
            this.questionMetadataService = questionMetadataService;
            this.productCourseManagementService = productCourseManagementService;
            this.userCapabilitiesHelper = userCapabilitiesHelper;
            this.courseHelper = courseHelper;
        }

        [HttpPost]
        public ActionResult UpdateMetadataField(string courseId, string questionId, string fieldName, string fieldValue)
        {
            if (fieldName == MetadataFieldNames.Flag)
            {
                if ((fieldValue == ((int)QuestionFlag.Flagged).ToString() && !userCapabilitiesHelper.GetCapabilities(courseId).Contains(Capability.FlagQuestion)) ||
                    (fieldValue == ((int)QuestionFlag.NotFlagged).ToString() && !userCapabilitiesHelper.GetCapabilities(courseId).Contains(Capability.UnflagQuestion)))
                {
                    return new HttpUnauthorizedResult();
                }
            }

            bool success = questionManagementService.UpdateQuestionField(courseHelper.GetCourse(courseId), questionId, fieldName, fieldValue, userCapabilitiesHelper.GetCapabilities(courseId));
            return JsonCamel(new { isError = !success });
        
        }

        [HttpPost]
        public ActionResult UpdateSharedMetadataField(string courseId, string questionId, string fieldName, List<string> fieldValues)
        {
            if (!userCapabilitiesHelper.GetCapabilities(courseId).Contains(Capability.EditSharedQuestionMetadata))
            {
                return new HttpUnauthorizedResult();
            }
            bool success = questionManagementService.UpdateSharedQuestionField(courseHelper.GetCourse(courseId), questionId, fieldName, fieldValues);
            return JsonCamel(new { isError = !success });
        }

        [HttpPost]
        public ActionResult BulkUpdateMetadataField(string courseId, string[] questionIds, string fieldName, string fieldValue)
        {
            var result = questionManagementService.BulklUpdateQuestionField(courseHelper.GetCourse(courseId), questionIds, fieldName, fieldValue, userCapabilitiesHelper.GetCapabilities(courseId));
            return JsonCamel(result);
        }

        public ActionResult CreateQuestion(string courseId, string questionType, string bank, string chapter)
        {
            if (!userCapabilitiesHelper.GetCapabilities(courseId).Contains(Capability.CreateQuestion))
            {
                return new HttpUnauthorizedResult();
            }
            var question = questionManagementService.CreateQuestion(courseHelper.GetCourse(courseId), questionType, bank, chapter);
            return JsonCamel(CreateQuestionViewModelForEditing(courseId, question));
        }

        [HttpPost]
        public ActionResult DuplicateQuestion(string courseId, string questionId, string version = null)
        {
            if (!userCapabilitiesHelper.GetCapabilities(courseId).Contains(Capability.DuplicateQuestion))
            {
                return new HttpUnauthorizedResult();
            }
            var question = questionManagementService.DuplicateQuestion(courseHelper.GetCourse(courseId), questionId, version);
            return JsonCamel(CreateQuestionViewModelForEditing(courseId, question));

        }

        public ActionResult GetAvailibleMetadata(string courseId)
        {
            return JsonCamel(questionMetadataService.GetAvailableFields(courseHelper.GetCourse(courseId)).Select(MetadataFieldsHelper.Convert).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateQuestion(string courseId, string questionJsonString)
        {
            // manual JSON deserialize is nessessary becauese of invalid model mapping on controller
            var questionViewModel = JsonConvert.DeserializeObject<QuestionViewModel>(questionJsonString);
            if ((!userCapabilitiesHelper.GetCapabilities(courseId).Contains(Capability.EditAvailableQuestion) &&
                 questionViewModel.Status == ((int) QuestionStatus.AvailableToInstructors).ToString()) ||
                (!userCapabilitiesHelper.GetCapabilities(courseId).Contains(Capability.EditInProgressQuestion) &&
                 questionViewModel.Status == ((int) QuestionStatus.InProgress).ToString()) ||
                (!userCapabilitiesHelper.GetCapabilities(courseId).Contains(Capability.EditDeletedQuestion) &&
                 questionViewModel.Status == ((int) QuestionStatus.Deleted).ToString()))
            {
                return new HttpUnauthorizedResult();
            }

            if (questionViewModel.IsShared && !IsAuthorizedToOverrideOnMetadata(courseId, questionViewModel))
            {
                return new HttpUnauthorizedResult();
            }

            if (questionViewModel.IsShared && !IsAuthorizedToOverrideOffMetadata(courseId, questionViewModel))
            {
                return new HttpUnauthorizedResult();
            }

            var question = Mapper.Map<Question>(questionViewModel);

            try
            {
                questionManagementService.UpdateQuestion(courseHelper.GetCourse(courseId), questionViewModel.RealQuestionId, question);
            }
            catch (Exception ex)
            {
                return JsonCamel(new { isError = true });
            }
            return JsonCamel(new { isError = false });
        }

        private bool IsAuthorizedToOverrideOffMetadata(string courseId, QuestionViewModel questionViewModel)
        {
            if (!userCapabilitiesHelper.GetCapabilities(courseId).Contains(Capability.RestoreLocalizedMetadataToSharedValue))
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
                    if ((!defaultFieldFromInitial.Value.IsCollectionEqual(localValuesFromInitial)) &&
                       (defaultValuesFromNew.IsCollectionEqual(localValuesFromNew)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool IsAuthorizedToOverrideOnMetadata(string courseId, QuestionViewModel questionViewModel)
        {
            if (!userCapabilitiesHelper.GetCapabilities(courseId).Contains(Capability.OverrideQuestionMetadata))
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
                    if (defaultFieldFromInitial.Value.IsCollectionEqual(localValuesFromInitial) &&
                        (!defaultValuesFromNew.IsCollectionEqual(localValuesFromNew)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public ActionResult GetQuestion(string courseId, string questionId)
        {
            var question = questionManagementService.GetQuestion(courseHelper.GetCourse(courseId), questionId);
            return JsonCamel(CreateQuestionViewModelForEditing(courseId, question));
        }

        private QuestionViewModel CreateQuestionViewModelForEditing(string courseId, Question question)
        {
            var tempQuestion = questionManagementService.CreateTemporaryQuestion(courseHelper.GetCourse(courseId), question.Id);
            var questionViewModel = Mapper.Map<Question, QuestionViewModel>(tempQuestion, opt => opt.Items.Add(courseId, courseHelper.GetCourse(courseId)));
            questionViewModel.RealQuestionId = question.Id;
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
            UpdateCapabilities(courseId, questionViewModel);
            return questionViewModel;
        }

        private void UpdateCapabilities(string courseId, QuestionViewModel questionViewModel)
        {
            var userCapabilities = userCapabilitiesHelper.GetCapabilities(courseId).ToList();
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
            questionViewModel.CanAddNotesQuestion = userCapabilities.Contains(Capability.AddNoteToQuestion);
            questionViewModel.CanRemoveNotesQuestion = userCapabilities.Contains(Capability.RemoveNoteFromQuestion);

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
        /// <param name="courseId"></param>
        /// <param name="questionsId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveFromTitle(string courseId, string[] questionsId)
        {
            bool isSuccess = questionManagementService.RemoveFromTitle(questionsId, courseHelper.GetCourse(courseId));
            return JsonCamel(new { isError = !isSuccess });
        }

        /// <summary>
        /// Share questions with selected  title, bank and chapter
        /// </summary>
        /// <param name="currentCourseId"></param>
        /// <param name="questionsId"></param>
        /// <param name="courseId"></param>
        /// <param name="bank"></param>
        /// <param name="chapter"></param>
        /// <returns></returns>
        public ActionResult PublishToTitle(string currentCourseId, string[] questionsId, int courseId, string bank, string chapter)
        {
            if (!userCapabilitiesHelper.GetCapabilities(currentCourseId).Contains(Capability.PublishQuestionToAnotherTitle))
            {
                return new HttpUnauthorizedResult();
            }
            var  result = questionManagementService.PublishToTitle(questionsId, courseId, bank, chapter, courseHelper.GetCourse(currentCourseId));
            return JsonCamel(result);
        }


        /// <summary>
        /// Returns current question's vesrion history 
        /// </summary>
        /// <returns></returns>
        public ActionResult GetQuestionVersions(string courseId, string questionId)
        {
            if (!userCapabilitiesHelper.GetCapabilities(courseId).Contains(Capability.ViewVersionHistory))
            {
                return new HttpUnauthorizedResult();
            }
            var versionHistory = Mapper.Map<QuestionHistoryViewModel>(questionManagementService.GetVersionHistory(courseHelper.GetCourse(courseId), questionId), opt => opt.Items.Add(courseId, courseId));
            return JsonCamel(versionHistory);
        }


        public ActionResult GetVersionPreviewLink(string courseId, string questionId, string version)
        {
            if (!userCapabilitiesHelper.GetCapabilities(courseId).Contains(Capability.TestSpecificVersion))
            {
                return new HttpUnauthorizedResult();
            }
            var tempVersion = questionManagementService.GetTemporaryQuestionVersion(courseHelper.GetCourse(courseId), questionId, version);
            return JsonCamel(new { Url = String.Format(ConfigurationHelper.GetActionPlayerUrlTemplate(), tempVersion.EntityId, tempVersion.QuizId) });
        }

        public ActionResult PublishDraftToOriginal(string courseId, string draftQuestionId)
        {
            if (!userCapabilitiesHelper.GetCapabilities(courseId).Contains(Capability.PublishDraft))
            {
                return new HttpUnauthorizedResult();
            }
            var success = questionManagementService.PublishDraftToOriginal(courseHelper.GetCourse(courseId), draftQuestionId);
            return JsonCamel(new {isError = !success});
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SaveAndPublishDraft(string courseId, string questionJsonString)
        {
            var success = false;
            // manual JSON deserialize is nessessary becauese of invalid model mapping on controller
            var questionViewModel = JsonConvert.DeserializeObject<QuestionViewModel>(questionJsonString);
            var question = Mapper.Map<Question>(questionViewModel);

            try
            {
                question = questionManagementService.UpdateQuestion(courseHelper.GetCourse(courseId), questionViewModel.RealQuestionId, question);
                success = questionManagementService.PublishDraftToOriginal(courseHelper.GetCourse(courseId), question.Id);
            }
            catch (Exception ex)
            {
                return JsonCamel(new { isError = !success });
            }
            return JsonCamel(new { isError = !success });
        }

        public ActionResult CreateDraft(string courseId, string questionId, string version = null, string questionStatus = null)
        {
            if (version == null && !userCapabilitiesHelper.GetCapabilities(courseId).Contains(Capability.CreateDraftFromOldVersion))
            {
                return new HttpUnauthorizedResult();
            }
            if (questionStatus == EnumHelper.GetEnumDescription(QuestionStatus.AvailableToInstructors) &&
                !userCapabilitiesHelper.GetCapabilities(courseId).Contains(Capability.CreateDraftFromAvailableQuestion))
            {
                return new HttpUnauthorizedResult();
            }
            var question = questionManagementService.CreateDraft(courseHelper.GetCourse(courseId), questionId);
            return JsonCamel(CreateQuestionViewModelForEditing(courseId, question));

        }

        public ActionResult RestoreVersion(string courseId, string questionId, string version)
        {
            if (!userCapabilitiesHelper.GetCapabilities(courseId).Contains(Capability.RestoreOldVersion))
            {
                return new HttpUnauthorizedResult();
            }
            var questionVersion = questionManagementService
                .RestoreQuestionVersion(courseHelper.GetCourse(courseId), questionId, version);
            return JsonCamel(Mapper.Map<QuestionVersionViewModel>(questionVersion));
        }

        public ActionResult GetUpdatedGraphEditor(string questionId, string interactionData)
        {
            return JsonCamel(new { EditorHtml = QuestionPreviewHelper.GetGraphEditor(interactionData, questionId, QuestionTypeHelper.GraphType) });
        }

        public ActionResult ClearQuestionResources(string courseId, string questionId)
        {
            questionManagementService.RemoveRelatedQuestionTempResources(questionId, courseHelper.GetCourse(courseId));
             return JsonCamel(new { isError = false });
        }
	}
  
}