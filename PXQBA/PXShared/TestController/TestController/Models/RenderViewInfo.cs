using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bfw.PX.PXPub.Controllers
{
    [Serializable]
    public class RenderViewInfo
    {
        public string viewPath { get; set; }
        public string viewModel { get; set; }
        public string viewModelType { get; set; }
        public string viewData { get; set; }
        public bool renderPartials { get; set; }
    }
}