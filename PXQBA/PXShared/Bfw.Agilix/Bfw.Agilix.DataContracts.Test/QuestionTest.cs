using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Bfw.Agilix.Dlap;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace Bfw.Agilix.DataContracts.Tests
{
    [TestClass]
    public class QuestionTest
    {
        [TestInitialize]
        public void TestInitialize()
        {

        }

        [TestMethod]
        public void ToEntity_Should_Have_IgnoreCaseTextType_For_ShortAnswer()
        {
            var question = new Question()
            {
                Id = "1",
                EntityId = "2",
                InteractionType = "text"
            };

            var result = question.ToEntity();

            Assert.AreEqual("IgnoreCase", result.Element(ElStrings.Interaction).Attribute(ElStrings.TextType).Value);
        }

        [TestMethod]
        public void Verify_If_Answers_Element_Is_Copied_Successfully_For_ShortAnswer_Questions()
        {
            var question = new Question()
            {
                Id = "1",
                EntityId = "2",
                InteractionType = "text",
                AnswerList = new List<string> { "20", "30", "40" }
            };

            var result = question.ToEntity();
            var nodeCount = result.Element("answer").Nodes().Count();
            Assert.AreEqual(3, nodeCount);
        }

        [TestMethod]
        public void ToEntity_Should_WriteInteractionType()
        {
            var question = new Question()
            {
                Id = "q1",
                EntityId = "ent1",
                Interaction = new QuestionInteraction()
                {
                    Flags = QuestionInteractionFlags.ExtraCredit | QuestionInteractionFlags.MaintainOrder, //=1000000010 = 0x202 = 514
                    Height = 10,
                    SignificantFigures = 11,
                    TextType = "text",
                    Type = "choice",
                    Width = 12
                }
            };

            //Act
            var result = question.ToEntity();

            //Assert
            var interactionElem = result.Element("interaction");
            Assert.IsNotNull(interactionElem);
            Assert.AreEqual("514", interactionElem.Attribute("flags").Value);
            Assert.AreEqual("10", interactionElem.Attribute("height").Value);
            Assert.AreEqual("choice", interactionElem.Attribute("type").Value);
            Assert.AreEqual("text", interactionElem.Attribute("texttype").Value);
            Assert.AreEqual("11", interactionElem.Attribute("significantfigures").Value);
            Assert.AreEqual("12", interactionElem.Attribute("width").Value);
        }

        /// <summary>
        /// Testing the following data
        /// <interaction type="match" flags="2" width="10" height="11" significantfigures="12" texttype="IgnoreCase">
        /// </summary>
        [TestMethod]
        public void ToEntity_Should_ReadInteractionTypes()
        {
            //Assign
            var questionsXml =
                Helper.GetXDocument(Entity.Questions.ToString())
                    .Element("responses")
                    .Element("response")
                    .Elements("question")
                    .FirstOrDefault();

            var question = new Question();

            //Act
            question.ParseEntity(questionsXml);
            //Assert
            Assert.AreEqual("match", question.Interaction.Type);
            Assert.AreEqual("match", question.InteractionType);
            Assert.AreEqual(10, question.Interaction.Width);
            Assert.AreEqual(11, question.Interaction.Height);
            Assert.AreEqual(12, question.Interaction.SignificantFigures);
            Assert.AreEqual("IgnoreCase", question.Interaction.TextType);
            Assert.AreEqual(QuestionInteractionFlags.MaintainOrder, question.Interaction.Flags);
        }
    }
    
    
}
