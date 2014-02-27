using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Bfw.Common.Collections;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using System.Collections;
using System.Configuration;
using Bfw.Common;
using System.IO;


namespace Bfw.PX.PXPub.Controllers
{
    /// <summary>
    /// Controller that has methods to return general grade information
    /// </summary>
    [PerfTraceFilter]
    public class GradeController: Controller
    {


        #region Data Members
        /// <summary>
        /// Business Context
        /// </summary>
        private BizSC.IBusinessContext Context { get; set; }
        /// <summary>
        /// Handle to the grade actions service
        /// </summary>
        private BizSC.IGradeActions GradeActions { get; set; }
        #endregion


        #region Constructors

        public GradeController(BizSC.IBusinessContext context, BizSC.IGradeActions gradeActions)
        {
            Context = context;
            GradeActions = gradeActions;

        }
        #endregion

        #region Action Methods

        /// <summary>
        /// Returns true if an Item has a grade
        /// </summary>
        /// <param name="itemId">Id for the Item which you are looking up</param>
        /// <returns>Returns true if an Item has a grade</returns>
        public ActionResult GradeExistForItem(List<string> itemIds)
        {
            var result = false;

            var grades = GradeActions.GetGrades(Context.EntityId, itemIds);

            if (!grades.IsNullOrEmpty())
            {

                result = true;
            }

            return Content(result.ToString());
        }

        #endregion
    }
}
