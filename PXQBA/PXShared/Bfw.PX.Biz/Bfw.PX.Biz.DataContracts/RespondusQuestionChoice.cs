using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    public class RespondusQuestionChoice
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
        /// Flag identifying whether answer is correct
        /// </summary>
        public bool IsCorrect { get; set; }

        /// <summary>
        /// Placeholder for parsing errors
        /// </summary>
        public string ValidationError { get; set; }
    }
}
