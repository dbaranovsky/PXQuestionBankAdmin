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
    public static class MenuItemCallbackMapper
    {
        public static MenuItemCallback ToMenuItemCallback(this BizDC.MenuItemCallback biz)
        {
            var model = new MenuItemCallback();

            if (biz != null)
            {
                model.Action = biz.Action;
                model.Controller = biz.Controller;
                model.LinkType = biz.LinkType;
                model.Name = biz.Name;
                model.RouteName = biz.RouteName;
                model.Url = biz.Url;
                model.StudentOverride = biz.StudentOverride;
                model.InstructorOverride = biz.InstructorOverride;
                model.Target = biz.Target;

                if (!biz.Parameters.IsNullOrEmpty())
                {
                    foreach (var param in biz.Parameters)
                    {
                        model.Parameters.Add(param.Key, param.Value.Value.ToString());
                    }
                }
            }

            return model;
        }

        public static BizDC.MenuItemCallback ToMenuItemCallback(this  MenuItemCallback model)
        {
            var biz = new BizDC.MenuItemCallback();
            
            if (model != null)
            {
                biz.Action = model.Action;
                biz.Controller = model.Controller;
                biz.LinkType = model.LinkType;
                biz.Name = model.Name;
                biz.RouteName = model.RouteName;
                biz.StudentOverride = model.StudentOverride;
                biz.InstructorOverride = model.InstructorOverride;
                biz.Url = model.Url;
                biz.Target = model.Target;
            }
            return biz;
        }
    }
}
