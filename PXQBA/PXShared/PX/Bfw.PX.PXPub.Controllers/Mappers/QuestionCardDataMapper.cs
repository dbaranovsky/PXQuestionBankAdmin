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
    public static class QuestionCardDataMapper
    {

        /// <summary>
        /// Maps a Course business object to a Course model.
        /// </summary>
        /// <param name="biz">The Course business object.</param>
        /// <returns>
        /// Course model.
        /// </returns>
        public static QuestionCardData ToQuestionCard(this Biz.DataContracts.QuestionCardData biz)
        {
            var model = new QuestionCardData();

            if (null != biz)
            {

                model.Filterable = biz.Filterable;
                model.FriendlyName = biz.FriendlyName;
                model.QuestionCardDataName = biz.QuestionCardDataName;
                if (biz.QuestionValues != null)
                {
                    model.QuestionValues = biz.QuestionValues;
                }

            }
            return model;
        }



     
    }
}
