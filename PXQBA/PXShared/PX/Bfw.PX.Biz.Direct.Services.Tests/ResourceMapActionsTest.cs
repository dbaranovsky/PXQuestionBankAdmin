using System;
using NSubstitute;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.DataContracts;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bfw.PX.Biz.Direct.Services.Tests
{
    [TestClass]
    public class ResourceMapActionsTest
    {
        private IBusinessContext context;
        private IContentActions contentActions;
        private ResourceMapActions actions;
        private string entityId = "123278";
        private string enrollmentId = "123280";

        [TestInitialize]
        public void TestInitialize()
        {
            context = Substitute.For<IBusinessContext>();
            context.Course.Returns(new Course { CourseType = CourseType.PersonalEportfolioDashboard.ToString() });
            context.EntityId.Returns(entityId);
            context.EnrollmentId.Returns(enrollmentId);
            contentActions = Substitute.For<IContentActions>();
            actions = new ResourceMapActions(context, contentActions);
        }

        [TestMethod]
        public void CanAddResourceMap_ForPresentationCourse()
        {
            context.Course.Returns(new Course { CourseType = CourseType.PersonalEportfolioDashboard.ToString() });
            var actionsResourceMapPath = String.Format("Templates/Data/XmlResources/Settings/ResourceMaps/{0}/{1}.pxres", entityId, enrollmentId);
            XmlResource resource = new XmlResource { EntityId = enrollmentId, ExtendedId = "6e48980e7de84e8a92c9556786a00627", Url = "Templates/Data/XmlResources/Documents/Assignments/6e48980e7de84e8a92c9556786a00627.pxres" };
            string testXml = "<ResourceMaps><Map Id=\"9d0d9be4207d485d9c5d5784bc733351\" ItemId=\"3e32dd734a744f79b3861313a1ea7e38\" MapType=\"\" Desc=\"Assignment\"><AssociatedItems><AssociatedItem Id=\"b1bb185e69d14c1d88baab0bd8336a52\" /></AssociatedItems></Map><Map Id=\"5287904074da4e10b7219e19a11d0cf7\" ItemId=\"02f49d952663489ba118358da6a68ef4\" MapType=\"Map\" Desc=\"Assignment\"><AssociatedItems><AssociatedItem Id=\"6e48980e7de84e8a92c9556786a00627\" /></AssociatedItems></Map></ResourceMaps>";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] testBytes = encoding.GetBytes(testXml);
            var contentStream = new MemoryStream(testBytes);
            var xmlResource = new XmlResource();
            var resourceStream = xmlResource.GetStream();
            contentStream.CopyTo(resourceStream);
            contentActions.ListResources(enrollmentId, actionsResourceMapPath, "").Returns(new List<Resource> { xmlResource });
            actions.AddResourceMap(resource, "02f49d952663489ba118358da6a68ef4", "Assignment", entityId);
            //Assert
            contentActions.Received().StoreResources(Arg.Is<IEnumerable<Resource>>(l => l.First().EntityId == enrollmentId && l.First().Url == actionsResourceMapPath));
        }
    }
}
