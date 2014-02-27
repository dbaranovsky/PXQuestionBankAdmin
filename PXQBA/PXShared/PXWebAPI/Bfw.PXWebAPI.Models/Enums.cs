using System;
using System.ComponentModel;

namespace Bfw.PXWebAPI.Models
{
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

    public enum DisplayOption
    {
        Student,
        Instructor,
        ebook,
        lms
    }

    /// <summary>
    /// Property that stores an enumeration value from the Bfw.PX.Biz.DataContracts.CourseType
    /// </summary>
    public enum CourseType
    {
        LMS,
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
        DigitalCollections,
        BCS
    }


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
        /// Derivative sibling copy.
        /// </summary>
        public const string DerivativeSiblingCopy = "DerivativeSiblingCopy";

        /// <summary>
        /// Copy.
        /// </summary>
        public const string Default = "Copy";
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
    /// Represent a user role in a course.
    /// </summary>
    public enum UserType
    {
        Instructor,
        Student,
        All
    }

    /// <summary>
    /// Represents the status of an item in DLAP and how it should be treated
    /// when synced to other systems.
    /// </summary>
    public enum SyncStatus
    {
        // Default value, implies that no status could be determined
        None,
        // Item has been created
        Created,
        // Item has been modified
        Modified,
        // Item has been deleted
        Deleted
    }
}