
namespace Bfw.PX.Biz.DataContracts
{
[System.Serializable]
    public class RelatedContent
    {
        /// <summary>
        /// The parent Id of the related content
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// The Id of the related content
        /// </summary>
        public string Id { get; set; }


        /// <summary>
        /// Gets or sets the type, ie: topic, content
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; set; }

        /// <summary>
        /// Gets of sets the threshold
        /// </summary>
        public double Threshold { get; set; }

        /// <summary>
        /// Gets or sets the sequence.
        /// </summary>
        /// <value>
        /// The sequence.
        /// </value>
        public string Sequence { get; set; }
    }
}
