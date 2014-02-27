using System;
using System.Collections.Generic;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class SystemMessageMapper
    {
        /// <summary>
        /// Convert content item to system message
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static SystemMessage ToSystemMessage(this BizDC.ContentItem biz)
        {
            var model = new SystemMessage();
            if (biz != null)
            {
                model.AvilableDate = biz.AvailableDate;
                model.ExpirationDate = biz.AssignmentSettings != null ? biz.AssignmentSettings.DueDate : DateTime.MaxValue;
                model.Title = biz.Title;
                model.Description = biz.Description;
            }
           
            return model;
        }
        
        /// <summary>
        /// Convert system message to content item
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static BizDC.ContentItem ToContentItem(this SystemMessage model)
        {
            var biz = new BizDC.ContentItem();

            if (null != model)
            {
                biz.Title = model.Title;
                biz.Description = model.Description;
                biz.Id = "System_Message";
                biz.AvailableDate = model.AvilableDate;

                biz.AssignmentSettings = new BizDC.AssignmentSettings
                {
                    IsAssignable = true,
                    DueDate = model.ExpirationDate
                };
            }

            return biz;
        }
    }

}
