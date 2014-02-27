using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PXWebAPI.Models
{
    /// <summary>
    /// A learning objective object associated with a course of an item
    /// </summary>
    public class LearningObjective
    {
        /// <summary>
        /// The GUID of the learning objective
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// Id of the group to which the learning objective applies.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Id of the learning objective.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Parent Id of the learning objective.
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// Sequence of the learning objective.
        /// </summary>
        public string Sequence { get; set; }

        /// <summary>
        /// Description of the learning objective.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Title of the learning objective.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Flag to identify if the LO is locked for edit
        /// </summary>
        public bool IsLocked { get; set; }

    }
}
