using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Common.Caching
{
    public class NullCacheProvider : ICacheProvider
    {
        public bool Disabled
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

        public void Store(string key, object item, CacheSettings settings)
        {
        }

        public void Store(string key, object item, CacheSettings settings, string region, string tag)
        {
        }

        public void Store(string key, object item, CacheSettings settings, string region, List<string> tags)
        {
        }

        public object Remove(string key)
        {
            return null;
        }

        public object Remove(string key, string region)
        {
            return null;
        }

        public object Fetch(string key)
        {
            return null;
        }

        public object Fetch(string key, string region)
        {
            return null;
        }

        public IDictionary<string, object> RemoveByTag(string tag, string region)
        {
            return null;
        }

        public IDictionary<string, object> FetchByTag(string tag, string region)
        {
            return null;
        }


        public void ClearRegion(string regionName)
        {

        }


        public void Store(IDictionary<string, object> items, CacheSettings settings, string region, string tag = "")
        {

        }

        public IDictionary<string, object> Remove(List<string> keys, string region)
        {
            return null;
        }

        public IDictionary<string, object> Fetch(List<string> keys, string region)
        {
            return null;
        }


        public IDictionary<string, object> FetchByAllTags(List<string> tags, string region)
        {
            return null;
        }

        public IDictionary<string, object> FetchByAnyTag(List<string> tags, string region)
        {
            return null;
        }

        public List<string> GetCacheItemTags(string itemKey, string region)
        {
            return null;
        }


        public bool IsCacheDisabledByCookie()
        {
            return false;
        }
    }
}
