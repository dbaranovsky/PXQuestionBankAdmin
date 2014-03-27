using AutoMapper;
using Bfw.Common.Logging;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Commands.Services.SQL;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.DataAccess.Data;
using System;
using System.Collections.Generic;
using InteractionType = Macmillan.PXQBA.Business.Models.InteractionType;
using Question = Macmillan.PXQBA.Business.Models.Question;


namespace Macmillan.PXQBA.Business.Services
{
    /// <summary>
    /// Service that handles operations with questions entities
    /// </summary>
    public class QuestionManagementService : IQuestionManagementService
    {
        private readonly IQuestionCommands questionCommands;

        public QuestionManagementService(IQuestionCommands questionCommands)
        {
            this.questionCommands = questionCommands;
        }

        /// <summary>
        /// Saves question to dlap db
        /// </summary>
        /// <param name="question">Question to save</param>
        public void SaveQuestion(Question question)
        {
            SaveQuestions(new List<Question>() { question });
        }

        /// <summary>
        /// Saves batch of questions to dlap db
        /// </summary>
        /// <param name="questions">Question to save</param>
        public void SaveQuestions(IList<Question> questions)
        {
            var qc = new QuestionCommands(new QBAUow(new QBADummyModelContainer(), new NullLogger()));
            qc.SaveQuestions(questions);
        }

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
    }
}