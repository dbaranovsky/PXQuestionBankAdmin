using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Linq;

namespace Bfw.PX.Biz.DataContracts.Test
{
    [TestClass]
    public class ContentItemTest
    {
        [TestMethod]
        public void SetVisibility_Should_Save_Date_AsUniversalTime()
        {
            var contentItem = new Biz.DataContracts.ContentItem()
            {
                Id = "2"
            };
            var date = new DateTime(2023, 1, 2, 3, 5, 40);
            var expected = "2023-01-02T08:05:40Z";

            contentItem.SetVisibility("restrictedbydate", date);

            XmlDocument element = new XmlDocument();
            element.LoadXml(contentItem.Visibility);
            var actual = element.GetElementsByTagName("date").Item(0).Attributes["endate"].Value;

            Assert.AreEqual(expected, actual);
        }

        #region Categories

        /// <summary>
        ///AddCategoryToItem sets sequence, category id, parentid
        ///</summary>
        [TestMethod()]
        public void AddCategoryToItem_Adds_Category()
        {
            ContentItem item = new ContentItem()
            {
                Id = "itemid",
                Categories = new List<TocCategory>()
                {
                    new TocCategory(){Id="cat1", ItemParentId = "parentid", Sequence = "a"}
                }
            };
            const string category = "cat2";
            const string itemParentId = "parent2";
            const string sequence = "b";
            item.AddCategoryToItem(category, itemParentId, sequence);
            Assert.AreEqual(item.Categories.Count, 2);
            Assert.AreEqual(item.Categories[1].Id, "cat2");
            Assert.AreEqual(item.Categories[1].ItemParentId, "parent2");
            Assert.AreEqual(item.Categories[1].Sequence, "b");
        }

        [TestCategory("ContentActions"), TestMethod()]
        public void SetSyllabusFilterCategory_Should_Add_Category_If_Valid_Toc_Is_Passed()
        {
            const string toc = "MYFILTER";
            const string parentId = "This is a parent Id";

            var item = new ContentItem()
            {
                ParentId = parentId,
                Sequence = "a"
            };

            item.SetSyllabusFilterCategory(item.ParentId, toc, item.Sequence);

            Assert.IsTrue(item.Categories.Any());
            Assert.AreEqual(item.Categories.FirstOrDefault(c => c.Id == toc).Text, toc);
        }

        [TestCategory("ContentActions"), TestMethod()]
        public void SetSyllabusFilterCategory_Should_Not_Add_Category_If_Invalid_Toc_Is_Passed()
        {
            const string parentId = "This is a parent Id";

            var item = new ContentItem()
            {
                ParentId = parentId,
                Sequence = "a"
            };

            item.SetSyllabusFilterCategory(item.ParentId, null, item.Sequence);

            Assert.IsNull(item.Categories);
        }

        [TestCategory("ContentActions"), TestMethod()]
        public void SetSyllabusFilterCategory_Should_Not_Add_Category_If_ParentId_Is_Null()
        {
            const string toc1 = "MYFILTER";
            const string toc2 = "YOURFILTER";
            const string toc3 = "HAHAHA";

            var toc = String.Join(",", toc1, toc2, toc3);

            var item = new ContentItem();

            item.SetSyllabusFilterCategory(item.ParentId, toc, item.Sequence);

            Assert.IsNull(item.Categories);
        }

        #endregion

        #region Containers and SubContainers

        #region GetContainer

        [TestMethod]
        public void GetContainer_Should_Return_Value_If_Toc_Exists()
        {
            const string toc = "assignmentfilter";
            const string tocVal = "ASSIGNMENT";

            var item = new ContentItem()
            {
                Containers = new List<Container>()
                {
                    new Container(toc, tocVal)
                }
            };

            var val = item.GetContainer(toc);

            Assert.AreEqual(val, tocVal);
        }

        [TestMethod]
        public void GetContainer_Should_Return_Null_If_Toc_Does_not_Exist()
        {
            const string toc = "assignmentfilter";
            const string tocVal = "ASSIGNMENT";

            var item = new ContentItem()
            {
                Containers = new List<Container>()
                {
                    new Container(toc, tocVal)
                }
            };

            var val = item.GetContainer("testfilter");

            Assert.IsNull(val);
        }

        #endregion
        
        #region GetSubContainer

