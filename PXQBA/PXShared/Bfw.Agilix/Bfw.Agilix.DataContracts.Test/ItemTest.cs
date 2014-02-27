using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;
using Helper = TestHelper.Helper;

namespace Bfw.Agilix.DataContracts.Tests
{
	[TestClass]
	public class ItemTest
	{
		private Item itm_WithContainerDlapExact_IsNull;
		private Item itm;
		
		private static XElement xItemElement;
		private static XElement xItemElement_WithContainerDlapExact_IsNull;

		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			xItemElement = Helper.GetResponse(Entity.Item, "GenericItem").Element("item");
			xItemElement_WithContainerDlapExact_IsNull = Helper.GetResponse(Entity.Item_WithNoDlapExactInContainers, "GenericItem").Element("item");
		}

		[TestInitialize]
		public void TestInitialize()
		{
			itm_WithContainerDlapExact_IsNull = new Bfw.Agilix.DataContracts.Item();
			itm = new Bfw.Agilix.DataContracts.Item();
		}

        [TestMethod]
		public void ItemTests_Item_ParseEntity_WhenContainerDlalpExact_IsNull()
        {

			 itm_WithContainerDlapExact_IsNull.ParseEntity(xItemElement_WithContainerDlapExact_IsNull);

			 var isTestContainerFailed = false;

			 foreach (var container in itm_WithContainerDlapExact_IsNull.Containers.Where(container => container.DlapType != "exact"))
			 {
				 isTestContainerFailed = true;
			 }

			 var isTestSubContainerFailed = false;

			 foreach (var container in itm_WithContainerDlapExact_IsNull.SubContainerIds.Where(container => container.DlapType != "exact"))
			 {
				 isTestSubContainerFailed = true;
			 }
			 Assert.IsTrue(!isTestContainerFailed & !isTestSubContainerFailed);

        }
        
        [TestMethod]
        public void ItemTests_Item_ParseEntity_WhenContainerDlalpExact_IsNotNull()
        {
			 var isTestContainerFailed = false;

			 itm.ParseEntity(xItemElement);

			 foreach (var container in itm.Containers)
			 {
				 if (container.DlapType != "exact")
				 {
					 isTestContainerFailed = true;
				 }
			 }

			 var isTestSubContainerFailed = false;

			 foreach (var container in itm.SubContainerIds)
			 {
				 if (container.DlapType != "exact")
				 {
					 isTestSubContainerFailed = true;
				 }
			 }
			 Assert.IsTrue(!isTestContainerFailed & !isTestSubContainerFailed);
        }
        [TestMethod]
        public void ItemTests_Item_ToEntity_WhenContainerDlalpExact_IsNotNull()
        {
			 itm.ParseEntity(xItemElement);

			 xItemElement = itm.ToEntity();

			 var isTestContainerOk = false;
			 var isTestSubContainerOk = false;

			 var xDataElement = xItemElement.Element("data");
			 if (xDataElement != null)
			 {
				 var xMeta_ContainersEl = xDataElement.Element("meta-containers");
				 if (xMeta_ContainersEl != null)
				 {

					 if (xMeta_ContainersEl.Elements("meta-container").All(container => container.Attribute("dlaptype").Value == "exact")) isTestContainerOk = true;
					
				 }

				 var xMeta_SubContainersEl = xDataElement.Element("meta-subcontainers");
				 if (xMeta_SubContainersEl != null)
				 {

					 if (xMeta_SubContainersEl.Elements("meta-subcontainerid").All(container => container.Attribute("dlaptype").Value == "exact")) isTestSubContainerOk = true;					 
				 }
			 }

			 Assert.IsTrue(isTestContainerOk & isTestSubContainerOk);

        }

