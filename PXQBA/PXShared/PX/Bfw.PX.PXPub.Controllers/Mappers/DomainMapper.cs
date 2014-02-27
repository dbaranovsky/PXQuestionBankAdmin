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
    public static class DomainMapper
    {

        /// <summary>
        /// Convert to a the Domain from a Biz Domain.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static Domain ToDomain(this BizDC.Domain biz)
        {
            return new Domain() { Id = biz.Id, Name = biz.Name };
        }
    }
}
