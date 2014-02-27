using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using Bfw.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

using Bfw.PX.Biz.ServiceContracts;
using PxDC = Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Controllers.Contracts;
using Bfw.PX.PXPub.Controllers.Helpers;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.PX.PXPub.Models;
using TestHelper;
using Helper = TestHelper.Helper;

namespace Bfw.PX.PXPub.Controllers.Tests.Helpers
{
    [TestClass]
    public class AssignmentCenterHelperTest
    {
        private AssignmentCenterHelper helper;

        private IBusinessContext context;
        private IContentActions contentActions;
        private IContentHelper contentHelper;
        private IAssignmentActions assignmentActions;
        private ICourseActions courseActions;
        private IGradeActions gradeActions;
        private IEnrollmentActions enrollmentActions;
        private IRubricActions rubricActions;
        private string _entityid;
        private string _removedValue;
        private string _defaultContainerToc;

        [TestInitialize]
        public void TestInitialize()
        {
            _entityid = "testid";
            _removedValue = "removed";
            _defaultContainerToc = "syllabusfilter";

            context = Substitute.For<IBusinessContext>();
            contentActions = Substitute.For<IContentActions>();
            contentHelper = Substitute.For<IContentHelper>();
            assignmentActions = Substitute.For<IAssignmentActions>();
            courseActions = Substitute.For<ICourseActions>();
            gradeActions = Substitute.For<IGradeActions>();
            enrollmentActions = Substitute.For<IEnrollmentActions>();
            rubricActions = Substitute.For<IRubricActions>();

            var course = new Bfw.Agilix.DataContracts.Course();
            course.ParseEntity(Helper.GetResponse(Entity.Course, "GenericCourse").Element("course"));
            course.Id = "1";
            context.Course = course.ToCourse();
            context.EntityId.Returns(_entityid);

            ConfigurationManager.AppSettings.Set("FaceplateRemoved", _removedValue);

            helper = new AssignmentCenterHelper(contentActions, context, contentHelper, assignmentActions,
                courseActions, gradeActions, enrollmentActions, rubricActions);
        }

        [TestCategory("AssignmentCenterHelper"), TestMethod]
        public void ItemOperation_Should_Update_Parent()
        {
            context.Course.CourseTimeZone = "Eastern Standard Time";

            DateTime assignmentDate = DateTime.Now;
            var item = SetAssignedItem(assignmentDate);
            var contentItem = SetContentItem(item);
            var entityId = "1";
            var parent = SetParentContentItem();
            var toc = "syllabusfilter";
            var parents = new List<Biz.DataContracts.ContentItem>() 
            { 
                parent
            };

            contentActions.GetContent(entityId, item.Id).Returns(contentItem);
            contentHelper.GetParentHeirachy(item.Id, TreeCategoryType.ManagementCard, toc, entityId).Returns(parents);
            contentActions.ListContentWithDueDates(entityId, null, "_1", toc).Returns(new List<Biz.DataContracts.ContentItem>()
            {
                contentItem
            });

            var result = helper.ItemOperation(item.Id, "2", item, Models.AssignmentCenterOperation.DatesAssigned, true, toc: toc, entityId: entityId);

            Assert.IsTrue(result[0].Id == parent.Id && result[0].EndDate == assignmentDate);
        }

        [TestCategory("AssignmentCenterHelper"), TestMethod]
        public void ItemOperation_AssignDatesToItem_Should_Not_Update_Points_If_It_Comes_In_Null()
        {
            var defaultPoints = 11.0;
            context.Course.CourseTimeZone = "Eastern Standard Time";

            DateTime assignmentDate = DateTime.Now.AddDays(15);
            var item = SetAssignedItem(assignmentDate);
            var contentItem = SetContentItem(item, defaultPoints);
            var entityId = "1";
            var targetId = "2";
            var toc = "syllabusfilter";
            var parent = SetParentContentItem();
            var parents = new List<Biz.DataContracts.ContentItem>() { parent };

            contentActions.GetContent(entityId, item.Id).Returns(contentItem);
            contentHelper.GetParentHeirachy(item.Id, TreeCategoryType.ManagementCard, toc, entityId).Returns(parents);
            contentActions.ListContentWithDueDates(entityId, null, "_1", toc).Returns(new List<Biz.DataContracts.ContentItem>()
            {
                contentItem
            });

            helper.ItemOperation(item.Id, targetId, item, Models.AssignmentCenterOperation.DatesAssigned, true, toc: toc, entityId: entityId);

            contentActions.Received()
                .UpdateAssignmentCenterItems(String.Empty, 
                    Arg.Is<IEnumerable<Biz.DataContracts.AssignmentCenterItem>>(x => x.FirstOrDefault().DefaultPoints == defaultPoints), 
                    toc, entityId);
        }

        [TestCategory("AssignmentCenterHelper"), TestMethod]
        public void SaveNavigationState_Should_Update_Parent()
        {
            var entityId = "1";
            var assignmentDate = DateTime.Now;
            var item = SetAssignedItem(assignmentDate);
            var contentItem = SetContentItem(item);
            var assignedCenterItem = SetAssignmentCenterItem(item, assignmentDate);
            var parent = SetParentAssignmentCenterItem();
            parent.Children = new List<AssignmentCenterItem>()
            {
                assignedCenterItem
            };
            var assignmentCenterCategory = new AssignmentCenterCategory()
            {
                Items = new List<AssignmentCenterItem>()
                 {
                     assignedCenterItem, parent
                 }
            };
            AssignmentCenterNavigationState state = new AssignmentCenterNavigationState()
            {
                Operation = AssignmentCenterOperation.DatesAssigned,
                EntityId = entityId,
                Changed = assignedCenterItem,
                Category = assignmentCenterCategory
            };

            contentActions.GetContent(entityId, item.Id).Returns(SetContentItem(item));
            gradeActions.GetGradeBookWeights(context.CourseId).Returns(new Biz.DataContracts.GradeBookWeights()
            {
                GradeWeightCategories = new List<Biz.DataContracts.GradeBookWeightCategory>() 
                { 
                   new Biz.DataContracts.GradeBookWeightCategory() 
                   { 
                       Id = "-1",
                       Items = new List<Biz.DataContracts.ContentItem>()
                       {
                           contentItem
                       }
                   }
                }
            });

            var result = helper.SaveNavigationState(state, _defaultContainerToc);

            Assert.IsTrue(result.Changes[1].Id == parent.Id && result.Changes[1].EndDate == assignmentDate);
        }

        /// <summary>
        /// When course type is not FLACEPLATE, changing gradebook category, or assigning due date should not update its sequence.
        /// </summary>
        [TestCategory("AssignmentCenterHelper"), TestMethod]
        public void SaveNavigationState_CourseNotFacePlate_ChangeCategoryOnAssignedItem_ExpectNotSequenceUpdated()
        {
            // Create test data
            const string entityId = "entityId";
            context.EntityId.Returns(entityId);
            context.Sequence("h", "").Returns("i");
            context.Course.CourseType = CourseType.XBOOK.ToString();

            var assignmentDate = DateTime.MinValue;
            var assigningItem = SetAssignedItem(assignmentDate);
            var contentItem = SetContentItem(assigningItem);
            var assignedCenterItem = SetAssignmentCenterItem(assigningItem, assignmentDate);
            assignedCenterItem.CategorySequence = "a";
            assignedCenterItem.GradebookCategory = "-1";
            var parent = SetParentAssignmentCenterItem();
            var contentItemAlreadyInGradebook = new PX.Biz.DataContracts.ContentItem { AssignmentSettings = new PxDC.AssignmentSettings { CategorySequence = "h" } };
            contentItem.AssignmentSettings.CategorySequence = "a";
            contentItem.AssignmentSettings.Category = "0";
            parent.Children = new List<AssignmentCenterItem>()
            {
                assignedCenterItem
            };
            var assignmentCenterCategory = new AssignmentCenterCategory()
            {
                Items = new List<AssignmentCenterItem>()
                 {
                     assignedCenterItem, parent
                 }
            };
            AssignmentCenterNavigationState state = new AssignmentCenterNavigationState()
            {
                Operation = AssignmentCenterOperation.DatesAssigned,
                EntityId = entityId,
                Changed = assignedCenterItem,
                Category = assignmentCenterCategory
            };

            // Subtitle methods
            contentActions.GetContent(entityId, assigningItem.Id).Returns(contentItem);
            contentActions.GetItems(entityId, Arg.Any<List<string>>()).Returns(new List<PX.Biz.DataContracts.ContentItem> { contentItemAlreadyInGradebook });

            gradeActions.GetGradeBookWeights(context.CourseId).Returns(new Biz.DataContracts.GradeBookWeights()
            {
                GradeWeightCategories = new List<Biz.DataContracts.GradeBookWeightCategory>() 
                { 
                   new Biz.DataContracts.GradeBookWeightCategory() 
                   { 
                       Id = "-1",
                       Items = new List<Biz.DataContracts.ContentItem>()
                       {
                           contentItemAlreadyInGradebook
                       }
                   }
                }
            });

            // Call test method
            var result = helper.SaveNavigationState(state, _defaultContainerToc);

            // Assert
            Assert.IsTrue(result.Changed.CategorySequence == "a");
        }

        /// <summary>
        /// When changing gradebook category on an assigned item, expect to update its sequence.
        /// </summary>
        [TestCategory("AssignmentCenterHelper"), TestMethod]
        public void SaveNavigationState_ChangeCategoryOnAssignedItem_ExpectSequenceUpdated()
        {
            // Create test data
            const string entityId = "entityId";
            context.EntityId.Returns(entityId);
            context.Sequence("h", "").Returns("i");

            var assignmentDate = DateTime.Now;
            var assigningItem = SetAssignedItem(assignmentDate);
            var contentItem = SetContentItem(assigningItem);
            var assignedCenterItem = SetAssignmentCenterItem(assigningItem, assignmentDate);
            assignedCenterItem.CategorySequence = "a";
            assignedCenterItem.GradebookCategory = "-1";
            var parent = SetParentAssignmentCenterItem();
            var contentItemAlreadyInGradebook = new PX.Biz.DataContracts.ContentItem { AssignmentSettings = new PxDC.AssignmentSettings { CategorySequence = "h" } };
            contentItem.AssignmentSettings.CategorySequence = "a";
            contentItem.AssignmentSettings.Category = "0";
            parent.Children = new List<AssignmentCenterItem>()
            {
                assignedCenterItem
            };
            var assignmentCenterCategory = new AssignmentCenterCategory()
            {
                Items = new List<AssignmentCenterItem>()
                 {
                     assignedCenterItem, parent
                 }
            };
            AssignmentCenterNavigationState state = new AssignmentCenterNavigationState()
            {
                Operation = AssignmentCenterOperation.DatesAssigned,
                EntityId = entityId,
                Changed = assignedCenterItem,
                Category = assignmentCenterCategory
            };

            // Subtitle methods
            contentActions.GetContent(entityId, assigningItem.Id).Returns(contentItem);
            contentActions.GetItems(entityId, Arg.Any<List<string>>()).Returns(new List<PX.Biz.DataContracts.ContentItem> { contentItemAlreadyInGradebook });

            gradeActions.GetGradeBookWeights(context.CourseId).Returns(new Biz.DataContracts.GradeBookWeights()
            {
                GradeWeightCategories = new List<Biz.DataContracts.GradeBookWeightCategory>() 
                { 
                   new Biz.DataContracts.GradeBookWeightCategory() 
                   { 
                       Id = "-1",
                       Items = new List<Biz.DataContracts.ContentItem>()
                       {
                           contentItemAlreadyInGradebook
                       }
                   }
                }
            });

            // Call test method
            var result = helper.SaveNavigationState(state, _defaultContainerToc);

            // Assert
            Assert.IsTrue(result.Changed.CategorySequence == "i");
        }

