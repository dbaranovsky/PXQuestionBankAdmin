using System;
using System.Collections.Generic;
using AutoMapper;
using Bfw.Agilix.Commands;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Question = Macmillan.PXQBA.Business.Models.Question;
using Agx = Bfw.Agilix.DataContracts;


namespace Macmillan.PXQBA.Business.Services
{
    /// <summary>
    /// Service that handles operations with questions entities
    /// </summary>
    public class QuestionManagementService : IQuestionManagementService
    {
        private readonly IContext businessContext;

        public QuestionManagementService(IContext businessContext)
        {
            this.businessContext = businessContext;
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
        public void SaveQuestions(List<Question> questions)
        {
            var cmd = new PutQuestions();
            cmd.Add(Mapper.Map<IList<Agx.Question>>(questions));

            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
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