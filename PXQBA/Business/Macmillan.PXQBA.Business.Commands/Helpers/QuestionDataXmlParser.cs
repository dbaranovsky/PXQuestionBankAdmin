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
    /// <summary>
    /// Helper that is used to convert Question object and its metadata fields to xml and vice versa
    /// </summary>
    public static class QuestionDataXmlParser
    {
        /// <summary>
        /// Parses xml returned from SOLR Search command into search result
        /// </summary>
        /// <param name="resultDoc">Xml returned from SOLR</param>
        /// <param name="sortingField">Field name that is used to sort by</param>
        /// <param name="fields">Field names that were requested</param>
        /// <returns>Search result</returns>
        public static QuestionSearchResult ToSearchResultEntity(XElement resultDoc, string sortingField, string[] fields)
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

            if (fields != null)
            {
                foreach (var field in fields)
                {
                    var element = resultDoc.Elements()
                                           .FirstOrDefault(elem => elem.Attribute("name")
                                           .Value == field);

                    if (element != null)
                    {
                        if (element.HasElements)
                        {
                            string value = string.Join(", ", element.Elements().Select(v => v.Value));
                            questionSearchResult.DynamicFields.Add(field, value);
                        }
                        else
                        {
                            string value = string.Join(", ", element.Value);
                            questionSearchResult.DynamicFields.Add(field, value);
                        }
                    }
                }
            }


            return questionSearchResult;
        }

        /// <summary>
        /// Parses xml returned from SOLR faceted search into faceted search result object
        /// </summary>
        /// <param name="resultDoc">Xml returned by SOLR</param>
        /// <returns>List of faceted search results</returns>
        public static IEnumerable<QuestionFacetedSearchResult> ToFacetedSearchResult(XElement resultDoc)
        {
            var questionSearchResult = new List<QuestionFacetedSearchResult>();
            if (resultDoc != null)
            {
                foreach (var xElement in resultDoc.Elements("int"))
                {
                    var result = new QuestionFacetedSearchResult();
                    if (xElement.Attribute("name") != null && !string.IsNullOrEmpty(xElement.Attribute("name").Value))
                    {
                        result.FacetedFieldValue = xElement.Attribute("name").Value;
                    }
                    var count = 0;
                    if (!string.IsNullOrEmpty(xElement.Value) && int.TryParse(xElement.Value, out count))
                    {
                        result.FacetedCount = count;
                    }
                    questionSearchResult.Add(result);
                }
            }
            return questionSearchResult;
        }

        /// <summary>
        /// Builds default metadata section object from the dictionary of xml elements
        /// </summary>
        /// <param name="metadataElements">Xml elements</param>
        /// <returns>Metadata section object</returns>
        public static QuestionMetadataSection GetDefaultSectionValues(Dictionary<string, XElement> metadataElements)
        {

            if (metadataElements == null || !metadataElements.ContainsKey(ElStrings.ProductCourseDefaults.ToString()))
            {
                return new QuestionMetadataSection();
            }
            var defaultsSection = metadataElements[ElStrings.ProductCourseDefaults.ToString()].Elements();
            return GetSectionValues(defaultsSection);
        }

        /// <summary>
        /// Builds list of metadata section objects for product courses the question belongs to
        /// </summary>
        /// <param name="metadataElements">Dictionary of xml elements</param>
        /// <returns>List of product course metadata section objects</returns>
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
            sectionValues.Flag = GetXElementValue(sectionElements, MetadataFieldNames.Flag);
            sectionValues.DynamicValues = sectionElements.Where(g => !MetadataFieldNames.GetStaticFieldNames().Contains(g.Name.LocalName)).GroupBy(elem => elem.Name.LocalName).ToDictionary(group => group.Key, group => group.Select(elem => elem.Value).ToList());
            return sectionValues;
        }

        private static string GetXElementValue(IEnumerable<XElement> elements, string fieldName)
        {
            var element = elements.FirstOrDefault(g => g.Name.LocalName == fieldName);
            return element != null ? element.Value : string.Empty;
        }

        /// <summary>
        /// Converts question into xml
        /// </summary>
        /// <param name="question">Question to convert</param>
        /// <returns>Dictionary of xml elements with question fields</returns>
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
                    AddMetadataElement(elements, defaultValue.Key, value);
                }
            }
            AddMetadataElement(elements, MetadataFieldNames.DlapTitle, section.Title);
            AddMetadataElement(elements, MetadataFieldNames.ProductCourse, section.ProductCourseId);
            AddMetadataElement(elements, MetadataFieldNames.Bank, section.Bank);
            AddMetadataElement(elements, MetadataFieldNames.Chapter, section.Chapter);
            AddMetadataElement(elements, MetadataFieldNames.Sequence, section.Sequence);
            AddMetadataElement(elements, MetadataFieldNames.ParentProductCourseId, section.ParentProductCourseId);
            AddMetadataElement(elements, MetadataFieldNames.Flag, section.Flag);
            return elements;
        }

        private static void AddMetadataElement(IList<XElement> elements, string name, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                elements.Add(new XElement(name, value, new XAttribute("dlaptype", "exact")));
            }
        }

        /// <summary>
        /// Gets value of metadata field by its name
        /// </summary>
        /// <param name="questionElements">List of question elements</param>
        /// <param name="metadataFieldName">Field name</param>
        /// <returns>Field value</returns>
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
