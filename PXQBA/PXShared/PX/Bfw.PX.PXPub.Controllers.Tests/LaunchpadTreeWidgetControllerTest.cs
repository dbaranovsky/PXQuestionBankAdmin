using System;
using System.Collections.Generic;
using System.Linq;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Controllers.Contracts;
using Bfw.PX.PXPub.Controllers.Widgets;
using Bfw.PX.PXPub.Models;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.PX.Biz.ServiceContracts;
using BizDC = Bfw.PX.Biz.DataContracts;
using System.Web.Mvc;
using NSubstitute;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    /// <summary>
    ///This is a test class for WritingTabControllerTest and is intended
    ///to contain all WritingTabControllerTest Unit Tests
    ///</summary>
    [TestClass]
    public class LaunchpadTreeWidgetControllerTest
    {
        private IBusinessContext _context;
        private IServiceLocator _serviceLocator;
        private IContentActions _contentActions;
        private IGradeActions _gradeActions;
        private IPageActions _pageActions;

        private ITreeWidgetHelper _treeWidgetHelper;

        private LaunchpadTreeWidgetController _controller;

        private readonly string _entityId = "137920";
        private readonly string _enrollmentId = "enrollmentId";
        private readonly string _productCourseId = "productCourseId";

        [TestInitialize()]
        public void MyTestInitialize()
        {
            _context = Substitute.For<IBusinessContext>();
            _contentActions = Substitute.For<IContentActions>();
            _gradeActions = Substitute.For<IGradeActions>();
            _treeWidgetHelper = Substitute.For<ITreeWidgetHelper>();
            _serviceLocator = Substitute.For<IServiceLocator>();
            _pageActions = Substitute.For<IPageActions>();

            var locator = _serviceLocator.GetInstance<IBusinessContext>();
            locator.AccessLevel = AccessLevel.Student;
            locator.EntityId.Returns("137920");
            locator.EnrollmentId.Returns("137920");
            ServiceLocator.SetLocatorProvider(() => _serviceLocator);
            
            _context.AccessLevel = AccessLevel.Student;
            _context.EntityId.Returns(_entityId);
            _context.ProductCourseId.Returns(_productCourseId);
            _context.EnrollmentId.Returns(_enrollmentId);
            _context.Course = new BizDC.Course()
            {
                CourseType = "LMS"
            };

            _controller = new LaunchpadTreeWidgetController(_context, _contentActions, _gradeActions, 
                null, null, null, null, null, null, _pageActions, _treeWidgetHelper);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void Test_LoadItems_Top_Level_Item_for_Student()
        {
            var itemId = "92b17e8ca0b84a9b9232f197cacf07db";
            var widgetId = "PX_LAUNCHPAD_ASSIGNED_WIDGET";
            var toc = "syllabusfilter";
            var subcontainer = "PX_MULTIPART_LESSONS";

            var bizContentItem = new Biz.DataContracts.ContentItem()
            {
                CourseId = _entityId,
                Containers = new List<Bfw.PX.Biz.DataContracts.Container>() {
                    new Bfw.PX.Biz.DataContracts.Container(toc, "Launchpad")
                },
                Id = itemId,
                SubContainerIds = new List<BizDC.Container>()
                {
                    new BizDC.Container(toc, subcontainer)
                },
                Type = "dropbox"
            };

            _contentActions.GetContent(_enrollmentId, itemId).Returns(bizContentItem);
            _contentActions.GetContent(_entityId, subcontainer).Returns(bizContentItem);
            _treeWidgetHelper.GetAssignedContent(null, string.Empty, string.Empty, string.Empty, string.Empty)
                .ReturnsForAnyArgs(new List<BizDC.ContentItem>());
            
            var result = _controller.LoadItem(itemId, widgetId) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Model);
            _contentActions.Received(1).GetContent(_enrollmentId, itemId);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        [ExpectedException(typeof(Exception), "The contentId value cannot be null or empty!")]
        public void EditContentView_Throws_Exception_If_contentId_Is_Missing()
        {
            var result = _controller.EditContentView(string.Empty, string.Empty, string.Empty, string.Empty, null);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        [ExpectedException(typeof(Exception), "The mode value cannot be null or empty!")]
        public void EditContentView_Throws_Exception_If_mode_Is_Missing()
        {
            var result = _controller.EditContentView("1", string.Empty, string.Empty, string.Empty, string.Empty);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        [ExpectedException(typeof(Exception), "The mode value should be rename or copy!")]
        public void EditContentView_Throws_Exception_If_mode_Is_Invalid()
        {
            var result = _controller.EditContentView("1", string.Empty, string.Empty, string.Empty, "invalid");
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void EditContentView_Accepts_mode_copy_Value()
        {
            var result = _controller.EditContentView("1", string.Empty, string.Empty, string.Empty, "copy");

            Assert.AreEqual("copy", (result as ViewResult).ViewData["mode"]);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void EditContentView_Accepts_mode_rename_Value()
        {
            var result = _controller.EditContentView("1", string.Empty, string.Empty, string.Empty, "rename");

            Assert.AreEqual("rename", (result as ViewResult).ViewData["mode"]);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void GetTreeRoot_WithFalseProductCourse_SetsProductCourseToFalse()
        {
            _context.AccessLevel = AccessLevel.Instructor;
            _treeWidgetHelper.GetContainersItems(_contentActions, _context, null, "", "").ReturnsForAnyArgs(new List<TreeWidgetViewItem>());
            BizDC.Widget widget = new BizDC.Widget()
            {
                Properties = new Dictionary<string, PropertyValue>()
            };
            widget.Properties.Add("bfw_use_product_course", new PropertyValue() { Type = PropertyType.Boolean, Value = false });
            _pageActions.GetWidget("").ReturnsForAnyArgs(widget);

            var result = _controller.Index(new Models.Widget()) as ViewResult;
            Assert.IsTrue(result.Model is TreeWidgetRoot);
            Assert.AreEqual(false, ((TreeWidgetRoot)result.Model).Settings.UseProductCourse);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void GetTreeRoot_WithTrueProductCourse_SetsProductCourseToFalse()
        {
            _context.AccessLevel = AccessLevel.Instructor;
            _treeWidgetHelper.GetContainersItems(_contentActions, _context, null, "", "").ReturnsForAnyArgs(new List<TreeWidgetViewItem>());
            BizDC.Widget widget = new BizDC.Widget()
            {
                Properties = new Dictionary<string, PropertyValue>()
            };
            widget.Properties.Add("bfw_use_product_course", new PropertyValue() { Type = PropertyType.Boolean, Value = true });
            _pageActions.GetWidget("").ReturnsForAnyArgs(widget);

            var result = _controller.Index(new Models.Widget()) as ViewResult;
            Assert.IsTrue(result.Model is TreeWidgetRoot);
            Assert.AreEqual(true, ((TreeWidgetRoot)result.Model).Settings.UseProductCourse);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void LoadItemsChildren_WhenNotUsingProductCourse_GetsParentUsingEntityId()
        {
            var itemid = "itemId";
            _context.AccessLevel = AccessLevel.Instructor;
            BizDC.Widget widget = new BizDC.Widget()
            {
                Properties = new Dictionary<string, PropertyValue>()
            };
            widget.Properties.Add("bfw_use_product_course", new PropertyValue() { Type = PropertyType.Boolean, Value = false });
            _pageActions.GetWidget("widgetId").ReturnsForAnyArgs(widget);
            _contentActions.GetContent(_context.EntityId, itemid).Returns(new BizDC.ContentItem()
            {
                Type = "dropbox"
            });

            _controller.LoadChildrenForParentItem(itemid, "2", "load", false, "widgetId");

           _contentActions.Received(1).GetContent(_context.EntityId, itemid);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void LoadItemsChildren_WhenUsingProductCourse_GetsParentUsingProductCourseId()
        {
            var itemid = "itemId";
            _context.AccessLevel = AccessLevel.Instructor;
            BizDC.Widget widget = new BizDC.Widget()
            {
                Properties = new Dictionary<string, PropertyValue>()
            };
            widget.Properties.Add("bfw_use_product_course", new PropertyValue() { Type = PropertyType.Boolean, Value = true });
            _pageActions.GetWidget("").ReturnsForAnyArgs(widget);
            _contentActions.GetContent(_context.ProductCourseId, itemid).Returns(new BizDC.ContentItem()
            {
                Type = "dropbox"
            });

            _controller.LoadChildrenForParentItem(itemid, "2", "load", false, "11");

            _contentActions.Received(1).GetContent(_context.ProductCourseId, itemid);
        }
    }
}
