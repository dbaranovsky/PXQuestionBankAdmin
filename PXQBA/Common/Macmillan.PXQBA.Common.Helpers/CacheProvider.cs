using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Macmillan.PXQBA.Common.Helpers.Constants;

namespace Macmillan.PXQBA.Common.Helpers
{
    public class CacheProvider
    {
        private static readonly MemoryCache Cache = MemoryCache.Default;

        private static readonly CacheItemPolicy Policy = new CacheItemPolicy
        {
            AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(ConfigurationHelper.GetCacheTimeout())
        };
        public static void AddCurrentTitleId(string titleId)
        {
            Cache.Add(new CacheItem(CacheKeys.ProductCourseId, titleId), Policy);
        }

        public static string GetCurrentTitleId()
        {
            if (Cache[CacheKeys.ProductCourseId] != null)
            {
                return Cache[CacheKeys.ProductCourseId].ToString();
            }
            throw new Exception("Current title is not found.");
        }

        public static void Add(string key, object value)
        {
            Cache.Add(new CacheItem(key, value), Policy);
        }

        public static object Get(string key)
        {
            return Cache[key];
        }
    }
}
