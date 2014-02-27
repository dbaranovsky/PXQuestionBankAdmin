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
    public static class CourseMaterialsMapper
    {

        /// <summary>
        /// Maps a CourseMaterials business object to a CourseMaterials model
        /// </summary>
        /// <param name="biz">Course Materials business object</param>
        /// <returns></returns>
        public static CourseMaterials ToCourseMaterials(this BizDC.CourseMaterials biz)
        {
            var model = new CourseMaterials();

            model.ResourceList = biz.ResourceList;
            model.AssestList = biz.AssestList;
            model.FromSave = biz.FromSave;
            model.ItemId = biz.ItemId;

            return model;
        }
    }
}