        [TestMethod]
        public void ItemTests_Item_ToEntity_WhenContainerDlalpExact_IsNull()
        {
			 itm_WithContainerDlapExact_IsNull.ParseEntity(xItemElement_WithContainerDlapExact_IsNull);

			 xItemElement_WithContainerDlapExact_IsNull = itm_WithContainerDlapExact_IsNull.ToEntity();

			 var isTestContainerOk = false;
			 var isTestSubContainerOk = false;

			 var xDataElement = xItemElement_WithContainerDlapExact_IsNull.Element("data");
			 if (xDataElement != null)
			 {
				 var xMeta_ContainersEl = xDataElement.Element("meta-containers");
				 if (xMeta_ContainersEl != null)
				 {

					 if (xMeta_ContainersEl.Elements("meta-container").All(container => container.Attribute("dlaptype").Value == "exact")) isTestContainerOk = true;

				 }

				 var xMeta_SubContainersEl = xDataElement.Element("meta-subcontainers");
				 if (xMeta_SubContainersEl != null)
				 {

					 if (xMeta_SubContainersEl.Elements("meta-subcontainerid").All(container => container.Attribute("dlaptype").Value == "exact")) isTestSubContainerOk = true;
				 }
			 }

			 Assert.IsTrue(isTestContainerOk & isTestSubContainerOk);

        }
        /// <summary>
        /// Test if item gets duedategrace data from XML
        /// </summary>
        /// 
        [TestCategory("Item"), TestMethod]
        public void ParseEntity_ExpectDueDateGraceNotNull()
        {
             itm.ParseEntity(xItemElement);
             Assert.IsTrue(itm.DueDateGrace.Equals(20160));
        }

        /// <summary>
        /// If graceduedate and allowlatesubmission are not found in xml data, 
        /// then Item.IsAllowLateSubmission should be false 
        /// </summary>
        [TestCategory("Item"), TestMethod]
        public void ParseEntity_WhenNotSet_ExpectAllowLateSubmissionIsFalse()
        {
            var xmlData = "<item id=\"a\"><data></data></item>";
            xItemElement = XElement.Parse(xmlData);
            itm.ParseEntity(xItemElement);
            Assert.IsFalse(itm.IsAllowLateSubmission);
        }

        /// <summary>
        /// If allowlatesubmission is false in xml data, 
        /// then Item.AllowLateSubmission should be false 
        /// </summary>
        [TestCategory("Item"), TestMethod]
        public void ParseEntity_WhenAllowLateSubmissionIsFalse_ExpectAllowLateSubmissionIsFalse()
        {
            var xmlData = "<item id=\"a\"><data><allowlatesubmission>false</allowlatesubmission></data></item>";
            xItemElement = XElement.Parse(xmlData);
            itm.ParseEntity(xItemElement);
            Assert.IsFalse(itm.IsAllowLateSubmission);
        }

        /// <summary>
        /// If allowlatesubmission is true in xml data, 
        /// then Item.IsAllowLateSubmission should be true 
        /// </summary>
        [TestCategory("Item"), TestMethod]
        public void ParseEntity_WhenAllowLateSubmissionIsTrue_ExpectAllowLateSubmissionIsFalse()
        {
            var xmlData = "<item id=\"a\"><data><allowlatesubmission>true</allowlatesubmission></data></item>";
            xItemElement = XElement.Parse(xmlData);
            itm.ParseEntity(xItemElement);
            Assert.IsTrue(itm.IsAllowLateSubmission);
        }

        /// <summary>
        /// If duedategrace is false in xml data, 
        /// then Item.IsAllowLateSubmission should be false 
        /// </summary>
        [TestCategory("Item"), TestMethod]
        public void ParseEntity_WhenDueDateGraceIsFalse_ExpectAllowLateSubmissionIsFalse()
        {
            var xmlData = "<item id=\"a\"><data><duedategrace>0</duedategrace></data></item>";
            xItemElement = XElement.Parse(xmlData);
            itm.ParseEntity(xItemElement);
            Assert.IsFalse(itm.IsAllowLateSubmission);
        }

        /// <summary>
        /// If duedategrace is true in xml data, 
        /// then Item.IsAllowLateSubmission should be true 
        /// </summary>
        [TestCategory("Item"), TestMethod]
        public void ParseEntity_WhenDueDateGraceIsTrue_ExpectAllowLateSubmissionIsFalse()
        {
            var xmlData = "<item id=\"a\"><data><duedategrace>1</duedategrace></data></item>";
            xItemElement = XElement.Parse(xmlData);
            itm.ParseEntity(xItemElement);
            Assert.IsTrue(itm.IsAllowLateSubmission);
        }

        /// <summary>
        /// If duedategrace is true and allowlatesubmission is true in xml data, 
        /// then Item.IsAllowLateSubmission should be true 
        /// </summary>
        [TestCategory("Item"), TestMethod]
        public void ParseEntity_WhenDueDateGraceIsTrue_AndAllowLateSubmissionIsTrue_ExpectAllowLateSubmissionIsFalse()
        {
            var xmlData = "<item id=\"a\"><data><duedategrace>1</duedategrace><allowlatesubmission>true</allowlatesubmission></data></item>";
            xItemElement = XElement.Parse(xmlData);
            itm.ParseEntity(xItemElement);
            Assert.IsTrue(itm.IsAllowLateSubmission);
        }

