using System.Collections.Generic;
using System.Linq;

namespace Macmillan.PXQBA.Common.Helpers
{
    public static class EnumerableHelper
    {
        public static bool IsCollectionEqual<T>(this IEnumerable<T> source, IEnumerable<T> targer)
        {
            return ((source.Count() == targer.Count()) && (!source.Except(targer).Any()) && (!targer.Except(source).Any()));
        }
    }
}
