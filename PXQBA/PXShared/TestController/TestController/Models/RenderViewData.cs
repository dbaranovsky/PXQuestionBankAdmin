using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bfw.PX.PXPub.Controllers
{
    [Serializable]
    public class RenderViewData
    {
        public string dataType { set; get; }
        public object dataValue { get; set; }
    }
}