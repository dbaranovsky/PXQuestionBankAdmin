using System;
using System.Linq;
using System.Xml.Linq;
using Bfw.Common;
using NSubstitute;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.Agilix.Dlap.Session;
using Bfw.PX.Biz.DataContracts;
using System.Collections.Generic;

namespace Bfw.PX.Biz.Direct.Services.Tests
{
    [TestClass]
    public class EportfolioActionsTest
    {
        private IBusinessContext context;
        private IContentActions contentActions;
        private ISessionManager sessionManager;
        private IEnrollmentActions enrollmentActions;
        private IUserActions userActions;
        private IDocumentConverter documentConverter;
        private IEportfolioCourseActions eportfolioCourseActions;
        private ICourseActions courseActions;
        private EPortfolioActions actions;
        private IResourceMapActions resourceMapActions;

        [TestInitialize]
        public void TestInitialize()
        {
            context = Substitute.For<IBusinessContext>();
            context.CurrentUser.Returns(new UserInfo { Id = "116712", Username = "6669306", ReferenceId = "6669306" });
            sessionManager = Substitute.For<ISessionManager>();
            contentActions = Substitute.For<IContentActions>();
            courseActions = Substitute.For<ICourseActions>();
            enrollmentActions = Substitute.For<IEnrollmentActions>();
            userActions = Substitute.For<IUserActions>();
            documentConverter = Substitute.For<IDocumentConverter>();
            eportfolioCourseActions = Substitute.For<IEportfolioCourseActions>();
            resourceMapActions = Substitute.For<IResourceMapActions>();
            actions = new EPortfolioActions(context, contentActions, sessionManager, enrollmentActions, userActions, documentConverter, eportfolioCourseActions, courseActions, resourceMapActions);
        }

        [TestMethod]
        public void CanCopy_UploadedItem_ToPresentation()
        {
            var fileName = "Test.docx";
            var sourceEntityId = "129889";
            var targetEntityId = "132175";
            var itemId = "650a4bab70f647959fc9e17484b6a46b";
            var documentId = "aad5439jdf342356b90";
            var sourceEnrollmentId = "129893";
            var presentationEnrollmentId = "132177";
            var dashboardCourseId = "123190";
            var dashboardEnrollmentId = "123192";
            var itemIds = new List<string> { itemId };
            var userId = context.CurrentUser.Id;
            var sourceCourseDomainId = "66159";
            var sourceItemHref = "Templates/Data/650a4bab70f647959fc9e17484b6a46b/index.html";
            courseActions.GetCourseByCourseId(sourceEntityId).Returns(new Course { Id = sourceEntityId, Domain = new Domain { Id = sourceCourseDomainId } });
            courseActions.GetCourseByCourseId(targetEntityId).Returns(new Course { Id = targetEntityId, Domain = new Domain { Id = "" } });
            if(!LmsIdIntegration.Enabled) 
               userActions.ListUsersLike(Arg.Is<UserInfo>(u => u.ReferenceId == (context.CurrentUser.RAId))).Returns(new List<UserInfo> { new UserInfo { Id = userId, DomainId = sourceCourseDomainId } });
            else
                userActions.ListUsersLike(Arg.Is<UserInfo>(u => u.Username == (context.CurrentUser.RAId))).Returns(new List<UserInfo> { new UserInfo { Id = userId, DomainId = sourceCourseDomainId } }); 

            enrollmentActions.GetUserEnrollmentId(userId, sourceEntityId).Returns(sourceEnrollmentId);
            enrollmentActions.GetUserEnrollmentId(context.CurrentUser.Id, targetEntityId).Returns(presentationEnrollmentId);
            eportfolioCourseActions.GetPersonalEportfolio(context.CurrentUser.RAId, Arg.Any<String>()).Returns(dashboardCourseId);
            enrollmentActions.GetUserEnrollmentId(context.CurrentUser.Id, dashboardCourseId).Returns(dashboardEnrollmentId);
            var ci = new ContentItem { Id = itemId, Href = sourceItemHref, Subtype = "UploadOrCompose", Type = "DocumentCollection", Categories = new List<TocCategory>() };
            ci.Properties = new Dictionary<string, PropertyValue>();
            ci.Properties.Add("FileName", new PropertyValue { Value = fileName });
            ci.Properties.Add("DocId", new PropertyValue { Value = documentId });
            contentActions.GetAllStudentItems(sourceEnrollmentId).Returns(new List<ContentItem> { ci });
            contentActions.GetResource(sourceEnrollmentId, sourceItemHref).Returns(new Resource { Url = sourceItemHref, EntityId = sourceEnrollmentId });
            var contentItem = new ContentItem { Id = documentId, Href = String.Format("Assets/{0}", fileName) };
            contentActions.GetContent(sourceEntityId, documentId).Returns(contentItem);
            contentActions.GetResource(sourceEnrollmentId, String.Format("Assets/{0}", fileName)).Returns(new Resource { Url = String.Format("Assets/{0}", fileName) });
            contentActions.GetEmptyStudentDoc().Returns(XDocument.Parse("<root/>"));
            List<ContentItem> rejectedItems;
            actions.CopyEportfolioItems(itemIds, sourceEntityId, targetEntityId, EportfolioItemCopyType.EportfolioToPresentation, out rejectedItems);
            //Assert
            contentActions.Received().StoreResources(Arg.Is<IEnumerable<Resource>>(l => l.First().EntityId == dashboardEnrollmentId && l.First().Url == sourceItemHref));
            contentActions.Received().CopyItemToAnotherEntity(Arg.Is<ContentItem>(c => c.Id == documentId), sourceEntityId, dashboardCourseId);
            contentActions.Received().CopyResourceToAnotherEntity(Arg.Is<ContentItem>(c => c.Id == documentId && c.Resources.First().Url == String.Format("Assets/{0}", fileName)), dashboardEnrollmentId);
        }

