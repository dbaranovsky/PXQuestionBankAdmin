using System.Web.Mvc;

namespace Macmillan.PXQBA.Web.Controllers.Infrastructure
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