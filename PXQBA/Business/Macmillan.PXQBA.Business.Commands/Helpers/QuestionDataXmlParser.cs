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
                                question.DefaultValues.Add(name.Substring(name.IndexOf('/') + 1), values);
                            }
                            if (name.Contains(ElStrings.ProductCourseSection.ToString()) && !name.Contains(DlapNamePart))
                            {
                                var productCourseId = name.Split(new[] { ElStrings.ProductCourseSection.ToString(), "/" }, StringSplitOptions.RemoveEmptyEntries)[0];
                                List<string> values = elem.Elements().Select(v => v.Value).ToList();
                                var productCourse = question.ProductCourseSections.FirstOrDefault(c => c.ProductCourseId == productCourseId);
                                if (productCourse == null)
                                {
                                    productCourse = new ProductCourseSection();
                                    productCourse.ProductCourseId = productCourseId;
                                    question.ProductCourseSections.Add(productCourse);
                                }
                                productCourse.ProductCourseValues.Add(name.Substring(name.IndexOf('/') + 1), values);
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
                    elem => elem.Name.LocalName.Equals("arr") && elem.Attribute("name").Value == sortingField);
            if (sortingElement != null)
            {
                questionSearchResult.SortingField = string.Join(", ", sortingElement.Elements().Select(v => v.Value));
            }
            return questionSearchResult;
        }

        public static Dictionary<string, List<string>> GetDefaultSectionValues(Dictionary<string, XElement> metadataElements)
        {
            var defaultsSection = metadataElements[ElStrings.ProductCourseDefaults.ToString()];
            return defaultsSection.Elements().GroupBy(elem => elem.Name.LocalName).ToDictionary(group => group.Key, group => group.Select(elem => elem.Value).ToList());
        }

        public static List<ProductCourseSection> GetProductCourseSectionValues(Dictionary<string, XElement> metadataElements)
        {
            var productCourseSections = metadataElements.Where(elem => elem.Key.Contains(ElStrings.ProductCourseSection.ToString()));
            var productCourseSectionValues = new List<ProductCourseSection>();
            foreach (var productCourseSection in productCourseSections)
            {
                productCourseSectionValues.Add(new ProductCourseSection
                                               {
                                                   ProductCourseId = productCourseSection.Key.Split(new[] { ElStrings.ProductCourseSection.ToString(), "/" }, StringSplitOptions.RemoveEmptyEntries)[0],
                                                   ProductCourseValues = productCourseSection.Value.Elements().GroupBy(elem => elem.Name.LocalName).ToDictionary(group => group.Key, group => group.Select(elem => elem.Value).ToList())
                                               });
            }
            return productCourseSectionValues;
        }

        public static Dictionary<string, XElement> ToXmlElements(Question question)
        {
            var elements = new Dictionary<string, XElement>();
            var defaultsSection = new XElement(ElStrings.ProductCourseDefaults);
            foreach (var defaultValue in question.DefaultValues)
            {
                foreach (var value in defaultValue.Value)
                {
                    defaultsSection.Add(new XElement(defaultValue.Key, value));
                }
            }
            elements.Add(ElStrings.ProductCourseDefaults.ToString(), defaultsSection);

            foreach (var productCourseSection in question.ProductCourseSections)
            {
                var productCourseSectionName = string.Format("{0}{1}", ElStrings.ProductCourseSection, productCourseSection.ProductCourseId);
                var section = new XElement(productCourseSectionName);
                foreach (var productCourseValue in productCourseSection.ProductCourseValues)
                {
                    foreach (var value in productCourseValue.Value)
                    {
                        section.Add(new XElement(productCourseValue.Key, value));
                    }
                }
                elements.Add(productCourseSectionName, section);
            }
            return elements;
        }
    }
}
