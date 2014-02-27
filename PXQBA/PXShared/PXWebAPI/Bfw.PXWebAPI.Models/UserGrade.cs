using System;
using System.Runtime.Serialization;

namespace Bfw.PXWebAPI.Models
{
    [Serializable]
    [DataContract]
    public class UserGrade
    {
        /// <summary>
        /// User Id
        /// </summary>
        [DataMember]
        public string UserId { get; set; }

        /// <summary>
        /// score the user received, with curving applied.
        /// </summary>
        [DataMember]
        public double Score { get; set; }

        /// <summary>
        /// Date the assignment was scored.
        /// </summary>
        [DataMember]
        public DateTime? ScoredDate { get; set; }

        /// <summary>
        /// Status of the grade
        /// </summary>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// Duration in seconds
        /// </summary>
        [DataMember]
        public int Duration { get; set; }
    }
}
