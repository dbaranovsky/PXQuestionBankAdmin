using Macmillan.PXQBA.Web.ViewModels.Editor;

namespace Macmillan.PXQBA.Web.ViewModels
{
    /// <summary>
    /// Question field info
    /// </summary>
    public class QuestionFieldViewModel
    {
        /// <summary>
        /// Metadata field name
        /// </summary>
        public string MetadataName { get; set; }

        /// <summary>
        /// Question metadata field friendly name
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// Question metadata field width
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// Indicates if column can be deleted from question list
        /// </summary>
        public bool CanNotDelete { get; set; }

        /// <summary>
        /// Descriptor of the field editor
        /// </summary>
        public FieldEditorDescriptor EditorDescriptor { get; set; }

        /// <summary>
        /// Indicates if inline editing allowed
        /// </summary>
        public bool IsInlineEditingAllowed { get; set; }

        /// <summary>
        /// Indicates if can add values for this field
        /// </summary>
        public bool CanAddValues { get; set; }

        /// <summary>
        /// Indicates if field is multiline
        /// </summary>
        public bool IsMultiline { get; set; }

        /// <summary>
        /// Filter type for this metadata field
        /// </summary>
        public string FilterType { get; set; }

        /// <summary>
        /// Indicates if deselect is allowed
        /// </summary>
        public bool AllowDeselect { get; set; }

        /// <summary>
        /// Indicates if allowed to add column to the list
        /// </summary>
        public bool ColumnAppendAllowed { get; set;}

        /// <summary>
        /// Indicates if filter for this field can be removed
        /// </summary>
        public bool CanCloseOnFilter { get; set; }

        /// <summary>
        /// Indicates if shared value can be updated
        /// </summary>
        public bool CanUpdateSharedValue { get; set; }
    }
}
