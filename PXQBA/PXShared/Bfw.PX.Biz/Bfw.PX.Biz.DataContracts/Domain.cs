using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Represents a domain item in the system. 
    /// See http://dev.dlap.bfwpub.com/Docs/Command/CreateDomains
    /// </summary>
    public class Domain
    {
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the domain name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the parent id.
        /// </summary>        
        public string ParentId { get; set; }

        /// <summary>
        /// Unique name that identifies the domain. 
        /// This is also the "login prefix" that each user enters with their username when they sign in.
        /// </summary>        
        public string Userspace { get; set; }

        /// <summary>
        /// Field reserved for any data the caller wishes to store. 
        /// We recommend it be a unique reference, such as from an external SIS system.
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// Gets or sets the HTS player URL.
        /// </summary>
        //public string HtsPlayerUrl { get; set; }

        public Dictionary<string, string> CustomQuestionUrls { get; set; }

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

        [NonSerialized]
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Domain"/> class.
        /// </summary>
        public Domain()
        {
        }
    }
}
