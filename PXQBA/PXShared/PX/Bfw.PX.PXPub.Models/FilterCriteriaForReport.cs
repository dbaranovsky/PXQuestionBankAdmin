
namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// For searching as well as for generating the report.
    /// </summary>
    public class FilterCriteriaForReport
    {
        /// <summary>
        /// Gets or sets the item id.
        /// This should be LO Id or Rubric Id.
        /// </summary>
        /// <value>
        /// The item id.
        /// </value>
        public string ItemId { get; set; }

        public string ItemName { get; set; }

        public string TermId { get; set; }

        public string TermName { get; set; }

        public string InstructorId { get; set; }

        public string TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the type of the report(rubrics/lo).
        /// </summary>
        /// <value>
        /// The type of the report.
        /// </value>
        public ReportType ReportType { get; set; }

        /// <summary>
        /// Gets or sets the type of the convertion.
        /// </summary>
        /// <value>
        /// The type of the convertion.
        /// </value>
        public ConversionType ConversionType { get; set; }
    }

}
