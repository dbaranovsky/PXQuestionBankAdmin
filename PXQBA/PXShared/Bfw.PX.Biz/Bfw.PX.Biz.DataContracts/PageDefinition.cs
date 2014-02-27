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
    public class PageDefinition
    {
        /// <summary>
        /// Name of the Page
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// List of zone items for the page
        /// </summary>
        [DataMember]
        public List<Zone> Zones { get; set; }


        /// <summary>
        /// List of Menu Items for this page.
        /// </summary>
        [DataMember]
        public string MenuId { get; set; }

        /// <summary>
        /// List of class's to create inside the page definition. 
        /// </summary>
        [DataMember]
        public List<string> CustomDivs { get; set; }

        /// <summary>
        /// True if this page definition is editable, False otherwise.
        /// </summary>
        [DataMember]
        public bool IsEditable { get; set; }

        public PageDefinition()
        {
            this.Zones = new List<Zone>();
        }

        /// <summary>
        /// Overriding ToString, will return Name, Zones and Widgets
        /// </summary>
        public override string ToString()
        {
            StringBuilder message = new StringBuilder(string.Empty);
            message.AppendFormat("Page Name: {0}", Name).AppendLine();

            foreach (var zone in Zones)
            {
                message.AppendFormat("Zone ID: {0}", zone.Id).AppendLine();
                foreach (var widget in zone.Widgets)
                {
                    message.AppendFormat("Widget ID: {0}", widget.Id).AppendLine();
                }
            }

            return message.ToString();
        }
    }
}
