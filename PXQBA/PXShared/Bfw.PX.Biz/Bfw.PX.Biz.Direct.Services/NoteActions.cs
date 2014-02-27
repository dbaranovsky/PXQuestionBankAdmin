using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.Common.Logging;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.PX.Notes;
using AgxDC = Bfw.Agilix.DataContracts;
using BizDC = Bfw.PX.Biz.DataContracts;
using Highlight = Bfw.PX.Biz.DataContracts.Highlight;
using Note = Bfw.PX.Biz.DataContracts.Note;

namespace Bfw.PX.Biz.Direct.Services
{
    /// <summary>
    /// Implementation of the INoteActions interface.
    /// </summary>
    public class NoteActions : INoteActions
    {
        public INotesDataContext NotesData { get; set; }
        protected IBusinessContext Context { get; set; }

        /// <summary>
        /// Gets or sets the session manager.
        /// </summary>
        /// <value>
        /// The session manager.
        /// </value>
        protected ISessionManager SessionManager { get; set; }

        public NoteActions(IBusinessContext context, ISessionManager sessionManager)
        {
            Context = context;
            SessionManager = sessionManager;
            NotesData = new NotesDataContext();
        }

        #region INoteActions Members

        /// <summary>
        /// Adds a highlight and note to the system.
        /// </summary>
        /// <param name="hlNotes">The highlight Notes.</param>
        /// <param name="highlightId">The highlight ID.</param>
        /// <param name="noteId">The note ID.</param>
        public void AddHighlightNote(BizDC.Highlight hlNotes, out Guid? highlightId, out Guid? noteId)
        {
            NotesData.AddHighlightNote(hlNotes, out highlightId, out noteId);
        }

        /// <summary>
        /// Adds a highlight to the system.
        /// </summary>
        /// <param name="highlight">The highlight data to store.</param>
        /// <returns></returns>
        public Guid AddHighlight(BizDC.Highlight highlight)
        {
            return NotesData.AddHighlight(highlight);
        }

        /// <summary>
        /// Adds a note to the system.
        /// </summary>
        /// <param name="note">The note data to store.</param>
        /// <returns></returns>
        public Guid AddNote(BizDC.Note note)
        {
            return NotesData.AddNote(note);
        }

        /// <summary>
        /// Adds a note to an existing highlight.
        /// </summary>
        /// <param name="note">The note.</param>
        public void AddNoteToHighlight(BizDC.Note note)
        {
            NotesData.AddNoteToHighlight(note);
        }

        /// <summary>
        /// Adds a note to an existing top note.
        /// </summary>
        /// <param name="note">The note.</param>
        private void AddNoteToTopNote(Note note)
        {
            NotesData.AddNoteToTopNote(note);
        }

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
        public Note AddNoteToHighlight(string highlightId, string itemId, string reviewId, String commentText, string description, PxHighlightType highlightType, bool isPublic)
        {
            using (Context.Tracer.DoTrace("NoteActions.AddNoteToHighlight(highlightId={0},itemId={1},reviewId={2})", highlightId, itemId, reviewId))
            {
                var note = new BizDC.Note
                {
                    HighlightId = new Guid(highlightId),
                    Text = commentText,
                    Description = description,
                    UserId = Context.CurrentUser.Id,
                    ItemId = itemId,
                    ReviewId = reviewId,
                    EnrollmentId = Context.EnrollmentId,
                    FirstName = Context.CurrentUser.FirstName,
                    LastName = Context.CurrentUser.LastName,
                    HighlightType = highlightType,
                    IsGeneral =
                        String.IsNullOrEmpty(commentText)
                            ? true
                            : false,
                    Public = isPublic
                };
                AddNoteToHighlight(note);
                return note;
            }
        }

