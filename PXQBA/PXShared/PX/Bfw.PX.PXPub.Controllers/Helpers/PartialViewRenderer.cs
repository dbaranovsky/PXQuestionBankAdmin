// -----------------------------------------------------------------------
// <copyright file="PartialViewRenderer.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Bfw.PX.PXPub.Controllers.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using System.IO;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class PartialViewRenderer
    {
        public static string ToString(ControllerContext controllerContext, string viewName, object model, ViewDataDictionary viewData, TempDataDictionary tempData)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = controllerContext.RouteData.GetRequiredString("action");

            controllerContext.Controller.ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                ViewContext viewContext = new ViewContext(controllerContext, viewResult.View, viewData, tempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
