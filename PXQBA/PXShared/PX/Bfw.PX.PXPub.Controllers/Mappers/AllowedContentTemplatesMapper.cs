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
    public static class AllowedContentTemplateMapper
    {
        public static AllowedContentTemplate ToAllowedContentTemplate(this BizDC.RelatedTemplate biz)
        {
            var model = new AllowedContentTemplate();

            model.TemplateID = biz.TemplateID;
            model.TemplateName = biz.DisplayName;
            return model;
        }
    }
}
