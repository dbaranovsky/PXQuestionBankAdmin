using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PXWebAPI.Models
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
