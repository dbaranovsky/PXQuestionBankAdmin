namespace Macmillan.PXQBA.Web.ViewModels
{
    /// <summary>
    /// Model for shared question duplicate from was created
    /// </summary>
    public class SharedQuestionDuplicateFromViewModel
    {
        /// <summary>
        /// Question id
        /// </summary>
        public string QuestionId { get; set; }

        /// <summary>
        /// List of titles question is shared with
        /// </summary>
        public string SharedWith { get; set; }
    }
}