using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Bfw.PX.PXPub.Models
{
	public class QuestionAdmin 
	{
	
		/// <summary>
		/// Gets or sets the QuizId id.
		/// </summary>
		/// <value>
		/// The QuizId id.
		/// </value>
		public string QuizId { get;set; }

		/// <summary>
		/// QuestionId
		/// </summary>
		public string QuestionId { get; set; }

        /// <summary>
        /// parentID
        /// </summary>
        public string ParentId { get; set; }

	}

    public class QuestionNotes
    {
        public IList<QuestionNote> NoteList
        {
            get;
            set;
        }

        public QuestionNotes()
        {
            NoteList = new List<QuestionNote>();
        }

    }

    public class QuestionLogs
    {
        public IList<QuestionLog> LogList
        {
            get;
            set;
        }

        public QuestionLogs()
        {
            LogList = new List<QuestionLog>();
        }

    }
}
