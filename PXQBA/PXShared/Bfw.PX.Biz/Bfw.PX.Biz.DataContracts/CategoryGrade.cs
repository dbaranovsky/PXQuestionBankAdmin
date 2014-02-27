using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
    [Serializable]
    [DataContract]
    public class CategoryGrade
    {
        /// <summary>
        /// The category id.
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// The category name.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// score the user received, with curving applied.
        /// </summary>
        [DataMember]
        public double Achieved { get; set; }

        /// <summary>
        /// Maximum possible score, with curing applied.
        /// </summary>
        [DataMember]
        public double Possible { get; set; }

        /// <summary>
        /// The score the user achieved in letter form.
        /// </summary>
        [DataMember]
        public string Letter { get; set; }
    }
}
