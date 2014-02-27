using System.Collections.Generic;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.Common.Logging;


namespace Bfw.PX.Biz.Direct.Services
{
    /// <summary>
    /// Implementation of the INoteLibraryActions interface.
    /// </summary>
    public class NoteLibraryActions : INoteLibraryActions
    {
        /// <summary>
        /// Note library resource file path convention.
        /// </summary>
        private const string URI = "NoteLibrary/{0}.pxres";

        /// <summary>
        /// The IBusinessContext implementation to use.
        /// </summary>
        protected IBusinessContext Context { get; set; }

        /// <summary>
        /// The IContentActions implementation to use.
        /// </summary>
        protected IContentActions ContentActions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoteLibraryActions"/> class.
        /// </summary>
        /// <param name="context">The IBusinessContext implementation.</param>
        /// <param name="contentActions">The IContentActions implementation.</param>
        public NoteLibraryActions(IBusinessContext context, IContentActions contentActions)
        {
            Context = context;
            ContentActions = contentActions;
        }

        /// <summary>
        /// Gets a lists of notes for the specified course.
        /// </summary>
        /// <param name="entityId">ID of the course or section.</param>
        /// <returns></returns>

        public IEnumerable<Note> ListNotes(string entityId)
        {
            using (Context.Tracer.DoTrace("NoteLibraryActions.ListNotes(entityId={0})", entityId))
            {
                var notes = new List<Note>();

                if (!string.IsNullOrEmpty(entityId))
                {
                    var path = string.Format(URI, "*");
                    var contents = ContentActions.ListResources(entityId, path, string.Empty);
                    foreach (var content in contents)
                    {
                        notes.Add(content.ToNote());
                    }
                }
                return notes;
            }
        }

        /// <summary>
        /// Stores a note to the specified course in the system.
        /// </summary>
        /// <param name="entityId">ID of the course or section.</param>
        /// <param name="note">The note to store.</param>
        /// <returns></returns>
        public Note SaveNote(string entityId, Note note)
        {
            using (Context.Tracer.DoTrace("NoteLibraryActions.SaveNote(entityId={0})", entityId))
            {
                List<Bfw.PX.Biz.DataContracts.Resource> resources = new List<Bfw.PX.Biz.DataContracts.Resource>();
                resources.Add(note.ToNote(URI));
                ContentActions.StoreResources(resources);
                return note;
            }
        }

        /// <summary>
        /// Deletes a note in the specified course.
        /// </summary>
        /// <param name="entityId">ID of the course or section.</param>
        /// <param name="noteId">ID of the note to delete.</param>
        public void DeleteNote(string entityId, string noteId)
        {
            using (Context.Tracer.DoTrace("NoteLibraryActions.DeleteNote(entityId={0},noteId={1})", entityId, noteId))
            {
                ContentActions.RemoveResource(entityId, string.Format(URI, noteId));
            }
        }

        /// <summary>
        /// Reorders a note in the specified course.
        /// </summary>
        /// <param name="entityId">ID of the course or section.</param>
        /// <param name="note">The note to reorder.</param>
        /// <param name="prevSequence">The prev sequence.</param>
        /// <param name="nextSequence">The next sequence.</param>
        /// <returns></returns>
        public Note ReorderNote(string entityId, Note note, string prevSequence, string nextSequence)
        {
            using (Context.Tracer.DoTrace("NoteLibraryActions.ReorderNote(entityId={0})", entityId))
            {
                string sequence = Context.Sequence(prevSequence, nextSequence);
                note.Sequence = sequence;
                var newnote = SaveNote(entityId, note);
                return newnote;
            }
        }
    }
}