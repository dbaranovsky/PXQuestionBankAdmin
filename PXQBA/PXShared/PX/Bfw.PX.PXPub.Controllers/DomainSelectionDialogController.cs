using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Bfw.Common.Collections;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;

using BizSC = Bfw.PX.Biz.ServiceContracts;
using BizDC = Bfw.PX.Biz.DataContracts;
using System;

namespace Bfw.PX.PXPub.Controllers
{
    public class DomainSelectionDialogController : Controller
    {
        /// <summary>
        /// Access to the current business context information.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public DomainSelectionDialogController(BizSC.IBusinessContext context)
        {
            Context = context;
        }

        /// <summary>
        /// The main method to present the domain selection dialog box
        /// </summary>
        /// <param name="controller">the controller to callback when the user hits submit</param>
        /// <param name="action">the method on the controller to call user selects successfully</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DomainSelection(string callbackFunction)
        {
            DomainSelection userDomains = new DomainSelection();
            userDomains.Domains = Context.GetRaUserDomains().Distinct().OrderBy(d => d.Name);

            List<BizDC.Domain> domainList = new List<BizDC.Domain>();
            String[] callback = callbackFunction.Split('/');

            if (!userDomains.Domains.IsNullOrEmpty())
            {
                domainList = userDomains.Domains.ToList();
                BizDC.Domain pxGeneric = userDomains.Domains.FirstOrDefault(i => i.Name.ToLowerInvariant() == "pxgeneric");
                if(pxGeneric != null){
                  domainList.Remove(pxGeneric);
                }
                userDomains.Domains = domainList;
            }

            userDomains.CallbackController = callback[0];
            userDomains.CallbackAction = callback[1];
            

            return View("~/Views/Shared/DomainSelectionDialog.ascx", userDomains);
           // return View("DomainSelectionDialog", userDomains);
        }


    }
}
