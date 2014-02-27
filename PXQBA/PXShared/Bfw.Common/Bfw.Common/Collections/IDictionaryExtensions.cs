using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;


namespace Bfw.Common.Collections
{
    /// <summary>
    /// Provides basic list and IEnumerable manipulations.  Of note are the implementations of
    /// Filter, Map, and Reduce.
    /// </summary>
    public static class IDictionaryExtentsions
    {
        /// <summary>
        /// Attempts to get an element with from dictionary, returns default value if key does not exist
        /// </summary>
        public static T GetValue<T>(this IDictionary<string, T> dictionary, string key, T defaultValue)
        {
            if (dictionary != null)
            {
                if (dictionary.ContainsKey(key))
                {
                    return dictionary[key];
                }
            }
            return defaultValue;
        }
    }
}