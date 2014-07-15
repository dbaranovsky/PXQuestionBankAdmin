using System.Collections.Generic;
using System.Reflection;
using Macmillan.PXQBA.Business.QuestionParserModule.DataContracts;

namespace Macmillan.PXQBA.Business.QuestionParserModule
{
    public abstract class QuestionParserBase : IQuestionParser
    {
        protected List<ParsedQuestion> QuestionList { get; set; }
       
        protected ParsedQuestionType Type { get; set; }

        protected QuestionParserBase()
        {
            Type = ParsedQuestionType.MultipleChoice;
            QuestionList = new List<ParsedQuestion>();
        }

        public abstract bool Recognize(string fileName);

        public abstract IEnumerable<ParsedQuestion> Parse(byte[] file);
    }
}