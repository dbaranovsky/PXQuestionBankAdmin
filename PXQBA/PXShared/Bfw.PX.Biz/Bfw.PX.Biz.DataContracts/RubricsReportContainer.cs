// -----------------------------------------------------------------------
// <copyright file="RubricsReportContainer.cs" company="Macmillan">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Bfw.PX.Biz.DataContracts
{
    using System.Collections.Generic;

    [System.Serializable]
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class RubricsReportContainer
    {

        /// <summary>
        /// Gets or sets the rubrics report.
        /// </summary>
        /// <value>
        /// The rubrics report.
        /// </value>
        public List<RubricsReport> RubricsReport { get; set; }

        /// <summary>
        /// Gets or sets the rubric content item.
        /// Just to store the ResourceStream of the rubric
        /// </summary>
        /// <value>
        /// The rubric content item.
        /// </value>
        public ContentItem RubricContentItem { get; set; }

        /// <summary>
        /// Gets or sets the aligned content items.
        /// </summary>
        /// <value>
        /// The aligned content items.
        /// </value>
        public List<ContentItem> AlignedContentItems { get; set; }

        /// <summary>
        /// Gets or sets the enrollments.
        /// </summary>
        /// <value>
        /// The enrollments.
        /// </value>
        public List<Enrollment> Enrollments { get; set; }

        /// <summary>
        /// Gets or sets the type of the report viewer.
        /// </summary>
        /// <value>
        /// The type of the report viewer.
        /// </value>
        public ReportViewerType ReportViewerType { get; set; }

    }
}
