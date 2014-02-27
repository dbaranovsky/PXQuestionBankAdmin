using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Bfw.Common.Collections;
using Bfw.PX.PXPub.Components;

using Bfw.PX.Abstractions;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;

using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers
{
    [PerfTraceFilter]
    public class EqController : Controller
    {
        /// <summary>
        /// Returns the Index view.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return new EmptyResult();
        }
    }
}
