using System.Collections.Generic;
using Macmillan.PXQBA.Business.QuestionParserModule.DataContracts;

namespace Macmillan.PXQBA.Business.QuestionParserModule
{
    public interface IQuestionParser
    {
        bool Recognize();
        IEnumerable<ParsedQuestion> Parse(string data);
    }
}
