using System;

namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Question note
    /// </summary>
    public class Note
    {
        /// <summary>
        /// Note id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Note text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Indicates wheather note is flagged
        /// </summary>
        public bool IsFlagged { get; set; }

        /// <summary>
        /// Id of the question note belongs to
        /// </summary>
        public string QuestionId { get; set; }
    }
}
