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

        public QuestionManagementService(IQuestionCommands questionCommands, ITemporaryQuestionOperation temporaryQuestionOperation)
        {
            this.questionCommands = questionCommands;
            this.temporaryQuestionOperation = temporaryQuestionOperation;
        }

        public PagedCollection<Question> GetQuestionList(SortCriterion sortCriterion, int startingRecordNumber, int recordCount)
        {
            //temporaryQuestionOperation.CopyQuestionToTemporaryQuiz("12c19f3103ad4da1b254dd67f17dd1b1");
            return questionCommands.GetQuestionList(CacheProvider.GetCurrentTitleId(), sortCriterion, startingRecordNumber, recordCount);
        }

        public Question CreateQuestion(QuestionType questiontype, string bank, string chapter)
        {
            Question question = GetNewQuestionTemplate();
            question.Type = questiontype;
            question.Bank = bank;
            question.Chapter = chapter;
            return questionCommands.CreateQuestion(CacheProvider.GetCurrentTitleId(), question);
        }

        public Question GetQuestion(string questionId)
        {
            return questionCommands.GetQuestion(questionId);
        }

        public Question DuplicateQuestion(string questionId)
        {
            Question question = GetQuestion(questionId);
            question.Id = Guid.NewGuid().ToString();
            question.Status = QuestionStatus.InProgress;
            return questionCommands.CreateQuestion(CacheProvider.GetCurrentTitleId(), question);
        }

        private Question GetNewQuestionTemplate()
        {
            var question = new Question();
            question.Status = QuestionStatus.InProgress;
            question.Title = "New question";
            question.Preview = "<h2>preview for test</h2>";
            question.Chapter = "Chapter 1";
            question.Bank = "End of Chapter Questions";
            return question;
        }

        public void UpdateQuestionSequence(string questionId, int newSequenceValue)
        {
            questionCommands.UpdateQuestionSequence(CacheProvider.GetCurrentTitleId(), questionId, newSequenceValue);
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

        public Question UpdateQuestion(Question question)
        {
            return questionCommands.UpdateQuestion(question);
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

        public bool UpdateQuestionField(string questionId, string fieldName, string fieldValue)
        {
            return questionCommands.UpdateQuestionField(questionId, fieldName, fieldValue);
        }
    }
}