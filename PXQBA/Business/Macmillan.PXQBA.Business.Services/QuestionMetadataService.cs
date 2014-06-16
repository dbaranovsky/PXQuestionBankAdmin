using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;

namespace Macmillan.PXQBA.Business.Services
{
    public class QuestionMetadataService : IQuestionMetadataService
    {
        private readonly IProductCourseOperation productCourseOperation;

        private readonly IKeywordOperation keywordOperation;
        public QuestionMetadataService(IProductCourseOperation productCourseOperation, IKeywordOperation keywordOperation)
        {
            this.productCourseOperation = productCourseOperation;
            this.keywordOperation = keywordOperation;
        }

        public IList<QuestionMetaField> GetAvailableFields(Course course)
        {
            var availableFields = course.FieldDescriptors.Select(Mapper.Map<QuestionMetaField>).ToList();

            AddManuallyEnteredKeywords(course, availableFields);
            
            var customFields = new List<QuestionMetaField>
            {
                new QuestionMetaField()
                {
                    FriendlyName = "Format",
                    Name = MetadataFieldNames.DlapType,
                    TypeDescriptor = new MetaFieldTypeDescriptor
                    {
                        Type = MetadataFieldType.SingleSelect,
                        AvailableChoice = QuestionTypeHelper.GetTypes().Select(t => new AvailableChoiceItem(t.Key, t.DisplayValue)).ToList()
                    }
                },
                new QuestionMetaField
                {
                    FriendlyName = "Status",
                    Name = MetadataFieldNames.QuestionStatus,
                    TypeDescriptor = new MetaFieldTypeDescriptor
                    {
                        Type = MetadataFieldType.SingleSelect,
                        AvailableChoice = new List<AvailableChoiceItem>()
                                          {
                                              new AvailableChoiceItem(((int)QuestionStatus.AvailableToInstructors).ToString(), EnumHelper.GetEnumDescription(QuestionStatus.AvailableToInstructors)),
                                              new AvailableChoiceItem(((int)QuestionStatus.InProgress).ToString(),EnumHelper.GetEnumDescription(QuestionStatus.InProgress)),
                                              new AvailableChoiceItem(((int)QuestionStatus.Deleted).ToString(),EnumHelper.GetEnumDescription(QuestionStatus.Deleted)),
                                          }

                    }
                },

                new QuestionMetaField()
                {
                    FriendlyName = "Textbook title",
                    Name = MetadataFieldNames.ProductCourse,
                    TypeDescriptor = new MetaFieldTypeDescriptor
                    {
                        Type = MetadataFieldType.SingleSelect,
                        AvailableChoice =  productCourseOperation.GetAvailableCourses().Select(pc=> new AvailableChoiceItem(pc.ProductCourseId, pc.Title)).ToList()
                    }
                },
                new QuestionMetaField()
                {
                    FriendlyName = "Target title",
                    Name = MetadataFieldNames.TargetProductCourse,
                    TypeDescriptor = new MetaFieldTypeDescriptor
                    {
                        Type = MetadataFieldType.SingleSelect,
                        AvailableChoice =  productCourseOperation.GetAvailableCourses().Where(c => c.QuestionRepositoryCourseId == course.QuestionRepositoryCourseId).Select(pc=> new AvailableChoiceItem(pc.ProductCourseId, pc.Title)).ToList()
                    }
                },
                new QuestionMetaField
                {
                    FriendlyName = "Flag",
                    Name = MetadataFieldNames.Flag,
                    TypeDescriptor = new MetaFieldTypeDescriptor
                    {
                        Type = MetadataFieldType.MultiSelect,
                        AvailableChoice = EnumHelper.GetEnumValues(typeof(QuestionFlag)).Select(f => new AvailableChoiceItem(f.Key, f.Value)).ToList()
                    }
                },
                new QuestionMetaField
                {
                    FriendlyName = "Contains text",
                    Name = MetadataFieldNames.ContainsText,
                    TypeDescriptor = new MetaFieldTypeDescriptor
                    {
                        Type = MetadataFieldType.Text,
                        AvailableChoice = new List<AvailableChoiceItem>()
                    }
                }

            };

            foreach (var customField in customFields)
            {
                if (availableFields.All(f => f.Name != customField.Name))
                {
                    availableFields.Add(customField);
                }
            }
            
            return availableFields;
        }

        private void AddManuallyEnteredKeywords(Course course, IEnumerable<QuestionMetaField> availableFields)
        {
            foreach (var questionMetaField in availableFields.Where(f => f.TypeDescriptor.Type == MetadataFieldType.Keywords))
            {
                var courseField = course.FieldDescriptors.FirstOrDefault(f => f.Name == questionMetaField.Name);
                if (courseField != null)
                {
                    var predefinedValues = courseField.CourseMetadataFieldValues.Select(v => v.Text);
                    var manuallyEnteredValues = keywordOperation.GetKeywordList(course.ProductCourseId, questionMetaField.Name).Where(v => !predefinedValues.Contains(v));
                    questionMetaField.TypeDescriptor.AvailableChoice.AddRange(manuallyEnteredValues.Select(v => new AvailableChoiceItem(v)));
                }
            } 
        }

        public IList<QuestionMetaField> GetDataForFields(Course course, IEnumerable<string> fieldsNames)
        {
            return GetAvailableFields(course).Where(f => fieldsNames.Contains(f.Name)).ToList();
        }

        public string GetQuestionCardLayout(Course course)
        {
            return course.QuestionCardLayout;
        }
    }
}


