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
    public class EnvironmentController : ApplicationController
    {
        #region Constructor

        public EnvironmentController(IApplicationContext context) : base(context)
        {       
        }

        #endregion

        public ActionResult Environments()
        {
            IMenuService menuService = new MenuService();
           

            SettingsModel settingsModel = new SettingsModel();
            settingsModel.MenuModel = menuService.GetSettingsMenu();

            this.ViewData.Model = settingsModel;


            return View();
        }

        public ActionResult AddEnvironment(int environmentId) 
        {
            PXEnvironment env = new PXEnvironment();
            if (environmentId != 0)
            {
                var layoutModel = this.ViewBag.LayoutModel as LayoutModel;
                env = (from e in layoutModel.PxEnvironments
                       where e.EnvironmentId == environmentId
                       select e).FirstOrDefault();
            }

            this.ViewData.Model = env;

            return View("_EnvironmentPartial");
        }

        [HttpPost]
        public ActionResult AddEnvironment(PXEnvironment env)
        {
            string action = "Added";
            if (env.EnvironmentId > 0)
            {
                action = "Updated";
            }

            string message = string.Empty;
            IEnvironmentService envService = new EnvironmentService();
            int newEnvId = envService.AddUpdateEnvironment(env, out message);
            if (string.IsNullOrEmpty(message))
            {
                message = "An error ocured while processing reuest for Environment";
            }
            
            bool result = false;
            if (newEnvId > 0)
            {
                result = true;
            }
            
            return Json(new { Result = result, EnvironmentId = newEnvId, Action = action, Message = message });
            //return View("Environments");
            //return result;
        }

        [HttpPost]
        public ActionResult DeleteEnvironment(int environmentId)
        {
            string message = "Environment was delete succssfully";

            IEnvironmentService envService = new EnvironmentService();
            bool result = envService.DeleteEnvironment(environmentId);
            if (!result)
            {
                message = "An error occured at server while deleting environment";
            }

            return Json(new { Message = message});
        }
    }
}
