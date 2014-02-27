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
    public static class WidgetDisplayOptionsMapper
    {
        public static WidgetDisplayOptions ToWidgetDisplayOptions(this BizDC.WidgetDisplayOptions biz)
        {
            var model = new WidgetDisplayOptions();

            if (biz != null)
            {
                foreach (var option in biz.DisplayOptions)
                {
                    model.DisplayOptions.Add(option);
                }
            }

            return model;
        }

        public static BizDC.WidgetDisplayOptions ToWidgetDisplayOptions(this WidgetDisplayOptions model)
        {
            var biz = new BizDC.WidgetDisplayOptions();

            if (model != null)
            {
                foreach (var option in model.DisplayOptions)
                {
                    biz.DisplayOptions.Add(option);
                }
            }

            return biz;
        }
    }
}
