using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Agilix.Dlap;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Represents an possible answer in a multiple choice or matching question.
    /// </summary>
    /// <summary>
    /// Question Interaction object 
    /// http://dlap.bfwpub.com/js/docs/#!/Schema/Question#schema-attr-question-interaction-type
    /// </summary>
    [Serializable]
    public class QuestionInteraction
    {

        /// <summary>
        /// An InteractionFlags value that control question behaviors such as whether it displays a workspace or is an extra-credit question.
        /// </summary>
        public QuestionInteractionFlags Flags { get; set; }


        /// <summary>
        /// Text entry width for text (fill-in-the-blank) questions.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Text entry height for essay questions.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Number of significant figures for the answer.
        /// </summary>
        public int SignificantFigures { get; set; }

        /// <summary>
        /// texttype defines the algorithm that determines correctness in text questions. The default is Normal.
        ///  Normal = exact text match
        ///  IgnoreCase = case-insensitive text match
        ///  Numeric = numeric match
        ///  MatchFunction = normalized expression tree match - 
        ///  a+b == b+a , a+a <> 2a
        ///  EquivalentFunction = expression evaluation match - 
        ///  a+b == b+a , a+a == 2a
        ///  RegularExpression = regular expression match
        /// </summary>
        public string TextType { get; set; }

        /// <summary>
        /// interaction defines the question type:
        ///  choice = multiple choice
        ///  match = matching
        ///  answer = multiple answer
        ///  text = fill in the blank
        ///  essay = long answer/essay question
        ///  composite = composite question
        ///  custom = custom question
        /// </summary>
        public string Type { get; set; }


    }
}
