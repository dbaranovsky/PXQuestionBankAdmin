using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Models
{
    
    public class MenuItemtDisplayOptions
    {

        public List<DisplayOption> DisplayOptions { get; set; }

        public MenuItemtDisplayOptions()
        {
            DisplayOptions = new List<DisplayOption>();
        }
    }

}
