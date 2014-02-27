using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Bfw.PXAP.Models
{
    public class SettingsModel
    {
        public SettingsModel()
        {
            this.MenuModel = new List<MainMenuModel>();
        }

        public List<MainMenuModel> MenuModel { get; set; }



    }
}
