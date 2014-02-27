using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Bfw.Common.Collections;

namespace Bfw.PX.PXPub.Models
{
    public class CustomQuestionComponent
    {
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
        /// Custom Question XML as stored in DLAP interaction data node.
        /// </summary>
        /// <value>
        /// Custom Question XML as stored in DLAP interaction data node.
        /// </value>
        public string CustomXML { get; set; }

        /// <summary>
        /// URL to the custom question editor service.
        /// </summary>
        /// <value>
        /// URL to the custom question editor service.
        /// </value>
        public string EditorURL { get; set; }         
    }
}
