// -----------------------------------------------------------------------
// <copyright file="RubricReport.cs" company="Macmillan">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
namespace Bfw.PX.Biz.DataContracts
{
[System.Serializable]
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class RubricsReport
    {
        /// <summary>
        /// Gets or sets the item reports.
        /// </summary>
        /// <value>
        /// The item reports.
        /// </value>
        //public List<ItemReport> ItemReports { get; set; }

        public string EntityId { get; set; }

        public string CourseName { get; set; }

        public string InstructorName { get; set; }

        public string InstructorId { get; set; }

        public string TemplateId { get; set; }

        public string TemplateName { get; set; }

        public List<StudentRubricReport> StudentReports { get; set; }
    }
}
