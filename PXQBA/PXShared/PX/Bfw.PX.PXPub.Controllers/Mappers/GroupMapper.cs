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
    public static class GroupMapper
    {

        /// <summary>
        /// Maps a Group from the business layer to a Group model.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static Group ToGroup(this BizDC.Group biz)
        {
            var model = new Group()
            {
                Name = biz.Name,
                GroupId = biz.Id,
                Members = biz.Members.Map(e => e.ToStudent()).ToList()
            };

            return model;
        }
    }
}
