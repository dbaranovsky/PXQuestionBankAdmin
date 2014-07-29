namespace Macmillan.PXQBA.Business.QuestionParserModule.DataContracts
{
    /// <summary>
    /// Parsed question choice
    /// </summary>
    public class ParsedQuestionChoice
    {
        /// <summary>
        /// Unique answer identifier within a question
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Answer text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Feedback per choice
        /// </summary>
        public string Feedback { get; set; }

        /// <summary>
        /// Correct match for this choice (used in matching questions)
        /// </summary>
        public string Answer { get; set; }

        /// <summary>
        /// Flag identifying whether answer is correct
        /// </summary>
        public bool IsCorrect { get; set; }

        /// <summary>
        /// Placeholder for parsing errors
        /// </summary>
        public string ValidationError { get; set; }
    }
}