        /// <summary>
        /// Adds a note to an existing top note.
        /// </summary>
        /// <param name="topNoteId">The top note ID.</param>
        /// <param name="itemId">The item ID.</param>
        /// <param name="commentText">The comment text.</param>
        /// <param name="description">The description.</param>
        /// <param name="isPublic">if set to <c>true</c> [is public].</param>
        /// <returns></returns>
        public Note AddNoteToTopNote(string topNoteId, string itemId, string commentText, string description, bool isPublic)
        {
            using (Context.Tracer.DoTrace("NoteActions.AddNoteToTopNote(topNoteId={0},itemId={1},commentText={2}, description={3}, isPublic={4})", topNoteId, itemId, commentText, description, isPublic))
            {
                var note = new Note
                {
                    TopNoteId = new Guid(topNoteId),
                    Text = commentText,
                    Description = description,
                    UserId = Context.CurrentUser.Id,
                    ItemId = itemId,
                    EnrollmentId = Context.EnrollmentId,
                    FirstName = Context.CurrentUser.FirstName,
                    LastName = Context.CurrentUser.LastName,
                    HighlightType = PxHighlightType.GeneralContent,
                    IsGeneral = true,
                    Public = isPublic,
                    CourseId = Context.CourseId
                };
                AddNoteToTopNote(note);
                return note;
            }
        }
        /// <summary>
        /// Updates an existing highlight's status. A highlight can be active, locked or deleted.
        /// </summary>
        /// <param name="highlightId">The highlight ID.</param>
        /// <param name="status">The status of the highlight.</param>
        /// <param name="courseId">The course ID.</param>
        public void UpdateHighlightStatus(string highlightId, HighlightStatus status, string courseId)
        {
            NotesData.UpdateHighlightStatus(highlightId, null, null, null, courseId, null, false, (int)status);
        }

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
        public void UpdateHighlightStatus(string highlightId, string userId, string itemId, string reviewId, string courseId, string enrollmentId, bool updateOnlyEmptyHighlights, HighlightStatus status)
        {
            using (Context.Tracer.DoTrace("NoteActions.UpdateHighlightStatus(highlightId={0},userId={1},itemId={2},reviewId={3},courseId={4},enrollmentId={5})", highlightId, userId, itemId, reviewId, courseId, enrollmentId))
            {
                if (string.IsNullOrEmpty(itemId)) itemId = null;
                if (string.IsNullOrEmpty(reviewId)) reviewId = null;
                if (string.IsNullOrEmpty(enrollmentId)) enrollmentId = null;
                if (string.IsNullOrEmpty(courseId)) courseId = null;

                NotesData.UpdateHighlightStatus(highlightId, userId, itemId, reviewId, courseId, enrollmentId, updateOnlyEmptyHighlights, (int)status);
            }
        }

        /// <summary>
        /// Updates an existing highlight note status. Status can be active, locked or deleted.
        /// </summary>
        /// <param name="highlightId">The highlight ID.</param>
        /// <param name="status">The status of the highlight.</param>
        /// <param name="updateParentHighlight">if set to <c>true</c> update parent highlight as well.</param>
        /// <param name="courseId">The course ID.</param>
        public void UpdateHighlightNoteStatus(string highlightId, HighlightStatus status, bool updateParentHighlight, string courseId)
        {
            NotesData.UpdateNoteStatus(highlightId, null, null, null, null, courseId, null, updateParentHighlight, (int)status);
        }

        /// <summary>
        /// Updates an existing note status. Status can be active, locked or deleted.
        /// </summary>
        /// <param name="noteId">The note ID.</param>
        /// <param name="status">The status of the highlight.</param>
        /// <param name="courseId">The course ID.</param>
        public void UpdateNoteStatus(string noteId, HighlightStatus status, string courseId)
        {
            NotesData.UpdateNoteStatus(null, noteId, null, null, null, courseId, null, false, (int)status);
        }

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
        public void UpdateNoteStatus(string noteId, string userId, string itemId, string reviewId, string courseId, string enrollmentId, bool updateParentHighlight, HighlightStatus status)
        {
            using (Context.Tracer.DoTrace("NoteActions.UpdateNoteStatus(noteId={0},userId={1},itemId={2},reviewId={3},courseId={4},enrollmentId={5})", noteId, userId, itemId, reviewId, courseId, enrollmentId))
            {
                if (string.IsNullOrEmpty(itemId)) itemId = null;
                if (string.IsNullOrEmpty(reviewId)) reviewId = null;
                if (string.IsNullOrEmpty(enrollmentId)) enrollmentId = null;
                if (string.IsNullOrEmpty(courseId)) courseId = null;

                NotesData.UpdateNoteStatus(null, noteId, userId, itemId, reviewId, courseId, enrollmentId,
                                           updateParentHighlight, (int)status);
            }
        }

