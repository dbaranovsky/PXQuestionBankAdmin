using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models.Web;

namespace Macmillan.PXQBA.Business.Commands.Contracts
{
    public interface INoteCommands
    {
        IEnumerable<Note> GetQuestionNotes(string questionId);
        Note SaveNote(Note note);
        void DeleteNote(Note note);
    }
}