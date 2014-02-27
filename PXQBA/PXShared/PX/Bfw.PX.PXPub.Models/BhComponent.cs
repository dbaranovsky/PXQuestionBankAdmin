using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Bfw.Common.Collections;

namespace Bfw.PX.PXPub.Models
{
    public class BhComponent
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the component.
        /// </summary>
        /// <value>
        /// The name of the component.
        /// </value>
        public string ComponentName { get; set; }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public object Parameters { get; set; }

        /// <summary>
        /// Gets the query string.
        /// </summary>
        /// <returns></returns>
        public string GetQueryString()
        {
            // Handle a dictionary of string->string if we're given one.
            if (Parameters is IDictionary<string, string>)
            {
                var dict = (IDictionary<string, string>)Parameters;
                return (!dict.IsNullOrEmpty())
                    ? String.Join("&", dict.Keys.Map(k => String.Join("=", new string[] { k, dict[k] })).ToArray())
                    : "";
            }

            // Otherwise, handle any given object (e.g., and object  literal) by using its properties and
            // the values of those properties as the keys and values.
            return (Parameters != null)
                ? String.Join("&", Parameters.GetType().GetProperties().Map(p => String.Join("=", new string[] { p.Name, StringIfNotNull(p.GetValue(Parameters, null)) })).ToArray())
                : "";
        }

        /// <summary>
        /// Strings if not null.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        private static string StringIfNotNull(object o)
        {
            return o != null ? o.ToString() : "";
        }

        /// <summary>
        /// 
        /// </summary>
        public string DomainUserSpace { get; set; }

    }
}