        [TestMethod]
        public void CanCopy_ComposedItem_ToPresentation()
        {
            var sourceEntityId = "129889";
            var targetEntityId = "132175";
            var itemId = "650a4bab70f647959fc9e17484b6a46b";
            var sourceEnrollmentId = "129893";
            var presentationEnrollmentId = "132177";
            var dashboardCourseId = "123190";
            var dashboardEnrollmentId = "123192";
            var itemIds = new List<string> { itemId };
            var userId = context.CurrentUser.Id;
            var sourceCourseDomainId = "66159";
            var sourceItemHref = "Templates/Data/650a4bab70f647959fc9e17484b6a46b/index.html";
            courseActions.GetCourseByCourseId(sourceEntityId).Returns(new Course { Id = sourceEntityId, Domain = new Domain { Id = sourceCourseDomainId } });
            courseActions.GetCourseByCourseId(targetEntityId).Returns(new Course { Id = targetEntityId, Domain = new Domain { Id = "" } });
            if(!LmsIdIntegration.Enabled) 
               userActions.ListUsersLike(Arg.Is<UserInfo>(u => u.ReferenceId == (context.CurrentUser.RAId))).Returns(new List<UserInfo> { new UserInfo { Id = userId, DomainId = sourceCourseDomainId } });
            else
                userActions.ListUsersLike(Arg.Is<UserInfo>(u => u.Username == (context.CurrentUser.RAId))).Returns(new List<UserInfo> { new UserInfo { Id = userId, DomainId = sourceCourseDomainId } });

            enrollmentActions.GetUserEnrollmentId(userId, sourceEntityId).Returns(sourceEnrollmentId);
            enrollmentActions.GetUserEnrollmentId(context.CurrentUser.Id, targetEntityId).Returns(presentationEnrollmentId);
            eportfolioCourseActions.GetPersonalEportfolio(context.CurrentUser.RAId, Arg.Any<String>()).Returns(dashboardCourseId);
            enrollmentActions.GetUserEnrollmentId(context.CurrentUser.Id, dashboardCourseId).Returns(dashboardEnrollmentId);
            var ci = new ContentItem { Id = itemId, Href = sourceItemHref, Subtype = "HtmlDocument", Type = "HtmlDocument", Categories = new List<TocCategory>() };
            contentActions.GetAllStudentItems(sourceEnrollmentId).Returns(new List<ContentItem> { ci });
            contentActions.GetResource(sourceEnrollmentId, sourceItemHref).Returns(new Resource { Url = sourceItemHref, EntityId = sourceEnrollmentId });
            contentActions.GetEmptyStudentDoc().Returns(XDocument.Parse("<root/>"));
            List<ContentItem> rejectedItems;
            actions.CopyEportfolioItems(itemIds, sourceEntityId, targetEntityId, EportfolioItemCopyType.EportfolioToPresentation, out rejectedItems);
            //Assert
            contentActions.Received().StoreResources(Arg.Is<IEnumerable<Resource>>(l => l.First().EntityId == dashboardEnrollmentId && l.First().Url == sourceItemHref));
            contentActions.DidNotReceive().CopyItemToAnotherEntity(Arg.Any<ContentItem>(), Arg.Any<String>(), Arg.Any<String>());
            contentActions.DidNotReceive().CopyResourceToAnotherEntity(Arg.Any<ContentItem>(), Arg.Any<String>());
        }

