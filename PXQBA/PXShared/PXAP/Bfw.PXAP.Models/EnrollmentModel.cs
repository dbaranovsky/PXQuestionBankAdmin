using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Bfw.PXAP.Models
{
    public class EnrollmentModel
    {
        public int EntityId { get; set; }
        public int StudentCount { get; set; }

        public List<ExternalMenuModel> ExternalMenuModel { get; set; }
        public List<MainMenuModel> MainMenuModel { get; set; }
        public List<MainMenuModel> MenuModel { get; set; }

        public EnrollmentModel()
        {
            
        }
    }
}
