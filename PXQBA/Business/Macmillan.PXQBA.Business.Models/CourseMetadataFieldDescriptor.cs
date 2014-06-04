using System.Collections.Generic;
using System.Security;

namespace Macmillan.PXQBA.Business.Models
{
    public class CourseMetadataFieldDescriptor
    {
        public string Name { get; set; }

        public bool Filterable { get; set; }

        public string Searchterm { get; set; }

        public string FriendlyName { get; set; }

        public MetadataFieldType Type { get; set; }

        public bool Hidden { get; set; }

        public IEnumerable<CourseMetadataFieldValue> CourseMetadataFieldValues { get; set; }

    }
}