        [TestMethod]
        public void WillNotSaveOriginalDocumentIfNotDocument()
        {
            var sourceEntityId = "129889";
            var targetEntityId = "132175";
            var itemId = "650a4bab70f647959fc9e17484b6a46b";
            var sourceEnrollmentId = "129893";
            var presentationEnrollmentId = "132177";
            var dashboardCourseId = "123190";
            var dashboardEnrollmentId = "123192";
            var itemIds = new List<string> { itemId };
            var userId = context.CurrentUser.Id;
            var sourceCourseDomainId = "66159";
            var sourceItemHref = "Templates/Data/650a4bab70f647959fc9e17484b6a46b/index.html";
            courseActions.GetCourseByCourseId(sourceEntityId).Returns(new Course { Id = sourceEntityId, Domain = new Domain { Id = sourceCourseDomainId } });
            courseActions.GetCourseByCourseId(targetEntityId).Returns(new Course { Id = targetEntityId, Domain = new Domain { Id = "" } });
            
            if (!LmsIdIntegration.Enabled)  
               userActions.ListUsersLike(Arg.Is<UserInfo>(u => u.ReferenceId == (context.CurrentUser.RAId))).Returns(new List<UserInfo> { new UserInfo { Id = userId, DomainId = sourceCourseDomainId } });
            else
                userActions.ListUsersLike(Arg.Is<UserInfo>(u => u.Username == (context.CurrentUser.RAId))).Returns(new List<UserInfo> { new UserInfo { Id = userId, DomainId = sourceCourseDomainId } });

            enrollmentActions.GetUserEnrollmentId(userId, sourceEntityId).Returns(sourceEnrollmentId);
            enrollmentActions.GetUserEnrollmentId(context.CurrentUser.Id, targetEntityId).Returns(presentationEnrollmentId);
            eportfolioCourseActions.GetPersonalEportfolio(context.CurrentUser.RAId, Arg.Any<String>()).Returns(dashboardCourseId);
            enrollmentActions.GetUserEnrollmentId(context.CurrentUser.Id, dashboardCourseId).Returns(dashboardEnrollmentId);
            var ci = new ContentItem { Id = itemId, Href = sourceItemHref, Subtype = "UploadOrCompose", Type = "DocumentCollection", Categories = new List<TocCategory>() };
            ci.Properties = new Dictionary<string, PropertyValue>();
            ci.Properties.Add("FileName", new PropertyValue { Value = "NotADocument" });
            contentActions.GetAllStudentItems(sourceEnrollmentId).Returns(new List<ContentItem> { ci });
            contentActions.GetResource(sourceEnrollmentId, sourceItemHref).Returns(new Resource { Url = sourceItemHref, EntityId = sourceEnrollmentId });
            contentActions.GetEmptyStudentDoc().Returns(XDocument.Parse("<root/>"));
            List<ContentItem> rejectedItems;
            actions.CopyEportfolioItems(itemIds, sourceEntityId, targetEntityId, EportfolioItemCopyType.EportfolioToPresentation, out rejectedItems);
            //Assert
            contentActions.Received().StoreResources(Arg.Is<IEnumerable<Resource>>(l => l.First().EntityId == dashboardEnrollmentId && l.First().Url == sourceItemHref));
            contentActions.DidNotReceive().CopyItemToAnotherEntity(Arg.Any<ContentItem>(), Arg.Any<String>(), Arg.Any<String>());
            contentActions.DidNotReceive().CopyResourceToAnotherEntity(Arg.Any<ContentItem>(), Arg.Any<String>());
        }

