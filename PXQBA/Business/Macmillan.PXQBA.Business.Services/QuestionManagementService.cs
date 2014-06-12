using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Common.Logging;
using Question = Macmillan.PXQBA.Business.Models.Question;

namespace Macmillan.PXQBA.Business.Services
{
    /// <summary>
    /// Service that handles operations with questions entities
    /// </summary>
    public class QuestionManagementService : IQuestionManagementService
    {
        private readonly IQuestionCommands questionCommands;
        private readonly ITemporaryQuestionOperation temporaryQuestionOperation;
        private readonly IProductCourseManagementService productCourseManagementService;

        public QuestionManagementService(IQuestionCommands questionCommands, ITemporaryQuestionOperation temporaryQuestionOperation, IProductCourseManagementService productCourseManagementService)
        {
            this.questionCommands = questionCommands;
            this.temporaryQuestionOperation = temporaryQuestionOperation;
            this.productCourseManagementService = productCourseManagementService;
        }

        public PagedCollection<Question> GetQuestionList(Course course, IEnumerable<FilterFieldDescriptor> filter, SortCriterion sortCriterion, int startingRecordNumber, int recordCount)
        {
            return questionCommands.GetQuestionList(course.QuestionRepositoryCourseId, course.ProductCourseId, filter, sortCriterion, startingRecordNumber, recordCount);
        }

        public Question CreateQuestion(Course course, string questionType, string bank, string chapter)
        {
            try
            {
                Question question = GetNewQuestionTemplate(course, questionType, bank, chapter);
                return questionCommands.CreateQuestion(course.ProductCourseId, question);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("CreateQuestion: courseId: {0}, questionType:{1}, bank:{2}, chapter: {3}",
                   course.ProductCourseId, questionType, bank,chapter), ex);
                throw;
            }

        }

