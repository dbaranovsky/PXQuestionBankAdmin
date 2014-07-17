using System.Collections.Generic;
using Macmillan.PXQBA.Business.QuestionParserModule.DataContracts;

namespace Macmillan.PXQBA.Business.QuestionParserModule
{
    public interface IQuestionParser
    {
        bool Recognize(string fileName);
       ValidationResult Parse(string fileName, byte[] file);
    }
}
