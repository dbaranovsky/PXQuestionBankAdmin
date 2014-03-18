using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Common.Helpers.Constants
{
    /// <summary>
    /// Represents configuration file key values
    /// </summary>
    class ConfigurationKeys
    {
        /// <summary>
        /// Represents the key for Mars login page
        /// </summary>
        public const string MarsPathLoginUrl = "MarsPathLogin";

        /// <summary>
        /// Key for brain honey default password
        /// </summary>
        public const string BrainhoneyDefaultPassword = "BrainhoneyDefaultPassword";

        /// <summary>
        /// Key for number of question on the page in question list
        /// </summary>
        public const string QuestionPerPage = "QuestionPerPage";

        /// <summary>
        /// Key for HTS converter page
        /// </summary>
        public const string HtsConverter = "HtsConverter";

        /// <summary>
        /// Key for Graph converter page
        /// </summary>
        public const string FmaGraphConverter = "FmaGraphConverter";

        /// <summary>
        /// Key for question types
        /// </summary>
        public static string QuestionTypes { get; set; }
    }
}
