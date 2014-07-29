using System.Collections.Generic;
using System.Reflection;
using Macmillan.PXQBA.Business.QuestionParserModule.DataContracts;

namespace Macmillan.PXQBA.Business.QuestionParserModule
{
    /// <summary>
    /// Base class for all parsers of the files with questions
    /// </summary>
    public abstract class QuestionParserBase : IQuestionParser
    {
        /// <summary>
        /// Parsing result
        /// </summary>
        protected ValidationResult Result { get; set; }

        /// <summary>
        /// Question that is being currently parsing
        /// </summary>
        protected ParsedQuestion CurrentQuestion { get; set; }
 
        /// <summary>
        /// Question type
        /// </summary>
        protected ParsedQuestionType Type { get; set; }

        protected QuestionParserBase()
        {
            Type = ParsedQuestionType.MultipleChoice;
        }

        public abstract bool Recognize(string fileName);

        public abstract ValidationResult Parse(string fileName, byte[] file);
    }
}