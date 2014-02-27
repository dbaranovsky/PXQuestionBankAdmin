using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    [DataContract]
    public class FeaturedItem
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string ImageUrl { get; set; }

        [DataMember]
        public string ItemId { get; set; }
    }
}
