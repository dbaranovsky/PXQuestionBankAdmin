using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Agilix.Dlap.Session;
using System.Xml.Linq;
using System.Globalization;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Represents the results of the http://dev.dlap.bfwpub.com/Docs/Command/GetQuestionAnalysis command.
    /// </summary>
    public class QuestionAnalysis : IDlapEntityParser
    {
        #region Properties

        /// <summary>
        /// Id of the question the statistics pertain to.
        /// </summary>
        public string QuestionId { get; set; }

        /// <summary>
        /// Version (represents how many times the question has been modified)
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Detail number of the question.
        /// </summary>
        public string QuestionNumber { get; set; }

        /// <summary>
        /// Number of enrollments that attempted the question.
        /// </summary>
        public int Enrollments { get; set; }

        /// <summary>
        /// Total number of attempts against the question.
        /// </summary>
        public int Attempts { get; set; }

        /// <summary>
        /// Average score of the question.
        /// </summary>
        public float Score { get; set; }

        /// <summary>
        /// Correlation coefficient between the student scores and how well they
        /// did on the overall assessment.
        /// </summary>
        public float Correlation { get; set; }

        /// <summary>
        /// Number of times the question was answered correctly.
        /// </summary>
        public int CorrectAnswerCount { get; set; }

        /// <summary>
        /// Average time it took to answer the question.
        /// </summary>
        public string AverageTime { get; set; }

        #endregion

        #region IDlapEntityParser Members

        /// <summary>
        /// Parses an XElement into internal object state. This allows for objects to be decomposed from
        /// parts of Dlap responses.
        /// </summary>
        /// <param name="element">element that contains the state to parse</param>
        /// <remarks></remarks>
        public void ParseEntity(System.Xml.Linq.XElement element)
        {
            var questionid = element.Element(ElStrings.question).Attribute(ElStrings.QuestionId);
            var version = element.Element(ElStrings.question).Attribute(ElStrings.Version);
            var question_number = element.Attribute(ElStrings.Number);
            var enrollments = element.Attribute(ElStrings.Enrollments);
            var attempts = element.Attribute(ElStrings.Attempts);
            var score = element.Element(ElStrings.question).Attribute(ElStrings.Score);
            var correlation = element.Attribute(ElStrings.Correlation);
            var averagetime = element.Attribute(ElStrings.Seconds);
            
            XAttribute correct_count = null;
            var answers = element.Elements(ElStrings.answer);

            foreach (var answer in answers)
            {
                var correct = answer.Attribute(ElStrings.Correct);
                if (correct != null && correct.Value == "true")
                {
                    correct_count = answer.Attribute(ElStrings.Count);
                }
            }


            QuestionId = (questionid != null)? questionid.Value: null;
            Version = (version != null) ? version.Value : null;
            QuestionNumber = (question_number != null) ? question_number.Value : null;

            if (null != enrollments)
            {
                Enrollments = Convert.ToInt32(enrollments.Value);
            }

            if (null != attempts)
            {
                Attempts = Convert.ToInt32(attempts.Value);
            }

            if (null != score)
            {
                Score = Convert.ToSingle(score.Value);
            }

            if (null != correlation)
            {
                Correlation = Convert.ToSingle(correlation.Value);
            }

            if (null != correct_count)
            {
                CorrectAnswerCount = Convert.ToInt32(correct_count.Value);    
            }

            if (null != averagetime)
            {
                int totalSeconds = Convert.ToInt32(Math.Round(Convert.ToSingle(averagetime.Value)));                                
                string minute = (totalSeconds / 60).ToString("00");
                totalSeconds = (totalSeconds % 60);
                string seconds = totalSeconds.ToString("00");                

                AverageTime = string.Format("{0}:{1}", minute, seconds);
            }

        }

        #endregion
    }
}
