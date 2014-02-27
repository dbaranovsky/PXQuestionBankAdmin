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
    public static class WidgetCallbackMapper
    {
        public static WidgetCallback ToWidgetCallback(this BizDC.WidgetCallback biz)
        {
            var model = new WidgetCallback();

            if (biz != null)
            {
                model.Action = biz.Action;
                model.Controller = biz.Controller;
                model.IsFNE = biz.IsFNE;
                model.Name = biz.Name;
                model.IsASync = biz.IsASync;
            }

            return model;
        }

        public static BizDC.WidgetCallback ToWidgetCallback(this  WidgetCallback model)
        {
            var biz = new BizDC.WidgetCallback();
            
            if (model != null)
            {
                biz.Action = model.Action;
                biz.Controller = model.Controller;
                biz.IsFNE = model.IsFNE;
                biz.Name = model.Name;
                biz.IsASync = model.IsASync;
            }

            return biz;
        }
    }
}
