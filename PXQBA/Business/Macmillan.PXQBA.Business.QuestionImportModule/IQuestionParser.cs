using System.Collections.Generic;
using Macmillan.PXQBA.Business.QuestionParserModule.DataContracts;

namespace Macmillan.PXQBA.Business.QuestionParserModule
{
    public interface IQuestionParser
    {
        IEnumerable<ParsedQuestion> Parse(string data);
    }
}
