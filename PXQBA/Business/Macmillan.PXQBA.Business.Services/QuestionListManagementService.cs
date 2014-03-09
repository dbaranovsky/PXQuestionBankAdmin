using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bfw.Agilix.Dlap.Session;
using Macmillan.PXQBA.Business.Contracts;

namespace Macmillan.PXQBA.Business.Services
{
    /// <summary>
    /// Represents the service that handles operations with question lists
    /// </summary>
    public class QuestionListManagementService : IQuestionListManagementService
    {
        private readonly IContext businessContext;

        public QuestionListManagementService(IContext businessContext)
        {
            this.businessContext = businessContext;
        }
    }
}
