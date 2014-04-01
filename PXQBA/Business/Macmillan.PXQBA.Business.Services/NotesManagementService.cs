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
        public void GetQuestionNotes(string questionId)
        {
            noteCommands.GetQuestionNotes(questionId);
        }

        public void SaveNote(Note note)
        {
            noteCommands.SaveNote(note);
        }

        public void DeleteNote(Note note)
        {
            noteCommands.DeleteNote(note);
        }
    }
}