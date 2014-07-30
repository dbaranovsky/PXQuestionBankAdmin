using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Question choice that is used in question body
    /// </summary>
    public class QuestionChoice
    {
        /// <summary>
        /// Choice id
        /// </summary>
        public string Id;

        /// <summary>
        /// Choice display text
        /// </summary>
        public string Text;

        /// <summary>
        /// Choice feedback
        /// </summary>
        public string Feedback;

        /// <summary>
        /// Choice correct answer
        /// </summary>
        public string Answer;
    }
}
