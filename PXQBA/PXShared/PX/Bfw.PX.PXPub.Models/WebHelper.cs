using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Bfw.PX.PXPub.Models
{
    public class WebHelper
    {
        /// <summary>
        /// I private variable for the holding the _serializer
        /// </summary>
        private static JavaScriptSerializer _serializer = new JavaScriptSerializer();

        /// <summary>
        /// Covers to a json.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        public static string ToJson(object o)
        {
            return new JavaScriptSerializer().Serialize(o).Replace("\"\\/Date(", "new Date(").Replace(")\\/\"", ")");
        }
    }
}
