using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    public class Ebook : ContentItem
    {

        public Ebook()
        {
            Type = "eBook";
        }

        /// <summary>
        /// eBook subtitle
        /// </summary>
        public string Subtitle { get; set; }

        /// <summary>
        /// Author name
        /// </summary>
        public string Authors { get; set; }

        /// <summary>
        /// Item id of the cover art
        /// </summary>
        public string CoverImage { get; set; }

        /// <summary>
        /// Root id
        /// </summary>
        public string RootId { get; set; }

        /// <summary>
        /// Catagory id
        /// </summary>
        public string CatagoryId { get; set; }
    }
}
