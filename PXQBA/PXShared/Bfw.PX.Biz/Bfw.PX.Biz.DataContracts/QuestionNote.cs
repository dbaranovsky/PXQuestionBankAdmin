using System.Runtime.Serialization;
using System;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Represents a note business object.
    /// </summary>
    [DataContract]
    public class QuestionNote
    {
        /// <summary>
        /// A unique note ID.
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// question id related to the note.
        /// </summary>
        [DataMember]
        public string QuestionId { get; set; }

        /// <summary>
        /// Parent course that the note belongs to.
        /// </summary>
        [DataMember]
        public string CourseId { get; set; }

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
