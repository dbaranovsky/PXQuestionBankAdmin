using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;
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

        public Question CreateQuestion(Course course, string questiontype, string bank, string chapter)
        {
            Question question = GetNewQuestionTemplate(course, questiontype, bank, chapter);
            return questionCommands.CreateQuestion(course.ProductCourseId, question);
        }

        public Question GetQuestion(Course course, string questionId, string version = null)
        {
            return questionCommands.GetQuestion(course.QuestionRepositoryCourseId, questionId, version);
        }

        public Question DuplicateQuestion(Course course, string questionId, string version = null)
        {
            Question question = GetQuestion(course, questionId, version);
            question.Id = Guid.NewGuid().ToString();
            question.Status = ((int)QuestionStatus.InProgress).ToString();
            if (question.ProductCourseSections.Count > 1)
            {
                question.ProductCourseSections.RemoveAll(s => s.ProductCourseId != course.ProductCourseId);
                question.DuplicateFromShared = questionId;
            }
            question.DuplicateFrom = questionId;
            question.DraftFrom = string.Empty;
            return questionCommands.CreateQuestion(course.ProductCourseId, question);
        }

   

        public Question UpdateQuestion(Course course, string sourceQuestionId, Question temporaryQuestion)
        {
            questionCommands.UpdateQuestionInTempQuiz(temporaryQuestion);
            var question = temporaryQuestionOperation.CopyQuestionToSourceCourse(course.QuestionRepositoryCourseId, sourceQuestionId);
            questionCommands.ExecuteSolrUpdateTask();
            return question;
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
            return questionCommands.GetVersionHistory(currentCourse.QuestionRepositoryCourseId, questionId);
        }

        public Question GetTemporaryQuestionVersion(Course currentCourse, string questionId, string version)
        {
            return temporaryQuestionOperation.CopyQuestionToTemporaryCourse(currentCourse.QuestionRepositoryCourseId, questionId, version);
        }

        public bool PublishDraftToOriginal(Course currentCourse, string draftQuestionId)
        {
            var draftQuestion = questionCommands.GetQuestion(currentCourse.QuestionRepositoryCourseId, draftQuestionId);
            if (!string.IsNullOrEmpty(draftQuestion.DraftFrom))
            {
                var originalQuestion = questionCommands.GetQuestion(currentCourse.QuestionRepositoryCourseId,
                    draftQuestion.DraftFrom);
                draftQuestion.Id = originalQuestion.Id;
                draftQuestion.DraftFrom = string.Empty;
                questionCommands.UpdateQuestion(draftQuestion);
                questionCommands.DeleteQuestion(currentCourse.QuestionRepositoryCourseId, draftQuestionId);
                return true;
            }
            return false;
        }

        public Question CreateDraft(Course course, string questionId, string version = null)
        {
            Question question = GetQuestion(course, questionId, version);
            question.Id = Guid.NewGuid().ToString();
            question.Status = ((int)QuestionStatus.InProgress).ToString();
            if (question.ProductCourseSections.Count > 1)
            {
                question.ProductCourseSections.RemoveAll(s => s.ProductCourseId != course.ProductCourseId);
            }
            question.DuplicateFromShared = string.Empty;
            question.DuplicateFrom = string.Empty;
            question.DraftFrom = questionId;
            return questionCommands.CreateQuestion(course.ProductCourseId, question);
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
                Chapter = chapter
            };


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
    }
}