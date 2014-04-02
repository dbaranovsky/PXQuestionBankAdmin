using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models.Web;

namespace Macmillan.PXQBA.Business.Contracts
{
    public interface INotesManagementService
    {
        IEnumerable<Note> GetQuestionNotes(string questionId);

        void SaveNote(Note note);

        void DeleteNote(Note note);
    }
}