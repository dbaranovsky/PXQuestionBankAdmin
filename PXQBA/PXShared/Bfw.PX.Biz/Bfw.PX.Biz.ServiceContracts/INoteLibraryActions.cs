using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// Provides methods to retrieve, store, and otherwise manipulate the note library.
    /// </summary>
    public interface INoteLibraryActions
    {
        /// <summary>
        /// Gets a lists of notes for the specified course.
        /// </summary>
        /// <param name="entityId">ID of the course or section.</param>
        /// <returns></returns>
        IEnumerable<Note> ListNotes(string entityId);

        /// <summary>
        /// Stores a note to the specified course in the system.
        /// </summary>
        /// <param name="entityId">ID of the course or section.</param>
        /// <param name="note">The note to store.</param>
        /// <returns></returns>
        Note SaveNote(string entityId, Note note);

        /// <summary>
        /// Deletes a note in the specified course.
        /// </summary>
        /// <param name="entityId">ID of the course or section.</param>
        /// <param name="noteId">ID of the note to delete.</param>
        void DeleteNote(string entityId, string noteId);

        /// <summary>
        /// Reorders a note in the specified course.
        /// </summary>
        /// <param name="entityId">ID of the course or section.</param>
        /// <param name="note">The note to reorder.</param>
        /// <param name="prevSequence">The prev sequence.</param>
        /// <param name="nextSequence">The next sequence.</param>
        /// <returns></returns>
        Note ReorderNote(string entityId, Note note, string prevSequence, string nextSequence);
    }
}