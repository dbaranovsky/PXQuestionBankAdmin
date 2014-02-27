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
    public static class AccountMapper
    {
        /// <summary>
        /// Maps a UserInfo object to an Account object.
        /// </summary>
        /// <param name="biz">UserInfo business object.</param>
        /// <returns>
        /// Account model.
        /// </returns>
        public static Account ToAccount(this BizDC.UserInfo biz)
        {
            var model = new Account();

            if (null != biz)
            {
                model.DisplayName = string.Format("{0} {1}", biz.FirstName, biz.LastName);
                model.Username = biz.Username;
                model.Password = biz.Password;
            }

            return model;
        }
    }
}