        /// <summary>
        /// If duedategrace is true and allowlatesubmission is false in xml data, 
        /// then Item.IsAllowLateSubmission should be false 
        /// </summary>
        [TestCategory("Item"), TestMethod]
        public void ParseEntity_WhenDueDateGraceIsTrue_AndAllowLateSubmissionIsFalse_ExpectAllowLateSubmissionIsFalse()
        {
            var xmlData = "<item id=\"a\"><data><duedategrace>1</duedategrace><allowlatesubmission>false</allowlatesubmission></data></item>";
            xItemElement = XElement.Parse(xmlData);
            itm.ParseEntity(xItemElement);
            Assert.IsFalse(itm.IsAllowLateSubmission);
        }

        /// <summary>
        /// If duedategrace is false and allowlatesubmission is true in xml data, 
        /// then Item.IsAllowLateSubmission should be true 
        /// </summary>
        [TestCategory("Item"), TestMethod]
        public void ParseEntity_WhenDueDateGraceIsFalse_AndAllowLateSubmissionIsTrue_ExpectAllowLateSubmissionIsTrue()
        {
            var xmlData = "<item id=\"a\"><data><duedategrace>0</duedategrace><allowlatesubmission>true</allowlatesubmission></data></item>";
            xItemElement = XElement.Parse(xmlData);
            itm.ParseEntity(xItemElement);
            Assert.IsTrue(itm.IsAllowLateSubmission);
        }

        /// <summary>
        /// If duedategrace is false and allowlatesubmission is false in xml data, 
        /// then Item.IsAllowLateSubmission should be false 
        /// </summary>
        [TestCategory("Item"), TestMethod]
        public void ParseEntity_WhenDueDateGraceIsFalse_AndAllowLateSubmissionIsFalse_ExpectAllowLateSubmissionIsFalse()
        {
            var xmlData = "<item id=\"a\"><data><duedategrace>0</duedategrace><allowlatesubmission>false</allowlatesubmission></data></item>";
            xItemElement = XElement.Parse(xmlData);
            itm.ParseEntity(xItemElement);
            Assert.IsFalse(itm.IsAllowLateSubmission);
        }

        /// <summary>
        /// If in examreviewrules, question and answer rules are set to true, item.ShowQuestionsAnswers should be "each"
        /// </summary>
        [TestCategory("Item"), TestMethod]
        public void ParseEntity_WhenSetToShowQuestionsAndAnswer_ExpectShowQuestionsAnswersEqualEach()
        {
            var xmlData = "<item id=\"a\"><data><examreviewrules>" +
                "<rule setting=\"Question\" condition=\"true\" /><rule setting=\"Answer\" condition=\"true\" />" +
                "<rule setting=\"Possible\" condition=\"false\" />" +
                "<rule setting=\"CorrectQuestion\" condition=\"/duedate&lt;=now\" />" +
                "<rule setting=\"CorrectChoice\" condition=\"/duedate&lt;=now\" />" +
                "<rule setting=\"Feedback\" condition=\"/duedate&lt;=now\" />" +
                "<rule setting=\"Feedback-GROUP\" condition=\"false\" />" +
                "</examreviewrules></data></item>";
            xItemElement = XElement.Parse(xmlData);
            itm.ParseEntity(xItemElement);
            Assert.IsTrue(itm.ShowQuestionsAnswers == ReviewSetting.Each);
        }

        /// <summary>
        /// If in examreviewrules, question and answer rules are set to true, item.ShowQuestionsAnswers should be "duedate"
        /// </summary>
        [TestCategory("Item"), TestMethod]
        public void ParseEntity_WhenSetToShowQuestionsAndAnswer_ExpectShowQuestionsAnswersEqualDueDate()
        {
            var xmlData = "<item id=\"a\"><data><examreviewrules>" +
                "<rule setting=\"Question\" condition=\"/duedate&lt;=now\" /><rule setting=\"Answer\" condition=\"/duedate&lt;=now\" />" +
                "<rule setting=\"Possible\" condition=\"false\" />" +
                "<rule setting=\"CorrectQuestion\" condition=\"/duedate&lt;=now\" />" +
                "<rule setting=\"CorrectChoice\" condition=\"/duedate&lt;=now\" />" +
                "<rule setting=\"Feedback\" condition=\"/duedate&lt;=now\" />" +
                "<rule setting=\"Feedback-GROUP\" condition=\"false\" />" +
                "</examreviewrules></data></item>";
            xItemElement = XElement.Parse(xmlData);
            itm.ParseEntity(xItemElement);
            Assert.IsTrue(itm.ShowQuestionsAnswers == ReviewSetting.DueDate);
        }

