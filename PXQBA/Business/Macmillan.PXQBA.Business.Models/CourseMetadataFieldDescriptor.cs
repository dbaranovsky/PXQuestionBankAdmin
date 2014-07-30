using System.Collections.Generic;
using System.Security;

namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Descriptor of question metadata field that is configured in course xml
    /// </summary>
    public class CourseMetadataFieldDescriptor
    {
        /// <summary>
        /// Field name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indicates if field is filterable
        /// </summary>
        public bool Filterable { get; set; }

        /// <summary>
        /// Indicates if field is displayed in question banks
        /// </summary>
        public bool DisplayInBanks { get; set; }

        /// <summary>
        /// Indicates if field is shown in filter in question banks
        /// </summary>
        public bool ShowFilterInBanks { get; set; }

        /// <summary>
        /// Indicates if search results match this field in question banks
        /// </summary>
        public bool MatchInBanks { get; set; }

        /// <summary>
        /// Indicates if field is displayed in current quiz
        /// </summary>
        public bool DisplayInCurrentQuiz { get; set; }

        /// <summary>
        /// Indicates if field is displayed in instructor quiz
        /// </summary>
        public bool DisplayInInstructorQuiz { get; set; }

        /// <summary>
        /// Indicates if field is displayed in resources
        /// </summary>
        public bool DisplayInResources { get; set; }

        /// <summary>
        /// Indicates if field should be shown in filter in resources
        /// </summary>
        public bool ShowFilterInResources { get; set; }

        /// <summary>
        /// Indicates if field should match search results in resources
        /// </summary>
        public bool MatchInResources { get; set; }

        /// <summary>
        /// Search term of the field
        /// </summary>
        public string Searchterm { get; set; }

        /// <summary>
        /// Friendly name 
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// Metadata field type
        /// </summary>
        public MetadataFieldType Type { get; set; }

        private IEnumerable<CourseMetadataFieldValue> courseMetadataFieldValues { get; set; }

        /// <summary>
        /// List of values for metadata field
        /// </summary>
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
