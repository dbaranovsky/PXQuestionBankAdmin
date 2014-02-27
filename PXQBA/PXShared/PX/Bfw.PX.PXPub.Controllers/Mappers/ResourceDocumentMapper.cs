using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Common.Collections;
using Bfw.Common;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class ResourceDocumentMapper
    {
        /// <summary>
        /// Maps a generic Resource from the business layer to a ResourceDocument type.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static ResourceDocument ToResourceDocument(this BizDC.Resource biz)
        {
            return new ResourceDocument { body = biz.GetStream().AsString(), title = biz.Name, url = biz.Url, enrollmentId = biz.EntityId, LastModifiedDate = biz.ModifiedDate };
        }
    }
}
