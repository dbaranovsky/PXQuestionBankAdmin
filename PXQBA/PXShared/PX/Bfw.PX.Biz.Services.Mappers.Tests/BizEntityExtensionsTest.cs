using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Bfw.Common.Collections;
using Bfw.Common.JqGridHelper;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.PX.Biz.DataContracts;
using AgDC = Bfw.Agilix.DataContracts;
using Course = Bfw.PX.Biz.DataContracts.Course;
using System.Text;

namespace Bfw.PX.Biz.Services.Mappers.Tests
{
    /// <summary>
    ///This is a test class for BizEntityExtensionsTest and is intended
    ///to contain all BizEntityExtensionsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BizEntityExtensionsTest
    {
        private TestContext testContextInstance;

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

        /// <summary>
        ///A test for ToQuestion
        ///</summary>
        [TestCategory("Mapping"), TestMethod()]
        public void BizEntityExtensionsTest_ToQuestionTest()
        {
            SearchResultDoc doc = new SearchResultDoc()
            {
                dlap_id = "Q|6112|questionId",
                dlap_html_text = "body text|choice1|choice2",
                dlap_title = "title",
                dlap_q_score = 1.0,
                dlap_q_type = "essay",
                Metadata = new Dictionary<string, string>(),
                entityid = "6112",
            };
            Question expected = new Question()
            {
                Id = "questionId",
                Body = "body text",
                Choices = new List<QuestionChoice>()
                {
                    new QuestionChoice(){Text="choice1"},
                    new QuestionChoice(){Text = "choice2"}
                },
                Title = "title",
                Points = 1.0,
                InteractionType = InteractionType.Essay,
                SearchableMetaData = new Dictionary<string, string>(),
                EntityId = "6112",
            };
            Question actual = BizEntityExtensions.ToQuestion(doc);
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Body, actual.Body);
            Assert.AreEqual(expected.Choices[0].Text, actual.Choices[0].Text);
            Assert.AreEqual(expected.Choices[1].Text, actual.Choices[1].Text);
            Assert.AreEqual(expected.Title, actual.Title);
            Assert.AreEqual(expected.Points, actual.Points);
            Assert.AreEqual(expected.InteractionType, actual.InteractionType);
            Assert.AreEqual(expected.SearchableMetaData.Map((k) => k.Key + k.Value).Reduce((v1, v2) => v1 + v2, string.Empty),
                actual.SearchableMetaData.Map((k) => k.Key + k.Value).Reduce((v1, v2) => v1 + v2, string.Empty));
            Assert.AreEqual(expected.EntityId, actual.EntityId);
        }

        #region Agilix Course -> Biz Course
        [TestCategory("Mapping"), TestMethod]
        public void BizEntityExtensionsTest_AgilixCourseToBizCourse_IfLmsId_Required_Exists()
        {
            var data = new XElement("data");
            data.Add(new XElement("bfw_lmsid_required", "true"));

            var oCourse = new Bfw.Agilix.DataContracts.Course()
            {
                Title = "New Course",
                Id = "344423",
                Data = data
            };

            var course = oCourse.ToCourse();

            Assert.AreEqual(true, course.LmsIdRequired);
        }

        [TestCategory("Mapping"), TestMethod]
        public void BizEntityExtensionsTest_AgilixCourseToBizCourse_IfLmsId_Label_Exists()
        {
            var data = new XElement("data");
            data.Add(new XElement("bfw_lmsid_label", "LMS Id"));

            var oCourse = new Bfw.Agilix.DataContracts.Course()
            {
                Title = "New Course",
                Id = "344423",
                Data = data
            };

            var course = oCourse.ToCourse();

            Assert.AreEqual("LMS Id", course.LmsIdLabel);
        }

        [TestCategory("Mapping"), TestMethod]
        public void BizEntityExtensionsTest_AgilixCourseToBizCourse_IfLmsIdPrompt_Exists()
        {
            var data = new XElement("data");
            data.Add(new XElement("bfw_lmsid_prompt", "Please enter LMS Id"));

            var oCourse = new Bfw.Agilix.DataContracts.Course()
            {
                Title = "New Course",
                Id = "344423",
                Data = data
            };

            var course = oCourse.ToCourse();

            Assert.AreEqual("Please enter LMS Id", course.LmsIdPrompt);
        }

