using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Available types for Submission Actions
    /// </summary>
    public enum SubmissionActionType
    {
        na, // default if type is not found in the xml
        start,
        save,
        resume,
        submit
    }
    /// <summary>
    /// Contains the string representations of the available Resource content-types
    /// </summary>
    public static class ResourceContentType
    {
        /// <summary>
        /// DLAP attributed resource MIME type.
        /// </summary>
        public const string Attributed = "application/x-dlap-attributed-resource";

        /// <summary>
        /// DLAP metadata MIME type.
        /// </summary>
        public const string Metadata = "application/x-dlap-resource-meta-xml";

        /// <summary>
        /// DLAP package MIME type.
        /// </summary>
        public const string Package = "application/x-dlap-resource-zip-package";

        /// <summary>
        /// Standard text/html MIME type.
        /// </summary>
        public const string Html = "text/html";

        public const string Csv = "text/csv";

        public const string Pdf = "application/pdf";
    }

    /// <summary>
    /// course sub type used in "meta-bfw_course_subtype" under "Course" XML
    /// </summary>
    public static class CourseSubType
    {
        /// <summary>
        /// "generic" DLAP attribute flag
        /// </summary>
        public const string Generic = "generic";

        /// <summary>
        /// "dashboard" DLAP attribute flag
        /// </summary>
        public const string Dashboard = "dashboard";
    }

    /// <summary>
    /// Determines whether or not a resource is available to students
    /// </summary>
    public enum ResourceStatus
    {
        /// <summary>
        /// Resource is visible to students.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Resource is hidden from students.
        /// </summary>
        Hidden = 1
    }

    /// <summary>
    /// Determines status of enrollment 
    /// </summary>
    public enum EnrollmentStatus
    {
        /// <summary>
        /// No status
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// User is actively enrolled.
        /// </summary>
        Active = 1,

        /// <summary>
        /// Applies only to student enrollments in courses or sections. Student has withdrawn from a course and receives no score. For example, the student withdrew from the course while within the grace period for withdrawals.
        /// </summary>
        Withdrawn = 4,
        
        /// <summary>
        /// Applies only to student enrollments in courses or sections. Student has withdrawn from the course and failed it. For example, the student dropped out after the grace period for withdrawals has passed.
        /// </summary>
        WithdrawnFailed = 5,

        /// <summary>
        /// Applies only to student enrollments in courses or sections. Student has withdrawn from the course because the student transferred away. Student receives no final score.
        /// </summary>
        Transferred = 6,

        /// <summary>
        /// Applies only to student enrollments in courses or sections. Student has completed the course and has received a passing final score.
        /// </summary>
        Completed = 7,

        /// <summary>
        /// Applies only to student enrollments in courses or sections. Student has completed the course but receives no credit for completing it.
        /// </summary>
        CompletedNoCredit = 8,

        /// <summary>
        /// User’s enrollment is currently suspended. User cannot participate until re-instated to the Active status.
        /// </summary>
        Suspended = 9,

        /// <summary>
        /// User’s enrollment is inactive.
        /// </summary>
        Inactive = 10,
    }

    /// <summary>
    /// Determines whether the course content is indexed and, therefore, searchable with the Search command. 
    /// Used in CopyCourses command.
    /// </summary>
    public enum IndexRule
    {
        Nothing = 0,
        Everything = 1,
        Deltas = 2,
    };

    /// <summary>
    /// Determines when an item recieves a score
    /// 0	Minutes:	        Completion is determined by the number of minutes spent on the activity.
    /// 1	Submission: 	    Completion is determined by a submission, post, or other action from the student.
    /// 2	PassingScore:	    Completion is determined by a passing score on the activity.
    /// </summary>
    public enum CompletionTrigger
    {
        Minutes = 0,
        Submission = 1,
        PassingScore = 2
    };

    /// <summary>
    /// Determines how a course's content is copied into a new course
    /// </summary>
    public struct CopyMethod
    {
        /// <summary>
        /// Copy all items and break propagation of changes.
        /// </summary>
        public const string Copy = "Copy";

        /// <summary>
        /// Don't copy the items, just inherit them.
        /// </summary>
        public const string Derivative = "Derivative";

        /// <summary>
        /// Copy all items, but maintain propagation of changes.
        /// </summary>
        public const string DerivativeCopy = "DerivativeCopy";

        /// <summary>
        /// Copy.
        /// </summary>
        public const string Default = "Copy";
    }

    /// <summary>
    /// Represents the DLAP DropboxType enum
    /// </summary>
    public enum DropBoxType
    {
        /// <summary>
        /// Accept a single document.
        /// </summary>
        SingleDocument = 0,

        /// <summary>
        /// Accepts a document template.
        /// </summary>
        DocumentTemplate,

        /// <summary>
        /// Accepts multiple documents.
        /// </summary>
        MultipleDocuments,

        /// <summary>
        /// Accepts only notes to be entered.
        /// </summary>
        NotesOnly,

        /// <summary>
        /// Accepts a URL to be entered.
        /// </summary>
        Url,

        /// <summary>
        /// Accepts an audio recording to be made.
        /// </summary>
        AudioRecording
    }

    /// <summary>
    /// Represents the type of Submission
    /// * attempt - This submission is for an exam.
    /// * assignment - This submission is for an assignment.
    /// * homework - This submssion if for a homework item.
    /// </summary>
    public enum SubmissionType
    {
        /// <summary>
        /// Submission of an assessment.
        /// </summary>
        Attempt,

        /// <summary>
        /// Submission for a general assignment.
        /// </summary>
        Assignment,

        /// <summary>
        /// Submission of a Homework item.
        /// </summary>
        Homework
    }

    /// <summary>
    /// Respresents type of response when teacher submitting response.
    /// </summary>
    public enum TeacherResponseType
    {
        /// <summary>
        /// Default, empty, value.
        /// </summary>
        None,

        /// <summary>
        /// Submission response.
        /// </summary>
        Submission,

        /// <summary>
        /// Response for a specific rubric item.
        /// </summary>
        RubricRow,

        /// <summary>
        /// Response for Likert scale.
        /// </summary>
        LikertQuestion
    }

    /// <summary>
    /// Respresents status of the grade when teacher grades.
    /// </summary>
    [Flags]
    public enum GradeStatus
    {
        /// <summary>
        /// Default, empty, value.
        /// </summary>
        None = 0,

        /// <summary>
        /// Assignment is marked as compelted.
        /// </summary>
        Completed = 1,

        //For Future Use = 2,

        /// <summary>
        /// Student may view score.
        /// </summary>
        ShowScore = 4,

        /// <summary>
        /// Student may resubmit the assignment.
        /// </summary>
        AllowResubmission = 8,

        /// <summary>
        /// Due date is not to being enforced.
        /// </summary>
        IgnoreDueDate = 16,

        /// <summary>
        /// Teach submitted on behalf of student.
        /// </summary>
        SubmittedByTeacher = 32,

        /// <summary>
        /// Assignment grading has started.
        /// </summary>
        Started = 64,

        /// <summary>
        /// Grade is excluded from final grade.
        /// </summary>
        Excluded = 128,

        /// <summary>
        /// Grade has been release.
        /// </summary>
        Released = 256,

        /// <summary>
        /// Grade counts as extra credit.
        /// </summary>
        ExtraCredit = 512,

        /// <summary>
        /// Assignment needs to be graded.
        /// </summary>
        NeedsGrading = 1024
    }

    /// <summary>
    /// Question delivery modes for quiz settings
    /// </summary>
    public enum QuestionDelivery
    {
        All = 0,
        One = 1,
        OneNoBacktrack = 2
    }

    /// <summary>
    /// Flags to represent certain properties of an exam.
    /// </summary>
    [Flags]
    public enum ExamFlags
    {
        None                 = 0,
        AllowMultipleAttemps = 1,
        ShuffleQuestions     = 2,
        ShuffleAnswers       = 4,
        ShowCorrectQuestion  = 8,
        ShowCorrectAnswer    = 16,
        Formative            = 32,
        Remediation          = 64,
        HideReview           = 128,
        AllowSaveAndContinue = 256,
        KeepParametersSame   = 512,
        RequireAnswers       = 1024,
        ShowCorrectChoice    = 2048,
        ShowFeedback         = 4096,
        ForwardNavigation    = 8192
    }

  

   public enum SubmissionGradeAction
    {
        NotSet = -1,
        Default = 0,
        Manual = 1,
        Full_Credit = 2,
    };

    /// <summary>
    /// Scored attempt levels for quiz settings
    /// </summary>
    public enum GradeRule
    {
        Last = 0,
        First = 1,
        Highest = 2,
        Lowest = 3,
        Average = 4,
        Total = 5
    }
    /// <summary>
    /// Quiz/Homework review settings
    /// </summary>
    public enum ReviewSetting
    {
        Each    = 0,
        DueDate = 1,
        Never   = 2,
        Second  = 3,
        Final   = 4
    }

    /// <summary>
    /// Grade Flags settings
    /// </summary>
    [Flags]
    public enum GradeFlags
    {
        None = 0x0,
        ExtraCredit = 0x1,
        ExcludeFromFinalGrade = 0x2,
        NoDrop = 0x4,
        PassingScoreRequired = 0x8,
        ZeroUnscored = 0x10,
    };

    /// <summary>
    /// GetEnrollment3 select flags
    /// </summary>
    [Flags]
    public enum EnrollmentSelect
    {
        [Description("course")]
        Course = 0x0,

        [Description("course.data")]
        CourseData = 0x1,

        [Description("domain")]
        Domain = 0x2,

        [Description("domain.data")]
        DomainData = 0x4,

        [Description("user")]
        User = 0x8,

        [Description("user.data")]
        UserData = 0x10, 
    }
}
