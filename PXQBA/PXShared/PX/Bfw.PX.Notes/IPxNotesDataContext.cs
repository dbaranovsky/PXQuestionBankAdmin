using System;
using System.Collections.Generic;
using System.Xml.Linq;
using BizDC = Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Notes
{
    public interface INotesDataContext
    {
        /// <summary>
        /// Adds the highlight note.
        /// </summary>
        /// <param name="highlight">The highlight.</param>
        /// <param name="highlightId">The highlight id.</param>
        /// <param name="noteId">The note id.</param>
        void AddHighlightNote(Biz.DataContracts.Highlight highlight, out Guid? highlightId, out Guid? noteId);

        /// <summary>
        /// Adds the highlight.
        /// </summary>
        /// <param name="highlight">The highlight.</param>
        /// <returns></returns>
        Guid AddHighlight(Biz.DataContracts.Highlight highlight);

        /// <summary>
        /// Adds the note.
        /// </summary>
        /// <param name="note">The note.</param>
        /// <returns></returns>
        Guid AddNote(Biz.DataContracts.Note note);

        /// <summary>
        /// Adds the note to highlight.
        /// </summary>
        /// <param name="note">The note.</param>
        void AddNoteToHighlight(Biz.DataContracts.Note note);

        /// <summary>
        /// Adds the note to top note.
        /// </summary>
        /// <param name="note">The note.</param>
        void AddNoteToTopNote(Biz.DataContracts.Note note);

        /// <summary>
        /// Updates the highlight status.
        /// </summary>
        /// <param name="highlightId">The highlight id.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="itemId">The item id.</param>
        /// <param name="reviewId">The review id.</param>
        /// <param name="courseId">The course id.</param>
        /// <param name="enrollmentId">The enrollment id.</param>
        /// <param name="updateOnlyEmptyHighlights">if set to <c>true</c> [update only empty highlights].</param>
        /// <param name="status">The status.</param>
        void UpdateHighlightStatus(string highlightId, string userId, string itemId, string reviewId,
            string courseId, string enrollmentId, bool updateOnlyEmptyHighlights, int status);

        /// <summary>
        /// Updates the note status.
        /// </summary>
        /// <param name="highlightId">The highlight id.</param>
        /// <param name="noteId">The note id.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="itemId">The item id.</param>
        /// <param name="reviewId">The review id.</param>
        /// <param name="courseId">The course id.</param>
        /// <param name="enrollmentId">The enrollment id.</param>
        /// <param name="updateParentHighlight">if set to <c>true</c> [update parent highlight].</param>
        /// <param name="status">The status.</param>
        void UpdateNoteStatus(string highlightId, string noteId, string userId, string itemId, string reviewId,
            string courseId, string enrollmentId, bool updateParentHighlight, int status);

        /// <summary>
        /// Updates the color of the highlight.
        /// </summary>
        /// <param name="highlightId">The highlight id.</param>
        /// <param name="color">The color.</param>
        void UpdateHighlightColor(string highlightId, string color);

        /// <summary>
        /// Updates the note.
        /// </summary>
        /// <param name="noteId">The note id.</param>
        /// <param name="noteText">The note text.</param>
        void UpdateNote(string noteId, string noteText);

        /// <summary>
        /// Shares the highlight.
        /// </summary>
        /// <param name="highlightId">The highlight id.</param>
        /// <param name="share">if set to <c>true</c> [share].</param>
        void ShareHighlight(string highlightId, bool share);

        /// <summary>
        /// Shares the note.
        /// </summary>
        /// <param name="noteId">The note id.</param>
        /// <param name="share">if set to <c>true</c> [share].</param>
        void ShareNote(string noteId, bool share);

        /// <summary>
        /// Shares the notes.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void ShareNotes(NoteSettingEntity entity);

        /// <summary>
        /// Stops the sharing.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void StopSharing(NoteSettingEntity entity);

        /// <summary>
        /// Gets all shared notes.
        /// </summary>
        /// <param name="studentId">The student id.</param>
        /// <param name="courseId">The course id.</param>
        /// <returns></returns>
        IEnumerable<BizDC.ShareNoteResult> GetAllSharedNotes(string studentId, string courseId);

        /// <summary>
        /// Gets the note settings.
        /// </summary>
        /// <param name="studentId">The student id.</param>
        /// <param name="courseId">The course id.</param>
        /// <param name="itemId">The item id.</param>
        /// <param name="reviewId">The review id.</param>
        /// <param name="enrollmentId">The enrollment id.</param>
        /// <returns></returns>
        IEnumerable<BizDC.ShareNoteResult> GetNoteSettings(string studentId, string courseId, string itemId,
            string reviewId, string enrollmentId);

        /// <summary>
        /// Gets the item highlight notes.
        /// </summary>
        /// <param name="noteSearch">The note search.</param>
        /// <returns></returns>
        IEnumerable<Biz.DataContracts.Highlight> GetItemHighlightNotes(XElement noteSearch);

        /// <summary>
        /// Gets the review highlight notes.
        /// </summary>
        /// <param name="noteSearch">The note search.</param>
        /// <returns></returns>
        IEnumerable<Biz.DataContracts.Highlight> GetReviewHighlightNotes(XElement noteSearch);

        /// <summary>
        /// Gets the item general notes.
        /// </summary>
        /// <param name="noteSearch">The note search.</param>
        /// <returns></returns>
        IEnumerable<BizDC.Note> GetItemGeneralNotes(XElement noteSearch);

        /// <summary>
        /// Gets the submission highlight notes.
        /// </summary>
        /// <param name="noteSearch">The note search.</param>
        /// <returns></returns>
        IEnumerable<Biz.DataContracts.Highlight> GetSubmissionHighlightNotes(XElement noteSearch);

        /// <summary>
        /// Gets the note count.
        /// </summary>
        /// <param name="highlightType">Type of the highlight.</param>
        /// <param name="courseId">The course id.</param>
        /// <param name="itemId">The item id.</param>
        /// <param name="reviewId">The review id.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="enrollmentId">The enrollment id.</param>
        /// <param name="highlightCount">The highlight count.</param>
        /// <param name="noteCount">The note count.</param>
        void GetNoteCount(BizDC.PxHighlightType highlightType, string courseId, string itemId, string reviewId,
            string userId, string enrollmentId, out int highlightCount, out int noteCount);

        /// <summary>
        /// Gets the notes by user.
        /// </summary>
        /// <param name="highlightType">Type of the highlight.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="courseId">The course id.</param>
        /// <param name="enrollmentId">The enrollment id.</param>
        /// <returns></returns>
        IEnumerable<BizDC.Note> GetNotesByUser(BizDC.PxHighlightType highlightType, string userId, string courseId,
            string enrollmentId);

        /// <summary>
        /// Gets the notes for peer review by user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="enrollmentIds">The enrollment ids.</param>
        /// <returns></returns>
        IEnumerable<BizDC.Note> GetNotesForPeerReviewByUser(string userId, string enrollmentIds);

        /// <summary>
        /// Gets the submission general notes.
        /// </summary>
        /// <param name="noteSearch">The note search.</param>
        /// <returns></returns>
        IEnumerable<BizDC.Note> GetSubmissionGeneralNotes(XElement noteSearch);

        /// <summary>
        /// Gets the review general notes.
        /// </summary>
        /// <param name="notesSearch">The notes search.</param>
        /// <returns></returns>
        IEnumerable<BizDC.Note> GetReviewGeneralNotes(XElement notesSearch);

        /// <summary>
        /// Initializes the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="courseId">The course id.</param>
        /// <param name="userType">Type of the user.</param>
        void InitializeUser(string userId, string firstName, string lastName, string courseId, int userType);

        /// <summary>
        /// Updates the note settings.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="courseId">The course id.</param>
        /// <param name="sharerId">The sharer id.</param>
        /// <param name="sharerHighlights">The sharer highlights.</param>
        /// <param name="sharerNotes">The sharer notes.</param>
        /// <param name="myHighlights">My highlights.</param>
        /// <param name="myNotes">My notes.</param>
        /// <param name="instructorHighlights">The instructor highlights.</param>
        /// <param name="instructorNotes">The instructor notes.</param>
        void UpdateNoteSettings(string userId, string courseId, string sharerId, bool? sharerHighlights,
            bool? sharerNotes, bool? myHighlights, bool? myNotes, bool? instructorHighlights, bool? instructorNotes);

    }
}
