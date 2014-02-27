using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    [DataContract]
    public class WidgetInputHelper
    {
        /// <summary>
        /// Name of the current input helper
        /// This also the the variable name that has to be posted
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// selector of the element to retrieve the value from
        /// </summary>
        [DataMember]
        public string Selector { get; set; }

        /// <summary>
        /// Default value to be used if the required element is not found 
        /// </summary>
        [DataMember]
        public string DefaultValue { get; set; }

        /// <summary>
        /// A flag which indicates whether to use the default value
        /// </summary>
        [DataMember]
        public bool UseDefaultValue { get; set; }
    }
}
