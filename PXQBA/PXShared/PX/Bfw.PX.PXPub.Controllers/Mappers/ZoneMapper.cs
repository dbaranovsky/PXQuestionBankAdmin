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
    public static class ZoneMapper
    {
        public static Zone ToZoneItem(this BizDC.Zone biz)
        {
            var model = new Zone();

            if (biz != null)
            {
                model.Id = biz.Id;
                model.ParentId = biz.ParentId;
                model.CourseID = biz.CourseID;
                model.Sequence = biz.Sequence;
                model.Title = biz.Title;
                model.IsSupportHide = biz.IsSupportHide;

                var buider = new StringBuilder();

                foreach (var allowed in biz.AllowedWidgets)
                {
                    buider.AppendFormat("{0},", allowed.widgetType);
                    model.AllowedWidgets.Add(allowed.ToAllowedWidget());
                }

                model.AllowedWidgetList = buider.ToString().TrimEnd(',');

                foreach (var widget in biz.Widgets)
                {
                    model.Widgets.Add(widget.ToWidgetItem());
                }
                model.DefaultPage = biz.DefaultPage.ToPageDefinition();
            }

            return model;
        }

        public static BizDC.Zone ToZoneItem(this Zone model)
        {
            var biz = new BizDC.Zone();

            if (model == null)
            {
                return biz;
            }

            biz.Id = model.Id;
            biz.ParentId = model.ParentId;
            biz.CourseID = model.CourseID;
            biz.Sequence = model.Sequence;
            biz.Title = model.Title;

            foreach (var widget in model.Widgets)
            {
                biz.Widgets.Add(widget.ToWidgetItem());
            }
            biz.DefaultPage = model.DefaultPage.ToPageDefinition();
            return biz;
        }
    }
}
