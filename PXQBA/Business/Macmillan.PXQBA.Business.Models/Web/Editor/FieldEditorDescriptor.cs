using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Models.Web.Editor
{
    public class FieldEditorDescriptor
    {
        public FieldEditorDescriptor(MetaFieldTypeDescriptor fieldTypeDescriptor)
        {
            EditorType = fieldTypeDescriptor.Type.ToString().ToLower();
            AvailableChoice = new Dictionary<string, string>();
            if (fieldTypeDescriptor.AvailableChoice != null)
            {
                foreach (var choice in fieldTypeDescriptor.AvailableChoice)
                {
                    AvailableChoice.Add(choice, choice);
                }
            }
        }

        public string EditorType { get; set; }

        /// <summary>
        /// On the ui display: Value(key)/Label(value)
        /// </summary>
        public Dictionary<string, string> AvailableChoice { get; set; } 
        
    }
}
