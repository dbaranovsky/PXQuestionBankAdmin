using System.Web.Mvc;
using System.Configuration;
using System.Web.Routing;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Models;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers
{
    public class AssignmentCenterSyllabusController : Controller
    {
        protected BizSC.IBusinessContext Context { get; set; }

        public AssignmentCenterSyllabusController(BizSC.IBusinessContext context)
        {
            Context = context;
        }

        
    }
}
