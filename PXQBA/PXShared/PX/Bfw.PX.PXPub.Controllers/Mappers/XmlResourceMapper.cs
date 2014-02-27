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
    public static class XmlResourceMapper
    {
        /// <summary>
        /// Map a ResourceDocument model to an xml resource business object.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Biz.DataContracts.XmlResource ToXmlResource(this ResourceDocument model)
        {
            string retval = "";
            string url = model.path;

            if (string.IsNullOrEmpty(model.path))
            {
                var resId = Guid.NewGuid().ToString("N");
                url = string.Format("Templates/Data/XmlResources/Documents/{0}.pxres", resId);
                retval = resId;
            }

            var resource = new Biz.DataContracts.XmlResource
            {
                Status = Biz.DataContracts.ResourceStatus.Normal,
                Url = url,
                EntityId = model.enrollmentId,
                Title = model.title,
                Body = model.body
            };

            resource.ExtendedProperties.Add(Biz.DataContracts.ResourceExtendedProperty.WordCount.ToString(), model.wordCount);
            resource.ExtendedProperties.Add("Status", "saved");
            resource.ExtendedId = retval;

            return resource;
        }

    }
}
