using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Types of respondus questions
    /// </summary>
    public enum RespondusType
    {
        MultipleChoice,
        FillInBlank
    }

    public class RespondusQuestion
    {
        public RespondusQuestion()
        {
            Choices = new List<RespondusQuestionChoice>();
        }

        /// <summary>
        /// Type of the question
        /// </summary>
        public RespondusType Type { get; set; }

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

        /// <summary>
        /// List of parsed answers for the question
        /// </summary>
        public List<RespondusQuestionChoice> Choices { get; set; }

        /// <summary>
        /// Flag identifying whether parsing is complete for the question
        /// </summary>
        public bool IsParsed { get; set; }

        /// <summary>
        /// Placeholder for parsing errors
        /// </summary>
        public string ValidationError { get; set; }
    }
}
