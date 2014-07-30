namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Question metadata field value configured in course xml
    /// </summary>
    public class CourseMetadataFieldValue
    {
        /// <summary>
        /// Value id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Value text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Value sequence
        /// </summary>
        public int Sequence { get; set; }

    }
}
