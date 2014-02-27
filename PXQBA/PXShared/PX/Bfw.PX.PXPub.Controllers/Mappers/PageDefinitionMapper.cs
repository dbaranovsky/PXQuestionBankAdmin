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
    public static class PageDefinitionMapper
    {
        public static PageDefinition ToPageDefinition(this BizDC.PageDefinition biz)
        {
            var model = new PageDefinition();

            if (biz == null)
            {
                return model;
            }

            model.Name = biz.Name;
            model.CustomDivs = biz.CustomDivs;
            foreach (var zone in biz.Zones)
            {
                model.Zones.Add(zone.ToZoneItem());
            }

            return model;
        }

        public static BizDC.PageDefinition ToPageDefinition(this PageDefinition model)
        {
            var biz = new BizDC.PageDefinition();

            if (model == null)
            {
                return biz;
            }

            biz.Name = model.Name;
            biz.CustomDivs = model.CustomDivs;
            foreach (var zone in model.Zones)
            {
                biz.Zones.Add(zone.ToZoneItem());
            }

            return biz;
        }
    }
}
