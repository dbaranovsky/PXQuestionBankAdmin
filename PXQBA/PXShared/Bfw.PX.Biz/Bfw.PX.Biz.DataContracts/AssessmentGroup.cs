using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Represents Group Settings for assessments.
    /// </summary>
    [DataContract]
    public class AssessmentGroup
    {
        /// <summary>
        /// Gets or sets the name the group.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the number of attempts the student may submit for this group. 
        /// The value 0 means that the user may use an unlimited amount of attempts.
        /// </summary>
        /// <value>
        /// The attempts.
        /// </value>
        [DataMember]
        public string Attempts { get; set; }

        /// <summary>
        /// Gets or sets the Timelimit in minutes, that a student may spend on this question group.
        /// </summary>
        /// <value>
        /// The time limit.
        /// </value>
        [DataMember]
        public string TimeLimit { get; set; }

        /// <summary>
        /// Flag for displaying questions in scrambled order.
        /// </summary>
        /// <value>
        /// The scrambled flag.
        /// </value>
        [DataMember]
        public string Scrambled { get; set; }

        /// <summary>
        /// Defines the homework group behavior, such as showing correct answer, displaying question feedback.
        /// </summary>
        /// <value>
        /// Any value of <see cref="HomeworkGroupFlags" />
        /// </value>
        [DataMember]
        public HomeworkGroupFlags Review { get; set; }

        /// <summary>
        /// Defines which attempt should be used to calculate the score for this group.
        /// </summary>
        /// <value>
        /// Any value of <see cref="submissiongradeaction" />
        /// </value>
        [DataMember]
        public SubmissionGradeAction SubmissionGradeAction { get; set; }

        /// <summary>
        /// Flag for whether to display question hints.
        /// </summary>
        /// <value>
        /// The hints value.
        /// </value>
        [DataMember]
        public string Hints { get; set; }

        /// <summary>
        /// The question review settings
        /// </summary>
        [DataMember]
        public ReviewSettings ReviewSettings { get; set; }
    }
}
