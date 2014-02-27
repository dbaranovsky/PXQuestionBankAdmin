using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    public class ProxyConfig
    {
        /// <summary>
        /// Structure holding information about usage of the item on the proxy page
        /// </summary>
        public ProxyConfig()
        {
            AllowComments = null;
        }

       

        /// <summary>
        /// The flag indicating usage of commenting 
        /// </summary>
        public bool? AllowComments { get; set; }
    }
}
