using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Bfw.Common.Caching;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Contracts;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Controllers.Widgets;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using FormCollection = System.Web.Mvc.FormCollection;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    [TestClass]
    public class InstructorConsoleWidgetControllerTest
    {
        private InstructorConsoleWidgetController controller;

        private IBusinessContext context;
        private IContentActions contentActions;
        private ICourseActions courseActions;
        private IPageActions pageActions;
        private IAssignmentActions assignmentActions;
        private IGradeActions gradeActions;
        private IEnrollmentActions enrollmentActions;
        private IContentHelper contentHelper;
        private IAssignmentCenterHelper assignmentCenterHelper;
        private IGradebookExportHelper gradebookExportHelper;
        private ICacheProvider cacheProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            context = Substitute.For<IBusinessContext>();
            contentActions = Substitute.For<IContentActions>();
            courseActions = Substitute.For<ICourseActions>();
            pageActions = Substitute.For<IPageActions>();
            assignmentActions = Substitute.For<IAssignmentActions>();
            gradeActions = Substitute.For<IGradeActions>();
            enrollmentActions = Substitute.For<IEnrollmentActions>();
            contentHelper = Substitute.For<IContentHelper>();
            assignmentCenterHelper = Substitute.For<IAssignmentCenterHelper>();
            gradebookExportHelper = Substitute.For<IGradebookExportHelper>();
            cacheProvider = Substitute.For<ICacheProvider>();
            controller = new InstructorConsoleWidgetController(context, contentActions, courseActions, pageActions, assignmentActions, gradeActions, enrollmentActions, contentHelper, assignmentCenterHelper, gradebookExportHelper);

            ConfigurationManager.AppSettings.Set("MyMaterials", "filter");
            InitializeControllerContext();
        }

        [TestCategory("InstructorConsole"), TestMethod]
        public void BatchDueDateUpdater_Rendered()
        {
            var result = controller.BatchDueDateUpdater();

            Assert.AreEqual(result.GetType().FullName, typeof(ViewResult).FullName);
        }

        [TestCategory("InstructorConsole"), TestMethod]
        public void GetBatchDueDateItemCount_ReturnsNumberOfAssignments()
        {
            string fromDate = "01/01/2013";
            string toDate = "02/01/2013";

            contentActions.ListContentWithDueDatesWithinRange("1", fromDate, toDate).ReturnsForAnyArgs(GetContentItemList());

            JsonResult result = controller.GetBatchDueDateItemCount(fromDate, toDate);
            var itemCount = result.Data.GetType().GetProperty("itemCount").GetValue(result.Data, null);

            Assert.AreEqual(2, itemCount);
        }

        [TestCategory("InstructorConsole"), TestMethod]
        public void BatchDueDateUpdate_ReturnsSuccess()
        {
            string fromDate = "01/01/2013";
            string toDate = "02/01/2013";
            DateTime newDate = new DateTime(2013, 3, 1);

            assignmentCenterHelper.ProcessAssignment("", new Models.AssignmentCenterItem(), false, "syllabusfilter", true).ReturnsForAnyArgs(new List<Models.AssignmentCenterItem>());
            contentActions.ListContentWithDueDatesWithinRange("1", fromDate, toDate).ReturnsForAnyArgs(GetContentItemList());

            JsonResult result = controller.BatchDueDateUpdate(fromDate, toDate, false, newDate);

            Assert.AreEqual("success", result.Data.GetType().GetProperty("status").GetValue(result.Data, null).ToString().ToLower());
        }

        [TestCategory("InstructorConsole"), TestMethod]
        public void BatchDueDateUpdate_ReturnsFail()
        {
            string fromDate = "01/01/2013";
            string toDate = "02/01/2013";
            DateTime newDate = new DateTime(2013, 3, 1);

            assignmentCenterHelper.ProcessAssignment("", new Models.AssignmentCenterItem(), false, "syllabusfilter", true).ReturnsForAnyArgs(new List<Models.AssignmentCenterItem>());
            contentActions.ListContentWithDueDatesWithinRange("1", fromDate, toDate).ReturnsForAnyArgs(new List<ContentItem>());

            JsonResult result = controller.BatchDueDateUpdate(fromDate, toDate, false, newDate);

            Assert.AreEqual("fail", result.Data.GetType().GetProperty("status").GetValue(result.Data, null).ToString().ToLower());
        }

        [TestCategory("InstructorConsole"), TestMethod]
        public void BatchDueDateUpdate_Should_Update_RestrictedByDate()
        {
            string fromDate = "01/01/2013";
            string toDate = "01/10/2013";
            DateTime newDate = new DateTime(2013, 3, 1);

            assignmentCenterHelper.ProcessAssignment("", new Models.AssignmentCenterItem(), true, "syllabusfilter", true).ReturnsForAnyArgs(new List<Models.AssignmentCenterItem>());
            contentActions.FindContentItems(new Agilix.DataContracts.ItemSearch()).ReturnsForAnyArgs(GetContentItemList());

            JsonResult result = controller.BatchDueDateUpdate(fromDate, toDate, true, newDate);

            contentActions.ReceivedWithAnyArgs(1).StoreContent(Arg.Any<ContentItem>());
            Assert.AreEqual("success", result.Data.GetType().GetProperty("status").GetValue(result.Data, null).ToString().ToLower());
        }

        [TestCategory("InstructorConsole"), TestMethod]
        public void UpdateUseWeightedCategories_Should_Save_Weight()
        {
            context.Course = new Course() { PassingScore = 100 };
            courseActions.UpdateCourse(new Course()).ReturnsForAnyArgs(context.Course);

            controller.UpdateUseWeightedCategories(new Models.GradebookPreferencesChangeState() { UseWeighted = true });

            Assert.AreEqual(true, context.Course.UseWeightedCategories);
        }

        [TestCategory("InstructorConsole"), TestMethod]
        public void InstructorConsoleWidgetAction_GetDefaultLaunchPadItems_MergesSubcontainersProperly()
        {
            var entityid = "testid";
            var containerid = "containerid";
            context.EntityId.Returns(entityid);

            contentActions.GetContainerItems(entityid, containerid, string.Empty, "syllabusfilter")
                .Returns(new List<ContentItem>()
                {
                    new ContentItem()
                    {
                        Categories = new List<TocCategory>()
                        {
                            new TocCategory()
                            {
                                Id = "blah"
                            }
                        },
                        Containers = new List<Container>()
                        {
                            new Container("syllabusfilter", "containerid")
                        },
                        SubContainerIds = new List<Container>()
                        {
                            new Container("syllabusfilter", string.Empty)
                        }
                    },
                });
            contentActions.GetContainerItems(entityid, containerid, "PX_MULTIPART_LESSONS", "syllabusfilter")
                .Returns(new List<ContentItem>()
                {
                    new ContentItem()
                    {
                        Categories = new List<TocCategory>()
                        {
                            new TocCategory()
                            {
                                Id = "blah"
                            }
                        },
                        Containers = new List<Container>()
                        {
                            new Container("syllabusfilter", "containerid")
                        },
                        SubContainerIds = new List<Container>()
                        {
                            new Container("syllabusfilter", "PX_MULTIPART_LESSONS")
                        }
                    },
                });

            var items = controller.GetDefaultLaunchPadItems(containerid);
            Assert.AreEqual(2, items.Count);
        }

        /// <summary>
        /// Test that we are filtering based on MyMaterials properly
        /// </summary>
        [TestCategory("InstructorConsole"), TestMethod]
        public void InstructorConsoleWidgetAction_GetDefaultLaunchPadItems_FiltersSubContainersProperly()
        {
            var entityid = "testid";
            var containerid = "containerid";
            context.EntityId.Returns(entityid);
            contentActions.GetContainerItems(entityid, containerid, string.Empty, "syllabusfilter")
                .Returns(new List<ContentItem>()
                {
                    new ContentItem()
                    {
                        Categories = new List<TocCategory>()
                        {
                            new TocCategory()
                            {
                                Id = "filter"
                            }
                        },
                        Containers = new List<Container>()
                        {
                            new Container("syllabusfilter", "containerid")
                        },
                        SubContainerIds = new List<Container>()
                        {
                            new Container("syllabusfilter", string.Empty)
                        }
                    },
                });
            contentActions.GetContainerItems(entityid, containerid, "PX_MULTIPART_LESSONS", "syllabusfilter")
                .Returns(new List<ContentItem>()
                {
                    new ContentItem()
                    {
                        Categories = new List<TocCategory>()
                        {
                            new TocCategory()
                            {
                                Id = "blah"
                            }
                        },
                        Containers = new List<Container>()
                        {
                            new Container("syllabusfilter", "containerid")
                        },
                        SubContainerIds = new List<Container>()
                        {
                            new Container("syllabusfilter", "PX_MULTIPART_LESSONS")
                        }
                    },
                });

            var items = controller.GetDefaultLaunchPadItems(containerid);
            Assert.AreEqual(1, items.Count);
        }
		
		/// <summary>
        /// Test if LMS Properties are updated
        /// </summary>
        [TestMethod, TestCategory("LMSIntegration")]
        public void Can_Update_LMSProperties_LMSID_Required()
        {
            context.Course = new Course() { Id = "153135" };
            context.CacheProvider.Returns(cacheProvider);
            context.CurrentUser.Returns(new UserInfo { Id = "15200", ReferenceId = "6669556" });
            var course = context.Course.ToCourse();
            var collection = new FormCollection();
            collection.Add("LMSId Required", "true");
            collection["view"] = "general";
            collection["isUploadValid"] = "false";
            courseActions.GetCourseByCourseId("153135").ReturnsForAnyArgs(new Course());
            var result = controller.Update(course, collection);
            Assert.AreEqual(null, result);    
        }

        [TestCategory("InstructorConsole"), TestMethod]
        public void InstructorConsoleWidgetAction_SetLaunchpadUnits_WithIncludeFalse_UpdatesSubcontainersToLaunchpadRemoved()
        {
            var entityid = "testid";
            context.EntityId.Returns(entityid);
            var item1 = new ContentItem()
            {
                Categories = new List<TocCategory>()
                {
                    new TocCategory()
                    {
                        Id = "blah"
                    }
                },
                Containers = new List<Container>()
                {
                    new Container("syllabusfilter", "Launchpad")
                },
                SubContainerIds = new List<Container>()
                {
                    new Container("syllabusfilter", string.Empty)
                }
            };
            var item2 =new ContentItem()
            {
                Categories = new List<TocCategory>()
                {
                    new TocCategory()
                    {
                        Id = "blah"
                    }
                },
                Containers = new List<Container>()
                {
                    new Container("syllabusfilter", "Launchpad")
                },
                SubContainerIds = new List<Container>()
                {
                    new Container("syllabusfilter", "PX_MULTIPART_LESSONS")
                }
            };
            contentActions.GetContainerItems(entityid, "Launchpad", string.Empty, "syllabusfilter")
                .Returns(new List<ContentItem>() { item1 });
            contentActions.GetContainerItems(entityid, "Launchpad", "PX_MULTIPART_LESSONS", "syllabusfilter")
                .Returns(new List<ContentItem>() { item2 });

            var json = controller.SetLaunchpadUnits(false);
            Assert.AreEqual("LaunchPadRemoved", item1.GetContainer());
            Assert.AreEqual("LaunchPadRemoved", item2.GetContainer());
        }

        [TestCategory("InstructorConsole"), TestMethod]
        public void InstructorConsoleWidgetAction_SetLaunchpadUnits_WithIncludeTrue_UpdatesSubcontainersToLaunchpad()
        {
            var entityid = "testid";
            context.EntityId.Returns(entityid);
            var item1 = new ContentItem()
            {
                Categories = new List<TocCategory>()
                {
                    new TocCategory()
                    {
                        Id = "blah"
                    }
                },
                Containers = new List<Container>()
                {
                    new Container("syllabusfilter", "LaunchPadRemoved")
                },
                SubContainerIds = new List<Container>()
                {
                    new Container("syllabusfilter", string.Empty)
                }
            };
            var item2 = new ContentItem()
            {
                Categories = new List<TocCategory>()
                {
                    new TocCategory()
                    {
                        Id = "blah"
                    }
                },
                Containers = new List<Container>()
                {
                    new Container("syllabusfilter", "LaunchPadRemoved")
                },
                SubContainerIds = new List<Container>()
                {
                    new Container("syllabusfilter", "PX_MULTIPART_LESSONS")
                }
            };
            contentActions.GetContainerItems(entityid, "LaunchPadRemoved", string.Empty, "syllabusfilter")
                .Returns(new List<ContentItem>() { item1 });
            contentActions.GetContainerItems(entityid, "LaunchPadRemoved", "PX_MULTIPART_LESSONS", "syllabusfilter")
                .Returns(new List<ContentItem>() { item2 });

            var json = controller.SetLaunchpadUnits(true);
            Assert.AreEqual("Launchpad", item1.GetContainer());
            Assert.AreEqual("Launchpad", item2.GetContainer());
        }
		
		/// <summary>
        /// Test if LMS Properties are updated
        /// </summary>
        [TestMethod, TestCategory("LMSIntegration")]
        public void Can_Update_LMSProperties_LMSID_Label()
        {
            context.Course = new Course() { Id = "153135" };
            context.CacheProvider.Returns(cacheProvider);
            context.CurrentUser.Returns(new UserInfo { Id = "15200", ReferenceId = "6669556" });
            var course = context.Course.ToCourse();
            var collection = new FormCollection();
            collection.Add("LMSId Label", "LMS Id");
            collection["view"] = "general";
            collection["isUploadValid"] = "false";
            courseActions.GetCourseByCourseId("153135").ReturnsForAnyArgs(new Course());
            var result = controller.Update(course, collection);
            Assert.AreEqual(null, result);   
        }

        [TestMethod]
        public void Update_Should_Return_Validation_Error_If_Invalid_Url()
        {
            var course = new Models.Course()
            {                
                AllowedThemes = "theme",
                DashboardSettings = new Models.DashboardSettings()
            };
            courseActions.GetCourseByCourseId(string.Empty).Returns(course.ToCourse());
            var formCollection = new FormCollection();
            formCollection.Add("View", "general");
            formCollection.Add("SyllabusType", "Url");
            formCollection.Add("SyllabusURL", "dummy url");
 
            var result = controller.Update(course, formCollection);
 
            Assert.IsTrue((result as JsonResult).Data.ToString().IndexOf("Please specify valid syllabus url") > 0);
        }
 
        [TestMethod]
        public void Update_Should_Save_Empty_Url()
        {
            context.CurrentUser = new UserInfo();
            var course = new Models.Course()
            {
                AllowedThemes = "theme",
                DashboardSettings = new Models.DashboardSettings()
            };
            courseActions.GetCourseByCourseId(string.Empty).Returns(course.ToCourse());
            var formCollection = new FormCollection();
            formCollection.Add("View", "general");
            formCollection.Add("SyllabusType", "Url");
            formCollection.Add("SyllabusURL", "");
 
            var result = controller.Update(course, formCollection);
 
            courseActions.Received(1).UpdateCourse(Arg.Is<Course>(c => c.Syllabus == string.Empty));
        }

        [TestMethod]
        public void ExportGradebook()
        {
            var enrollments = new List<Enrollment>();
            enrollmentActions.GetEntityEnrollmentsWithGrades(context.EntityId).Returns(enrollments);
            gradebookExportHelper.GetCsvString(enrollments).Returns("dummy string");

            var result = controller.ExportGradebook();
            var stream = (result as FileStreamResult).FileStream;
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, (int)stream.Length);            

            Assert.AreEqual("dummy string", System.Text.Encoding.Unicode.GetString(bytes));
        }

        /// <summary>
        /// When instructor changes the lms label to empty, the label should be changed to the default.
        /// </summary>
        [TestCategory("InstructorConsole"), TestMethod]
        public void WhenPutEmptyLmsLabel_ShouldUseTheDefaultLabel()
        {
            context.CurrentUser = new UserInfo();
            var course = new Models.Course()
            {
                AllowedThemes = "theme",
                DashboardSettings = new Models.DashboardSettings(),
                LmsIdLabel = ""
            };
            courseActions.GetCourseByCourseId(string.Empty).Returns(course.ToCourse());
            var formCollection = new FormCollection();
            formCollection.Add("View", "general");
            formCollection.Add("isUploadValid", "false");
            controller.Update(course, formCollection);

            courseActions.Received(1).UpdateCourse(Arg.Is<Course>(c => c.LmsIdLabel == "Students, enter your Campus ID here:"));
        }

        private void InitializeControllerContext()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            var request = Substitute.For<HttpRequestBase>();
            var requestContext = Substitute.For<RequestContext>();

            var routeData = new RouteData();
            requestContext.RouteData = routeData;

            controller.ControllerContext = new ControllerContext() { HttpContext = httpContext, RequestContext = requestContext };
            controller.ControllerContext.RouteData.Returns(routeData);
        }

        private List<Bfw.PX.Biz.DataContracts.ContentItem> GetContentItemList()
        {
            List<Bfw.PX.Biz.DataContracts.ContentItem> result = new List<Biz.DataContracts.ContentItem>();

            IDictionary<string, MetadataValue> dict = new Dictionary<string, MetadataValue>();
            dict.Add("AgilixDisciplineId", new MetadataValue() { Value = "0" });

            result.Add(new Biz.DataContracts.ContentItem()
            {
                Id = "1",
                Title = "Simple Title 1",
                Sequence = "a",
                Metadata = dict,                 
                Subtype = "Assignment",
                Type = "Assignment",
                AssignmentSettings = new Biz.DataContracts.AssignmentSettings() 
                    { 
                        DueDate = new DateTime(2013, 1, 10), 
                        DueDateTZ = new Common.DateTimeWithZone(new DateTime(2013, 1, 10), TimeZoneInfo.Local, false), 
                        CategorySequence = "a" 
                    }
            });
            result.Add(new Biz.DataContracts.ContentItem()
            {
                Id = "2",
                Title = "Simple Title 2",
                Sequence = "b",
                Metadata = dict,
                Subtype = "Assignment",
                Type = "Assignment",
                AssignmentSettings = new Biz.DataContracts.AssignmentSettings()
                {
                    DueDate = new DateTime(2013, 1, 20),
                    DueDateTZ = new Common.DateTimeWithZone(new DateTime(2013, 1, 20), TimeZoneInfo.Local, false),
                    CategorySequence = "a"
                },
                Visibility = "<bfw_visibility><roles><instructor /><student><restriction><date endate=\"2013-01-05T05:00:00Z\" /></restriction></student></roles></bfw_visibility>"
            });
            result.Add(new Biz.DataContracts.ContentItem() 
            { 
                Id = "3",
                Title = "Unit Title 1",
                Sequence = "c",
                Metadata = dict,
                Subtype = "PxUnit",
                Type = "PxUnit",
                AssignmentSettings = new Biz.DataContracts.AssignmentSettings()
                {
                    DueDate = new DateTime(2013, 1, 15),
                    DueDateTZ = new Common.DateTimeWithZone(new DateTime(2013, 1, 15), TimeZoneInfo.Local, false),
                    CategorySequence = "a"
                }

            });

            return result;
        }
    }
}
