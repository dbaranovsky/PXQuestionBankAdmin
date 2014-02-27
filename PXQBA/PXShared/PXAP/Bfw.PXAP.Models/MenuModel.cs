using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PXAP.Models
{
    public class MainMenuModel
    {
        public string MenuName { get; set; }

        public string MenuAction { get; set; }

        public string MenuController { get; set; }
    }

    public class ExternalMenuModel
    {
        /// <summary>
        /// Name of the menu item
        /// </summary>
        public string MenuName { get; set; }

        /// <summary>
        /// URL value tagged to the menu item
        /// </summary>
        public string MenuURL { get; set; }
    }

    public class TestModel
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string city { get; set; }
        public string country { get; set; }
    }

}