        [TestMethod]
        public void CanCopy_Folder_ToPresentation()
        {
            var sourceEntityId = "129889";
            var targetEntityId = "132175";
            var itemId = "650a4bab70f647959fc9e17484b6a46b";
            var sourceEnrollmentId = "129893";
            var presentationEnrollmentId = "132177";
            var dashboardCourseId = "123190";
            var dashboardEnrollmentId = "123192";
            var itemIds = new List<CopyToItem> { new CopyToItem { Id = itemId, Type = "eportfolio", ParentId = "PX_COURSE_EPORTFOLIO_ROOT_ITEM" } };
            var userId = context.CurrentUser.Id;
            var sourceCourseDomainId = "66159";
            courseActions.GetCourseByCourseId(sourceEntityId).Returns(new Course { Id = sourceEntityId, Domain = new Domain { Id = sourceCourseDomainId } });
            courseActions.GetCourseByCourseId(targetEntityId).Returns(new Course { Id = targetEntityId, Domain = new Domain { Id = "" } });

            if(!LmsIdIntegration.Enabled) 
               userActions.ListUsersLike(Arg.Is<UserInfo>(u => u.ReferenceId == (context.CurrentUser.RAId))).Returns(new List<UserInfo> { new UserInfo { Id = userId, DomainId = sourceCourseDomainId } });
            else
                userActions.ListUsersLike(Arg.Is<UserInfo>(u => u.Username == (context.CurrentUser.RAId))).Returns(new List<UserInfo> { new UserInfo { Id = userId, DomainId = sourceCourseDomainId } });

            enrollmentActions.GetUserEnrollmentId(userId, sourceEntityId).Returns(sourceEnrollmentId);
            enrollmentActions.GetUserEnrollmentId(context.CurrentUser.Id, targetEntityId).Returns(presentationEnrollmentId);
            eportfolioCourseActions.GetPersonalEportfolio(context.CurrentUser.RAId, Arg.Any<String>()).Returns(dashboardCourseId);
            enrollmentActions.GetUserEnrollmentId(context.CurrentUser.Id, dashboardCourseId).Returns(dashboardEnrollmentId);
            var ci = new ContentItem { Id = itemId, Type = "Assignment", ParentId = "PX_TEMPLATE", DefaultCategoryParentId = "PX_COURSE_EPORTFOLIO_ROOT_ITEM", Subtype = "ePortfolio" };
            contentActions.GetAllStudentItems(sourceEnrollmentId).Returns(new List<ContentItem> { ci });
            contentActions.GetStudentResourceId(presentationEnrollmentId).Returns(String.Format("enrollment_{0}", presentationEnrollmentId));
            var doc = XDocument.Parse("<items/>");
            contentActions.GetEmptyStudentDoc().Returns(doc);
            List<ContentItem> rejectedItems;
            actions.CopyEportfolioItems(itemIds, sourceEntityId, targetEntityId, EportfolioItemCopyType.EportfolioToPresentation, out rejectedItems);
            //Assert
            contentActions.Received().StoreResources(Arg.Is<IEnumerable<Resource>>(l => l.First().EntityId == presentationEnrollmentId && l.First().Url == String.Format("enrollment_{0}", presentationEnrollmentId)));
        }

