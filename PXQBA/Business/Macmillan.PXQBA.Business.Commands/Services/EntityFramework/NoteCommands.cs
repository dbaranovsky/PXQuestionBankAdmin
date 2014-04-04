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
        private readonly QBADummyModelContainer dbContext;

        public NoteCommands(QBADummyModelContainer dbContext)
        {
            this.dbContext = dbContext;
        }

        public IEnumerable<Note> GetQuestionNotes(string questionId)
        {
            return dbContext.Notes.Where(note => note.Question.DlapId == questionId).Select(Mapper.Map<Note>);
        }

        public Note SaveNote(Note note)
        {
            var question = dbContext.Questions.FirstOrDefault(q => q.DlapId == note.QuestionId);
            if (question == null)
            {
                throw new ArgumentException("Unable to find question for note");
            }
            var noteToAdd = Mapper.Map<DataAccess.Data.Note>(note);
            noteToAdd.QuestionId = question.Id;
            dbContext.Notes.AddObject(noteToAdd);
            dbContext.SaveChanges();
            return Mapper.Map<Note>(noteToAdd);
        }

        public void DeleteNote(Note note)
        {
            var noteToDelete = Mapper.Map<DataAccess.Data.Note>(note);
            dbContext.Notes.Attach(noteToDelete);
            dbContext.Notes.DeleteObject(noteToDelete);
            dbContext.SaveChanges();
        }
    }
}