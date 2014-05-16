using System.Linq;
using System.Xml.Linq;
using Macmillan.PXQBA.Business.Commands.Helpers;
using Macmillan.PXQBA.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macmillan.PXQBA.Business.Services.Tests
{
    [TestClass]
    public class QuestionDataXmlParserTests
    {

        public static string XmlString = @"
                     <doc entityid=""71836"" class=""question"" questionid=""B8B35A1E8D1A4A70A2E622727A135D4A"">
                        <str name=""dlap_id"">71836|Q|B8B35A1E8D1A4A70A2E622727A135D4A</str>
                        <str name=""dlap_class"">question</str>
                        <str name=""dlap_q_type"">text</str>
                        <arr name=""dlap_html_text"">
                        <str>
                        A premature identity formation that involves wholesale acceptance of parental values is called _______ .
                        </str>
                        <str>foreclosure</str>
                        </arr>
                        <str name=""dlap_q_texttype"">IgnoreCase</str>
                        <arr name=""title"">
                        <str>
                        4. (FB) A premature identity formation that involves wholesale acceptance of parental values is called _____...
                        </str>
                        </arr>
                        <arr name=""product-course-defaults/chapter"">
                        <str>Chapter 16</str>
                        </arr>
                        <arr name=""product-course-defaults/bank"">
                        <str>Fill In The Blank</str>
                        </arr>
                        <arr name=""product-course-defaults/sequence_dlap_l"">
                        <long>1130</long>
                        </arr>
                        <arr name=""product-course-defaults/sequence_dlap_d"">
                        <double>1130.0</double>
                        </arr>
                        <arr name=""product-course-defaults/sequence"">
                        <str>1130</str>
                        </arr>
                        <arr name=""product-course-defaults/page_dlap_l"">
                        <long>472</long>
                        </arr>
                        <arr name=""product-course-defaults/page_dlap_d"">
                        <double>472.0</double>
                        </arr>
                        <arr name=""product-course-defaults/page"">
                        <str>472</str>
                        </arr>
                        <arr name=""product-course-defaults/subtopic"">
                        <str>Not Yet Achieved</str>
                        </arr>
                        <arr name=""product-course-defaults/topic"">
                        <str>Identity</str>
                        </arr>
                        <arr name=""product-course-defaults/whyl_dlap_l"">
                        <long>3</long>
                        </arr>
                        <arr name=""product-course-defaults/whyl_dlap_d"">
                        <double>3.0</double>
                        </arr>
                        <arr name=""product-course-defaults/whyl"">
                        <str>3</str>
                        </arr>
                        <arr name=""product-course-defaults/format"">
                        <str>Fill-in-the-Blank</str>
                        </arr>
                        <arr name=""product-course-defaults/title"">
                        <str>
                        4. (FB) A premature identity formation that involves wholesale acceptance of parental values is called _____...
                        </str>
                        </arr>
                        <arr name=""product-course-id-71836/productcourseid_dlap_l"">
                        <long>71836</long>
                        </arr>
                        <arr name=""product-course-id-71836/productcourseid_dlap_d"">
                        <double>71836.0</double>
                        </arr>
                        <arr name=""product-course-id-71836/productcourseid"">
                        <str>71836</str>
                        </arr>
                        <arr name=""product-course-id-71836/chapter"">
                        <str>Chapter 16</str>
                        <str>Chapter 166</str>
                        </arr>
                        <arr name=""product-course-id-71836/bank"">
                        <str>Fill In The Blank</str>
                        </arr>
                        <arr name=""product-course-id-71836/sequence_dlap_l"">
                        <long>1129</long>
                        </arr>
                        <arr name=""product-course-id-71836/sequence_dlap_d"">
                        <double>1129.0</double>
                        </arr>
                        <arr name=""product-course-id-71836/sequence"">
                        <str>1129</str>
                        </arr>
                        <arr name=""product-course-id-85256/chapter"">
                        <str>Chapter 17</str>
                        </arr>
                        <arr name=""product-course-id-85256/bank"">
                        <str>Fill In The Blank</str>
                        </arr>
                        <arr name=""product-course-id-85256/sequence_dlap_l"">
                        <long>1132</long>
                        </arr>
                        <arr name=""product-course-id-85256/sequence_dlap_d"">
                        <double>1132.0</double>
                        </arr>
                        <arr name=""product-course-id-85256/sequence"">
                        <str>1132</str>
                        </arr>
                        <arr name=""usedin"">
                        <str>
                        LOR_psychportal__bergerca9edsm5__master_QUIZ_D2AF53C24F299728DE6E2F42053B02CA
                        </str>
                        </arr>
                        <arr name=""page_dlap_l"">
                        <long>472</long>
                        </arr>
                        <arr name=""page_dlap_d"">
                        <double>472.0</double>
                        </arr>
                        <arr name=""page"">
                        <str>472</str>
                        </arr>
                        <arr name=""subtopic"">
                        <str>Not Yet Achieved</str>
                        </arr>
                        <arr name=""topic"">
                        <str>Identity</str>
                        </arr>
                        <arr name=""whyl_dlap_l"">
                        <long>3</long>
                        </arr>
                        <arr name=""whyl_dlap_d"">
                        <double>3.0</double>
                        </arr>
                        <arr name=""whyl"">
                        <str>3</str>
                        </arr>
                        <arr name=""format"">
                        <str>Fill-in-the-Blank</str>
                        </arr>
                        <arr name=""questionstatus_dlap_l"">
                        <long>1</long>
                        </arr>
                        <arr name=""questionstatus_dlap_d"">
                        <double>1.0</double>
                        </arr>
                        <arr name=""questionstatus"">
                        <str>1</str>
                        </arr>
                        <arr name=""exercisenumber_dlap_l"">
                        <long>999</long>
                        </arr>
                        <arr name=""exercisenumber_dlap_d"">
                        <double>999.0</double>
                        </arr>
                        <arr name=""exercisenumber"">
                        <str>999</str>
                        </arr>
                        <arr name=""score"">
                        <float name=""score"">16.262583</float>
                        </arr>
                     </doc>";

        [TestMethod]
        public void ParseQuestionData()
        {
            XElement questionXmlData = XElement.Parse(XmlString);
            var question = QuestionDataXmlParser.ToQuestionEntity(questionXmlData);

            Assert.IsNotNull(question);
            Assert.Equals(question.Id, "B8B35A1E8D1A4A70A2E622727A135D4A");
            //Assert.IsTrue(fields.SingleOrDefault(x => x.Friendlyname == "Difficulty").CourseMetadataFieldValues.Count() == 3);
            //Assert.IsTrue(fields.SingleOrDefault(x => x.Name == "guidance").Filterable == false);

            //Assert.IsFalse(fields.SingleOrDefault(x => x.Friendlyname == "Core Concept").CourseMetadataFieldValues.Any());
        }
    }
}