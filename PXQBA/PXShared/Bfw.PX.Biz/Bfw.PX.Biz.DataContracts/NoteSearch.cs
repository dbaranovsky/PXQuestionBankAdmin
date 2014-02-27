using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Contains all possible criteria necessary to search notes.
    /// </summary>
    [XmlRoot("notesSearch")]
    public class NoteSearch
    {
        /// <summary>
        /// Gets or sets the highlight ID.
        /// </summary>
        [XmlElement("highlightId")]
        public string HighlightId { get; set; }

        /// <summary>
        /// Gets or sets the note ID.
        /// </summary>
        [XmlElement("noteId")]
        public string NoteId { get; set; }

        /// <summary>
        /// Gets or sets the course ID.
        /// </summary>
        [XmlElement("courseId")]
        public string CourseId { get; set; }

        /// <summary>
        /// Gets or sets the item ID.
        /// </summary>
        [XmlElement("itemId")]
        public string ItemId { get; set; }

        /// <summary>
        /// Gets or sets the peer review ID.
        /// </summary>
        [XmlElement("reviewId")]
        public string ReviewId { get; set; }

        /// <summary>
        /// Gets or sets the type of the highlight.
        /// </summary>
        [XmlElement("highlightType")]
        public PxHighlightType HighlightType { get; set; }

        /// <summary>
        /// Gets or sets the type of the note.
        /// </summary>
        [XmlElement("noteType")]
        public int NoteType { get; set; }

        /// <summary>
        /// Gets or sets the enrollment ID.
        /// </summary>
        [XmlElement("enrollmentId")]
        public string EnrollmentId { get; set; }

        /// <summary>
        /// The current user ID.
        /// </summary>
        [XmlElement("currentUserId")]
        public string CurrentUserId { get; set; }

        /// <summary>
        /// User id to search for.
        /// </summary>
        [XmlElement("userId")]
        public string UserId { get; set; }

        /// <summary>
        /// Search for public highlights.
        /// </summary>
        [XmlElement("highlightPublic")]
        public bool? HighlightPublic { get; set; }

        /// <summary>
        /// Gets a value indicating whether [highlight public specified].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [highlight public specified]; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool HighlightPublicSpecified { get { return HighlightPublic.HasValue; } }

        /// <summary>
        /// Search for a specific highlight status.
        /// </summary>
        private int _highlightStatus = -1;

        /// <summary>
        /// Search for a specific highlight status.
        /// </summary>
        [XmlElement("highlightStatus")]
        public int HighlightStatus
        {
            get { return _highlightStatus; }
            set { _highlightStatus = value; }
        }

        /// <summary>
        /// Search for public notes.
        /// </summary>
        [XmlElement("notePublic")]
        public bool? NotePublic { get; set; }

        /// <summary>
        /// Gets a value indicating whether [note public specified].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [note public specified]; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool NotePublicSpecified { get { return NotePublic.HasValue; } }

        /// <summary>
        /// Associated note display settings for the current user(show my notes, show instructor notes).
        /// </summary>
        [XmlElement("userNote")]
        public UserNote UserNote { get; set; }

        /// <summary>
        /// Associated note display settings for the current user(show my highlights, show instructor highlights).
        /// </summary>
        [XmlElement("userHighlight")]
        public UserHighlight UserHighlight { get; set; }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        public ContentType ContentType { get; set; }

        /// <summary>
        /// Gets or sets the video id.
        /// </summary>
        /// <value>
        /// The video id.
        /// </value>
        [XmlElement("videoId")]
        public string VideoId { get; set; }

        /// <summary>
        /// Gets or sets the student user id.
        /// </summary>
        /// <value>
        /// The student user id.
        /// </value>
        [XmlElement("studentUserId")]
        public string StudentUserId { get; set; }


    }

    /// <summary>
    /// Contains note display settings for an active user.
    /// </summary>
    [Serializable]
    [XmlRoot("userNote")]
    public class UserNote
    {
        /// <summary>
        /// ID of the current user.
        /// </summary>
        [XmlElement("currentUserId")]
        public string MyUserId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show only current user notes.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show my notes]; otherwise, <c>false</c>.
        /// </value>
        [XmlElement("showMyNotes")]
        public bool ShowMyNotes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show only instructor notes.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show instructor note]; otherwise, <c>false</c>.
        /// </value>
        [XmlElement("showInstructorNotes")]
        public bool ShowInstructorNote { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show only public notes.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show public note]; otherwise, <c>false</c>.
        /// </value>
        [XmlElement("showPublicNotes")]
        public bool ShowPublicNote { get; set; }

        /// <summary>
        /// Show only notes from the users with IDs in this collection.
        /// </summary>
        [XmlElement("userId")]
        public List<string> UserIds { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show all student notes].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show all student notes]; otherwise, <c>false</c>.
        /// </value>
        [XmlElement("showAllStudentNotes")]
        public bool? ShowAllStudentNotes { get; set; }
    }

    /// <summary>
    /// Contains highlight display settings for an active user.
    /// </summary>
    [Serializable]
    [XmlRoot("userHighlight")]
    public class UserHighlight
    {
        /// <summary>
        /// ID of the current user.
        /// </summary>
        [XmlElement("currentUserId")]
        public string MyUserId { get; set; }

        /// <summary>
        /// /// /// Gets or sets a value indicating whether to show only current user highlights.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show my highlight]; otherwise, <c>false</c>.
        /// </value>
        [XmlElement("showMyHighlights")]
        public bool ShowMyHighlight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show only instructor highlights.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show instructor highlight]; otherwise, <c>false</c>.
        /// </value>
        [XmlElement("showInstructorHighlights")]
        public bool ShowInstructorHighlight { get; set; }

        /// <summary>
        /// Show only highlights from the users with IDs in this collection.
        /// </summary>
        [XmlElement("userId")]
        public List<string> UserIds { get; set; }
    }
}