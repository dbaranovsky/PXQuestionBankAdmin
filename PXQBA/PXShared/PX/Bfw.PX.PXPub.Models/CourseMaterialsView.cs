using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models {

    public class CourseMaterialsView {
        /// <summary>
        /// The set of categories this content item is assigned
        /// </summary>
        /// <value>
        /// The categories.
        /// </value>
        public IEnumerable<TocCategory> Categories { get; set; }
    }
}
