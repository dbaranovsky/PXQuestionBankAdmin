using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web;

using Bfw.Common.Logging;

using Microsoft.Practices.ServiceLocation;

namespace Bfw.PX.PXPub.Components
{
    /// <summary>
    /// A filter that starts and stops a trace using the ILogger.
    /// </summary>
    public class PerfTraceFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Gets or sets the trace.
        /// </summary>
        /// <value>
        /// The trace.
        /// </value>
        protected ITraceHandle Trace { get; set; }

        /// <summary>
        /// Gets or sets the tracer.
        /// </summary>
        /// <value>
        /// The tracer.
        /// </value>
        protected ITraceManager Tracer { get; set; }

        /// <summary>
        /// Gets or sets the result trace.
        /// </summary>
        /// <value>
        /// The result trace.
        /// </value>
        protected ITraceHandle ResultTrace { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        protected ILogger Logger { get; set; }

        /// <summary>
        /// The qualified name of the action, Controller.Action format.
        /// </summary>
        protected string ActionName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerfTraceFilterAttribute"/> class.
        /// </summary>
        public PerfTraceFilterAttribute()
        {
            Logger = ServiceLocator.Current.GetInstance<ILogger>();
            Tracer = ServiceLocator.Current.GetInstance<ITraceManager>();
        }

        /// <summary>
        /// Called by the MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ActionName = string.Format("{0}.{1}", filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, filterContext.ActionDescriptor.ActionName);

            Logger.Log(string.Format("Executing {0}", ActionName), LogSeverity.Debug);

            const string actionKey = "EXECUTING_ACTION";
            if (!HttpContext.Current.Items.Contains(actionKey))
            {
                HttpContext.Current.Items.Add(actionKey, ActionName);
            }
            else
            {
                HttpContext.Current.Items[actionKey] = ActionName;
            }
            Trace = Tracer.StartTrace(ActionName);
        }

        /// <summary>
        /// Called by the MVC framework after the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (Trace != null)
            {
                Trace.Dispose();
            }
        }

        /// <summary>
        /// Called by the MVC framework before the action result executes.
        /// </summary>
        /// <param name="filterContext">The filter context</param>
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            ResultTrace = Tracer.StartTrace(string.Format("Executing ActionResult for {0}", ActionName));
        }

        /// <summary>
        /// Called by the MVC framework after the action result executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (ResultTrace != null)
            {
                ResultTrace.Dispose();
            }
        }
    }
}