        /// <summary>
        /// When changing due date on an assigned item, expect not to update its sequence.
        /// </summary>
        [TestCategory("AssignmentCenterHelper"), TestMethod]
        public void SaveNavigationState_AssignDueDateToAssignedItem_ExpectSequenceNotUpdated()
        {
            // Create test data
            const string entityId = "entityId";
            context.EntityId.Returns(entityId);
            context.Sequence("h", "").Returns("i");

            var assignmentDate = DateTime.Now;
            var assigningItem = SetAssignedItem(assignmentDate);
            var contentItem = SetContentItem(assigningItem);
            var assignedCenterItem = SetAssignmentCenterItem(assigningItem, assignmentDate);
            assignedCenterItem.CategorySequence = "a";
            assignedCenterItem.GradebookCategory = "-1";
            var parent = SetParentAssignmentCenterItem();
            var contentItemAlreadyInGradebook = new PX.Biz.DataContracts.ContentItem { AssignmentSettings = new PxDC.AssignmentSettings { CategorySequence = "h" } };
            contentItem.AssignmentSettings.CategorySequence = "a";
            contentItem.AssignmentSettings.Category = "-1";
            parent.Children = new List<AssignmentCenterItem>()
            {
                assignedCenterItem
            };
            var assignmentCenterCategory = new AssignmentCenterCategory()
            {
                Items = new List<AssignmentCenterItem>()
                 {
                     assignedCenterItem, parent
                 }
            };
            AssignmentCenterNavigationState state = new AssignmentCenterNavigationState()
            {
                Operation = AssignmentCenterOperation.DatesAssigned,
                EntityId = entityId,
                Changed = assignedCenterItem,
                Category = assignmentCenterCategory
            };

            // Subtitle methods
            contentActions.GetContent(entityId, assigningItem.Id).Returns(contentItem);
            contentActions.GetItems(entityId, Arg.Any<List<string>>()).Returns(new List<PX.Biz.DataContracts.ContentItem> { contentItemAlreadyInGradebook });

            gradeActions.GetGradeBookWeights(context.CourseId).Returns(new Biz.DataContracts.GradeBookWeights()
            {
                GradeWeightCategories = new List<Biz.DataContracts.GradeBookWeightCategory>() 
                { 
                   new Biz.DataContracts.GradeBookWeightCategory() 
                   { 
                       Id = "-1",
                       Items = new List<Biz.DataContracts.ContentItem>()
                       {
                           contentItemAlreadyInGradebook
                       }
                   }
                }
            });

            // Call test method
            var result = helper.SaveNavigationState(state, _defaultContainerToc);

            // Assert
            Assert.IsTrue(result.Changed.CategorySequence == "a");
        }

        /// <summary>
        /// When assigning a due date to unassigned item, expect to update its sequence so it appears at the end of the category in gradebook.
        /// </summary>
        [TestCategory("AssignmentCenterHelper"), TestMethod]
        public void SaveNavigationState_AssignDueDateToUnassignedItem_ExpectSequenceUpdated()
        {
            var stubSequence = "h";
            var gradebookCategoryId = "-1";

            // Create test data
            const string entityId = "entityId";
            context.EntityId.Returns(entityId);
            context.Sequence(stubSequence, "").Returns("i");

            var assignmentDate = DateTime.MinValue;
            var assigningItem = SetAssignedItem(assignmentDate);
            var contentItem = SetContentItem(assigningItem);
            var assignedCenterItem = SetAssignmentCenterItem(assigningItem, assignmentDate);
            assignedCenterItem.GradebookCategory = gradebookCategoryId;

            var parent = SetParentAssignmentCenterItem();
            var contentItemAlreadyInGradebook = new PX.Biz.DataContracts.ContentItem { AssignmentSettings = new PxDC.AssignmentSettings{CategorySequence = stubSequence} };
            contentItem.AssignmentSettings.CategorySequence = "a";
            contentItem.AssignmentSettings.Category = gradebookCategoryId;
            parent.Children = new List<AssignmentCenterItem>()
            {
                assignedCenterItem
            };
            var assignmentCenterCategory = new AssignmentCenterCategory()
            {
                Items = new List<AssignmentCenterItem>()
                 {
                     assignedCenterItem, parent
                 }
            };
            AssignmentCenterNavigationState state = new AssignmentCenterNavigationState()
            {
                Operation = AssignmentCenterOperation.DatesAssigned,
                EntityId = entityId,
                Changed = assignedCenterItem,
                Category = assignmentCenterCategory
            };

            // Subtitle methods
            contentActions.GetContent(entityId, assigningItem.Id).Returns(contentItem);
            contentActions.GetItems(entityId, Arg.Any<List<string>>()).Returns(new List<PX.Biz.DataContracts.ContentItem> { contentItemAlreadyInGradebook });

            gradeActions.GetGradeBookWeights(context.CourseId).Returns(new Biz.DataContracts.GradeBookWeights()
            {
                GradeWeightCategories = new List<Biz.DataContracts.GradeBookWeightCategory>() 
                { 
                   new Biz.DataContracts.GradeBookWeightCategory() 
                   { 
                       Id = gradebookCategoryId,
                       Items = new List<Biz.DataContracts.ContentItem>()
                       {
                           contentItemAlreadyInGradebook
                       }
                   }
                }
            });

            // Call test method
            var result = helper.SaveNavigationState(state, _defaultContainerToc);

            // Assert
            Assert.IsTrue(result.Changed.CategorySequence == "i");
        }

        [TestMethod]
        public void AssignmentCenterHelper_AddParentItem_Should_Add_AssignmentCenterItems_If_Exits()
        {

        }

        [TestMethod]
        public void AssignmentCenterHelper_UpdateUnitGradebookCategoryTo_Should_Create_New_GradebookCategory_If_None()
        {

        }

        /// <summary>
        /// ddGradeBookCategoryToCourse should add a new category to the course with an id +1 of the previous category id, and increment sequence
        /// </summary>
        [TestCategory("AssignmentCenterHelper"), TestMethod]
        public void AddGradeBookCategoryToCourse_Should_IncrementCategoryId_Sequence()
        {
            var categoryName = "new category";
            context.Course.GradeBookCategoryList = new List<PxDC.GradeBookWeightCategory>()
            {
                new PxDC.GradeBookWeightCategory() {Id = "", Sequence = "a", Text = "cat_broken1"},
                new PxDC.GradeBookWeightCategory() {Id = "aaa", Sequence = "", Text = "cat_broken2"},
                new PxDC.GradeBookWeightCategory() {Id = "5aa", Sequence = "5", Text = "cat_broken3"},
                new PxDC.GradeBookWeightCategory() {Id = "0", Sequence = "a", Text = "cat1"},
                new PxDC.GradeBookWeightCategory() {Id = "1", Sequence = "b", Text = "cat2"},
                new PxDC.GradeBookWeightCategory() {Id = "2", Sequence = "c", Text = "cat3"},
                new PxDC.GradeBookWeightCategory() {Id = "3", Sequence = "d", Text = "cat4"},
                new PxDC.GradeBookWeightCategory() {Id = "4", Sequence = "e", Text = "cat5"}
            };
            context.Sequence("e", string.Empty).Returns("f");

            helper.AddGradeBookCategoryToCourse(categoryName);

            Assert.AreEqual(context.Course.GradeBookCategoryList.Count, 9);
            var category = context.Course.GradeBookCategoryList.FirstOrDefault(c => c.Text == categoryName);
            Assert.IsNotNull(category);
            Assert.AreEqual(category.Sequence, "f");
            Assert.AreEqual(category.Id, "5");
        }

        [TestCategory("AssignmentCenterHelper"), TestMethod]
        public void AddGradeBookCategoryToCourse_Should_IncrementCategoryId_Sequence_Even_If_GradeBookCategoryList_Is_Empty()
        {
            var categoryName = "new category";
            context.Course.GradeBookCategoryList = new List<PxDC.GradeBookWeightCategory>();
            context.Sequence("a", string.Empty).Returns("a");

            helper.AddGradeBookCategoryToCourse(categoryName);

            Assert.AreEqual(context.Course.GradeBookCategoryList.Count, 1);
            var category = context.Course.GradeBookCategoryList.FirstOrDefault(c => c.Text == categoryName);
            Assert.IsNotNull(category);
            Assert.AreEqual(category.Sequence, "a");
            Assert.AreEqual(category.Id, "0");
        }

        /// <summary>
        /// The function should return all assigned items
        /// </summary>
        [TestCategory("AssignmentCenterHelper"), TestMethod]
        public void GetDueAssignmentsForInstructor_Should_Return_List_Of_Assignments()
        {
            contentActions.ListContentWithDueDatesWithinRange("", "", "").ReturnsForAnyArgs(GetDueItems());

            var result = helper.GetDueAssignmentsForInstructor("entityId", 7);

            Assert.IsTrue(result.Count() == 3);
        }

        [TestCategory("AssignmentCenterHelper"), TestMethod]
        public void View_Rendered_For_Student_NonDSTTimeZone()
        {
            context.EnrollmentId = "1";
            context.AccessLevel = AccessLevel.Student;
            context.Course.CourseTimeZone = "US Mountain Standard Time"; //Arizona Time, does not have a DST adjustment (vs Mountain Standard Time which does)
            var gradeList = new List<Biz.DataContracts.Grade>();

            bool getDueSoonCalled = false;
            gradeActions.GetDueSoonItemsWithGrades(null, context.EnrollmentId, -420, false, false, 15)
                .Returns(gradeList).AndDoes(ci => getDueSoonCalled = true);

            //act
            helper.GetDueAssignmentsForStudent(15, false, false);


            Assert.IsTrue(getDueSoonCalled);
        }

