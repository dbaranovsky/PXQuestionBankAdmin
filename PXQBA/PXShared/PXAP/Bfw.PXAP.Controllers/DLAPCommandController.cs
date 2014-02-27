
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.Profile;
using Bfw.Common.Collections;
using Bfw.Common.JqGridHelper;

using Bfw.PXAP.Components;
using Bfw.PXAP.Models;
using Bfw.PXAP.ServiceContracts;
using Bfw.PXAP.Services;

namespace Bfw.PXAP.Controllers
{
    // Controller for the move content action
    public class DlapCommandController : ApplicationController
    {
        private IMenuService menuService;
        private IDlapService dlapService;
        public DlapCommandController(IApplicationContext context) : base(context)
        {
            menuService = new MenuService();
            dlapService = new DlapService(context);
        }

       [HttpGet]
        public ActionResult Index()
        {
            ModelState.Clear();
            var content = new DlapCommandModel();
            content.MenuModel = menuService.GetDLAPMenu();

            return View(content);
        }

        [ValidateInput(false)]
       public ActionResult RunCommand(DlapCommandModel content)
        {
            ModelState.Clear();
            if (content == null || content.command == null)
            {
                content = new DlapCommandModel();
                content.result = "NO DATA";
            }
            else if(content.method == DlapCommandModel.HttpMethod.GET)
            {
                string entityId = content.entityid.ToString();
                var result = dlapService.RunCommand(content.command, ref entityId, DlapCommandModel.HttpMethod.GET);
                content.entityid = Int32.Parse(entityId);
                if (result.Length > 1000000)
                {
                    result = result.Substring(0, 1000000);
                    result += "Result too long";
                }
                content.result = result;
                content.postdata = string.Empty;
                content.method = DlapCommandModel.HttpMethod.GET;
            }
            else if (content.method == DlapCommandModel.HttpMethod.POST)
            {
                string getCommand = content.command;
                string entityId = content.entityid.ToString();
                content.entityid = Int32.Parse(entityId);
                if (content.postdata.IsNullOrEmpty())
                {
                    content.postdata = content.result;
                }
                if (content.postdata.IsNullOrEmpty())
                {
                    content = new DlapCommandModel();
                    content.result = "NO DATA SUBMITTED";
                }
                else
                {
                    content = dlapService.ConvertGetToPost(content, getCommand);

                    
                    var result = dlapService.RunCommand(content.command, ref entityId, DlapCommandModel.HttpMethod.POST, content.postdata);
                    
                    if (result.Length > 1000000)
                    {
                        result = result.Substring(0, 1000000);
                        result += "Result too long";
                    }
                    content.result = result;
                }

                content.command = getCommand;
              
            }
            content.MenuModel = menuService.GetDLAPMenu();

            return View("Index", content);
        }

        [ValidateInput(false)]
        public ActionResult PutItems(DlapCommandModel content)
        {
            ModelState.Clear();
            if (content == null)
            {
                content = new DlapCommandModel();
                content.result = "NO DATA";
            }
            else
            {
              
               
                string entityId = content.entityid.ToString();
                var result = dlapService.RunCommand(content.command, ref entityId, DlapCommandModel.HttpMethod.POST, content.postdata);
                content.entityid = Int32.Parse(entityId);
                content.result = result;
                
                //restore original get command
                

            }
            content.MenuModel = menuService.GetDLAPMenu();

            return View("Index", content);
        }
    }
}
