using System;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Widgets;
using Bfw.PX.PXPub.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Linq;
using Bfw.PX.PXPub.Controllers.Contracts;
using System.Web.Mvc;

namespace Bfw.PX.PXPub.Controllers.Tests.Widgets
{
    [TestClass]
    public class ContentWidgetControllerTest
    {
        private IBusinessContext _context;
        private INavigationActions _navigationActions;
        private IContentActions _contentActions;
        private IAssignmentActions _assignmentActions;        
        private IRubricActions _rubricActions;
        private IGradeActions _gradeActions;
        private IUserActions _userActions;
        private ICourseActions _courseActions;
        private ISharedCourseActions _sharedCourseActions;

        private IContentHelper _contentHelper;
        private IAssignmentCenterHelper _assignmentCenterHelper;

        private ContentWidgetController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            _context = Substitute.For<IBusinessContext>();
            _navigationActions = Substitute.For<INavigationActions>();
            _contentActions = Substitute.For<IContentActions>();
            _assignmentActions = Substitute.For<IAssignmentActions>();           
            _rubricActions = Substitute.For<IRubricActions>();
            _gradeActions = Substitute.For<IGradeActions>();
            _courseActions = Substitute.For<ICourseActions>();
            _sharedCourseActions = Substitute.For<ISharedCourseActions>();
            _userActions = Substitute.For<IUserActions>();
            _rubricActions = Substitute.For<IRubricActions>();

            _contentHelper = Substitute.For<IContentHelper>();
            _assignmentCenterHelper = Substitute.For<IAssignmentCenterHelper>();

            _context.Course = new Biz.DataContracts.Course();

            _controller = new ContentWidgetController(_context, _navigationActions, _contentActions, _assignmentActions,
                 _courseActions, _contentHelper, _assignmentCenterHelper, _gradeActions, _rubricActions,
                _sharedCourseActions, _userActions);
        }

        /// <summary>
        /// If item has "meta-content-type", it should be passed to AssignedItem.ContentType
        /// </summary>
        [TestCategory("ContentWidgetController"), TestMethod]
        public void AssignTab_WhenItemHasMetaContentType_ShouldPassToAssignItem()
        {
            _assignmentCenterHelper.FindFilter(null, true, false).ReturnsForAnyArgs(new AssignmentCenterFilterSection());
            _context.EntityId.Returns("testEntityId");
            var dcItem = new Biz.DataContracts.ContentItem { Type = "folder" };
            dcItem.FacetMetadata.Add("meta-content-type", "testContentType");
            _contentActions.GetContent("testEntityId", "testItem").Returns(dcItem);
            var contentItem = new ContentItem{ Id = "testItem"};
            var contentView = new ContentView{Content = contentItem};

            ViewResult  result = (ViewResult)_controller.AssignTab(contentView, false, "");
            var assignedItem = ((ContentView) result.ViewData.Model).AssignedItem;
            Assert.IsTrue(assignedItem.ContentType == "testContentType");

        }

        /// <summary>
        /// If item does not have"meta-content-type", AssignedItem.ContentType should be null
        /// </summary>
        [TestCategory("ContentWidgetController"), TestMethod]
        public void AssignTab_WhenItemNotContainMetaContentType_ShouldBeNull()
        {
            _assignmentCenterHelper.FindFilter(null, true, false).ReturnsForAnyArgs(new AssignmentCenterFilterSection());
            _context.EntityId.Returns("testEntityId");
            var dcItem = new Biz.DataContracts.ContentItem { Type = "folder" };
            _contentActions.GetContent("testEntityId", "testItem").Returns(dcItem);
            var contentItem = new ContentItem { Id = "testItem" };
            var contentView = new ContentView { Content = contentItem };

            ViewResult result = (ViewResult)_controller.AssignTab(contentView, false, "");
            var assignedItem = ((ContentView)result.ViewData.Model).AssignedItem;
            Assert.IsTrue(assignedItem.ContentType == null);

        }

    }
}
