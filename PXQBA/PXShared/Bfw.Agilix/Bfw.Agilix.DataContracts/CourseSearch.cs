namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Parameters for a Course search
    /// </summary>
    public class CourseSearch
    {
        /// <summary>
        /// The ID of the course to search for.
        /// </summary>
        public string CourseId { get; set; }

        /// <summary>
        /// A title (or part of a title) used to search for a course.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Maximum number of courses to return. 
        /// Pass 0 to not limit the number of courses to return.
        ///  A large limit or no limit may cause a slow response time.
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Specifies whether to show current courses, deleted courses, or both.
        /// current - Show only current courses, i.e., courses that are not deleted.
        /// deleted - Show only deleted courses.
        /// currentanddeleted - Show current and deleted courses.
        /// </summary>
        public string Show { get; set; }

        /// <summary>
        /// Filters the list of courses to courses that match the value of text in one of several fields defined in Course.
        /// For each listed course one of the following must be true for the value of text:
        /// - The value exactly matches the course's id.
        /// - The value exactly matches the course's reference.
        /// - The value is a close match to the course's title.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Optional list of subtypes to list. Specify one or more of the following:
        /// C - to list courses
        /// S - to list sections
        /// </summary>
        public string Subtype { get; set; }

        /// <summary>
        /// Id of the domain the <see cref="CourseId" /> is in.
        /// </summary>
        public string DomainId { get; set; }

        /// <summary>
        /// XPath syntax based query where / is assumed to be the data element of the course.
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public CourseSearch()
        {
            DomainId = "0";
        }
    }
}
