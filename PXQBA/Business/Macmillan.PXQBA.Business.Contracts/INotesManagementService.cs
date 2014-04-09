using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models.Web;

namespace Macmillan.PXQBA.Business.Contracts
{
    public interface INotesManagementService
    {
        IEnumerable<Note> GetQuestionNotes(string questionId);

        Note CreateNote(Note note);

        void DeleteNote(Note note);
        void SaveNote(Note note);
    }
}