using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Models
{
    public class MetaFieldTypeDescriptor
    {
        public MetaFieldTypeDescriptor()
        {
        }

        public MetaFieldTypeDescriptor(MetadataFieldType type)
        {
            Type = type;
        }

        public MetadataFieldType Type { get; set; }
        public List<AvailableChoiceItem> AvailableChoice { get; set; }
    }

    public class QuestionMetaField
    {
        public string Name { get; set; }
        public string FriendlyName { get; set; }
        public MetaFieldTypeDescriptor TypeDescriptor { get; set; }
    }
}
