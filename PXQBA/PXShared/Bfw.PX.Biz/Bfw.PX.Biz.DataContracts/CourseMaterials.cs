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
    public class CourseMaterials
    {
        /// <summary>
        /// The set of categories this content item is assigned
        /// </summary>
        /// <value>
        /// The categories.
        /// </value>
        [DataMember]
        public IEnumerable<TocCategory> Categories { get; set; }

        /// <summary>
        /// This is so we can load the course materials already in existance to be viewed.
        /// </summary>
        [DataMember]
        public List<ContentItem> ResourceList { get; set; }

        /// <summary>
        /// List of assest items corresponding to the resource items
        /// </summary>
        [DataMember]
        public List<ContentItem> AssestList { get; set; }

        /// <summary>
        /// Indicated wether a save function was called to remove modal.
        /// </summary>
        [DataMember]
        public bool FromSave { get; set; }

        [DataMember]
        public string ItemId { get; set; }

        /// <summary>
        /// default contructor
        /// </summary>        
        public CourseMaterials()
        {
            ResourceList = new List<ContentItem>();
            AssestList = new List<ContentItem>();
        }
    }
}
