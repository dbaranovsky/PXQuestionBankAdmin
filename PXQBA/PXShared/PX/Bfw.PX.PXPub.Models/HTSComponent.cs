using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Bfw.Common.Collections;

namespace Bfw.PX.PXPub.Models
{
    public class HTSComponent
    {
        /// <summary>
        /// Gets or sets the url to the HTS player.
        /// </summary>
        /// <value>
        /// The url of the player.
        /// </value>
        public string PlayerUrl { get; set; }

        /// <summary>
        /// Gets or sets the url to the HTS editor.
        /// </summary>
        /// <value>
        /// The url of the editor.
        /// </value>
        public string EditorUrl { get; set; }

        /// <summary>
        /// Gets or sets the id of the Question.
        /// </summary>
        /// <value>
        /// The id of the Question.
        /// </value>
        public string QuestionId { get; set; }

        /// <summary>
        /// Gets or sets the id of the Quiz.
        /// </summary>
        /// <value>
        /// The id of the Quiz.
        /// </value>
        public string QuizId { get; set; }

        /// <summary>
        /// Gets or sets the id of the Entity.
        /// </summary>
        /// <value>
        /// The id of the Entity.
        /// </value>
        public string EntityId { get; set; }

        /// <summary>
        /// Gets or sets the id of the Enrollment.
        /// </summary>
        /// <value>
        /// The id of the Enrollment.
        /// </value>
        public string EnrollmentId { get; set; }

        /// <summary>
        /// Boolean flag to indicate if question should be converted.
        /// </summary>
        /// <value>
        /// True if question should be converted on load of the HTS editor. False otherwise.
        /// </value>
        public bool IsAdvancedConvert { get; set; }


        /// <summary>
        /// Boolean flag to indicate if Component is used Stand Alone
        /// </summary>
        /// <value>
        /// True if true component will not call PxQuiz setup hooks 
        /// </value>
        public bool UseAsStandalone { get; set; }         
    }
}
