using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Web.Mvc;

namespace Bfw.PXAP.Models
{
    [Serializable]
    [XmlRoot("Progress")]
    public class ProgressModel
    {
        [XmlAttribute("ProcessId")]
        public Int64 ProcessId { get; set; }

        [XmlAttribute("Status")]
        public string Status { get; set; }

        [XmlAttribute("Percentage")]
        public int Percentage { get; set; }
    }
}
