using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Bfw.Agilix.DataContracts;
using Macmillan.PXQBA.Business.Commands.DataContracts;
using Macmillan.PXQBA.Business.Models;
using Question = Macmillan.PXQBA.Business.Models.Question;

namespace Macmillan.PXQBA.Business.Commands.Helpers
{
    public static class QuestionDataXmlParser
    {
        private const string DlapNamePart = "_dlap_";

        public static Question ToQuestionEntity(XElement resultDoc)
        {
            var question = new Question();
            question.Id = resultDoc.Attribute("questionid").Value;
            resultDoc.Elements()
                        .Where(elem => elem.Name.LocalName.Equals("arr")).ToList()
                        .ForEach(elem =>
                        {
                            var name = elem.Attribute("name").Value;
                            if (name.Contains(ElStrings.ProductCourseDefaults.ToString()) && !name.Contains(DlapNamePart))
                            {
                                List<string> values = elem.Elements().Select(v => v.Value).ToList();
                                var metafieldName = name.Substring(name.IndexOf('/') + 1);
                                if (metafieldName == MetadataFieldNames.DlapTitle)
                                {
                                    question.DefaultSection.Title = values.FirstOrDefault();
                                }
                                else if (metafieldName == MetadataFieldNames.Bank)
                                {
                                    question.DefaultSection.Bank = values.FirstOrDefault();
                                }
                                else if (metafieldName == MetadataFieldNames.Chapter)
                                {
                                    question.DefaultSection.Chapter = values.FirstOrDefault();
                                }
                                else
                                {
                                    question.DefaultSection.DynamicValues.Add(metafieldName, values);
                                }
                            }
                            if (name.Contains(ElStrings.ProductCourseSection.ToString()) && !name.Contains(DlapNamePart))
                            {
                                var productCourseId = name.Split(new[] { ElStrings.ProductCourseSection.ToString(), "/" }, StringSplitOptions.RemoveEmptyEntries)[0];
                                List<string> values = elem.Elements().Select(v => v.Value).ToList();
                                var productCourse = question.ProductCourseSections.FirstOrDefault(c => c.ProductCourseId == productCourseId);
                                if (productCourse == null)
                                {
                                    productCourse = new QuestionMetadataSection();
                                    productCourse.ProductCourseId = productCourseId;
                                    question.ProductCourseSections.Add(productCourse);
                                }
                                var metafieldName = name.Substring(name.IndexOf('/') + 1);
                                if (metafieldName == MetadataFieldNames.DlapTitle)
                                {
                                    productCourse.Title = values.FirstOrDefault();
                                }
                                else if (metafieldName == MetadataFieldNames.Bank)
                                {
                                    productCourse.Bank = values.FirstOrDefault();
                                }
                                else if (metafieldName == MetadataFieldNames.Chapter)
                                {
                                    productCourse.Chapter = values.FirstOrDefault();
                                }
                                else
                                {
                                    productCourse.DynamicValues.Add(metafieldName, values);
                                }
                            }

                        });
            return question;
        }

        public static QuestionSearchResult ToSearchResultEntity(XElement resultDoc, string sortingField)
        {
            var questionSearchResult = new QuestionSearchResult();
            questionSearchResult.QuestionId = resultDoc.Attribute("questionid").Value;
            questionSearchResult.SortingField = string.Empty;
            var sortingElement = resultDoc.Elements()
                .FirstOrDefault(
                    elem => elem.Attribute("name").Value == sortingField);
            if (sortingElement != null)
            {
                if (sortingElement.HasElements)
                {
                    questionSearchResult.SortingField = string.Join(", ", sortingElement.Elements().Select(v => v.Value));
                }
                else
                {
                    questionSearchResult.SortingField = string.Join(", ", sortingElement.Value);
                }
            }
            var draftFrom = resultDoc.Elements()
                .FirstOrDefault(
                    elem => elem.Attribute("name").Value == MetadataFieldNames.DraftFrom);
            if (draftFrom != null)
            {
                questionSearchResult.DraftFrom = string.Join(", ", draftFrom.Elements().Select(v => v.Value));
            }
            return questionSearchResult;
        }

        public static QuestionMetadataSection GetDefaultSectionValues(Dictionary<string, XElement> metadataElements)
        {

            if (metadataElements == null || !metadataElements.ContainsKey(ElStrings.ProductCourseDefaults.ToString()))
            {
                return new QuestionMetadataSection();
            }
            var defaultsSection = metadataElements[ElStrings.ProductCourseDefaults.ToString()].Elements();
            return GetSectionValues(defaultsSection);
        }

