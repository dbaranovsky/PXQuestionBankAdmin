using System;
using System.Collections.Generic;
using Bfw.PXWebAPI.Models.Response;
using NSubstitute;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PXWebAPI.Models;
using Bfw.PXWebAPI.Controllers;
using Bfw.PXWebAPI.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using System.IO;
using System.Web.SessionState;
using System.Reflection;
using System.Web.Script.Serialization;

namespace Bfw.PXWebAPI.Tests
{
    [TestClass]
    public class ContentControllerTest
    {
        private ContentController controller;

        private IBusinessContext context;
        private IApiContentActions content;
        private IApiSearchActions search;
        private IApiCourseActions course;
        private IApiUserActions user;

        private HttpContext httpContext;

        [TestInitialize]
        public void TestInitialize()
        {
            context = Substitute.For<IBusinessContext>();
            content = Substitute.For<IApiContentActions>();
            search = Substitute.For<IApiSearchActions>();
            course = Substitute.For<IApiCourseActions>();
            user = Substitute.For<IApiUserActions>();

            controller = new ContentController(context, content, search, course, user);

            InitializeControllerContext();      
        }

        [TestMethod]
        public void ExpectToGetListOfItemsToSync()
        {
            //arrange
            var courseId = "1234";
            var since = DateTime.Now.AddDays(-4);

            content.GetItemsToSync(courseId, since).ReturnsForAnyArgs(new SyncItemList
            {
                Error = false,
                Message = "",
                SyncItems = new List<SyncItem> {
                    new SyncItem(),
                    new SyncItem(),
                    new SyncItem(),
                    new SyncItem()
                }
            });

            //act
            var itemsToSync = controller.Sync(courseId, since);

            //assert
            content.Received().GetItemsToSync(courseId, since);
            Assert.AreEqual(4, itemsToSync.Count);            
        }

        [TestMethod]
        public void ExpectToGetTOC()
        {
            //arrange
            var courseId = "120478";
            var itemId = "PX_MULTIPART_LESSONS";

            var toc = controller.TableofContents(itemId, courseId).ReturnsForAnyArgs(new TableofContentsItemListResponse() {error_message = "success", results = 
                
                new List<TableofContentsItem>() {
                new TableofContentsItem()
                {
                    ItemId = "29851825abfe4c528e756219cb5128c5",
                    DueDate = null,
                    MaxPoints = 0,
                    Title = "Dan Homework",
                    Description = null,
                    Category = "-1",
                    Visibility = true,
                    Sequence = "ba",
                    iconUri = null,
                    ParentId = "PX_MANIFEST",
                    Type = "Homework",
                    SubType = null,
                    CourseId = null,
                    Removed = false
                }
               }, status_code = 0});

            //assert
            Assert.IsNotNull(toc);
        }

        [TestMethod]
        public void Details_Should_Return_Empty_Response()
        {
            var result = controller.Details("1");

            Assert.AreEqual(Helper.NO_RESULTS, result.error_message);
        }

        [TestMethod]
        public void Details_Should_Return_Full_Structure()
        {
            var collection = HttpContext.Current.Request.Form;
            var propInfo = collection.GetType().GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
            propInfo.SetValue(collection, false, new object[] { });
            collection[Helper.ITEMIDS] = "2";
            propInfo.SetValue(collection, true, new object[] { });

            content.GetItems("", "", Helper.DEFAULT_TOC).ReturnsForAnyArgs(new List<Models.ContentItem> { new Models.ContentItem() });

            var result = controller.Details("1");
            string expected = "{\"results\":[{\"ItemId\":null,\"DueDate\":null,\"MaxPoints\":0,\"Title\":null,\"Description\":null,\"Category\":null,\"Visibility\":false,\"Sequence\":null,\"iconUri\":null,\"ParentId\":null,\"SubContainerId\":null,\"Type\":null,\"SubType\":null,\"CourseId\":null,\"Removed\":false}],\"status_code\":0,\"error_message\":null}";

            Assert.AreEqual(expected, new JavaScriptSerializer().Serialize(result));
        }

        private void InitializeControllerContext()
        {
            var httpRequest = new HttpRequest("", "http://url/", "");
            var stringWriter = new StringWriter();
            var httpResponce = new HttpResponse(stringWriter);
            httpContext = new HttpContext(httpRequest, httpResponce);

            var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
                                                    new HttpStaticObjectsCollection(), 10, true,
                                                    HttpCookieMode.AutoDetect,
                                                    SessionStateMode.InProc, false);

            httpContext.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(
                                        BindingFlags.NonPublic | BindingFlags.Instance,
                                        null, CallingConventions.Standard,
                                        new[] { typeof(HttpSessionStateContainer) },
                                        null).Invoke(new object[] { sessionContainer });

            HttpContext.Current = httpContext;
        }
    }
}
