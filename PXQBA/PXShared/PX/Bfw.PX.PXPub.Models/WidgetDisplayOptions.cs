using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Models
{
    
    public class WidgetDisplayOptions
    {

        public List<DisplayOption> DisplayOptions { get; set; }

        public WidgetDisplayOptions()
        {
            DisplayOptions = new List<DisplayOption>();
        }
    }

}