        /// <summary>
        /// Updates the color of the highlight.
        /// Color setting for the Highlight with accepted string values "color-1", "color-2",..."color-n"
        /// mapping to a CSS rule.
        /// </summary>
        /// <param name="highlightId">The highlight ID.</param>
        /// <param name="color">Color setting for the Highlight.</param>
        public void UpdateHighlightColor(string highlightId, string color)
        {
            NotesData.UpdateHighlightColor(highlightId, color);
        }

        /// <summary>
        /// Updates the text of an existing note.
        /// </summary>
        /// <param name="noteId">ID of the note to update.</param>
        /// <param name="noteText">The note text.</param>
        public void UpdateNote(string noteId, string noteText)
        {
            NotesData.UpdateNote(noteId, noteText);
        }

        /// <summary>
        /// Toggles the sharing of a highlight.
        /// </summary>
        /// <param name="highlightId">ID of highlight to share.</param>
        /// <param name="share">if set to <c>true</c> makes the highlight publicly shared.</param>
        public void ShareHighlight(string highlightId, bool share)
        {
            NotesData.ShareHighlight(highlightId, share);
        }

        /// <summary>
        /// Creates a note sharing relationship between two users.
        /// </summary>
        /// <param name="shareNotes">The note sharing relationship info.</param>
        public void ShareNotes(ShareNoteResult shareNotes)
        {
            NotesData.ShareNotes(shareNotes.ToShareNote());
        }

        /// <summary>
        /// Gets a set of highlights that match the specified <see cref="NoteSearch"/> criteria.
        /// </summary>
        /// <param name="noteSearch">The note search criteria.</param>
        /// <param name="gradeActions">An IGradeActions implementation.</param>
        /// <returns></returns>
        public IEnumerable<BizDC.Highlight> GetHighlights(NoteSearch noteSearch, IGradeActions gradeActions)
        {
            using (Context.Tracer.StartTrace("NoteActions.GetHighlights"))
            {
                switch (noteSearch.HighlightType)
                {
                    case PxHighlightType.GeneralContent:
                        return GetItemHighlightNotes(noteSearch).ToList();
                    case PxHighlightType.WritingAssignment:
                        return GetSubmissionHighlightNotes(noteSearch, gradeActions).ToList();
                    case PxHighlightType.PeerReview:
                        return GetReviewHighlightNotes(noteSearch, gradeActions).ToList();
                    default:
                        return new List<BizDC.Highlight>();
                }
            }
        }

        /// <summary>
        /// Removes sharing of a note between users.
        /// </summary>
        /// <param name="result">The result.</param>
        public void StopSharing(ShareNoteResult sharedNote)
        {
            NotesData.StopSharing(sharedNote.ToShareNote());
        }

        /// <summary>
        /// Gets a set of notes that match the specified <see cref="NoteSearch"/> criteria.
        /// </summary>
        /// <param name="noteSearch">The note search critera.</param>
        /// <returns></returns>
        public IEnumerable<BizDC.Note> GetNotes(NoteSearch noteSearch)
        {
            switch (noteSearch.HighlightType)
            {
                case PxHighlightType.GeneralContent:
                    return GetItemGeneralNotes(noteSearch).ToList();
                case PxHighlightType.WritingAssignment:
                    return GetSubmissionGeneralNotes(noteSearch).ToList();
                case PxHighlightType.PeerReview:
                    return GetReviewGeneralNotes(noteSearch).ToList();
                default:
                    return new List<BizDC.Note>();
            }
        }

