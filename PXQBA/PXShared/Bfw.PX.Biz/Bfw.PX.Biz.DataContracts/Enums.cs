using System;
using System.ComponentModel;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Available types for Submission Actions
    /// </summary>
    public enum SubmissionActionType
    {
        na, // default value if type is not found in the xml
        start,
        save,
        resume,
        submit
    }

    /// <summary>
    /// Available types for ContentItem properties.
    /// </summary>
    public enum PropertyType
    {
        String = 0,
        Integer,
        Float,
        DateTime,
        Xml,
        Html,
        Boolean
    }

    /// <summary>
    /// Determines the type of bookmark.
    /// </summary>
    public enum EPortfolioType
    {
        Item = 0,
        Folder = 1
    }

    /// <summary>
    /// Contains the string representations of the available Resource content-types.
    /// </summary>
    public static class ResourceContentType
    {
        public const string Attributed = "application/x-dlap-attributed-resource";
        public const string Metadata = "application/x-dlap-resource-meta-xml";
        public const string Package = "application/x-dlap-resource-zip-package";
        public const string Html = "text/html";
    }

    /// <summary>
    /// Determines how a course's content is copied into a new course.
    /// </summary>
    public struct CopyMethod
    {
        public const string Copy = "Copy";
        public const string Derivative = "Derivative";
        public const string DerivativeCopy = "DerivativeCopy";
        public const string Default = "Copy";
        public const string StaticCopy = "StaticCopy";
        public const string DerivativeSiblingCopy = "DerivativeSiblingCopy";
    }

    /// <summary>
    /// Determines whether or not a resource is available to students.
    /// </summary>
    public enum ResourceStatus
    {
        Normal = 0,
        Hidden = 1
    }

    /// <summary>
    /// Represents the types of drop boxes.
    /// </summary>
    public enum DropBoxType
    {
        None = -1,
        SingleDocument = 0,
        DocumentTemplate = 1,
        MultipleDocuments = 2,
        NotesOnly = 3,
        Url = 4,
        AudioRecording = 5
    }

    /// <summary>
    /// Flags to represt Home work group.
    /// </summary>
    public enum HomeworkGroupFlags
    {
        Off = 0, // None
        CorrectAndFeedback = 1, // KeepTogether
        CorrectOnly = 2048, // ShowCorrectChoice       
        FeedbackOnly = 4096 // ShowFeedback
    }

    /// <summary>
    /// Flags to represent certain properties of an exam.
    /// </summary>
    [Flags]
    public enum ExamFlags
    {
        None = 0,
        AllowMultipleAttemps = 1,
        ShuffleQuestions = 2,
        ShuffleAnswers = 4,
        ShowCorrectQuestion = 8,
        ShowCorrectAnswer = 16,
        Formative = 32,
        Remediation = 64,
        HideReview = 128,
        AllowSaveAndContinue = 256,
        KeepParametersSame = 512
    }

    /// <summary>
    /// Types of Question interactions.
    /// </summary>
    public enum InteractionType
    {
        Bank = -1,
        Choice = 0,
        Match = 1,
        Answer = 2,
        Text = 3,
        Essay = 4,
        Composite = 5,
        Custom = 6,
        NotBank = 7

    }

    /// <summary>
    /// Represents the type of Submission.
    /// * Attempt - This submission is for an exam.
    /// * Assignment - This submission is for an assignment.
    /// * Homework - This submssion if for a homework item.
    /// </summary>
    public enum SubmissionType
    {
        Attempt,
        Assignment,
        Homework
    }

    /// <summary>
    /// Respresents type of response when teacher submitting response.
    /// </summary>
    public enum TeacherResponseType
    {
        None,
        Submission,
        RubricRow,
        PeerRubricRow,
        LikertQuestion
    }

    /// <summary>
    /// Respresents status of the grade when teacher grades.
    /// </summary>
    [Flags]
    public enum GradeStatus
    {
        None = 0,
        Completed = 1,
        //For Future Use = 2,
        ShowScore = 4,
        AllowResubmission = 8,
        IgnoreDueDate = 16,
        SubmittedByTeacher = 32,
        Started = 64,
        Excluded = 128,
        Released = 256,
        ExtraCredit = 512,
        NeedsGrading = 1024
    }

    /// <summary>
    /// Represents status of the assignment submission of the student and the response of the instructor before and after he grades it.
    /// </summary>
    public enum SubmissionStatus
    {
        Saved,
        Submitted,
        Completed,
        NotCompleted,
        NotGraded,
        Graded,
        Unsubmitted
    }

    /// <summary>
    /// Represents extended property types attached to an XmlResource. 
    /// </summary>
    public enum ResourceExtendedProperty
    {
        Status,
        WordCount,
        ItemId,
        AssignmentId,
        Comment,
        FileName,
        FileSize
    }

    /// <summary>
    /// Represent a user role in a course.
    /// </summary>
    public enum UserType
    {
        Instructor,
        Student,
        All
    }

    /// <summary>
    /// Represents the type of an item attached to an ePortfolio.
    /// </summary>
    public enum EportfolioItemType
    {
        Comment,
        PeerReview,
        Resource,
        Item
    }

    /// <summary>
    /// Represents the type of copy
    /// </summary>
    public enum EportfolioItemCopyType
    {
        Undefined,
        EportfolioToEportfolio,
        EportfolioToPresentation,
        EportfolioToDashboard,
        DashboardToPresentation,
        DashboardToEportfolio,
        PresentationToEportfolio,
        PresentationToPresentation
    }



    /// <summary>
    /// Represent the document output types supported by the ASPOSE.
    /// </summary>
    public enum DocumentOutputType
    {
        [Description("application/msword")]
        Doc,
        [Description("application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
        Docx,
        [Description("application/pdf")]
        Pdf,
        [Description("text/html")]
        Html,
        [Description("plain/zip")]
        Zip,
        [Description("image/jpg")]
        Image,
        [Description("image/png")]
        Picture
    }

    /// <summary>
    /// represet the question meta data fields
    /// </summary>
    public enum QuestionMetaDataFields
    {
        [Description("totalUsed")]
        TotalUsed,
        [Description("createdBy")]
        CreatedBy,
        [Description("userCreated")]
        UserCreated,
        [Description("publisherEdited")]
        PublisherEdited,
        [Description("publisherSupplied")]
        PublisherSupplied,
        [Description("modifiedBy")]
        ModifiedBy,
        [Description("ExerciseNo")]
        ExerciseNumber,
        [Description("Question used In")]
        UsedIn,
        [Description("Question Congnitive Level")]
        CongnitiveLevel,
        [Description("Question Blooms Domain")]
        BloomsDomain,
        [Description("Question Guidance")]
        Guidance,
        [Description("Question Free Response Question")]
        FreeResponseQuestion,
        [Description("Question Difficulty")]
        Difficulty,
        [Description("ebook section")]
        EbookSection
    }


    /// <summary>
    /// Represents the type of highlights supported.
    /// * GeneralContent - A highlight on ebook or external content.
    /// * WritingAssignment - A highlight on a writing assignment submission.
    /// * PeerReview - A highlight on a submission in a peer review.
    /// </summary>
    public enum PxHighlightType
    {
        None,
        GeneralContent,
        WritingAssignment,
        PeerReview
    }

    public enum ContentType
    {
        Video,
        Other
    }

    /// <summary>
    /// Represents the status of a highlight.
    /// </summary>
    public enum HighlightStatus
    {
        Active,
        Locked,
        Deleted,
        Hide,
    }

    /// <summary>
    /// Represents a note type, note attached to a highlight or public/general notes.
    /// </summary>
    public enum NoteType
    {
        None,
        HighlightNote,
        GeneralNote
    }

    public enum DisplayOption
    {
        Student,
        Instructor,
        ebook,
        lms
    }

    /////// <summary>
    /////// Scored attempt levels for quiz settings
    /////// </summary>
    //public enum ScoredAttempt
    //{
    //    Full_Credit_On_Completion = 2,
    //    Use_Score_Earned = 0,
    //    Manually_Graded = 1
    //}

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
    /// Assessment review settings
    /// </summary>
    public enum ReviewSetting
    {
        Each = 0,
        DueDate = 1,
        Never = 2,
        Second = 3,
        Final = 4
    }

    /// <summary>
    /// Property that stores an enumeration value from the Bfw.PX.Biz.DataContracts.CourseType
    /// </summary>
    public enum CourseType
    {
        Eportfolio,
        EportfolioDashboard,
        EportfolioTemplate,
        FACEPLATE,
        PersonalEportfolioProductMaster,
        PersonalEportfolioDashboard,
        ProgramDashboard,
        PersonalEportfolioPresentation,
        LearningCurve,
        XCLASS,
        XBOOK,
        XBookV2
    }

    public enum CourseReadOnly
    {
        No = 0,
        Yes = 1,
        IfNotOwner = 2
    }

    public enum GradeRule
    {
        Last = 0,
        First = 1,
        Highest = 2,
        Lowest = 3,
        Average = 4,
        Total = 5
    }

    public enum SubmissionGradeAction
    {
        NotSet = -1,
        Default = 0,
        Manual = 1,
        FullCredit = 2
    }

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

    public enum ReportViewerType
    {
        PM,
        Instructor
    }

}