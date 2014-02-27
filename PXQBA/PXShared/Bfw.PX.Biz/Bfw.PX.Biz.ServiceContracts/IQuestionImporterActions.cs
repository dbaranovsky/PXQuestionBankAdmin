using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.ServiceContracts
{
    public interface IQuestionImporterActions
    {
        List<RespondusQuestion> Parse(string data);
        List<RespondusQuestion> ValidateThruDLAP(string entityId, List<RespondusQuestion> questions, IQuestionActions questionActions);
        List<RespondusQuestion> Import(string entityId, List<RespondusQuestion> questions, string quizId, IQuestionActions questionActions);
    }
}
