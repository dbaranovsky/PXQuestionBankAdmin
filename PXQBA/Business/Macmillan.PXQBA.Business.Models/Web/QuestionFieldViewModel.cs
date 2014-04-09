using Macmillan.PXQBA.Business.Models.Web.Editor;

namespace Macmillan.PXQBA.Business.Models.Web
{
    public class QuestionFieldViewModel
    {
               
        public string MetadataName { get; set; }

        public string FriendlyName { get; set; }

        public string Width { get; set; }

        public string LeftIcon { get; set; }

        public bool CanNotDelete { get; set; }

        public FieldEditorDescriptor EditorDescriptor { get; set; }

        public bool IsInlineEditingAllowed { get; set; }
    }
}
