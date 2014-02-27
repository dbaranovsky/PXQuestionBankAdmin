using System.Runtime.Serialization;
using System;
namespace Bfw.PX.Biz.DataContracts
{
[Serializable]

    public enum QuestionLogEventType
    {
        Added,
        Modified,
        Flagged,
        UnFlagged,
        NoteAdded
    }


    /// <summary>
    /// Represents a note business object.
    /// </summary>
    [DataContract]
    public class QuestionLog
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
        public QuestionLogEventType EventType { get; set; }

        /// <summary>
        /// Created date of the note.
        /// </summary>
        [DataMember]
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        [DataMember]
        public string UserId { get; set; }

        /// <summary>
        /// First name of the note creator.
        /// </summary>
        [DataMember]
        public string FirstName { get; set; }

        /// <summary>
        /// Last name of the note creator.
        /// </summary>
        [DataMember]
        public string LastName { get; set; }

        /// <summary>
        /// question version info
        /// </summary>
        [DataMember]
        public string Version { get; set; }

        /// <summary>
        /// Changes made to the question while editing
        /// </summary>
        [DataMember]
        public string ChangesMadeXML { get; set; }
    }
}
