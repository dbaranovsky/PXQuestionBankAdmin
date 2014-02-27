using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Bfw.Agilix.Dlap;
using Bfw.Common.Collections;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.Biz.Direct.Services.Helper;
using Microsoft.Practices.ServiceLocation;
using NSubstitute;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;
using System.Collections.Generic;
using Bfw.Agilix.Commands;
using Bfw.PX.Biz.DataContracts;
using Bfw.Common.Database;
using System.Data.Common;
using Bfw.Common.Caching;
using Bfw.PX.Biz.Direct.Services;

namespace Bfw.PX.Biz.Direct.Services.Tests
{
    using System.ComponentModel;
    using System.Windows;
    using System.Reflection;
    using TestHelper;

    [TestClass]
    public class ContentActionsTest
    {
        private IBusinessContext context;
        private ISessionManager sessionManager;
        private IDocumentConverter documentConverter;
        private IDatabaseManager dbManager;
        private ContentActions contentActions;
        private ISession session;
        private ICacheProvider cacheProvider;
        private IServiceLocator serviceLocator;        
        private IItemQueryActions itemQueryActions;

        [TestInitialize]
        public void TestInitialize()
        {
            context = Substitute.For<IBusinessContext>();
            sessionManager = Substitute.For<ISessionManager>();
            documentConverter = Substitute.For<IDocumentConverter>();
            dbManager = Substitute.For<IDatabaseManager>();
            itemQueryActions = Substitute.For<IItemQueryActions>();
            session = Substitute.For<ISession>();
            sessionManager.CurrentSession.Returns(session);
            contentActions = new ContentActions(context, sessionManager, documentConverter, dbManager, itemQueryActions);
            cacheProvider = Substitute.For<ICacheProvider>();
            context.CacheProvider.Returns(cacheProvider);

            context.Course = new DataContracts.Course()
            {
                CourseType = "LMS"
            };

            context.CurrentUser = new UserInfo()
            {
                Id = "userId"
            };

            serviceLocator = Substitute.For<IServiceLocator>();
            serviceLocator.GetInstance<IBusinessContext>().Returns(context);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
        }

        [TestMethod]
        public void CanGetResourceContent_FromUploadedDocument_InPresentationCourse()
        {
            var resourcePath = "Assets/Test.docx";
            var entityId = "234554";
            var studentDashboradEnrollmentId = "173781";
            var itemId = "fb741536702745d7b9be2345c34c3d24";
            var item = new Item { Href = resourcePath };
            session.When(s => s.ExecuteAsAdmin(Arg.Any<GetItems>())).Do(s =>
            {
                s.Arg<GetItems>().Items = new List<Item>();
                s.Arg<GetItems>().Items.Add(item);
            });
            session.When(s => s.Execute(Arg.Any<GetResource>())).Do(s => { s.Arg<GetResource>().Resource = new Bfw.Agilix.DataContracts.Resource { Url = resourcePath }; });
            ContentItem contentItem = contentActions.GetContent(entityId, studentDashboradEnrollmentId, itemId, true);
            Assert.IsNotNull(contentItem);
            Assert.IsNotNull(contentItem.Resources);
            Assert.AreEqual(resourcePath, contentItem.Resources.First().Url);
        }

