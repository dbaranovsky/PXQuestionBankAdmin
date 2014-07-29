using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.QuestionParserModule.DataContracts
{
    /// <summary>
    /// Parsed question
    /// </summary>
    public class ParsedQuestion
    {
        /// <summary>
        /// Type of the question
        /// </summary>
        public ParsedQuestionType Type { get; set; }

        /// <summary>
        /// Unique question identifier 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Question's title (optional)
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Question's text (question itself)
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// General Feedback (optional)
        /// </summary>
        public string Feedback { get; set; }

        /// <summary>
        /// Attempt's point for the question (optional)
        /// </summary>
        public double? Points { get; set; }

        private List<ParsedQuestionChoice> choices;

        /// <summary>
        /// List of parsed answers for the question
        /// </summary>
        public List<ParsedQuestionChoice> Choices
        {
            get
            {
                if (choices == null)
                {
                    choices = new List<ParsedQuestionChoice>();
                }
                return choices;
            }
            set
            {
                choices = value;
            }
        }

        /// <summary>
        /// Flag identifying whether parsing is complete for the question
        /// </summary>
        public bool IsParsed { get; set; }

      

        /// <summary>
        /// Custom metadata of QTI questions
        /// </summary>
        public SerializableDictionary<string, List<string>> MetadataSection { get; set; }

    }
}
