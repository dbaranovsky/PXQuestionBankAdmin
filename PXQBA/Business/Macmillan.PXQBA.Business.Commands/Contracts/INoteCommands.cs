using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Commands.Contracts
{
    /// <summary>
    /// Represents list of operations available for question notes
    /// </summary>
    public interface INoteCommands
    {
        /// <summary>
        /// Loads the list of question notes from database
        /// </summary>
        /// <param name="questionId">Question id to get the notes for</param>
        /// <returns>List of notes</returns>
        IEnumerable<Note> GetQuestionNotes(string questionId);

        /// <summary>
        /// Creates a note in the database
        /// </summary>
        /// <param name="note">Note to create</param>
        /// <returns>Created note</returns>
        Note CreateNote(Note note);

        /// <summary>
        /// Deletes note from database
        /// </summary>
        /// <param name="note">Note to delete</param>
        void DeleteNote(Note note);

        /// <summary>
        /// Updates existing note in the database
        /// </summary>
        /// <param name="note">Note to update</param>
        /// <returns>Updated note</returns>
        Note UpdateNote(Note note);
    }
}