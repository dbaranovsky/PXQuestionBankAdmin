using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Bfw.PXAP.Models
{
    [Serializable]
    [XmlRoot("Environment")]
    public class PXEnvironment
    {
        [XmlAttribute("EnvironmentId")]
        public int EnvironmentId { get; set; }

        [XmlAttribute("Title")]
        public string Title { get; set; }

        [XmlAttribute("Description")]
        public string Description { get; set; }

        [XmlAttribute("DlapServer")]
        public string DlapServer { get; set; }

        [XmlAttribute("BrainHoneyServer")]
        public string BrainHoneyServer { get; set; }

        [XmlAttribute("BrainHoneyDocs")]
        public string BrainHoneyDocs { get; set; }

        [XmlAttribute("PxDocs")]
        public string PxDocs { get; set; }

        [XmlElement("Sources")]
        public List<string> Sources { get; set; }
        
        [XmlIgnore]
        public List<SelectListItem> SourcesSelectList
        {
            get
            {
                List<SelectListItem> selectList = new List<SelectListItem>();

                foreach (string s in this.Sources)
                {
                    selectList.Add(new SelectListItem() { Text = s, Value = s, Selected = false });
                }

                return selectList;
            }
        }

        public PXEnvironment()
        {
            Sources = new List<string>();
        }

    }

    public class EnvSources
    {
        public int EnvironmentId { get; set; }
        public string Source { get; set; }
    }
}
