using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Agilix.Dlap
{
    /// <summary>
    /// Available Dlap request types
    /// </summary>
    public enum DlapRequestType
    {
        /// <summary>
        /// Indicates that no real value has been set
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates that the Dlap request should use the HTTP GET verb
        /// </summary>
        Get = 1,

        /// <summary>
        /// Indicates that the Dlap request should use the HTTP POST verb
        /// </summary>
        Post = 2
    }

    /// <summary>
    /// Available request modes
    /// </summary>
    public enum DlapRequestMode
    {
        /// <summary>
        /// Indicates no real value has been set
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates a single request
        /// </summary>
        Single = 1,

        /// <summary>
        /// Indicates a batch of requests
        /// </summary>
        Batch = 2
    }

    /// <summary>
    /// Possible dlap response  codes
    /// </summary>
    public enum DlapResponseCode
    {
        /// <summary>
        /// Indicates an empty response code
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates the request completed successfully
        /// </summary>
        OK = 1,

        /// <summary>
        /// Indicates an error occured during the request
        /// </summary>
        Error = 2,

        /// <summary>
        /// Error bubbled up from the HttpServer
        /// </summary>
        HttpServer = 3,

        /// <summary>
        /// User is not authorized
        /// </summary>
        NoAuthentication = 4,

        /// <summary>
        /// User does not have the permissions to perform the requested action
        /// </summary>
        AccessDenied = 5,

        /// <summary>
        /// Indicates that user did not provide the necessary login information
        /// </summary>
        InvalidCredentials = 6,

        /// <summary>
        /// Requested item does not exist at the specified location, or at all
        /// </summary>
        DoesNotExist = 7,

        /// <summary>
        /// Indicates some kind of internal SQL error
        /// </summary>
        Sql = 8,

        /// <summary>
        /// Request is not properly formatted
        /// </summary>
        BadRequest = 9,

        /// <summary>
        /// NullReference exception was bubbled up from DLAP
        /// </summary>
        NullReference = 10,

        /// <summary>
        /// IndexOutOfRange exception bubbled up from DLAP
        /// </summary>
        IndexOutOfRange = 11,

        /// <summary>
        /// Indicates that there was a bad format
        /// </summary>
        Format = 12,

        /// <summary>
        /// An argument to the command was missing or invalid
        /// </summary>
        Argument = 13,

        /// <summary>
        /// Item or resource requested does not exist
        /// </summary>
        ResourceNotFound = 14,

        /// <summary>
        /// Error bubbled up from the SOLR indexing service
        /// </summary>
        Solr = 15,

        /// <summary>
        /// Error when key isn't found in DLAP
        /// </summary>
        KeyNotFound = 16
    }

    /// <summary>
    /// Determines the content type sent for the request
    /// </summary>
    public enum DlapRequestContentType
    {
        /// <summary>
        /// Indicates no value has been set
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates text/xml will be sent
        /// </summary>
        Text = 1,

        /// <summary>
        /// Indicates raw data will be sent
        /// </summary>
        Binary = 2
    }

    /// <summary>
    /// Used to identified what Dlap primitive the item element represents
    /// </summary>
    public enum DlapItemType
    {
        /// <summary>
        /// Default, indicates no type
        /// </summary>
        None = 0,
        /// <summary>
        /// Item is an AssetLink
        /// </summary>
        AssetLink,
        /// <summary>
        /// Item is an Assessment (aka quiz)
        /// </summary>
        Assessment,
        /// <summary>
        /// Item is an Assignment
        /// </summary>
        Assignment,
        /// <summary>
        /// Item is a CustomActivity
        /// </summary>
        CustomActivity,
        /// <summary>
        /// Item is a Discussion
        /// </summary>
        Discussion,
        /// <summary>
        /// Item is a Folder
        /// </summary>
        Folder,
        /// <summary>
        /// Item is a Homework (aka quiz)
        /// </summary>
        Homework,
        /// <summary>
        /// Item is a Resource
        /// </summary>
        Resource,
        /// <summary>
        /// Item is an RSSFeed
        /// </summary>
        RssFeed,
        /// <summary>
        /// Item is a Shortcut
        /// </summary>
        Shortcut,
        /// <summary>
        /// Item is a Survey
        /// </summary>
        Survey,
        /// <summary>
        /// Html Document
        /// </summary>
        HtmlDocument,
        /// <summary>
        /// Document Collection
        /// </summary>
        DocumentCollection,
        /// <summary>
        /// Item is a Custom item
        /// </summary>
        Custom
    }

    /// <summary>
    /// Determines how the dropbox for an assignment should be configured
    /// </summary>
    public enum DlapDropboxType
    {
        /// <summary>
        /// Dropbox only allows a single document to be uploaded
        /// </summary>
        SingleDocument = 0,
        /// <summary>
        /// Dropbox allows a document template to be uploaed
        /// </summary>
        DocumentTemplate,
        /// <summary>
        /// Dropbox allows more than one document to be uploaded
        /// </summary>
        MultipleDocuments,
        /// <summary>
        /// Dropbox only allows notes to be entered
        /// </summary>
        NotesOnly,
        /// <summary>
        /// Dropbox allows only a URL to be entered
        /// </summary>
        Url,
        /// <summary>
        /// Dropbox allows capturing audio
        /// </summary>
        AudioRecording
    }

    /// <summary>
    /// The rights that a user has
    /// </summary>
    [Flags]
    public enum DlapRights : long
    {
        /// <summary>
        /// User has no rights
        /// </summary>
        None = 0L,
        /// <summary>
        /// User has right to submit assignments for grading, corresponds to the "Student" checkbox in BrainHoney
        /// </summary>
        Participate = 0x01L,
        /// <summary>
        /// User can create a Domain type entity
        /// </summary>
        CreateDomain = 0x10L,
        /// <summary>
        /// User can read a Domain's informatino
        /// </summary>
        ReadDomain = 0x20L,
        /// <summary>
        /// User can modify a Domain
        /// </summary>
        UpdateDomain = 0x40L,
        /// <summary>
        /// User can delete a Domain
        /// </summary>
        DeleteDomain = 0x80L,
        /// <summary>
        /// User can create new User accounts
        /// </summary>
        CreateUser = 0x100L,
        /// <summary>
        /// User can read information from User accounts
        /// </summary>
        ReadUser = 0x200L,
        /// <summary>
        /// User can modify User accounts
        /// </summary>
        UpdateUser = 0x400L,
        /// <summary>
        /// User can delete User accounts
        /// </summary>
        DeleteUser = 0x800L,
        /// <summary>
        /// User can create Course type entities
        /// </summary>
        CreateCourse = 0x10000L,
        /// <summary>
        /// User can read information about a Course
        /// </summary>
        ReadCourse = 0x20000L,
        /// <summary>
        /// User can modify an existing Course
        /// </summary>
        UpdateCourse = 0x40000L,
        /// <summary>
        /// User can delete an existing Course
        /// </summary>
        DeleteCourse = 0x80000L,
        /// <summary>
        /// User can create a Section type entity inside of a Course
        /// </summary>
        CreateSection = 0x100000L,
        /// <summary>
        /// User can read information about a Section
        /// </summary>
        ReadSection = 0x200000L,
        /// <summary>
        /// User can modify and existing Section
        /// </summary>
        UpdateSection = 0x400000L,
        /// <summary>
        /// User can delete a section
        /// </summary>
        DeleteSection = 0x800000L,
        /// <summary>
        /// User can grade student submissions
        /// </summary>
        GradeAssignment = 0x1000000L,
        /// <summary>
        /// User can grade forum entries and posts
        /// </summary>
        GradeForum = 0x2000000L,
        /// <summary>
        /// User can grade an exam
        /// </summary>
        GradeExam = 0x4000000L,
        /// <summary>
        /// User can modify the contents and settings for the gradebook
        /// </summary>
        SetupGradebook = 0x8000000L,
        /// <summary>
        /// User can control a Domain (e.g. Owner box in BrainHoney)
        /// </summary>
        ControlDomain = 0x10000000L,
        /// <summary>
        /// User can control a Course (e.g. Owner box in BrainHoney)
        /// </summary>
        ControlCourse = 0x20000000L,
        /// <summary>
        /// User can control a Section (e.g. Owner box in BrainHoney)
        /// </summary>
        ControlSection = 0x40000000L,
        /// <summary>
        /// User can grant other users permissions to read the gradebook
        /// </summary>
        ControlReadGradebook = 0x80000000L,
        /// <summary>
        /// User can access reports for a Domain
        /// </summary>
        ReportDomain = 0x100000000L,
        /// <summary>
        /// User can access reports for a Course
        /// </summary>
        ReportCourse = 0x200000000L,
        /// <summary>
        /// User can access reports for a Section
        /// </summary>
        ReportSection = 0x400000000L,
        /// <summary>
        /// User can create announcments for a Domain
        /// </summary>
        PostDomainAnnouncements = 0x800000000L,
        /// <summary>
        /// User can impersonate another user
        /// </summary>
        Proxy = 0x1000000000L,
        /// <summary>
        /// User can access reports about Users
        /// </summary>
        ReportUser = 0x4000000000L,
        /// <summary>
        /// User has ability to submit a student's overall grade for a course
        /// </summary>
        SubmitFinalGrade = 0x8000000000L,
        /// <summary>
        /// User can grant or revoke rights to another enrollment.
        /// </summary>
        ControlEnrollment = 0x10000000000L,
        /// <summary>
        /// User can read data, including grades, that is associated with the other user's enrollment.
        /// </summary>
        ReadEnrollment = 0x20000000000L,
        /// <summary>
        /// User can read all course content data, including assessment questions and hidden-from-student content, regardless of their other enrollment rights on the course.
        /// </summary>
        ReadCourseFull = 0x40000000000L,
        /// <summary>
        /// User can assign rights or subscriptions to users in the domain.
        /// </summary>
        ControlUser = 0x80000000000L,
        /// <summary>
        /// User can read objectives and objective maps from an objective set.
        /// </summary>
        ReadObjective = 0x100000000000L,
        /// <summary>
        /// User can add, update, or delete objectives or objective maps in an objective set.
        /// </summary>
        UpdateObjective = 0x200000000000L,
        /// <summary>
        /// User can read credits for other users.
        /// </summary>
        ReadCredits = 0x400000000000L,
        /// <summary>
        /// User can issue, spend, or transfer credits.
        /// </summary>
        UpdateCredits = 0x800000000000L
    }

    /// <summary>
    /// http://dlap.bfwpub.com/js/docs/#!/Enum/InteractionFlags
    /// </summary>
    [Flags]
    [Serializable]
    public enum QuestionInteractionFlags
    {
        /// <summary>
        /// No flags.
        /// </summary>
        None = 0x0,
        /// <summary>
        /// The question shows a space where students can show their work.
        /// </summary>
        ShowWorkspace = 0x1,
        /// <summary>
        /// Displays all choices beneath the interaction in the order in which they are created; i.e., does not randomize choice order.
        /// </summary>
        MaintainOrder = 0x2,
        /// <summary>
        /// The interaction uses inline choices.
        /// </summary>
        Inline = 0x4,
        /// <summary>
        /// The interaction has multiple choices.
        /// </summary>
        Multiple = 0x8,
        /// <summary>
        /// The interaction has multiple choices.
        /// </summary>
        ExtraCredit = 0x200,
    }
}