        [TestCategory("Mapping"), TestMethod]
        public void BizEntityExtensionsTest_AgilixCourseToBizCourse_WithFalseDisableComments_ParsesFalse()
        {
            var data = new XElement("data");
            data.Add(new XElement("bfw_disable_comments", "false"));

            var aCourse = new Bfw.Agilix.DataContracts.Course()
            {
                Title = "Title",
                Id = "ID",
                Data = data
            };

            var parsedCourse = aCourse.ToCourse();
            Assert.AreEqual(false, parsedCourse.DisableComments);
        }

        [TestCategory("Mapping"), TestMethod]
        public void BizEntityExtensionsTest_AgilixCourseToBizCourse_WithTrueDisableComments_ParsesTrue()
        {
            var data = new XElement("data");
            data.Add(new XElement("bfw_disable_comments", "true"));

            var aCourse = new Bfw.Agilix.DataContracts.Course()
            {
                Title = "Title",
                Id = "ID",
                Data = data
            };

            var parsedCourse = aCourse.ToCourse();
            Assert.AreEqual(true, parsedCourse.DisableComments);
        }

        [TestCategory("Mapping"), TestMethod]
        public void BizEntityExtensionsTest_AgilixCourseToBizCourse_WithOutDisableComments_ParsesFalse()
        {
            var data = new XElement("data");

            var aCourse = new Bfw.Agilix.DataContracts.Course()
            {
                Title = "Title",
                Id = "ID",
                Data = data
            };

            var parsedCourse = aCourse.ToCourse();
            Assert.AreEqual(false, parsedCourse.DisableComments);
        }
        #endregion Agilix Course -> Biz Course

        #region Biz Item -> Agilix Item
        /// <summary>
        /// Value set in ContentItem.DueDateGrace should pass to Item.DueDateGrace
        /// </summary>
        [TestCategory("Mapping"), TestMethod]
        public void BizEntityExtensionsTest_BizItemToAgilixItem_WhenAllowLateSubmssion_ExpectDueDateGraceIsSet()
        {
            ContentItem cItem = new ContentItem { DueDateGrace = 100, Type = "None"};
            var item = cItem.ToItem();
            Assert.IsTrue(item.DueDateGrace == 100);
        }

        /// <summary>
        /// Value set in ContentItem.GradeFlags ExtraCredit should pass to Item.GradeFlags ExtraCredit
        /// </summary>
        [TestCategory("Mapping"), TestMethod]
        public void BizEntityExtensionsTest_BizItemToAgilixItem_WhenGradeFlagExtraCredit_ExpectExtraCreditIsSet()
        {
            ContentItem cItem = new ContentItem { DueDateGrace = 100, Type = "None", GradeFlags = GradeFlags.ExtraCredit };
            var item = cItem.ToItem();
            Assert.IsTrue((item.GradeFlags & AgDC.GradeFlags.ExtraCredit) == AgDC.GradeFlags.ExtraCredit);
        }

        /// <summary>
        /// Value set in ContentItem.DueDateGrace should pass to Item.DueDateGrace
        /// </summary>
        [TestCategory("Mapping"), TestMethod]
        public void BizEntityExtensionsTest_BizItemToAgilixItem_WhenNotAllowLateSubmssion_ExpectDueDateGraceIsZero()
        {
            ContentItem cItem = new ContentItem { DueDateGrace = 100, Type = "None", AssignmentSettings = new AssignmentSettings { IsAllowLateGracePeriod = false } };
            var item = cItem.ToItem();
            Assert.IsTrue(item.DueDateGrace == 0);
        }

        /// <summary>
        /// Ensure that containers are being passed properly when goin from biz item -> agilix item
        /// </summary>
        [TestCategory("Mapping"), TestMethod]
        public void BizEntityExtensionsTest_BizItemToAgilixItem_AgilixContainersSet()
        {
            var toc1 = "syllabusfilter";
            var container1 = "foo";
            var toc2 = "assignmentfilter";
            var container2 = "bar;";
            ContentItem citem = new ContentItem()
            {
                Type = "None",
                Containers = new List<Container>()
                {
                    new Container(toc1, container1),
                    new Container(toc2, container2),
                }
            };

            var aitem = citem.ToItem();
            Assert.AreEqual(2, aitem.Containers.Count);
            Assert.IsTrue(aitem.Containers.Any(c => c.Toc.Equals(toc1)));
            Assert.IsTrue(aitem.Containers.Any(c => c.Toc.Equals(toc2)));
            Assert.AreEqual(container1, aitem.Containers.FirstOrDefault(c => c.Toc.Equals(toc1)).Value);
            Assert.AreEqual(container2, aitem.Containers.FirstOrDefault(c => c.Toc.Equals(toc2)).Value);
        }

