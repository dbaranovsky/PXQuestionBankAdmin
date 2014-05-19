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
                        AvailableChoice = ConfigurationHelper.GetQuestionTypes().Select(d=>new AvailableChoiceItem(d.Value)).ToList()
                    }
                },
                new QuestionMetaField
                {
                    FriendlyName = "Status",
                    Name = MetadataFieldNames.DlapStatus,
                    TypeDescriptor = new MetaFieldTypeDescriptor
                    {
                        Type = MetadataFieldType.SingleSelect,
                        AvailableChoice = new List<AvailableChoiceItem>()
                                          {
                                              new AvailableChoiceItem(EnumHelper.GetEnumDescription(QuestionStatus.AvailableToInstructors)),
                                              new AvailableChoiceItem(EnumHelper.GetEnumDescription(QuestionStatus.InProgress)),
                                              new AvailableChoiceItem(EnumHelper.GetEnumDescription(QuestionStatus.Deleted)),
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
                    Name = MetadataFieldNames.ProductCourseName,
                    TypeDescriptor = new MetaFieldTypeDescriptor
                    {
                        Type = MetadataFieldType.SingleSelect,
                        AvailableChoice =  productCourseOperation.GetAvailableCourses().Select(pc=> new AvailableChoiceItem(pc.ProductCourseId, pc.Title)).ToList()
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


