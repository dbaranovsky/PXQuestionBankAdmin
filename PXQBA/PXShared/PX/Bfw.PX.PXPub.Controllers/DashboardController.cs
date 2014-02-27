using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using BizDC = Bfw.PX.Biz.DataContracts;
using Bfw.Common;
using Bfw.PX.PXPub.Components;
using Bfw.Common.Collections;
using System.Text;
using System.Web.Routing;
using System.Xml;
using System.Configuration;


namespace Bfw.PX.PXPub.Controllers {
    [PerfTraceFilter]
    public class DashboardController : Controller
    {

        #region Properties
        /// <summary>
        /// Content Actions
        /// </summary>
        protected IContentActions ContentActions { get; set; }
        /// <summary>
        /// Access to the current business context information.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Access to an IUserActions implementation.
        /// </summary>
        /// <value>
        /// The user actions.
        /// </value>
        protected BizSC.IUserActions UserActions { get; set; }

        protected ICourseMaterialsActions CourseMaterialsActions { get; set; }

        protected IPageActions PageActions { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs 
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="userActions">The user actions.</param>
        /// <param name="contentHelper">Context Helper</param>
        public DashboardController( BizSC.IBusinessContext context, BizSC.IUserActions userActions,
            IContentActions contentActions, ICourseMaterialsActions courseMaterialsActions)
        {
            Context = context;
            UserActions = userActions;
            ContentActions = contentActions;
            CourseMaterialsActions = courseMaterialsActions;

        }

        #endregion

        #region Methods

        public ActionResult Index(string Options)
        {
 
            return View();
        }




        #endregion

    }
}
