using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Bfw.Common.Collections;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    [DataContract]
    public class WidgetDisplayOptions
    {
        [DataMember]
        public List<DisplayOption> DisplayOptions { get; set; }

        public WidgetDisplayOptions()
        {
            DisplayOptions = new List<DisplayOption>();
        }
    }
}
