using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Bfw.PXAP.Components;
using Bfw.PXAP.Models;
using Bfw.PXAP.ServiceContracts;
using Bfw.PXAP.Services;

namespace Bfw.PXAP.Controllers
{
    [LogonAuthorize]
    public abstract class ApplicationController : Controller
    {
        #region Properties

        protected IApplicationContext Context { get; set; }

        #endregion


        #region Constructor

        public ApplicationController(IApplicationContext context)
        {
            Context = context;
        }

        #endregion


        #region methods which are overridden

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            this.SetEnvironment();

            IEnvironmentService envService = new EnvironmentService();
            var allEnvironments = envService.GetEnvironments();



            LayoutModel layoutModel = new LayoutModel()
            {
                CurrentEnvironment = this.Context.Environment, 
                PxEnvironments = allEnvironments
            };

            MenuService menuService = new MenuService();
            var externalMenu = menuService.GetExternalMenu(layoutModel.GetCurrentEnvironment());

            var environmentService = new EnvironmentService();
            environmentService.ChangeCachingConfiguration(Context.Environment);

            layoutModel.ExternalMenuModel = externalMenu;
            layoutModel.MainMenuModel = menuService.GetMainMenu();

            this.ViewData["LayoutModel"] = layoutModel;
        }

        #endregion

        #region methods

        /// <summary>
        /// Gets environment from routData, create enum and set it on Context
        /// </summary>
        private void SetEnvironment()
        {
            //HttpContextWrapper cw = new HttpContextWrapper();
            //cw.
            //this.Response.

            string env = this.RouteData.Values["environment"] as string;
            if (env == null)
            {
                env = string.Empty;
            }

            this.Context.Environment = env;
        }

        #endregion
    }
}
