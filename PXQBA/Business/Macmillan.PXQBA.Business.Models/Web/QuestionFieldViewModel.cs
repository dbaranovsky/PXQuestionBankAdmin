using Macmillan.PXQBA.Business.Models.Web.Editor;

namespace Macmillan.PXQBA.Business.Models.Web
{
    public class QuestionFieldViewModel
    {
        static public QuestionFieldViewModel Convert(QuestionMetaField metaField)
        {
            var model = new QuestionFieldViewModel();

            model.MetadataName = metaField.Name;
            model.FriendlyName = metaField.FriendlyName;
            model.Width = "10%";
            model.CanNotDelete = false;
            model.IsInlineEditingAllowed = false;
            model.EditorDescriptor = new FieldEditorDescriptor(metaField.TypeDescriptor);

            // \todo Move custom field-level settings to configuration
            if (metaField.Name == MetadataFieldNames.DlapTitle)
            {
                model.Width = "30%";
                model.CanNotDelete = true;
                model.LeftIcon = "glyphicon glyphicon-chevron-right titles-expander";
            }

            if (metaField.Name == MetadataFieldNames.DlapStatus)
            {
                model.IsInlineEditingAllowed = true;
            }

            if (metaField.Name == MetadataFieldNames.Sequence)
            {
                model.IsInlineEditingAllowed = true;
            }

            return model;
        }
        
        public string MetadataName { get; set; }

        public string FriendlyName { get; set; }

        public string Width { get; set; }

        public string LeftIcon { get; set; }

        public bool CanNotDelete { get; set; }

        public FieldEditorDescriptor EditorDescriptor { get; set; }

        public bool IsInlineEditingAllowed { get; set; }
    }
}
