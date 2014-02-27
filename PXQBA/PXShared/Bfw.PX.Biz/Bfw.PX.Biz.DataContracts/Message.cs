using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Represents a Message business object.
    /// </summary>
    [DataContract]
    public class Message
    {
        /// <summary>
        /// User information for the message creator.
        /// </summary>
        [DataMember]
        public UserInfo Author { get; set; }

        /// <summary>
        /// Message body.
        /// </summary>
        [DataMember]
        public string Body { get; set; }

        /// <summary>
        /// Date the message was created.
        /// </summary>
        [DataMember]
        public DateTime Date { get; set; }
    }
}
