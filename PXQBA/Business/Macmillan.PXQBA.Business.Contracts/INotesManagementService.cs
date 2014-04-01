using Macmillan.PXQBA.Business.Models.Web;

namespace Macmillan.PXQBA.Business.Contracts
{
    public interface INotesManagementService
    {
        void GetQuestionNotes(string questionId);

        void SaveNote(Note note);

        void DeleteNote(Note note);
    }
}