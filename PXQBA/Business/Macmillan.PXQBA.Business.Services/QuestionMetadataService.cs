using System.Collections.Generic;
using System.Linq;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Business.Models.Web;
using Macmillan.PXQBA.Business.Models.Web.Editor;
using Macmillan.PXQBA.Common.Helpers;

namespace Macmillan.PXQBA.Business.Services
{
    public class QuestionMetadataService : IQuestionMetadataService
    {
        public IList<QuestionMetaField> GetAvailableFields()
        {
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
                                                                           "Chapter 10",
                                                                           "Chapter 20"
                                                                       }
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
                                                                           "End of Chapter Questions",
                                                                           "Beginning of Chapter Questions",
                                                                           "Middle of Chapter Questions"
                                                                       }
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
                                                                       }
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
                                                                       }
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
                                                                       }
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

