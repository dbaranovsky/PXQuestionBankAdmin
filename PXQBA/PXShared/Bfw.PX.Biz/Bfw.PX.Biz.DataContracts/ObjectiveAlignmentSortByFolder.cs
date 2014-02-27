using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    public class ObjectiveAlignmentSortByFolder
    {
        /// <summary>
        /// e-Portfolio folders
        /// </summary>
        /// <value>
        /// e-Portfolio folders.
        /// </value>
        public String Folder { get; set; }

        /// <summary>
        /// List of Learning Objectives.
        /// </summary>
        /// <value>
        /// List of Learning Objectives.
        /// </value>
        public List<LearningObjective> LearningObjectives { get; set; }

    }
}
