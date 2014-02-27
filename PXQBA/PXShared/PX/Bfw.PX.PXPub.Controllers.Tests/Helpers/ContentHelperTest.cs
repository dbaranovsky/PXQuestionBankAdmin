using Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Controllers.Mappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Models;
using NSubstitute;
using System.Web;
using System.Linq;
using System.IO;
using Microsoft.Practices.ServiceLocation;
using System.Collections.Generic;
using ContentItem = Bfw.PX.Biz.DataContracts.ContentItem;
using Course = Bfw.PX.Biz.DataContracts.Course;
using CourseType = Bfw.PX.PXPub.Models.CourseType;
using Container = Bfw.PX.PXPub.Models.Container;
using System.Xml.Linq;
using Bfw.Common.Caching;

namespace Bfw.PX.PXPub.Controllers.Tests.Helpers
{
    
    
    /// <summary>
    ///This is a test class for ContentHelperTest and is intended
    ///to contain all ContentHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ContentHelperTest
    {


        private TestContext testContextInstance;
        private IBusinessContext _context;
        private INavigationActions _navActions;
        private IContentActions _contActions;
        private IAssignmentActions _assignmentActions;
        private IGradeActions _gradeActions;        
        private IResourceMapActions _resourceMapActions;
        private INoteActions _noteActions;
        private IUserActivitiesActions _userActivitiesActions;
        private ContentHelper _target;
        private IServiceLocator _serviceLocator;
        private ICacheProvider _cacheProvider;
        private string _defaultToc;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes

        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            
            _context = Substitute.For<IBusinessContext>();
            _navActions = Substitute.For<INavigationActions>(); 
            _contActions = Substitute.For<IContentActions>(); 
            _assignmentActions = Substitute.For<IAssignmentActions>();  
            _gradeActions = Substitute.For<IGradeActions>(); 
            _resourceMapActions = Substitute.For<IResourceMapActions>();            
            _noteActions = Substitute.For<INoteActions>();
            _userActivitiesActions = Substitute.For<IUserActivitiesActions>();
            _cacheProvider = Substitute.For<ICacheProvider>();
            _serviceLocator = Substitute.For<IServiceLocator>();            
            ServiceLocator.SetLocatorProvider(() => _serviceLocator);
            _context.CacheProvider.Returns(_cacheProvider);
            _target = new ContentHelper(_context, _navActions, _contActions, _assignmentActions, _gradeActions, _resourceMapActions, _noteActions, _userActivitiesActions);

            _defaultToc = "syllabusfilter";
        }

        #endregion


        /// <summary>
        ///A test for StoreLink
        /// Method should store title, subtitle, url
        /// Preserve container/subcontainer
        ///</summary>
        [TestMethod()]
        public void StoreLinkTest()
        {
            var lnk = new Link()
            {
                Id = "itemId1",
                EntityId = "entityId1",
                Title = "link title",
                SubTitle = "link subtitle",
                Url = "http://cnn.com",
                IsBeingEdited = true,
                Type = "link"
            };
            var serverContentItem = new Biz.DataContracts.ContentItem()
            {
                Id = "itemId1",
                CourseId = "entityId1",
                Title = "untitled",
                SubTitle = "",
                Href = "",
                Type = "link"
            };

            var expectedStoredItem = new Biz.DataContracts.ContentItem()
            {
                Id = "itemId1",
                CourseId = "entityId1",
                Title = "link title",
                SubTitle = "link subtitle",
                Href = "http://cnn.com"
            };
            string courseId = "entityId1";

            _contActions.GetContent("entityId1", "itemId1")
                .Returns(c => serverContentItem);

            //ACT
            _target.StoreLink(lnk, courseId);

            //ASSERT:check if item was stored with modified properties
            _contActions.Received().StoreContent(Arg.Is<Biz.DataContracts.ContentItem>(c =>
                c.Id == expectedStoredItem.Id
                && c.CourseId == "entityId1"
                && c.Title == expectedStoredItem.Title
                && c.SubTitle == expectedStoredItem.SubTitle
                && c.Href == expectedStoredItem.Href
                ));
        }

