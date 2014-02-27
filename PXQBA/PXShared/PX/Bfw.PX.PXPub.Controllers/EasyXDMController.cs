using System.Web.Mvc;
using System.Configuration;
using System.Web.Routing;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Models;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using System;

namespace Bfw.PX.PXPub.Controllers
{
    [PerfTraceFilter]
    public class EasyXDMController : Controller
    {
        /// <summary>
        /// Access to the current business context information.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Constructs a default AccountWidgetController. Depends on a business context
        /// and user actions implementation.
        /// </summary>
        /// <param name="context">The context.</param>
        public EasyXDMController(BizSC.IBusinessContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Generates the necessary code to create the consumer
        /// </summary>
        /// <returns></returns>
        public ActionResult Consumer(EasyXDM xdm)
        {
            return View(xdm);
        }

        /// <summary>
        /// Generates the necessary code to create the provider
        /// </summary>
        /// <returns></returns>
        public ActionResult Provider(EasyXDM xdm)
        {
            return View(xdm);
        }
    }
}
