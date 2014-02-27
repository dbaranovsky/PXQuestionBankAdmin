using System;
using System.Collections.Generic;
using System.Linq;
using Bfw.PX.Biz.ServiceContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using TestHelper;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.Common.Collections;

namespace Bfw.PX.Biz.Direct.Services.Tests
{
    [TestClass]
    public class NavigationActionsTest
    {
        private IBusinessContext context;
        private IContentActions contentActions;
        private IItemQueryActions itemQueryActions;

        private NavigationActions navigationActions;

        [TestInitialize]
        public void TestInitialize()
        { 
            context = Substitute.For<IBusinessContext>();
            contentActions = Substitute.For<IContentActions>();
            itemQueryActions = Substitute.For<IItemQueryActions>();

            navigationActions = new NavigationActions(context, contentActions, itemQueryActions);
        }

        [TestMethod]
        public void GetNavigation_Should_Return_Menu_Items()
        {
            var assetLink = new Agilix.DataContracts.Item(){ Type = Bfw.Agilix.Dlap.DlapItemType.AssetLink };
            var customActivity = new Agilix.DataContracts.Item(){ Type = Bfw.Agilix.Dlap.DlapItemType.CustomActivity };
            var links = new List<Agilix.DataContracts.Item>()
            {
                 assetLink,
                 customActivity
            };
            var folder = new Agilix.DataContracts.Item(){ Type = Bfw.Agilix.Dlap.DlapItemType.Folder };
            contentActions.ListChildren("siteId", "navigationId", "categoryId", "").Returns(new List<Agilix.DataContracts.Item>() 
            { 
                new Agilix.DataContracts.Item()
                {
                    Children = new List<Agilix.DataContracts.Item>()
                    {
                        assetLink,
                        customActivity,                        
                        folder
                    }
                }
            });
            
            var result = navigationActions.GetNavigation("siteId", "navigationId", "categoryId");
            
            Assert.IsTrue(ObjectComparer.AreObjectsEqual(folder.ToMenuItem(), result.Children.First()));
            Assert.IsTrue(ObjectComparer.AreObjectsEqual(links.Map(o => o.ToContentItem()), result.Links));
        }

        [TestMethod]
        public void LoadNavigation_Should_Return_Menu_Items()
        {
            var folder = new Agilix.DataContracts.Item() { Type = Bfw.Agilix.Dlap.DlapItemType.Folder };
            contentActions.ListChildren("siteId", "navigationId", "categoryId", "").Returns(new List<Agilix.DataContracts.Item>() 
            { 
                new Agilix.DataContracts.Item()
                {
                    Children = new List<Agilix.DataContracts.Item>()
                    {
                        folder
                    }
                }
            });

            var result = navigationActions.LoadNavigation("siteId", "navigationId", "categoryId");

            Assert.IsTrue(ObjectComparer.AreObjectsEqual(folder.ToMenuItem(), result.Children.First()));
        }

        [TestMethod]
        public void LoadNavigation_Should_Return_Menu_Items_For_Ebook()
        {
            var folder = new Agilix.DataContracts.Item() { Type = Bfw.Agilix.Dlap.DlapItemType.Folder };
            contentActions.GetContent(context.EntityId, "navigationId").ReturnsForAnyArgs(new DataContracts.ContentItem());
            contentActions.ListChildren("siteId", "navigationId", "ebook", "").ReturnsForAnyArgs(new List<Agilix.DataContracts.Item>() 
            { 
                new Agilix.DataContracts.Item()
                {
                    Children = new List<Agilix.DataContracts.Item>()
                    {
                        folder
                    }
                }
            });

            var result = navigationActions.LoadNavigation("siteId", "navigationId", "ebook");

            Assert.IsTrue(ObjectComparer.AreObjectsEqual(folder.ToMenuItem(), result.Children.First()));
        }

        [TestMethod]
        public void LoadNavigation_Should_Return_Menu_Items_For_MyMaterials()
        {
            var assetLink = new Agilix.DataContracts.Item() { Type = Bfw.Agilix.Dlap.DlapItemType.AssetLink };
            var folder = new Agilix.DataContracts.Item() { Type = Bfw.Agilix.Dlap.DlapItemType.Folder };
            contentActions.GetContent(context.EntityId, "navigationId").ReturnsForAnyArgs(new DataContracts.ContentItem());
            contentActions.ListChildren("siteId", "navigationId", System.Configuration.ConfigurationManager.AppSettings["MyMaterials"], "").ReturnsForAnyArgs(new List<Agilix.DataContracts.Item>() 
            { 
                new Agilix.DataContracts.Item()
                {
                    Children = new List<Agilix.DataContracts.Item>()
                    {
                        folder,
                        assetLink
                    }
                }
            });

            var result = navigationActions.LoadNavigation("siteId", "navigationId", System.Configuration.ConfigurationManager.AppSettings["MyMaterials"]);

            Assert.IsTrue(ObjectComparer.AreObjectsEqual(assetLink.ToMenuItem(), result.Children.First()));
        }
    }
}