        [TestMethod]
        public void GetSubContainer_Should_Return_Value_If_Toc_Exists()
        {
            const string toc = "assignmentfilter";
            const string tocVal = "ASSIGNMENT";

            var item = new ContentItem()
            {
                SubContainerIds = new List<Container>()
                {
                    new Container(toc, tocVal)
                }
            };

            var val = item.GetSubContainer(toc);

            Assert.AreEqual(val, tocVal);
        }

        [TestMethod]
        public void GetSubContainer_Should_Return_Null_If_Toc_Does_not_Exist()
        {
            const string toc = "assignmentfilter";
            const string tocVal = "ASSIGNMENT";

            var item = new ContentItem()
            {
                SubContainerIds = new List<Container>()
                {
                    new Container(toc, tocVal)
                }
            };

            var val = item.GetSubContainer("testfilter");

            Assert.IsNull(val);
        }

        #endregion
        
        #region SetContainer
        
        [TestMethod]
        public void SetContainer_Should_Set_Toc_If_Valid()
        {
            const string toc = "assignmentfilter";
            const string tocVal = "ASSIGNMENT";

            var item = new ContentItem();

            item.SetContainer(tocVal, toc);

            Assert.IsTrue(item.Containers.Any());
            Assert.AreEqual(item.Containers.FirstOrDefault(x => x.Toc == toc).Value, tocVal);
        }

        [TestMethod]
        public void SetContainer_Should_Not_Set_Duplicate_Tocs()
        {
            const string toc = "assignmentfilter";
            const string tocVal = "ASSIGNMENT";
            const string tocNewVal = "NewTest";

            var item = new ContentItem();

            item.SetContainer(tocVal, toc);
            item.SetContainer(tocVal, toc);
            item.SetContainer(tocNewVal, toc);

            Assert.IsTrue(item.Containers.Count == 1);
            Assert.AreEqual(item.Containers.FirstOrDefault(x => x.Toc == toc).Value, tocNewVal);
        }

        [TestMethod]
        public void SetContainer_Should_Not_Set_Toc_If_Passed_Toc_Is_Empty()
        {
            const string tocVal = "ASSIGNMENT";

            var item = new ContentItem();

            item.SetContainer(tocVal, String.Empty);

            Assert.IsFalse(item.Containers.Any());
        }

        [TestMethod]
        public void SetContainer_Should_Not_Set_Toc_If_Passed_Toc_Is_Null()
        {
            const string tocVal = "ASSIGNMENT";

            var item = new ContentItem();

            item.SetContainer(tocVal, null);

            Assert.IsFalse(item.Containers.Any());
        }

        #endregion

        #region SetSubContainer

        [TestMethod]
        public void SetSubContainer_Should_Set_Toc_If_Valid()
        {
            const string toc = "assignmentfilter";
            const string tocVal = "ASSIGNMENT";

            var item = new ContentItem();

            item.SetSubContainer(tocVal, toc);

            Assert.IsTrue(item.SubContainerIds.Any());
            Assert.AreEqual(item.SubContainerIds.FirstOrDefault(x => x.Toc == toc).Value, tocVal);
        }

        [TestMethod]
        public void SetSubContainer_Should_Not_Set_Duplicate_Tocs()
        {
            const string toc = "assignmentfilter";
            const string tocVal = "ASSIGNMENT";
            const string tocNewVal = "NewTest";

            var item = new ContentItem();

            item.SetSubContainer(tocVal, toc);
            item.SetSubContainer(tocVal, toc);
            item.SetSubContainer(tocNewVal, toc);

            Assert.IsTrue(item.SubContainerIds.Count == 1);
            Assert.AreEqual(item.SubContainerIds.FirstOrDefault(x => x.Toc == toc).Value, tocNewVal);
        }

        [TestMethod]
        public void SetSubContainer_Should_Not_Set_Toc_If_Passed_Toc_Is_Empty()
        {
            const string tocVal = "ASSIGNMENT";

            var item = new ContentItem();

            item.SetSubContainer(tocVal, String.Empty);

            Assert.IsFalse(item.SubContainerIds.Any());
        }

        [TestMethod]
        public void SetSubContainer_Should_Not_Set_Toc_If_Passed_Toc_Is_Null()
        {
            const string tocVal = "ASSIGNMENT";

            var item = new ContentItem();

            item.SetSubContainer(tocVal, null);

            Assert.IsFalse(item.SubContainerIds.Any());
        }

        #endregion

        #endregion
    }
}
