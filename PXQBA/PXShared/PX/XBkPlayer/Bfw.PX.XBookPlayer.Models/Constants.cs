namespace Bfw.PX.XBkPlayer.Models
{
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
    /// Assessment Type
    /// </summary>
    public enum AssessmentType
    {
        Quiz,
        Homework,
        LearningCurve
    }

    /// <summary>
    /// Quiz Types
    /// </summary>
    public enum QuizType
    {
        Assessment,
        Homework,
        LearningCurve
    }
}
