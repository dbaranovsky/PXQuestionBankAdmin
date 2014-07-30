using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Section in question xml where metadata fields are stored
    /// </summary>
    public class QuestionMetadataSection
    {
        /// <summary>
        /// Product course id section belongs to. It's empty if default section
        /// </summary>
        public string ProductCourseId { get; set; }

        /// <summary>
        /// Question title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Question bank
        /// </summary>
        public string Bank { get; set; }

        /// <summary>
        /// Question chapter
        /// </summary>
        public string Chapter { get; set; }

        /// <summary>
        /// Question sequence
        /// </summary>
        public string Sequence { get; set; }

        private Dictionary<string, List<string>> dynamicValues;

        /// <summary>
        /// List of dynamic metadata fields 
        /// </summary>
        public Dictionary<string, List<string>> DynamicValues
        {
            get
            {
                if (dynamicValues == null)
                {
                    dynamicValues = new Dictionary<string, List<string>>();
                }
                return dynamicValues;
            }
            set
            {
                dynamicValues = value;
            }
        }

        /// <summary>
        /// Parent product course id. It is set if question was initially created not in the course this section belongs to
        /// </summary>
        public string ParentProductCourseId { get; set; }

        /// <summary>
        /// If question is flagged
        /// </summary>
        public string Flag { get; set; }
    }
}