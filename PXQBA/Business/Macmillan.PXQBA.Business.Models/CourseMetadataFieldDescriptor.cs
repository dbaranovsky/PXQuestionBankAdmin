using System.Collections.Generic;
using System.Security;

namespace Macmillan.PXQBA.Business.Models
{
    public class CourseMetadataFieldDescriptor
    {
        public string Name { get; set; }

        public bool Filterable { get; set; }

        public bool DisplayInBanks { get; set; }

        public bool ShowFilterInBanks { get; set; }

        public bool MatchInBanks { get; set; }

        public bool DisplayInCurrentQuiz { get; set; }

        public bool DisplayInInstructorQuiz { get; set; }

        public bool DisplayInResources { get; set; }

        public bool ShowFilterInResources { get; set; }

        public bool MatchInResources { get; set; }

        public string Searchterm { get; set; }

        public string FriendlyName { get; set; }

        public MetadataFieldType Type { get; set; }

        private IEnumerable<CourseMetadataFieldValue> courseMetadataFieldValues { get; set; }

        public IEnumerable<CourseMetadataFieldValue> CourseMetadataFieldValues
        {
            get
            {
                if (courseMetadataFieldValues == null)
                {
                    courseMetadataFieldValues = new List<CourseMetadataFieldValue>();
                }
                return courseMetadataFieldValues;
            }
            set
            {
                courseMetadataFieldValues = value;
            }
        }

    }
}