        [TestCategory("AssignmentCenterHelper"), TestMethod]
        public void View_Rendered_For_Student_DSTTimeZone()
        {
            context.EnrollmentId = "1";
            context.AccessLevel = AccessLevel.Student;
            context.Course.CourseTimeZone = "Mountain Standard Time"; //Mountain Standard Time, DOES have a DST adjustment
            var timeOffset =
                TimeZoneInfo.FindSystemTimeZoneById(context.Course.CourseTimeZone).IsDaylightSavingTime(DateTime.Now)
                    ? -420
                    : -360;
            var gradeList = new List<Biz.DataContracts.Grade>();

            bool getDueSoonCalled = false;
            gradeActions.GetDueSoonItemsWithGrades(null, context.EnrollmentId, timeOffset, false, false, 15)
                .ReturnsForAnyArgs(gradeList).AndDoes(ci => getDueSoonCalled = true);

            //act
            helper.GetDueAssignmentsForStudent(15, false, false);


            Assert.IsTrue(getDueSoonCalled);
        }

        /// <summary>
        /// The function should return assigned items for student
        /// </summary>
        [TestCategory("AssignmentCenterHelper"), TestMethod]
        public void GetDueAssignmentsForStudent_Should_Return_List_Of_Assignments()
        {
            context.AccessLevel = AccessLevel.Student;
            contentActions.Context.Returns(context);
            gradeActions.GetDueSoonItemsWithGrades(null, context.EnrollmentId, 0, false, false, 7).ReturnsForAnyArgs(GetDueGrades());
            contentActions.GetItems(context.EntityId, new List<string>()).ReturnsForAnyArgs(GetDueItems());

            var result = helper.GetDueAssignmentsForStudent(7, false, false);

            Assert.IsTrue(result.Count() == 2);
            Assert.AreEqual("1", result.First().Id);
            Assert.AreEqual("title 1", result.First().Title);
            Assert.AreEqual("subtitle 1", result.First().SubTitle);
            Assert.AreEqual("dropbox", result.First().Type);
            Assert.AreEqual(10, result.First().MaxPoints);
            Assert.AreEqual(new DateTime(2023, 2, 3), result.First().AssignmentSettings.DueDate);
            Assert.IsTrue(ObjectComparer.AreObjectsEqual(new Common.DateTimeWithZone(new DateTime(2023, 2, 3), TimeZoneInfo.Local, false), result.First().AssignmentSettings.DueDateTZ));
            Assert.AreEqual(10, result.First().AssignmentSettings.Points);
        }

        [TestCategory("AssignmentCenterHelper"), TestMethod]
        public void SaveNavigationState_DatesUnAssigned_With_No_UnitContainer_Toc_Should_Call_UnAssignItem()
        {
            // Arrange
            const string parentId = "AssignmentUnit_113333";
            const string itemId = "bi_112334";
            const string toc = "assignmentfilter";
            var duedate = DateTime.Now.AddDays(10);
            const string sequence = "aaa";
            const string unitSequence = "a";
            const string folderType = "pxunit";
            const string tocParentId = "tocParent1111";
            const string unitContainerValue = "containerxxx";

            var contentItem = new AssignmentCenterItem()
            {
                Id = itemId,
                StartDate = DateTime.MinValue,
                EndDate = DateTime.MinValue,
                Sequence = sequence,
                ParentId = tocParentId
            };

            var state = new AssignmentCenterNavigationState()
            {
                Above = null,
                Below = null,
                Category = new AssignmentCenterCategory() { Items = new List<AssignmentCenterItem>() { contentItem } },
                Changed = contentItem,
                EntityId = _entityid,
                Operation = AssignmentCenterOperation.DatesUnAssigned,
                UnitContainer = { }
            };

            // Stub
            context.AccessLevel = AccessLevel.Instructor;

            var contentParentItem = new PxDC.ContentItem()
            {
                Id = parentId,
                ParentId = "PX_MULTIPART_LESSONS",
                Sequence = unitSequence,
                Type = folderType,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    DueDate = duedate,
                    DueDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                }
            };

            var contentItem1 = new PxDC.ContentItem()
            {
                Id = itemId,
                ParentId = parentId,
                Sequence = sequence,
                Type = folderType,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    DueDate = duedate,
                    DueDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                }
            };
            var unassignedContentItem1 = contentItem1;
            unassignedContentItem1.AssignmentSettings.StartDate = DateTime.MinValue;
            unassignedContentItem1.AssignmentSettings.DueDate = DateTime.MinValue;

            contentActions.GetContent(_entityid, parentId).Returns(contentParentItem);
            contentActions.GetContent(_entityid, itemId).Returns(contentItem1);

            context.Sequence(null, "").Returns(sequence);

            var parents = new List<PxDC.ContentItem> { contentParentItem };
            contentHelper.GetParentHeirachy(itemId, TreeCategoryType.ManagementCard, toc, _entityid).Returns(parents);

            var listContentWithDueDates = new List<PxDC.ContentItem>() { unassignedContentItem1 };
            contentActions.ListContentWithDueDates(_entityid, unitContainerValue, parentId, toc).Returns(listContentWithDueDates);

            // Act
            var changedState = helper.SaveNavigationState(state, toc);

            // Assert
            contentActions.Received().UnAssignAssignmentCenterItems(null, Arg.Any<IEnumerable<PxDC.AssignmentCenterItem>>(), toc, true);
       
