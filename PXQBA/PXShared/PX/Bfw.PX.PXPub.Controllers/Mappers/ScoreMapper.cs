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
    public static class ScoreMapper
    {
        /// <summary>
        /// Maps a Grade business object to a Grade model.
        /// </summary>
        /// <param name="biz">The Grade business object.</param>
        /// <returns>
        /// Grade model.
        /// </returns>
        public static Score ToScore(this BizDC.Grade biz)
        {
            var model = new Score();

            if (null != biz)
            {
                model.Correct = biz.Achieved;
                model.Possible = biz.Possible;
                model.Date = biz.ScoredDate.HasValue ? biz.ScoredDate.Value : DateTime.MinValue;
            }

            return model;
        }
    }
}
