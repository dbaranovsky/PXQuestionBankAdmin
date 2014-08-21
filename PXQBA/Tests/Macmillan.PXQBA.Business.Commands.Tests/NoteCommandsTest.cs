using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Bfw.Common.Database;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Commands.Services.SQLOperations;
using Macmillan.PXQBA.Business.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Macmillan.PXQBA.Business.Commands.Tests
{
    [TestClass]
    public class NoteCommandsTest
    {
        private INoteCommands noteCommands;
        private IDatabaseManager databaseManager;
      
        [TestInitialize]
        public void TestInitialize()
        {
            databaseManager = Substitute.For<IDatabaseManager>();

            noteCommands = new NoteCommands(databaseManager);
        }

        [TestMethod]
        public void GetQuestionNotes_AnyQuestionId_ReturnValidNotes()
        {
            string questionId = "questionId";
            string noteText = "NoteText";
            var databaseRecord = new DatabaseRecord();
            databaseRecord["Id"] = (long)1;
            databaseRecord["QuestionId"] = questionId;
            databaseRecord["NoteText"] = noteText;


            databaseManager.Query(Arg.Any<DbCommand>()).Returns(new List<DatabaseRecord>() { databaseRecord, databaseRecord });
            var notes = noteCommands.GetQuestionNotes(questionId);

            Assert.AreEqual(notes.Count(), 2);
            Assert.IsTrue(notes.ToArray()[0].Id==1);
            Assert.IsTrue(notes.ToArray()[0].QuestionId == questionId);
            Assert.IsTrue(notes.ToArray()[0].Text == noteText);
        }

        [TestMethod]
        public void DeleteNote_AnyNote_ExecuteNonQueryInvoked()
        {
            var invokeCount = 0;
            var note = new Note()
            {
                Id = 1
            };
            databaseManager.When(m => m.ExecuteNonQuery(Arg.Do<DbCommand>(x =>
            {
                Assert.IsTrue(x.CommandText == "dbo.DeleteQBANote");
                Assert.IsTrue(x.Parameters["@noteId"].Value.ToString() == note.Id.ToString());
            }))).Do(x => invokeCount++);

            noteCommands.DeleteNote(note);

            Assert.AreEqual(invokeCount, 1);
        }


        [TestMethod]
        public void GetQuestionNotes_AnyNote_ExecuteNonQueryInvoked()
        {
            var invokeCount = 0;
            databaseManager.When(m => m.ExecuteNonQuery(Arg.Any<DbCommand>())).Do(x => invokeCount++);

            noteCommands.CreateNote(new Note());

            Assert.AreEqual(invokeCount, 1);
        }

         [TestMethod]
        public void UpdateNote_AnyNote_ExecuteNonQueryInvoked()
        {
            var invokeCount = 0;
             var note = new Note()
                        {
                            Id = 1
                        };
            databaseManager.When(m => m.ExecuteNonQuery(Arg.Do<DbCommand>(x =>
                                                                          {
                                                                              Assert.IsTrue(x.CommandText == "dbo.UpdateQBANote");
                                                                              Assert.IsTrue(x.Parameters["@noteId"].Value.ToString() == note.Id.ToString());
                                                                          }))).Do(x => invokeCount++);

            noteCommands.UpdateNote(note);
            Assert.AreEqual(invokeCount, 1);
        }

    }
}