        [TestMethod]
        public void StoreModule_Should_Save_n_Maintain_Containers()
        {
            _context.Course = new Biz.DataContracts.Course();

            var contentHelper = new ContentHelper(_context, _navActions, _contActions,
                                               _assignmentActions, _gradeActions, _resourceMapActions, _noteActions,
                                               _userActivitiesActions);
            
            HttpContext.Current = new HttpContext(new HttpRequest("", "http://test.com", ""), new HttpResponse(new StringWriter()));

            const string toc = "whatever_filter";
            const string root = "PXUNITROOT";
            var pxUnit = new PxUnit
            {
                Id = "TestId",
                SyllabusFilter = toc,
                Sequence = "a",
                ParentId = root
            };

            pxUnit.SetSyllabusFilterCategory(pxUnit.ParentId, toc: toc, sequence: "a");

            pxUnit.Containers.Add(new Container(toc, "Launchpad"));
            pxUnit.SubContainerIds.Add(new Container(toc, pxUnit.ParentId));

            contentHelper.StoreModule(pxUnit, "1111");

            var validCat = pxUnit.Categories.FirstOrDefault(c => c.Id == toc);
            Assert.IsNotNull(validCat);

            Assert.IsTrue(pxUnit.Containers.Exists(c => c.Toc == toc));
            Assert.IsTrue(pxUnit.SubContainerIds.Exists(c => c.Toc == toc));
            Assert.IsTrue(pxUnit.SubContainerIds.Exists(c => c.Value == root));
        }

        [TestMethod]
        public void LoadContentView_Populates_Grades_ForStudent()
        {
            _context.EntityId.Returns("entityid");
            _context.Course = new Course()
            {
               Id = "entityId"
            };
            var contentItem = new Biz.DataContracts.ContentItem()
            {
                Id = "itemid",
                ActualEntityid = "entityid",
                Type = "none",
                GradebookInfo = new GradebookInfo()
                {
                    ItemId = "itemid",
                    LastScore = 5,
                    IsUserSubmitted = true
                }

            };
            _contActions.GetContent("entityid", "itemid", true, null).Returns(contentItem);
            bool getGradesCalled = false;
            _contActions.GetGradesPerItem(Arg.Any<IList<ContentItem>>(), "entityid").WhenForAnyArgs(a => getGradesCalled = true)
                .Do(a => getGradesCalled = true);


            var model = _target.LoadContentView("itemid", ContentViewMode.Preview, false, false, getChildrenGrades: true);

            
            Assert.IsTrue(getGradesCalled);
            Assert.AreEqual(contentItem.GradebookInfo.LastScore, model.Content.Score);
            Assert.AreEqual(contentItem.GradebookInfo.IsUserSubmitted, model.Content.IsUserSubmitted);
            Assert.AreEqual(contentItem.GradebookInfo.ItemId, model.Content.Id);
        }

        [TestMethod]
        public void LoadContentView_LoadsChildrenLinks_for_LinkCollection()
        {
            _context.EntityId.Returns("entityId");
            _context.CourseId = "entityId";
            _serviceLocator.GetInstance<IBusinessContext>().Returns(_context);

            _context.Course = new Course()
            {
                Id = "entityId"
            };
            var contentItem = new Biz.DataContracts.ContentItem()
            {
                Id = "itemid",
                ActualEntityid = "entityId",
                Type = "linkcollection",
            };
            _contActions.GetContent("entityId", "itemid", true, null).Returns(contentItem);
            bool listChildrenCalled = false;

            _contActions.ListChildren("entityId", "itemid")
                .Returns(new List<ContentItem>()
                {
                    new ContentItem() {Id = "lnk1", Href = "url1"},
                    new ContentItem() {Id = "lnk2", Href = "url2"},
                })
                .AndDoes(ci => listChildrenCalled = true);

            var model = _target.LoadContentView("itemid", ContentViewMode.Preview, false, false, getChildrenGrades: true);


            Assert.IsTrue(listChildrenCalled);
            Assert.IsTrue(model.Content is LinkCollection);
            Assert.AreEqual(((LinkCollection)model.Content).Links.Count, 2);
            Assert.AreEqual(((LinkCollection)model.Content).Links[0].Id, "lnk1");
            Assert.AreEqual(((LinkCollection)model.Content).Links[1].Id, "lnk2");

        }

