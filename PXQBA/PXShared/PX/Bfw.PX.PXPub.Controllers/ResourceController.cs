using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Models;

using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers
{
    [PerfTraceFilter]
    public class ResourceController : Controller
    {
        /// <summary>
        /// Contains business layer context information.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }
        /// <summary>
        /// Gets or sets the content actions.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="contentActions">The content actions.</param>
        public ResourceController(BizSC.IBusinessContext context, BizSC.IContentActions contentActions)
        {
            Context = context;
            ContentActions = contentActions;
        }

        /// <summary>
        /// Lists the resources.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="resourcePath">The resource path.</param>
        /// <returns></returns>
        public ActionResult ListResources(string id, string resourcePath)
        {
            var result = ContentActions.ListResources(id, resourcePath, "//AssignmentId[text() = 'cde62368c8004b548b051527035f1497']");
            List<ResourceDocument> resList = new List<ResourceDocument>();

            foreach (var item in result)
            {
                resList.Add(new ResourceDocument() { title = item.Name, path = item.Url, contenttype = item.ContentType });
            }

            ViewData.Model = resList;
            return View();
        }
    }
}
