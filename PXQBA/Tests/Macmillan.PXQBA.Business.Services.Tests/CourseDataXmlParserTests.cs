using System.Linq;
using System.Xml.Linq;
using Macmillan.PXQBA.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macmillan.PXQBA.Business.Services.Tests
{
    [TestClass]
    public class CourseDataXmlParserTests
    {

        public static string XmlString = @"
                      <data>
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
        public void ParseMetaAvailableQuestionData()
        {
            XElement courseDataXml = XElement.Parse(XmlString);
            var fields = CourseDataXmlParser.ParseMetaAvailableQuestionData(courseDataXml);

            Assert.IsTrue(fields.Count()==8);
            Assert.IsTrue(fields.SingleOrDefault(x => x.Friendlyname == "Difficulty").CourseMetadataFieldValues.Count()==3);
            Assert.IsTrue(fields.SingleOrDefault(x => x.Name == "guidance").Filterable==false);

            Assert.IsFalse(fields.SingleOrDefault(x => x.Friendlyname == "Core Concept").CourseMetadataFieldValues.Any());
        }
    }
}