            foreach (var item in changedState.Changes)
            {
                if (item.Id == parentId)
                    continue;

                /* due date */
                Assert.AreEqual(item.EndDate.Year, DateTime.MinValue.Year); // dlap item's due date is set to item's EndDate
                Assert.AreEqual(item.StartDate.Year, DateTime.MinValue.Year);
            }
        }

        #region Assignment Unit

        [TestCategory("AssignmentCenterHelper"), TestMethod]
        public void SaveNavigationState_MoveAndAssign_Should_Assign_Single_Item_into_Assignment_Unit()
        {
            // Arrange
            const string unitId = "AssignmentUnit_113333";
            const string itemId = "bi_112334";
            const string toc = "assignmentfilter";
            var duedate = DateTime.Now.AddDays(10);
            const string sequence = "aaa";
            const string unitSequence = "a";
            const string folderType = "pxunit";
            const string tocParentId = "tocParent1111";
            const double testPoints = 11.4;

            var contentItem = new AssignmentCenterItem()
            {
                Id = itemId,
                StartDate = duedate,
                EndDate = duedate,
                Sequence = sequence,
                ParentId = tocParentId,
                DefaultPoints = testPoints
            };

            var state = new AssignmentCenterNavigationState()
            {
                Above = null,
                Below = null,
                Category = new AssignmentCenterCategory() { },
                Changed = contentItem,
                EntityId = _entityid,
                Operation = AssignmentCenterOperation.MoveAndAssign,
                UnitContainer = new AssignmentUnitContainer()
                {
                    Toc = toc,
                    UnitId = unitId
                }
            };

            // Stub
            context.AccessLevel = AccessLevel.Instructor;
            var unitItem = new PxDC.ContentItem()
            {
                Id = unitId,
                ParentId = "PX_MULTIPART_LESSONS",
                Sequence = unitSequence,
                Type = folderType,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    DueDate = DateTime.MinValue,
                    DueDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                }
            };
            var contentItem1 = new PxDC.ContentItem()
            {
                Id = itemId,
                ParentId = unitId,
                Sequence = sequence,
                Type = folderType,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    DueDate = DateTime.MinValue,
                    DueDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                },
                SubContainerIds = new List<PxDC.Container>()
                {
                    new PxDC.Container(toc, "")
                }
            };

            contentActions.GetContent(_entityid, unitId).Returns(unitItem);
            contentActions.GetContent(_entityid, itemId).Returns(contentItem1);

            context.Sequence(null, "").Returns(sequence);

            var parents = new List<PxDC.ContentItem> { unitItem };
            contentHelper.GetParentHeirachy(itemId, TreeCategoryType.ManagementCard, toc, _entityid).Returns(parents);

            // Act
            var changedState = helper.SaveNavigationState(state, toc);

            // Assert
            contentActions.Received().UpdateAssignmentCenterItems(unitId, Arg.Any<IEnumerable<PxDC.AssignmentCenterItem>>(), toc, String.Empty);
            contentActions.Received().UpdateAssignmentCenterItems(String.Empty, Arg.Any<IEnumerable<PxDC.AssignmentCenterItem>>(), toc, _entityid);

            foreach (var item in changedState.Changes)
            {
                if (item.Id == unitId)
                    continue;

                /* containers */
                Assert.AreEqual(item.GetSubContainer(toc), unitId); // dlap item's container and subcontainer have given TOC. 
                // and dlap item's given TOC's value of subcontainerId is UnitContainer's UnitId

                /* due date */
                Assert.AreEqual(item.EndDate, duedate); // dlap item's due date is set to item's EndDate
                if (item.Type == folderType)
                {
                    Assert.AreEqual(item.StartDate, duedate);
                }

                /* sequence */
                Assert.AreEqual(item.Sequence, sequence); // dlap item's syllabus filter sequence remains the same

                /* points */
                Assert.AreEqual(item.Points, testPoints);
            }
        }

        [TestCategory("AssignmentCenterHelper"), TestMethod]
        public void SaveNavigationState_MoveAndAssign_Should_Assign_Parent_Item_into_Assignment_Unit()
        {
            // Arrange
            const string unitId = "AssignmentUnit_113333";
            const string itemId = "bi_112334";
            const string toc = "assignmentfilter";
            var duedate = DateTime.Now.AddDays(10);
            const string sequence = "bb";
            const string unitSequence = "a";
            const string folderType = "pxunit";
            const string tocParentId = "tocParent1111";
            const double testPoints = 40.0;

            var child1 = new AssignmentCenterItem()
            {
                Id = itemId + "_a",
                StartDate = duedate,
                EndDate = duedate,
                Sequence = "a",
                ParentId = itemId,
                Children = null
            };
            var child2a = new AssignmentCenterItem()
            {
                Id = itemId + "_b_2a",
                StartDate = duedate,
                EndDate = duedate,
                Sequence = "a",
                ParentId = itemId + "_b",
                Children = null
            };
            var child2 = new AssignmentCenterItem()
            {
                Id = itemId + "_b",
                StartDate = duedate,
                EndDate = duedate,
                Sequence = "b",
                ParentId = itemId,
                Children = new List<AssignmentCenterItem>() { child2a }
            };

            var contentItem = new AssignmentCenterItem()
            {
                Id = itemId,
                StartDate = duedate,
                EndDate = duedate,
                Sequence = sequence,
                ParentId = tocParentId,
                Children = new List<AssignmentCenterItem>() { child1, child2 },
                DefaultPoints = testPoints
            };

            var state = new AssignmentCenterNavigationState()
            {
                Above = null,
                Below = null,
                Category = new AssignmentCenterCategory() { },
                Changed = contentItem,
                EntityId = _entityid,
                Operation = AssignmentCenterOperation.MoveAndAssign,
                UnitContainer = new AssignmentUnitContainer()
                {
                    Toc = toc,
                    UnitId = unitId
                }
            };

            // Stub
            context.AccessLevel = AccessLevel.Instructor;
            var unitItem = new PxDC.ContentItem()
            {
                Id = unitId,
                ParentId = "PX_MULTIPART_LESSONS",
                Sequence = unitSequence,
                Type = folderType,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    DueDate = DateTime.MinValue,
                    DueDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                }
            };
            var contentItem1 = new PxDC.ContentItem()
            {
                Id = itemId,
                ParentId = unitId,
                Sequence = sequence,
                Type = folderType,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    DueDate = DateTime.MinValue,
                    DueDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                },
                SubContainerIds = new List<PxDC.Container>()
                {
                    new PxDC.Container(toc, "")
                }
            };

            var contentChild1 = new PxDC.ContentItem()
            {
                Id = itemId + "_a",
                Categories = new List<PxDC.TocCategory>()
                {
                    new PxDC.TocCategory()
                    {
                        Id = "cat1",
                        ItemParentId = itemId,
                        Sequence = "a"
                    }
                },
                Sequence = "a",
                Type = "",
                ParentId = itemId,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    DueDate = DateTime.MinValue,
                    DueDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                }
            };
            var contentChild2a = new PxDC.ContentItem()
            {
                Id = itemId + "_b_2a",
                Categories = new List<PxDC.TocCategory>()
                {
                    new PxDC.TocCategory()
                    {
                        Id = "cat1",
                        ItemParentId = itemId + "_b",
                        Sequence = "a"
                    }
                },
                Sequence = "a",
                Type = "",
                ParentId = itemId + "_b",
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    DueDate = DateTime.MinValue,
                    DueDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                }
            };
            var contentChild2 = new PxDC.ContentItem()
            {
                Id = itemId + "_b",
                Categories = new List<PxDC.TocCategory>()
                {
                    new PxDC.TocCategory()
                    {
                        Id = "cat1",
                        ItemParentId = itemId,
                        Sequence = "b"
                    }
                },
                Sequence = "b",
                Type = folderType,
                ParentId = itemId,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    DueDate = DateTime.MinValue,
                    DueDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                }
            };

            contentActions.GetContent(_entityid, unitId).Returns(unitItem);
            contentActions.GetContent(_entityid, itemId).Returns(contentItem1);

            var listChildren = new List<PxDC.ContentItem>() { contentChild1, contentChild2a, contentChild2 };
            contentActions.ListChildren(String.Empty, itemId, 1, "syllabusfilter").Returns(listChildren);
            context.Sequence(null, "").Returns(sequence);

            var parents = new List<PxDC.ContentItem> { unitItem };
            contentHelper.GetParentHeirachy(itemId, TreeCategoryType.ManagementCard, toc, _entityid).Returns(parents);

            // Act
            var changedState = helper.SaveNavigationState(state, toc);

            // Assert
            contentActions.Received().UpdateAssignmentCenterItems(unitId, Arg.Any<IEnumerable<PxDC.AssignmentCenterItem>>(), toc, String.Empty);
            contentActions.Received().UpdateAssignmentCenterItems(String.Empty, Arg.Any<IEnumerable<PxDC.AssignmentCenterItem>>(), toc, _entityid);

            Assert.IsTrue(changedState.Changes.Exists(x => x.Id == itemId + "_b_2a"));

            foreach (var item in changedState.Changes)
            {
                if (item.Id == unitId)
                    continue;

                /* containers */
                Assert.AreEqual(item.GetSubContainer(toc), unitId); // dlap item's container and subcontainer have given TOC. 
                // and dlap item's given TOC's value of subcontainerId is UnitContainer's UnitId

                /* due dates */
                Assert.AreEqual(item.EndDate, duedate); // dlap item's due date is set to item's EndDate
                if (item.Type == folderType) // if it is a folder/PxUnit, its start date is set to item's Start Date if the original date val is min
                {
                    Assert.AreEqual(item.StartDate, duedate);
                }

                /* sequence */
                if (item.Id != itemId)
                {
                    var mockedItem = listChildren.FirstOrDefault(c => c.Id == item.Id);
                    Assert.IsNotNull(mockedItem);
                    Assert.AreEqual(item.Sequence, mockedItem.Sequence);
                }
                else
                {
                    Assert.AreEqual(item.Sequence, sequence); // dlap item's syllabus filter sequence remains the same
                 
                    /* points */
                    Assert.AreEqual(item.Points, testPoints);
                }
            }
        }

        [TestCategory("AssignmentCenterHelper"), TestMethod]
        public void SaveNavigationState_MoveAndAssign_Should_Change_Assigned_Parents_DueDate()
        {
            /* Logic
             * hierachy: unit -> contentItem1 -> contentChild1 (childless), contentChild2 -> contentChild2a
             * unit and contentItem1 has old due date assigned to its structure 
             * contentChild2a's duedate is now changed.
             * 
             * Expected Result:
             * unit and contentItem1 will now have date range;
             * contentChild1 will remain the same;
             * contentChild2a's parent contentChild2 will have a new due date;
             */

            // Arrange
            const string unitId = "AssignmentUnit_113333";
            const string itemId = "bi_112334";
            const string testChildId = itemId + "_b_2a";
            const string toc = "assignmentfilter";
            var oldDuedate = DateTime.Now.AddDays(7);
            var duedate = DateTime.Now.AddDays(13);
            const string sequence = "bb";
            const string unitSequence = "a";
            const string folderType = "pxunit";
            const string unitContainerValue = "XbookV2";

            var child2a = new AssignmentCenterItem()
            {
                Id = itemId + "_b_2a",
                StartDate = DateTime.MinValue,
                EndDate = duedate,
                Sequence = "a",
                ParentId = itemId + "_b",
                Children = null
            };

            var state = new AssignmentCenterNavigationState()
            {
                Above = null,
                Below = null,
                Category = new AssignmentCenterCategory() { },
                Changed = child2a,
                EntityId = _entityid,
                Operation = AssignmentCenterOperation.MoveAndAssign,
                UnitContainer = new AssignmentUnitContainer()
                {
                    Toc = toc,
                    UnitId = unitId
                }
            };

            // Stub
            context.AccessLevel = AccessLevel.Instructor;
            context.Course.CourseTimeZone = "Eastern Standard Time";

            var unitContainer = new PxDC.Container(toc, unitContainerValue);
            var unitItem = new PxDC.ContentItem()
            {
                Id = unitId,
                ParentId = "PX_MULTIPART_LESSONS",
                Sequence = unitSequence,
                Type = folderType,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    StartDate = oldDuedate,
                    DueDate = oldDuedate,
                    StartDateTZ = new DateTimeWithZone(oldDuedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false),
                    DueDateTZ = new DateTimeWithZone(oldDuedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                },
                Containers = { unitContainer }
            };
            var contentItem1 = new PxDC.ContentItem()
            {
                Id = itemId,
                Categories = new List<PxDC.TocCategory>()
                {
                    new PxDC.TocCategory()
                    {
                        Id = toc,
                        ItemParentId = unitId,
                        Sequence = "a"
                    }
                },
                ParentId = unitId,
                Sequence = sequence,
                Type = folderType,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    StartDate = oldDuedate,
                    DueDate = oldDuedate,
                    StartDateTZ = new DateTimeWithZone(oldDuedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false),
                    DueDateTZ = new DateTimeWithZone(oldDuedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                }
            };

            var contentChild1 = new PxDC.ContentItem()
            {
                Id = itemId + "_a",
                Categories = new List<PxDC.TocCategory>()
                {
                    new PxDC.TocCategory()
                    {
                        Id = toc,
                        ItemParentId = itemId,
                        Sequence = "a"
                    }
                },
                Sequence = "a",
                Type = "",
                ParentId = itemId,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    StartDate = DateTime.MinValue,
                    DueDate = oldDuedate,
                    StartDateTZ = new DateTimeWithZone(DateTime.MinValue, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false),
                    DueDateTZ = new DateTimeWithZone(oldDuedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                }
            };
            var contentChild2a = new PxDC.ContentItem()
            {
                Id = testChildId,
                Categories = new List<PxDC.TocCategory>()
                {
                    new PxDC.TocCategory()
                    {
                        Id = toc,
                        ItemParentId = itemId + "_b",
                        Sequence = "a"
                    }
                },
                Sequence = "a",
                Type = "",
                ParentId = itemId + "_b",
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    StartDate = DateTime.MinValue,
                    DueDate = duedate,
                    StartDateTZ = new DateTimeWithZone(DateTime.MinValue, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false),
                    DueDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                }
            };
            var contentChild2 = new PxDC.ContentItem()
            {
                Id = itemId + "_b",
                Categories = new List<PxDC.TocCategory>()
                {
                    new PxDC.TocCategory()
                    {
                        Id = toc,
                        ItemParentId = itemId,
                        Sequence = "b"
                    }
                },
                Sequence = "b",
                Type = folderType,
                ParentId = itemId,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    StartDate = oldDuedate,
                    DueDate = oldDuedate,
                    StartDateTZ = new DateTimeWithZone(oldDuedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false),
                    DueDateTZ = new DateTimeWithZone(oldDuedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                }
            };

            contentActions.GetContent(_entityid, unitId).Returns(unitItem);
            contentActions.GetContent(_entityid, testChildId).Returns(contentChild2a);

            var parents = new List<PxDC.ContentItem> { contentChild2, contentItem1, unitItem };
            contentHelper.GetParentHeirachy(testChildId, TreeCategoryType.ManagementCard, toc, _entityid).Returns(parents);

            var listContentWithDueDates = new List<PxDC.ContentItem>() { contentItem1, contentChild1, contentChild2, contentChild2a };
            contentActions.ListContentWithDueDates(_entityid, unitContainerValue, unitId, toc).Returns(listContentWithDueDates);

            // Act
            var assignedParents = helper.ProcessAssignment(testChildId, state.Changed, false, toc, true,_entityid);

            // Assert
            contentActions.Received().UpdateAssignmentCenterItems(String.Empty, Arg.Any<IEnumerable<PxDC.AssignmentCenterItem>>(), toc, _entityid);

            Assert.IsTrue(assignedParents.Exists(x => x.Id == itemId));

            Assert.AreEqual(assignedParents.Find(p => p.Id == contentChild2.Id).StartDate, duedate);
            Assert.AreEqual(assignedParents.Find(p => p.Id == contentItem1.Id).StartDate, oldDuedate);
            foreach (var parent in assignedParents)
            {
                Assert.AreEqual(parent.EndDate, duedate); // dlap item's due date propergates up to top parent
            }
        }
        
        [TestCategory("AssignmentCenterHelper"), TestMethod]
        public void SaveNavigationState_RemoveOnUnassign_Should_Unassign_and_Remove_a_Parent_Item_and_its_Children()
        {
            /* Logic
             * hierachy: unit -> contentItem1 -> contentChild1 (childless), contentChild2 (has childA and childB)
             * 
             * Expected Result:
             * contentItem1 and its descendents: contentChild1, contentChild2, contentChild2's childA and childB will get unassigned
             * as well as their sub-container's value and bfw_toc's parentId  will be changed to FACEPLATE_REMOVED 
             */

            // Arrange
            const string unitId = "AssignmentUnit_AtoZ";
            const string itemId = "i_am_a_parent";
            const string testChild = "i_am_a_child";
            const string testChild1 = testChild + "_1";
            const string testChild2 = testChild + "_2";
            const string testChild2A = testChild + "_2_a";
            const string testChild2B = testChild + "_2_b";

            const string toc = "assignmentfilter";
            var duedate = DateTime.Now.AddDays(13);
            const string sequence = "bb";
            const string unitSequence = "a";
            const string folderType = "pxunit";
            const string unitContainerValue = "XbookV2";

            var child2A = new AssignmentCenterItem()
            {
                Id = testChild2A,
                StartDate = duedate,
                EndDate = duedate,
                Sequence = "a",
                ParentId = testChild2,
                Children = null
            };
            var child2B = new AssignmentCenterItem()
            {
                Id = testChild2B,
                StartDate = duedate,
                EndDate = duedate,
                Sequence = "b",
                ParentId = testChild2,
                Children = null
            };
            var child2 = new AssignmentCenterItem()
            {
                Id = testChild2,
                StartDate = duedate,
                EndDate = duedate,
                Sequence = "b",
                ParentId = itemId,
                Children = new List<AssignmentCenterItem>() { child2A, child2B }
            };
            var child1 = new AssignmentCenterItem()
            {
                Id = testChild1,
                StartDate = duedate,
                EndDate = duedate,
                Sequence = "a",
                ParentId = itemId,
                Children = null
            };
            var topItem = new AssignmentCenterItem()
            {
                Id = itemId,
                StartDate = duedate,
                EndDate = duedate,
                Sequence = "a",
                ParentId = unitId,
                Children = new List<AssignmentCenterItem>() { child1, child2 }
            };

            var state = new AssignmentCenterNavigationState()
            {
                Above = null,
                Below = null,
                Category = new AssignmentCenterCategory()
                {
                    Items = new List<AssignmentCenterItem>() { topItem }
                },
                Changed = topItem,
                EntityId = _entityid,
                Operation = AssignmentCenterOperation.RemoveOnUnassign,
                Toc = toc,
                RemoveFrom = toc
            };

            // Stub
            context.AccessLevel = AccessLevel.Instructor;
            context.Course.CourseTimeZone = "Eastern Standard Time";

            var unitContainer = new PxDC.Container("assignmentfilter", unitContainerValue);
            var unitSubContainer = new PxDC.Container("assignmentfilter", unitId);
            var unitItem = new PxDC.ContentItem()
            {
                Id = unitId,
                ParentId = "PX_MULTIPART_LESSONS",
                Sequence = unitSequence,
                Type = folderType,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    StartDate = duedate,
                    DueDate = duedate,
                    StartDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false),
                    DueDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                },
                Containers = { unitContainer }
            };

            var contentTopItem = new PxDC.ContentItem()
            {
                Id = itemId,
                Categories = new List<PxDC.TocCategory>()
                {
                    new PxDC.TocCategory()
                    {
                        Id = toc,
                        ItemParentId = unitId,
                        Sequence = "a"
                    }
                },
                Containers = { unitContainer },
                SubContainerIds = { unitSubContainer },
                ParentId = unitId,
                Sequence = sequence,
                Type = folderType,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    StartDate = duedate,
                    DueDate = duedate,
                    StartDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false),
                    DueDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                }
            };
            var contentChild1 = new PxDC.ContentItem()
            {
                Id = testChild1,
                Categories = new List<PxDC.TocCategory>()
                {
                    new PxDC.TocCategory()
                    {
                        Id = toc,
                        ItemParentId = itemId,
                        Sequence = "a"
                    }
                },
                Sequence = "a",
                Type = "",
                ParentId = itemId,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    StartDate = DateTime.MinValue,
                    DueDate = duedate,
                    StartDateTZ = new DateTimeWithZone(DateTime.MinValue, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false),
                    DueDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                }
            };
            var contentChild2 = new PxDC.ContentItem()
            {
                Id = testChild2,
                Categories = new List<PxDC.TocCategory>()
                {
                    new PxDC.TocCategory()
                    {
                        Id = toc,
                        ItemParentId = itemId,
                        Sequence = "b"
                    }
                },
                Sequence = "b",
                Type = folderType,
                ParentId = itemId,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    StartDate = duedate,
                    DueDate = duedate,
                    StartDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false),
                    DueDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                }
            };
            var contentChild2A = new PxDC.ContentItem()
            {
                Id = testChild2A,
                Categories = new List<PxDC.TocCategory>()
                {
                    new PxDC.TocCategory()
                    {
                        Id = toc,
                        ItemParentId = testChild2,
                        Sequence = "b"
                    }
                },
                Sequence = "b",
                Type = folderType,
                ParentId = testChild2,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    StartDate = duedate,
                    DueDate = duedate,
                    StartDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false),
                    DueDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                }
            };
            var contentChild2B = new PxDC.ContentItem()
            {
                Id = testChild2B,
                Categories = new List<PxDC.TocCategory>()
                {
                    new PxDC.TocCategory()
                    {
                        Id = toc,
                        ItemParentId = testChild2,
                        Sequence = "b"
                    }
                },
                Sequence = "b",
                Type = folderType,
                ParentId = testChild2,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    StartDate = duedate,
                    DueDate = duedate,
                    StartDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false),
                    DueDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                }
            };

            /* ProcessAssignments method's calls */
            contentActions.GetContent(_entityid, unitId).Returns(unitItem);
            contentActions.GetContent(_entityid, itemId).Returns(contentTopItem);

            var parents = new List<PxDC.ContentItem> { unitItem };
            contentHelper.GetParentHeirachy(itemId, TreeCategoryType.ManagementCard, toc, _entityid).Returns(parents);

            var listContentWithDueDates = new List<PxDC.ContentItem>() { contentTopItem, contentChild1, contentChild2, contentChild2A, contentChild2B };
            contentActions.ListContentWithDueDates(_entityid, unitContainerValue, unitId, toc).Returns(listContentWithDueDates);
            
            /* RemoveItem method's calls */
            var listContainerItems = new List<PxDC.ContentItem>() { contentTopItem, contentChild1, contentChild2, contentChild2A, contentChild2B };
            contentActions.GetContainerItemsForParent(_entityid, unitContainerValue, unitId, itemId, toc).Returns(listContainerItems);

            // Act
            var changes = helper.SaveNavigationState(state, toc).Changes;

            // Assert
            /* ProcessAssignments method's calls */
            contentActions.Received().UnAssignAssignmentCenterItems(String.Empty, Arg.Any<IEnumerable<PxDC.AssignmentCenterItem>>(), toc, false);
            /* RemoveItem method's calls */
            contentActions.Received().GetContainerItemsForParent(_entityid, unitContainerValue, unitId, itemId, toc);
            contentActions.Received().StoreContent(Arg.Is<PxDC.ContentItem>(x => RemoveOnUnassignCheckStoreContent(x, listContainerItems, toc)));

            Assert.IsTrue(changes.Exists(x => x.Id == itemId));

            foreach (var changedItem in changes)
            {
                if (changedItem.Id == unitId)
                {
                    Assert.AreEqual(changedItem.StartDate, duedate);
                    Assert.AreEqual(changedItem.EndDate, duedate);
                }
                else
                {
                    Assert.AreEqual(changedItem.StartDate.Year, DateTime.MinValue.Year);
                    Assert.AreEqual(changedItem.EndDate.Year, DateTime.MinValue.Year);
                }
            }
        }

        [TestCategory("AssignmentCenterHelper"), TestMethod]
        public void SaveNavigationState_RemoveOnUnassign_Should_Unassign_and_Remove_a_Single_Child_Item()
        {
            /* Logic
             * hierachy: unit -> contentItem1 -> contentChild1 (childless), contentChild2 (has childA and childB)
             * unit (duedate <-> duedate1)
             * contentItem (duedate <-> duedate1)
             * contentChild1 (duedate1)
             * contentChild2 (duedate <-> duedate1)
             * contentChild2A (duedate1)
             * contentChild2B (duedate)
             * 
             * Expected Result:
             * unit (duedate1)
             * contentItem (duedate1)
             * contentChild1 (duedate1)
             * contentChild2 (duedate1)
             * contentChild2A (duedate1)
             * childB is unassigned
             * as well as their sub-container's value and bfw_toc's parentId  will be changed to FACEPLATE_REMOVED 
             */

            // Arrange
            const string unitId = "AssignmentUnit_AtoZ";
            const string itemId = "i_am_a_parent";
            const string testChild = "i_am_a_child";
            const string testChild1 = testChild + "_1";
            const string testChild2 = testChild + "_2";
            const string testChild2A = testChild + "_2_a";
            const string testChild2B = testChild + "_2_b";

            const string toc = "assignmentfilter";
            var duedate = DateTime.Now.AddDays(13);
            var duedate1 = DateTime.Now.AddDays(18);
            const string sequence = "bb";
            const string unitSequence = "a";
            const string folderType = "pxunit";
            const string unitContainerValue = "XbookV2";

            var child2B = new AssignmentCenterItem()
            {
                Id = testChild2B,
                StartDate = duedate,
                EndDate = duedate,
                Sequence = "b",
                ParentId = testChild2,
                Children = null
            };

            var state = new AssignmentCenterNavigationState()
            {
                Above = null,
                Below = null,
                Category = new AssignmentCenterCategory()
                {
                    Items = new List<AssignmentCenterItem>() { child2B }
                },
                Changed = child2B,
                EntityId = _entityid,
                Operation = AssignmentCenterOperation.RemoveOnUnassign,
                Toc = toc,
                RemoveFrom = toc
            };

            // Stub
            context.AccessLevel = AccessLevel.Instructor;
            context.Course.CourseTimeZone = "Eastern Standard Time";

            var unitContainer = new PxDC.Container("assignmentfilter", unitContainerValue);
            var unitSubContainer = new PxDC.Container("assignmentfilter", unitId);
            var unitItem = new PxDC.ContentItem()
            {
                Id = unitId,
                ParentId = "PX_MULTIPART_LESSONS",
                Sequence = unitSequence,
                Type = folderType,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    StartDate = duedate,
                    DueDate = duedate1,
                    StartDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false),
                    DueDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                },
                Containers = { unitContainer }
            };

            var contentTopItem = new PxDC.ContentItem()
            {
                Id = itemId,
                Categories = new List<PxDC.TocCategory>()
                {
                    new PxDC.TocCategory()
                    {
                        Id = toc,
                        ItemParentId = unitId,
                        Sequence = "a"
                    }
                },
                Containers = { unitContainer },
                SubContainerIds = { unitSubContainer },
                ParentId = unitId,
                Sequence = sequence,
                Type = folderType,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    StartDate = duedate,
                    DueDate = duedate1,
                    StartDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false),
                    DueDateTZ = new DateTimeWithZone(duedate1, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                }
            };
            var contentChild1 = new PxDC.ContentItem()
            {
                Id = testChild1,
                Categories = new List<PxDC.TocCategory>()
                {
                    new PxDC.TocCategory()
                    {
                        Id = toc,
                        ItemParentId = itemId,
                        Sequence = "a"
                    }
                },
                Sequence = "a",
                Type = "",
                ParentId = itemId,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    StartDate = duedate1,
                    DueDate = duedate1,
                    StartDateTZ = new DateTimeWithZone(duedate1, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false),
                    DueDateTZ = new DateTimeWithZone(duedate1, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                }
            };
            var contentChild2 = new PxDC.ContentItem()
            {
                Id = testChild2,
                Categories = new List<PxDC.TocCategory>()
                {
                    new PxDC.TocCategory()
                    {
                        Id = toc,
                        ItemParentId = itemId,
                        Sequence = "b"
                    }
                },
                Containers = { unitContainer },
                SubContainerIds = { unitSubContainer },
                Sequence = "b",
                Type = folderType,
                ParentId = itemId,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    StartDate = duedate,
                    DueDate = duedate1,
                    StartDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false),
                    DueDateTZ = new DateTimeWithZone(duedate1, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                }
            };
            var contentChild2A = new PxDC.ContentItem()
            {
                Id = testChild2A,
                Categories = new List<PxDC.TocCategory>()
                {
                    new PxDC.TocCategory()
                    {
                        Id = toc,
                        ItemParentId = testChild2,
                        Sequence = "b"
                    }
                },
                Containers = { unitContainer },
                SubContainerIds = { unitSubContainer },
                Sequence = "a",
                Type = folderType,
                ParentId = testChild2,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    StartDate = duedate1,
                    DueDate = duedate1,
                    StartDateTZ = new DateTimeWithZone(duedate1, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false),
                    DueDateTZ = new DateTimeWithZone(duedate1, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                }
            };
            var contentChild2B = new PxDC.ContentItem()
            {
                Id = testChild2B,
                Categories = new List<PxDC.TocCategory>()
                {
                    new PxDC.TocCategory()
                    {
                        Id = toc,
                        ItemParentId = testChild2,
                        Sequence = "b"
                    }
                },
                Containers = { unitContainer },
                SubContainerIds = { unitSubContainer },
                Sequence = "b",
                Type = folderType,
                ParentId = testChild2,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    StartDate = duedate,
                    DueDate = duedate,
                    StartDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false),
                    DueDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                }
            };

            var unassignedContentChild2B = contentChild2B;
            unassignedContentChild2B.AssignmentSettings.StartDate = DateTime.MinValue;
            unassignedContentChild2B.AssignmentSettings.DueDate = DateTime.MinValue;
            unassignedContentChild2B.AssignmentSettings.StartDateTZ = new DateTimeWithZone(DateTime.MinValue, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false);
            unassignedContentChild2B.AssignmentSettings.DueDateTZ = new DateTimeWithZone(DateTime.MinValue, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false);
            
            /* ProcessAssignments method's calls */
            contentActions.GetContent(_entityid, unitId).Returns(unitItem);
            contentActions.GetContent(_entityid, testChild2B).Returns(contentChild2B);

            var parents = new List<PxDC.ContentItem> { contentChild2, contentChild1, contentTopItem, unitItem };
            contentHelper.GetParentHeirachy(testChild2B, TreeCategoryType.ManagementCard, toc, _entityid).Returns(parents);

            var listContentWithDueDates = new List<PxDC.ContentItem>() { contentTopItem, contentChild1, contentChild2, contentChild2A, unassignedContentChild2B };
            contentActions.ListContentWithDueDates(_entityid, unitContainerValue, unitId, toc).Returns(listContentWithDueDates);

            /* RemoveItem method's calls */
            var listContainerItems = new List<PxDC.ContentItem>() { };
            contentActions.GetContainerItemsForParent(_entityid, unitContainerValue, unitId, testChild2, toc).Returns(listContainerItems);

            // Act
            var changes = helper.SaveNavigationState(state, toc).Changes;

            // Assert
            /* ProcessAssignments method's calls */
            contentActions.Received().UnAssignAssignmentCenterItems(String.Empty, Arg.Any<IEnumerable<PxDC.AssignmentCenterItem>>(), toc, true);
            /* RemoveItem method's calls */
            contentActions.Received().GetContainerItemsForParent(_entityid, unitContainerValue, unitId, testChild2B, toc);
            contentActions.Received().StoreContent(Arg.Is<PxDC.ContentItem>(x => RemoveOnUnassignCheckStoreContent(x, listContainerItems, toc)));

            Assert.IsTrue(changes.Exists(x => x.Id == testChild2B));

            foreach (var changedItem in changes)
            {
                if (changedItem.Id == testChild2B)
                {  
                    Assert.AreEqual(changedItem.StartDate.Year, DateTime.MinValue.Year);
                    Assert.AreEqual(changedItem.EndDate.Year, DateTime.MinValue.Year);
                }
                else
                {
                   Assert.AreEqual(changedItem.StartDate, duedate1);
                   Assert.AreEqual(changedItem.EndDate, duedate1);
                }
            }
        }

        [TestCategory("AssignmentCenterHelper"), TestMethod]
        public void SaveNavigationState_RemoveOnUnassign_Should_Unassign_and_Remove_a_Unit_and_its_Children()
        {
            /* Logic
             * hierachy: unit -> contentItem1 -> contentChild1 (childless), contentChild2 (has childA and childB)
             * 
             * Expected Result:
             * unit and its descendents: contentItem1, contentChild1, contentChild2, contentChild2's childA and childB will get unassigned
             * as well as their sub-container's value and bfw_toc's parentId  will be changed to FACEPLATE_REMOVED 
             */

            // Arrange
            const string unitId = "AssignmentUnit_AtoZ";
            const string itemId = "i_am_a_parent";
            const string testChild = "i_am_a_child";
            const string testChild1 = testChild + "_1";
            const string testChild2 = testChild + "_2";
            const string testChild2A = testChild + "_2_a";
            const string testChild2B = testChild + "_2_b";

            const string toc = "assignmentfilter";
            var duedate = DateTime.Now.AddDays(13);
            const string sequence = "bb";
            const string unitSequence = "a";
            const string folderType = "pxunit";
            const string unitContainerValue = "XbookV2";
            const string unitParentId = "PX_MULTIPART_LESSONS";

            var child2A = new AssignmentCenterItem()
            {
                Id = testChild2A,
                StartDate = duedate,
                EndDate = duedate,
                Sequence = "a",
                ParentId = testChild2,
                Children = null
            };
            var child2B = new AssignmentCenterItem()
            {
                Id = testChild2B,
                StartDate = duedate,
                EndDate = duedate,
                Sequence = "b",
                ParentId = testChild2,
                Children = null
            };
            var child2 = new AssignmentCenterItem()
            {
                Id = testChild2,
                StartDate = duedate,
                EndDate = duedate,
                Sequence = "b",
                ParentId = itemId,
                Children = new List<AssignmentCenterItem>() { child2A, child2B }
            };
            var child1 = new AssignmentCenterItem()
            {
                Id = testChild1,
                StartDate = duedate,
                EndDate = duedate,
                Sequence = "a",
                ParentId = itemId,
                Children = null
            };
            var topItem = new AssignmentCenterItem()
            {
                Id = itemId,
                StartDate = duedate,
                EndDate = duedate,
                Sequence = "a",
                ParentId = unitId,
                Children = new List<AssignmentCenterItem>() { child1, child2 }
            };
            var unit = new AssignmentCenterItem()
            {
                Id = unitId,
                StartDate = duedate,
                EndDate = duedate,
                Sequence = "a",
                ParentId = unitParentId,
                Children = new List<AssignmentCenterItem>() { topItem }
            };

            var state = new AssignmentCenterNavigationState()
            {
                Above = null,
                Below = null,
                Category = new AssignmentCenterCategory()
                {
                    Items = new List<AssignmentCenterItem>() { unit }
                },
                Changed = unit,
                EntityId = _entityid,
                Operation = AssignmentCenterOperation.RemoveOnUnassign,
                Toc = toc,
                RemoveFrom = toc
            };

            // Stub
            context.AccessLevel = AccessLevel.Instructor;
            context.Course.CourseTimeZone = "Eastern Standard Time";

            var unitContainer = new PxDC.Container("assignmentfilter", unitContainerValue);
            var unitSubContainer = new PxDC.Container("assignmentfilter", unitId);
            var unitParentSubContainer = new PxDC.Container("assignmentfilter", unitParentId);
            var unitItem = new PxDC.ContentItem()
            {
                Id = unitId,
                Categories = new List<PxDC.TocCategory>()
                {
                    new PxDC.TocCategory()
                    {
                        Id = toc,
                        ItemParentId = unitParentId,
                        Sequence = "a"
                    }
                },
                Containers = { unitContainer },
                SubContainerIds = { unitParentSubContainer },
                ParentId = unitParentId,
                Sequence = unitSequence,
                Type = folderType,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    StartDate = duedate,
                    DueDate = duedate,
                    StartDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false),
                    DueDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                }
            };

            var contentTopItem = new PxDC.ContentItem()
            {
                Id = itemId,
                Categories = new List<PxDC.TocCategory>()
                {
                    new PxDC.TocCategory()
                    {
                        Id = toc,
                        ItemParentId = unitId,
                        Sequence = "a"
                    }
                },
                Containers = { unitContainer },
                SubContainerIds = { unitSubContainer },
                ParentId = unitId,
                Sequence = sequence,
                Type = folderType,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    StartDate = duedate,
                    DueDate = duedate,
                    StartDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false),
                    DueDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                }
            };
            var contentChild1 = new PxDC.ContentItem()
            {
                Id = testChild1,
                Categories = new List<PxDC.TocCategory>()
                {
                    new PxDC.TocCategory()
                    {
                        Id = toc,
                        ItemParentId = itemId,
                        Sequence = "a"
                    }
                },
                Containers = { unitContainer },
                SubContainerIds = { unitSubContainer },
                Sequence = "a",
                Type = "",
                ParentId = itemId,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    StartDate = DateTime.MinValue,
                    DueDate = duedate,
                    StartDateTZ = new DateTimeWithZone(DateTime.MinValue, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false),
                    DueDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                }
            };
            var contentChild2 = new PxDC.ContentItem()
            {
                Id = testChild2,
                Categories = new List<PxDC.TocCategory>()
                {
                    new PxDC.TocCategory()
                    {
                        Id = toc,
                        ItemParentId = itemId,
                        Sequence = "b"
                    }
                },
                Containers = { unitContainer },
                SubContainerIds = { unitSubContainer },
                Sequence = "b",
                Type = folderType,
                ParentId = itemId,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    StartDate = duedate,
                    DueDate = duedate,
                    StartDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false),
                    DueDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                }
            };
            var contentChild2A = new PxDC.ContentItem()
            {
                Id = testChild2A,
                Categories = new List<PxDC.TocCategory>()
                {
                    new PxDC.TocCategory()
                    {
                        Id = toc,
                        ItemParentId = testChild2,
                        Sequence = "b"
                    }
                },
                Containers = { unitContainer },
                SubContainerIds = { unitSubContainer },
                Sequence = "b",
                Type = folderType,
                ParentId = testChild2,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    StartDate = duedate,
                    DueDate = duedate,
                    StartDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false),
                    DueDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                }
            };
            var contentChild2B = new PxDC.ContentItem()
            {
                Id = testChild2B,
                Categories = new List<PxDC.TocCategory>()
                {
                    new PxDC.TocCategory()
                    {
                        Id = toc,
                        ItemParentId = testChild2,
                        Sequence = "b"
                    }
                },
                Containers = { unitContainer },
                SubContainerIds = { unitSubContainer },
                Sequence = "b",
                Type = folderType,
                ParentId = testChild2,
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    StartDate = duedate,
                    DueDate = duedate,
                    StartDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false),
                    DueDateTZ = new DateTimeWithZone(duedate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), false)
                }
            };

            /* ProcessAssignments method's calls */
            contentActions.GetContent(_entityid, unitId).Returns(unitItem);

            var parents = new List<PxDC.ContentItem> { unitItem };
            contentHelper.GetParentHeirachy(unitId, TreeCategoryType.ManagementCard, toc, _entityid).Returns(parents);

            var listContentWithDueDates = new List<PxDC.ContentItem>() { contentChild2B, contentChild2A, contentChild2, contentChild1, contentTopItem };
            contentActions.ListContentWithDueDates(_entityid, unitContainerValue, unitId, toc).Returns(listContentWithDueDates);

            /* RemoveItem method's calls */
            var listContainerItems = new List<PxDC.ContentItem>() { contentTopItem, contentChild1, contentChild2, contentChild2A, contentChild2B };
            contentActions.GetContainerItemsForParent(_entityid, unitContainerValue, unitId, unitParentId, toc).Returns(listContainerItems);

            // Act
            var changes = helper.SaveNavigationState(state, toc).Changes;

            // Assert
            /* ProcessAssignments method's calls */
            contentActions.Received().UnAssignAssignmentCenterItems(String.Empty, Arg.Any<IEnumerable<PxDC.AssignmentCenterItem>>(), toc, true);
            /* RemoveItem method's calls */
            contentActions.Received().GetContainerItemsForParent(_entityid, unitContainerValue, unitId, unitId, toc);
            contentActions.Received().StoreContent(Arg.Is<PxDC.ContentItem>(x => RemoveOnUnassignCheckStoreContent(x, listContainerItems, toc)));

            Assert.IsTrue(changes.Exists(x => x.Id == itemId));

            foreach (var changedItem in changes)
            {
                Assert.AreEqual(changedItem.StartDate.Year, DateTime.MinValue.Year);
                Assert.AreEqual(changedItem.EndDate.Year, DateTime.MinValue.Year);
            }
        }
        
        #endregion

        /// <summary>
        /// The function should assign all items with default points within given unit
        /// </summary>
        [TestCategory("AssignmentCenterHelper"), TestMethod]
        public void SaveNavigationState_Should_Assign_UnitItems_With_DefaultPoints()
        {
            var unit = SetParentAssignmentCenterItem();
            unit.EndDate = DateTime.Now;
            unit.StartDate = unit.EndDate;
            var children = new List<AssignmentCenterItem>() 
            {                   
                new AssignmentCenterItem()
                {
                    Id = "1",
                    ParentId = unit.Id,
                    DefaultPoints = 0,
                    Type = "assignment"
                },
                new AssignmentCenterItem()
                {
                    Id = "2",
                    ParentId = unit.Id,
                    DefaultPoints = 10,
                    Type = "assignment"
                },
                new AssignmentCenterItem()
                {
                    Id = "3",
                    ParentId = unit.Id,
                    DefaultPoints = 20,
                    Type = "assignment"
                }
            };
            unit.Children = children;
            var category = new List<AssignmentCenterItem>();
            category.Add(unit);
            category.AddRange(children);
            var changed = new AssignmentCenterNavigationState()
            {
                Changed = unit,
                Category = new AssignmentCenterCategory() { Items = category },
                Operation = AssignmentCenterOperation.DatesAssigned,
            };
            contentActions.GetContent("", "").ReturnsForAnyArgs(new Biz.DataContracts.ContentItem()
            {
                AssignmentSettings = new Biz.DataContracts.AssignmentSettings()
                {
                    CategorySequence = "a"
                }
            });

            var result = helper.SaveNavigationState(changed, _defaultContainerToc);

            Assert.AreEqual(3, result.Changes.Count);
            Assert.AreEqual("1", category.Except(result.Changes).FirstOrDefault().Id);
        }

        /// <summary>
        /// The function should assign an item within unit without changing unit items with default points
        /// </summary>
        [TestCategory("AssignmentCenterHelper"), TestMethod]
        public void SaveNavigationState_Should_Assign_Item_Without_Affecting_UnitItems_With_DefaultPoints()
        {
            var unit = SetParentAssignmentCenterItem();
            var item = new AssignmentCenterItem()
            {
                Id = "1",
                ParentId = unit.Id,
                DefaultPoints = 0,
                Type = "assignment",
                EndDate = DateTime.Now
            };
            var children = new List<AssignmentCenterItem>() 
            {                   
                item,
                new AssignmentCenterItem()
                {
                    Id = "2",
                    ParentId = unit.Id,
                    DefaultPoints = 10,
                    Type = "assignment"
                },
                new AssignmentCenterItem()
                {
                    Id = "3",
                    ParentId = unit.Id,
                    DefaultPoints = 20,
                    Type = "assignment"
                }
            };
            unit.Children = children;
            var category = new List<AssignmentCenterItem>();
            category.Add(unit);
            category.AddRange(children);
            var changed = new AssignmentCenterNavigationState()
            {
                Changed = item,
                Category = new AssignmentCenterCategory() { Items = category },
                Operation = AssignmentCenterOperation.DatesAssigned,
            };
            contentActions.GetContent("", "").ReturnsForAnyArgs(new Biz.DataContracts.ContentItem()
            {
                AssignmentSettings = new Biz.DataContracts.AssignmentSettings()
                {
                    CategorySequence = "a"
                }
            });

            var result = helper.SaveNavigationState(changed, _defaultContainerToc);

            Assert.AreEqual(2, result.Changes.Count);
            Assert.IsTrue(result.Changes.Count(i => i.Id.Equals("_1")) > 0);
            Assert.IsTrue(result.Changes.Count(i => i.Id.Equals("1")) > 0);
        }

        /// <summary>
        /// The function should remove an item within unit without changing unit items with default points
        /// </summary>
        [TestCategory("AssignmentCenterHelper"), TestMethod]
        public void SaveNavigationState_Should_Remove_Item_Without_Affecting_UnitItems_With_DefaultPoints()
        {
            var unit = SetParentAssignmentCenterItem();
            unit.EndDate = DateTime.Now;
            unit.StartDate = unit.EndDate;
            var item = new AssignmentCenterItem()
            {
                Id = "1",
                ParentId = unit.Id,
                DefaultPoints = 0,
                Type = "assignment"
            };
            var children = new List<AssignmentCenterItem>() 
            {                   
                item,
                new AssignmentCenterItem()
                {
                    Id = "2",
                    ParentId = unit.Id,
                    DefaultPoints = 10,
                    Type = "assignment"
                },
                new AssignmentCenterItem()
                {
                    Id = "3",
                    ParentId = unit.Id,
                    DefaultPoints = 20,
                    Type = "assignment"
                }
            };
            unit.Children = children;
            var category = new List<AssignmentCenterItem>();
            category.Add(unit);
            category.AddRange(children);
            var changed = new AssignmentCenterNavigationState()
            {
                Changed = item,
                Category = new AssignmentCenterCategory() { Items = category },
                Operation = AssignmentCenterOperation.Removed,
            };
            contentActions.GetContent("", "").ReturnsForAnyArgs(new Biz.DataContracts.ContentItem()
            {
                AssignmentSettings = new Biz.DataContracts.AssignmentSettings()
                {
                    CategorySequence = "a"
                },
                Type = "assignment",
                Categories = new List<Biz.DataContracts.TocCategory>() 
                { 
                    new Biz.DataContracts.TocCategory()
                    {
                        Id ="cat",
                        Type= _defaultContainerToc
                    }
                }
            });

            var result = helper.SaveNavigationState(changed, _defaultContainerToc);

            Assert.AreEqual(2, result.Changes.Count);
            Assert.IsTrue(result.Changes.Count(i => i.Id.Equals("_1")) > 0);
            Assert.IsTrue(result.Changes.Count(i => i.Id.Equals("1")) > 0);
        }

        /// <summary>
        /// The function should unassign an item within unit without changing unit items with default points
        /// </summary>
        [TestCategory("AssignmentCenterHelper"), TestMethod]
        public void SaveNavigationState_Should_Unassign_Item_Without_Affecting_UnitItems_With_DefaultPoints()
        {
            var unit = SetParentAssignmentCenterItem();
            unit.EndDate = DateTime.Now;
            unit.StartDate = unit.EndDate;
            var item = new AssignmentCenterItem()
            {
                Id = "1",
                ParentId = unit.Id,
                DefaultPoints = 0,
                Type = "assignment"
            };
            var children = new List<AssignmentCenterItem>() 
            {                   
                item,
                new AssignmentCenterItem()
                {
                    Id = "2",
                    ParentId = unit.Id,
                    DefaultPoints = 10,
                    Type = "assignment"
                },
                new AssignmentCenterItem()
                {
                    Id = "3",
                    ParentId = unit.Id,
                    DefaultPoints = 20,
                    Type = "assignment"
                }
            };
            unit.Children = children;
            var category = new List<AssignmentCenterItem>();
            category.Add(unit);
            category.AddRange(children);
            var changed = new AssignmentCenterNavigationState()
            {
                Changed = item,
                Category = new AssignmentCenterCategory() { Items = category },
                Operation = AssignmentCenterOperation.DatesUnAssigned,
            };
            contentActions.GetContent("", "").ReturnsForAnyArgs(new Biz.DataContracts.ContentItem()
            {
                AssignmentSettings = new Biz.DataContracts.AssignmentSettings()
                {
                    CategorySequence = "a"
                },
                Type = "assignment",
                Categories = new List<Biz.DataContracts.TocCategory>() 
                { 
                    new Biz.DataContracts.TocCategory()
                    {
                        Id ="cat",
                        Type = _defaultContainerToc
                    }
                }
            });

            var result = helper.SaveNavigationState(changed, _defaultContainerToc);

            Assert.AreEqual(2, result.Changes.Count);
            Assert.IsTrue(result.Changes.Count(i => i.Id.Equals("_1")) > 0);
            Assert.IsTrue(result.Changes.Count(i => i.Id.Equals("1")) > 0);
        }

        /// <summary>
        /// Make sure that the value in web.config for FaceplateRemoved gets set on the default toc when
        /// calling RemoveItem
        /// </summary>
        [TestCategory("AssignmentCenterHelper"), TestMethod]
        public void AssigmentCenterHelperAction_RemoveItem_DefaultsToSyllabusFilter()
        {
            PxDC.ContentItem item = new PxDC.ContentItem();
            helper.RemoveItem(item, String.Empty, _defaultContainerToc, _defaultContainerToc);
            Assert.AreEqual(_removedValue, item.GetContainer(_defaultContainerToc));
        }
        
        [TestCategory("AssignmentCenterHelper"), TestMethod]
        public void AssigmentCenterHelperAction_RemoveItem_Should_Flag_Toc()
        {
            const string parentId = "This is a parent Id";
            const string removeFrom = "MYFILTER";

            var item = new PxDC.ContentItem();
            
            var resultItem = helper.RemoveItem(item, parentId, _defaultContainerToc, removeFrom);

            Assert.AreEqual(resultItem.Containers.First(c => c.Toc == removeFrom).Value, _removedValue);
        } 

        [TestCategory("AssignmentCenterHelper"), TestMethod]
        public void AssigmentCenterHelperAction_RemoveItem_Should_Flag_More_Than_One_Toc_If_Passed_In_Commas()
        {
            var removalFlag = ConfigurationManager.AppSettings["FaceplateRemoved"];
            
            const string parentId = "This is a parent Id";
            const string toRemove1 = "FilterNumber1";
            const string toRemove2 = "FilterNumber2";

            var item = new PxDC.ContentItem();

            var removeFrom = String.Join(",", toRemove1, toRemove2);

            var resultItem = helper.RemoveItem(item, parentId, _defaultContainerToc, removeFrom);

            Assert.AreEqual(resultItem.Containers.First(c => c.Toc == toRemove1).Value, removalFlag);
            Assert.AreEqual(resultItem.Containers.First(c => c.Toc == toRemove2).Value, removalFlag);
        }

        /// <summary>
        /// Makes sure that parent container and subcontainer of the default toc are set on
        /// the item when calling UpdateContainer
        /// </summary>
        [TestCategory("AssignmentCenterHelper"), TestMethod]
        public void AssignmentCenterHelperAction_UpdateContainer_GetsContainerValuesFromParentByDefault()
        {
            AssignmentCenterItem item = new AssignmentCenterItem();
            var container = "getsome";
            var subcontainer = "pwnd";

            contentActions.GetContent(_entityid, "parentid").ReturnsForAnyArgs(new PxDC.ContentItem()
            {
                Containers = new List<PxDC.Container>()
                {
                    new PxDC.Container(_defaultContainerToc, container)
                },
                SubContainerIds = new List<PxDC.Container>()
                {
                    new PxDC.Container(_defaultContainerToc, subcontainer)
                }
            });
            var updatedItems = helper.UpdateContainer(item, _defaultContainerToc);
            Assert.AreEqual(container, item.GetContainer());
            Assert.AreEqual(subcontainer, item.GetSubContainer());
        }

        /// <summary>
        /// Makes sure that NewItem calls list children with the proper default toc argument
        /// </summary>
        [TestCategory("AssignmentCenterHelper"), TestMethod]
        public void AssignmentCenterHelperAction_NewItem_CallsListChildrenWithDefaultTOC()
        {
            AssignmentCenterItem item = new AssignmentCenterItem();
            var contentid = "contentid";
            var parentid = "parentid";
            var container = "booya";
            var subcontainer = "subbooya";

            //Content item
            contentActions.GetContent(_entityid, contentid).Returns(new PxDC.ContentItem()
            {
                Type = "mock",
                Containers = new List<PxDC.Container>()
                {
                    new PxDC.Container(_defaultContainerToc, container)
                },
                SubContainerIds = new List<PxDC.Container>()
                {
                    new PxDC.Container(_defaultContainerToc, subcontainer)
                },
                //TODO: ToAssignmentCenter requires a DueDate but has no check for it. We should fix this
                AssignmentSettings = new PxDC.AssignmentSettings()
                {
                    DueDate = Convert.ToDateTime("01/01/0001"),
                    DueDateTZ = new Common.DateTimeWithZone(Convert.ToDateTime("01/01/0001"), TimeZoneInfo.Local, false)
                }
            });

            var updatedItems = helper.NewItem(contentid, parentid, _defaultContainerToc);
            Assert.AreEqual(1, contentActions.ListChildren(_entityid, parentid, 1, _defaultContainerToc).ReceivedCalls().Count());
        }

        private List<Biz.DataContracts.Grade> GetDueGrades()
        {
            List<Biz.DataContracts.Grade> result = new List<Biz.DataContracts.Grade>();

            foreach (var item in GetDueItems())
            {
                result.Add(new Biz.DataContracts.Grade() { GradedItem = item });
            }

            return result;
        }

        private List<Biz.DataContracts.ContentItem> GetDueItems()
        {
            List<Biz.DataContracts.ContentItem> result = new List<Biz.DataContracts.ContentItem>();

            result.Add(new Biz.DataContracts.ContentItem()
            {
                Id = "1",
                Title = "title 1",
                SubTitle = "subtitle 1",
                Subtype = "dropbox",
                Type = "dropbox",
                MaxPoints = 10,
                AssignmentSettings = new Biz.DataContracts.AssignmentSettings()
                {
                    Points = 10,
                    StartDate = new DateTime(2023, 1, 2),
                    StartDateTZ = new Common.DateTimeWithZone(new DateTime(2023, 1, 2), TimeZoneInfo.Local, false),
                    DueDate = new DateTime(2023, 2, 3),
                    DueDateTZ = new Common.DateTimeWithZone(new DateTime(2023, 2, 3), TimeZoneInfo.Local, false),
                }
            });
            result.Add(new Biz.DataContracts.ContentItem()
            {
                Id = "2",
                Subtype = "PxUnit",
                Type = "PxUnit",
                AssignmentSettings = new Biz.DataContracts.AssignmentSettings()
                {
                    StartDate = new DateTime(2023, 1, 2),
                    StartDateTZ = new Common.DateTimeWithZone(new DateTime(2023, 1, 2), TimeZoneInfo.Local, false),
                    DueDate = new DateTime(2023, 2, 3),
                    DueDateTZ = new Common.DateTimeWithZone(new DateTime(2023, 2, 3), TimeZoneInfo.Local, false),
                }
            });
            result.Add(new Biz.DataContracts.ContentItem()
            {
                Id = "3",
                Subtype = "link",
                Type = "link",
                HiddenFromStudents = true,
                AssignmentSettings = new Biz.DataContracts.AssignmentSettings()
                {
                    StartDate = new DateTime(2023, 1, 2),
                    StartDateTZ = new Common.DateTimeWithZone(new DateTime(2023, 1, 2), TimeZoneInfo.Local, false),
                    DueDate = new DateTime(2023, 2, 3),
                    DueDateTZ = new Common.DateTimeWithZone(new DateTime(2023, 2, 3), TimeZoneInfo.Local, false),
                }
            });
            result.Add(new Biz.DataContracts.ContentItem()
            {
                Id = "4",
                Subtype = "dropbox",
                Type = "dropbox",
                Visibility = "<bfw_visibility><roles><student><restriction><date endate=\"9/30/2023 4:00:00 AM\" /></restriction></student></roles></bfw_visibility>",
                AssignmentSettings = new Biz.DataContracts.AssignmentSettings()
                {
                    StartDate = new DateTime(2023, 1, 2),
                    StartDateTZ = new Common.DateTimeWithZone(new DateTime(2023, 1, 2), TimeZoneInfo.Local, false),
                    DueDate = new DateTime(2023, 2, 3),
                    DueDateTZ = new Common.DateTimeWithZone(new DateTime(2023, 2, 3), TimeZoneInfo.Local, false),
                }
            });

            return result;
        }

        private static bool RemoveOnUnassignCheckStoreContent(PxDC.ContentItem item, IEnumerable<PxDC.ContentItem> children, string toc)
        {
            var flag = ConfigurationManager.AppSettings["FaceplateRemoved"];

            // check the top item's container and subcontainer
            var cat = item.Categories.FirstOrDefault(c => c.Id == toc);
            var valid = item.GetContainer(toc) == flag && cat != null && cat.ItemParentId == flag;

            // check the children's container and subcontainer
            foreach (var child in children)
            {
                valid = child.GetContainer(toc) == flag && child.GetSubContainer(toc) == item.Id;
            }

            return valid;
        }

        private Models.AssignedItem SetAssignedItem(DateTime assignmentDate)
        {
            var item = new Models.AssignedItem()
            {
                Id = "1",
                DueDate = assignmentDate,
                SubType = "Dropbox",
                Type = "Dropbox"
            };

            return item;
        }

        private Biz.DataContracts.ContentItem SetContentItem(Models.AssignedItem item, double defaultPoints = 0)
        {
            var parentid = "_1";
            var toc = "syllabusfilter";

            var contentItem = new Biz.DataContracts.ContentItem()
            {
                Id = item.Id,
                ParentId = parentid,
                Categories = new List<PxDC.TocCategory>()
                {
                    new PxDC.TocCategory()
                    {
                        Id = toc,
                        ItemParentId = parentid,
                        Sequence = "a"
                    }
                },
                Subtype = item.SubType,
                Type = item.Type,
                AssignmentSettings = new Biz.DataContracts.AssignmentSettings()
                {
                    DueDate = item.DueDate,
                    DueDateTZ = new Common.DateTimeWithZone(item.DueDate, TimeZoneInfo.Local, false)
                },
                DefaultPoints = (int)defaultPoints
            };

            return contentItem;
        }
        
        private AssignmentCenterItem SetAssignmentCenterItem(Models.AssignedItem item, DateTime assignmentDate)
        {
            var assignedCenterItem = new AssignmentCenterItem()
            {
                Type = "Dropbox",
                Id = item.Id,
                ParentId = "_1",
                EndDate = assignmentDate
            };

            return assignedCenterItem;
        }

        private AssignmentCenterItem SetParentAssignmentCenterItem()
        {
            var assignedCenterItem = new AssignmentCenterItem()
            {
                Type = "pxunit",
                Id = "_1"
            };

            return assignedCenterItem;
        }

        private Biz.DataContracts.ContentItem SetParentContentItem()
        {
            var parentid = "_1";

            var parent = new Biz.DataContracts.ContentItem()
            {
                Type = "pxunit",
                Id = parentid,
                AssignmentSettings = new Biz.DataContracts.AssignmentSettings()
                {
                    StartDate = DateTime.MinValue,
                    StartDateTZ = new Common.DateTimeWithZone(DateTime.MinValue, TimeZoneInfo.Local, false),
                    DueDate = DateTime.MinValue,
                    DueDateTZ = new Common.DateTimeWithZone(DateTime.MinValue, TimeZoneInfo.Local, false)
                }
            };

            return parent;
        }
    }
}
