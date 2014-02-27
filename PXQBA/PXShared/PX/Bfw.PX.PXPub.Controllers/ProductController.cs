using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Components;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Mappers;

namespace Bfw.PX.PXPub.Controllers
{
    [PerfTraceFilter]
    public class ProductController : Controller
    {
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ProductController(BizSC.IBusinessContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Returns the Index View for this Controller.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View("ProductHome");
        }
    }
}
