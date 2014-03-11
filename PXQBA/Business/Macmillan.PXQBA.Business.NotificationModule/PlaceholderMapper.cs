using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Business.NotificationModule
{
    /// <summary>
    /// Construct placeholders
    /// </summary>
    public class PlaceholderMapper
    {
        public const string QuestionIdToken = "[QUESTIONID]";
        public const string UserNameToken = "[USERNAME]";

        /// <summary>
        /// Gets or sets question id
        /// </summary>
        public string QuestionId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets user name
        /// </summary>
        public string UserName
        {
            get;
            set;
        }

        private Dictionary<string, string> mappings;

        /// <summary>
        /// Execute creating placeholder
        /// </summary>
        public Dictionary<string, string> Mappings
        {
            get
            {
                if (mappings == null)
                {
                    mappings = new Dictionary<string, string>
                    {
                        {QuestionIdToken, QuestionId},
                        {UserNameToken, UserName},
                    };
                }
                return mappings;
            }
        }
    }
}
