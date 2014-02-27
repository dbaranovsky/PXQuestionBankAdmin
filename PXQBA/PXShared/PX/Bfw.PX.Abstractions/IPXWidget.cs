using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Widget = Bfw.PX.PXPub.Models.Widget;


namespace Bfw.PX.Abstractions
{
    /// <summary>
    /// Widget interface
    /// </summary>
    public interface IPXWidget
    {
        /// <summary>
        /// Action providing the display of the widget when it is in 'Summary' mode.
        /// </summary>
        /// <returns></returns>
        ActionResult Summary(Widget widget);

        /// <summary>
        /// Action providing the display of the widget when it is in 'View All' mode.
        /// </summary>
        /// <returns></returns>
        ActionResult ViewAll(Widget widget);
    }
}
