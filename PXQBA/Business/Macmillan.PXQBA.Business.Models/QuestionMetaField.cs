using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Models
{
    public enum MetaFieldType
    {
        Text = 0,
        SingleSelect,
        MultiSelect,
    }
    
    public class MetaFieldTypeDescriptor
    {
        public MetaFieldTypeDescriptor()
        {
        }

        public MetaFieldTypeDescriptor(MetaFieldType type)
        {
            Type = type;
        }
        
        public MetaFieldType Type { get; set; }
        public Dictionary<string, string> AvailableChoice { get; set; }
    }

    public class QuestionMetaField
    {
        public string Name { get; set; }
        public string FriendlyName { get; set; }
        public MetaFieldTypeDescriptor TypeDescriptor { get; set; }
    }
}
