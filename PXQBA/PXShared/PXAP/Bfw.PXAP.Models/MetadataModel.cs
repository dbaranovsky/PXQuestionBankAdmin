using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Bfw.PXAP.Models
{
    public class MetadataModel
    {
        public int EntityId { get; set; }
        public string ParentId { get; set; }
        public string FieldName { get; set; }
        public bool Exact { get; set; }
        public bool Recursive { get; set; }
        public string ParentCategory { get; set; }
        public string Value { get; set; }

        public List<ExternalMenuModel> ExternalMenuModel { get; set; }
        public List<MainMenuModel> MainMenuModel { get; set; }
        public List<MainMenuModel> MenuModel { get; set; }

        public MetadataModel()
        {
            //Set defaults
            Exact = true;
            Recursive = true;
        }
    }
}
