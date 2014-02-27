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
    public static class UserMessageMapper
    {

        /// <summary>
        /// Maps a Message business object to a Message model.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static UserMessage ToMessage(this BizDC.Message biz)
        {
            return new UserMessage
                       {
                           DisplayAuthor = (biz.Author == null) ? "Unknown" : biz.Author.FormattedName,
                           Body = System.Web.HttpUtility.HtmlDecode(biz.Body),
                           Date = biz.Date
                       };
        }
    }
}