        /// <summary>
        /// Ensure that subcontainers are being passed properly when goin from biz item -> agilix item
        /// </summary>
        [TestCategory("Mapping"), TestMethod]
        public void BizEntityExtensionsTest_BizItemToAgilixItem_AgilixSubContainersSet()
        {
            var toc1 = "syllabusfilter";
            var subcontainer1 = "foo";
            var toc2 = "assignmentfilter";
            var subcontainer2 = "bar;";
            ContentItem citem = new ContentItem()
            {
                Type = "None",
                SubContainerIds = new List<Container>()
                {
                    new Container(toc1, subcontainer1),
                    new Container(toc2, subcontainer2),
                }
            };

            var aitem = citem.ToItem();
            Assert.AreEqual(2, aitem.SubContainerIds.Count);
            Assert.IsTrue(aitem.SubContainerIds.Any(c => c.Toc.Equals(toc1)));
            Assert.IsTrue(aitem.SubContainerIds.Any(c => c.Toc.Equals(toc2)));
            Assert.AreEqual(subcontainer1, aitem.SubContainerIds.FirstOrDefault(c => c.Toc.Equals(toc1)).Value);
            Assert.AreEqual(subcontainer2, aitem.SubContainerIds.FirstOrDefault(c => c.Toc.Equals(toc2)).Value);
        }

        /// <summary>
        /// Value set in ContentItem.CategorySequence should pass to Item.CategorySequence AND Item.Sequence
        /// </summary>
        [TestCategory("Mapping"), TestMethod]
        public void BizEntityExtensionsTest_BizItemToAgilixItem_SetCategorySequence_SetsSequence()
        {
            ContentItem cItem = new ContentItem { Id="itemid", Type = "None", 
                AssignmentSettings = new AssignmentSettings() { CategorySequence = "seq"}
            };
            var item = cItem.ToItem();
            Assert.AreEqual("seq", item.CategorySequence);
            Assert.AreEqual("seq", item.Sequence);
        }

        /// <summary>
        /// If this is sco item and it is not a learning curve, then we expect submissionGradeAction value is passed to the custom fields of agilix item.
        /// </summary>
        [TestCategory("Mapping"), TestMethod]
        public void ToItem_IfScoItemNotLearningCurve_ExpectSubmissionGradeActionIsSetInCustomFields()
        {
            ContentItem cItem = new ContentItem
            {
                Id = "itemid",
                Type = "None",
                Sco = true,
                AssignmentSettings = new AssignmentSettings() { SubmissionGradeAction = SubmissionGradeAction.FullCredit }
            };
            var item = cItem.ToItem();
            var customFields = item.Data.Descendants("customfields").ToList();
            Assert.AreEqual(1, customFields.Count());
            var fields = customFields.Descendants("field");
            bool hasSubmissiongradeAction = false;
            foreach (var field in fields)
            {
                if (field.Attribute("name").Value == "submissiongradeaction" && field.Attribute("value").Value == "Full_Credit")
                {
                    hasSubmissiongradeAction = true;
                }

            }
            Assert.IsTrue(hasSubmissiongradeAction);

        }

        /// <summary>
        /// If this is sco item and it is a learning curve, then we expect submissionGradeAction value is passed to the custom fields of agilix item.
        /// </summary>
        [TestCategory("Mapping"), TestMethod]
        public void ToItem_IfScoItemAndLearningCurve_ExpectSubmissionGradeActionIsNotSetInCustomFields()
        {
            ContentItem cItem = new ContentItem
            {
                Id = "itemid",
                Type = "CustomActivity",
                Href = "learningcurve.bfwpub.com/index.php",
                Sco = true,
                AssignmentSettings = new AssignmentSettings() { SubmissionGradeAction = SubmissionGradeAction.FullCredit }
            };
            var item = cItem.ToItem();
            var customFields = item.Data.Descendants("customfields").ToList();
            Assert.AreEqual(1, customFields.Count());
            var fields = customFields.Descendants("field");
            bool hasSubmissiongradeAction = false;
            foreach (var field in fields)
            {
                if (field.Attribute("name").Value == "submissiongradeaction" && field.Attribute("value").Value == "Full_Credit")
                {
                    hasSubmissiongradeAction = true;
                }

            }
            Assert.IsFalse(hasSubmissiongradeAction);

        }
        #endregion Biz Item -> Agilix Item

