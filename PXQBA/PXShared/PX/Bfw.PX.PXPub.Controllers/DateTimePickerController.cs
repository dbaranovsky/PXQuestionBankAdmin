using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using ContentItem = Bfw.PX.Biz.DataContracts.ContentItem;
using TocCategory = Bfw.PX.PXPub.Models.TocCategory;
using Resource = Bfw.PX.Biz.DataContracts.Resource;
using Bfw.Common;
using Bfw.PX.PXPub.Components;

namespace Bfw.PX.PXPub.Controllers {
    [PerfTraceFilter]
    public class DateTimePickerController : Controller
    {

        #region Properties

        /// <summary>
        /// Access to an INavigationActions implementation.
        /// </summary>
        protected INavigationActions NavigationActions { get; set; }
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
        /// Gets or sets the content helper.
        /// </summary>
        /// <value>
        /// The content helper.
        /// </value>
        protected ContentHelper ContentHelper { get; set; }
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
        public DateTimePickerController( BizSC.IBusinessContext context, BizSC.IUserActions userActions, ContentHelper contentHelper,
            IContentActions contentActions, ICourseMaterialsActions courseMaterialsActions, INavigationActions navActions)
        {
            Context = context;
            UserActions = userActions;
            ContentHelper = contentHelper;
            ContentActions = contentActions;
            CourseMaterialsActions = courseMaterialsActions;
            NavigationActions = navActions;
        }

        #endregion

        #region Methods

        public ActionResult Index(string Options)
        {
            return View("DateTimePicker");
        }




        #endregion

    }
}