        /// <summary>
        /// Gets a highlight by highlight ID and type.
        /// </summary>
        /// <param name="highlightId">ID of the highlight to get.</param>
        /// <param name="highlightType">Type of the highlight.</param>
        /// <returns></returns>
        public BizDC.Highlight GetHighlightByHighlightId(string highlightId, PxHighlightType highlightType)
        {
            using (Context.Tracer.DoTrace("NoteActions.GetHighlightByHighlightId(highlightId={0})", highlightId))
            {
                var noteSearch = new NoteSearch { HighlightId = highlightId, NoteType = (int)NoteType.HighlightNote, HighlightType = highlightType };
                switch (noteSearch.HighlightType)
                {
                    case PxHighlightType.GeneralContent:
                        return GetItemHighlightNotes(noteSearch).FirstOrDefault();
                    case PxHighlightType.WritingAssignment:
                        return GetSubmissionHighlightNotes(noteSearch, null).FirstOrDefault();
                    case PxHighlightType.PeerReview:
                        return GetReviewHighlightNotes(noteSearch, null).FirstOrDefault();
                    default:
                        return null;
                }
            }
        }

        /// <summary>
        /// Gets the notes by highlight ID and type.
        /// </summary>
        /// <param name="highlightId">ID of the highlight.</param>
        /// <param name="highlightType">Type of the highlight.</param>
        /// <returns></returns>
        public IEnumerable<BizDC.Note> GetNotesByHighlightId(string highlightId, PxHighlightType highlightType)
        {
            using (Context.Tracer.DoTrace("NoteActions.GetNotesByHighlightId(highlightId={0})", highlightId))
            {
                var noteSearch = new NoteSearch { HighlightId = highlightId, NoteType = (int)NoteType.HighlightNote, HighlightType = highlightType };
                var hResult = new List<BizDC.Highlight>();
                var nResult = new List<BizDC.Note>();
                switch (noteSearch.HighlightType)
                {
                    case PxHighlightType.GeneralContent:
                        hResult = GetItemHighlightNotes(noteSearch).ToList();
                        break;
                    case PxHighlightType.WritingAssignment:
                        hResult = GetSubmissionHighlightNotes(noteSearch, null).ToList();
                        break;
                    case PxHighlightType.PeerReview:
                        hResult = GetReviewHighlightNotes(noteSearch, null).ToList();
                        break;
                }
                if (hResult.Any())
                {
                    nResult = hResult.FirstOrDefault().Notes.ToList();
                }
                return nResult;
            }
        }

        /// <summary>
        /// Gets the item highlight notes that match the specified <see cref="NoteSearch" /> criteria.
        /// Item highlights are highlights on General/eBook content.
        /// </summary>
        /// <param name="noteSearch">The note search criteria.</param>
        /// <returns></returns>
        public IEnumerable<BizDC.Highlight> GetItemHighlightNotes(NoteSearch notesSearch)
        {
            using (Context.Tracer.StartTrace("NoteActions.GetItemHighlightNotes"))
            {
                var xDoc = PxXmlSerializer.Serialize(notesSearch, "");
                return NotesData.GetItemHighlightNotes(xDoc.Root);
            }
        }

        /// <summary>
        /// Gets the item general notes that match the specified <see cref="NoteSearch" /> criteria.
        /// Item general notes are notes on General/eBook content that are not attached to highlights.
        /// </summary>
        /// <param name="noteSearch">The note search criteria.</param>
        /// <returns></returns>
        public IEnumerable<BizDC.Note> GetItemGeneralNotes(NoteSearch notesSearch)
        {
            using (Context.Tracer.StartTrace("NoteActions.GetItemGeneralNotes"))
            {
                notesSearch.NoteType = (int)NoteType.GeneralNote;
                var xDoc = PxXmlSerializer.Serialize(notesSearch, "");
                return NotesData.GetItemGeneralNotes(xDoc.Root);
            }
        }

