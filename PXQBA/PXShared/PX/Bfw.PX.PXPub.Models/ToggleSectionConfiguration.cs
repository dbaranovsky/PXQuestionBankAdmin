using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class ToggleSectionConfiguration
    {
        public string HeadTitle { get; set; }
        public Boolean IsOpenByDefault { get; set; }
        public Boolean IsShowSectionCheckBox { get; set; }
        public Boolean IsSectionCheckBoxChecked { get; set; }
        public Boolean IsHideTopArea { get; set; }
        public Boolean IsItemLocked { get; set; }
    }
}
