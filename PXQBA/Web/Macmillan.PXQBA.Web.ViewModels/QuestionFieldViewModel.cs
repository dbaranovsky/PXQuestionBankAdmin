using Macmillan.PXQBA.Web.ViewModels.Editor;

namespace Macmillan.PXQBA.Web.ViewModels
{
    public class QuestionFieldViewModel
    {
               
        public string MetadataName { get; set; }

        public string FriendlyName { get; set; }

        public string Width { get; set; }

        public bool CanNotDelete { get; set; }

        public FieldEditorDescriptor EditorDescriptor { get; set; }

        public bool IsInlineEditingAllowed { get; set; }

        public bool CanAddValues { get; set; }

        public bool IsMultiline { get; set; }

        public string FilterType { get; set; }

        public bool AllowDeselect { get; set; }
    }
}
