using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Common.Helpers
{
    public static class CourseDataXmlParser
    {
        public static IEnumerable<CourseMetadataFieldDescriptor> ParseMetaAvailableQuestionData(XElement xmlCourseData)
        {

            var fieldsList = new List<CourseMetadataFieldDescriptor>();
            if (xmlCourseData != null)
            {
                foreach (XElement xmlField in xmlCourseData.Elements("meta-available-question-data").Nodes())
                {
                    var field = new CourseMetadataFieldDescriptor();
                    field.Name = xmlField.Name.LocalName;

                    if (xmlField.Attribute("filterable") != null)
                    {
                        field.Filterable = Convert.ToBoolean(xmlField.Attribute("filterable").Value);
                    }
                    else
                    {
                        field.Filterable = false;
                    }

                    if (xmlField.Attribute("searchterm") != null)
                    {
                        field.Searchterm = xmlField.Attribute("searchterm").Value;
                    }
                    else
                    {
                        field.Searchterm = xmlField.Name.LocalName + ":";
                    }

                    if (xmlField.Attribute("friendlyname") != null)
                    {
                        field.Friendlyname = xmlField.Attribute("friendlyname").Value;
                    }
                    else
                    {
                        field.Friendlyname = char.ToUpper(xmlField.Name.LocalName[0]) + xmlField.Name.LocalName.Substring(1);
                    }
                    
                    if (xmlField.Attribute("type") != null)
                    {
                        field.Type = ParseFieldType(xmlField.Attribute("type").Value);
                    }
                    else
                    {
                        field.Type = MetadataFieldType.Text;
                    }

                    if (xmlField.Attribute("hidden") != null)
                    {
                        field.Filterable = Convert.ToBoolean(xmlField.Attribute("hidden").Value);
                    }
                    else
                    {
                        field.Filterable = false;
                    }

                    field.CourseMetadataFieldValues = ParseCourseMetadataFieldValues(xmlField);

                    fieldsList.Add(field);
                }
            }

            return fieldsList;
        }

        public static string ParseQuestionBankRepositoryCourse(XElement xmlCourseData)
        {
            XElement questionBankRepositoryCourse = xmlCourseData.Element("QuestionBankRepositoryCourse");
            if (questionBankRepositoryCourse != null)
            {
                return questionBankRepositoryCourse.Value;
            }

            return String.Empty;
        }

        private static IEnumerable<CourseMetadataFieldValue> ParseCourseMetadataFieldValues(XElement xmlField)
        {
            var valuesList = new List<CourseMetadataFieldValue>();

            foreach (XElement xmlValue in xmlField.Nodes())
            {
                CourseMetadataFieldValue fieldValue = new CourseMetadataFieldValue();

                if (xmlValue.Attribute("text") != null)
                {
                    fieldValue.Text = xmlValue.Attribute("text").Value;
                }
                else
                {
                    continue;
                }

                if (xmlValue.Attribute("sequence") != null)
                {
                    fieldValue.Sequence = Int32.Parse(xmlValue.Attribute("sequence").Value);
                }
                else
                {
                    continue;
                }

                valuesList.Add(fieldValue); 
            }

            return valuesList.OrderBy(fv=>fv.Sequence);
        }

        private static MetadataFieldType ParseFieldType(string typeValue)
        {
            if (typeValue == "single-select")
            {
                return MetadataFieldType.SingleSelect;
            }

            if (typeValue == "multi-select")
            {
                return MetadataFieldType.MultiSelect;
            }

            return MetadataFieldType.Text;
        }
    }
}
