using System;
using System.Collections.Generic;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// Provides all business logic functions in regards to all forms of notes.
    /// </summary>
    public interface INoteActions
    {
        /// <summary>
        /// Adds a highlight and note to the system.
        /// </summary>
        /// <param name="highlight">The highlight.</param>
        /// <param name="highlightId">The highlight ID.</param>
        /// <param name="noteId">The note ID.</param>
        void AddHighlightNote(Highlight highlight, out Guid? highlightId, out Guid? noteId);

        /// <summary>
        /// Adds a highlight to the system.
        /// </summary>
        /// <param name="highlight">The highlight data to store.</param>
        /// <returns></returns>
        Guid AddHighlight(Highlight highlight);

        /// <summary>
        /// Adds a note to the system.
        /// </summary>
        /// <param name="note">The note data to store.</param>
        /// <returns></returns>
        Guid AddNote(Note note);

        /// <summary>
        /// Adds a note to an existing highlight.
        /// </summary>
        /// <param name="note">The note.</param>
        void AddNoteToHighlight(Note note);

        /// <summary>
        /// Adds a note to an existing highlight.
        /// </summary>
        /// <param name="highlightId">The highlight ID.</param>
        /// <param name="itemId">The item ID.</param>
        /// <param name="reviewId">The review ID.</param>
        /// <param name="commentText">The comment text.</param>
        /// <param name="description">The description.</param>
        /// <param name="highlightType">Type of the highlight.</param>
        /// <param name="isPublic">if set to <c>true</c> [is public].</param>
        /// <returns></returns>
        Note AddNoteToHighlight(string highlightId, string itemId, string reviewId, String commentText,
                                string description, PxHighlightType highlightType, bool isPublic);

        /// <summary>
        /// Adds a note to an existing top note.
        /// </summary>
        /// <param name="topNoteId">The top note ID.</param>
        /// <param name="itemId">The item ID.</param>
        /// <param name="commentText">The comment text.</param>
        /// <param name="description">The description.</param>
        /// <param name="isPublic">if set to <c>true</c> [is public].</param>
        /// <returns></returns>
        Note AddNoteToTopNote(string topNoteId, string itemId, String commentText,
                                string description, bool isPublic);
        /// <summary>
        /// Updates an existing highlight's status. A highlight can be active, locked or deleted.
        /// </summary>
        /// <param name="highlightId">The highlight ID.</param>
        /// <param name="status">The status of the highlight.</param>
        /// <param name="courseId">The course ID.</param>
        void UpdateHighlightStatus(string highlightId, HighlightStatus status, string courseId);

        /// <summary>
        /// Updates status of all highlights matching filter criteria. Status can be active, locked or deleted.
        /// </summary>
        /// <param name="highlightId">The highlight ID.</param>
        /// <param name="userId">The user ID.</param>
        /// <param name="itemId">The item ID.</param>
        /// <param name="reviewId">The review ID.</param>
        /// <param name="courseId">The course ID.</param>
        /// <param name="enrollmentId">The enrollment ID.</param>
        /// <param name="updateOnlyEmptyHighlights">if set to <c>true</c> only update highlights with no notes.</param>
        /// <param name="status">The status to update to.</param>
        void UpdateHighlightStatus(string highlightId, string userId, string itemId, string reviewId, string courseId,
                                   string enrollmentId, bool updateOnlyEmptyHighlights, HighlightStatus status);

        /// <summary>
        /// Updates an existing highlight note status. Status can be active, locked or deleted.
        /// </summary>
        /// <param name="highlightId">The highlight ID.</param>
        /// <param name="status">The status of the highlight.</param>
        /// <param name="updateParentHighlight">if set to <c>true</c> update parent highlight as well.</param>
        /// <param name="courseId">The course ID.</param>
        void UpdateHighlightNoteStatus(string highlightId, HighlightStatus status, bool updateParentHighlight, string courseId);

        /// <summary>
        /// Updates an existing note status. Status can be active, locked or deleted.
        /// </summary>
        /// <param name="noteId">The note ID.</param>
        /// <param name="status">The status of the highlight.</param>
        /// <param name="courseId">The course ID.</param>
        void UpdateNoteStatus(string noteId, HighlightStatus status, string courseId);

        /// <summary>
        /// Updates status of all notes matching filter criteria. Status can be active, locked or deleted.
        /// </summary>
        /// <param name="noteId">The note ID.</param>
        /// <param name="userId">The user ID.</param>
        /// <param name="itemId">The item ID.</param>
        /// <param name="reviewId">The review ID.</param>
        /// <param name="courseId">The course ID.</param>
        /// <param name="enrollmentId">The enrollment ID.</param>
        /// <param name="updateParentHighlight">if set to <c>true</c> update parent highlights as well.</param>
        /// <param name="status">The status of the highlight.</param>
        void UpdateNoteStatus(string noteId, string userId, string itemId, string reviewId, string courseId,
                              string enrollmentId, bool updateParentHighlight, HighlightStatus status);

        /// <summary>
        /// Updates the color of the highlight.
        /// Color setting for the Highlight with accepted string values "color-1", "color-2",..."color-n"
        /// mapping to a CSS rule.
        /// </summary>
        /// <param name="highlightId">The highlight ID.</param>
        /// <param name="color">Color setting for the Highlight.</param>
        void UpdateHighlightColor(string highlightId, string color);

        /// <summary>
        /// Updates the text of an existing note.
        /// </summary>
        /// <param name="noteId">ID of the note to update.</param>
        /// <param name="noteText">The note text.</param>
        void UpdateNote(string noteId, string noteText);

        /// <summary>
        /// Toggles the sharing of a highlight.
        /// </summary>
        /// <param name="highlightId">ID of highlight to share.</param>
        /// <param name="share">if set to <c>true</c> makes the highlight publicly shared.</param>
        void ShareHighlight(string highlightId, bool share);

        /// <summary>
        /// Creates a note sharing relationship between two users.
        /// </summary>
        /// <param name="shareNotes">The note sharing relationship info.</param>
        void ShareNotes(ShareNoteResult shareNotes);

        /// <summary>
        /// Gets a set of highlights that match the specified <see cref="NoteSearch" /> criteria.
        /// </summary>
        /// <param name="noteSearch">The note search criteria.</param>
        /// <param name="gradeActions">An IGradeActions implementation.</param>
        /// <returns></returns>
        IEnumerable<Highlight> GetHighlights(NoteSearch noteSearch, IGradeActions gradeActions);

        /// <summary>
        /// Gets a set of notes that match the specified <see cref="NoteSearch" /> criteria.
        /// </summary>
        /// <param name="noteSearch">The note search critera.</param>
        /// <returns></returns>
        IEnumerable<Note> GetNotes(NoteSearch noteSearch);

        /// <summary>
        /// Gets a highlight by highlight ID and type.
        /// </summary>
        /// <param name="highlightId">ID of the highlight to get.</param>
        /// <param name="highlightType">Type of the highlight.</param>
        /// <returns></returns>
        Highlight GetHighlightByHighlightId(string highlightId, PxHighlightType highlightType);

        /// <summary>
        /// Gets the notes by highlight ID and type.
        /// </summary>
        /// <param name="highlightId">ID of the highlight.</param>
        /// <param name="highlightType">Type of the highlight.</param>
        /// <returns></returns>
        IEnumerable<Note> GetNotesByHighlightId(string highlightId, PxHighlightType highlightType);

        /// <summary>
        /// Gets the item highlight notes that match the specified <see cref="NoteSearch" /> criteria.
        /// Item highlights are highlights on General/eBook content.
        /// </summary>
        /// <param name="noteSearch">The note search criteria.</param>
        /// <returns></returns>
        IEnumerable<Highlight> GetItemHighlightNotes(NoteSearch noteSearch);

        /// <summary>
        /// Gets the item general notes that match the specified <see cref="NoteSearch" /> criteria.
        /// Item general notes are notes on General/eBook content that are not attached to highlights.
        /// </summary>
        /// <param name="noteSearch">The note search criteria.</param>
        /// <returns></returns>
        IEnumerable<Note> GetItemGeneralNotes(NoteSearch noteSearch);

        /// <summary>
        /// Gets the submission highlight notes that match the specified <see cref="NoteSearch" /> criteria.
        /// Submission highlights are highlights on writing assignments.
        /// </summary>
        /// <param name="noteSearch">The note search criteria.</param>
        /// <param name="gradeActions">An IGradeActions implementation.</param>
        /// <returns></returns>
        IEnumerable<Highlight> GetSubmissionHighlightNotes(NoteSearch noteSearch, IGradeActions gradeActions);

        /// <summary>
        /// Gets the submission general notes that match the specified <see cref="NoteSearch" /> criteria.
        /// Submission general notes are notes on writing assignments that are not attached to highlights.
        /// </summary>
        /// <param name="noteSearch">The note search criteria.</param>
        /// <returns></returns>
        IEnumerable<Note> GetSubmissionGeneralNotes(NoteSearch noteSearch);

        /// <summary>
        /// Gets peer review highlight notes that match the specified <see cref="NoteSearch" /> criteria.
        /// Review highlights are highlights on peer review assignments.
        /// </summary>
        /// <param name="noteSearch">The note search criteria.</param>
        /// <param name="gradeActions">An IGradeActions implementation.</param>
        /// <returns></returns>
        IEnumerable<Highlight> GetReviewHighlightNotes(NoteSearch noteSearch, IGradeActions gradeActions);

        /// <summary>
        /// Gets peer review general notes that match the specified <see cref="NoteSearch" /> criteria.
        /// Review general notes are notes on peer review assignments that are not attached to highlights.
        /// </summary>
        /// <param name="noteSearch">The note search criteria.</param>
        /// <returns></returns>
        IEnumerable<Note> GetReviewGeneralNotes(NoteSearch noteSearch);

        /// <summary>
        /// Gets all shared notes from the specified student and course.
        /// </summary>
        /// <param name="studentId">The student ID.</param>
        /// <param name="courseId">The course ID.</param>
        /// <returns></returns>
        IEnumerable<ShareNoteResult> GetAllSharedNotes(string studentId, string courseId);

        /// <summary>
        /// Gets all note sharing relationships for specified user.
        /// </summary>
        /// <param name="studentId">The student ID.</param>
        /// <param name="courseId">The course ID.</param>
        /// <param name="itemId">The item ID.</param>
        /// <param name="reviewId">The peer review ID.</param>
        /// <param name="enrollmentId">The enrollment ID.</param>
        IEnumerable<ShareNoteResult> GetNoteSettings(string studentId, string courseId, string itemId, string reviewId,
                                                     string enrollmentId);

        /// <summary>
        /// Stores user information (name, user type,...) into notes databases.
        /// </summary>
        /// <param name="user">The user information to store.</param>
        /// <param name="courseId">The course ID.</param>
        /// <param name="type">The user type.</param>
        void InitializeUser(UserInfo user, string courseId, UserType type);

        /// <summary>
        /// Updates existing note sharing relationship settings.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="courseId">The course ID.</param>
        /// <param name="sharerId">The sharer ID.</param>
        /// <param name="highlights">If true share highlights.</param>
        /// <param name="notes">If true share notes.</param>
        void UpdateNoteSettings(string userId, string courseId, string sharerId, bool? highlights, bool? notes);

        /// <summary>
        /// Search for a set of highlights in a given source string.
        /// If match found, wraps substring with html span tags
        /// </summary>
        /// <param name="strToHighlight">Source string to search in.</param>
        /// <param name="highlights">List of highlights that should be searched for.</param>
        /// <returns></returns>
        string HighlightText(string strToHighlight, IList<Highlight> highlights);

        /// <summary>
        /// Removes sharing of a note between users.
        /// </summary>
        /// <param name="result">The result.</param>
        void StopSharing(ShareNoteResult result);

        /// <summary>
        ///  Sets the css Class name on the highlight object
        /// </summary>
        /// <param name="hl"></param>
        void SetClassName(Highlight hl);

        /// <summary>
        /// Gets the highlight note count for the specified filter values.
        /// </summary>
        /// <param name="highlightType">Type of the highlight.</param>
        /// <param name="courseId">The course ID.</param>
        /// <param name="itemId">The item ID.</param>
        /// <param name="reviewId">The review ID.</param>
        /// <param name="userId">The user ID.</param>
        /// <param name="enrollmentId">The enrollment ID.</param>
        /// <param name="highlightCount">The highlight count.</param>
        /// <param name="noteCount">The note count.</param>
        void GetHighlightNoteCount(PxHighlightType highlightType, string courseId, string itemId, string reviewId, string userId,
                         string enrollmentId, out int highlightCount, out int noteCount);

        /// <summary>
        /// Gets the note count for the specified filter values.
        /// </summary>
        /// <param name="highlightType">Type of the highlight.</param>
        /// <param name="courseId">The course ID.</param>
        /// <param name="itemId">The item ID.</param>
        /// <param name="reviewId">The review ID.</param>
        /// <param name="userId">The user ID.</param>
        /// <param name="enrollmentId">The enrollment ID.</param>
        /// <returns></returns>
        int GetNoteCount(PxHighlightType highlightType, string courseId, string itemId, string reviewId, string userId,
                         string enrollmentId);

        /// <summary>
        /// Gets the highlight count for the specified filter values.
        /// </summary>
        /// <param name="highlightType">Type of the highlight.</param>
        /// <param name="courseId">The course ID.</param>
        /// <param name="itemId">The item ID.</param>
        /// <param name="reviewId">The review ID.</param>
        /// <param name="userId">The user ID.</param>
        /// <param name="enrollmentId">The enrollment ID.</param>
        /// <returns></returns>
        int GetHighlightCount(PxHighlightType highlightType, string courseId, string itemId, string reviewId,
                              string userId,
                              string enrollmentId);

        /// <summary>
        /// Gets the notes for the specified user.
        /// </summary>
        /// <param name="highlightType">Type of the highlight.</param>
        /// <param name="userId">The user ID.</param>
        /// <param name="courseId">The course ID.</param>
        /// <param name="enrollmentId">The enrollment ID.</param>
        /// <returns></returns>
        IEnumerable<Note> GetNotesByUser(PxHighlightType highlightType, string userId, string courseId, string enrollmentId);

        /// <summary>
        /// Shares a note.
        /// </summary>
        /// <param name="noteId">ID of a note to share.</param>
        /// <param name="share">if set to <c>true</c> marks a note as publicly shared.</param>
        void ShareNote(string noteId, bool share);

    }
}