        #region Agilix Item -> Biz Item
        /// <summary>
        /// Ensure that containers are being passed properly when goin from biz item -> agilix item
        /// </summary>
        [TestCategory("Mapping"), TestMethod]
        public void BizEntityExtensionsTest_AgilixItemToBizItem_AgilixContainersSet()
        {
            var toc1 = "syllabusfilter";
            var container1 = "foo";
            var toc2 = "assignmentfilter";
            var container2 = "bar;";
            AgDC.Item aitem = new AgDC.Item()
            {
                Containers = new List<AgDC.Container>()
                {
                    new AgDC.Container(toc1, container1),
                    new AgDC.Container(toc2, container2),
                }
            };

            var citem = aitem.ToContentItem();
            Assert.AreEqual(2, citem.Containers.Count);
            Assert.IsTrue(citem.Containers.Any(c => c.Toc.Equals(toc1)));
            Assert.IsTrue(citem.Containers.Any(c => c.Toc.Equals(toc2)));
            Assert.AreEqual(container1, citem.Containers.FirstOrDefault(c => c.Toc.Equals(toc1)).Value);
            Assert.AreEqual(container2, citem.Containers.FirstOrDefault(c => c.Toc.Equals(toc2)).Value);
        }

        /// <summary>
        /// Ensure that subcontainers are being passed properly when goin from biz item -> agilix item
        /// </summary>
        [TestCategory("Mapping"), TestMethod]
        public void BizEntityExtensionsTest_AgilixItemToBizItem_AgilixSubContainersSet()
        {
            var toc1 = "syllabusfilter";
            var subcontainer1 = "foo";
            var toc2 = "assignmentfilter";
            var subcontainer2 = "bar;";
            AgDC.Item aitem = new AgDC.Item()
            {
                SubContainerIds = new List<AgDC.Container>()
                {
                    new AgDC.Container(toc1, subcontainer1),
                    new AgDC.Container(toc2, subcontainer2),
                }
            };

            var citem = aitem.ToContentItem();
            Assert.AreEqual(2, citem.SubContainerIds.Count);
            Assert.IsTrue(citem.SubContainerIds.Any(c => c.Toc.Equals(toc1)));
            Assert.IsTrue(citem.SubContainerIds.Any(c => c.Toc.Equals(toc2)));
            Assert.AreEqual(subcontainer1, citem.SubContainerIds.FirstOrDefault(c => c.Toc.Equals(toc1)).Value);
            Assert.AreEqual(subcontainer2, citem.SubContainerIds.FirstOrDefault(c => c.Toc.Equals(toc2)).Value);
        }

        [TestCategory("Mapping"), TestMethod]
        public void BizEntityExtensionsTest_AgilixGradeListToBizGradeList_AllValuesSet()
        {
            var status = "status";
            var responsev = "responsev";
            var seconds = "seconds";
            var submittedDate = DateTime.Today;
            var submittedv = "submittedv";

            AgDC.GradeList alist = new AgDC.GradeList()
            {
                Status = status,
                Responseversion = responsev,
                Seconds = seconds,
                SubmittedDate = submittedDate,
                Submittedversion = submittedv
            };

            var blist = alist.ToGradeList();
            Assert.AreEqual(status, blist.Status);
            Assert.AreEqual(responsev, blist.Responseversion);
            Assert.AreEqual(seconds, blist.Seconds);
            Assert.AreEqual(submittedDate, blist.SubmittedDate);
            Assert.AreEqual(submittedv, blist.Submittedversion);
        }

        [TestCategory("Mapping"), TestMethod]
        public void BizEntityExtensionsAction_AgilixItemToBizItem_ExamTemplateSet()
        {
            var examTemplate = "examTemplate";
            var doc =
                XDocument.Load(
                    new MemoryStream(
                        ASCIIEncoding.Default.GetBytes(string.Format("<data><examtemplate>{0}</examtemplate></data>", examTemplate))));
            AgDC.Item item = new AgDC.Item()
            {
                Data = doc.Element("data")
            };

            var contentitem = item.ToContentItem();

            Assert.AreEqual(examTemplate, contentitem.ExamTemplate);
        }
        #endregion Agilix Item -> Biz Item

