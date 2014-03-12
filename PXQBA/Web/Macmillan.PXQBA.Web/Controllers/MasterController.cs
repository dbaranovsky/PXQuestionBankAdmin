using System.Web.Mvc;
using Macmillan.PXQBA.Web.ActionResults;

namespace Macmillan.PXQBA.Web.Controllers
{
    public abstract class MasterController: Controller
    {
        protected ActionResult JsonCamel(object data)
        {
            return new JsonCamelCaseResult
            {
                Data = data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}