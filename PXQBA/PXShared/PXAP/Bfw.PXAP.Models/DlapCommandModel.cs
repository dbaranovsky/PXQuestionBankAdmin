using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Web.Mvc;

namespace Bfw.PXAP.Models
{
    public class DlapCommandModel
    {
        public enum HttpMethod
        {
            GET,
            POST

        }
        public int entityid { get; set; }

        [Required]
        public string command { get; set; }
        public string postdata { get; set; }
        public HttpMethod method { get; set; }
        public string result { get; set; }

        public List<ExternalMenuModel> ExternalMenuModel { get; set; }
        public List<MainMenuModel> MainMenuModel { get; set; }
        public List<MainMenuModel> MenuModel { get; set; }

        public DlapCommandModel()
        {
            
        }
    }
}
