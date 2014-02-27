using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class AllowedWidget
    {
        /// <summary>
        /// Template name of the widget
        /// </summary>
        public string widgetType { get; set; }

        /// <summary>
        /// Display name of the widget
        /// </summary>
        public string widgetName { get; set; }
    }
}
