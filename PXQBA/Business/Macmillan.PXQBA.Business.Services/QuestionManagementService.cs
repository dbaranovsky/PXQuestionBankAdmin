using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
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

        public PagedCollection<Question> GetQuestionList(string courseId, SortCriterion sortCriterion, int startingRecordNumber, int recordCount)
        {
            return questionCommands.GetQuestionList(courseId, sortCriterion, startingRecordNumber, recordCount);
        }

        public Question CreateQuestion(Question question)
        {
            throw new System.NotImplementedException();
        }

        public Question GetQuestion(string questionId)
        {
            throw new System.NotImplementedException();
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