using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Models
{
    public class Submission
    {
        /// <summary>
        /// Name for the submission
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Which quiz the submission is for
        /// </summary>
        /// <value>
        /// The quiz id.
        /// </value>
        public string QuizId { get; set; }

        /// <summary>
        /// Which question the submission is for
        /// </summary>
        /// <value>
        /// The quiz id.
        /// </value>
        public string QuestionId { get; set; }

        /// <summary>
        /// What enrollment this submission belongs to
        /// </summary>
        /// <value>
        /// The enrollment id.
        /// </value>
        public string EnrollmentId { get; set; }

        /// <summary>
        /// Gets or sets the grade.
        /// </summary>
        /// <value>
        /// The grade.
        /// </value>
        public Grade Grade { get; set; }

        /// <summary>
        /// Score the student received for the submission, or null if not yet graded
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        public double? Score { get; set; }

        /// <summary>
        /// The date the submission occurred
        /// </summary>
        /// <value>
        /// The date submitted.
        /// </value>
        public DateTime DateSubmitted { get; set; }

        /// <summary>
        /// Body of the submission
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public string Body { get; set; }

        /// <summary>
        /// Data xml to be inserted with submission zip file.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public XDocument Data { get; set; }

        /// <summary>
        /// Data xml to be inserted with submission zip file.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public Stream StreamData { get; set; }

        /// <summary>
        /// Notes for the submission.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Submitted file for the submission.
        /// </summary>
        public string SubmittedFileName { get; set; }

        /// <summary>
        /// Word count of the submission
        /// </summary>
        /// <value>
        /// The word count.
        /// </value>
        public string WordCount { get; set; }

        /// <summary>
        /// Version of the submission
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the resource path.
        /// </summary>
        /// <value>
        /// The resource path.
        /// </value>
        public string ResourcePath { get; set; }

        /// <summary>
        /// Gets or sets the full name of the student.
        /// </summary>
        /// <value>
        /// The full name of the student.
        /// </value>
        public string StudentFullName { get; set; }

        /// <summary>
        /// Gets or sets the first name of the student.
        /// </summary>
        /// <value>
        /// The first name of the student.
        /// </value>
        public string StudentFirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the student.
        /// </summary>
        /// <value>
        /// The last name of the student.
        /// </value>
        public string StudentLastName { get; set; }

        /// <summary>
        /// Gets or sets the submitted date.
        /// </summary>
        /// <value>
        /// The submitted date.
        /// </value>
        public DateTime SubmittedDate { get; set; }

        /// <summary>
        /// Set of attempts for each question in a homework
        /// </summary>
        /// <value>
        /// The question attempts.
        /// </value>
        public IDictionary<string, IList<QuestionAttempt>> QuestionAttempts { get; set; }

        /// <summary>
        /// Gets or sets the submission attempt.
        /// </summary>
        /// <value>
        /// The submission attempt.
        /// </value>
        public IDictionary<string, SubmissionAttempt> SubmissionAttempts { get; set; }

        /// <summary>
        /// If true, then empty submissions are allowed
        /// </summary>
        public bool AllowEmptySumbission { get; set; }

        /// <summary>
        /// If true, this submission can not be modified.
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Status of the submission before and after instructor grades it.
        /// </summary>
        public SubmissionStatus SubmissionStatus { get; set; }
    }
   

    public class QuestionAttempt
    {
        /// <summary>
        /// ID of the question that was attempted
        /// </summary>
        /// <value>
        /// The question id.
        /// </value>
        public string QuestionId { get; set; }

        /// <summary>
        /// answer the user submitted for this attempt
        /// </summary>
        /// <value>
        /// The attempt answer.
        /// </value>
        public string AttemptAnswer { get; set; }

        /// <summary>
        /// points attributed for this question
        /// </summary>
        /// <value>
        /// The calculated points.
        /// </value>
        public string PointsComputed { get; set; }

        /// <summary>
        /// points available for this question
        /// </summary>
        /// <value>
        /// The available points.
        /// </value>
        public string PointsPossible { get; set; }

        /// <summary>
        /// ID of the attempt
        /// </summary>
        /// <value>
        /// The attempt id.
        /// </value>
        public string PartId { get; set; }

        /// <summary>
        /// Version of the attempt
        /// </summary>
        /// <value>
        /// The attempt version.
        /// </value>
        public string AttemptVersion { get; set; }
    }
}
