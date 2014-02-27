using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
    public class ItemLink
    {
        /// <summary>
        /// ID of the course that contains Id. If Id is empty, this is the derivative course ID that links to the entityid specified in the request.
        /// </summary>
        public string EntityId { get; set; }

        /// <summary>
        /// ID of the item that links to itemid; or empty string ("") if you did not specify an itemid.
        /// </summary>
        public string Id { get; set; }
    }
}