        public static List<QuestionMetadataSection> GetProductCourseSectionValues(Dictionary<string, XElement> metadataElements)
        {
            if (metadataElements == null)
            {
                return new List<QuestionMetadataSection>()
                       {
                           new QuestionMetadataSection()
                       };
            }
            var productCourseSections = metadataElements.Where(elem => elem.Key.Contains(ElStrings.ProductCourseSection.ToString()));
            return productCourseSections.Select(productCourseSection => GetSectionValues(productCourseSection.Value.Elements())).ToList();
        }

        private static QuestionMetadataSection GetSectionValues(IEnumerable<XElement> sectionElements)
        {
            var sectionValues = new QuestionMetadataSection();
            sectionValues.ProductCourseId = GetXElementValue(sectionElements, MetadataFieldNames.ProductCourse);
            sectionValues.Title = GetXElementValue(sectionElements, MetadataFieldNames.DlapTitle);
            sectionValues.Bank = GetXElementValue(sectionElements, MetadataFieldNames.Bank);
            sectionValues.Chapter = GetXElementValue(sectionElements, MetadataFieldNames.Chapter);
            sectionValues.Sequence = GetXElementValue(sectionElements, MetadataFieldNames.Sequence);
            sectionValues.ParentProductCourseId = GetXElementValue(sectionElements, MetadataFieldNames.ParentProductCourseId);
            sectionValues.DynamicValues = sectionElements.Where(g => !MetadataFieldNames.GetStaticFieldNames().Contains(g.Name.LocalName)).GroupBy(elem => elem.Name.LocalName).ToDictionary(group => group.Key, group => group.Select(elem => elem.Value).ToList());
            return sectionValues;
        }

        private static string GetXElementValue(IEnumerable<XElement> elements, string fieldName)
        {
            var element = elements.FirstOrDefault(g => g.Name.LocalName == fieldName);
            return element != null ? element.Value : string.Empty;
        }

        public static Dictionary<string, XElement> ToXmlElements(Question question)
        {
            var elements = new Dictionary<string, XElement>();
            var defaultsSection = new XElement(ElStrings.ProductCourseDefaults);
            defaultsSection.Add(GetXmlElementsFromSection(question.DefaultSection));
           
            elements.Add(ElStrings.ProductCourseDefaults.ToString(), defaultsSection);
            elements.Add(MetadataFieldNames.DuplicateFromShared, new XElement(MetadataFieldNames.DuplicateFromShared, question.DuplicateFromShared));
            elements.Add(MetadataFieldNames.DuplicateFrom, new XElement(MetadataFieldNames.DuplicateFrom, question.DuplicateFrom));
            elements.Add(MetadataFieldNames.DraftFrom, new XElement(MetadataFieldNames.DraftFrom, question.DraftFrom));
            elements.Add(MetadataFieldNames.RestoredFromVersion, new XElement(MetadataFieldNames.RestoredFromVersion, question.RestoredFromVersion));
            elements.Add(MetadataFieldNames.IsPublishedFromDraft, new XElement(MetadataFieldNames.IsPublishedFromDraft, question.IsPublishedFromDraft));
            elements.Add(MetadataFieldNames.ModifiedBy, new XElement(MetadataFieldNames.ModifiedBy, question.ModifiedBy));
            foreach (var productCourseSection in question.ProductCourseSections)
            {
                var productCourseSectionName = string.Format("{0}{1}", ElStrings.ProductCourseSection, productCourseSection.ProductCourseId);
                if (!elements.ContainsKey(productCourseSectionName))
                {
                    var section = new XElement(productCourseSectionName);
                    section.Add(GetXmlElementsFromSection(productCourseSection));
                    elements.Add(productCourseSectionName, section);
                }
            }
            return elements;
        }

        private static IEnumerable<XElement> GetXmlElementsFromSection(QuestionMetadataSection section)
        {
            var elements = new List<XElement>();
            foreach (var defaultValue in section.DynamicValues)
            {
                foreach (var value in defaultValue.Value)
                {
                    elements.Add(new XElement(defaultValue.Key, value));
                }
            }
            elements.Add(new XElement(MetadataFieldNames.DlapTitle, section.Title));
            elements.Add(new XElement(MetadataFieldNames.ProductCourse, section.ProductCourseId));
            elements.Add(new XElement(MetadataFieldNames.Bank, section.Bank));
            elements.Add(new XElement(MetadataFieldNames.Chapter, section.Chapter));
            elements.Add(new XElement(MetadataFieldNames.Sequence, section.Sequence));
            elements.Add(new XElement(MetadataFieldNames.ParentProductCourseId, section.ParentProductCourseId));
            return elements;
        }

        public static string GetMetadataField(Dictionary<string, XElement> questionElements, string metadataFieldName)
        {
            if (questionElements!= null && questionElements.ContainsKey(metadataFieldName))
            {
                return questionElements[metadataFieldName].Value;
            }
            return string.Empty;
        }
    }
}
