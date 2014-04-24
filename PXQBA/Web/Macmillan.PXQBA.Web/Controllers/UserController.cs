using System.Web.Mvc;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class UserController : MasterController
    {
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Keeps the session alive.
        /// </summary>
        /// <param name="data">Random value.</param>
        /// <returns></returns>
        public ActionResult KeepSessionAlive(string data)
        {
            return new JsonResult { Data = "Success" };
        }
	}
}