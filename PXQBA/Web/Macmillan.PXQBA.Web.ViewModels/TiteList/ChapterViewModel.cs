namespace Macmillan.PXQBA.Web.ViewModels.TiteList
{
    /// <summary>
    /// View model for chapter 
    /// </summary>
    public class ChapterViewModel
    {
        /// <summary>
        /// Chapter id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Chapter title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Questions count per chapter
        /// </summary>
        public int QuestionsCount { get; set; }
    }
}