        public Question GetQuestion(Course course, string questionId, string version = null)
        {
            try
            {
                return questionCommands.GetQuestion(course.QuestionRepositoryCourseId, questionId, version);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetQuestion: courseId: {0}, questionId:{1}, version:{2}",
                    course.ProductCourseId, questionId, string.IsNullOrEmpty(version) ? string.Empty : version), ex);
            }
        }

        public Question DuplicateQuestion(Course course, string questionId, string version = null)
        {
            Question question = GetQuestion(course, questionId, version);
            question.Id = Guid.NewGuid().ToString();
            question.Status = ((int)QuestionStatus.InProgress).ToString();
            ClearServiceFields(question);
            if (question.ProductCourseSections.Count > 1)
            {
                question.ProductCourseSections.RemoveAll(s => s.ProductCourseId != course.ProductCourseId);
                question.DuplicateFromShared = questionId;
            }
            question.DuplicateFrom = questionId;
            return questionCommands.CreateQuestion(course.ProductCourseId, question);
        }

   

        public Question UpdateQuestion(Course course, string sourceQuestionId, Question temporaryQuestion)
        {
            try
            {
                questionCommands.UpdateQuestionInTempQuiz(temporaryQuestion);
                var question = temporaryQuestionOperation.CopyQuestionToSourceCourse(course.QuestionRepositoryCourseId, sourceQuestionId);
                questionCommands.ExecuteSolrUpdateTask();
                return question;
            }
            catch (Exception ex)
            {
                StaticLogger.LogError(
                    string.Format("Update question: courseId: {0}, sourceQuestionId:{1}, temporaryQuestionId:{2}",
                        course.ProductCourseId, sourceQuestionId, temporaryQuestion.Id), ex);
                throw;
            }
            
        }

        public bool UpdateQuestionField(Course course, string questionId, string fieldName, string fieldValue)
        {
            return questionCommands.UpdateQuestionField(course.ProductCourseId, course.QuestionRepositoryCourseId, questionId, fieldName, fieldValue);
        }

        public bool UpdateSharedQuestionField(Course course, string questionId, string fieldName, IEnumerable<string> fieldValues)
        {
            return questionCommands.UpdateSharedQuestionField(course.QuestionRepositoryCourseId, questionId, fieldName, fieldValues);
        }

        public bool BulklUpdateQuestionField(Course course, string[] questionId, string fieldName, string fieldValue,
            bool isSharedField = false)
        {
            return questionCommands.BulklUpdateQuestionField(course.ProductCourseId, course.QuestionRepositoryCourseId, questionId, fieldName, fieldValue);
        }

        public Question CreateTemporaryQuestion(Course course, string questionId)
        {
            return temporaryQuestionOperation.CopyQuestionToTemporaryCourse(course.QuestionRepositoryCourseId, questionId);
        }
    
        public bool RemoveFromTitle(string[] questionsId, Course currentCourse)
        {
            bool isSuccess = questionCommands.RemoveFromTitle(questionsId, currentCourse.QuestionRepositoryCourseId, currentCourse.ProductCourseId);
            return isSuccess;
        }

        public bool PublishToTitle(string[] questionsId, int courseIdToPublish, string bank, string chapter, Course currentCourse)
        {
            var questions = questionCommands.GetQuestions(currentCourse.QuestionRepositoryCourseId, questionsId);
            foreach (var question in questions)
            {
                question.DefaultSection = GetDefaultSection(question, currentCourse);
                
                question.ProductCourseSections.RemoveAll(s => s.ProductCourseId == courseIdToPublish.ToString());
                question.ProductCourseSections.Add(GetNewProductCourseSection(courseIdToPublish, bank, chapter, currentCourse, question));
            }
            
            bool isSuccess = questionCommands.UpdateQuestions(questions, currentCourse.QuestionRepositoryCourseId);
            return isSuccess;
        }

        public IEnumerable<Question> GetVersionHistory(Course currentCourse, string questionId)
        {
            try
            {
                return questionCommands.GetVersionHistory(currentCourse.QuestionRepositoryCourseId, questionId);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetVersionHistory: courseId: {0}, questionId:{1}",
                    currentCourse.ProductCourseId, questionId), ex);
            }
        }

        public Question GetTemporaryQuestionVersion(Course currentCourse, string questionId, string version)
        {
            try
            {
                return temporaryQuestionOperation.CopyQuestionToTemporaryCourse(currentCourse.QuestionRepositoryCourseId, questionId, version);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetTemporaryQuestionVersion: courseId: {0}, questionId:{1}, verstion: {2}",
                    currentCourse.ProductCourseId, questionId, version), ex);
            }
        }

        public bool PublishDraftToOriginal(Course currentCourse, string draftQuestionId)
        {
            try
            {
                var draftQuestion = questionCommands.GetQuestion(currentCourse.QuestionRepositoryCourseId, draftQuestionId);
                if (!string.IsNullOrEmpty(draftQuestion.DraftFrom))
                {
                    var originalQuestion = questionCommands.GetQuestion(currentCourse.QuestionRepositoryCourseId,
                        draftQuestion.DraftFrom);
                    draftQuestion.Id = originalQuestion.Id;
                    ClearServiceFields(draftQuestion);
                    draftQuestion.IsPublishedFromDraft = true;
                    questionCommands.UpdateQuestion(draftQuestion);
                    DeleteDraft(currentCourse.QuestionRepositoryCourseId, draftQuestionId);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                StaticLogger.LogError(
                   string.Format("PublishDraftToOriginal: courseId: {0}, draftQuestionId:{1}", currentCourse.ProductCourseId, draftQuestionId), ex);
                return false;
            }
        }

        private void DeleteDraft(string questionRepositoryCourseId, string draftQuestionId)
        {
            UpdateSubDrafts(questionRepositoryCourseId, draftQuestionId);
            questionCommands.DeleteQuestion(questionRepositoryCourseId, draftQuestionId);
        }

        private void UpdateSubDrafts(string questionRepositoryCourseId, string draftQuestionId)
        {
            var question = questionCommands.GetQuestion(questionRepositoryCourseId, draftQuestionId);
            if (question != null)
            {
                var subDrafts = questionCommands.GetQuestionDrafts(questionRepositoryCourseId, question);
                foreach (var draft in subDrafts)
                {
                    draft.DraftFrom = question.DraftFrom;
                }
                questionCommands.UpdateQuestions(subDrafts, questionRepositoryCourseId);
            }
        }

        public Question CreateDraft(Course course, string questionId, string version = null)
        {
            Question question = GetQuestion(course, questionId, version);
            question.Id = Guid.NewGuid().ToString();
            question.Status = ((int)QuestionStatus.InProgress).ToString();
            if (question.ProductCourseSections.Count > 1)
            {
                question.ProductCourseSections.RemoveAll(s => s.ProductCourseId != course.ProductCourseId);
                question.ProductCourseSections.First().ParentProductCourseId = string.Empty;
                question.ProductCourseSections.First().Sequence = string.Empty;
            }
            ClearServiceFields(question);
            question.DraftFrom = questionId;
            return questionCommands.CreateQuestion(course.ProductCourseId, question);
        }

        public bool RemoveQuestion(Course course, string questionId)
        {
            try
            {
                Question question = GetQuestion(course, questionId);
                if (question != null &&
                    (question.Version == 1 && string.IsNullOrEmpty(question.DuplicateFrom) && string.IsNullOrEmpty(question.DuplicateFromShared))
                   )
                {
                    questionCommands.DeleteQuestion(course.QuestionRepositoryCourseId, questionId);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("RemoveQuestion: courseId: {0}, questionId:{1}", course.ProductCourseId, questionId), ex);
            }
        }

        public Question RestoreQuestionVersion(Course course, string questionId, string version)
        {
            var questionVersion = GetQuestion(course, questionId, version);
            if (questionVersion != null)
            {
                questionVersion.RestoredFromVersion = questionVersion.Version.ToString();
                var draftFrom = questionVersion.DraftFrom;
                ClearServiceFields(questionVersion);
                questionVersion.DraftFrom = draftFrom;
                questionCommands.UpdateQuestion(questionVersion);
            }
            return questionVersion;
        }

        private QuestionMetadataSection GetNewProductCourseSection(int courseIdToPublish, string bank, string chapter, Course currentCourse, Question question)
        {
            var courseToPublish = productCourseManagementService.GetProductCourse(courseIdToPublish.ToString());
            var newProductCourseValues = new QuestionMetadataSection()
            {
                ProductCourseId = courseIdToPublish.ToString(),
                Bank = bank,
                Chapter = chapter,
                ParentProductCourseId = currentCourse.ProductCourseId,
                Title = question.DefaultSection.Title,
                Sequence =  question.DefaultSection.Sequence
            };
            foreach (var defaultValue in question.DefaultSection.DynamicValues)
            {
                var fieldDescriptor = courseToPublish.FieldDescriptors.FirstOrDefault(f => f.Name == defaultValue.Key);
                if (fieldDescriptor != null && !newProductCourseValues.DynamicValues.ContainsKey(defaultValue.Key))
                {
                    var intersectValues = fieldDescriptor.CourseMetadataFieldValues.Any() ? defaultValue.Value.Intersect(fieldDescriptor.CourseMetadataFieldValues.Select(v => v.Text)) : defaultValue.Value;
                    newProductCourseValues.DynamicValues[defaultValue.Key] = intersectValues.ToList();
                }
            }
            return newProductCourseValues;
        }

        private QuestionMetadataSection GetDefaultSection(Question question, Course currentCourse)
        {
            var currentProductCourseSection = question.ProductCourseSections.First(s => s.ProductCourseId == currentCourse.ProductCourseId);

            var result = new QuestionMetadataSection
            {
                Title = currentProductCourseSection.Title,
                Bank = currentProductCourseSection.Bank,
                Chapter = currentProductCourseSection.Chapter,
                Sequence = currentProductCourseSection.Sequence
            };
            foreach (var fieldName in currentCourse.FieldDescriptors.Where(f => !MetadataFieldNames.GetStaticFieldNames().Contains(f.Name)).Select(f => f.Name))
            {
                var values = currentProductCourseSection.DynamicValues.ContainsKey(fieldName)
                    ? currentProductCourseSection.DynamicValues[fieldName]
                    : new List<string>();
                result.DynamicValues.Add(fieldName, values);
            }
            return result;
        }

        private Question GetNewQuestionTemplate(Course course, string questionType, string bank, string chapter)
        {
            var question = new Question();
            question.Id = Guid.NewGuid().ToString();
            question.EntityId = course.QuestionRepositoryCourseId;
            var metadataSection = new QuestionMetadataSection
            {
                ProductCourseId = course.ProductCourseId,
                Bank = bank,
                Chapter = chapter,
                Title = "New Question"
            };

            question.Score = 1;

            foreach (var field in course.FieldDescriptors.Where(field => !MetadataFieldNames.GetStaticFieldNames().Contains(field.Name) && !metadataSection.DynamicValues.ContainsKey(field.Name)))
            {
                metadataSection.DynamicValues.Add(field.Name, new List<string>());
            }
            question.ProductCourseSections.Add(metadataSection);
            question.Status = ((int)QuestionStatus.InProgress).ToString();
            question.Body = string.Empty;
            question.InteractionData = string.Empty;
            var type = QuestionTypeHelper.GetQuestionType(questionType);
            question.InteractionType = string.IsNullOrEmpty(type.Custom) ? type.Key : type.Custom;
            question.CustomUrl = string.IsNullOrEmpty(type.Custom) ? type.Custom : type.Key;
            return question;
        }

        private void ClearServiceFields(Question question)
        {
            question.IsPublishedFromDraft = false;
            question.DuplicateFromShared = string.Empty;
            question.DuplicateFrom = string.Empty;
            question.DraftFrom = string.Empty;
            question.Version = 0;
        }
    }
}