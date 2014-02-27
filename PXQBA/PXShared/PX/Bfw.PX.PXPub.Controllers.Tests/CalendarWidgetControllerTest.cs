using System;
using System.Collections.Generic;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Widgets;
using Bfw.PX.PXPub.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Linq;
using Bfw.PX.PXPub.Controllers.Contracts;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    [TestClass]
    public class CalendarWidgetControllerTest
    {
        private CalendarWidgetController controller;

        private IBusinessContext context;
        private IGradeActions gradeActions;
        private IContentActions contentActions;
        private IPageActions pageActions;
        private IAssignmentCenterHelper assignmentHelper;
        private IGroupActions groupActions;

        [TestInitialize]
        public void TestInitialize()
        {
            context = Substitute.For<IBusinessContext>();
            gradeActions = Substitute.For<IGradeActions>();
            contentActions = Substitute.For<IContentActions>();
            pageActions = Substitute.For<IPageActions>();
            assignmentHelper = Substitute.For<IAssignmentCenterHelper>();
            groupActions = Substitute.For<IGroupActions>();

            context.Course = new Biz.DataContracts.Course();

            controller = new CalendarWidgetController(context, gradeActions, contentActions, pageActions, assignmentHelper, groupActions);
        }

        /// <summary>
        /// Controller is supposed to return correct number of assignment groups and retrieved assignments for instructor view
        /// </summary>
        [TestCategory("CalendarWidgetController"), TestMethod]
        public void Controller_Should_Return_Correct_Assignments_For_Instructor()
        {
            context.AccessLevel = AccessLevel.Instructor;
            assignmentHelper.GetDueAssignmentsForInstructor("entityId", 7 * 22).ReturnsForAnyArgs(GetItems());

            var result = controller.AgendaFullView("entityId");
            var model = result.Model as AssignmentWidget;
                      
            Assert.IsTrue((from s in model.Groups where s.Assignments != null && s.Assignments.Count(i => i.Id.Equals("1")) > 0 select s).Count() > 0);
            Assert.IsTrue((from s in model.Groups where s.Assignments != null && s.Assignments.Count(i => i.Id.Equals("2")) > 0 select s).Count() > 0);
        }

        /// <summary>
        /// Controller is supposed to return correct number of assignment groups and retrieved assignments for student view
        /// </summary>
        [TestCategory("CalendarWidgetController"), TestMethod]
        public void Controller_Should_Return_Correct_Assignments_For_Student()
        {
            context.AccessLevel = AccessLevel.Student;
            assignmentHelper.GetDueAssignmentsForStudent(7 * 22, true, true).ReturnsForAnyArgs(GetItems().Where(i => !i.Id.Equals("2")));

            var result = controller.AgendaFullView("entityId");
            var model = result.Model as AssignmentWidget;

            Assert.IsTrue((from s in model.Groups where s.Assignments != null && s.Assignments.Count(i => i.Id.Equals("1")) > 0 select s).Count() > 0);
            Assert.IsTrue((from s in model.Groups where s.Assignments != null && s.Assignments.Count(i => i.Id.Equals("2")) > 0 select s).Count() == 0);
        }

        /// <summary>
        /// AgendaFullView should not return duplicated week in the model for instructor.
        /// </summary>
        [TestCategory("CalendarWidgetController"), TestMethod]
        public void AgendaFullView_ForInstructor_NoDuplicatedWeekShouldBeInTheModel()
        {
            context.AccessLevel = AccessLevel.Instructor;
            assignmentHelper.GetDueAssignmentsForInstructor("entityId", 7 * 22).ReturnsForAnyArgs(GetItemsInNext6Months());

            var result = controller.AgendaFullView("entityId");
            var model = result.Model as AssignmentWidget;

            var duplicatedFound = 0;
            var weeks = new List<string>();
            model.Groups.ForEach(w =>
            {
                var weekTitle = w.Title;
                if (weeks.Contains(weekTitle))
                {
                    duplicatedFound++;
                }
                else
                {
                    weeks.Add(weekTitle);
                }
            });
       
            Assert.IsTrue(duplicatedFound == 0);
        }

        /// <summary>
        /// AgendaFullView should not return duplicated week in the model for student.
        /// </summary>
        [TestCategory("CalendarWidgetController"), TestMethod]
        public void AgendaFullView_ForStudent_NoDuplicatedWeekShouldBeInTheModel()
        {
            context.AccessLevel = AccessLevel.Student;
            assignmentHelper.GetDueAssignmentsForStudent(7 * 22, true, true).ReturnsForAnyArgs(GetItemsInNext6Months());

            var result = controller.AgendaFullView("entityId");
            var model = result.Model as AssignmentWidget;

            var duplicatedFound = 0;
            var weeks = new List<string>();
            model.Groups.ForEach(w =>
            {
                var weekTitle = w.Title;
                if (weeks.Contains(weekTitle))
                {
                    duplicatedFound++;
                }
                else
                {
                    weeks.Add(weekTitle);
                }
            });

            Assert.IsTrue(duplicatedFound == 0);
        }

        /// <summary>
        /// This test to ensure that subtitle of the calendar model have been passed properly to the view
        /// </summary>
        [TestCategory("CalendarWidgetController"), TestMethod]
        public void AgendaFullView_OnRender_Displays_Item_SubTitle()
        {
            context.AccessLevel = AccessLevel.Instructor;
            assignmentHelper.GetDueAssignmentsForInstructor("entityId", 7 * 22).ReturnsForAnyArgs(GetItems());

            var result = controller.AgendaFullView("entityId");
            var model = result.Model as AssignmentWidget;

            // Verifying if the subtitles of the items have been passed to the model propely
            Assert.AreEqual("subtitle 1", (from g in model.Groups where g.Assignments != null && g.Assignments.Count(a => a.Id.Equals("1")).Equals(1) select g.Assignments.Where(a => a.Id.Equals("1"))).Single().Single().FriendlyNameSourceType);
            Assert.AreEqual("subtitle 2", (from g in model.Groups where g.Assignments != null && g.Assignments.Count(a => a.Id.Equals("2")).Equals(1) select g.Assignments.Where(a => a.Id.Equals("2"))).Single().Single().FriendlyNameSourceType);
        }

        /// <summary>
        /// Ensure that assignments in the same day are sorted first by due date and then by title
        /// </summary>
        [TestCategory("CalendarWidgetController"), TestMethod]
        public void AgendaFullView_NextWeek_Items_Ordered_ByDueDate_ThenTitle()
        {
            context.AccessLevel = AccessLevel.Instructor;
            assignmentHelper.GetDueAssignmentsForInstructor("entityId", 7 * 22).ReturnsForAnyArgs(GetItemsInFutureWeek(true));
            var result = controller.AgendaFullView("entityId");
            var model = result.Model as AssignmentWidget;
            //Verify sort order for next week assignments within the same day
            var targetGroup = model.Groups.SingleOrDefault(g => g.Assignments != null && g.Assignments.Count == 3);
            Assert.IsNotNull(targetGroup);
            var assignments = targetGroup.Assignments;
            Assert.AreEqual("With first due date - first in list", assignments.First().Title);
            Assert.AreEqual("Then a minute later - last in list", assignments.Last().Title);
        }

        /// <summary>
        /// Ensure that assignments in the same day are sorted first by due date and then by title
        /// </summary>
        [TestCategory("CalendarWidgetController"), TestMethod]
        public void AgendaFullView_AfterNextWeek_Items_Ordered_ByDueDate_ThenTitle()
        {
            context.AccessLevel = AccessLevel.Instructor;
            assignmentHelper.GetDueAssignmentsForInstructor("entityId", 7 * 22).ReturnsForAnyArgs(GetItemsInFutureWeek(false));
            var result = controller.AgendaFullView("entityId");
            var model = result.Model as AssignmentWidget;
            //Verify sort order for next week assignments within the same day
            var targetGroup = model.Groups.SingleOrDefault(g => g.Assignments != null && g.Assignments.Count == 3);
            Assert.IsNotNull(targetGroup);
            var assignments = targetGroup.Assignments;
            Assert.AreEqual("With first due date - first in list", assignments.First().Title);
            Assert.AreEqual("Then a minute later - last in list", assignments.Last().Title);
        }

        /// <summary>
        /// Mocks list of assigned items
        /// </summary>
        /// <returns></returns>
        private List<Bfw.PX.Biz.DataContracts.ContentItem> GetItems()
        {
            var result = new List<Bfw.PX.Biz.DataContracts.ContentItem>();

            result.Add(new Bfw.PX.Biz.DataContracts.ContentItem() 
            {
                Id = "1",
                Type = "dropbox",
                Title = "title 1",
                SubTitle = "subtitle 1",
                AssignmentSettings = new AssignmentSettings()
                {
                    DueDate = DateTime.Now.AddDays(1).ToUniversalTime()
                }
            });
            result.Add(new Bfw.PX.Biz.DataContracts.ContentItem()
            {
                Id = "2",
                Type = "dropbox",
                Title = "title 2",
                SubTitle = "subtitle 2",
                AssignmentSettings = new AssignmentSettings()
                {
                    DueDate = DateTime.Now.AddDays(2).ToUniversalTime()
                }
            });

            return result;
        }

        /// <summary>
        /// Mocks list of assigned items
        /// </summary>
        /// <returns></returns>
        private List<Bfw.PX.Biz.DataContracts.ContentItem> GetItemsInNext6Months()
        {
            var result = new List<Bfw.PX.Biz.DataContracts.ContentItem>();
            for (int i = 1; i < 6; i++)
            {
                result.Add(new Bfw.PX.Biz.DataContracts.ContentItem()
                {
                    Id = i.ToString(),
                    Type = "dropbox",
                    Title = "title " + i,
                    SubTitle = "subtitle " + i,
                    AssignmentSettings = new AssignmentSettings()
                    {
                        DueDate = DateTime.Now.AddMonths(i).ToUniversalTime()
                    }
                });
            }
            
            return result;
        }

        private List<Bfw.PX.Biz.DataContracts.ContentItem> GetItemsInFutureWeek(bool isNext)
        {
            var result = new List<Bfw.PX.Biz.DataContracts.ContentItem>();
            DateTime baseTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 59, 0);
            int dayOfWeek = (int)baseTime.DayOfWeek;
            DateTime startOfWeek = baseTime.AddDays(-(int)baseTime.DayOfWeek);
            var daysAdded = isNext ? 8 : 16;
            DateTime targetMonday = startOfWeek.AddDays(daysAdded);
            result.Add(new Bfw.PX.Biz.DataContracts.ContentItem()
            {
                Id = "4324321",
                Type = "dropbox",
                Title = "With first due date - first in list",
                SubTitle = "subtitle",
                AssignmentSettings = new AssignmentSettings() { DueDate = targetMonday.ToUniversalTime() }
            });
            result.Add(new Bfw.PX.Biz.DataContracts.ContentItem()
            {
                Id = "4324327",
                Type = "dropbox",
                Title = "Then a minute later - last in list",
                SubTitle = "subtitle",
                AssignmentSettings = new AssignmentSettings() { DueDate = targetMonday.AddMinutes(1).ToUniversalTime() }
            });
            result.Add(new Bfw.PX.Biz.DataContracts.ContentItem()
            {
                Id = "4324329",
                Type = "dropbox",
                Title = "Another at a minute later",
                SubTitle = "subtitle",
                AssignmentSettings = new AssignmentSettings() { DueDate = targetMonday.AddMinutes(1).ToUniversalTime() }
            });
            return result;
        }
    }
}
