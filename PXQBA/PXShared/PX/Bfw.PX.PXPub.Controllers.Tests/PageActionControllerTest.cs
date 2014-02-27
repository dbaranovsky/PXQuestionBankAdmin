using System;
using NSubstitute;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Contracts;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    [TestClass]
    public class PageActionControllerTest
    {
        private IPageActions pageActions;
        private IContentHelper helper;
        private IBusinessContext context;
        private INavigationActions navigationActions;
        private ICourseMaterialsActions courseMaterialsActions;
        private IContentActions contentActions;
        private PageActionController controller;

        [TestInitialize]
        public void Initialize()
        {
            pageActions = Substitute.For<IPageActions>();
            helper = Substitute.For<IContentHelper>();
            context = Substitute.For<IBusinessContext>();
            navigationActions = Substitute.For<INavigationActions>();
            courseMaterialsActions = Substitute.For<ICourseMaterialsActions>();
            contentActions = Substitute.For<IContentActions>();
            controller = new PageActionController(pageActions, helper, context, navigationActions, courseMaterialsActions, contentActions);
        }

        [TestMethod]
        public void AddWidget_InvalidateCacheForBranchedCourses()
        {
            var courseId = "156470";
            var pageName = "PX_HOME_FACEPLATE_START";
            var zoneId = "PX_HOME_FACEPLATE_START_ZONE_LEFT";
            var templateId = "PX_AssignmentWidget";
            var widgetId = "NotKnownYet";
            context.Course.Returns(new Course { Id = courseId });
            pageActions.GetWidgetTemplate(templateId).Returns(new Widget());
            pageActions.AddWidget(pageName, zoneId, templateId, "", "", widgetId).Returns(new Widget());
            controller.AddWidget(pageName, zoneId, templateId, "", "", widgetId);
            //Assert
            helper.Received().InvalidateCachedPageDefinitionsForDerivedCourses(courseId, pageName);
        }

        [TestMethod]
        public void RemoveWidget_InvalidateCacheForBranchedCourses()
        {
            var courseId = "156470";
            var widgetId = "adh9087hfid89upfd333";
            var pageName = "PX_HOME_FACEPLATE_START";
            context.Course.Returns(new Course { Id = courseId });
            controller.RemoveWidget(widgetId, pageName);
            //Assert
            pageActions.Received().RemoveWidget(widgetId, pageName);
            helper.Received().InvalidateCachedPageDefinitionsForDerivedCourses(courseId, pageName);
        }

        [TestMethod]
        public void MoveWidget_InvalidateCacheForBranchedCourses()
        {
            var courseId = "156470";
            var pageName = "PX_HOME_FACEPLATE_START";
            var zoneName = "PX_HOME_FACEPLATE_START_ZONE_LEFT";
            var widgetId = "49ioj309wefij23409uw";
            var minSequence = "g";
            var maxSequence = "h";
            context.Course.Returns(new Course { Id = courseId });
            pageActions.MoveWidget(pageName, zoneName, widgetId, minSequence, maxSequence).Returns("ga");
            controller.MoveWidget(pageName, zoneName, widgetId, minSequence, maxSequence);
            //Assert
            helper.Received().InvalidateCachedPageDefinitionsForDerivedCourses(courseId, pageName);
        }
    }
}
 