using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Contracts
{
    public interface IBulkOperationService
    {
        bool SetStatus(string[] questionIds, QuestionStatus status);
        bool RemoveFromTitle(string[] questionsId, Course currentCourse);
        bool PublishToTitle(string[] questionsId, int courseId, string bank, string chapter);
    }
}
