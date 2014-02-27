using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    public class ObjectiveAlignmentSortByObjective
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public LearningObjective LearningObjective { get; set; }


        /// <summary>
        /// List of e-Portfolio folders
        /// </summary>
        /// <value>
        /// List of e-Portfolio folders.
        /// </value>
        public List<String> Folders { get; set; }
    }
}
