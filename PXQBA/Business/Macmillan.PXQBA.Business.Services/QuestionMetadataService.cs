using System.Collections.Generic;
using System.Linq;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models.Web;

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
                                                           EditorType = EditorType.Text.ToString().ToLower()
                                                           
                                                       },
                                                       new QuestionFieldDescriptor()
                                                       {
                                                           FriendlyName = "Bank",
                                                           MetadataName = "bank",
                                                           Width = "10%",
                                                           CanNotDelete = false,
                                                           EditorType = EditorType.Text.ToString().ToLower()
                                                       },
                                                       new QuestionFieldDescriptor()
                                                       {
                                                           FriendlyName = "Seq",
                                                           MetadataName = "seq",
                                                           Width = "10%",
                                                           CanNotDelete = false,
                                                           EditorType = EditorType.Number.ToString().ToLower()
                                                       },
                                                       new QuestionFieldDescriptor()
                                                       {
                                                           FriendlyName = "Title",
                                                           MetadataName = "dlap_title",
                                                           LeftIcon = "glyphicon glyphicon-chevron-right titles-expander",
                                                           Width = "30%",
                                                           CanNotDelete = true,
                                                           EditorType = EditorType.Text.ToString().ToLower()
                                                       },
                                                       new QuestionFieldDescriptor()
                                                       {
                                                           FriendlyName = "Format",
                                                           MetadataName = "dlap_q_type",
                                                           Width = "10%",
                                                           CanNotDelete = false,
                                                           EditorType = EditorType.None.ToString().ToLower()
                                                       },
                                                       new QuestionFieldDescriptor()
                                                       {
                                                           FriendlyName = "Status",
                                                           MetadataName = "dlap_q_status",
                                                           Width = "10%",
                                                           CanNotDelete = false,
                                                           EditorType = EditorType.Status.ToString().ToLower()
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

