using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Bfw.Agilix.Dlap.Session;



namespace Bfw.Agilix.DataContracts
{

    [DataContract]
    public class ItemAnalysisDetail : IDlapEntityParser
    {
        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        /// <value>
        /// The number.
        /// </value>
        [DataMember]
        public int Number { get; set; }

        /// <summary>
        /// Gets or sets the enrollments.
        /// </summary>
        /// <value>
        /// The enrollments.
        /// </value>
        [DataMember]
        public int Enrollments { get; set; }

        /// <summary>
        /// Gets or sets the attempts.
        /// </summary>
        /// <value>
        /// The attempts.
        /// </value>
        [DataMember]
        public int Attempts { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        [DataMember]
        public double Score { get; set; }

        /// <summary>
        /// Gets or sets the correlation.
        /// </summary>
        /// <value>
        /// The correlation.
        /// </value>        
        [DataMember]
        public float Correlation { get; set; }


        /// <summary>
        /// Gets or sets the rubric rule.
        /// </summary>
        /// <value>
        /// The rubric rules.
        /// </value>
        [DataMember]
        public RubricRule RubricRule { get; set; }

        /// <summary>
        /// Gets or sets the grades.
        /// </summary>
        /// <value>
        /// The grades.
        /// </value>
        [DataMember]
        public List<EnrollmentGrade> Grades { get; set; }


        /// <summary>
        /// Parses an XElement into internal object state. This allows for objects to be decomposed from
        /// parts of Dlap responses.
        /// </summary>
        /// <param name="element">element that contains the state to parse</param>
        public void ParseEntity(System.Xml.Linq.XElement element)
        {
            var number = element.Attribute(ElStrings.Number);
            var enrollmentCount = element.Attribute(ElStrings.Enrollments);
            var attempts = element.Attribute(ElStrings.Attempts);
            var score = element.Attribute(ElStrings.Score);
            var correlation = element.Attribute(ElStrings.Correlation);

            var rubricRule = element.Element(ElStrings.RubricRule);
            var grades = element.Element(ElStrings.Grades);
            var enrollments = grades.Elements(ElStrings.Enrollment);

            if (enrollments != null)
            {
                Grades = new List<EnrollmentGrade>();
                foreach (var grade in enrollments)
                {
                    var enrollmentId = grade.Attribute(ElStrings.EnrollmentId);
                    var achieved = grade.Attribute(ElStrings.Achieved);
                    var possible = grade.Attribute(ElStrings.possible);
                    Grades.Add(new EnrollmentGrade
                    {
                        EnrollmentId = enrollmentId != null ? enrollmentId.Value : null,
                        Achieved = Convert.ToInt32(achieved.Value),
                        Possible = Convert.ToInt32(possible.Value)
                    });
                }
            }

            if (rubricRule != null)
            {
                RubricRule = new RubricRule();
                var rubricRuleId = rubricRule.Attribute(ElStrings.Id);
                var max = rubricRule.Attribute(ElStrings.Max);
                if (null != rubricRuleId)
                {
                    RubricRule.Id = rubricRuleId.Value;
                }
                if (null != max)
                {
                    int maxValue;
                    if (Int32.TryParse(max.Value, out maxValue))
                    {
                        RubricRule.Max = maxValue;
                    }
                }
            }

            if (null != enrollmentCount)
            {
                Enrollments = Convert.ToInt32(enrollmentCount.Value);
            }

            if (null != attempts)
            {
                Attempts = Convert.ToInt32(attempts.Value);
            }

            if (null != score)
            {
                Single scoreValue;
                if (Single.TryParse(score.Value, out scoreValue))
                {
                    Score = scoreValue;
                }
            }

            if (null != correlation)
            {
                Correlation = Convert.ToSingle(correlation.Value);
            }

        }
    }
}
