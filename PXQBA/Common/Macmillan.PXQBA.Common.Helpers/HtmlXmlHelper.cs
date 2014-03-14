using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Macmillan.PXQBA.Common.Helpers
{ /// <summary>
    /// This class containshelper methods to modify html and xml.
    /// </summary>
    public static class HtmlXmlHelper
    {
        /// <summary>
        /// Switches the name of the attribute.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="newVal">The new val.</param>
        public static void SwitchAttributeName(XElement element, string from, string to, object newVal)
        {
            try
            {
                var attr = element.Attribute(from);
                element.Add(new XAttribute(to, newVal != null ? newVal : attr.Value));
                attr.Remove();
            }
            catch
            {
                
            }
        }

        /// <summary>
        /// This method will clean up tags which are not required in HTS Preview when interaction data is wrapped and sent to HTS Player
        /// </summary>
        /// <param name="result">result string that need to be formatted</param>
        /// <returns>final result string after clean up of CDATA tag and proper closing of meta, link and span tags></returns>
        public static string CleanupHtmlString(string result)
        {
            result = Regex.Replace(result, "<meta(.*?)>", "<meta$1/>");
            result = Regex.Replace(result, "<meta(.*?)//>", "<meta$1/>");
            result = Regex.Replace(result, "<link(.*?)>", "<link$1/>");
            result = Regex.Replace(result, "<link(.*?)//>", "<link$1/>");
            result = Regex.Replace(result, "<span _fck_bookmark=(.*?)>", "<span _fck_bookmark=$1/>");
            result = Regex.Replace(result, "<span _fck_bookmark=(.*?)//>", "<span _fck_bookmark=$1/>");

            string pattern = @"<!\[CDATA\[(.*?)\]\]>";
            do
            {
                foreach (Match match in Regex.Matches(result, pattern))
                {
                    result = Regex.Replace(result, pattern, "$1", RegexOptions.Multiline | RegexOptions.Singleline);
                }
            }
            while (Regex.Matches(result, pattern).Count > 0);

            result = result.Replace("&nbsp;", " ");
            return result;
        }

    }
}
