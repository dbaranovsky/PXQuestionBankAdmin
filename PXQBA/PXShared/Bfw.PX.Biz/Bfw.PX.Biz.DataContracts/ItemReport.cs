// -----------------------------------------------------------------------
// <copyright file="ItemReport.cs" company="Macmillan">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
namespace Bfw.PX.Biz.DataContracts
{
[System.Serializable]

    /// <summary>
    /// item report for rubrics
    /// </summary>
    public class ItemReport
    {
        /// <summary>
        /// Gets or sets the item id.
        /// </summary>
        /// <value>
        /// The item id.
        /// </value>
        public string ItemId { get; set; }

        /// <summary>
        /// Gets or sets the entity id.
        /// </summary>
        /// <value>
        /// The entity id.
        /// </value>
        public string EntityId { get; set; }

        /// <summary>
        /// Gets or sets the rubric grades.
        /// </summary>
        /// <value>
        /// The rubric grades.
        /// </value>
        public List<RubricGrade> RubricGrades { get; set; }
    }
}
