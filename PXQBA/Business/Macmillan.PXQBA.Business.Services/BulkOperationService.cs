using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Services
{
    public class BulkOperationService : IBulkOperationService
    {
        private readonly IBulkOperation bulkOperation;

        public BulkOperationService(IBulkOperation bulkOperation)
        {
            this.bulkOperation = bulkOperation;
        }

        public bool SetStatus(string[] questionIds, QuestionStatus status)
        {
            return bulkOperation.SetStatus(questionIds, status);
        }
    }
}