        [TestMethod]
        public void CanGetContentResourceWithRubric()
        {
            var resourcePath = "Templates/Data/6e1aa3132b8d49cba67ef83c1fbdb0a6/index.html";
            var rubricPath = "Templates/Data/c1547871ef5f46438d051ac166737e0d/_rubric.xml";
            var entityId = "129889";
            context.EntityId.Returns(entityId);
            var itemId = "6e1aa3132b8d49cba67ef83c1fbdb0a6";
            var item = new Item { Href = resourcePath, HasRubric = true, Rubric = rubricPath };
            item.Data = XElement.Parse(@"<data>
                                          <bfw_properties>
                                            <bfw_property name='bfw_SendReminder' type='Boolean'>true</bfw_property>
                                          </bfw_properties>
                                        </data>");
            session.When(s => s.ExecuteAsAdmin(Arg.Any<GetItems>())).Do(s =>
            {
                s.Arg<GetItems>().Items = new List<Item>();
                s.Arg<GetItems>().Items.Add(item);
            });
            session.When(s => s.Execute(Arg.Is<GetResource>(x => x.ResourcePath == resourcePath))).Do(s => { s.Arg<GetResource>().Resource = new Bfw.Agilix.DataContracts.Resource { Url = resourcePath }; });
            session.When(s => s.Execute(Arg.Is<GetResource>(x => x.ResourcePath == rubricPath))).Do(s => { s.Arg<GetResource>().Resource = new Bfw.Agilix.DataContracts.Resource { Url = rubricPath }; });
            var record = new DatabaseRecord();
            record["SendOnDate"] = DateTime.Now;
            record["EmailBody"] = "Hi";
            record["EmailSubject"] = "Important";
            dbManager.Query("", new object()).ReturnsForAnyArgs(new List<DatabaseRecord> { record });
            ContentItem contentItem = contentActions.GetContent(entityId, itemId, true);
            Assert.AreEqual(resourcePath, contentItem.Resources.First().Url);
            Assert.IsNotNull(contentItem.Rubric);
            Assert.AreEqual(rubricPath, contentItem.Rubric.Url);
        }

        [TestMethod]
        public void Email_Reminder_Duration_Zero_Difference()
        {
            // Assign
            var reminderEmail = new ReminderEmail();
            var testSpan = new TimeSpan(days:0, hours: 0, minutes:0, seconds:0);

            // Act
            contentActions.CalculateDurationTypeAndDaysBefore(testSpan, reminderEmail);

            // Assert
            Assert.AreEqual(0, reminderEmail.DaysBefore, "Calculate difference for reminder email for edge case of 0 is not working in ContentActions.");
        }

        [TestMethod]
        public void Email_Reminder_Duration_24_Hours_TimeDfference()
        {
            // Assign
            var reminderEmail = new ReminderEmail();
            var testSpan = new TimeSpan(days: 0, hours: 24, minutes: 0, seconds: 0);

            // Act
            contentActions.CalculateDurationTypeAndDaysBefore(testSpan, reminderEmail);

            // Assert
            Assert.AreEqual("day", reminderEmail.DurationType );
            Assert.AreEqual(1, reminderEmail.DaysBefore, "Calculate difference for reminder email for day is not working in ContentActions.");
        }

        [TestMethod]
        public void Email_Reminder_Duration_23_Hours_TimeDfference()
        {
            // Assign
            var reminderEmail = new ReminderEmail();
            var testSpan = new TimeSpan(days: 0, hours: 23, minutes: 0, seconds: 0);

            // Act
            contentActions.CalculateDurationTypeAndDaysBefore(testSpan, reminderEmail);

            // Assert
            Assert.AreEqual("hour", reminderEmail.DurationType);
            Assert.AreEqual(23, reminderEmail.DaysBefore, "Calculate difference for reminder email for hors is not working in ContentActions.");
        }

        [TestMethod]
        public void Email_Reminder_Duration_25_Hours_TimeDfference()
        {
            // Assign
            var reminderEmail = new ReminderEmail();
            var testSpan = new TimeSpan(days: 0, hours: 25, minutes: 0, seconds: 0);

            // Act
            contentActions.CalculateDurationTypeAndDaysBefore(testSpan, reminderEmail);

            // Assert
            Assert.AreEqual("hour", reminderEmail.DurationType );
            Assert.AreEqual(25, reminderEmail.DaysBefore, "Calculate difference for reminder email for hors is not working in ContentActions.");
        }

        [TestMethod]
        public void Email_Reminder_Duration80MinuteDifference()
        {
            // Assign
            var reminderEmail = new ReminderEmail();
            var testSpan = new TimeSpan(days: 0, hours: 0, minutes: 80, seconds: 0);

            // Act
            contentActions.CalculateDurationTypeAndDaysBefore(testSpan, reminderEmail);

            // Assert
            Assert.AreEqual("minute", reminderEmail.DurationType);
            Assert.AreEqual(80, reminderEmail.DaysBefore, "Calculate difference for reminder email for minutes is not working in ContentActions.");
        }

        [TestMethod]
        public void Email_Reminder_Duration2WeekDifferenceFrom14Days()
        {
            // Assign
            var reminderEmail = new ReminderEmail();
            var testSpan = new TimeSpan(days: 14, hours: 0, minutes: 0, seconds: 0);

            // Act
            contentActions.CalculateDurationTypeAndDaysBefore(testSpan, reminderEmail);

            // Assert
            Assert.AreEqual("week", reminderEmail.DurationType);
            Assert.AreEqual(2, reminderEmail.DaysBefore, "Calculate difference for reminder email for week is not working in ContentActions.");
        }

        [TestMethod]
        public void Email_Reminder_Duration13Days()
        {
            // Assign
            var reminderEmail = new ReminderEmail();
            var testSpan = new TimeSpan(days: 13, hours: 0, minutes: 0, seconds: 0);

            // Act
            contentActions.CalculateDurationTypeAndDaysBefore(testSpan, reminderEmail);

            // Assert
            Assert.AreEqual("day", reminderEmail.DurationType);
            Assert.AreEqual(13, reminderEmail.DaysBefore, "Calculate difference for reminder email for day is not working in ContentActions.");
        }

        [TestMethod]
        public void Email_Reminder_Duration15Days()
        {
            // Assign
            var reminderEmail = new ReminderEmail();
            var testSpan = new TimeSpan(days: 15, hours: 0, minutes: 0, seconds: 0);

            // Act
            contentActions.CalculateDurationTypeAndDaysBefore(testSpan, reminderEmail);

            // Assert
            Assert.AreEqual("day", reminderEmail.DurationType);
            Assert.AreEqual(15, reminderEmail.DaysBefore, "Calculate difference for reminder email for day is not working in ContentActions.");
        }

        [TestMethod]
        public void GetReminderEmailDetailsWhenRecordDoesNotExist()
        {
            // Assign
            var entityId = "entityId";
            var itemId = "itemId";
            var dueDate = new DateTime(year: 2013, month: 08, day: 08);

            dbManager.Query(Arg.Any<string>(), Arg.Any<object[]>())
                .ReturnsForAnyArgs(a => new BindingList<DatabaseRecord>());

            // Act
            var reminderEmail = contentActions.GetReminderMailDetails(entityId, itemId, dueDate);

            // Assert
            dbManager.Received().ConfigureConnection(Arg.Any<string>());
            Assert.IsNull(reminderEmail.Body,"Failed - Reminder Email Body has some content.");
        }

        [TestMethod]
        public void GetReminderEmailDetailsWhenRecordExists()
        {
            // Assign
            var entityId = "entityId";
            var itemId = "itemId";
            var dueDate = new DateTime(year: 2013, month: 08, day: 08);

            var record = new DatabaseRecord();
            record["SendOnDate"] = DateTime.Parse("8/02/2013");
            record["EmailBody"] = "Test";
            record["EmailSubject"] = "Test Message";
            dbManager.Query("", new object()).ReturnsForAnyArgs(new List<DatabaseRecord> { record });
            
            // Act
            var reminderEmail = contentActions.GetReminderMailDetails(entityId, itemId, dueDate);

            // Assert
            dbManager.Received().ConfigureConnection(Arg.Any<string>());
            Assert.AreEqual("day",reminderEmail.DurationType,"Duration type was expected as day");
        }

        [TestMethod]
        public void Remove_CDATA_Tag_From_HTML()
        {
            // lets create three-level deep CDATA tags to represent content blocks plus another CDATA
            string initialString = @"<![CDATA[<![CDATA[<![CDATA[<![CDATA[]]>]]>]]>]]><![CDATA[<![CDATA[]]]]>>";

            string result = HtmlXmlHelper.CleanupHtmlString(initialString);

            Assert.IsTrue(result.Length == 0, "After clean up of html string it should be blank string");
        }

        [TestMethod]
        public void Content_Without_CDATA_Tag_ReturnsSameResult()
        {
            string initialString = @"<html><title>test</title><body>This is a test html body</body></html>";

            string result = HtmlXmlHelper.CleanupHtmlString(initialString);

            Assert.AreEqual(initialString, result, "After clean up of html without CDATA string it should be same string");
        }

        [TestMethod]
        public void Content_With_Open_SPAN_Tag_with_fck_bookmark_attribute_MustBe_Closed()
        {
            string initialString = @"<span _fck_bookmark=""true"" id=""1342650118259S"" /><span _fck_bookmark=""true"" id=""something"" > sample>.";
            string expectedString = @"<span _fck_bookmark=""true"" id=""1342650118259S"" /><span _fck_bookmark=""true"" id=""something"" /> sample>.";

            string cleanupString = HtmlXmlHelper.CleanupHtmlString(initialString);

            Assert.AreEqual(expectedString, cleanupString, "After clean up of html span tag with _fck_bookmark attribute must be closed.");
        }

        [TestMethod]
        public void Content_With_META_Tag_MustBe_Closed()
        {
            string initialString = @"<meta content=""Microsoft Word 11"" name=""Generator"" >";
            string expectedString = @"<meta content=""Microsoft Word 11"" name=""Generator"" />";

            string result = HtmlXmlHelper.CleanupHtmlString(initialString);

            Assert.AreEqual(expectedString, result, "After clean up of html meta tag must be closed");
        }

        [TestMethod]
        public void Content_With_link_Tag_MustBe_Closed()
        {
            string initialString = @"<link href=""file:///C:\\clip_filelist.xml"" rel=""File-List"" >";
            string expectedString = @"<link href=""file:///C:\\clip_filelist.xml"" rel=""File-List"" />";

            string result = HtmlXmlHelper.CleanupHtmlString(initialString);

            Assert.AreEqual(expectedString, result, "After clean up of html meta tag must be closed");
        }

        //[TestMethod]
        //[ExpectedException(typeof(ArgumentNullException), "items")]
        //public void ListChildren_Should_Throw_Exception_If_No_Items_Found()
        //{
        //    var result = contentActions.ListChildren("entityId", "parentId", "enrollment_12345", "userId", false);
        //}

        [TestMethod]
        public void ListChildren_Should_Return_Children()
        {
            context.Course.CourseType = CourseType.PersonalEportfolioPresentation.ToString();
            var parentItems = new List<Item>() 
            { 
                new Item()
                {
                    Id = "1"
                },
            };
            var childItems = new List<Item>() 
            { 
                new Item()
                {
                    Id = "2",
                    ParentId = "1"
                },
            };
            sessionManager.CurrentSession.WhenForAnyArgs(o => o.ExecuteAsAdmin(Arg.Any<Batch>())).Do(o =>
            {
                Type type = o.Arg<Batch>().GetType();
                var commands = from k in (Dictionary<string, DlapCommand>)(type.GetProperty("CommandSet", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(o.Arg<Batch>(), null))
                          select k.Value;
                (commands.ToArray()[0] as GetItems).Items = childItems;
                (commands.ToArray()[1] as GetItems).Items = parentItems;
            });

            var result = contentActions.ListChildren("entityId", "parentId", "enrollment_12345", "userId", false);
            parentItems.AddRange(childItems);

            Assert.IsTrue(ObjectComparer.AreObjectsEqual(parentItems.OrderBy(o => o.Id), result.OrderBy(o => o.Id).ToList()));
        }

        [TestMethod]
        public void ListDescendents_Should_Return_Descendents()
        {
            var childItems = new List<Item>() 
            { 
                new Item()
                {
                    Id = "1",
                    Children = new List<Item>()
                    {
                        new Item() 
                        {
                            Id = "2",
                            ParentId = "1"
                        }
                    }
                },
                new Item()
                {
                    Id = "2",
                    ParentId = "1"
                },
            };
            sessionManager.CurrentSession.WhenForAnyArgs(o => o.ExecuteAsAdmin(Arg.Any<GetItems>())).Do(o =>
            {
                if (o.Arg<GetItems>().SearchParameters != null)
                {
                    if (o.Arg<GetItems>().SearchParameters.EntityId == "entityId" && o.Arg<GetItems>().SearchParameters.ItemId == "itemId")
                    {
                        o.Arg<GetItems>().Items = childItems;
                    }
                }
            });
            sessionManager.CurrentSession.WhenForAnyArgs(o => o.Execute(Arg.Any<GetResource>())).Do(o =>
            {
                o.Arg<GetResource>().Resource = new Agilix.DataContracts.Resource() { Extension = "doc" };
            });

            var result = contentActions.ListDescendents("entityId", "itemId", "userId");

            Assert.IsTrue(ObjectComparer.AreObjectsEqual(childItems.First(), result.First()));
        }

        [TestMethod]
        public void DoItemSearch_Should_Return_Search_Results()
        {
            var items = new List<Item>() 
            { 
                new Item()
                {
                    Id = "2",
                    ParentId = "1"
                }
            };
            sessionManager.CurrentSession.WhenForAnyArgs(o => o.ExecuteAsAdmin(Arg.Any<Batch>())).Do(o =>
            {
                Type type = o.Arg<Batch>().GetType();
                var commands = from k in (Dictionary<string, DlapCommand>)(type.GetProperty("CommandSet", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(o.Arg<Batch>(), null))
                               select k.Value;
                (commands.ToArray()[0] as GetItems).Items = items;
            });
            var queryParams = new System.Collections.Generic.Dictionary<string, string>();
            itemQueryActions.BuildItemSearchQuery("entityId", queryParams, "userId", "or").ReturnsForAnyArgs(new ItemSearch());

            var result = contentActions.DoItemSearch("entityId", queryParams, "userId");

            Assert.AreEqual(items.First(), result.First());
        }

        [TestMethod]
        public void GetChildItems_Should_Return_List_Of_Items()
        {
            var items1 = new List<Item>() 
            { 
                new Item()
                {
                    Id = "1",
                }
            };
            var items2 = new List<Item>()
            {
                new Item()
                {
                    Id = "2",
                }
            };
            sessionManager.CurrentSession.WhenForAnyArgs(o => o.ExecuteAsAdmin(Arg.Any<Batch>())).Do(o =>
            {
                Type type = o.Arg<Batch>().GetType();
                var commands = from k in (Dictionary<string, DlapCommand>)(type.GetProperty("CommandSet", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(o.Arg<Batch>(), null))
                               select k.Value;
                (commands.ToArray()[0] as GetItems).Items = items1;
                (commands.ToArray()[1] as GetItems).Items = items2;
            });

            var result = contentActions.GetChildItems("entityId", new List<string>() { "1", "2" }, "toc", false);

            Assert.IsTrue(result.Count(o => o.Id.Equals("1")) > 0);
            Assert.IsTrue(result.Count(o => o.Id.Equals("2")) > 0);
        }

        [TestMethod]
        public void GetItems_Should_Return_List_Of_Items()
        {
            var items1 = new List<Item>() 
            { 
                new Item()
                {
                    Id = "1",
                }
            };
            var items2 = new List<Item>()
            {
                new Item()
                {
                    Id = "2",
                }
            };
            sessionManager.CurrentSession.WhenForAnyArgs(o => o.ExecuteAsAdmin(Arg.Any<Batch>())).Do(o =>
            {
                Type type = o.Arg<Batch>().GetType();
                var commands = from k in (Dictionary<string, DlapCommand>)(type.GetProperty("CommandSet", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(o.Arg<Batch>(), null))
                               select k.Value;
                (commands.ToArray()[0] as GetItems).Items = items1;
                (commands.ToArray()[1] as GetItems).Items = items2;
            });

            var result = contentActions.GetItems("entityId", new List<string>() { "1", "2" }, false).ToList();

            Assert.IsTrue(result.Count(o => o.Id.Equals("1")) > 0);
            Assert.IsTrue(result.Count(o => o.Id.Equals("2")) > 0);
        }

        [TestMethod]
        public void DoItemsSearch_Should_Use_CourseId_For_Instructor()
        {
            context.AccessLevel = AccessLevel.Instructor;
            itemQueryActions.BuildItemSearchQuery("", new Dictionary<string, string>(), "", "").ReturnsForAnyArgs(new ItemSearch()
            {
                EntityId = "entityId"
            });
            SetEnvironmentForDoItemsSearch();

            var result = contentActions.DoItemsSearch(context.EntityId, new Dictionary<string, string>());
            var calls = session.ReceivedCalls();
            var batch = calls.First().GetArguments().First() as Batch;

            Assert.AreEqual(context.EntityId, batch.Commands.First().ToRequest().Parameters.Single(p => p.Key.Equals("entityid")).Value);
        }

        [TestMethod]
        public void DoItemsSearch_Should_Use_ProductCourseId_For_Instructor()
        {
            context.AccessLevel = AccessLevel.Instructor;
            itemQueryActions.BuildItemSearchQuery("", new Dictionary<string, string>(), "", "").ReturnsForAnyArgs(new ItemSearch()
            {
                EntityId = "productCourseId"
            });
            SetEnvironmentForDoItemsSearch();

            var result = contentActions.DoItemsSearch(context.ProductCourseId, new Dictionary<string, string>());
            var calls = session.ReceivedCalls();
            var batch = calls.First().GetArguments().First() as Batch;

            Assert.AreEqual(context.ProductCourseId, batch.Commands.First().ToRequest().Parameters.Single(p => p.Key.Equals("entityid")).Value);
        }

        [TestMethod]
        public void DoItemsSearch_Should_Use_EnrollmentId_For_Student()
        {
            context.AccessLevel = AccessLevel.Student;
            itemQueryActions.BuildItemSearchQuery("", new Dictionary<string, string>(), "", "").ReturnsForAnyArgs(new ItemSearch()
            {
                EntityId = "enrollmentId"
            });
            SetEnvironmentForDoItemsSearch();

            var result = contentActions.DoItemsSearch(context.EntityId, new Dictionary<string, string>());
            var calls = session.ReceivedCalls();
            var batch = calls.First().GetArguments().First() as Batch;

            Assert.AreEqual(context.EnrollmentId, batch.Commands.First().ToRequest().Parameters.Single(p => p.Key.Equals("entityid")).Value);
        }

        [TestMethod]
        public void DoItemsSearch_Should_Use_ProductCourseId_For_Student()
        {
            context.AccessLevel = AccessLevel.Student;
            itemQueryActions.BuildItemSearchQuery("", new Dictionary<string, string>(), "", "").ReturnsForAnyArgs(new ItemSearch()
            {
                EntityId = "productCourseId"
            }); SetEnvironmentForDoItemsSearch();

            var result = contentActions.DoItemsSearch(context.ProductCourseId, new Dictionary<string, string>());
            var calls = session.ReceivedCalls();
            var batch = calls.First().GetArguments().First() as Batch;

            Assert.AreEqual(context.ProductCourseId, batch.Commands.First().ToRequest().Parameters.Single(p => p.Key.Equals("entityid")).Value);
        }

        private void SetEnvironmentForDoItemsSearch()
        {
            context.EntityId.Returns("entityId");
            context.EnrollmentId.Returns("enrollmentId");
            context.ProductCourseId.Returns("productCourseId");
        }

        /// <summary>
        /// When items are assigned via the launchpad, we should set the gradable flag = true in DLAP
        /// (Except for folders)
        /// </summary>
        [TestMethod]
        public void UpdateAssignmentCenterItems_ShouldSetGradableWhenAssigned()
        {
            var dlapItem = new Item()
            {
                Id = "itemid",
                EntityId = "entityid",
                IsGradable = false
            };
            session.When(s => s.ExecuteAsAdmin(Arg.Any<Batch>())).Do(s =>
            {
                s.Arg<Batch>().CommandAs<GetItems>("itemid").Items = new List<Item>();
                s.Arg<Batch>().CommandAs<GetItems>("itemid").Items.Add(dlapItem);
            });

            var assignmentItem = new AssignmentCenterItem()
            {
                Id = "itemid",
                EndDate = DateTime.Now, //End Date > 0 determines if item is assigned/gradable
            };

            Item result = null;
            session.When(s => s.ExecuteAsAdmin(Arg.Any<PutItems>())).Do(s =>
            {
                result = s.Arg<PutItems>().Items.FirstOrDefault();
            });

            contentActions.UpdateAssignmentCenterItems("", new List<AssignmentCenterItem>() {assignmentItem}, "", "entityid");

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsGradable);
        }

        /// <summary>
        ///A test for CopyItem
        ///</summary>
        [TestMethod()]
        public void CopyItem_ShouldClearGradableFlag()
        {
            string entityId = "entityId";
            string fromItemId = "item_to_copy";
            string toItemId = "new_item_id";
            string parentId = "parentItem";
            IEnumerable<TocCategory> categories = null; 
            bool removeDesc = true; 
            string title = "new_title";
            string subTitle = "new_subtitle"; 
            string description = "new_description";
            bool? hiddenFromStudent = null;
            bool? includePoints = false;

            var sourceItemXml = Helper.GetXDocument("AssignedQuiz");
            var item = new Item();
            item.ParseEntity(sourceItemXml.Root);

            session.When(s => s.ExecuteAsAdmin(Arg.Any<GetRawItem>())).Do(s =>
            {
                s.Arg<GetRawItem>().ItemDocument = sourceItemXml;
                s.Arg<GetRawItem>().ItemId = fromItemId;
            });
            Assert.IsTrue(item.IsGradable);
            var result = contentActions.CopyItem(entityId, fromItemId, toItemId, parentId, categories, removeDesc, title, subTitle, description, hiddenFromStudent, includePoints);
            Assert.AreEqual(result.Id, toItemId);
            Assert.IsFalse(result.AssignmentSettings.IsGradeable);
            Assert.IsFalse(result.ToItem().IsGradable);
        }

        /// <summary>
        /// When items are assigned via the launchpad, we should NOT set  the gradable flag = true in DLAP
        /// For folder pxunits
        /// </summary>
        [TestMethod]
        public void UpdateAssignmentCenterItems_ShouldSetNotSetGradable_ForFolders()
        {
            var dlapItem = new Item()
            {
                Id = "itemid",
                EntityId = "entityid",
                IsGradable = false,
                Type = DlapItemType.Folder
            };
            var type = new XElement("bfw_type");
            type.Value = "PxUnit";
            dlapItem.Data.Add(type);

            session.When(s => s.ExecuteAsAdmin(Arg.Any<Batch>())).Do(s =>
            {
                s.Arg<Batch>().CommandAs<GetItems>("itemid").Items = new List<Item>();
                s.Arg<Batch>().CommandAs<GetItems>("itemid").Items.Add(dlapItem);
            });

            var assignmentItem = new AssignmentCenterItem()
            {
                Id = "itemid",
                EndDate = DateTime.Now //End Date > 0 determines if item is assigned/gradable
            };

            Item result = null;
            session.When(s => s.ExecuteAsAdmin(Arg.Any<PutItems>())).Do(s =>
            {
                result = s.Arg<PutItems>().Items.FirstOrDefault();
            });

            contentActions.UpdateAssignmentCenterItems("", new List<AssignmentCenterItem>() { assignmentItem }, "", "entityid");

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsGradable);
        }

        /// <summary>
        /// When items are unassigned via the launchpad, we should set the gradable flag = false in DLAP
        /// </summary>
        [TestMethod]
        public void UpdateAssignmentCenterItems_ShouldSetGradable_False_WhenUnAssigned()
        {
            var dlapItem = new Item()
            {
                Id = "itemid",
                EntityId = "entityid",
                IsGradable = true
            };
            session.When(s => s.ExecuteAsAdmin(Arg.Any<Batch>())).Do(s =>
            {
                s.Arg<Batch>().CommandAs<GetItems>("itemid").Items = new List<Item>();
                s.Arg<Batch>().CommandAs<GetItems>("itemid").Items.Add(dlapItem);
            });

            var assignmentItem = new AssignmentCenterItem()
            {
                Id = "itemid",
                EndDate = DateTime.MinValue, //End Date > 0 determines if item is assigned/gradable
            };

            Item result = null;
            session.When(s => s.ExecuteAsAdmin(Arg.Any<PutItems>())).Do(s =>
            {
                result = s.Arg<PutItems>().Items.FirstOrDefault();
            });

            contentActions.UnAssignAssignmentCenterItems("", new List<AssignmentCenterItem>() { assignmentItem }, "", false);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsGradable);
        }

        /// <summary>
        /// When items are moved, we need to update the DLAP item's category and sequence
        /// </summary>
        [TestMethod]
        public void UpdateAssignmentCenterItems_UpdatesCategory_ParentId_Seq()
        {
            var dlapItem = new Item()
            {
                Id = "itemid",
                EntityId = "entityid"
    
            };
            var tocs = new XElement("bfw_tocs");
            tocs.Add(new XElement("toc",
                            new XAttribute("type", "bfw_toc"),
                            new XAttribute("parentid", "parentid"),
                            new XAttribute("sequence", "seq"),
                            "toc"));

            dlapItem.Data.Add(tocs);

            session.When(s => s.ExecuteAsAdmin(Arg.Any<Batch>())).Do(s =>
            {
                s.Arg<Batch>().CommandAs<GetItems>("itemid").Items = new List<Item>();
                s.Arg<Batch>().CommandAs<GetItems>("itemid").Items.Add(dlapItem);
            });

            var assignmentItem = new AssignmentCenterItem()
            {
                Id = "itemid",
                Sequence = "newseq",
                ParentId = "newparentid"
            };

            Item result = null;
            session.When(s => s.ExecuteAsAdmin(Arg.Any<PutItems>())).Do(s =>
            {
                result = s.Arg<PutItems>().Items.FirstOrDefault();
            });

            contentActions.UpdateAssignmentCenterItems("", new List<AssignmentCenterItem>() { assignmentItem }, "toc");

            Assert.IsNotNull(result);
            Assert.AreEqual("itemid", result.Id);
            ContentItem resultItem = result.ToContentItem(context);
            var category = resultItem.Categories.FirstOrDefault(c => c.Id == "toc");
            Assert.IsNotNull(category);
            Assert.AreEqual("newparentid", category.ItemParentId);
            Assert.AreEqual("newseq", category.Sequence);
        }

        /// <summary>
        /// If a grade does not have released status but grade release date has been passed, then the grade should be returned
        ///</summary>
        [TestCategory("ContentActions"),TestMethod()]
        public void GetGradesPerItem_GradeDoesNotHaveReleasedStatus_PassReleaseDate_ExpectGradeIsReturned()
        {
            ContentItem item = new ContentItem()
            {
                Id = "itemid",
                Type = "Assignment",
                AssignmentSettings = new AssignmentSettings
                {
                    GradeReleaseDate = DateTime.UtcNow.AddHours(-1)
                }
                
            };
            var items = new List<ContentItem> {item};
            session.When(s => s.ExecuteAsAdmin(Arg.Any<GetGrades>())).Do(s =>
            {
                s.Arg<GetGrades>().Enrollments = new List<Bfw.Agilix.DataContracts.Enrollment>
                {
                    new Bfw.Agilix.DataContracts.Enrollment
                    {
                        User = new AgilixUser{Id = "userId"},
                        ItemGrades = new List<Bfw.Agilix.DataContracts.Grade>
                        { 
                            new Bfw.Agilix.DataContracts.Grade
                            {
                                Item = item.ToItem(),
                                Status = Bfw.Agilix.DataContracts.GradeStatus.ShowScore,
                                ScoredDate = DateTime.Now.AddHours(-2)
                            }
                        }
                    }
                };
            });
            var grade = contentActions.GetGradesPerItem(items, "entityId");
            Assert.IsTrue(grade.Count() == 1);

        }

        /// <summary>
        /// If a grade does not have released status and grade release date has not been passed, then the grade should not be returned
        ///</summary>
        [TestCategory("ContentActions"), TestMethod()]
        public void GetGradesPerItem_GradeDoesNotHaveReleasedStatus_AndNotPassReleaseDate_ExpectGradeNotReturn()
        {
            ContentItem item = new ContentItem()
            {
                Id = "itemid",
                Type = "Assignment",
                AssignmentSettings = new AssignmentSettings
                {
                    GradeReleaseDate = DateTime.UtcNow.AddHours(1)
                }

            };
            var items = new List<ContentItem> { item };
            session.When(s => s.ExecuteAsAdmin(Arg.Any<GetGrades>())).Do(s =>
            {
                s.Arg<GetGrades>().Enrollments = new List<Bfw.Agilix.DataContracts.Enrollment>
                {
                    new Bfw.Agilix.DataContracts.Enrollment
                    {
                        User = new AgilixUser{Id = "userId"},
                        ItemGrades = new List<Bfw.Agilix.DataContracts.Grade>
                        { 
                            new Bfw.Agilix.DataContracts.Grade
                            {
                                Item = item.ToItem(),
                                Status = Bfw.Agilix.DataContracts.GradeStatus.ShowScore,
                                ScoredDate = DateTime.Now.AddHours(-2)
                            }
                        }
                    }
                };
            });
            var grade = contentActions.GetGradesPerItem(items, "entityId");
            Assert.IsTrue(grade.IsNullOrEmpty());

        }

        [TestMethod]
        public void GetItemLinks_ExpectItems()
        {
            var courseId1 = "181975";
            var courseId2 = "181979";
            session.When(s => s.ExecuteAsAdmin(Arg.Any<GetItemLinks>())).Do( s =>
            {
                s.Arg<GetItemLinks>().ItemLinks = new List<Bfw.Agilix.DataContracts.ItemLink> { 
                    new Bfw.Agilix.DataContracts.ItemLink{ EntityId = courseId1 }, new Bfw.Agilix.DataContracts.ItemLink{ EntityId = courseId2 } 
                };
            });
            var result = contentActions.GetItemLinks("156470").ToList();
            Assert.AreEqual(courseId1, result.First().EntityId);
            Assert.AreEqual(courseId2, result.Last().EntityId);
        }

        [TestMethod]
        public void GetItemLinks_ExpectNoItems()
        {
            session.When(s => s.ExecuteAsAdmin(Arg.Any<GetItemLinks>())).Do(s =>
            {
                s.Arg<GetItemLinks>().ItemLinks = new List<Bfw.Agilix.DataContracts.ItemLink>();
            });
            var result = contentActions.GetItemLinks("156470").ToList();
            Assert.AreEqual(0, result.Count);
        }

        [TestCategory("ContentActions"), TestMethod()]
        public void UnAssignAssignmentCenterItems_Should_Keep_Item_In_Gradebook()
        {
            List<AssignmentCenterItem> list = new List<AssignmentCenterItem>()
            {
                new AssignmentCenterItem()
                {
                    Id = "itemId",
                    CategorySequence = "a",
                    Points = 10
                }
            };
            List<Item> items = new List<Item>()
            {
                new Item()
                {
                    Id = "itemId",
                    CategorySequence = "a"
                }
            };
            session.WhenForAnyArgs(s => s.ExecuteAsAdmin(Arg.Any<Batch>())).Do(s => (s.Arg<Batch>().Commands.First() as GetItems).Items = items);

            contentActions.UnAssignAssignmentCenterItems("categoryId", list, "toc", true);

            session.ReceivedCalls().ToList().ForEach(c =>
                { 
                    if(c.GetArguments().First() is PutItems)
                    {
                        var putItems = c.GetArguments().First() as PutItems;
                        Assert.IsTrue(putItems.Items.First().MaxPoints == 10);
                        Assert.IsTrue(putItems.Items.First().CategorySequence == "a");
                    }
                });
        }

        [TestCategory("ContentActions"), TestMethod()]
        public void UnAssignAssignmentCenterItems_Should_Remove_Item_From_Gradebook()
        {
            List<AssignmentCenterItem> list = new List<AssignmentCenterItem>()
            {
                new AssignmentCenterItem()
                {
                    Id = "itemId",
                    CategorySequence = "a",
                    Points = 0
                }
            };
            List<Item> items = new List<Item>()
            {
                new Item()
                {
                    Id = "itemId",
                    CategorySequence = "a"
                }
            };
            session.WhenForAnyArgs(s => s.ExecuteAsAdmin(Arg.Any<Batch>())).Do(s => (s.Arg<Batch>().Commands.First() as GetItems).Items = items);

            contentActions.UnAssignAssignmentCenterItems("categoryId", list, "toc", false);

            session.ReceivedCalls().ToList().ForEach(c =>
            {
                if (c.GetArguments().First() is PutItems)
                {
                    var putItems = c.GetArguments().First() as PutItems;
                    Assert.IsTrue(putItems.Items.First().MaxPoints == 0);
                    Assert.IsTrue(putItems.Items.First().CategorySequence == "");
                }
            });
        }
    }
}
