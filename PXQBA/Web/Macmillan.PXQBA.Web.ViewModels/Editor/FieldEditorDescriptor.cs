using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Web.ViewModels.Editor
{
    public class FieldEditorDescriptor
    {
        public FieldEditorDescriptor(MetaFieldTypeDescriptor fieldTypeDescriptor)
        {
            EditorType = fieldTypeDescriptor.Type.ToString().ToLower();
            AvailableChoice = fieldTypeDescriptor.AvailableChoice;
        }

        public string EditorType { get; set; }

        /// <summary>
        /// On the ui display: Value(key)/Label(value)
        /// </summary>
        public Dictionary<string, string> AvailableChoice { get; set; } 
        
    }
}
