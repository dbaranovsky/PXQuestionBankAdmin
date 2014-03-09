using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macmillan.PXQBA.Common.Helpers.Constants;

namespace Macmillan.PXQBA.Common.Helpers
{
    public static class ConfigurationHelper
    {
        /// <summary>
        /// Gets Mars login url from config
        /// </summary>
        /// <returns>Mars login url</returns>
        public static string GetMarsLoginUrl()
        {
            var url = ConfigurationManager.AppSettings[ConfigurationKeys.MarsPathLoginUrl];
            if (!string.IsNullOrEmpty(url))
            {
                return url;
            }
            return "http://dev.activation.macmillanhighered.com/account/logon?target={0}";
        }

        public static string GetBrainhoneyDefaultPassword()
        {
            var url = ConfigurationManager.AppSettings[ConfigurationKeys.BrainhoneyDefaultPassword];
            if (!string.IsNullOrEmpty(url))
            {
                return url;
            }
            return "fakepassword";
        }
    }
}
