using System.Runtime.Serialization;
using System.Xml.Linq;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.Agilix.DataContracts
{
    [System.Serializable]
    /// <summary>
    /// Information about a domain
    /// </summary>
    [DataContract]
    public class Domain : IDlapEntityParser
    {
        #region Properties

        /// <summary>
        /// Id of the domain.
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Name of the domain.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// AKA login prefix for the domain.
        /// </summary>
        public string Userspace { get; set; }

        /// <summary>
        /// Id from external source that identifies the domain.
        /// </summary>
        public string Reference { get; set; }

        private string _dataString;

        [System.Runtime.Serialization.OnSerializing]
        private void OnSerializing(System.Runtime.Serialization.StreamingContext context)
        {
            if (_data != null)
            {
                _dataString = _data.ToString();
            }
        }

        [System.Runtime.Serialization.OnDeserialized]
        private void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            if (!string.IsNullOrEmpty(_dataString))
            {
                _data = XElement.Parse(_dataString);
            }
        }

        [System.NonSerialized]
        private XElement _data;

        /// <summary>
        /// Custom data stored on the domain.
        /// </summary>
        public XElement Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        #endregion

        #region IDlapEntityParser Members

        /// <summary>
        /// Parses an XElement into internal object state. This allows for objects to be decomposed from
        /// parts of Dlap responses.
        /// </summary>
        /// <param name="element">element that contains the state to parse</param>
        /// <remarks></remarks>
        public void ParseEntity(System.Xml.Linq.XElement element)
        {
            var domainId = element.Attribute("domainid") ?? element.Attribute("id");
            if (domainId != null)
                Id = domainId.Value;

            var domainName = element.Attribute("name");
            if (domainName != null)
            {
                Name = domainName.Value;
            }

            var userSpace = element.Attribute("userspace");
            if (userSpace != null)
            {
                Userspace = userSpace.Value;
            }

            var reference = element.Attribute("reference");
            if (reference != null)
            {
                Reference = reference.Value;
            }

            Data = element.Element("data");
        }

        /// <summary>
        /// Create Domain entity information. Attribute "Name" and "UserSpace" are mandatory for this entity.
        /// </summary>
        /// <returns></returns>
        public XElement ToEntity()
        {
            var element = new XElement("domain");

            element.Add(new XAttribute("name", Name));

            if (!string.IsNullOrEmpty(Id)) 
                element.Add(new XAttribute("domainid", Id));


            element.Add(new XAttribute("userspace", Userspace));

            if (!string.IsNullOrEmpty(Reference))
                element.Add(new XAttribute("reference", Reference));

            if (Data != null)
            {
                element.Add(Data);
            }

            return element;
        }

        #endregion
    }
}