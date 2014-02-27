using System.Runtime.Serialization;


namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class EnrollmentGrade
    {
        /// <summary>
        /// Gets or sets the enrollment id.
        /// </summary>
        /// <value>
        /// The enrollment id.
        /// </value>
        [DataMember]
        public string EnrollmentId { get; set; }

        /// <summary>
        /// Gets or sets the achieved.
        /// </summary>
        /// <value>
        /// The achieved.
        /// </value>
        [DataMember]
        public int Achieved { get; set; }

        /// <summary>
        /// Gets or sets the possible.
        /// </summary>
        /// <value>
        /// The possible.
        /// </value>
        [DataMember]
        public int Possible { get; set; }
    }
}
