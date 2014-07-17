using System.Collections.Generic;
using System.Reflection;
using Macmillan.PXQBA.Business.QuestionParserModule.DataContracts;

namespace Macmillan.PXQBA.Business.QuestionParserModule
{
    public abstract class QuestionParserBase : IQuestionParser
    {
        protected ParsedQuestionType Type { get; set; }

        protected QuestionParserBase()
        {
            Type = ParsedQuestionType.MultipleChoice;
        }

        public abstract bool Recognize(string fileName);

        public abstract ValidationResult Parse(string fileName, byte[] file);
    }
}