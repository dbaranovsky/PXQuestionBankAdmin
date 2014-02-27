
using System.Runtime.Serialization;
namespace Bfw.Agilix.DataContracts
{
    [DataContract]
    public class RubricRule
    {
        /// <summary>
        /// Gets or sets the rule id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the max.
        /// </summary>
        /// <value>
        /// The max.
        /// </value>
        [DataMember]
        public int Max { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        [DataMember]
        public string Body { get; set; }
    }
}