        /// <summary>
        /// LoadContentView should not show results (item analysis screen) to instructor
        /// for Resource items (Agilix does not support this)
        /// </summary>
        [TestMethod]
        public void LoadContentView_DisablesResultsForResourceType()
        {
            _context.EntityId.Returns("entityId");
            _context.CourseId = "entityId";
            _context.AccessLevel = AccessLevel.Instructor;
            _context.Course = new Course() { Id = "entityId" };
            _serviceLocator.GetInstance<IBusinessContext>().Returns(_context);

            _context.Course = new Course()
            {
                Id = "entityId"
            };
            var contentItem = new Biz.DataContracts.ContentItem()
            {
                Id = "itemid",
                Type = "Resource",
                Subtype = "externalcontent",
                Href = "comingsoon.html"
            };
            
            _contActions.GetContent("entityId", "itemid", true, null).Returns(contentItem);
            
            var model = _target.LoadContentView("itemid", ContentViewMode.Preview, false, false);

            Assert.IsFalse((model.AllowedModes & ContentViewMode.Results) == ContentViewMode.Results);
        }

        /// <summary>
        /// LoadContentView should show results to instructor
        /// for all types besides Resources (including custom types)
        /// </summary>
        [TestMethod]
        public void LoadContentView_EnableResultsForAssignmentType()
        {
            _context.EntityId.Returns("entityId");
            _context.CourseId = "entityId";
            _context.AccessLevel = AccessLevel.Instructor;
            _context.Course = new Course(){ Id = "entityId" };
            _serviceLocator.GetInstance<IBusinessContext>().Returns(_context);

            //Assignment Type
            var contentItem = new Biz.DataContracts.ContentItem()
            {
                Id = "itemid",
                Type = "Assignment",
                Subtype = "externalcontent",
                Href = "comingsoon.html"
            };
            _contActions.GetContent("entityId", "itemid", true, null).Returns(contentItem);

            var model = _target.LoadContentView("itemid", ContentViewMode.Preview, false, false);

            Assert.IsTrue((model.AllowedModes & ContentViewMode.Results) == ContentViewMode.Results);

            //Custom Type

            contentItem = new Biz.DataContracts.ContentItem()
            {
                Id = "itemid",
                Type = "ASDASDAS" //Custom item types should show results
            };
            _contActions.GetContent("entityId", "itemid", true, null).Returns(contentItem);

            model = _target.LoadContentView("itemid", ContentViewMode.Preview, false, false);

            Assert.IsTrue((model.AllowedModes & ContentViewMode.Results) == ContentViewMode.Results);
        }

        /// <summary>
        /// The Link type verificatioon - bad data for href tag, should be presented as empty string if invalid
        /// </summary>
        [TestMethod]
        public void LoadContentView_Returns_Empty_Url_For_Link_If_Href_Invalid()
        {
            _context.EntityId.Returns("entityId");
            _context.CourseId = "entityId";
            _context.AccessLevel = AccessLevel.Instructor;
            _context.Course = new Course() { Id = "entityId" };
            _serviceLocator.GetInstance<IBusinessContext>().Returns(_context);
            var contentItem = new Biz.DataContracts.ContentItem()
            {
                Id = "itemid",
                Type = "Link",
                Subtype = "link",
                Href = "/so_so_broken.html"
            };
            _contActions.GetContent("entityId", "itemid", true, null).Returns(contentItem);

            var model = _target.LoadContentView("itemid", ContentViewMode.Preview, false, false);

            Assert.IsTrue(model.Content.Url.Length == 0);
        }

        /// <summary>
        /// The Link type verificatioon - bad data for href tag, should be presented as empty string if invalid
        /// </summary>
        [TestMethod]
        public void LoadContentView_Returns_Url_For_Link_If_Href_Valid()
        {
            _context.EntityId.Returns("entityId");
            _context.CourseId = "entityId";
            _context.AccessLevel = AccessLevel.Instructor;
            _context.Course = new Course() { Id = "entityId" };
            _serviceLocator.GetInstance<IBusinessContext>().Returns(_context);
            var contentItem = new Biz.DataContracts.ContentItem()
            {
                Id = "itemid",
                Type = "Link",
                Subtype = "link",
                Href = "http://google.com"
            };
            _contActions.GetContent("entityId", "itemid", true, null).Returns(contentItem);

            var model = _target.LoadContentView("itemid", ContentViewMode.Preview, false, false);

            Assert.AreEqual(contentItem.Href, model.Content.Url);
        }

        [TestMethod]
        public void InvalidateCachedPageDefinitionsForBranchedCourses()
        {
            var courseId = "123456";
            var productCourseId = "85256";
            var pageName = "PX_HOME_FACEPLATE_START";
            _context.CourseId.Returns(courseId);
            _context.ProductCourseId.Returns(productCourseId);
            _serviceLocator.GetInstance<IBusinessContext>().Returns(_context);
            _contActions.GetItemLinks(courseId).Returns(new List<ItemLink> { new ItemLink{ EntityId = "181975" }, new ItemLink{ EntityId = "181979" } });
            _target.InvalidateCachedPageDefinitionsForDerivedCourses(courseId, pageName);
            //Assert
            _cacheProvider.Received().Remove(String.Format("PX_COURSE_181975_PAGE_{0}", pageName), "ProductCourse_" + productCourseId);
            _cacheProvider.Received().Remove(String.Format("PX_COURSE_181979_PAGE_{0}", pageName), "ProductCourse_" + productCourseId);
        }