        /// <summary>
        /// Gets the submission highlight notes that match the specified <see cref="NoteSearch" /> criteria.
        /// Submission highlights are highlights on writing assignments.
        /// </summary>
        /// <param name="noteSearch">The note search criteria.</param>
        /// <param name="gradeActions">An IGradeActions implementation.</param>
        /// <returns></returns>
        public IEnumerable<BizDC.Highlight> GetSubmissionHighlightNotes(NoteSearch notesSearch, IGradeActions gradeActions)
        {
            using (Context.Tracer.StartTrace("NoteActions.GetSubmissionHighlightNotes"))
            {
                var xDoc = PxXmlSerializer.Serialize(notesSearch, "");

                //To write the logic to show only those submissions which are submitted. PLATX-4433

                var data = NotesData.GetSubmissionHighlightNotes(xDoc.Root);

                return data;
            }
        }

        /// <summary>
        /// Gets the submission general notes that match the specified <see cref="NoteSearch" /> criteria.
        /// Submission general notes are notes on writing assignments that are not attached to highlights.
        /// </summary>
        /// <param name="noteSearch">The note search criteria.</param>
        /// <returns></returns>
        public IEnumerable<BizDC.Note> GetSubmissionGeneralNotes(NoteSearch notesSearch)
        {
            using (Context.Tracer.StartTrace("NoteActions.GetSubmissionGeneralNotes"))
            {
                notesSearch.NoteType = (int)NoteType.GeneralNote;
                var xDoc = PxXmlSerializer.Serialize(notesSearch, "");
                return NotesData.GetSubmissionGeneralNotes(xDoc.Root);
            }
        }

        /// <summary>
        /// Gets peer review highlight notes that match the specified <see cref="NoteSearch" /> criteria.
        /// Review highlights are highlights on peer review assignments.
        /// </summary>
        /// <param name="noteSearch">The note search criteria.</param>
        /// <param name="gradeActions">An IGradeActions implementation.</param>
        /// <returns></returns>
        public IEnumerable<BizDC.Highlight> GetReviewHighlightNotes(NoteSearch notesSearch, IGradeActions gradeActions)
        {
            using (Context.Tracer.StartTrace("NoteActions.GetReviewHighlightNotes"))
            {
                var xDoc = PxXmlSerializer.Serialize(notesSearch, "");
                var data = NotesData.GetReviewHighlightNotes(xDoc.Root);

                if (gradeActions != null)
                {
                    data = GetStudentsSubmittedData(gradeActions, data);
                }

                return data;
            }
        }

