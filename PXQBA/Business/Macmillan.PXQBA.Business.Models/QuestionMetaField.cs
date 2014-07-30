using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Descriptor of metadata field type
    /// </summary>
    public class MetaFieldTypeDescriptor
    {
        public MetaFieldTypeDescriptor()
        {
        }

        public MetaFieldTypeDescriptor(MetadataFieldType type)
        {
            Type = type;
        }

        /// <summary>
        /// Metadata field type
        /// </summary>
        public MetadataFieldType Type { get; set; }

        /// <summary>
        /// List of available values for the field
        /// </summary>
        public List<AvailableChoiceItem> AvailableChoice { get; set; }
    }

    /// <summary>
    /// Question metadata field
    /// </summary>
    public class QuestionMetaField
    {
        /// <summary>
        /// Field name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Field display name
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// Field type descriptor
        /// </summary>
        public MetaFieldTypeDescriptor TypeDescriptor { get; set; }
    }
}
