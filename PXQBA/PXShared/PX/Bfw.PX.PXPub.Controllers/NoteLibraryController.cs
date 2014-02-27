using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers
{
    [HandleError]
    [PerfTraceFilter]
    public class NoteLibraryController : Controller
    {
        #region Data Members

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Gets or sets the library actions.
        /// </summary>
        /// <value>
        /// The library actions.
        /// </value>
        protected BizSC.INoteLibraryActions LibraryActions { get; set; }

        /// <summary>
        /// A private member for notes a collection of Notes.
        /// </summary>
        private List<Note> _notes = new List<Note>();
        /// <summary>
        /// Lazy load notes.
        /// </summary>
        protected List<Note> Notes
        {
            get
            {
                if (_notes.IsNullOrEmpty())
                {
                    var biz = LibraryActions.ListNotes(Context.EnrollmentId);
                    if (!biz.IsNullOrEmpty())
                    {
                        _notes = biz.Map(b => b.ToNote(Context)).OrderBy(n => n.Sequence).ToList();
                    }
                }
                else
                {
                    _notes = _notes.OrderBy(n => n.Sequence).ToList();
                }

                return _notes;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="NoteLibraryController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="libraryActions">The library actions.</param>
        public NoteLibraryController(BizSC.IBusinessContext context, BizSC.INoteLibraryActions libraryActions)
        {
            Context = context;
            LibraryActions = libraryActions;
        }

        #region Action Methods

        /// <summary>
        /// Return the Index view for this Controller.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (!Context.CourseIsProductCourse)
            {
                return View(Notes);
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// Saves the note.
        /// </summary>
        /// <param name="newNote">The new note.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveNote(Note newNote)
        {
            newNote.Title = HttpUtility.HtmlEncode(HttpUtility.HtmlEncode(newNote.Title));
            newNote.Text = HttpUtility.HtmlEncode(HttpUtility.HtmlEncode(newNote.Text));
            StartCreationOfNote(newNote);
            ViewData.Model = Notes;
            return View("NoteList");
        }

        /// <summary>
        /// Reorders the notes.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="minSequence">The min sequence.</param>
        /// <param name="maxSequence">The max sequence.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ReorderNotes(string id, string minSequence, string maxSequence)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var note = (from c in Notes where c.NoteId == id select c).FirstOrDefault();
                var updatedNote = LibraryActions.ReorderNote(id, note.ToNote(), minSequence, maxSequence);

                Notes.Remove(note);
                Notes.Add(updatedNote.ToNote(Context));
            }

            ViewData.Model = Notes;
            return View("NoteList");
        }

        /// <summary>
        /// Creates a note and returns the Index view.
        /// </summary>
        /// <param name="newNote">The new note.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateNote(Note newNote)
        {
            if (!Context.CourseIsProductCourse)
            {
                if (!string.IsNullOrEmpty(newNote.Title)) newNote.Title.Replace("&nbsp;", " ");
                if (!string.IsNullOrEmpty(newNote.Text)) newNote.Text.Replace("&nbsp;", " ");
                StartCreationOfNote(newNote);
                return View("Index", Notes);
            }
            else
            {
                return View("Index", null);
            }
        }

        /// <summary>
        /// Deletes the note.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public ActionResult DeleteNote(string id)
        {
            if (id != null)
            {
                LibraryActions.DeleteNote(Context.EnrollmentId, id);
            }

            ViewData.Model = Notes;
            return View("NoteList");
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets the sequence for new note.
        /// </summary>
        /// <returns></returns>
        private string GetSequenceForNewNote()
        {
            var firstNote = Notes.FirstOrDefault();
            var sequence = string.Empty;

            if (firstNote != null)
            {
                sequence = Context.Sequence(string.Empty, firstNote.Sequence);
            }
            else
            {
                sequence = Context.Sequence(string.Empty, string.Empty);
            }

            return sequence;
        }

        /// <summary>
        /// Create_s the note.
        /// </summary>
        /// <param name="newNote">The new note.</param>
        /// <returns></returns>
        private Note Create_Note(Note newNote)
        {
            bool isNewNote = (string.IsNullOrEmpty(newNote.Sequence)) ? true : false;
            newNote.Sequence = isNewNote ? GetSequenceForNewNote() : newNote.Sequence;
            newNote.NoteId = newNote.NoteId ?? Context.NewItemId();

            if (isNewNote)
            {
                newNote.EntityId = Context.EnrollmentId;
            }

            var note = LibraryActions.SaveNote(Context.EnrollmentId, newNote.ToNote());
            note.Title = HttpUtility.HtmlDecode(HttpUtility.HtmlDecode(note.Title));
            note.Text = HttpUtility.HtmlDecode(HttpUtility.HtmlDecode(note.Text));
            return note.ToNote(Context);
        }

        /// <summary>
        /// Starts the creation of note.
        /// </summary>
        /// <param name="newNote">The new note.</param>
        private void StartCreationOfNote(Note newNote)
        {
            bool isNewNote = (string.IsNullOrEmpty(newNote.Sequence));
            var note = Create_Note(newNote);

            if (isNewNote && !(_notes.IsNullOrEmpty()))
            {
                Notes.Insert(0, note);
            }
        }


        #endregion
    }
}
