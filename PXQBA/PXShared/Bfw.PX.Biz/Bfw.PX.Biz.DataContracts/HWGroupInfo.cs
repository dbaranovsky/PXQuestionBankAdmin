using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Represents Part of Group Settings for Homeworks.
    /// </summary>
    [DataContract]
    public class HomeworkGroupInfo
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Flag for displaying questions in scrambled order.
        /// </summary>
        /// <value>
        /// The scrambled flag.
        /// </value>       
        [DataMember]
        public string Scrambled { get; set; }

        /// <summary>
        /// Flag for whether to display question hints.
        /// </summary>
        /// <value>
        /// The hints value.
        /// </value>
        [DataMember]
        public string Hints { get; set; }
    }
}