        /// <summary>
        /// If in examreviewrules, question and answer rules are set to true, item.ShowQuestionsAnswers should be "never"
        /// </summary>
        [TestCategory("Item"), TestMethod]
        public void ParseEntity_WhenSetToShowQuestionsAndAnswer_ExpectShowQuestionsAnswersEqualNever()
        {
            var xmlData = "<item id=\"a\"><data><examreviewrules>" +
                "<rule setting=\"Question\" condition=\"false\" /><rule setting=\"Answer\" condition=\"false\" />" +
                "<rule setting=\"Possible\" condition=\"false\" />" +
                "<rule setting=\"CorrectQuestion\" condition=\"/duedate&lt;=now\" />" +
                "<rule setting=\"CorrectChoice\" condition=\"/duedate&lt;=now\" />" +
                "<rule setting=\"Feedback\" condition=\"/duedate&lt;=now\" />" +
                "<rule setting=\"Feedback-GROUP\" condition=\"false\" />" +
                "</examreviewrules></data></item>";
            xItemElement = XElement.Parse(xmlData);
            itm.ParseEntity(xItemElement);
            Assert.IsTrue(itm.ShowQuestionsAnswers == ReviewSetting.Never);
        }

        /// <summary>
        /// AddHomeworkReviewSettingsXml should not change item grade release date
        /// </summary>
        [TestCategory("Item"), TestMethod]
	    public void AddHomeworkReviewSettingsXml_ShouldNotChangeItemGradeReleaseDate()
        {
            itm.ShowScoreAfter = ReviewSetting.DueDate;
            var existingReleaseDate = DateTime.Today.AddMonths(2).Date.ToString("s") + "Z";
            var xmlData = "<data><gradereleasedate>" + existingReleaseDate + "</gradereleasedate></data>";
            var data = XElement.Parse(xmlData);
	        itm.AddHomeworkReviewSettingsXml(data);
            Assert.AreEqual(existingReleaseDate, data.Element(ElStrings.GradeReleaseDate).Value);
	    }


        /// <summary>
        /// Item.ShowQuestionsAnswers should be passed in Data as Question and Answer elements
        /// </summary>
        [TestCategory("Item"), TestMethod]
        public void AddHomeworkReviewSettingsXml_WhenShowQuestionsAnswers_ExpectDataHasQuestionAndAnswerElement()
        {
            itm.ShowQuestionsAnswers = ReviewSetting.DueDate;
            var xmlData = "<data><examreviewrules></examreviewrules></data>";
            var data = XElement.Parse(xmlData);
            itm.AddHomeworkReviewSettingsXml(data);
            var examReviewRulesElement = data.Element(ElStrings.ExamReviewRules);
            Assert.IsNotNull(examReviewRulesElement);
            bool foundQuestionElement = false, foundAnswerElement = false;
            var rules = examReviewRulesElement.Elements("rule");
            Assert.IsTrue(rules != null && rules.Any());
            foreach (var rule in rules)
            {
                if (rule.Attribute("setting").Value == "Question")
                    foundQuestionElement = true;
                else if (rule.Attribute("setting").Value == "Answer")
                    foundAnswerElement = true;
            }
            Assert.IsTrue(foundAnswerElement);
            Assert.IsTrue(foundQuestionElement);

        }

        /// <summary>
        /// Item.ShowScoreAfter should be passed in Data as Possible element
        /// </summary>
        [TestCategory("Item"), TestMethod]
        public void AddHomeworkReviewSettingsXml_WhenShowScoreAfter_ExpectDataHasPossibleElement()
        {
            itm.ShowScoreAfter = ReviewSetting.DueDate;
            var xmlData = "<data><examreviewrules></examreviewrules></data>";
            var data = XElement.Parse(xmlData);
            itm.AddHomeworkReviewSettingsXml(data);
            var examReviewRulesElement = data.Element(ElStrings.ExamReviewRules);
            Assert.IsNotNull(examReviewRulesElement);
            bool foundPossibleElement = false;
            var rules = examReviewRulesElement.Elements("rule");
            Assert.IsTrue(rules != null && rules.Any());
            foreach (var rule in rules)
            {
                if (rule.Attribute("setting").Value == "Possible")
                    foundPossibleElement = true;
            }
            Assert.IsTrue(foundPossibleElement);

        }

