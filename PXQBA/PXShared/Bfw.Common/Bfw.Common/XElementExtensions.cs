using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Bfw.Common
{
    /// <summary>
    /// Static methods for XML Element. 
    /// </summary>
    public static class XElementExtensions
    {
        /// <summary>
        /// Tries to remove the node from it's parent element.
        /// </summary>
        /// <param name="node">The node to try removing.</param>
        public static void TryRemove(this XNode node)
        {
            if (node != null)
            {
                node.Remove();
            }
        }
        /// <summary>
        /// Attempts to get attribute value of an element
        /// Returns default value if attribute does not exist or element is null
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="attributeName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string AttributeValue(this XElement elem, string attributeName, string defaultValue = "")
        {
            if (elem == null)
                return defaultValue;

            XAttribute attr = elem.Attribute(attributeName);
            
            if (attr == null)
                return defaultValue;

            return attr.Value;
        }


        /// <summary>
        /// Attempts to get attribute value of an element as an integer
        /// Returns default value if attribute does not exist or element is null
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="elemName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int AttributeValueAsInt(this XElement elem, XName elemName, int defaultValue = 0)
        {
            var val = elem.AttributeValue(elemName.ToString(), defaultValue.ToString());
            int ival = defaultValue;
            int.TryParse(val, out ival);
            return ival;
        }
    }
}
