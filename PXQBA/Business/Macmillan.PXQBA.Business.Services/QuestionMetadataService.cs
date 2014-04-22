using System.Collections.Generic;
using System.Linq;
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

        public IList<QuestionMetaField> GetAvailableFields()
        {
            var productCourse = productCourseOperation.GetCurrentProductCourse();
            var fields = new List<QuestionMetaField>
                                                   {
                                                       new QuestionMetaField()
                                                       {
                                                           FriendlyName = "Chapter",
                                                           Name = MetadataFieldNames.Chapter,
                                                           TypeDescriptor = new MetaFieldTypeDescriptor
                                                               {
                                                                   Type = MetaFieldType.SingleSelect,
                                                                   AvailableChoice = new List<string>
                                                                       {
                                                                           "Chapter 1",
                                                                           "Chapter 2",
                                                                           "Chapter 3"
                                                                       }.ToDictionary(it => it)
                                                               }
                                                           
                                                       },
                                                       new QuestionMetaField()
                                                       {
                                                           FriendlyName = "Bank",
                                                           Name = MetadataFieldNames.Bank,
                                                           TypeDescriptor = new MetaFieldTypeDescriptor
                                                               {
                                                                   Type = MetaFieldType.SingleSelect,
                                                                   AvailableChoice = new List<string>
                                                                       {
                                                                           "End of Chapter",
                                                                           "Beginning of Chapter Questions",
                                                                           "Middle of Chapter Questions"
                                                                       }.ToDictionary(it => it)
                                                               }      
                                                       },
                                                       new QuestionMetaField
                                                       {
                                                           FriendlyName = "Seq",
                                                           Name = MetadataFieldNames.Sequence,
                                                           TypeDescriptor = new MetaFieldTypeDescriptor(MetaFieldType.Text)
                                                       },
                                                       new QuestionMetaField
                                                       {
                                                           FriendlyName = "Title",
                                                           Name = MetadataFieldNames.DlapTitle,
                                                           TypeDescriptor = new MetaFieldTypeDescriptor(MetaFieldType.Text)
                                                       },
                                                       new QuestionMetaField()
                                                       {
                                                           FriendlyName = "Format",
                                                           Name = MetadataFieldNames.DlapType,
                                                           TypeDescriptor = new MetaFieldTypeDescriptor
                                                               {
                                                                   Type = MetaFieldType.SingleSelect,
                                                                   AvailableChoice = new List<string>
                                                                       {
                                                                           EnumHelper.GetEnumDescription(QuestionType.MultipleAnswer),
                                                                           EnumHelper.GetEnumDescription(QuestionType.MultipleChoice),
                                                                           EnumHelper.GetEnumDescription(QuestionType.Matching),
                                                                           EnumHelper.GetEnumDescription(QuestionType.ShortAnswer),
                                                                           EnumHelper.GetEnumDescription(QuestionType.Essay),
                                                                           EnumHelper.GetEnumDescription(QuestionType.GraphExcepcise),
                                                                       }.ToDictionary(it => it)
                                                               }      
                                                       },
                                                       new QuestionMetaField
                                                       {
                                                           FriendlyName = "Status",
                                                           Name = MetadataFieldNames.DlapStatus,
                                                           TypeDescriptor = new MetaFieldTypeDescriptor
                                                               {
                                                                   Type = MetaFieldType.SingleSelect,
                                                                   AvailableChoice = new List<string>
                                                                       {
                                                                           EnumHelper.GetEnumDescription(QuestionStatus.AvailableToInstructors),
                                                                           EnumHelper.GetEnumDescription(QuestionStatus.InProgress),
                                                                           EnumHelper.GetEnumDescription(QuestionStatus.Deleted),
                                                                       }.ToDictionary(it => it)
                                                               }      
                                                       },
                                                       new QuestionMetaField
                                                       {
                                                           FriendlyName = "Difficulty",
                                                           Name = MetadataFieldNames.Difficulty,
                                                           TypeDescriptor = new MetaFieldTypeDescriptor
                                                               {
                                                                   Type = MetaFieldType.SingleSelect,
                                                                   AvailableChoice = new List<string>
                                                                       {
                                                                           "Easy",
                                                                           "Medium",
                                                                           "Hard",
                                                                       }.ToDictionary(it => it)
                                                               }      
                                                       },
                                                       new QuestionMetaField()
                                                       {
                                                           FriendlyName = "Keywords",
                                                           Name = MetadataFieldNames.Keywords,
                                                           TypeDescriptor = new MetaFieldTypeDescriptor
                                                               {
                                                                   Type = MetaFieldType.MultiSelect,
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
                                                           FriendlyName = "Suggested Use",
                                                           Name = MetadataFieldNames.SuggestedUse,
                                                           TypeDescriptor = new MetaFieldTypeDescriptor
                                                               {
                                                                   Type = MetaFieldType.MultiSelect,
                                                                   AvailableChoice = new List<string>
                                                                       {
                                                                           "Pre-class",
                                                                           "In-class",
                                                                           "Post-class",
                                                                           "Exam"
                                                                       }.ToDictionary(it => it)
                                                               }
                                                           
                                                       },
                                                       new QuestionMetaField()
                                                       {
                                                           FriendlyName = "Guidance",
                                                           Name = MetadataFieldNames.Guidance,
                                                           TypeDescriptor = new MetaFieldTypeDescriptor(MetaFieldType.Text)
                                                       },

                                                        new QuestionMetaField()
                                                       {
                                                           FriendlyName = "Learning Objective",
                                                           Name = MetadataFieldNames.LearningObjectives,
                                                           TypeDescriptor = new MetaFieldTypeDescriptor
                                                               {
                                                                   Type = MetaFieldType.MultiSelect,
                                                                   AvailableChoice = productCourse.LearningObjectives.ToDictionary(lo => lo.Guid, lo => lo.Description)
                                                               }
                                                           
                                                       },
                                                   };

            return fields;
        }

        public IList<QuestionMetaField> GetDataForFields(IEnumerable<string> fieldsNames)
        {
            return GetAvailableFields().Where(f => fieldsNames.Contains(f.Name)).ToList();
        }
    }
}

