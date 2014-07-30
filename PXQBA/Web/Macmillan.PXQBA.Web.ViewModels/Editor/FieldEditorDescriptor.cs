using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Web.ViewModels.Editor
{
    /// <summary>
    /// Contains info regarding the type and available values for editor control
    /// </summary>
    public class FieldEditorDescriptor
    {
        public FieldEditorDescriptor(MetaFieldTypeDescriptor fieldTypeDescriptor)
        {
            EditorType = GetEditorType(fieldTypeDescriptor.Type).ToString().ToLower();
            AvailableChoice = fieldTypeDescriptor.AvailableChoice;
        }

        /// <summary>
        /// Type of the editor control
        /// </summary>
        public string EditorType { get; set; }

        /// <summary>
        /// Available options for editor control
        /// </summary>
        public List<AvailableChoiceItem> AvailableChoice { get; set; }

        private static EditorType GetEditorType(MetadataFieldType type)
        {
            switch (type)
            {
                case MetadataFieldType.ItemLink:
                    return Editor.EditorType.ItemLink;
                case MetadataFieldType.Keywords:
                    return Editor.EditorType.Keywords;
                case MetadataFieldType.MultiSelect:
                    return Editor.EditorType.MultiSelect;
                case MetadataFieldType.MultilineText:
                    return Editor.EditorType.MultilineText;
                case MetadataFieldType.SingleSelect:
                    return Editor.EditorType.SingleSelect;
                case MetadataFieldType.Text:
                    return Editor.EditorType.Text;
            }
            return Editor.EditorType.Text;
        }
    }
}
