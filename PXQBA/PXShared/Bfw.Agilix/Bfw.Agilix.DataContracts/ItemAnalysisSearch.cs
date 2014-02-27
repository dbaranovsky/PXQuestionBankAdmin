// -----------------------------------------------------------------------
// <copyright file="ItemAnalysisSearch.cs" company="Macmillan">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Bfw.Agilix.DataContracts
{

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ItemAnalysisSearch
    {
        /// <summary>
        /// The ID of the course to search for
        /// </summary>
        public string EntityId { get; set; }


        /// <summary>
        /// Gets or sets the item id.
        /// </summary>
        /// <value>
        /// The item id.
        /// </value>
        public string ItemId { get; set; }

        /// <summary>
        /// Gets or sets the group id.
        /// </summary>
        /// <value>
        /// The group id.
        /// </value>
        public string GroupId { get; set; }

        /// <summary>
        /// Filters out enrollments by enrollmentid
        /// </summary>
        public string EnrollmentId { get; set; }

        /// <summary>
        /// If set to true, will return all enrollments, even the ones that are invalid.
        /// </summary>
        public bool AllStatus { get; set; }


        /// <summary>
        /// Gets or sets the summary.
        /// </summary>
        /// <value>
        /// The summary.
        /// </value>
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ItemAnalysisSearch"/> is verbose.
        /// </summary>
        /// <value>
        ///   <c>true</c> if verbose; otherwise, <c>false</c>.
        /// </value>
        public bool Verbose { get; set; }


        /// <summary>
        /// Gets or sets the set id.
        /// </summary>
        /// <value>
        /// The set id.
        /// </value>
        public string SetId { get; set; }
    }
}