        [TestMethod]
        public void InvalidateCachedPageDefinitionsForBranchedCourses_NoDerivatives()
        {
            var courseId = "123456";
            var pageName = "PX_HOME_FACEPLATE_START";
            _contActions.GetItemLinks(courseId).Returns(new List<ItemLink>());
            _target.InvalidateCachedPageDefinitionsForDerivedCourses(courseId, pageName);
            //Assert
            _cacheProvider.DidNotReceive().Remove(Arg.Any<String>());
        }

        [TestMethod]
        public void EditCustomWidget_RemovePageDefinitionsFromCache_DerivedCourses()
        {
            var courseId = "123456";
            var productCourseId = "85256";
            var pageName = "PX_HOME_FACEPLATE_START";
            var widget = new CustomWidget();
            var contentItem = new Bfw.PX.Biz.DataContracts.ContentItem();
            _contActions.GetContent(courseId, widget.Id).Returns(contentItem);
            _context.Course.Returns(new Course { Id = courseId, CourseStartPage = pageName });
            _context.CourseId.Returns(courseId);
            _context.ProductCourseId.Returns(productCourseId);
            _serviceLocator.GetInstance<IBusinessContext>().Returns(_context);
            _contActions.GetItemLinks(courseId).Returns(new List<ItemLink> { new ItemLink { EntityId = "181975" }, new ItemLink { EntityId = "181979" } });
            _target.StoreCustomWidget(widget, courseId);
            //Assert
            _contActions.Received().StoreContent(Arg.Any<Bfw.PX.Biz.DataContracts.ContentItem>());
            _cacheProvider.Received(2).Remove(Arg.Any<string>(), Arg.Any<string>());
        }

        [TestCategory("ContentHelper"),TestMethod]
        public void LoadContentView_ForDropBox_IfNotAssigned_ExpectNoResultViewMode()
        {
            Dropbox dropBox = new Dropbox { IsAssigned = false};
            _context.AccessLevel = AccessLevel.Instructor;
            _context.Course = new Course();
            var contentView = _target.LoadContentView(dropBox, ContentViewMode.Edit, false, false);
            Assert.IsFalse((contentView.AllowedModes & ContentViewMode.Results) == ContentViewMode.Results);
        }

        [TestCategory("ContentHelper"), TestMethod]
        public void LoadContentView_ForDropBox_IfAssigned_ExpectResultViewMode()
        {
            Dropbox dropBox = new Dropbox { IsAssigned = true };
            _context.AccessLevel = AccessLevel.Instructor;
            _context.Course = new Course();
            var contentView = _target.LoadContentView(dropBox, ContentViewMode.Edit, false, false);
            Assert.IsTrue((contentView.AllowedModes & ContentViewMode.Results) == ContentViewMode.Results);
        }

        [TestCategory("ContentHelper"), TestMethod]
        public void StoreHtmlDocument_IfAssigned_SetTypeToAssignment()
        {
            HtmlDocument hd = new HtmlDocument { Id = "testHtmlDocument", IsBeingEdited = true};
            var expectedContent = new ContentItem {AssignmentSettings = new AssignmentSettings {DueDate = DateTime.Now}};
            _contActions.GetContent("entityId", "testHtmlDocument").Returns(expectedContent);
            _target.StoreHtmlDocument(hd, "entityId");
            Assert.AreEqual("Assignment", expectedContent.Type);
        }

        [TestCategory("ContentHelper"), TestMethod]
        public void StoreHtmlDocument_IfNotAssigned_SetTypeToResource()
        {
            HtmlDocument hd = new HtmlDocument { Id = "testHtmlDocument", IsBeingEdited = true };
            var expectedContent = new ContentItem { AssignmentSettings = new AssignmentSettings { DueDate = DateTime.MinValue } };
            _contActions.GetContent("entityId", "testHtmlDocument").Returns(expectedContent);
            _target.StoreHtmlDocument(hd, "entityId");
            Assert.AreEqual("Resource", expectedContent.Type);
        }
    }
}
