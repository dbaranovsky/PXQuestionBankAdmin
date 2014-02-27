using System;
namespace Bfw.PX.PXPub.Models
{
    public class QuestionNote
    {
        /// <summary>
        /// A unique note ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// question id related to the note.
        /// </summary>
        public string QuestionId { get; set; }

        /// <summary>
        /// Parent course that the note belongs to.
        /// </summary>
        public string CourseId { get; set; }

        /// <summary>
        /// Gets or sets the note text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Created date of the note.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// First name of the note creator.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name of the note creator.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Path for the attached file
        /// </summary>
        public string AttachPath { get; set; }
    }

}