        [TestMethod]
        public void CanCopy_ReflectionAssignment_ToPresentation()
        {
            var sourceEntityId = "129889";
            var targetEntityId = "132175";
            var itemId = "c18ef225e41e4c2fa2870999482ac984";
            var sourceEnrollmentId = "129893";
            var presentationEnrollmentId = "132177";
            var dashboardCourseId = "123190";
            var dashboardEnrollmentId = "123192";
            var itemIds = new List<CopyToItem> { new CopyToItem { Id = itemId, Type = "reflectionassignment", ParentId = "PX_COURSE_EPORTFOLIO_ROOT_ITEM" } };
            var userId = context.CurrentUser.Id;
            var sourceCourseDomainId = "66159";
            courseActions.GetCourseByCourseId(sourceEntityId).Returns(new Course { Id = sourceEntityId, Domain = new Domain { Id = sourceCourseDomainId } });
            courseActions.GetCourseByCourseId(targetEntityId).Returns(new Course { Id = targetEntityId, Domain = new Domain { Id = "" } });

            if(!LmsIdIntegration.Enabled)
              userActions.ListUsersLike(Arg.Is<UserInfo>(u => u.ReferenceId == (context.CurrentUser.RAId))).Returns(new List<UserInfo> { new UserInfo { Id = userId, DomainId = sourceCourseDomainId } });
            else
                userActions.ListUsersLike(Arg.Is<UserInfo>(u => u.Username == (context.CurrentUser.RAId))).Returns(new List<UserInfo> { new UserInfo { Id = userId, DomainId = sourceCourseDomainId } });

            enrollmentActions.GetUserEnrollmentId(userId, sourceEntityId).Returns(sourceEnrollmentId);
            enrollmentActions.GetUserEnrollmentId(context.CurrentUser.Id, targetEntityId).Returns(presentationEnrollmentId);
            eportfolioCourseActions.GetPersonalEportfolio(context.CurrentUser.RAId, Arg.Any<String>()).Returns(dashboardCourseId);
            enrollmentActions.GetUserEnrollmentId(context.CurrentUser.Id, dashboardCourseId).Returns(dashboardEnrollmentId);
            var ci = new ContentItem { Id = itemId, Type = "Assignment", ParentId = "PX_TEMPLATE", DefaultCategoryParentId = "PX_COURSE_EPORTFOLIO_ROOT_ITEM", Subtype = "ReflectionAssignment", Categories = new List<TocCategory>() };
            contentActions.GetAllStudentItems(sourceEnrollmentId).Returns(new List<ContentItem> { ci });
            contentActions.GetStudentResourceId(presentationEnrollmentId).Returns(String.Format("enrollment_{0}", presentationEnrollmentId));
            resourceMapActions.GetResourcesForItem(itemId, sourceEnrollmentId, sourceEntityId).Returns(new List<Resource> { new XmlResource { EntityId = sourceEnrollmentId } });
            var doc = XDocument.Parse("<items/>");
            contentActions.GetEmptyStudentDoc().Returns(doc);
            List<ContentItem> rejectedItems;
            actions.CopyEportfolioItems(itemIds, sourceEntityId, targetEntityId, EportfolioItemCopyType.EportfolioToPresentation, out rejectedItems);
            //Assert
            contentActions.Received().CopyResourceToAnotherEntity(Arg.Is<ContentItem>(c => c.Resources.First().EntityId == sourceEnrollmentId), dashboardEnrollmentId);
            resourceMapActions.Received().AddResourceMap(Arg.Is<Resource>(r => r.EntityId == sourceEnrollmentId), itemId, "Assignment", dashboardCourseId);
            contentActions.Received().StoreResources(Arg.Is<IEnumerable<Resource>>(l => l.First().EntityId == dashboardEnrollmentId));
        }