        /// <summary>
        /// Item.ShowRightWrong should be passed in Data as CorrectQuestion element
        /// </summary>
        [TestCategory("Item"), TestMethod]
        public void AddHomeworkReviewSettingsXml_WhenShowRightWrong_ExpectDataHasCorrectQuestionElement()
        {
            itm.ShowRightWrong = ReviewSetting.DueDate;
            var xmlData = "<data><examreviewrules></examreviewrules></data>";
            var data = XElement.Parse(xmlData);
            itm.AddHomeworkReviewSettingsXml(data);
            var examReviewRulesElement = data.Element(ElStrings.ExamReviewRules);
            Assert.IsNotNull(examReviewRulesElement);
            bool foundCorrectQuestionElement = false;
            var rules = examReviewRulesElement.Elements("rule");
            Assert.IsTrue(rules != null && rules.Any());
            foreach (var rule in rules)
            {
                if (rule.Attribute("setting").Value == "CorrectQuestion")
                    foundCorrectQuestionElement = true;
            }
            Assert.IsTrue(foundCorrectQuestionElement);

        }

        /// <summary>
        /// Item.ShowAnswers should be passed in Data as CorrectChoice element
        /// </summary>
        [TestCategory("Item"), TestMethod]
        public void AddHomeworkReviewSettingsXml_WhenShowAnswers_ExpectDataHasCorrectChoiceElement()
        {
            itm.ShowAnswers = ReviewSetting.DueDate;
            var xmlData = "<data><examreviewrules></examreviewrules></data>";
            var data = XElement.Parse(xmlData);
            itm.AddHomeworkReviewSettingsXml(data);
            var examReviewRulesElement = data.Element(ElStrings.ExamReviewRules);
            Assert.IsNotNull(examReviewRulesElement);
            bool foundCorrectChoiceElement = false;
            var rules = examReviewRulesElement.Elements("rule");
            Assert.IsTrue(rules != null && rules.Any());
            foreach (var rule in rules)
            {
                if (rule.Attribute("setting").Value == "CorrectChoice")
                    foundCorrectChoiceElement = true;
            }
            Assert.IsTrue(foundCorrectChoiceElement);

        }

        /// <summary>
        /// Item.ShowFeedbackAndRemarks should be passed in Data as Feedback element
        /// </summary>
        [TestCategory("Item"), TestMethod]
        public void AddHomeworkReviewSettingsXml_WhenShowFeedbackAndRemarks_ExpectDataHasFeedbackElement()
        {
            itm.ShowAnswers = ReviewSetting.DueDate;
            var xmlData = "<data><examreviewrules></examreviewrules></data>";
            var data = XElement.Parse(xmlData);
            itm.AddHomeworkReviewSettingsXml(data);
            var examReviewRulesElement = data.Element(ElStrings.ExamReviewRules);
            Assert.IsNotNull(examReviewRulesElement);
            bool foundFeedbackElement = false;
            var rules = examReviewRulesElement.Elements("rule");
            Assert.IsTrue(rules != null && rules.Any());
            foreach (var rule in rules)
            {
                if (rule.Attribute("setting").Value == "Feedback")
                    foundFeedbackElement = true;
            }
            Assert.IsTrue(foundFeedbackElement);

        }

        /// <summary>
        /// Item.ShowSolutions should be passed in Data as Feedback-GROUP element
        /// </summary>
        [TestCategory("Item"), TestMethod]
        public void AddHomeworkReviewSettingsXml_WhenShowSolutions_ExpectDataHasFeedbackGroupElement()
        {
            itm.ShowAnswers = ReviewSetting.DueDate;
            var xmlData = "<data><examreviewrules></examreviewrules></data>";
            var data = XElement.Parse(xmlData);
            itm.AddHomeworkReviewSettingsXml(data);
            var examReviewRulesElement = data.Element(ElStrings.ExamReviewRules);
            Assert.IsNotNull(examReviewRulesElement);
            bool foundFeedbackGROUPElement = false;
            var rules = examReviewRulesElement.Elements("rule");
            Assert.IsTrue(rules != null && rules.Any());
            foreach (var rule in rules)
            {
                if (rule.Attribute("setting").Value == "Feedback")
                    foundFeedbackGROUPElement = true;
            }
            Assert.IsTrue(foundFeedbackGROUPElement);

        }
		[TestCleanup]
		public void TestCleanup()
		{
			itm_WithContainerDlapExact_IsNull = null;
			itm = null;
		}
	}
}
