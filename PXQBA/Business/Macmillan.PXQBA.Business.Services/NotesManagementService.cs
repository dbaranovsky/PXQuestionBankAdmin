using System.Collections.Generic;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models.Web;

namespace Macmillan.PXQBA.Business.Services
{
    /// <summary>
    /// Service to manage question notes
    /// </summary>
    public class NotesManagementService : INotesManagementService
    {
         private readonly INoteCommands noteCommands;
        
        public NotesManagementService(INoteCommands noteCommands)
        {
            this.noteCommands = noteCommands;
        }
        public IEnumerable<Note> GetQuestionNotes(string questionId)
        {
            return noteCommands.GetQuestionNotes(questionId);
        }

        public Note SaveNote(Note note)
        {
           return noteCommands.SaveNote(note);
        }

        public void DeleteNote(Note note)
        {
            noteCommands.DeleteNote(note);
        }
    }
}