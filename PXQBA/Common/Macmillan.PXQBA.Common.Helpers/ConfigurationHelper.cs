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

        /// <summary>
        /// Get question per page parameter from config
        /// </summary>
        /// <returns>Question per page</returns>
        public static int GetQuestionPerPage()
        {
            const int defaultValue = 50;

            var questionPerPage = ConfigurationManager.AppSettings[ConfigurationKeys.QuestionPerPage];
            if (!string.IsNullOrEmpty(questionPerPage))
            {
                return Int32.Parse(questionPerPage);
            }

            return defaultValue;
        }
    }
}
