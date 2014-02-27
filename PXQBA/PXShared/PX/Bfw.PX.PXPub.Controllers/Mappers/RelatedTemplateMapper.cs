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
    public static class RelatedTemplateMapper
    {
        public static RelatedTemplate ToRelatedTemplate(this BizDC.RelatedTemplate biz)
        {
            var model = new RelatedTemplate();

            model.Id = biz.TemplateID;
            model.DisplayName = biz.DisplayName;
            model.Message = biz.Message;

            return model;
        }
    }
}