        /// <summary>
        /// If grace period is set to 1 minutes, expect return 1 minutes
        /// </summary>
        [TestCategory("Mapping"), TestMethod]
        public void CalculateGraceDurationInMinute_WhenTypeIsMinute_ExpectReturn1Minute()
        {
            var duration = BizEntityExtensions.CalculateGraceDurationInMinute(1, "Minute");
            Assert.IsTrue(duration == 1);
        }

        /// <summary>
        /// If grace period is set to 1 hour, expect return 60 minutes
        /// </summary>
        [TestCategory("Mapping"), TestMethod]
        public void CalculateGraceDurationInMinute_WhenTypeIsHour_ExpectReturn60Minute()
        {
            var duration = BizEntityExtensions.CalculateGraceDurationInMinute(1, "Hour");
            Assert.IsTrue(duration == 60);
        }

        /// <summary>
        /// If grace period is set to 1 day, expect return 1440 minutes
        /// </summary>
        [TestCategory("Mapping"), TestMethod]
        public void CalculateGraceDurationInMinute_WhenTypeIsDay_ExpectReturn1440Minute()
        {
            var duration = BizEntityExtensions.CalculateGraceDurationInMinute(1, "Day");
            Assert.IsTrue(duration == 1440);
        }

        /// <summary>
        /// If grace period is set to 1 week, expect return 10080 minutes
        /// </summary>
        [TestCategory("Mapping"), TestMethod]
        public void CalculateGraceDurationInMinute_WhenTypeIsWeek_ExpectReturn10080Minute()
        {
            var duration = BizEntityExtensions.CalculateGraceDurationInMinute(1, "Week");
            Assert.IsTrue(duration == 10080);
        }

        /// <summary>
        /// If grace period is set to infinite, expect return -1
        /// </summary>
        [TestCategory("BizEntityExtensions"), TestMethod]
        public void CalculateGraceDurationInMinute_WhenTypeIsInfinite_ExpectReturnNegativeOne()
        {
            var duration = BizEntityExtensions.CalculateGraceDurationInMinute(1, "Infinite");
            Assert.IsTrue(duration == -1);
        }

