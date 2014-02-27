using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bfw.Common.Caching;
using Bfw.Common.Collections;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.PX.PXPub.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TestHelper;
using System;
using Note = Bfw.PX.Biz.DataContracts.Note;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    [TestClass]
    public class HighlightContollerTest
    {
        private IBusinessContext _context;
        private INoteLibraryActions _noteLibraryActions;
        private INoteActions _noteActions;
        private IEnrollmentActions _enrollmentActions;
        private IContentActions _contentActions;
        private IGradeActions _gradeActions;
        private IResponseProxy _responseProxy;
        private IUserActions _userActions;
        private HighlightController _controller;

        /// <summary>
        /// Basic init steps needed for all test methods as part of the arrangement 
        /// to help setup request context to act as helper for each test method
        /// </summary>
        [TestInitialize]
        public void _InitializeControllerContext()
        {
            _context = Substitute.For<IBusinessContext>();
            _noteLibraryActions = Substitute.For<INoteLibraryActions>();
            _noteActions = Substitute.For<INoteActions>();
            _enrollmentActions = Substitute.For<IEnrollmentActions>();
            _contentActions = Substitute.For<IContentActions>();
            _gradeActions = Substitute.For<IGradeActions>();
            _responseProxy = Substitute.For<IResponseProxy>();
            _userActions = Substitute.For<IUserActions>();
            _controller = new HighlightController(_context, _noteLibraryActions, _noteActions,_enrollmentActions, _contentActions, _gradeActions, _responseProxy, _userActions);
            _InitiazlieControllerContext();
        }

        private void _InitiazlieControllerContext()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            var requestContext = Substitute.For<RequestContext>();

            var routeData = new RouteData();
            routeData.Values.Add("controller", "Highlight");
            requestContext.RouteData = routeData;

            _controller.ControllerContext = new ControllerContext()
            {
                Controller =  _controller,
                HttpContext = httpContext,
                RequestContext = requestContext
            };
        }

        private void _WriteViewDataToView(ViewContext view, TextWriter writer)
        {
            if (view != null && !view.ViewData.IsNullOrEmpty())
            {
                foreach (var viewdataMapping in view.ViewData)
                {
                    writer.Write(viewdataMapping + ",");
                }
            }
        }

        /// <summary>
        /// The ActionResult returns from HighlightCollection() should have current user id that passed from view data.
        /// </summary>
        [TestCategory("HighlightController"), TestMethod]
        public void HighlightControllerTest_HighlightCollection_ViewDataShouldHaveCurrentUserId()
        {
            var model = new  DocumentToView { HighlightType = (int)PxHighlightType.GeneralContent};
            _context.Course = new Biz.DataContracts.Course {Id = "testcourse"};
            _context.CurrentUser = new UserInfo {Id = "testUser"};

            _noteActions.GetHighlights(null, null).ReturnsForAnyArgs(new List<Highlight>());

            var view = Substitute.For<IView>();
            view.When(x => x.Render(Arg.Any<ViewContext>(), Arg.Any<TextWriter>()))
                .Do(x => _WriteViewDataToView((ViewContext)x[0], (TextWriter)x[1]));


            var engine = Substitute.For<IViewEngine>();
            var viewEngineResult = new ViewEngineResult(view, engine);
            engine.FindPartialView(null, null, false).ReturnsForAnyArgs(viewEngineResult);
            
            ViewEngines.Engines.Add(engine);
            var jsonResult  = (JsonResult)_controller.HighlightCollection(model);
            Assert.IsTrue(jsonResult.Data.ToString().Contains("notesHtml = [CurrentUserId, testUser],"));
        }

        /// <summary>
        /// If calls AddCommentToTopNote() with a empty top note id, it should return false
        /// </summary>
        [TestCategory("HighlightController"), TestMethod]
        public void HighlightControllerTest_AddCommentToTopNote_EmptyTopNoteId_ExpectReturnFalse()
        {
            _context.Course = new Biz.DataContracts.Course { Id = "testcourse" };
            _context.CurrentUser = new UserInfo { Id = "testUser" };
            var note = new Note {};

            _noteActions.AddNoteToTopNote(null, null, null, null, false).ReturnsForAnyArgs(note);

            var view = Substitute.For<IView>();
            view.When(x => x.Render(Arg.Any<ViewContext>(), Arg.Any<TextWriter>()))
                .Do(x => _WriteViewDataToView((ViewContext)x[0], (TextWriter)x[1]));

            var result = (JsonResult)_controller.AddCommentToTopNote("", "", "", "", "", true, HighlightStatus.Active);
            Assert.IsTrue(result.Data.ToString().Equals("False"));

        }

        /// <summary>
        /// If calls AddCommentToTopNote() with valid info, it should return the view successfully.
        /// </summary>
        [TestCategory("HighlightController"), TestMethod]
        public void HighlightControllerTest_AddCommentToTopNote_ExpectSuccess()
        {
            _context.Course = new Biz.DataContracts.Course { Id = "testcourse" };
            _context.CurrentUser = new UserInfo { Id = "testUser" };
            var note = new Note { };

            _noteActions.AddNoteToTopNote(null, null, null, null, false).ReturnsForAnyArgs(x =>
            {
                note.Text = (string)x[2];
                return note;
            });

            var view = Substitute.For<IView>();
            view.When(x => x.Render(Arg.Any<ViewContext>(), Arg.Any<TextWriter>()))
                .Do(x => _WriteViewDataToView((ViewContext)x[0], (TextWriter)x[1]));

            var result = (ViewResult) _controller.AddCommentToTopNote("123", "", "", "topNoteId", "", true, HighlightStatus.Active);
            var resultModel = (List<Models.Note>) result.Model;
            Assert.AreEqual(resultModel.Count(), 1);
            Assert.AreEqual(resultModel.First().UserId, "testUser");
            Assert.AreEqual(resultModel.First().Text, "123");
        }

        /// <summary>
        /// If calls AddCommentToTopNote() with CommentText == "Enter comment here" or CommentLink == "http://", CommentText should be empty when returns
        /// </summary>
        [TestCategory("HighlightController"), TestMethod]
        public void HighlightControllerTest_AddCommentToTopNote_ExpectEmptyCommentText()
        {
            _context.Course = new Biz.DataContracts.Course { Id = "testcourse" };
            _context.CurrentUser = new UserInfo { Id = "testUser" };
            var note = new Note { };

            _noteActions.AddNoteToTopNote(null, null, null, null, false).ReturnsForAnyArgs(x =>
            {
                note.Text = (string) x[2];
                return note;
            });

            var view = Substitute.For<IView>();
            view.When(x => x.Render(Arg.Any<ViewContext>(), Arg.Any<TextWriter>()))
                .Do(x => _WriteViewDataToView((ViewContext)x[0], (TextWriter)x[1]));

            var result = (ViewResult)_controller.AddCommentToTopNote("Enter comment here", "http://", "", "topNoteId", "", true, HighlightStatus.Active);
            var resultModel = (List<Models.Note>)result.Model;
            Assert.AreEqual(resultModel.Count(), 1);
            Assert.AreEqual(resultModel.First().Text, "");
        }

        /// <summary>
        /// If ToggleLockTopNote() is called to lock note, the note should be locked.
        /// </summary>
        [TestCategory("HighlightController"), TestMethod]
        public void HighlightControllerTest_ToggleLockTopNote_ExpectNoteLocked()
        {
            _context.EntityId.ReturnsForAnyArgs("courseId");
            var isNoteLocked = false;
            _noteActions.When(x => x.UpdateNoteStatus("noteId", HighlightStatus.Locked, "courseId")).Do(x => isNoteLocked = true);
            _controller.ToggleLockTopNote("noteId", true);
            Assert.IsTrue(isNoteLocked);
        }

        /// <summary>
        /// If ToggleLockTopNote() is called to unlock note, the note should be unlocked.
        /// </summary>
        [TestCategory("HighlightController"), TestMethod]
        public void HighlightControllerTest_ToggleLockTopNote_ExpectNoteUnlocked()
        {
            _context.EntityId.ReturnsForAnyArgs("courseId");
            var isNoteUnlocked = false;
            _noteActions.When(x => x.UpdateNoteStatus("noteId", HighlightStatus.Active, "courseId")).Do(x => isNoteUnlocked = true);
            _controller.ToggleLockTopNote("noteId", false);
            Assert.IsTrue(isNoteUnlocked);
        }
    }
}
