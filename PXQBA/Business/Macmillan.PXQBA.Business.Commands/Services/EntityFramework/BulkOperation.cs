using System.Linq;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.DataAccess.Data;

namespace Macmillan.PXQBA.Business.Commands.Services.EntityFramework
{
    public class BulkOperation : IBulkOperation
    {
        private readonly QBADummyModelContainer dbContext;

        public BulkOperation(QBADummyModelContainer dbContext)
        {
            this.dbContext = dbContext;
        }

        public bool SetStatus(string[] questionId, QuestionStatus status)
        {
            var questions = dbContext.Questions.Where(q => questionId.Contains(q.DlapId));
            foreach (var question in questions)
            {
                question.Status = (int)status;
            }
            dbContext.SaveChanges();
            return true;
        }
    }
}