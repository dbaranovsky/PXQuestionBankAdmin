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
        /// CQ request string with placeholders
        /// </summary>
        public const string CQScriptString = "if(typeof CQ ==='undefined')CQ = window.parent.CQ; CQ.questionInfoList['{0}'] = {{ divId: '{1}', version: '{2}', mode: '{3}', question: {{ body: '{4}', data: {5}}}, response: {{ pointspossible: '{6}', pointsassigned: '{7}'}} }}";
        
    }
}