        /// <summary>
        /// Gets the students submitted data.
        /// </summary>
        /// <param name="gradeActions">An IGradeActions implementation.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private IEnumerable<Highlight> GetStudentsSubmittedData(IGradeActions gradeActions, IEnumerable<Highlight> data)
        {
            using (Context.Tracer.StartTrace("NoteActions.GetStudentsSubmittedData"))
            {
                var cmd = new GetEntityEnrollmentList()
                {
                    SearchParameters = new AgxDC.EntitySearch { CourseId = Context.CourseId }
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                var enrollments = cmd.Enrollments;

                //find out the enrollments related to instructor who can grade the assignment.
                var instructors = from c in enrollments where c.Flags.ToString().Contains("GradeAssignment") select c.User.Id;


                var submissions = (from h in data
                                   let eId = enrollments.Where(c => h.UserId == c.User.Id).Select(c => c.Id).FirstOrDefault()
                                   select new Submission
                                              {
                                                  EnrollmentId = eId,
                                                  ItemId = h.ReviewId ?? h.ItemId
                                              }).ToList();
                var sub = gradeActions.GetStudentsSubmissionInfo(submissions).AsQueryable().ToList();
                var enrollmentsSubmitted = (from s in sub select s.EnrollmentId).ToList();
                var usersSubmitted = (from c in enrollments where enrollmentsSubmitted.Contains(c.Id) select c.User.Id).ToList();
                var result = from d in data where (usersSubmitted.Contains(d.UserId) || d.UserId == Context.CurrentUser.Id || instructors.Contains(d.UserId)) select d;

                return result;
            }
        }

        /// <summary>
        /// Gets peer review general notes that match the specified <see cref="NoteSearch" /> criteria.
        /// Review general notes are notes on peer review assignments that are not attached to highlights.
        /// </summary>
        /// <param name="noteSearch">The note search criteria.</param>
        /// <returns></returns>
        public IEnumerable<BizDC.Note> GetReviewGeneralNotes(NoteSearch notesSearch)
        {
            using (Context.Tracer.StartTrace("NoteActions.GetReviewGeneralNotes"))
            {
                notesSearch.NoteType = (int)NoteType.GeneralNote;
                var xDoc = PxXmlSerializer.Serialize(notesSearch, "");
                return NotesData.GetReviewGeneralNotes(xDoc.Root);
            }
        }

        /// <summary>
        /// Sets highlight note relations.
        /// </summary>
        /// <param name="highlights">The highlights.</param>
        /// <param name="notes">The notes.</param>
        /// <param name="noteSearch">The note search.</param>
        /// <returns></returns>
        private static IEnumerable<Biz.DataContracts.Highlight> SetHighlightNoteRelation(IEnumerable<Highlight> highlights, IEnumerable<Note> notes, NoteSearch noteSearch)
        {
            foreach (var highlight in highlights)
            {
                var h = highlight;
                highlight.Notes = notes.Where(n => n.HighlightId == h.HighlightId);
                SetHighlightProperties(highlight, noteSearch);
            }
            return highlights;
        }

        /// <summary>
        /// Sets the highlight properties.
        /// </summary>
        /// <param name="highlight">The highlight.</param>
        /// <param name="noteSearch">The note search.</param>
        private static void SetHighlightProperties(Highlight highlight, NoteSearch noteSearch)
        {
            highlight.NoteType = (NoteType)noteSearch.NoteType;
            highlight.HighlightType = noteSearch.HighlightType;
        }

        /// <summary>
        /// Search for a set of highlights in a given source string.
        /// If match found, wraps substring with html span tags
        /// </summary>
        /// <param name="strToHighlight">Source string to search in.</param>
        /// <param name="highlights">List of highlights that should be searched for.</param>
        /// <returns></returns>
        public string HighlightText(string strToHighlight, IList<BizDC.Highlight> highlights)
        {
            using (Context.Tracer.StartTrace("NoteActions.HighlightText"))
            {
                const string regTagName = @"((<[^>]*>)|\s|\t)+";

                Regex regExp;

                using (Context.Tracer.StartTrace("HighlightText"))
                {
                    foreach (var h in highlights)
                    {
                        // ignore page level highlights
                        if (String.IsNullOrEmpty(h.Text)) continue;

                        var words = h.Text.Split(default(Char[]), StringSplitOptions.RemoveEmptyEntries);

                        for (var i = 0; i < words.Length; i++)
                        {
                            words[i] = Regex.Escape(words[i]);
                        }

                        var sPattern = string.Join(regTagName, words);
                        regExp = new Regex(sPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        var h1 = h;
                        strToHighlight = regExp.Replace(strToHighlight, new MatchEvaluator(match => InjectHighlight(match, h1)));
                    }
                }

                return strToHighlight;
            }
        }

        /// <summary>
        /// Sets the css Class name on the highlight object
        /// </summary>
        /// <param name="hl"></param>
        public void SetClassName(BizDC.Highlight hl)
        {
            string color = string.IsNullOrEmpty(hl.Color) ? "color-1" : hl.Color;
            string cssClass = "highlight " + color + " user-" + hl.UserId;

            if (!Context.IsAnonymous && hl.UserId == Context.CurrentUser.Id)
                cssClass = cssClass + " mine";

            if (!Context.IsAnonymous && !hl.Notes.IsNullOrEmpty())
                cssClass += " has-notes";

            if (hl.Status == (int)HighlightStatus.Locked)
                cssClass += " locked";

            if (hl.Public)
                cssClass += " public";

            hl.ClassName = cssClass;
        }

        /// <summary>
        /// Wraps substring match returned from regex search with html span tags and css
        /// </summary>
        /// <param name="m"></param>
        /// <param name="hl"></param>
        /// <returns></returns>
        private String InjectHighlight(Match m, BizDC.Highlight hl)
        {
            using (Context.Tracer.StartTrace("NoteActions.InjectHighlight"))
            {
                string color = string.IsNullOrEmpty(hl.Color) ? "color-1" : hl.Color;
                string cssClass = "highlight " + color + " user-" + hl.UserId;

                if (!Context.IsAnonymous && hl.UserId == Context.CurrentUser.Id)
                    cssClass = cssClass + " mine";

                if (!Context.IsAnonymous && !hl.Notes.IsNullOrEmpty())
                    cssClass += " has-notes";

                if (hl.Status == (int)HighlightStatus.Locked)
                    cssClass += " locked";

                if (hl.Public)
                    cssClass += " public";

                string initialMatch = m.Value;

                if (m.Success)
                {
                    const string sPattern = @"(<[^>]*>)?(?<text>[^<>][^<>]+[^<>])(<[^>]*>)?";
                    var matches = Regex.Matches(initialMatch, sPattern, RegexOptions.IgnoreCase);

                    if (matches.Count > 0)
                    {
                        foreach (Match match in matches)
                        {
                            string textFragment = match.Groups["text"].ToString();
                            string highlightFragment = "<span id='highlight-" + hl.HighlightId + "' class='" + cssClass + "'>" + textFragment + "</span>";
                            initialMatch = initialMatch.Replace(textFragment, highlightFragment);
                        }

                        return initialMatch;
                    }

                    return "<span id='highlight-" + hl.HighlightId + "' class='" + cssClass + "'>" + m.Value + "</span>";
                }

                return m.Value;
            }
        }

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
        public void GetHighlightNoteCount(PxHighlightType highlightType, string courseId, string itemId, string reviewId, string userId, string enrollmentId, out int highlightCount, out int noteCount)
        {
            using (Context.Tracer.DoTrace("NoteActions.GetHighlightNoteCount(courseId={0},itemId={1},reviewId={2},userId={3},enrollmentId={4})", courseId, itemId, reviewId, userId, enrollmentId))
            {
                NotesData.GetNoteCount(highlightType, courseId, itemId, reviewId, userId, enrollmentId, out highlightCount, out noteCount);
            }
        }

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
        public int GetNoteCount(PxHighlightType highlightType, string courseId, string itemId, string reviewId, string userId, string enrollmentId)
        {
            using (Context.Tracer.DoTrace("NoteActions.GetNoteCount(courseId={0},itemId={1},reviewId={2},userId={3},enrollmentId={4})", courseId, itemId, reviewId, userId, enrollmentId))
            {
                int highlightCount, noteCount;
                GetHighlightNoteCount(highlightType, courseId, itemId, reviewId, userId, enrollmentId, out highlightCount, out noteCount);
                return noteCount;
            }
        }

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
        public int GetHighlightCount(PxHighlightType highlightType, string courseId, string itemId, string reviewId, string userId, string enrollmentId)
        {
            using (Context.Tracer.DoTrace("NoteActions.GetHighlightCount(courseId={0},itemId={1},reviewId={2},userId={3},enrollmentId={4})", courseId, itemId, reviewId, userId, enrollmentId))
            {
                int highlightCount, noteCount;
                GetHighlightNoteCount(highlightType, courseId, itemId, reviewId, userId, enrollmentId, out highlightCount, out noteCount);
                return highlightCount;
            }
        }

        /// <summary>
        /// Gets all shared notes from the specified student and course.
        /// </summary>
        /// <param name="studentId">The student ID.</param>
        /// <param name="courseId">The course ID.</param>
        /// <returns></returns>
        public IEnumerable<ShareNoteResult> GetAllSharedNotes(string studentId, string courseId)
        {
            return NotesData.GetAllSharedNotes(studentId, courseId);
        }

        /// <summary>
        /// Gets all note sharing relationships for specified user.
        /// </summary>
        /// <param name="studentId">The student ID.</param>
        /// <param name="courseId">The course ID.</param>
        /// <param name="itemId">The item ID.</param>
        /// <param name="reviewId">The peer review ID.</param>
        /// <param name="enrollmentId">The enrollment ID.</param>
        /// <returns></returns>
        public IEnumerable<ShareNoteResult> GetNoteSettings(string studentId, string courseId, string itemId, string reviewId, string enrollmentId)
        {
            if (string.IsNullOrEmpty(itemId))
                itemId = null;

            if (string.IsNullOrEmpty(reviewId))
                reviewId = null;

            if (string.IsNullOrEmpty(enrollmentId))
                enrollmentId = null;

            return NotesData.GetNoteSettings(studentId, courseId, itemId, reviewId, enrollmentId);
        }

        /// <summary>
        /// Stores user information (name, user type,...) into notes databases.
        /// </summary>
        /// <param name="user">The user information to store.</param>
        /// <param name="courseId">The course ID.</param>
        /// <param name="type">The user type.</param>
        public void InitializeUser(UserInfo user, string courseId, Bfw.PX.Biz.DataContracts.UserType type)
        {
            var t = 1;
            if (type == Bfw.PX.Biz.DataContracts.UserType.Instructor)
                t = 0;
            if (!user.Id.IsNullOrEmpty())
            {
                NotesData.InitializeUser(user.Id, user.FirstName, user.LastName, courseId, t);
            }
            
        }

        /// <summary>
        /// Updates existing note sharing relationship settings.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="courseId">The course ID.</param>
        /// <param name="sharerId">The sharer ID.</param>
        /// <param name="highlights">If true share highlights.</param>
        /// <param name="notes">If true share notes.</param>
        public void UpdateNoteSettings(string userId, string courseId, string sharerId, bool? highlights, bool? notes)
        {
            if (userId == sharerId)
            {
                //my notes/highlights
                NotesData.UpdateNoteSettings(userId, courseId, null, null, null, highlights, notes, null, null);
            }
            else if (sharerId == "0")
            {
                //instructor notes/highlights
                NotesData.UpdateNoteSettings(userId, courseId, null, null, null, null, null, highlights, notes);
            }
            else
            {
                //normal case of enabling or disabling a share from a user
                NotesData.UpdateNoteSettings(userId, courseId, sharerId, highlights, notes, null, null, null, null);
            }
        }

        /// <summary>
        /// Gets the notes for the specified user.
        /// </summary>
        /// <param name="highlightType">Type of the highlight.</param>
        /// <param name="userId">The user ID.</param>
        /// <param name="courseId">The course ID.</param>
        /// <param name="enrollmentId">The enrollment ID.</param>
        /// <returns></returns>
        public IEnumerable<Note> GetNotesByUser(PxHighlightType highlightType, string userId, string courseId, string enrollmentId)
        {
            return NotesData.GetNotesByUser(highlightType, userId, courseId, enrollmentId);
        }

        /// <summary>
        /// Gets the notes for a peer review for the specified user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="enrollmentIds">The enrollment IDs.</param>
        /// <returns></returns>
        public IEnumerable<Note> GetNotesForPeerReviewByUser(string userId, string[] enrollmentIds)
        {
            return NotesData.GetNotesForPeerReviewByUser(userId, string.Join(",", enrollmentIds));
        }

        /// <summary>
        /// Shares a note.
        /// </summary>
        /// <param name="noteId">ID of a note to share.</param>
        /// <param name="share">if set to <c>true</c> marks a note as publicly shared.</param>
        public void ShareNote(string noteId, bool share)
        {
            NotesData.ShareNote(noteId, share);
        }

        #endregion
    }
}