using System;
using System.ComponentModel;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Specifies the state of social commenting for items being displayed
    /// </summary>
    public enum SocialCommentingState
    {
        /// <summary>
        /// Indicates that the user has wants commenting for item
        /// </summary>
        Active,
        /// <summary>
        /// Indicates that the user has disabled commenting for item
        /// </summary>
        DisabledByUser,
        /// <summary>
        /// Indicates that the view will not render any social commenting component
        /// </summary>
        None
    };

    /// <summary>
    /// Specifies the currently active view of the content
    /// </summary>
    /// 
    [Flags]
    public enum ContentViewMode
    {
        /// <summary>
        /// Indicates no view
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates CreateAndAssign view
        /// </summary>
        Create = 2,

        /// <summary>
        /// Indicates Preview view
        /// </summary>
        Preview = 4,

        /// <summary>
        /// Indicates edit view
        /// </summary>
        Edit = 8,

        /// <summary>
        /// Indicates Settings view
        /// </summary>
        Settings = 16,

        /// <summary>
        /// Indicates Results view
        /// </summary>
        Results = 32,

        /// <summary>
        /// Indicates Discussion view
        /// </summary>
        Discussion = 64,
        /// <summary>
        /// Indicates assign view
        /// </summary>
        Assign = 128,

        /// <summary>
        /// Indicates Rubrics view for Assignments
        /// </summary>
        Rubrics = 256,

        /// <summary>
        /// Indicates if content is readonly
        /// </summary>
        ReadOnly = 512,

        /// <summary>
        /// Indicates user wants to see related resources
        /// </summary>
        MoreResources = 1024,

        /// <summary>
        /// Indicates user wants to see related resources
        /// </summary>
        MoreResourcesStatic = 2048,

        /// <summary>
        /// Indicates question editor view
        /// </summary>
        Questions = 4096,

        /// <summary>
        /// Indicates Metadata view
        /// </summary>
        Metadata = 8196,

        /// <summary>
        /// Indicates the item is simply an external URL shown in an FNE window
        /// </summary>
        ExternalUrl = 16392,

        /// <summary>
        /// Indicates the item is in grading mode an FNE window
        /// </summary>
        Grading = 32784
    }

    /// <summary>
    /// Represents the category the assignment falls under
    /// </summary>
    //public enum AssignmentCategory
    //{
    //    Homework,
    //    Quiz,
    //    Exam,
    //    FinalExam
    //}

    /// <summary>
    /// Represents the DLAP DropboxType enum
    /// </summary>
    public enum DropboxType
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
    /// Determines the status of the content
    /// </summary>
    public enum ContentStatus
    {
        None = 0,
        New,
        Existing
    }

    /// <summary>
    /// Quiz Types
    /// </summary>
    public enum QuizType
    {
        Assessment,
        Homework,
        LearningCurve,
        CustomActivity,
        HtmlQuiz
    }

    /// <summary>
    /// Assignment Status
    /// </summary>
    public enum AssignmentStatus
    {
        New,
        Saved,
        Submitted,
        Unsubmitted,
        GradeSaved,
        Graded
    }

    /// <summary>
    /// Completeion Status
    /// </summary>
    public enum CompletionStatus
    {
        NotAllowed,
        NotStarted,
        NotCompleted,
        Completed,
        NotGraded,
        Graded,
        NotRequired
    }
    /// <summary>
    /// Upload Type
    /// </summary>
    public enum UploadType
    {
        Default,
        Assignment,
        Aspose
    }

    /// <summary>
    /// Upload File Types
    /// </summary>
    public enum UploadFileType
    {
        Any,
        Restricted // This means only .doc, .docx, .rtf
    }

    /// <summary>
    /// enum to get the collection types.
    /// </summary>
    public enum QuizCollectionType
    {
        [Description("All Collections")]
        All,
        [Description("Questions by chapter")]
        ByPublisher,
        [Description("Questions I've created or edited")]
        ByMe,
        [Description("Questions by assessment")]
        InExistingAssessment
    }


    /// <summary>
    /// enum to get the collection types.
    /// </summary>
    public enum QuestionStatusType
    {
        [Description("In Progress")]
        InProgress,
        [Description("Available to Instructor")]
        AvailabletoInstructor,
        [Description("Deleted")]
        Deleted
    }


    ////mapped from SubmissionGradeAction
    //public enum ScoredAttempt
    //{
    //    Full_Credit_On_Completion = 2,
    //    Use_Score_Earned = 0,
    //    Manually_Graded = 1
    //}
    public enum SubmissionGradeAction
    {
        NotSet = -1,
        Default = 0,
        Manual = 1,
        Full_Credit = 2
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
    /// Question delivery modes for quiz settings
    /// </summary>
    public enum QuestionDelivery
    {
        All = 0,
        One = 1,
        OneNoBacktrack = 2
    }

    /// <summary>
    /// Various changes in state that can happen in assignment center.
    /// </summary>
    public enum AssignmentCenterOperation
    {
        /// <summary>
        /// No, or an unknown, operation has occured
        /// </summary>
        None = 0,
        /// <summary>
        /// An item has been moved.
        /// </summary>
        Move = 1,
        /// <summary>
        /// An item has been assigned due dates or had it's assignment date changed.
        /// </summary>
        DatesAssigned = 2,
        /// <summary>
        /// The move and assign
        /// </summary>
        MoveAndAssign = 3,
        /// <summary>
        /// An item has been assigned points or had it's points changed.
        /// </summary>
        PointsAssigned = 4,
        /// <summary>
        /// An item has been removed.
        /// </summary>
        Removed = 5,
        /// <summary>
        /// A new item has been added to the category.
        /// </summary>
        NewItem = 6,
        /// <summary>
        /// An item has been hidden/shown to students.
        /// </summary>
        ToggleItemVisibility = 7,
        /// <summary>
        /// A new item has been added by using an existing item
        /// </summary>
        AddExistingItem = 8,

        /// <summary>
        /// UnAssign a given item
        /// </summary>
        DatesUnAssigned = 9,

        /// <summary>
        /// The function removes the item chain when it is un-assigned
        /// </summary>
        RemoveOnUnassign = 10,

        /// <summary>
        /// Show items in a dialog
        /// </summary>
        Dialog,

        /// <summary>
        /// Copy item
        /// </summary>
        Copy,

        ShowOrHideFromStudents,
    }

    /// <summary>
    /// Property that stores an enumeration value from the Bfw.PX.Biz.DataContracts.CourseType
    /// </summary>
    public enum CourseType
    {
        FACEPLATE = 2,
        LearningCurve = 3,
        XCLASS = 4,
        XBOOK = 5,
        XBookV2 = 8
    }

    public enum TreeCategoryType
    {
        TOC,
        Assignment,
        ManagementCard
    }


    public enum HostMode
    {
        ContentBrowser,
        AssignmentCenter,
        FacePlate,
        XClass,
        XBook
    }

    public enum DifficultyLevel
    {
        Easy = 1,
        Hard = 2,
        VeryHard = 3
    }

    public enum NoteCreator
    {
        Administrator = 1,
        Student = 2,
        Instructor = 3
    }

    public enum AssignmentVisibility
    {
        Private,
        Shared
    }

    public enum FeedbackType
    {
        Question,
        Choice,
        Other
    }

    public enum ContentAreaType
    {
        PX,
        Agilix
    }

    public enum ReportViewerType
    {
        PM,
        Instructor
    }

    public enum ReportType
    {
        LearningObjective,
        Rubric
    }

    public enum ConversionType
    {
        Pdf,
        Csv,
        Word,
        Html
    }

    public enum SemesterType
    {
        Fall,
        Winter,
        Spring,
        Summer
    }

    public enum QuizBrowserMode
    {
        Quiz, //viewing a quiz
        QuestionPicker, //viewing available questions in question picker
        Resources //viewing in resources browser
    }

    /// <summary>
    /// Late grace duration type for assignment settings
    /// </summary>
    public enum LateGraceDurationType
    {
        Minute,
        Hour,
        Day,
        Week,
        Infinite
    }
}
