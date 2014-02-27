using System;
using System.Collections.Generic;
using System.Linq;

using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Contracts;
using Bfw.PX.PXPub.Controllers.Widgets;
using Bfw.PX.PXPub.Models;

using BizDC = Bfw.PX.Biz.DataContracts;
using PxM = Bfw.PX.PXPub.Models;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    [TestClass]
    public class ContentTreeWidgetControllerTest
    {
        private LaunchpadTreeWidgetController _controller;
        private IBusinessContext _context;
        private IContentActions _contentActions;
        private IGradeActions _gradeActions;
        private ICourseActions _courseActions;
        private IEnrollmentActions _enrollmentActions;
        private IRubricActions _rubricAction;
        private IRSSFeedActions _rssFeedActions;
        private IPageActions _pageActions;
        private ITreeWidgetHelper _helper;
        private PxM.Widget _emptyTreeWidget;
        private TreeWidgetSettings _settings;

        [TestInitialize]
        public void Setup()
        {
            _context = Substitute.For<IBusinessContext>();
            _contentActions = Substitute.For<IContentActions>();
            _gradeActions = Substitute.For<IGradeActions>();
            _courseActions = Substitute.For<ICourseActions>();
            _enrollmentActions = Substitute.For<IEnrollmentActions>();
            _rubricAction = Substitute.For<IRubricActions>();
            _rssFeedActions = Substitute.For<IRSSFeedActions>();
            _pageActions = Substitute.For<IPageActions>();
            _helper = Substitute.For<ITreeWidgetHelper>();

            _helper.DefaultToc.Returns("defaulttoc");
            _helper.DefaultContainer.Returns("defaultcontainer");
            _helper.DefaultSubContainer.Returns("defaultsubcontainer");

            _context.Course.Returns(new Bfw.PX.Biz.DataContracts.Course()
            {
                Id = "12345",
                IsSandboxCourse = false
            });
            _controller = new LaunchpadTreeWidgetController(_context, _contentActions, _gradeActions, _courseActions,
                _enrollmentActions, _rubricAction, null, _rssFeedActions, null, _pageActions, _helper);

            _settings = new TreeWidgetSettings();
            _emptyTreeWidget = new PxM.Widget()
            {
                Properties = new Dictionary<string, PropertyValue>()
            };

        }

        #region SetTreeWidgetSettings - Default Setting Tests
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithNo_DisableEditingProperty_AllowEditingIsTrue()
        {
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsTrue(_settings.AllowEditing);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithNo_ShowItemsOnlyProperty_ShowOnlyFilterIsNull()
        {
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsNull(_settings.ShowOnlyFilter);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithNo_DisableDragDropProperty_AllowDragDropIsTrue()
        {
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsTrue(_settings.AllowDragDrop);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithNo_ShowCollapsedUnassignedProperty_ShowCollapsedUnassignedIsFalse()
        {
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.ShowCollapsedUnassigned);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithNo_SortByDueDateProperty_SortyByDueDateIsFalse()
        {
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.SortByDueDate);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithNo_CollapseUnassignedProperty_CollapseUnassignedIsFalse()
        {
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.CollapseUnassigned);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithNo_SpliAssignedProperty_SplitAssignedIsFalse()
        {
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.SplitAssigned);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithNo_AllowPastDueProperty_AllowPastDueIsFalse()
        {
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.AllowPastDue);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithNo_DueLaterDaysProperty_DueLaterDaysIs14()
        {
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.AreEqual(14, _settings.DueLaterDays);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithNo_AllowDueLaterProperty_AllowDueLaterIsFalse()
        {
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.AllowDueLater);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithNo_GreyoutPastDueProperty_GreyoutPastDueIsFalse()
        {
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.GreyoutPastDue);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithNo_LaunchpadTitleProperty_LaunchpadTitleIsNull()
        {
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsNull(_settings.Title);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithNo_HideStudentDateDataProperty_ShowStudentDateDataIsTrue()
        {
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsTrue(_settings.ShowStudentDateData);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithNo_ShowDescriptionProperty_ShowDescriptionIsTrue()
        {
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsTrue(_settings.ShowDescription);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithNo_ShowBrowseMoreResourcesProperty_ShowBrowseMoreResourcesIsTrue()
        {
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsTrue(_settings.ShowBrowseMoreResources);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithNo_OpenContentOnClickProperty_OpenContentOnClickIsFalse()
        {
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.OpenContentOnClick);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithNo_FneOnlyLearningCurve_FneOnlyLearningCurveIsFalse()
        {
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.FneOnlyLearningCurve);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithNo_ScrollViewportOnOpen_ScrollViewportOnOpenIsTrue()
        {
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsTrue(_settings.ScrollOnOpen);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithNo_CloseAllOnOpen_CloseAllOnOpenIsTrue()
        {
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsTrue(_settings.CloseAllOnOpen);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithNo_TOC_TOCIssyllabusfilter()
        {
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.AreEqual(_helper.DefaultToc, _settings.TOC);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithNo_Container_ContainerIsLaunchpad()
        {
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.AreEqual(_helper.DefaultContainer, _settings.Container);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithNo_SubContainer_SubContainerIs_PX_MULTIPART_LESSON()
        {
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.AreEqual(_helper.DefaultSubContainer, _settings.SubContainer);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithNo_AssignmentTOC_AssignmentTOCIsEmptyString()
        {
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.AreEqual(string.Empty, _settings.AssignmentTOC);
        }
        #endregion SetTreeWidgetSettings - Default Setting Tests

        #region SetTreeWidgetSettings - Properties Set
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithTrue_DisableEditingProperty_AllowEditingIsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_disableediting", new PropertyValue()
            {
                Value = "true"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.AllowEditing);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithFalse_DisableEditingProperty_AllowEditingIsTrue()
        {
            _emptyTreeWidget.Properties.Add("bfw_disableediting", new PropertyValue()
            {
                Value = "false"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsTrue(_settings.AllowEditing);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithBadValue_DisableEditingProperty_AllowEditingIsTrue()
        {
            _emptyTreeWidget.Properties.Add("bfw_disableediting", new PropertyValue()
            {
                Value = "poop"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsTrue(_settings.AllowEditing);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithValue_ShowItemsOnlyProperty_ShowOnlyFilterIsValue()
        {
            var val = "unassign";
            _emptyTreeWidget.Properties.Add("bfw_showitemsonly", new PropertyValue()
            {
                Value = val
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.AreEqual(val, _settings.ShowOnlyFilter);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithTrue_DisableDragDropProperty_AllowDragDropIsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_disabledraganddrop", new PropertyValue()
            {
                Value = "true"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.AllowDragDrop);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithFalse_DisableDragDropProperty_AllowDragDropIsTrue()
        {
            _emptyTreeWidget.Properties.Add("bfw_disabledraganddrop", new PropertyValue()
            {
                Value = "false"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsTrue(_settings.AllowDragDrop);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithBadValue_DisableDragDropProperty_AllowDragDropIsTrue()
        {
            _emptyTreeWidget.Properties.Add("bfw_disabledraganddrop", new PropertyValue()
            {
                Value = "8"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsTrue(_settings.AllowDragDrop);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithTrue_ShowCollapsedUnassignedProperty_ShowCollapsedUnassignedIsTrue()
        {
            _emptyTreeWidget.Properties.Add("bfw_showcollapseunassigned", new PropertyValue()
            {
                Value = "true"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsTrue(_settings.ShowCollapsedUnassigned);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithFalse_ShowCollapsedUnassignedProperty_ShowCollapsedUnassignedIsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_showcollapseunassigned", new PropertyValue()
            {
                Value = "false"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.ShowCollapsedUnassigned);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithBadValue_ShowCollapsedUnassignedProperty_ShowCollapsedUnassignedIsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_showcollapseunassigned", new PropertyValue()
            {
                Value = "arggggggg"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.ShowCollapsedUnassigned);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithTrue_SortByDueDateProperty_SortyByDueDateIsTrue()
        {
            _emptyTreeWidget.Properties.Add("bfw_sortbyduedate", new PropertyValue()
            {
                Value = "true"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsTrue(_settings.SortByDueDate);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithFalse_SortByDueDateProperty_SortyByDueDateIsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_sortbyduedate", new PropertyValue()
            {
                Value = "false"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.SortByDueDate);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithBadValue_SortByDueDateProperty_SortyByDueDateIsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_sortbyduedate", new PropertyValue()
            {
                Value = "dasjila"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.SortByDueDate);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithTrue_CollapseUnassignedProperty_CollapseUnassignedIsTrue()
        {
            _emptyTreeWidget.Properties.Add("bfw_collapseunassigned", new PropertyValue()
            {
                Value = "true"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsTrue(_settings.CollapseUnassigned);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithFalse_CollapseUnassignedProperty_CollapseUnassignedIsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_collapseunassigned", new PropertyValue()
            {
                Value = "false"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.CollapseUnassigned);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithBadValue_CollapseUnassignedProperty_CollapseUnassignedIsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_collapseunassigned", new PropertyValue()
            {
                Value = "ghreger"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.CollapseUnassigned);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithTrue_SplitAssignedProperty_SplitAssignedIsTrue()
        {
            _emptyTreeWidget.Properties.Add("bfw_splitassigned", new PropertyValue()
            {
                Value = "true"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsTrue(_settings.SplitAssigned);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithFalse_SplitAssignedProperty_SplitAssignedIsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_splitassigned", new PropertyValue()
            {
                Value = "false"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.SplitAssigned);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithBadValue_SplitAssignedProperty_SplitAssignedIsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_splitassigned", new PropertyValue()
            {
                Value = "gsertewer"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.SplitAssigned);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithTrue_AllowPastDueProperty_AllowPastDueIsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_togglepastdue", new PropertyValue()
            {
                Value = "true"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.AllowPastDue);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithTrue_AllowPastDueProperty_AndShowPastDueTrue_AllowPastDueIsTrue()
        {
            _emptyTreeWidget.Properties.Add("bfw_togglepastdue", new PropertyValue()
            {
                Value = "true"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings, true);
            Assert.IsTrue(_settings.AllowPastDue);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithFalse_AllowPastDueProperty_AllowPastDueIsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_togglepastdue", new PropertyValue()
            {
                Value = "false"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings, true);
            Assert.IsFalse(_settings.AllowPastDue);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithBadValue_AllowPastDueProperty_AllowPastDueIsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_togglepastdue", new PropertyValue()
            {
                Value = "edfdsgers"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings, true);
            Assert.IsFalse(_settings.AllowPastDue);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithValid_DueLaterDaysProperty_DueLaterDaysIsValue()
        {
            var val = "78";
            _emptyTreeWidget.Properties.Add("bfw_toggleduelaterdays", new PropertyValue()
            {
                Value = val
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.AreEqual(Convert.ToInt32(val), _settings.DueLaterDays);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithBadValue_DueLaterDaysProperty_DueLaterDaysIs14()
        {
            var val = "pooo";
            _emptyTreeWidget.Properties.Add("bfw_toggleduelaterdays", new PropertyValue()
            {
                Value = val
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.AreEqual(14, _settings.DueLaterDays);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithTrue_AllowDueLaterProperty_AllowDueLaterIsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_toggleduelater", new PropertyValue()
            {
                Value = "true"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.AllowDueLater);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithTrue_AllowDueLaterProperty_AndTrueShowDueLaterToggle_AllowDueLaterIsTrue()
        {
            _emptyTreeWidget.Properties.Add("bfw_toggleduelater", new PropertyValue()
            {
                Value = "true"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings, false, true);
            Assert.IsTrue(_settings.AllowDueLater);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithFalse_AllowDueLaterProperty_AndTrueShowDueLaterToggle_AllowDueLaterIsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_toggleduelater", new PropertyValue()
            {
                Value = "false"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings, false, true);
            Assert.IsFalse(_settings.AllowDueLater);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithBadValue_AllowDueLaterProperty_AllowDueLaterIsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_toggleduelater", new PropertyValue()
            {
                Value = "gdsgds"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings, false, true);
            Assert.IsFalse(_settings.AllowDueLater);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithTrue_GreyoutPastDueProperty_GreyoutPastDueIsTrue()
        {
            _emptyTreeWidget.Properties.Add("bfw_grayoutpastduelater", new PropertyValue()
            {
                Value = "true"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsTrue(_settings.GreyoutPastDue);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithFalse_GreyoutPastDueProperty_GreyoutPastDueIsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_grayoutpastduelater", new PropertyValue()
            {
                Value = "false"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.GreyoutPastDue);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithBadValue_GreyoutPastDueProperty_GreyoutPastDueIsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_grayoutpastduelater", new PropertyValue()
            {
                Value = "dsakldsajkl"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.GreyoutPastDue);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithValue_LaunchpadTitleProperty_LaunchpadTitleIsValue()
        {
            var val = "My Title";
            _emptyTreeWidget.Properties.Add("bfw_launchpadtitle", new PropertyValue()
            {
                Value = val
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.AreEqual(val, _settings.Title);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithTrue_HideStudentDateDataProperty_ShowStudentDateDataIsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_hidedatestudentdata", new PropertyValue()
            {
                Value = "true"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.ShowStudentDateData);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithFalse_HideStudentDateDataProperty_ShowStudentDateDataIsTrue()
        {
            _emptyTreeWidget.Properties.Add("bfw_hidedatestudentdata", new PropertyValue()
            {
                Value = "false"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsTrue(_settings.ShowStudentDateData);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithBadValue_HideStudentDateDataProperty_ShowStudentDateDataIsTrue()
        {
            _emptyTreeWidget.Properties.Add("bfw_hidedatestudentdata", new PropertyValue()
            {
                Value = "gserfe"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsTrue(_settings.ShowStudentDateData);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithTrue_ShowDescriptionProperty_ShowDescriptionIsTrue()
        {
            _emptyTreeWidget.Properties.Add("bfw_showdescription", new PropertyValue()
            {
                Value = "true"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsTrue(_settings.ShowDescription);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithFalse_ShowDescriptionProperty_ShowDescriptionIsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_showdescription", new PropertyValue()
            {
                Value = "false"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.ShowDescription);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithBadValue_ShowDescriptionProperty_ShowDescriptionIsTrue()
        {
            _emptyTreeWidget.Properties.Add("bfw_showdescription", new PropertyValue()
            {
                Value = "dsakldsajkl"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsTrue(_settings.ShowDescription);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithTrue_ShowBrowseMoreResourcesProperty_ShowBrowseMoreResourcesIsTrue()
        {
            _emptyTreeWidget.Properties.Add("bfw_showbrowsemoreresource", new PropertyValue()
            {
                Value = "true"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsTrue(_settings.ShowBrowseMoreResources);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithFalse_ShowBrowseMoreResourcesProperty_ShowBrowseMoreResourcesIsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_showbrowsemoreresource", new PropertyValue()
            {
                Value = "false"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.ShowBrowseMoreResources);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithBadValue_ShowBrowseMoreResourcesProperty_ShowBrowseMoreResourcesIsTrue()
        {
            _emptyTreeWidget.Properties.Add("bfw_showbrowsemoreresource", new PropertyValue()
            {
                Value = "dsakldsajkl"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsTrue(_settings.ShowBrowseMoreResources);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithTrue_OpenContentOnClickProperty_OpenContentOnClickIsTrue()
        {
            _emptyTreeWidget.Properties.Add("bfw_opencontentonclick", new PropertyValue()
            {
                Value = "true"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsTrue(_settings.OpenContentOnClick);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithFalse_OpenContentOnClickProperty_OpenContentOnClickIsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_opencontentonclick", new PropertyValue()
            {
                Value = "false"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.OpenContentOnClick);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithBadValue_OpenContentOnClickProperty_OpenContentOnClickIsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_opencontentonclick", new PropertyValue()
            {
                Value = "dsakldsajkl"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.OpenContentOnClick);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithFalse_ShowExpandIconAtAllLevels_ShowExpandIconAtAllLevelsIsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_showexpandiconatalllevels", new PropertyValue()
            {
                Value = "false"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.ShowExpandIconAtAllLevels);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithBadValue_ShowExpandIconAtAllLevels_ShowExpandIconAtAllLevelsIsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_showexpandiconatalllevels", new PropertyValue()
            {
                Value = "safsdfasdf"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.ShowExpandIconAtAllLevels);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithTrue_FneOnlyLearningCurveProperty_FneOnlyLearningCurveIsTrue()
        {
            _emptyTreeWidget.Properties.Add("bfw_fneonlylearningcurve", new PropertyValue()
            {
                Value = "true"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsTrue(_settings.FneOnlyLearningCurve);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithFalse_FneOnlyLearningCurveProperty_FneOnlyLearningCurveIsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_fneonlylearningcurve", new PropertyValue()
            {
                Value = "false"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.FneOnlyLearningCurve);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithBadValue_FneOnlyLearningCurveProperty_FneOnlyLearningCurveIsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_fneonlylearningcurve", new PropertyValue()
            {
                Value = "dsakldsajkl"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.FneOnlyLearningCurve);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_ShowAssignmentUnitWorkflow_ShowAssignmentUnitWorkflowIsTrue()
        {
            _emptyTreeWidget.Properties.Add("bfw_assignment_unit_flow", new PropertyValue()
            {
                Value = true
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsTrue(_settings.ShowAssignmentUnitWorkflow);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_ShowAssignmentUnitWorkflow_ShowAssignmentUnitWorkflowIsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_assignment_unit_flow", new PropertyValue()
            {
                Value = false
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.ShowAssignmentUnitWorkflow);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithBadValue_ShowAssignmentUnitWorkflow_ShowAssignmentUnitWorkflowIsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_assignment_unit_flow", new PropertyValue()
            {
                Value = "safsdfasdf"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.ShowAssignmentUnitWorkflow);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithValue_TOC_TOCIsValue()
        {
            var testValue = "testfilter";
            _emptyTreeWidget.Properties.Add("bfw_toc", new PropertyValue()
            {
                Value = testValue
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.AreEqual(testValue, _settings.TOC);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithValue_Container_ContainerIsValue()
        {
            var testValue = "container";
            _emptyTreeWidget.Properties.Add("bfw_container_id", new PropertyValue()
            {
                Value = testValue
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.AreEqual(testValue, _settings.Container);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithValue_SubContainer_SubContainerIsValue()
        {
            var testValue = "subcontainer";
            _emptyTreeWidget.Properties.Add("bfw_subcontainer_id", new PropertyValue()
            {
                Value = testValue
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.AreEqual(testValue, _settings.SubContainer);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithValue_AssignmentTOC_AssignmentTOCIsValue()
        {
            var val = "foo";
            _emptyTreeWidget.Properties.Add("bfw_assignmenttoc", new PropertyValue()
            {
                Value = val
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.AreEqual(val, _settings.AssignmentTOC);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithValue_RemoveOnUnassign_RemoveOnUnassignIsTrue()
        {
            _emptyTreeWidget.Properties.Add("bfw_removeOnUnassign", new PropertyValue()
            {
                Value = true
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsTrue(_settings.RemoveOnUnassign);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithValue_RemoveOnUnassign_RemoveOnUnassignIsFalse()
        {
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.RemoveOnUnassign);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithValue_RemovableSetting_Switch_IsTrue()
        {
            _emptyTreeWidget.Properties.Add("bfw_removable_switch", new PropertyValue()
            {
                Value = true
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsTrue(_settings.RemovableSetting.Switch);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithValue_RemovableSetting_Switch_IsFalse()
        {
            _emptyTreeWidget.Properties.Add("bfw_removable_switch", new PropertyValue()
            {
                Value = false
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsFalse(_settings.RemovableSetting.Switch);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithValue_RemovableSetting_Switch_IsTrue_ByDefault()
        {
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsTrue(_settings.RemovableSetting.Switch);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithValue_RemovableSetting_XPathQuery()
        {
            _emptyTreeWidget.Properties.Add("bfw_removable_xpath_query", new PropertyValue()
            {
                Value = "something"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsNotNull(_settings.RemovableSetting.XPathQueryFilter);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithNoValue_RemovableSetting_XPathQuery()
        {
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsNull(_settings.RemovableSetting.XPathQueryFilter);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithValue_RemovableSetting_RemoveFromTocs()
        {
            _emptyTreeWidget.Properties.Add("bfw_remove_from_toc", new PropertyValue()
            {
                Value = "xfilter"
            });
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsNotNull(_settings.RemovableSetting.RemoveFromTocs);
        }
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_SetTreeWidgetSettings_WithNoValue_RemovableSetting_RemoveFromTocs()
        {
            _controller.SetTreeWidgetSettings(_emptyTreeWidget, _settings);
            Assert.IsNull(_settings.RemovableSetting.RemoveFromTocs);
        }

        #endregion SetTreeWidgetSettings - Properties Set

        /// <summary>
        /// If content item is found, then it should return a response with status = Success, item id and title.
        /// </summary>
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_GetGradableItems_ExpectResultHasNoFolder()
        {
            var grades = new List<BizDC.Grade> { new BizDC.Grade { ItemId = "item1" }, new BizDC.Grade { ItemId = "Folder1" } };
            _context.EnrollmentId = "student1";

            _contentActions.GetItems(null, null).ReturnsForAnyArgs(new List<BizDC.ContentItem>
            {
                new BizDC.ContentItem
                {
                    Subtype = "PxUnit"
                }, 
                new BizDC.ContentItem
                {
                    Type = "ExternalContent"
                }
            });
            var items = _controller.GetGradableItems(grades, true);
            Assert.AreEqual(items.Where(i => i.Subtype == "PxUnit").Count(), 0);
        }

        /// <summary>
        /// If content item is found, then it should return a response with status = Success, item id and title.
        /// </summary>
        [TestCategory("ContentTreeWidget"), TestMethod]
        public void ContentTreeWidgetController_GetGradableItems_ExpectResultHasFolder()
        {
            var grades = new List<BizDC.Grade> { new BizDC.Grade { ItemId = "item1" }, new BizDC.Grade { ItemId = "Folder1" } };
            _context.EnrollmentId = "student1";

            _contentActions.GetItems(null, null).ReturnsForAnyArgs(new List<BizDC.ContentItem>
            {
                new BizDC.ContentItem
                {
                    Subtype = "PxUnit"
                }, 
                new BizDC.ContentItem
                {
                    Type = "ExternalContent"
                }
            });
            var items = _controller.GetGradableItems(grades, false);
            Assert.AreEqual(items.Where(i => i.Subtype == "PxUnit").Count(), 1);
        }
    }
}
