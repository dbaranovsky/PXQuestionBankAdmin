using System;
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
            return qbaUow.DbContext.Notes.Where(note => note.Question.DlapId == questionId).Select(Mapper.Map<Note>);
        }

        public Note SaveNote(Note note)
        {
            var question = qbaUow.DbContext.Questions.FirstOrDefault(q => q.DlapId == note.QuestionId);
            if (question == null)
            {
                throw new ArgumentException("Unable to find question for note");
            }
            var noteToAdd = Mapper.Map<DataAccess.Data.Note>(note);
            noteToAdd.QuestionId = question.Id;
            qbaUow.DbContext.Notes.Add(noteToAdd);
            qbaUow.Commit();
            return Mapper.Map<Note>(noteToAdd);
        }

        public void DeleteNote(Note note)
        {
            var noteToDelete = Mapper.Map<DataAccess.Data.Note>(note);
            qbaUow.DbContext.Notes.Attach(noteToDelete);
            qbaUow.DbContext.Notes.Remove(noteToDelete);
            qbaUow.Commit();
        }
    }
}