namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Prodct course chapter
    /// </summary>
    public class Chapter
    {
        /// <summary>
        /// Chapter title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Number of questions in chapter
        /// </summary>
        public int QuestionsCount { get; set; }
    }
}