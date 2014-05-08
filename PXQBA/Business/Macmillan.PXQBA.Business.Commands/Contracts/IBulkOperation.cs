using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Commands.Contracts
{
    public interface IBulkOperation
    {
        bool SetStatus(string[] questionId, QuestionStatus status);
        bool RemoveFromTitle(string[] questionsId, string productCourseId);
        bool PublishToTitle(string[] questionsId, int courseId, string bank, string chapter);
    }
}
