using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class AllowedWidgetMapper
    {
        public static Models.AllowedWidget ToAllowedWidget(this BizDC.AllowedWidget biz)
        {
            var model = new Models.AllowedWidget();

            if (biz != null)
            {
                model.widgetName = biz.widgetName;
                model.widgetType = biz.widgetType;
            }

            return model;
        }

        public static BizDC.AllowedWidget ToAllowedWidget(this Models.AllowedWidget model)
        {
            var biz = new BizDC.AllowedWidget();

            if (model != null)
            {
                biz.widgetName = model.widgetName;
                biz.widgetType = model.widgetType;
            }

            return biz;

        }
    }
}
