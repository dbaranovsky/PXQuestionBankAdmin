using System.Linq;
using System.Xml.Linq;
using Macmillan.PXQBA.Business.Commands.Helpers;
using Macmillan.PXQBA.Business.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macmillan.PXQBA.Business.Services.Tests
{
    [TestClass]
    public class CourseDataXmlParserTests
    {

        public static string XmlString = @"
                      <data>
                        <QuestionBankRepositoryCourse>22250</QuestionBankRepositoryCourse>
                        <meta-available-question-data>
                            <coreconcept filterable=""true"" friendlyname=""Core Concept"" />
                            <difficulty filterable=""true"" searchterm=""difficulty:"" friendlyname=""Difficulty"">
                                <value text=""Easy"" sequence=""1"" />
                                <value text=""Medium"" sequence=""2""/>
                                <value text=""Hard"" sequence=""3"" />
                            </difficulty>
                            <cognitivelevel filterable=""true"" searchterm=""cognitivelevel:"" friendlyname=""Cognitive Level"" />
                            <bloomdomain filterable=""true"" searchterm=""bloomdomain:"" friendlyname=""Bloom's level"" />
                            <suggesteduse filterable=""true"" friendlyname=""Suggested Use"" />
                            <guidance filterable=""false"" />
                            <bank filterable=""true"" friendlyname=""Bank"" type=""single-select"">
                                <value text=""End of Chapter Questions"" sequence=""3"" />
                                <value text=""Beginning of Chapter Questions"" sequence=""1""/>
                                <value text=""Middle of Chapter Questions"" sequence=""2"" />
                            </bank>
                            <chapter filterable=""true"" friendlyname=""Module"" type=""single-select"">
                                <value text=""Chapter 1"" sequence=""1"" />
                                <value text=""Chapter 10"" sequence=""2""/>
                                <value text=""Chapter 20"" sequence=""3"" />
                            </chapter>
                        </meta-available-question-data>
                      </data>";

        [TestMethod]
        public void ParseMetaAvailableQuestionData_Parsed()
        {
            XElement courseDataXml = XElement.Parse(XmlString);
            var fields = CourseDataXmlParser.ParseMetaAvailableQuestionData(courseDataXml);
            
            Assert.IsTrue(fields.Count()==8);
            Assert.IsTrue(fields.SingleOrDefault(x => x.Friendlyname == "Difficulty").CourseMetadataFieldValues.Count()==3);
            Assert.IsTrue(fields.SingleOrDefault(x => x.Friendlyname == "Difficulty")
                            .CourseMetadataFieldValues.Where(i => i.Sequence == 1)
                            .SingleOrDefault().Text == "Easy");

            Assert.IsTrue(fields.SingleOrDefault(x => x.Name == "guidance").Filterable==false);
            Assert.IsFalse(fields.SingleOrDefault(x => x.Friendlyname == "Core Concept").CourseMetadataFieldValues.Any());
        }

        [TestMethod]
        public void ParseMetaAvailableQuestionData_Difficulty_Parsed()
        {
            XElement courseDataXml = XElement.Parse(XmlString);
            var fields = CourseDataXmlParser.ParseMetaAvailableQuestionData(courseDataXml);

            var difficultyFieldValues = fields.SingleOrDefault(x => x.Friendlyname == "Difficulty").CourseMetadataFieldValues;

             Assert.IsTrue(difficultyFieldValues.Where(i => i.Sequence == 1)
                           .SingleOrDefault().Text == "Easy");
             Assert.IsTrue(difficultyFieldValues.Where(i => i.Sequence == 2)
                           .SingleOrDefault().Text == "Medium");
             Assert.IsTrue(difficultyFieldValues.Where(i => i.Sequence == 3)
                           .SingleOrDefault().Text == "Hard");
        }


        [TestMethod]
        public void ParseMetaAvailableQuestionData_SingleSelect_Parsed()
        {
            XElement courseDataXml = XElement.Parse(XmlString);
            var fields = CourseDataXmlParser.ParseMetaAvailableQuestionData(courseDataXml);

            Assert.IsTrue(fields.SingleOrDefault(x => x.Friendlyname == "Module").Type == MetadataFieldType.SingleSelect);
            Assert.IsTrue(fields.SingleOrDefault(x => x.Name == "bank").Type == MetadataFieldType.SingleSelect);
        }

        [TestMethod]
        public void ParseMetaAvailableQuestionData_Type_TextByDefault()
        {
            XElement courseDataXml = XElement.Parse(XmlString);
            var fields = CourseDataXmlParser.ParseMetaAvailableQuestionData(courseDataXml);

            Assert.IsTrue(fields.SingleOrDefault(x => x.Name == "coreconcept").Type == MetadataFieldType.Text);
        }

        [TestMethod]
        public void ParseQuestionBankRepositoryCourse_Parsed()
        {
            XElement courseDataXml = XElement.Parse(XmlString);
            var questionBankRepositoryCourse = CourseDataXmlParser.ParseQuestionBankRepositoryCourse(courseDataXml);

            Assert.AreEqual("22250", questionBankRepositoryCourse);
        }
    }
}
