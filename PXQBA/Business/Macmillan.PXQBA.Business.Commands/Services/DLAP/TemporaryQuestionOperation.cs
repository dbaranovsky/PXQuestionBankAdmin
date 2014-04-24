using System;
using System.Collections.Generic;
using System.Linq;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Common.Helpers;

namespace Macmillan.PXQBA.Business.Commands.Services.DLAP
{
    public class TemporaryQuestionOperation : ITemporaryQuestionOperation
    {
        private readonly IContext businessContext;

        private const string ItemIdTemplate = "QBA_temp_quiz_{0}";

        private const string TemporaryCourseId = "200117";

        public TemporaryQuestionOperation(IContext businessContext)
        {
            this.businessContext = businessContext;
        }

        public void CopyQuestionToTemporaryQuiz(string sourceProductCourseId, string questionIdToCopy)
        {
            var question = GetQuestion(sourceProductCourseId, questionIdToCopy);
            if (question == null)
            {
                throw new NullReferenceException("There is no such question in the course");
            }
            CopyQuestionToTemporaryQuiz(question);
        }

        private Question CopyQuestionToTemporaryQuiz(Question questionToCopy)
        {
            var item = GetTemporaryQuiz();
            //questionToCopy.EntityId
            //var cmd = new PutQuestions();
            //cmd.Add(questionToCopy);
            //businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            return null;
        }

        private Item GetTemporaryQuiz()
        {
            var item = GetExistingTemporaryQuiz();
            if (item == null)
            {
                item = CreateTemporaryQuiz();
                if (item == null)
                {
                    throw new Exception("Temporary quiz creation failed");
                }
            }
            return item;
        }

        private Question GetQuestion(string productCourseId, string questionId)
        {
            var cmd = new GetQuestions();
            cmd.SearchParameters = new QuestionSearch
            {
                EntityId = productCourseId,
                QuestionIds = new List<string> {questionId}
            };
            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            return cmd.Questions.FirstOrDefault();
        }

        private Item GetExistingTemporaryQuiz()
        {
            var cmd = new GetItems();
            cmd.SearchParameters = new ItemSearch()
            {
                EntityId = TemporaryCourseId,
                ItemId = GetTemporaryQuizId()
            };
            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            return cmd.Items.FirstOrDefault();
        }

        private Item CreateTemporaryQuiz()
        {
            var cmd = new PutItems();
            cmd.Add(CreateTemporaryItem());
            businessContext.SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            return cmd.Items.FirstOrDefault();
        }

        private Item CreateTemporaryItem()
        {
            return new Item
            {
                EntityId = TemporaryCourseId,
                Id = GetTemporaryQuizId()
            };
        }

        private string GetTemporaryQuizId()
        {
            return String.Format(ItemIdTemplate, businessContext.CurrentUser.Id + "111");
        }
    }
}