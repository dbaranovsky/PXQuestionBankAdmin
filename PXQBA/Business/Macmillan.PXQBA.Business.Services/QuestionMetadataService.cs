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

                new QuestionMetaField
                {
                    FriendlyName = "Question title",
                    Name = MetadataFieldNames.DlapTitle,
                    TypeDescriptor = new MetaFieldTypeDescriptor(MetadataFieldType.Text)
                },

                new QuestionMetaField()
                {
                    FriendlyName = "Format",
                    Name = MetadataFieldNames.DlapType,
                    TypeDescriptor = new MetaFieldTypeDescriptor
                    {
                        Type = MetadataFieldType.SingleSelect,
                        AvailableChoice = ConfigurationHelper.GetQuestionTypes().Select(d=>d.Value).ToDictionary(v=>v)
                    }
                },
                new QuestionMetaField
                {
                    FriendlyName = "Status",
                    Name = MetadataFieldNames.DlapStatus,
                    TypeDescriptor = new MetaFieldTypeDescriptor
                    {
                        Type = MetadataFieldType.SingleSelect,
                        AvailableChoice = new List<string>
                        {
                            EnumHelper.GetEnumDescription(QuestionStatus.AvailableToInstructors),
                            EnumHelper.GetEnumDescription(QuestionStatus.InProgress),
                            EnumHelper.GetEnumDescription(QuestionStatus.Deleted),
                        }.ToDictionary(it => it)
                    }
                },
                new QuestionMetaField()
                {
                    FriendlyName = "Keywords",
                    Name = MetadataFieldNames.Keywords,
                    TypeDescriptor = new MetaFieldTypeDescriptor
                    {
                        Type = MetadataFieldType.MultiSelect,
                        AvailableChoice = new List<string>
                        {
                            "Keyword 1",
                            "Keyword 2",
                            "Keyword 3"
                        }.ToDictionary(it => it)
                    }

                },
                new QuestionMetaField()
                {
                    FriendlyName = "Learning Objective",
                    Name = MetadataFieldNames.LearningObjectives,
                    TypeDescriptor = new MetaFieldTypeDescriptor
                    {
                        Type = MetadataFieldType.MultiSelect,
                        AvailableChoice =
                            course.LearningObjectives.ToDictionary(lo => lo.Guid, lo => lo.Description)
                    }

                },

                new QuestionMetaField()
                {
                    FriendlyName = "Textbook title",
                    Name = MetadataFieldNames.ProductCourse,
                    TypeDescriptor = new MetaFieldTypeDescriptor
                    {
                        Type = MetadataFieldType.SingleSelect,
                        AvailableChoice = productCourseOperation.GetAvailableCourses().ToDictionary(pc => pc.ProductCourseId, pc => pc.Title)
                    }
                },

                //new QuestionMetaField
                //{
                //    FriendlyName = "Difficulty",
                //    Name = MetadataFieldNames.Difficulty,
                //    TypeDescriptor = new MetaFieldTypeDescriptor
                //    {
                //        Type = MetadataFieldType.SingleSelect,
                //        AvailableChoice = new List<string>
                //        {
                //            "Easy",
                //            "Medium",
                //            "Hard",
                //        }.ToDictionary(it => it)
                //    }
                //},
                //new QuestionMetaField()
                //{
                //    FriendlyName = "Suggested Use",
                //    Name = MetadataFieldNames.SuggestedUse,
                //    TypeDescriptor = new MetaFieldTypeDescriptor
                //    {
                //        Type = MetadataFieldType.MultiSelect,
                //        AvailableChoice = new List<string>
                //        {
                //            "Pre-class",
                //            "In-class",
                //            "Post-class",
                //            "Exam"
                //        }.ToDictionary(it => it)
                //    }

                //},
                //new QuestionMetaField()
                //{
                //    FriendlyName = "Guidance",
                //    Name = MetadataFieldNames.Guidance,
                //    TypeDescriptor = new MetaFieldTypeDescriptor(MetadataFieldType.Text)
                //},
                //new QuestionMetaField()
                //{
                //    FriendlyName = "Chapter",
                //    Name = MetadataFieldNames.Chapter,
                //    TypeDescriptor = new MetaFieldTypeDescriptor
                //    {
                //        Type = MetaFieldType.SingleSelect,
                //        AvailableChoice = course.Chapters.Select(ch => ch.Title).ToDictionary(it => it)
                //    }

                //},
                //new QuestionMetaField()
                //{
                //    FriendlyName = "Question Bank",
                //    Name = MetadataFieldNames.Bank,
                //    TypeDescriptor = new MetaFieldTypeDescriptor
                //    {
                //        Type = MetadataFieldType.SingleSelect,
                //         AvailableChoice = course.Banks.ToDictionary(it => it)
                //    }
                //},
                //new QuestionMetaField
                //{
                //    FriendlyName = "Sequence",
                //    Name = MetadataFieldNames.Sequence,
                //    TypeDescriptor = new MetaFieldTypeDescriptor(MetadataFieldType.Text)
                //},
            };

            availableFields.AddRange(customFields);

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


