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

        public QuestionMetadataService(IProductCourseOperation productCourseOperation)
        {
            this.productCourseOperation = productCourseOperation;
        }

        public IList<QuestionMetaField> GetAvailableFields(Course course)
        {
            var availableFields = course.FieldDescriptors.Select(Mapper.Map<QuestionMetaField>).ToList();
 
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
                    FriendlyName = "Learning Objective",
                    Name = MetadataFieldNames.LearningObjectives,
                    TypeDescriptor = new MetaFieldTypeDescriptor
                    {
                        Type = MetadataFieldType.MultiSelect,
                        AvailableChoice =  course.LearningObjectives.Select(l => new AvailableChoiceItem(l.Guid, l.Description)).ToList()
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
                new QuestionMetaField
                {
                    FriendlyName = "Flag",
                    Name = MetadataFieldNames.Flag,
                    TypeDescriptor = new MetaFieldTypeDescriptor
                    {
                        Type = MetadataFieldType.MultiSelect,
                        AvailableChoice = EnumHelper.GetEnumValues(typeof(QuestionFlag)).Select(f => new AvailableChoiceItem(f.Key, f.Value)).ToList()
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


