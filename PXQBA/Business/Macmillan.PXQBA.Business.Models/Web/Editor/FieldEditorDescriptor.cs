using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Models.Web.Editor
{
    public class FieldEditorDescriptor
    {
        public FieldEditorDescriptor(EditorType editorType)
        {
            EditorType = editorType.ToString().ToLower();
            AvailableChoice = new Dictionary<string, string>();
        }

        public string EditorType { get; set; }

        /// <summary>
        /// On the ui display: Value(key)/Label(value)
        /// </summary>
        public Dictionary<string, string> AvailableChoice { get; set; } 
        
    }
}
