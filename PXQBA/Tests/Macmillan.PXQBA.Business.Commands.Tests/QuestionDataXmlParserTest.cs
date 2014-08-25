using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Macmillan.PXQBA.Business.Commands.Helpers;
using Macmillan.PXQBA.Business.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macmillan.PXQBA.Business.Commands.Tests
{
    [TestClass]
    public class QuestionDataXmlParserTest
    {
        [TestMethod]
        public void ToSearchResultEntity_ResultDocWithElements_SearchedByField()
        {
            var result = QuestionDataXmlParser.ToSearchResultEntity(XElement.Parse(searchEntityWithElements), "score", new[] { "bank" });
            Assert.IsTrue(result.DynamicFields.Count == 1);
            Assert.IsTrue(result.DynamicFields["bank"] == "Test Bank");
            Assert.IsTrue(result.SortingField == "9.422424");
        }

        [TestMethod]
        public void ToSearchResultEntity_ResultDocWithoutElements_SearchedByField()
        {
            var result = QuestionDataXmlParser.ToSearchResultEntity(XElement.Parse(searchEntityWithoutElements), "score", new[] { "bank" });
            Assert.IsTrue(result.DynamicFields.Count == 1);
            Assert.IsTrue(result.QuestionId == "2CB4C3C29D8EEDCFC6316EC5A4000044");
         
        }

        [TestMethod]
        public void ToFacetedSearchResult_ResultDoc_FacetedSearchResult()
        {
           var result = QuestionDataXmlParser.ToFacetedSearchResult(XElement.Parse(facetedResult));
            Assert.IsTrue(result.Count() == 24);
            Assert.IsTrue(result.First(x => x.FacetedFieldValue == "Chapter 1").FacetedCount==3);
        }
        [TestMethod]
        public void GetDefaultSectionValues_MetdataElements_DeafultSection()
        {
           var section = QuestionDataXmlParser.GetDefaultSectionValues(new Dictionary<string, XElement>()
                                                                      {
                                                                          {"product-course-id-12", XElement.Parse(productCourseSection123)},
                                                                          {"product-course-defaults", XElement.Parse(productCourseSection)}
                                                                      });
           Assert.IsTrue(section.Bank == "Bank");
           Assert.IsTrue(section.DynamicValues.Count == 1);
          
        }

        [TestMethod]
        public void GetProductCourseSectionValues_MetdataElements_DeafultSection()
        {
            var sections = QuestionDataXmlParser.GetProductCourseSectionValues(new Dictionary<string, XElement>()
                                                                      {
                                                                          {"product-course-id-123", XElement.Parse(productCourseSection123)},
                                                                          {"product-course-id-12", XElement.Parse(productCourseSection)}
                                                                      });

            Assert.IsTrue(sections.Count()== 2);
            Assert.IsTrue(sections.Any(x => x.DynamicValues.Any() && x.DynamicValues.First().Value.First() == "1"));
        }

          [TestMethod]
        public void GetMetadataField_MetadaElements_BankValue()
        {
            var result = QuestionDataXmlParser.GetMetadataField(new Dictionary<string, XElement>
                                                                {
                                                                    {MetadataFieldNames.Chapter, new XElement("chptr")},
                                                                    {
                                                                        MetadataFieldNames.Bank,
                                                                        new XElement("bank") {Value = "test"}
                                                                    }

                                                                }, "bank");
            Assert.IsTrue(result == "test");

        }  
        
        [TestMethod]
          public void ToXmlElements_QuestionToConvert_CorrectXml()
        {

            var question = new Models.Question()
            {
                Id = "1",
                Version = 3,
                ProductCourseSections = new List<QuestionMetadataSection>()
                                                        {
                                                            new QuestionMetadataSection()
                                                            {
                                                                ProductCourseId = "12",
                                                                Bank = "Test Bank"
                                                            }
                                                        },
                InteractionType = "2"

            };

            var result = QuestionDataXmlParser.ToXmlElements(question);
            Assert.IsTrue(result.Count() == 8);
            Assert.IsTrue(result.ContainsKey("product-course-id-12"));

        }


        #region test xml's


        private string searchEntityWithElements = @"
    <doc entityid=""6712"" class=""question"" questionid=""2CB4C3C29D8EEDCFC6316EC5A4000044"">
      <str name=""dlap_id"">6712|Q|2CB4C3C29D8EEDCFC6316EC5A4000044</str>
      <str name=""dlap_class"">question</str>
      <str name=""bank"">Test Bank</str>
      <arr name=""draftfrom"">
        <item>41546</item>
        <item>4154dfsdf6</item>
      </arr>
      <arr name=""score"">
        <float name=""score"">9.422424</float>
      </arr>
    </doc>";


        private string searchEntityWithoutElements = @"
    <doc entityid=""6712"" class=""question"" questionid=""2CB4C3C29D8EEDCFC6316EC5A4000044"">
      <str name=""dlap_id"">6712|Q|2CB4C3C29D8EEDCFC6316EC5A4000044</str>
      <str name=""dlap_class"">question</str>
      <str name=""bank""></str>
      <arr name=""draftfrom"">
      </arr>
      <arr name=""score"">
      
      </arr>
    </doc>";

        private string facetedResult = @"

      <lst name=""product-course-id-12/chapter_dlap_e"">
        <int>434</int>
        <int name=""Chapter 11"">dfs</int>
        <int name="""">417</int>
        <int name=""Chapter 3""></int>
        <int name=""Chapter 7"">384</int>
        <int name=""Chapter 10"">378</int>
        <int name=""Chapter 2"">376</int>
        <int name=""Chapter 8"">369</int>
        <int name=""Chapter 13"">356</int>
        <int name=""Chapter 19"">336</int>
        <int name=""Chapter 15"">327</int>
        <int name=""Chapter 16"">325</int>
        <int name=""Chapter 17"">324</int>
        <int name=""Chapter 5"">312</int>
        <int name=""Chapter 14"">311</int>
        <int name=""Chapter 4"">294</int>
        <int name=""Chapter 9"">291</int>
        <int name=""Chapter 20"">287</int>
        <int name=""Chapter 10A"">244</int>
        <int name=""Chapter 18"">217</int>
        <int name=""Chapter 2A"">75</int>
        <int name=""Chapter 19A"">41</int>
        <int name=""Chapter 1"">3</int>
        <int name=""First Chapter"">0</int>
      </lst>";

   
        private const string productCourseSection = @"<product-course-id-12>
<bank>Bank</bank>
<chapter>Test Chapter</chapter>
<productcourseid>12</productcourseid>
<sequence>3</sequence>
<field>1</field>
</product-course-id-12>";

        private const string productCourseSection123 = @"<product-course-id-123>
<bank>Test Bank</bank>
<chapter>Test Chapter</chapter>
<productcourseid>12</productcourseid>
<sequence>3</sequence>
</product-course-id-123>";

     
      
        #endregion
    }
}