        /// <summary>
        /// To test that calling ToXmlResource returns Comment field in proper format.
        /// </summary>
        [TestCategory("BizEntityExtensions"), TestMethod]
        public void ReturnCommentFieldInProperFormat()
        {
            //Arrange
            string str = "<xmlResource><title>Student 03 - outside</title><body><![CDATA[]]></body><ContentType>xml-res</ContentType><Extension>pxres</Extension><Url>Templates/Data/XmlResources/Documents/Assignments/fe17ad4628ea4b6697440749c62751ab.pxres</Url><EntityId>159364</EntityId><ExtendedProperties><AssignmentId>3fe40551c8f14e47b243b60e6a39815a</AssignmentId><Status>submitted</Status><WordCount></WordCount><Comment><p>Here's my homework   <a href=\"/launchpad/hockenbury6e/159351/Downloads/Document?id=4ced85ae61d843ea9397eadb16faa219&amp;name=Student 03 - inside.txt\">Student 03 - inside.txt</a></p></Comment><FileName>Student 03 - outside.txt</FileName><FileSize>0K</FileSize></ExtendedProperties></xmlResource>";
            Byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
            var stream = new MemoryStream();
            stream.Write(bytes, 0, bytes.Length);
            var res = new AgDC.Resource() { ResourceStream = stream, Url = "" };

            //Act
            XmlResource result = res.ToXmlResource();

            //Assert
            string expected = "<Comment>  <p>Here's my homework   <a href=\"/launchpad/hockenbury6e/159351/Downloads/Document?id=4ced85ae61d843ea9397eadb16faa219&amp;name=Student 03 - inside.txt\">Student 03 - inside.txt</a></p></Comment>";
            var actual = result.ExtendedProperties.ToArray()[0].Value;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The property should be initialized regardless of presence of the corresponding xelement
        /// </summary>        
        [TestCategory("Mapping"), TestMethod]
        public void ToWidgetItem_Should_Initialize_InstructorConsoleSettings()
        {
            var settings = new XElement("instructor_console_settings");
            var showchapters = new XElement("resource");
            showchapters.Add(new XAttribute("name", "showchapters"));
            showchapters.Add(new XAttribute("type", "showchapters"));
            showchapters.Add(new XAttribute("value", "showchapters"));
            settings.Add(showchapters);
            var showtypes = new XElement("resource");
            showtypes.Add(new XAttribute("name", "showtypes"));
            showtypes.Add(new XAttribute("type", "showtypes"));
            showtypes.Add(new XAttribute("value", "showtypes"));
            settings.Add(showtypes);
            var showebook = new XElement("resource");
            showebook.Add(new XAttribute("name", "showebook"));
            showebook.Add(new XAttribute("type", "showebook"));
            showebook.Add(new XAttribute("value", "showebook"));
            settings.Add(showebook);
            var showmyresources = new XElement("resource");
            showmyresources.Add(new XAttribute("name", "showmyresources"));
            showmyresources.Add(new XAttribute("type", "showmyresources"));
            showmyresources.Add(new XAttribute("value", "showmyresources"));
            settings.Add(showmyresources);
            var showgeneral = new XElement("resource");
            showgeneral.Add(new XAttribute("name", "showgeneral"));
            showgeneral.Add(new XAttribute("type", "showgeneral"));
            showgeneral.Add(new XAttribute("value", "showgeneral"));
            settings.Add(showgeneral);
            var shownavigation = new XElement("resource");
            shownavigation.Add(new XAttribute("name", "shownavigation"));
            shownavigation.Add(new XAttribute("type", "shownavigation"));
            shownavigation.Add(new XAttribute("value", "shownavigation"));
            settings.Add(shownavigation);
            var showlaunchpad = new XElement("resource");
            showlaunchpad.Add(new XAttribute("name", "showlaunchpad"));
            showlaunchpad.Add(new XAttribute("type", "showlaunchpad"));
            showlaunchpad.Add(new XAttribute("value", "showlaunchpad"));
            settings.Add(showlaunchpad);            
            var showwelcomereturn = new XElement("resource");
            showwelcomereturn.Add(new XAttribute("name", "showwelcomereturn"));
            showwelcomereturn.Add(new XAttribute("type", "showwelcomereturn"));
            showwelcomereturn.Add(new XAttribute("value", "showwelcomereturn"));
            settings.Add(showwelcomereturn);
            var showbatchupdater = new XElement("resource");
            showbatchupdater.Add(new XAttribute("name", "showbatchupdater"));
            showbatchupdater.Add(new XAttribute("type", "showbatchupdater"));
            showbatchupdater.Add(new XAttribute("value", "showbatchupdater"));
            settings.Add(showbatchupdater);
            var showmanageannouncemets = new XElement("resource");
            showmanageannouncemets.Add(new XAttribute("name", "showmanageannouncemets"));
            showmanageannouncemets.Add(new XAttribute("type", "showmanageannouncemets"));
            showmanageannouncemets.Add(new XAttribute("value", "showmanageannouncemets"));
            settings.Add(showmanageannouncemets); 
            var data = new XElement("data");
            data.Add(settings);
            var result = BizEntityExtensions.ToWidgetItem(new AgDC.Item() 
            { 
                Data = data
            });
 
            Assert.IsNotNull(result.InstructorConsoleSettings);
            Assert.IsTrue(result.InstructorConsoleSettings.ShowChapters);
            Assert.IsTrue(result.InstructorConsoleSettings.ShowTypes);
            Assert.IsTrue(result.InstructorConsoleSettings.ShowEbook);
            Assert.IsTrue(result.InstructorConsoleSettings.ShowMyResources);
            Assert.IsTrue(result.InstructorConsoleSettings.ShowGeneral);
            Assert.IsTrue(result.InstructorConsoleSettings.ShowNavigation);
            Assert.IsTrue(result.InstructorConsoleSettings.ShowLaunchPad);
            Assert.IsTrue(result.InstructorConsoleSettings.ShowWelcomeReturn);
            Assert.IsTrue(result.InstructorConsoleSettings.ShowBatchUpdater);
            Assert.IsTrue(result.InstructorConsoleSettings.ShowManageAnnouncemets);
        }

        /// <summary>
        /// The category grades (totals) should be mapped into biz
        /// </summary>
        [TestMethod]
        public void ToEnrollment_From_Agilix_Should_Map_CategoryGrades()
        {
            var catgrades = new List<AgDC.CategoryGrade>() 
            {
                new AgDC.CategoryGrade()
                {
                    Id = "1",
                    Name = "category"
                }
            };
            var enrollment = new AgDC.Enrollment()
            {
                Domain = new AgDC.Domain()
            };
            enrollment.GetType().GetProperty("CategoryGrades").SetValue(enrollment, catgrades, null);

            var biz = BizEntityExtensions.ToEnrollment(enrollment);

            Assert.AreEqual("1", biz.CategoryGrades.First().Id);
            Assert.AreEqual("category", biz.CategoryGrades.First().Name);
        }

        /// <summary>
        /// The overall grade for biz should contain Achieved and Possible points
        /// </summary>
        [TestMethod]
        public void ToEnrollment_From_Agilix_Should_Map_OverallGrades()
        {
            var grade = new AgDC.Grade();
            grade.GetType().GetProperty("Achieved").SetValue(grade, 5, null);
            grade.GetType().GetProperty("Possible").SetValue(grade, 10, null);
            var enrollment = new AgDC.Enrollment()
            {
                Domain = new AgDC.Domain(),
                OverallGrade = grade
            };

            var biz = BizEntityExtensions.ToEnrollment(enrollment);

            Assert.AreEqual(5, biz.OverallAchieved);
            Assert.AreEqual(10, biz.OverallPossible);
        }

        /// <summary>
        /// The category grade should map all proeprties into biz
        /// </summary>
        [TestMethod]
        public void ToCategoryGrade_Should_Map_CategoryGrade()
        {
            var category = new AgDC.CategoryGrade()
            {
                Id = "1",
                Name = "category"
            };
            category.GetType().GetProperty("Achieved").SetValue(category, 10, null);
            category.GetType().GetProperty("Possible").SetValue(category, 20, null);
            category.GetType().GetProperty("Letter").SetValue(category, "B", null);

            var biz = BizEntityExtensions.ToCategoryGrade(category);

            Assert.AreEqual("1", biz.Id);
            Assert.AreEqual("category", biz.Name);
            Assert.AreEqual(10, biz.Achieved);
            Assert.AreEqual(20, biz.Possible);
            Assert.AreEqual("B", biz.Letter);
        }

        [TestMethod]
        public void Agilix_Biz_ToCourse_Should_Map_QuestionBankRepositoryCourse_If_Present()
        {
            var data = new XElement("data");
            data.Add(new XElement("QuestionBankRepositoryCourse", "questionCourseId"));
            var course = new AgDC.Course() 
            { 
                Data = data
            };

            var biz = BizEntityExtensions.ToCourse(course);

            Assert.AreEqual("questionCourseId", biz.QuestionBankRepositoryCourse);
        }

        [TestCategory("ToCourse"), TestMethod]
        public void IfEnableArgaUrlMappingFlag_NotInCourse_ExpectEnableArgaUrlMappingSetToTrue()
        {
            var data = new XElement("data");
            var course = new AgDC.Course()
            {
                Data = data
            };

            var biz = BizEntityExtensions.ToCourse(course);

            Assert.IsTrue(biz.EnableArgaUrlMapping);
        }
        [TestCategory("ToCourse"), TestMethod]
        public void IfEnableArgaUrlMappingFlag_TrueInCourse_ExpectEnableArgaUrlMappingSetToTrue()
        {
            var data = XDocument.Parse("<data><enableArgaUrlMapping>true</enableArgaUrlMapping></data>").Element("data");
            var course = new AgDC.Course()
            {
                Data = data
            };

            var biz = BizEntityExtensions.ToCourse(course);

            Assert.IsTrue(biz.EnableArgaUrlMapping);
        }

        [TestCategory("ToCourse"), TestMethod]
        public void IfEnableArgaUrlMappingFlag_FalseInCourse_ExpectEnableArgaUrlMappingSetToFalse()
        {
            var data = XDocument.Parse("<data><enableArgaUrlMapping>false</enableArgaUrlMapping></data>").Element("data");

            var course = new AgDC.Course()
            {
                Data = data
            };

            var biz = BizEntityExtensions.ToCourse(course);

            Assert.IsFalse(biz.EnableArgaUrlMapping);
        }
    }
}
