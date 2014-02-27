using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using Bfw.Common.Collections;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    [DataContract]
    public class WidgetCallback
    {
        /// <summary>
        /// Name of the callback
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Controller for callback
        /// </summary>
        [DataMember]
        public string Controller { get; set; }

        /// <summary>
        /// Action for callback
        /// </summary>
        [DataMember]
        public string Action { get; set; }

        /// <summary>
        /// FNE value of callback
        /// </summary>
        [DataMember]
        public bool IsFNE { get; set; }

        /// <summary>
        /// Load using ASync methodologies
        /// </summary>
        [DataMember]
        public bool IsASync { get; set; }
    }
}
