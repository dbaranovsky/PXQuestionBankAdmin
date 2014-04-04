using System.Collections.Generic;
using System.Linq;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Business.Models.Web;
using Macmillan.PXQBA.Business.Models.Web.Editor;

namespace Macmillan.PXQBA.Business.Services
{
    public class QuestionMetadataService : IQuestionMetadataService
    {
        public IList<QuestionFieldDescriptor> GetAvailableFields()
        {
            var fields = new List<QuestionFieldDescriptor>()
                                                   {
                                                       new QuestionFieldDescriptor()
                                                       {
                                                           FriendlyName = "Chapter",
                                                           MetadataName = "chapter",
                                                           Width = "10%",
                                                           CanNotDelete = false,
                                                           EditorDescriptor = new FieldEditorDescriptor(EditorType.Text)
                                                           
                                                       },
                                                       new QuestionFieldDescriptor()
                                                       {
                                                           FriendlyName = "Bank",
                                                           MetadataName = "bank",
                                                           Width = "10%",
                                                           CanNotDelete = false,
                                                           EditorDescriptor = new FieldEditorDescriptor(EditorType.Text)
                                                       },
                                                       new QuestionFieldDescriptor()
                                                       {
                                                           FriendlyName = "Seq",
                                                           MetadataName = "seq",
                                                           Width = "10%",
                                                           CanNotDelete = false,
                                                           EditorDescriptor = new FieldEditorDescriptor(EditorType.Number)
                                                       },
                                                       new QuestionFieldDescriptor()
                                                       {
                                                           FriendlyName = "Title",
                                                           MetadataName = "dlap_title",
                                                           LeftIcon = "glyphicon glyphicon-chevron-right titles-expander",
                                                           Width = "30%",
                                                           CanNotDelete = true,
                                                           EditorDescriptor = new FieldEditorDescriptor(EditorType.Text)
                                                       },
                                                       new QuestionFieldDescriptor()
                                                       {
                                                           FriendlyName = "Format",
                                                           MetadataName = "dlap_q_type",
                                                           Width = "10%",
                                                           CanNotDelete = false,
                                                           EditorDescriptor = new FieldEditorDescriptor(EditorType.None)
                                                       },
                                                       new QuestionFieldDescriptor()
                                                       {
                                                           FriendlyName = "Status",
                                                           MetadataName = "dlap_q_status",
                                                           Width = "10%",
                                                           CanNotDelete = false,
                                                           EditorDescriptor = new FieldEditorDescriptor(EditorType.SingleSelect)
                                                                              {
                                                                                  //On the ui display: Value(key)/Label(value)

                                                                                  //Description from Macmillan.PXQBA.Business.Models.QuestionStatus
                                                                                  AvailableChoice =
                                                                                  {
                                                                                      {QuestionStatus.AvailableToInstructors.ToString(), "Available to instructors"},
                                                                                      {QuestionStatus.InProgress.ToString(), "In progress"},
                                                                                      {QuestionStatus.Deleted.ToString(), "Deleted"}
                                                                                  }
                                                                              }
                                                       }
                                                   };

            return fields;
        }

        public IList<QuestionFieldDescriptor> GetDataForFields(IEnumerable<string> fieldsNames)
        {
            return GetAvailableFields().Where(f => fieldsNames.Contains(f.MetadataName)).ToList();
        }
    }
}

