using System;
using Bfw.PX.Notes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.Agilix.Dlap.Session;
using Bfw.PX.Biz.DataContracts;


namespace Bfw.PX.Biz.Direct.Services.Tests
{
   

    [TestClass]
    public class NoteActionsTest
    {
        private NoteActions _actions;

        private IBusinessContext _context;

        private ISessionManager _sessionManager;
        private INotesDataContext _noteDataContext;
        [TestInitialize]
        public void TestInitialize()
        {
            _context = Substitute.For<IBusinessContext>();
            _context.CurrentUser = new UserInfo {Id="userid", FirstName ="test", LastName = "testUser"};
            _context.EnrollmentId = "enrollmentId";
            _context.CourseId = "courseId";
            _sessionManager = Substitute.For<ISessionManager>();
            _noteDataContext = Substitute.For<INotesDataContext>();
            _actions = new NoteActions(_context, _sessionManager) {NotesData = _noteDataContext};
        }


        /// <summary>
        /// If adding a note to topnote without top note id, FormatException should be thrown
        /// </summary>
        [TestCategory("NoteActions"), TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void NoteActionTests_AddNoteToTopNoteWithoutTopNoteId_ExpectInvalidGuidException()
        {
           _actions.AddNoteToTopNote("", "testItemId", "text", "description", true);
        }

        /// <summary>
        /// If a note is added to the top note, it should return a note id
        /// </summary>
         [TestCategory("NoteActions"), TestMethod]
        public void NoteActionTests_AddNoteToTopNote_ExpectNoteId()
         {

             _actions.AddNoteToTopNote("11111111-1111-1111-1111-111111111111", "testItemId", "text", "description", true);
             _noteDataContext.Received().AddNoteToTopNote(Arg.Any<DataContracts.Note>());

        }
    }
}
