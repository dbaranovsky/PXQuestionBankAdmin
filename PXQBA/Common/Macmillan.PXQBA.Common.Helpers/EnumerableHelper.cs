using System.Collections.Generic;
using System.Linq;

namespace Macmillan.PXQBA.Common.Helpers
{
    /// <summary>
    /// Helper with extensions to Enumerable type
    /// </summary>
    public static class EnumerableHelper
    {
        /// <summary>
        /// Checks if 2 IEnumerable lists are identical
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="source">Source list</param>
        /// <param name="target">Target list</param>
        /// <returns>Check result</returns>
        public static bool IsCollectionEqual<T>(this IEnumerable<T> source, IEnumerable<T> target)
        {
            return ((source.Count() == target.Count()) && (!source.Except(target).Any()) && (!target.Except(source).Any()));
        }
    }
}
