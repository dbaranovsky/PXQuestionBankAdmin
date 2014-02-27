using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bfw.PXAP.Models;
using Bfw.PXAP.Services;
using Bfw.PXAP.Components;
using Bfw.Common.JqGridHelper;

using Bfw.PXAP.ServiceContracts;
using Bfw.PXAP.Services;

namespace Bfw.PXAP.Controllers
{
    public class LogSearchController : ApplicationController
    {

        #region Constructor
        public LogSearchController(IApplicationContext context) : base(context)
        {
        }
        #endregion Constructor

        public ActionResult LogSearch(string Severity)
        {
            var layoutModel = this.ViewBag.LayoutModel as LayoutModel;

            ILogSearchService service = new LogSearchService();
            LogSearchModel logSearchModel = service.GetLogSearchModel(layoutModel.GetCurrentEnvironment());

            if (!string.IsNullOrEmpty(Severity))
            {
                var sev = (from s in logSearchModel.SeverityOptions
                           where s.Value.ToUpper() == Severity.ToUpper()
                           select s).FirstOrDefault();

                if (sev != null)
                {
                    sev.Selected = true;
                }
            }

            return View(logSearchModel);
        }

        //[HttpPost]
        //public ActionResult LogSearch(LogSearchModel model)
        //{
        //    //if (ModelState.IsValid)
        //    //{
        //    //    LogSearchService objLogSearchService = new LogSearchService();
        //    //    model.Logs = objLogSearchService.GetLogs(model.Severity, model.Category, model.Source, model.Message.Trim(), model.StartDate, model.EndDate);
        //    //}

        //    return View(model);
        //}
        
        public ActionResult LogSearchGrid(string severity, string category, string startDate, string endDate, string source, string message, string sidx, string sord, int page, int rows)
        {
            LogSearchService objLogSearchService = new LogSearchService();

            //if (source.ToUpper() == "ALL")
            //{
            //    var layoutModel = this.ViewBag.LayoutModel as LayoutModel;
            //    var currEnv = layoutModel.GetCurrentEnvironment();
            //    if (currEnv != null && currEnv.Sources != null && currEnv.Sources.Count != 0)
            //    {
            //        source = string.Join("','", currEnv.Sources);                    
            //    }
            //}
            //source = string.Format("'{0}'", source);

            var logs = objLogSearchService.GetLogs(severity, category, source, message, startDate, endDate);         

            var model = from log in logs.AsQueryable()
                        select new
                        {
                            Id = log.LogID,
                            Severity = log.Severity,
                            Source = log.Source,
                            Time = log.Time.ToString(),
                            Category = log.CategoryName,
                            Message = log.Message,
                            Actions = string.Format("<a href=\"javascript:showMessageDdetails({0})\">View</a>", log.LogID)                             
                        };


            var result = model.ToJqGridData(page, rows, sidx + " " + sord, "",
                new[] {
                    "Id","Severity", "Source", "Time", "Category", "Message", "Actions"
                });

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetErrorMessage(int logID)
        {
            ILogSearchService service = new LogSearchService();
            string sErrorMessage = service.GetErrorMessage(logID);

            return Json(new { ErrorMessage = sErrorMessage },JsonRequestBehavior.AllowGet);
        }
    }
}