        [TestMethod]
        public void CanNotCopyExistingItem_ToPresentation()
        {
            var sourceEntityId = "129889";
            var targetEntityId = "132175";
            var itemId = "c18ef225e41e4c2fa2870999482ac984";
            var sourceEnrollmentId = "129893";
            var presentationEnrollmentId = "132177";
            var dashboardCourseId = "123190";
            var dashboardEnrollmentId = "123192";
            var itemIds = new List<CopyToItem> { new CopyToItem { Id = itemId, Type = "reflectionassignment", ParentId = "PX_COURSE_EPORTFOLIO_ROOT_ITEM" } };
            var userId = context.CurrentUser.Id;
            var sourceCourseDomainId = "66159";
            courseActions.GetCourseByCourseId(sourceEntityId).Returns(new Course { Id = sourceEntityId, Domain = new Domain { Id = sourceCourseDomainId } });
            courseActions.GetCourseByCourseId(targetEntityId).Returns(new Course { Id = targetEntityId, Domain = new Domain { Id = "" } });

            if(!LmsIdIntegration.Enabled)
               userActions.ListUsersLike(Arg.Is<UserInfo>(u => u.ReferenceId == (context.CurrentUser.RAId))).Returns(new List<UserInfo> { new UserInfo { Id = userId, DomainId = sourceCourseDomainId } });
            else
                userActions.ListUsersLike(Arg.Is<UserInfo>(u => u.Username == (context.CurrentUser.RAId))).Returns(new List<UserInfo> { new UserInfo { Id = userId, DomainId = sourceCourseDomainId } });

            enrollmentActions.GetUserEnrollmentId(userId, sourceEntityId).Returns(sourceEnrollmentId);
            enrollmentActions.GetUserEnrollmentId(context.CurrentUser.Id, targetEntityId).Returns(presentationEnrollmentId);
            eportfolioCourseActions.GetPersonalEportfolio(context.CurrentUser.RAId, Arg.Any<String>()).Returns(dashboardCourseId);
            enrollmentActions.GetUserEnrollmentId(context.CurrentUser.Id, dashboardCourseId).Returns(dashboardEnrollmentId);
            contentActions.GetAllStudentItems(dashboardEnrollmentId).Returns(new List<ContentItem> { new ContentItem { Id = itemId, Categories = new List<TocCategory> { new TocCategory { Id = String.Format("ep_course_{0}", targetEntityId) } } } });
            List<ContentItem> rejectedItems;
            actions.CopyEportfolioItems(itemIds, sourceEntityId, targetEntityId, EportfolioItemCopyType.EportfolioToPresentation, out rejectedItems);
            Assert.AreEqual(1, rejectedItems.Count);
            Assert.AreEqual(itemId, rejectedItems.First().Id);
        }

        [TestMethod]
        public void LoadResults_Returns_Result_Details_For_Item_And_Enrollment()
        {
            userActions.LoadUserProfile().Returns(new UserProfileResponse()
            {
                UserProfile = new List<UserProfile>() 
                { 
                    new UserProfile()
                    {
                        
                    }
                }
            });
            enrollmentActions.GetEntityEnrollments(context.EntityId).Returns(
            new List<Enrollment>() 
            { 
                new Enrollment()
                {
                    Id = "enrollmentId",
                    User = new UserInfo()
                    {
                    
                    }
                }
            });
            contentActions.ListDescendents(context.EntityId, "", "").ReturnsForAnyArgs(new List<Agilix.DataContracts.Item>() 
            { 
                new Agilix.DataContracts.Item()
                {
                    Id = "itemId"
                }
            });

            var result = actions.LoadResults("itemId", "enrollmentId");

            Assert.AreEqual("itemId", result.First().ItemsIds.First());
            Assert.AreEqual("itemId", result.First().ResultItems.Id);
        }

    }
}
