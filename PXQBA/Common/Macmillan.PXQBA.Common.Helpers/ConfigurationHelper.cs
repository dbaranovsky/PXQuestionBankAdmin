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

        /// <summary>
        /// Gets BH default password from config
        /// </summary>
        /// <returns>Default password</returns>
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
            var questionPerPage = ConfigurationManager.AppSettings[ConfigurationKeys.QuestionPerPage];
            if (!string.IsNullOrEmpty(questionPerPage))
            {
                return Int32.Parse(questionPerPage);
            }

            return 50;
        }

        /// <summary>
        /// Gets converter url for HTS questions
        /// </summary>
        /// <returns>HTS url</returns>
        public static string GetHTSConverterUrl()
        {
            var url = ConfigurationManager.AppSettings[ConfigurationKeys.HtsConverter];
            if (!string.IsNullOrEmpty(url))
            {
                return url;
            }
            return "http://dev.px.bfwpub.com/PxHTS/PxPlayer.ashx";
        }

        /// <summary>
        /// Gets Fma Graph converter url
        /// </summary>
        /// <returns>Fma Graph url</returns>
        public static string GetFmaGraphConverterUrl()
        {
            var url = ConfigurationManager.AppSettings[ConfigurationKeys.FmaGraphConverter];
            if (!string.IsNullOrEmpty(url))
            {
                return url;
            }
            return "http://dev.px.bfwpub.com/PxEG/authormode2.aspx";
        }

        /// <summary>
        /// Gets the list of available question types
        /// </summary>
        /// <returns>Question types list</returns>
        public static Dictionary<string, string> GetQuestionTypes()
        {
            var settingString = ConfigurationManager.AppSettings[ConfigurationKeys.QuestionTypes];
            if (!string.IsNullOrEmpty(settingString))
            {
                return settingString.Split('|').Select(type => type.Split(':')).ToDictionary(parts => parts[0], parts => parts[1]);
            }
            return new Dictionary<string, string>
            {
                {"choice", "Multiple Choice"},
                {"text", "Short Answer"},
                {"essay", "Essay"},
                {"match", "Matching"},
                {"answer", "Multiple Answer"},
                {"HTS","Advanced Question"},
                {"FMA_GRAPH", "Graph Exercise"}
            };
        }
    }
}
