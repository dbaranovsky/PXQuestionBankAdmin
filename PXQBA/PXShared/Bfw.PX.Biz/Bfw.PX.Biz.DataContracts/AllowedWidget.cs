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
    public class AllowedWidget
    {
        /// <summary>
        /// Template name of the widget
        /// </summary>
        [DataMember]   
        public string widgetType { get; set; }

        /// <summary>
        /// Display name of the widget
        /// </summary>
        [DataMember]
        public string widgetName { get; set; }
    }
}
