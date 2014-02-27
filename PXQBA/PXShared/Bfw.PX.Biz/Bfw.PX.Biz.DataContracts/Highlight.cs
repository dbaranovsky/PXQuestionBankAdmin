using System;
using System.Collections.Generic;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Represents a Highlight business object.
    /// </summary>
    public class Highlight
    {
        /// <summary>
        /// Gets or sets the course ID.
        /// </summary>
        /// <value>
        /// The course ID.
        /// </value>
        public string CourseId { get; set; }

        /// <summary>
        /// Gets or sets the highlight ID.
        /// </summary>
        /// <value>
        /// The highlight ID.
        /// </value>
        public Guid HighlightId { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Type of Highlight (GeneralComment, WritingAssignment, PeerReview).
        /// </summary>
        public PxHighlightType HighlightType { get; set; }

        /// <summary>
        /// Gets or sets the type of the note.
        /// </summary>
        public NoteType NoteType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Highlight"/> is public.
        /// </summary>
        /// <value>
        ///   <c>true</c> if public; otherwise, <c>false</c>.
        /// </value>
        public bool Public { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the associated item ID.
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// Gets or sets the associated enrollment ID.
        /// </summary>
        public string EnrollmentId { get; set; }

        /// <summary>
        /// Gets or sets the associated peer review ID.
        /// </summary>
        public string ReviewId { get; set; }

        /// <summary>
        /// First name of the highlight creator.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name of the highlight creator.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the last modified date.
        /// </summary>
        public DateTime Modified { get; set; }

        /// <summary>
        /// Collection of notes attached to this highlight instance.
        /// </summary>
        public IEnumerable<Note> Notes { get; set; }


        /// <summary>
        /// Css class names(s) that should be applied to highlight
        /// </summary>
        public string ClassName { get; set; }

        public string Start { get; set; }
        public int? StartOffset { get; set; }
        public string End { get; set; }
        public int? EndOffset { get; set; }

    }
}
