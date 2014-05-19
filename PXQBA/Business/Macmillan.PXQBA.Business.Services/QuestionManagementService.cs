﻿using System;
using System.Collections.Generic;
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
        private readonly IBulkOperation bulkOperation;
        private readonly IProductCourseOperation productCourseOperation;

        public QuestionManagementService(IQuestionCommands questionCommands, ITemporaryQuestionOperation temporaryQuestionOperation, IBulkOperation bulkOperation, IProductCourseOperation productCourseOperation)
        {
            this.questionCommands = questionCommands;
            this.temporaryQuestionOperation = temporaryQuestionOperation;
            this.bulkOperation = bulkOperation;
            this.productCourseOperation = productCourseOperation;
        }

        public PagedCollection<Question> GetQuestionList(Course course, IEnumerable<FilterFieldDescriptor> filter, SortCriterion sortCriterion, int startingRecordNumber, int recordCount)
        {
            questionCommands.UpdateQuestion(new Question());
            //productCourseOperation.GetQuestionList(course.ProductCourseId, filter, sortCriterion, startingRecordNumber, recordCount);
            //temporaryQuestionOperation.CopyQuestionToTemporaryCourse(course.ProductCourseId, "PxTempQBAQuestion_115457_Choice");
            return questionCommands.GetQuestionList(course.QuestionRepositoryCourseId, course.ProductCourseId, filter, sortCriterion, startingRecordNumber, recordCount);
        }

        public Question CreateQuestion(Course course, QuestionType questiontype, string bank, string chapter)
        {
            Question question = GetNewQuestionTemplate();
            question.Type = questiontype;
            //question.LocalMetadata.Bank = bank;
            //question.LocalMetadata.Chapter = chapter;
            return questionCommands.CreateQuestion(course.QuestionRepositoryCourseId, question);
        }

        public Question GetQuestion(Course course, string questionId)
        {
            return questionCommands.GetQuestion(course.QuestionRepositoryCourseId, questionId);
        }

        public Question DuplicateQuestion(Course course, string questionId)
        {
            Question question = GetQuestion(course, questionId);
            question.Id = Guid.NewGuid().ToString();
            //question.LocalMetadata.Status = QuestionStatus.InProgress;
            question.QuestionIdDuplicateFrom = questionId;
            return questionCommands.CreateQuestion(course.ProductCourseId, question);
        }

        private Question GetNewQuestionTemplate()
        {
            var question = new Question();
            //question.LocalMetadata = new QuestionStaticMetadata();
            //question.LocalMetadata.Status = QuestionStatus.InProgress;
            //question.LocalMetadata.Title = "New question";
            //question.Preview = "<h2>preview for test</h2>";
            //question.LocalMetadata.Chapter = "Chapter 1";
            //question.LocalMetadata.Bank = "End of Chapter Questions";
            //question.LocalMetadata.LearningObjectives = new List<LearningObjective>();
            return question;
        }

        public IEnumerable<QuestionType> GetQuestionTypesForCourse()
        {
            // \todo Populate with actual data
            var availableTypes = new List<QuestionType>
                {
                    QuestionType.MultipleChoice,
                    QuestionType.Matching,
                    QuestionType.MultipleAnswer,
                    QuestionType.ShortAnswer,
                    QuestionType.Essay,
                    QuestionType.GraphExcepcise,
                };
            return availableTypes;
        }

        public Question UpdateQuestion(Course course, string sourceQuestionId, Question temporaryQuestion)
        {
            var updatedTempQuestion = questionCommands.UpdateQuestion(temporaryQuestion);
            temporaryQuestionOperation.CopyQuestionToSourceCourse(course.QuestionRepositoryCourseId, sourceQuestionId);
            return questionCommands.UpdateQuestion(updatedTempQuestion);
        }

        /*
        public void CreateQuestion(string questionType)
        {
            var shortQuestionType = Question.QuestionTypeShortNameFromId(questionType);
            var question = new Question
            {
                Id = Guid.NewGuid().ToString(),
                EntityId = "6710",
                Type = shortQuestionType,
                InteractionType = Mapper.Map<InteractionType>(questionType)
            };
            if (shortQuestionType == "HTS" || shortQuestionType == "CUSTOM")
            {
                question.Text = "Advanced Question";
                question.CustomUrl = "HTS";
                question.Type = "CUSTOM";
            }

            if (shortQuestionType == "FMA_GRAPH")
            {
                question.Text = "Graphing exercise";
                question.CustomUrl = "FMA_GRAPH";
                question.Type = "CUSTOM";
            }
            question.InteractionType = Mapper.Map<InteractionType>(question.Type);
            SaveQuestion(question);
        }
         */

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
    }
}