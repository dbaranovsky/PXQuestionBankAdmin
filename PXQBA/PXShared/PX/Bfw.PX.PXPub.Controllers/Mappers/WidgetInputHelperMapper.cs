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
    public static class WidgetInputHelperMapper
    {
        /// <summary>
        /// Conversion from biz WidgetInputHelper to model
        /// </summary>
        /// <param name="biz"></param>
        /// <returns></returns>
        public static WidgetInputHelper ToWidgetInputHelper(this BizDC.WidgetInputHelper biz)
        {
            var model = new WidgetInputHelper();

            model.Name = biz.Name;
            model.Selector = biz.Selector;
            model.DefaultValue = biz.DefaultValue;
            model.UseDefaultValue = biz.UseDefaultValue;

            return model;
        }

        /// <summary>
        /// Conversion from model WidgetInputHelper to biz
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static BizDC.WidgetInputHelper ToWidgetInputHelper(this WidgetInputHelper model)
        {
            var biz = new BizDC.WidgetInputHelper();
            biz.Name = model.Name;
            biz.Selector = model.Selector;
            biz.DefaultValue = model.DefaultValue;
            biz.UseDefaultValue = model.UseDefaultValue;

            return biz;
        }
    }
}
