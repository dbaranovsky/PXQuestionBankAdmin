using System.Xml.Linq;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.Agilix.DataContracts
{
    public class ItemRelatedContent : IDlapEntityParser
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
        /// Threshold
        /// </summary>
        public double Threshold { get; set; }

        /// <summary>
        /// Gets or sets the sequence.
        /// </summary>
        /// <value>
        /// The sequence.
        /// </value>
        public string Sequence { get; set; }


        /// <summary>
        /// Parse and objective XML element and populate this object's state.
        /// </summary>
        /// <param name="root"></param>
        public void ParseEntity(XElement root)
        {
            if (root.Name != ElStrings.Item.LocalName)
            {
                throw new DlapEntityFormatException(string.Format("Expected root element to be 'item', but got '{0}' instead", root.Name));
            }

            var guid = root.Attribute(ElStrings.Id);
            if (guid == null)
            {
                throw new DlapEntityFormatException("Required attribute 'guid' is missing from 'item' element");
            }
            else
            {
                Id = guid.Value;
            }
            var parentId = root.Attribute(ElStrings.ParentId);

            if (parentId != null)
            {
                ParentId = parentId.Value;
            }

            var type = root.Attribute(ElStrings.type);

            if (type != null)
            {
                Type = type.Value;
            }

            var threshold = root.Attribute(ElStrings.threshold);
            if (threshold != null)
            {
                double temp;
                if (!double.TryParse(threshold.Value, out temp))
                {
                    Threshold = 0.0;
                }
                else
                {
                    Threshold = temp;
                }
            }

            var sequence = root.Attribute(ElStrings.sequence);
            if (sequence != null)
            {
                Sequence = sequence.Value;
            }
        }
    }
}
