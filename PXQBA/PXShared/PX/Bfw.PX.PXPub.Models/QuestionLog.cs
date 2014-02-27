using System;
using System.Collections.Generic;
namespace Bfw.PX.PXPub.Models
{
    public class QuestionLog
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
        public string Type { get; set; }

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
        /// question version assigned on creation / modification
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        ///  parse the XML changesMade 
        /// </summary>
        public List<QuestionFieldLog> ParsedFields { get; set; }


    }

   
    public class QuestionFieldLog
    {
        public string Field { get; set; }
        public string OriginalValue { get; set; }
        public string NewValue { get; set; }
        public string version { get; set; }
    }
}
