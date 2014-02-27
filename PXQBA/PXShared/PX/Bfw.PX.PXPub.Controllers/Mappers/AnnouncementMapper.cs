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
    public static class AnnouncementMapper
    {
        /// <summary>
        /// Maps an Announcement business object to an Announcement model.
        /// </summary>
        /// <param name="biz">Announcement business object.</param>
        /// <returns>
        /// Announcement model.
        /// </returns>
        public static Announcement ToAnnouncement(this BizDC.Announcement biz)
        {
            var model = new Announcement();

            if (null != biz)
            {
                model.Title = biz.Title;
                model.Body = biz.Body;
                model.DisplayDate = biz.CreationDate;
                model.Path = biz.Path;
                model.StartDate = biz.StartDate;
                model.EndDate = biz.EndDate;
                model.PrimarySortOrder = biz.PrimarySortOrder;
                model.PinSortOrder = biz.PinSortOrder;
                model.IsArchived = biz.IsArchived;                
            }

            return model;
        }
    }
}
