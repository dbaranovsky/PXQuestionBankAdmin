using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Contracts
{
    /// <summary>
    /// Represents service to manage business logic with notes
    /// </summary>
    public interface INotesManagementService
    {
        /// <summary>
        /// Loads notes for question
        /// </summary>
        /// <param name="questionId">Question id</param>
        /// <returns>Question notes</returns>
        IEnumerable<Note> GetQuestionNotes(string questionId);

        /// <summary>
        /// Creates note in database
        /// </summary>
        /// <param name="note">Note to save</param>
        /// <returns>Created note</returns>
        Note CreateNote(Note note);

        /// <summary>
        /// Deletes note from database
        /// </summary>
        /// <param name="note">Note to delete</param>
        void DeleteNote(Note note);

        /// <summary>
        /// Updates note in database
        /// </summary>
        /// <param name="note">Note to save changes for</param>
        void SaveNote(Note note);
    }
}