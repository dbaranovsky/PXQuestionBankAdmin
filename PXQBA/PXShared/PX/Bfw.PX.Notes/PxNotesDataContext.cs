using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using System.Xml.Linq;
using Bfw.Common.Collections;
using Bfw.PX.Biz.DataContracts;
using BizDC = Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Notes
{
    /// <summary>
    /// Data context for notes.
    /// </summary>
    public class NotesDataContext : INotesDataContext
    {
        /// <summary>
        /// The notes DB.
        /// </summary>
        private readonly NotesDataClassesDataContext _notesDb;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotesDataContext"/> class.
        /// </summary>
        public NotesDataContext()
        {
            _notesDb = new NotesDataClassesDataContext(ConfigurationManager.ConnectionStrings["PXData"].ConnectionString);
        }

        /// <summary>
        /// Adds the highlight note.
        /// </summary>
        /// <param name="highlight">The highlight.</param>
        /// <param name="highlightId">The highlight id.</param>
        /// <param name="noteId">The note id.</param>
        public void AddHighlightNote(Biz.DataContracts.Highlight highlight, out Guid? highlightId, out Guid? noteId)
        {
            highlightId = Guid.Empty;
            noteId = Guid.Empty;
            try
            {
                _notesDb.Connection.Open();
                _notesDb.Transaction = _notesDb.Connection.BeginTransaction();
                var note = highlight.Notes.FirstOrDefault();
                _notesDb.AddHighlightNote(highlight.Text, highlight.Description, note.Text, (int)highlight.HighlightType,
                            highlight.ItemId, highlight.ReviewId, highlight.EnrollmentId, highlight.CourseId, highlight.UserId, highlight.FirstName,
                            highlight.LastName, highlight.Color, highlight.Public, note.Public, ref highlightId, ref noteId);
                _notesDb.Transaction.Commit();
                _notesDb.Connection.Close();
            }
            catch (Exception)
            {
                _notesDb.Transaction.Rollback();
            }
        }

        /// <summary>
        /// Adds the highlight.
        /// </summary>
        /// <param name="highlight">The highlight.</param>
        /// <returns></returns>
        public Guid AddHighlight(BizDC.Highlight highlight)
        {
            Guid? highlightId = Guid.Empty;
            try
            {
                _notesDb.Connection.Open();
                _notesDb.Transaction = _notesDb.Connection.BeginTransaction();
                highlightId = Guid.NewGuid();
                _notesDb.AddHighlight(highlight.Text, highlight.Description, (int)highlight.HighlightType,
                         highlight.ItemId, highlight.ReviewId, highlight.EnrollmentId, highlight.CourseId, highlight.Public, highlight.UserId, highlight.FirstName,
                         highlight.LastName, highlight.Color,highlight.Start, highlight.StartOffset,highlight.End,highlight.EndOffset, ref highlightId);
                _notesDb.Transaction.Commit();
                _notesDb.Connection.Close();
            }
            catch (Exception)
            {
                _notesDb.Transaction.Rollback();
            }
            return highlightId.Value;
        }

        /// <summary>
        /// Adds the note.
        /// </summary>
        /// <param name="note">The note.</param>
        /// <returns></returns>
        public Guid AddNote(BizDC.Note note)
        {
            var noteId = Guid.Empty;
            try
            {
                _notesDb.Connection.Open();
                _notesDb.Transaction = _notesDb.Connection.BeginTransaction();
                noteId = Guid.NewGuid();
                _notesDb.AddNote(noteId, note.Text, note.Description, (int)note.HighlightType, note.IsGeneral,
                         note.ItemId, note.ReviewId, note.EnrollmentId, note.Public, note.UserId, note.FirstName,
                         note.LastName, note.CourseId);
                _notesDb.Transaction.Commit();
                _notesDb.Connection.Close();
            }
            catch (Exception)
            {
                _notesDb.Transaction.Rollback();
            }
            return noteId;
        }


        /// <summary>
        /// Adds the note to highlight.
        /// </summary>
        /// <param name="note">The note.</param>
        public void AddNoteToHighlight(BizDC.Note note)
        {
            try
            {
                Guid? noteId = null;
                _notesDb.Connection.Open();
                _notesDb.Transaction = _notesDb.Connection.BeginTransaction();
                _notesDb.AddNoteToHighlight(note.HighlightId, note.Text, note.Description, (int)note.HighlightType, note.ItemId, note.ReviewId, note.EnrollmentId,
                                  note.Public, note.UserId, note.FirstName, note.LastName, note.CourseId, ref noteId);
                _notesDb.Transaction.Commit();
                _notesDb.Connection.Close();

                note.NoteId = noteId.Value;
            }
            catch (Exception)
            {
                _notesDb.Transaction.Rollback();
            }

        }

        /// <summary>
        /// Adds the note to top note.
        /// </summary>
        /// <param name="note">The note.</param>
        public void AddNoteToTopNote(BizDC.Note note)
        {
            try
            {
                Guid? noteId = null;
                _notesDb.Connection.Open();
                _notesDb.Transaction = _notesDb.Connection.BeginTransaction();
                _notesDb.AddNoteToTopNote(note.TopNoteId, note.Text, note.Description, (int) note.HighlightType,
                    note.ItemId, note.ReviewId, note.EnrollmentId,
                    note.Public, note.UserId, note.FirstName, note.LastName, note.CourseId, ref noteId);
                _notesDb.Transaction.Commit();
                _notesDb.Connection.Close();

                note.NoteId = noteId.HasValue ? noteId.Value : Guid.Empty;
            }
            catch (Exception)
            {
                _notesDb.Transaction.Rollback();
                    
            }

        }
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
        public void UpdateHighlightStatus(string highlightId, string userId, string itemId, string reviewId, string courseId, string enrollmentId, bool updateOnlyEmptyHighlights, int status)
        {
            try
            {
                _notesDb.Connection.Open();
                _notesDb.Transaction = _notesDb.Connection.BeginTransaction();
                Guid? guidId = null;

                if (!string.IsNullOrEmpty(highlightId))
                {
                    guidId = new Guid(highlightId);
                }

                _notesDb.UpdateHighlightStatus(guidId, userId, itemId, reviewId, courseId, enrollmentId, updateOnlyEmptyHighlights, status);
                _notesDb.Transaction.Commit();
                _notesDb.Connection.Close();
            }
            catch (Exception)
            {
                _notesDb.Transaction.Rollback();
            }

        }

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
        public void UpdateNoteStatus(string highlightId, string noteId, string userId, string itemId, string reviewId, string courseId, string enrollmentId, bool updateParentHighlight, int status)
        {
            try
            {
                _notesDb.Connection.Open();
                _notesDb.Transaction = _notesDb.Connection.BeginTransaction();

                Guid? highlightGuid = null;

                if (!string.IsNullOrEmpty(highlightId))
                    highlightGuid = new Guid(highlightId);

                Guid? noteGuid = null;

                if (!string.IsNullOrEmpty(noteId))
                {
                    noteGuid = new Guid(noteId);
                }

                _notesDb.UpdateNoteStatus(highlightGuid, noteGuid, userId, itemId, reviewId, courseId, enrollmentId, updateParentHighlight, status);
                _notesDb.Transaction.Commit();
                _notesDb.Connection.Close();
            }
            catch (Exception)
            {
                _notesDb.Transaction.Rollback();
            }

        }

        /// <summary>
        /// Updates the color of the highlight.
        /// </summary>
        /// <param name="highlightId">The highlight id.</param>
        /// <param name="color">The color.</param>
        public void UpdateHighlightColor(string highlightId, string color)
        {
            try
            {
                _notesDb.Connection.Open();
                _notesDb.Transaction = _notesDb.Connection.BeginTransaction();
                var guidId = new Guid(highlightId);
                _notesDb.UpdateHighlightColor(guidId, color);
                _notesDb.Transaction.Commit();
                _notesDb.Connection.Close();
            }
            catch (Exception)
            {
                _notesDb.Transaction.Rollback();
            }

        }

        /// <summary>
        /// Updates the note.
        /// </summary>
        /// <param name="noteId">The note id.</param>
        /// <param name="noteText">The note text.</param>
        public void UpdateNote(string noteId, string noteText)
        {
            try
            {
                _notesDb.Connection.Open();
                _notesDb.Transaction = _notesDb.Connection.BeginTransaction();
                var guidId = new Guid(noteId);
                _notesDb.UpdateNote(guidId, noteText);
                _notesDb.Transaction.Commit();
                _notesDb.Connection.Close();
            }
            catch (Exception)
            {
                _notesDb.Transaction.Rollback();
            }

        }

        /// <summary>
        /// Shares the highlight.
        /// </summary>
        /// <param name="highlightId">The highlight id.</param>
        /// <param name="share">if set to <c>true</c> [share].</param>
        public void ShareHighlight(string highlightId, bool share)
        {
            try
            {
                _notesDb.Connection.Open();
                _notesDb.Transaction = _notesDb.Connection.BeginTransaction();
                var guidId = new Guid(highlightId);
                _notesDb.ShareHighlight(guidId, share);
                _notesDb.Transaction.Commit();
                _notesDb.Connection.Close();
            }
            catch (Exception)
            {
                _notesDb.Transaction.Rollback();
            }

        }

        /// <summary>
        /// Shares the note.
        /// </summary>
        /// <param name="noteId">The note id.</param>
        /// <param name="share">if set to <c>true</c> [share].</param>
        public void ShareNote(string noteId, bool share)
        {
            try
            {
                _notesDb.Connection.Open();
                _notesDb.Transaction = _notesDb.Connection.BeginTransaction();
                var guidId = new Guid(noteId);
                _notesDb.ShareNote(guidId, share);
                _notesDb.Transaction.Commit();
                _notesDb.Connection.Close();
            }
            catch (Exception)
            {
                _notesDb.Transaction.Rollback();
            }

        }

        /// <summary>
        /// Shares the notes.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void ShareNotes(NoteSettingEntity entity)
        {
            try
            {
                _notesDb.Connection.Open();
                _notesDb.Transaction = _notesDb.Connection.BeginTransaction();
                _notesDb.ShareNotes(entity.StudentId, entity.CourseId, entity.SharedStudentId,
                    entity.FirstNameSharer, entity.LastNameSharer, entity.FirstNameSharee, entity.LastNameSharee);
                _notesDb.Transaction.Commit();
                _notesDb.Connection.Close();
            }
            catch (Exception)
            {
                _notesDb.Transaction.Rollback();
            }

        }

        /// <summary>
        /// Stops the sharing.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void StopSharing(NoteSettingEntity entity)
        {
            try
            {
                _notesDb.Connection.Open();
                _notesDb.Transaction = _notesDb.Connection.BeginTransaction();
                _notesDb.StopSharingToUser(entity.StudentId, entity.SharedStudentId, entity.CourseId);
                _notesDb.Transaction.Commit();
                _notesDb.Connection.Close();
            }
            catch (Exception)
            {
                _notesDb.Transaction.Rollback();
            }

        }

        /// <summary>
        /// Gets all shared notes.
        /// </summary>
        /// <param name="studentId">The student id.</param>
        /// <param name="courseId">The course id.</param>
        /// <returns></returns>
        public IEnumerable<ShareNoteResult> GetAllSharedNotes(string studentId, string courseId)
        {
            var results = _notesDb.GetAllSharedNotes(studentId, courseId).ToList();
            return results;
        }

        /// <summary>
        /// Gets the note settings.
        /// </summary>
        /// <param name="studentId">The student id.</param>
        /// <param name="courseId">The course id.</param>
        /// <param name="itemId">The item id.</param>
        /// <param name="reviewId">The review id.</param>
        /// <param name="enrollmentId">The enrollment id.</param>
        /// <returns></returns>
        public IEnumerable<ShareNoteResult> GetNoteSettings(string studentId, string courseId, string itemId, string reviewId, string enrollmentId)
        {
            var results = _notesDb.GetAllNoteSettings(studentId, courseId, itemId, reviewId, enrollmentId);
            return results.ToList();
        }

        /// <summary>
        /// Gets the item highlight notes.
        /// </summary>
        /// <param name="noteSearch">The note search.</param>
        /// <returns></returns>
        public IEnumerable<Biz.DataContracts.Highlight> GetItemHighlightNotes(XElement noteSearch)
        {
            var multipleResults = _notesDb.GetItemHighlightNotes(noteSearch);
            return SetHighlightNoteRelation(multipleResults, noteSearch);
        }

        /// <summary>
        /// Gets the item general notes.
        /// </summary>
        /// <param name="noteSearch">The note search.</param>
        /// <returns></returns>
        public IEnumerable<BizDC.Note> GetItemGeneralNotes(XElement noteSearch)
        {
            var notesResults = _notesDb.GetItemGeneralNotes(noteSearch).ToList();
            return notesResults;
        }

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
        public void UpdateNoteSettings(string userId, string courseId, string sharerId, bool? sharerHighlights, bool? sharerNotes, bool? myHighlights, bool? myNotes, bool? instructorHighlights, bool? instructorNotes)
        {
            _notesDb.UpdateNoteSettings(userId, courseId, sharerId, sharerHighlights, sharerNotes, myHighlights, myNotes, instructorHighlights, instructorNotes);
        }

        /// <summary>
        /// Initializes the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="courseId">The course id.</param>
        /// <param name="userType">Type of the user.</param>
        public void InitializeUser(string userId, string firstName, string lastName, string courseId, int userType)
        {
            _notesDb.InitializeUser(userId, firstName, lastName, courseId, userType);
        }

        /// <summary>
        /// Gets the submission highlight notes.
        /// </summary>
        /// <param name="noteSearch">The note search.</param>
        /// <returns></returns>
        public IEnumerable<Biz.DataContracts.Highlight> GetSubmissionHighlightNotes(XElement noteSearch)
        {
            var multipleResults = _notesDb.GetSubmissionHighlightNotes(noteSearch);
            return SetHighlightNoteRelation(multipleResults, noteSearch);
        }

        /// <summary>
        /// Gets the submission general notes.
        /// </summary>
        /// <param name="noteSearch">The note search.</param>
        /// <returns></returns>
        public IEnumerable<BizDC.Note> GetSubmissionGeneralNotes(XElement noteSearch)
        {
            var notesResults = _notesDb.GetSubmissionGeneralNotes(noteSearch).ToList();
            return notesResults;
        }

        /// <summary>
        /// Gets the review highlight notes.
        /// </summary>
        /// <param name="noteSearch">The note search.</param>
        /// <returns></returns>
        public IEnumerable<Biz.DataContracts.Highlight> GetReviewHighlightNotes(XElement noteSearch)
        {
            var multipleResults = _notesDb.GetReviewHighlightNotes(noteSearch);
            return SetHighlightNoteRelation(multipleResults, noteSearch);
        }

        /// <summary>
        /// Gets the review general notes.
        /// </summary>
        /// <param name="notesSearch">The notes search.</param>
        /// <returns></returns>
        public IEnumerable<BizDC.Note> GetReviewGeneralNotes(XElement notesSearch)
        {
            var notesResults = _notesDb.GetReviewGeneralNotes(notesSearch).ToList();
            return notesResults;
        }

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
        public void GetNoteCount(PxHighlightType highlightType, string courseId, string itemId, string reviewId, string userId, string enrollmentId, out int highlightCount, out int noteCount)
        {
            highlightCount = 0;
            noteCount = 0;
            var noteCountResult = _notesDb.GetNoteCount(itemId, reviewId, courseId, userId, enrollmentId, (int)highlightType).SingleOrDefault();
            if (noteCountResult == null)
            {
                return;
            }
            highlightCount = noteCountResult.HighlightCount.Value;
            noteCount = noteCountResult.NoteCount.Value;
        }

        /// <summary>
        /// Gets the notes by user.
        /// </summary>
        /// <param name="highlightType">Type of the highlight.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="courseId">The course id.</param>
        /// <param name="enrollmentId">The enrollment id.</param>
        /// <returns></returns>
        public IEnumerable<BizDC.Note> GetNotesByUser(PxHighlightType highlightType, string userId, string courseId, string enrollmentId)
        {
            return _notesDb.GetAllNotesByUser((int)highlightType, userId, courseId, enrollmentId).ToList();
        }

        /// <summary>
        /// Gets the notes for peer review by user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="enrollmentIds">The enrollment ids.</param>
        /// <returns></returns>
        public IEnumerable<BizDC.Note> GetNotesForPeerReviewByUser(string userId, string enrollmentIds)
        {
            return _notesDb.GetNotesForPeerReviewByUser(userId, enrollmentIds).ToList();
        }

        /// <summary>
        /// Sets the highlight note relation.
        /// </summary>
        /// <param name="multipleResults">The multiple results.</param>
        /// <param name="noteSearch">The note search.</param>
        /// <returns></returns>
        private static IEnumerable<Biz.DataContracts.Highlight> SetHighlightNoteRelation(IMultipleResults multipleResults, XElement noteSearch)
        {
            var hightlightResults = multipleResults.GetResult<Biz.DataContracts.Highlight>().ToList();
            var noteResults = multipleResults.GetResult<BizDC.Note>().ToList();
            var noteType = (BizDC.NoteType)Convert.ToInt32(noteSearch.Element("noteType").Value);
            PxHighlightType highlightType;
            Enum.TryParse<PxHighlightType>(noteSearch.Element("highlightType").Value, out highlightType);
            foreach (var highlightResult in hightlightResults)
            {
                var result = highlightResult;
                highlightResult.Notes = noteResults.Where(n => n.HighlightId == result.HighlightId);
                highlightResult.NoteType = noteType;
                highlightResult.HighlightType = highlightType;
                highlightResult.Notes = highlightResult.Notes.Map(n => { n.HighlightType = highlightType; return n; });
            }

            IEnumerable<BizDC.Note> topNotes = noteResults.Where(n => !n.HighlightId.HasValue && !n.TopNoteId.HasValue);
            if (topNotes.Any())
            {
                foreach (var note in topNotes)
                {
                    var topHighlight = new Biz.DataContracts.Highlight
                                           {
                                               ItemId = note.ItemId,
                                               ReviewId = note.ReviewId,
                                               EnrollmentId = note.EnrollmentId,
                                               CourseId = note.CourseId,
                                               HighlightType = highlightType,
                                               NoteType = NoteType.GeneralNote,
                                               Description = note.Description,
                                               UserId = note.UserId,
                                               Public = note.Public,
                                               Status = note.Status
                                           };
                    note.IsGeneral = true;
                    note.HighlightType = highlightType;
                    var responses = new List<Biz.DataContracts.Note>() { note };
                    responses.AddRange(noteResults.Where(n => n.TopNoteId.HasValue && n.TopNoteId == note.NoteId));
                    topHighlight.Notes = responses;
                    hightlightResults.Insert(0, topHighlight);
                }
            }

            return hightlightResults;
        }

    }
}
