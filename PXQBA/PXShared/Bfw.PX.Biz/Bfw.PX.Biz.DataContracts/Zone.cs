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
    public class Zone
    {
        /// The unique Id of the item.
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Id of the resourceentityid the item belongs to.
        /// </summary>
        [DataMember]
        public string CourseID { get; set; }

        /// <summary>
        /// Id of the parent content item, if any exist.
        /// </summary>
        [DataMember]
        public string ParentId { get; set; }

        /// <summary>
        /// The order in which this item should be displayed relative to other items with the same parent.
        /// </summary>
        [DataMember]
        public string Sequence { get; set; }

        /// <summary>
        /// The title of the item.
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public IList<Widget> Widgets { get; set; }

        [DataMember]
        public PageDefinition DefaultPage { get; set; }

        [DataMember]
        public IList<AllowedWidget> AllowedWidgets { get; set; }

        /// <summary>
        /// does this Zone supports hide
        /// </summary>
        [DataMember]
        public bool IsSupportHide { get; set; }

        public Zone()
        {
            Widgets = new List<Widget>();
            AllowedWidgets = new List<AllowedWidget>();
        }
    }
}
