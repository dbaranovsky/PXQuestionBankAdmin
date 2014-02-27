using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Bfw.PXAP.Models
{
    public class ContentModel
    {
        public int EntityId { get; set; }
        public int MoveToEntityId { get; set; }
        public string ParentId { get; set; }   
        public string Category { get; set; }
        public string ContentType { get; set; }
        public string ContentSubType { get; set; }
        public bool MoveToParent { get; set; }


        public List<ExternalMenuModel> ExternalMenuModel { get; set; }
        public List<MainMenuModel> MainMenuModel { get; set; }
        public List<MainMenuModel> MenuModel { get; set; }

        public ContentModel()
        {
            
        }
    }
}
