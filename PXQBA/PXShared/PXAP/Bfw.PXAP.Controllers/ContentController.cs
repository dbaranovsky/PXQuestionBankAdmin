
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.Profile;
using Bfw.Common.JqGridHelper;

using Bfw.PXAP.Components;
using Bfw.PXAP.Models;
using Bfw.PXAP.ServiceContracts;
using Bfw.PXAP.Services;

namespace Bfw.PXAP.Controllers
{
    // Controller for the move content action
    public class ContentController : ApplicationController
    {
        public ContentController(IApplicationContext context) : base(context) { }

        public ActionResult Copy()
        {
            return View();
        }

        public ActionResult Index()
        {
            IMenuService menuService = new MenuService();            
            ContentModel contentModel = new ContentModel();
            contentModel.MenuModel = menuService.GetContentMenu();
            contentModel.MoveToParent = true;
            this.ViewData.Model = contentModel;
            return View();
        }


        /// <summary>
        /// This is the method called by the AJAX form submit
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CopyContent()
        {
            //need the progress model here in order to make sure the process updates properly as the metadata is applied
            String sProcess = Request["processId"].ToString();

            Int64 iProcess = 0;
            Int64.TryParse(sProcess, out iProcess);

            //gather the form post data
            //CopyContent(entityId, parentId, category, contentType, contentSubType, moveToEntityId, moveToParent, iProcess);    
            String entityId = Request["EntityId"].ToString();
            String moveToEntityId = Request["MoveToEntityId"].ToString();
            bool moveToParent = Request["moveToParent"].ToString().Contains("true");            
            String parentId = Request["ParentId"].ToString();
            String category = Request["Category"].ToString();
            String contentType = Request["ContentType"].ToString();
            String contentSubType = Request["ContentSubType"].ToString();


            IContentService svc = new ContentService(Context);
            svc.CopyContent(entityId, parentId, category, contentType, contentSubType, moveToEntityId, moveToParent, iProcess);            

            return Json(new
            {
                ProcessId = iProcess
            }, JsonRequestBehavior.DenyGet);

        }


    }
}
