using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.DataAccess.Data;
using Note = Macmillan.PXQBA.Business.Models.Web.Note;

namespace Macmillan.PXQBA.Business.Commands.Services.EntityFramework
{
    public class NoteCommands : INoteCommands
    {
        private readonly IQBAUow qbaUow;

        public NoteCommands(IQBAUow qbaUow)
        {
            this.qbaUow = qbaUow;
        }

        public IEnumerable<Note> GetQuestionNotes(string questionId)
        {
           
            //return qbaUow.DbContext.Notes.Where(note => note.QuestionId == int.Parse(questionId)).Select(Mapper.Map<Note>);
            //Mock beacuse of code failing
            List<Note> notes  = new List<Note>
                                {
                                    new Note()
                                    {
                                       Id = 15,
                                       IsFlagged = false,
                                       Text = "Lorem Ipsum ",
                                       QuestionId = 1
                                    },
                                     new Note()
                                    {
                                       Id = 16,
                                       IsFlagged = false,
                                       Text = "Lorem Ipsum Odor",
                                       QuestionId = 1
                                    },
                                };
            return notes;
        }

        public void SaveNote(Note note)
        {
            qbaUow.DbContext.Notes.Add(Mapper.Map<DataAccess.Data.Note>(note));
            qbaUow.Commit();
        }

        public void DeleteNote(Note note)
        {
            qbaUow.DbContext.Notes.Remove(Mapper.Map<DataAccess.Data.Note>(note));
            qbaUow.Commit();
        }
    }
}