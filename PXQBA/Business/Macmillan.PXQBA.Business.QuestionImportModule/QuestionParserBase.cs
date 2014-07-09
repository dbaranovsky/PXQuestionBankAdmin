using System.Collections.Generic;
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
        public abstract IEnumerable<ParsedQuestion> Parse(string data);
    }
}