using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class DashboardItemMapper
    {
        /// <summary>
        /// Converts the DashboardItem (Data Contract) to DashboardItem (Model)
        /// </summary>
        /// <param name="Biz"></param>
        /// <returns></returns>
        public static DashboardItem ToDashboardItem(this BizDC.DashboardItem Biz)
        {
            var Model = new DashboardItem();

            if (null != Biz)
            {
                Model.CourseId = Biz.CourseId;
                Model.CourseTitle = Biz.CourseTitle;
                Model.OwnerId = Biz.OwnerId;
                Model.OwnerFirstName = Biz.OwnerFirstName;
                Model.OwnerLastName = Biz.OwnerLastName;
                Model.OwnerName = Biz.OwnerName;
                Model.OwnerEmail = Biz.OwnerEmail;
                Model.Count = Biz.Count;
                Model.Notes = Biz.Notes;
                Model.Users = Biz.Users;
            }

            return Model;
        }
    }
}
