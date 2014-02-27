using System;
using System.Runtime.Serialization;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Represents a note business object.
    /// </summary>    

    [DataContract]
    public class Note
    {
        /// <summary>
        /// A unique note ID.
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Parent course that the note belongs to.
        /// </summary>
        [DataMember]
        public string EntityId { get; set; }

        /// <summary>
        /// Gets or sets the note title.
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the note text.
        /// </summary>
        [DataMember]
        public string Text { get; set; }

        /// <summary>
        /// Created date of the note.
        /// </summary>
        [DataMember]
        public DateTime Created { get; set; }

        /// <summary>
        /// Name of the note creator.
        /// </summary>
        [DataMember]
        public string CreatedBy { get; set; }

        /// <summary>
        /// The order in which this item should be displayed relative to other items with the same parent.
        /// </summary>
        [DataMember]
        public string Sequence { get; set; }

        /// <summary>
        /// The last modified date.
        /// </summary>
        [DataMember]
        public DateTime Modified { get; set; }

        /// <summary>
        /// Note description text.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Parent course id.
        /// </summary>
        public string CourseId { get; set; }

        /// <summary>
        /// The id of parent top note
        /// </summary>
        public Guid? TopNoteId { get; set; }

        /// <summary>
        /// The id of the associated highlight, if exist.
        /// </summary>
        public Guid? HighlightId { get; set; }

        /// <summary>
        /// Gets or sets the type of the highlight.
        /// </summary>
        public PxHighlightType HighlightType { get; set; }

        /// <summary>
        /// ID of the note.
        /// </summary>
        public Guid NoteId { get; set; }

        /// <summary>
        /// Indicates whether this is a general note(not attached to a highlight).
        /// </summary>
        /// <value>
        /// 	<c>true</c> if note is general; otherwise, <c>false</c>.
        /// </value>
        public bool IsGeneral { get; set; }

        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Indicates whether this <see cref="Note"/> is public, making it visible to other users.
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
        /// Enrollment ID of the note creator.
        /// </summary>
        public string EnrollmentId { get; set; }

        /// <summary>
        /// Gets or sets the item ID.
        /// </summary>
        /// <value>
        /// The item ID.
        /// </value>
        public string ItemId { get; set; }

        /// <summary>
        /// ID of the associated peer review item, if exist.
        /// </summary>
        public string ReviewId { get; set; }

        /// <summary>
        /// First name of the note creator.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name of the note creator.
        /// </summary>
        public string LastName { get; set; }
    }
}
