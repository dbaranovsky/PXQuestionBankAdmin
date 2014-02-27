using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// A learning objective object associated with a course of an item
    /// </summary>
    public class ItemLearningObjective
    {
        /// <summary>
        /// The GUID of the learning objective
        /// </summary>
        public string Guid { get; set; }
    }
}
