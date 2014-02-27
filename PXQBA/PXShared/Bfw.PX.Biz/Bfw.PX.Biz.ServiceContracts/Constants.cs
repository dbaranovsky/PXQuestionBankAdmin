using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.ServiceContracts
{
    public class Constants
    {
        /// <summary>
        /// Used to mark off when a search should be done using natural Agilix parents, instead of
        /// bfw metadata toc parents
        /// </summary>
        public const string USE_AGILIX_PARENT = "AGX_PARENT_SEARCH";
    }
}
