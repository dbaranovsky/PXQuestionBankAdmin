using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

using Bfw.PX.Account.Abstract;

namespace Bfw.PX.Account.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class HomeController : Controller
    {
        #region Properties

        /// <summary>
        /// Context that contains all request information
        /// </summary>
        protected IRequestContext Context { get; set; }

        #endregion

        public HomeController(IRequestContext ctx)
        {
            Context = ctx;
        }

        #region Actions

        /// <summary>
        /// Only executed when a user is errorneously directed to the application root. This should
        /// not be possible in a working environment. View should probably contain some kind of message
        /// and a link to a catalog or user help page
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        #endregion
    }
}
