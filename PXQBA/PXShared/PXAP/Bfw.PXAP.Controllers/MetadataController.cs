
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
    public class MetadataController : ApplicationController
    {
        public MetadataController(IApplicationContext context) : base(context) { }

        public ActionResult Search()
        {
            return View();
        }

        public ActionResult Index()
        {

            IMenuService menuService = new MenuService();
            //SettingsModel settings = new SettingsModel();
            MetadataModel metaModel = new MetadataModel();
            metaModel.MenuModel = menuService.GetMetadataMenu();
            this.ViewData.Model = metaModel;

            return View();
        }

        
        /// <summary>
        /// This is the method called by the AJAX form submit
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ApplyMetadata()
        {
            //need the progress model here in order to make sure the process updates properly as the metadata is applied
            String sProcess = Request["processId"].ToString();

            Int64 iProcess = 0;
            Int64.TryParse(sProcess, out iProcess);

            //gather the form post data
            //AddMetadata(int entityId, int parentId, string xmlField, bool bExact, string sValue, Int64 processId)
            String entityId = Request["EntityId"].ToString();
            String parentId = Request["ParentId"].ToString();
            String xmlField = Request["FieldName"].ToString();
            bool bExact = false;
            String chkExact = Request["Exact"].ToString();
            String parentCategory = Request["ParentCategory"].ToString();
            bool recursive = Request["Recursive"].ToString().Contains("true");
            if (chkExact.Contains("true"))
                bExact = true;

            String sValue = Request["Value"].ToString();

            IMetadataService svc = new MetadataService(Context);
            svc.AddMetadata(entityId, parentId, xmlField, bExact, sValue, iProcess, parentCategory, recursive);

            return Json(new
            {
                ProcessId = iProcess
            }, JsonRequestBehavior.DenyGet);
            
        }

    }
}
