using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IProductCourseOperation productCourseOperation;

        public QuestionManagementService(IQuestionCommands questionCommands, ITemporaryQuestionOperation temporaryQuestionOperation,  IProductCourseOperation productCourseOperation)
        {
            this.questionCommands = questionCommands;
            this.temporaryQuestionOperation = temporaryQuestionOperation;
            this.productCourseOperation = productCourseOperation;
        }

        public PagedCollection<Question> GetQuestionList(Course course, IEnumerable<FilterFieldDescriptor> filter, SortCriterion sortCriterion, int startingRecordNumber, int recordCount)
        {
            return questionCommands.GetQuestionList(course.QuestionRepositoryCourseId, course.ProductCourseId, filter, sortCriterion, startingRecordNumber, recordCount);
        }

        public Question CreateQuestion(Course course, string questiontype, string bank, string chapter)
        {
            Question question = GetNewQuestionTemplate(course, questiontype, bank, chapter);
            return questionCommands.CreateQuestion(question);
        }

        public Question GetQuestion(Course course, string questionId)
        {
            return questionCommands.GetQuestion(course.QuestionRepositoryCourseId, questionId);
        }

        public Question DuplicateQuestion(Course course, string questionId)
        {
            Question question = GetQuestion(course, questionId);
            question.Id = Guid.NewGuid().ToString();
            question.Status = ((int)QuestionStatus.InProgress).ToString();
            question.QuestionIdDuplicateFrom = questionId;
            return questionCommands.CreateQuestion(question);
        }

        private Question GetNewQuestionTemplate(Course course, string questionType, string bank, string chapter)
        {
            var question = new Question();
            question.Id = Guid.NewGuid().ToString();
            question.EntityId = course.QuestionRepositoryCourseId;
            var values = new Dictionary<string, List<String>>
                         {
                             {MetadataFieldNames.ProductCourse, new List<string> {course.ProductCourseId}},
                             {MetadataFieldNames.Bank, new List<string> {bank}},
                             {MetadataFieldNames.Chapter, new List<string> {chapter}},
                         };


            foreach (var field in course.FieldDescriptors.Where(field => !values.ContainsKey(field.Name)))
            {
                values.Add(field.Name, new List<string>());
            }
            question.DefaultValues = values.Skip(1).ToDictionary(item => item.Key, item => item.Value);
            question.ProductCourseSections.Add(new ProductCourseSection
                                               {
                                                   ProductCourseId = course.ProductCourseId,
                                                   ProductCourseValues = values
                                               });
            question.Status = ((int)QuestionStatus.InProgress).ToString();
            question.Body = string.Empty;
            question.InteractionData = string.Empty;
            var type = QuestionTypeHelper.GetQuestionType(questionType);
            question.InteractionType = string.IsNullOrEmpty(type.Custom) ? type.Key : type.Custom;
            question.CustomUrl = string.IsNullOrEmpty(type.Custom) ? type.Custom : type.Key;
            question.Answer = string.Empty;
            return question;
        }

        public Question UpdateQuestion(Course course, string sourceQuestionId, Question temporaryQuestion)
        {
            questionCommands.UpdateQuestion(temporaryQuestion);
            return temporaryQuestionOperation.CopyQuestionToSourceCourse(course.QuestionRepositoryCourseId, sourceQuestionId);
        }

        public bool UpdateQuestionField(Course course, string questionId, string fieldName, string fieldValue, bool isSharedField = false)
        {
            if (isSharedField)
            {
                return questionCommands.UpdateSharedQuestionField(course.QuestionRepositoryCourseId, questionId, fieldName, fieldValue);
            }
            return questionCommands.UpdateQuestionField(course.ProductCourseId, course.QuestionRepositoryCourseId, questionId, fieldName, fieldValue);
        }

        public Question CreateTemporaryQuestion(Course course, string questionId)
        {
            //PxTempQBAQuestion_115457_Essay
            //PxTempQBAQuestion_115457_Choice
            return temporaryQuestionOperation.CopyQuestionToTemporaryCourse(course.QuestionRepositoryCourseId, questionId);
        }



    
        public bool RemoveFromTitle(string[] questionsId, Course currentCourse)
        {
            bool isSuccess = questionCommands.RemoveFromTitle(questionsId, currentCourse.QuestionRepositoryCourseId, currentCourse.ProductCourseId);
            return isSuccess;
        }

        public bool PublishToTitle(string[] questionsId, int courseId, string bank, string chapter)
        {
            bool isSuccess = questionCommands.PublishToTitle(questionsId, courseId, bank, chapter);
            return isSuccess;
        }
    }
}