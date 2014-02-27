using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Configuration;

using Bfw.PX.PXPub.Components;

using Bfw.PX.Abstractions;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    /// <summary>
    /// Provided all actions required to implement the account widget which allows
    /// users to login, log out, and view their profile information
    /// </summary>
    [PerfTraceFilter]
    public class FacePlate_PxUnitWidgetController : Controller, IPXWidget
    {
        /// <summary>
        /// Access to the current business context information
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Access to an IUserActions implementation
        /// </summary>
        /// <value>
        /// The user actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }

        /// <summary>
        /// Gets or sets the assignment center helper.
        /// </summary>
        /// <value>
        /// The assignment center helper.
        /// </value>
        protected AssignmentCenterHelper AssignmentCenterHelper { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="contentActions"></param>
        public FacePlate_PxUnitWidgetController(BizSC.IBusinessContext context, BizSC.IContentActions contentActions, AssignmentCenterHelper assignmentHelper)
        {
            Context = context;
            ContentActions = contentActions;
            AssignmentCenterHelper = assignmentHelper;
        }

        #region IPXWidget Members

        /// <summary>
        /// Shows the login status of the currently logged in user. If the user is
        /// anonymous then they are considered to be not authenticated.
        /// </summary>
        /// <returns></returns>
        public ActionResult Summary()
        {
            var course = AssignmentCenterHelper.LoadAssignmentCenterData( "", "", "syllabus", true);
            var units = course.GetLessons().Where(i => !string.IsNullOrEmpty(i.Thumbnail));

            units.OrderBy(x => x.Title);

            var pxUnit = new PxUnit();
            pxUnit.AddPxUnit(units.OrderBy(x => x.Title).ToList());

            ViewData.Model = pxUnit;
            return View();
        }


        /// <summary>
        /// Shows all data related to the currently logged in user
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewAll()
        {
            return View();
        }

        #endregion
    }
}
