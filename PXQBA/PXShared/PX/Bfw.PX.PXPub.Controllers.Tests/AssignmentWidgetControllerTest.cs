using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Widgets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using TestHelper;
using Bfw.PX.Biz.Services.Mappers;
using System.Web.Mvc;
using Bfw.Agilix.Dlap;
using System.Linq;
using System.Xml.Linq;
using Bfw.Agilix.Commands;
using Bfw.Common.Collections;
using System.Collections.Generic;
using System;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Controllers.Contracts;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    [TestClass]
    public class AssignmentWidgetControllerTest
    {
        private AssignmentWidgetController controller;

        private IBusinessContext context;
        private IGradeActions grades;
        private IContentActions content;
        private IAssignmentActions assignments;
        private IAssignmentCenterHelper assignmentHelper;

        [TestInitialize]
        public void TestInitialize()
        { 
            context = Substitute.For<IBusinessContext>();
            grades = Substitute.For<IGradeActions>();
            content = Substitute.For<IContentActions>();
            assignments = Substitute.For<IAssignmentActions>();
            assignmentHelper = Substitute.For<IAssignmentCenterHelper>();

            var course = new Bfw.Agilix.DataContracts.Course();
            course.ParseEntity(Helper.GetResponse(Entity.Course, "GenericCourse").Element("course"));
            course.Id = "1";
            context.Course = course.ToCourse();

            this.controller = new AssignmentWidgetController(context, grades, content, assignments, assignmentHelper);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void View_Rendered_For_Instructor()
        {
            context.AccessLevel = AccessLevel.Instructor;

            var result = controller.View(null);
            var model = (result as ViewResult).Model;

            Assert.AreEqual(typeof(Bfw.PX.PXPub.Models.AssignmentWidget), model.GetType());
            Assert.AreEqual("today", (model as Bfw.PX.PXPub.Models.AssignmentWidget).Groups[0].Title);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void View_Rendered_For_Student()
        {
            context.EnrollmentId = "1";
            context.AccessLevel = AccessLevel.Student;
            var getItems = new GetDueSoonList();
            getItems.ParseResponse(new DlapResponse() { ResponseXml = XDocument.Parse(Helper.GetContent(Entity.GetDueSoonList)) });
            grades.GetDueSoonItemsWithGrades(context.EnrollmentId, -240).ReturnsForAnyArgs(getItems.Grades.Map(o => o.ToGrade()));

            var result = controller.View(null);
            var model = (result as ViewResult).Model;

            Assert.AreEqual(typeof(Bfw.PX.PXPub.Models.AssignmentWidget), model.GetType());
            Assert.AreEqual("today", (model as Bfw.PX.PXPub.Models.AssignmentWidget).Groups[0].Title);
        }    

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void Launchpad_OnRender_For_Instructor_Returns_Assignments()
        {
            context.AccessLevel = AccessLevel.Instructor;
            assignmentHelper.GetDueAssignmentsForInstructor("entityId", 7).ReturnsForAnyArgs(new List<Bfw.PX.Biz.DataContracts.ContentItem>() 
            { 
                GetItem()
            });

            var result = controller.LaunchPad();
            var model = (result as ViewResult).Model;

            Assert.AreEqual(typeof(Bfw.PX.PXPub.Models.UpcommingActivitiesModel), model.GetType());
            Assert.AreEqual(1, (model as Bfw.PX.PXPub.Models.UpcommingActivitiesModel).CountOfAssignments);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void Launchpad_OnRender_For_Student_Returns_Assignments()
        {
            context.AccessLevel = AccessLevel.Student;
            var item = GetItem();
            List<ContentItem> items = new List<ContentItem>() 
            { 
                item
                ,
                new ContentItem()
                {
                    Subtype = "dropbox",
                    AssignmentSettings = new AssignmentSettings()
                    {
                        DueDate = (DateTime.Now).AddHours(1)
                    }
                }
                ,
                new ContentItem()
                {
                    Subtype = "dropbox",
                    AssignmentSettings = new AssignmentSettings()
                    {
                        DueDate = (DateTime.Now).AddHours(1)
                    }
                }
            };
            assignmentHelper.GetDueAssignmentsForStudent(7, false, false).ReturnsForAnyArgs(items);
            content.GetItems("", new List<string>()).ReturnsForAnyArgs(new List<ContentItem>() { item });

            var result = controller.LaunchPad();
            var model = (result as ViewResult).Model;

            Assert.AreEqual(typeof(Bfw.PX.PXPub.Models.UpcommingActivitiesModel), model.GetType());
            Assert.AreEqual(3, (model as Bfw.PX.PXPub.Models.UpcommingActivitiesModel).CountOfAssignments);
        }

        [TestCategory("ContentTreeWidget"), TestMethod]
        public void Launchpad_Should_Not_Have_Assignments()
        {
            context.AccessLevel = AccessLevel.Instructor;
            content.ListContentWithDueDatesWithinRange("", "", "").ReturnsForAnyArgs(new List<Bfw.PX.Biz.DataContracts.ContentItem>() 
            {                 
            });

            var result = controller.LaunchPad();
            var model = (result as ViewResult).Model;

            Assert.AreEqual(null, model);
        }

        private ContentItem GetItem()
        {
            return new ContentItem()
            {
                Id = "1",
                Type = "dropbox",
                Subtype = "dropbox",
                ParentId = "PX_MULTIPART_LESSONS",
                SubContainerIds = new List<Container>()
                {
                    new Container("syllabusfilter", "PX_MULTIPART_LESSONS")
                },
                AssignmentSettings = new AssignmentSettings()
                {
                    DueDate = (DateTime.Now).AddHours(1)
                }
            };
        }

        private ContentItem GetUnit()
        {
            return new ContentItem()
            {
                Id = "1",
                Type = "folder",
                Subtype = "pxunit",
                ParentId = "PX_MULTIPART_LESSONS",
                SubContainerIds = new List<Container>()
                {
                    new Container("syllabusfilter", "PX_MULTIPART_LESSONS")
                },
                AssignmentSettings = new AssignmentSettings()
                {
                    DueDate = (DateTime.Now).AddHours(1),
                    StartDate = (DateTime.Now).AddHours(1)
                }
            };
        }
    }   
}
