using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Business object defining a note sharing relationship between users.
    /// </summary>
    public class ShareNoteResult
    {
        /// <summary>
        /// ID of the student that is sharing their notes.
        /// </summary>
        public string StudentId { get; set; }

        /// <summary>
        /// ID of the student that "StudentId" is sharing their notes with.
        /// </summary>
        public string SharedStudentId { get; set; }

        /// <summary>
        /// ID of the course the notes are being shared in.
        /// </summary>
        public string CourseId { get; set; }

        /// <summary>
        /// First name of the user that is sharing their notes.
        /// </summary>
        public string FirstNameSharer { get; set; }

        /// <summary>
        /// Last name of the user that is sharing their notes.
        /// </summary>
        public string LastNameSharer { get; set; }

        /// <summary>
        /// First name of the user that "StudentId" is sharing notes with.
        /// </summary>
        public string FirstNameSharee { get; set; }

        /// <summary>
        /// Last name of the user that "StudentId" is sharing notes with.
        /// </summary>
        public string LastNameSharee { get; set; }

        /// <summary>
        /// True if "SharedStudentId" has enabled viewing of the highlights from "StudentId".
        /// </summary>
        public bool HighlightsEnabled { get; set; }

        /// <summary>
        /// True if "SharedStudentId" has enabled viewing of the notes from "StuentId".
        /// </summary>
        public bool NotesEnabled { get; set; }

        /// <summary>
        /// Number of notes that "StudentId" has available.
        /// </summary>
        public int? ItemCount { get; set; }
    }
}
