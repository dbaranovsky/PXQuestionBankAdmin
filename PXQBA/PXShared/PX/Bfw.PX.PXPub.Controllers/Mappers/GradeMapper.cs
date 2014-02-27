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
    public static class GradeMapper
    {
        /// <summary>
        /// To the grade.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static Grade ToGrade(this BizDC.Grade biz)
        {
            var model = new Grade();

            if (null != biz)
            {                
                model.ItemId = biz.ItemId;
                model.ItemTitle = biz.ItemName;
                model.Achieved = biz.Achieved;
                model.RawAchieved = biz.RawAchieved;
                model.RawPossible = biz.RawPossible;
                model.Letter = biz.Letter;
                model.Possible = biz.Possible;
            }

            return model;
        }
    }
}
