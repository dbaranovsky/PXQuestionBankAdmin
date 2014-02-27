// -----------------------------------------------------------------------
// <copyright file="FilterCriteriaForReport.cs" company="Macmillan">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Bfw.PX.Biz.DataContracts
{
[System.Serializable]

    /// <summary>
    /// TODO: Update summary.
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

        public string TermId { get; set; }

        public string InstructorId { get; set; }

        public string TemplateId { get; set; }
    }
}
