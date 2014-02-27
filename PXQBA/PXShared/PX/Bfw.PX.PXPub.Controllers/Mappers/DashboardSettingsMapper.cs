using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class DashboardSettingsMapper
    {


        /// Maps Biz.DashboardSettings to Model.DashboardSettings
        /// </summary>

        public static Models.DashboardSettings ToDashboardSettings(this BizDC.DashboardSettings biz)
        {
            var model = new Models.DashboardSettings();

                model.IsInstructorDashboardOn = biz.IsInstructorDashboardOn;
                model.IsProgramDashboardOn = biz.IsProgramDashboardOn;


            if (biz.DashboardHomePageStart != null)
            {
                model.DashboardHomePageStart = biz.DashboardHomePageStart;
            }
            if (biz.ProgramDashboardHomePageStart != null)
            {
                model.ProgramDashboardHomePageStart = biz.ProgramDashboardHomePageStart;
            }

               
           
            return model;
        }

        public static BizDC.DashboardSettings ToDashboardSettings(this Models.DashboardSettings model)
        {
            var biz = new BizDC.DashboardSettings();

            biz.IsInstructorDashboardOn = model.IsInstructorDashboardOn;
            biz.IsProgramDashboardOn = model.IsProgramDashboardOn;


            if (model.DashboardHomePageStart != null)
            {
                biz.DashboardHomePageStart = model.DashboardHomePageStart;
            }
            if (model.ProgramDashboardHomePageStart != null)
            {
                biz.ProgramDashboardHomePageStart = model.ProgramDashboardHomePageStart;
            }



            return biz;
        }
    }
}
