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
    public class MenuItemCallback
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
        public string LinkType { get; set; }

        /// <summary>
        /// Route name for callback
        /// </summary>
        [DataMember]
        public string RouteName { get; set; }

        /// <summary>
        /// URL name for callback
        /// </summary>
        [DataMember]
        public string Url { get; set; }

        /// <summary>
        /// Target to update on load of callback
        /// </summary>
        [DataMember]
        public string Target { get; set; }

        /// <summary>
        /// In cases where you want to override the default menuitem behavior for a student
        /// </summary>
        [DataMember]
        public string StudentOverride { get; set; }

        /// <summary>
        /// In cases where you want to override the default menuitem behavior for an instructor
        /// </summary>
        [DataMember]
        public string InstructorOverride { get; set; }

        [DataMember]
        public IDictionary<string, PropertyValue> Parameters { get; set; }
    }
}
