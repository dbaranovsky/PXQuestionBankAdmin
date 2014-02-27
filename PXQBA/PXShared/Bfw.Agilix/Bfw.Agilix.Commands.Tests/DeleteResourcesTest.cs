namespace Bfw.Agilix.Commands.Tests
{
    using System.Collections.Generic;
    using System.Xml.Linq;
    using Bfw.Agilix.DataContracts;
    using Bfw.Agilix.Dlap;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    // ReSharper disable InconsistentNaming
    public class DeleteResourcesTest
    {
        [TestMethod]
        public void DeleteResource_Command_Type_IsPost()
        {
            // Arrange
            var deleteResource = new DeleteResources();

            // Act
            var request = deleteResource.ToRequest();

            // Assert
            Assert.AreEqual(DlapRequestType.Post, request.Type);
        }

        [TestMethod]
        public void DeleteResource_Command_With_SomeResource_Create_ResourceList_ToDelete()
        {
            // Arrange
            XDocument expectedResult = XDocument.Parse(@"<requests>  <resource entityid=""someId"" path=""valid Path1"" /><resource entityid=""someId"" path=""valid Path2"" /></requests>");
            var deleteResources = new DeleteResources
            {
                ResourcesToDelete =
                    new List<Resource>
                    {
                        (new Resource { EntityId = "someId", Url = "valid Path1" }),
                        (new Resource { EntityId = "someId", Url = "valid Path2" })
                    }
            };

            // Act
            var request = deleteResources.ToRequest();
            var actualResult = request.GetXmlRequestBody();

            // Assert
            Assert.AreEqual(expectedResult.ToString(), actualResult.ToString(), "Failed to create DLAP request for DELETE Resoures");
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException), "DeleteResources request failed - with response code ")]
        public void Parse_DeleteResource_With_Incorrect_ResponseCode_Throws_Error()
        {
            // Arrange
            var dlapResponse = new DlapResponse { Code = DlapResponseCode.AccessDenied };

            // Act
            (new DeleteResources()).ParseResponse(dlapResponse);
        }
    }
    // ReSharper restore InconsistentNaming
}