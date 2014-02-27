using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Account.Models
{
    public class UserLogin
    {
        #region Properties

        /// <summary>
        /// Url to which the authentication request will be posted
        /// </summary>
        public string AuthenticationUrl { get; set; }

        /// <summary>
        /// Url to redirect the user to upon successful login
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Message to display to the user depending on their request status
        /// </summary>
        public string Message { get; set; }

        #endregion
    }
}
