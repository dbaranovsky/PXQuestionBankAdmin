using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bfw.PXAP.Models;
using Bfw.PXAP.Services;
using Bfw.PXAP.Components;
using Bfw.Common.JqGridHelper;

namespace Bfw.PXAP.Controllers
{
    public class HomeController : ApplicationController
    {

        #region Constructor

        public HomeController(IApplicationContext context) : base(context)
        {       
        }

        #endregion

        public ActionResult Index()
        {
            DashboardModel objDashboardModel = new DashboardModel();
            var layoutModel = this.ViewBag.LayoutModel as LayoutModel;
            var currEnv = layoutModel.GetCurrentEnvironment();
            var logSummary = this.GetLogSummarModel();
            objDashboardModel.LogSummary = new LogSummaryViewModel();
            objDashboardModel.LogSummary.LogSummary = logSummary;
            objDashboardModel.LogSummary.Environment = currEnv;

            return View(objDashboardModel);
        }

        public ActionResult LoadLogSummary()
        {
            var model = new LogSummaryViewModel();
            model.LogSummary = this.GetLogSummarModel();
            var layoutModel = this.ViewBag.LayoutModel as LayoutModel;
            var currEnv = layoutModel.GetCurrentEnvironment();
            model.Environment = currEnv;

            return View("_LogSummaryPartial", model);
        }

        public ActionResult ClearLogs(string severity)
        {
            var objDashboardService = new DashboardService();
            objDashboardService.ClearLogs(severity);
            
            return Json(new { Message = "success" }, JsonRequestBehavior.AllowGet);
        }

        private List<LogSummaryModel> GetLogSummarModel()
        {
            DashboardService objDashboardService = new DashboardService();
            var layoutModel = this.ViewBag.LayoutModel as LayoutModel;
            var currEnv = layoutModel.GetCurrentEnvironment();

            if (currEnv != null && currEnv.Sources != null && currEnv.Sources.Count != 0)
            {
                string sources = string.Join("</source><source>", currEnv.Sources.ToArray());
                sources = string.Format("<sources><source>{0}</source></sources>", sources);
                return objDashboardService.GetLogSummary(sources);
            }

            return new List<LogSummaryModel>();
            
        }

        public ActionResult About(int? id)
        {
            return View();
        }
      
    }
}
