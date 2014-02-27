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
    public class ContentController : Controller
    {
        /// <summary>
        /// Contains business layer context information.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ContentController(BizSC.IBusinessContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Return the Index View.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public ActionResult Index(ContentIndex model)
        {
            ViewData["id"] = model.id;
            ViewData.Model = model;
            return View();
        }
    }
}
