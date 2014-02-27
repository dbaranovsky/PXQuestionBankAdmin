using System.Collections.Generic;
using Bdc = Bfw.PX.Biz.DataContracts;
using bizSC = Bfw.PX.Biz.ServiceContracts;
namespace Bfw.PX.PXPub.Models {

    public class CourseMaterials
    {
        /// <summary>
        /// The set of categories this content item is assigned
        /// </summary>
        /// <value>
        /// The categories.
        /// </value>
        public IEnumerable<TocCategory> Categories { get; set; }

        /// <summary>
        /// This is so we can load the course materials already in existance to be viewed.
        /// </summary>
        public List<Bdc.ContentItem> ResourceList { get; set; }

        /// <summary>
        /// List of assest items corresponding to the resource items
        /// </summary>
        public List<Bdc.ContentItem> AssestList { get; set; }

        /// <summary>
        /// Indicated wether a save function was called to remove modal.
        /// </summary>
        public bool FromSave { get; set; }

        public string ItemId { get; set; }
        /// <summary>
        /// Users Access Level
        /// </summary>
        public bizSC.AccessLevel AccessLevel { get; set; }

        /// <summary>
        /// default contructor
        /// </summary>
        public CourseMaterials()
        {
            ResourceList = new List<Bdc.ContentItem>();
            AssestList = new List<Bdc.ContentItem>();
        }
    }
}
