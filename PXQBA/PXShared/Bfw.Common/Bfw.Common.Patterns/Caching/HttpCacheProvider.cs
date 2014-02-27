using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Web;
using System.Web.Caching;
using System.Text.RegularExpressions;
using System.Configuration;

using Bfw.Common.Caching;
using Bfw.Common.Collections;

namespace Bfw.Common.Patterns.Caching
{
    /// <summary>
    /// Manages creating and outputing traces
    /// </summary>
    public class HttpCacheProvider : ICacheProvider
    {
        #region Properties

        /// <summary>
        /// Configuration section that stores all of the DLAP connection information.
        /// </summary>
        private CachingResponseProxySection configuration = null;

        /// <summary>
        /// Configuration section that stores all of the Cache settings information.
        /// </summary>
        /// <value>Set <see cref="configuration"/></value>
        protected CachingResponseProxySection Configuration
        {
            get
            {
                if (this.configuration == null)
                {
                    this.configuration = ConfigurationManager.GetSection("pxCacheManager") as CachingResponseProxySection;
                }

                return this.configuration;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Maps the CachePriority to a value in System.Web.CacheItemPriority.
        /// </summary>
        /// <param name="priority">Priority of the cached item.</param>
        /// <returns>Corresponding value from System.Web.CacheItemPriority.</returns>
        private CacheItemPriority MapPriority(CachePriority priority)
        {
            CacheItemPriority mapped;

            switch (priority)
            {
                case CachePriority.High:
                    mapped = CacheItemPriority.High;
                    break;

                case CachePriority.Low:
                    mapped = CacheItemPriority.Low;
                    break;

                default:
                    mapped = CacheItemPriority.Normal;
                    break;
            }

            return mapped;
        }


        /// <summary>
        /// Check if Caching is disabled by cookie.
        /// </summary>
        public bool IsCacheDisabledByCookie()
        {
            return (HttpContext.Current.Request["cachestat"] == "off");
        }

        public bool IsCacheEnabled()
        {
            return (!IsCacheDisabledByCookie() && Configuration.ObjectCacheEnabled);
        }
        #endregion

        #region ICacheProvider Members

        /// <summary>
        /// Flag to disable caching (overwrites the config setting)
        /// </summary>
        public bool Disabled
        {
            set;
            get;
        }

        /// <summary>
        /// Stores item in cache with the specified unique item key.
        /// </summary>
        /// <param name="key">Unique item key in cache.</param>
        /// <param name="data">Object to store in cache.</param>
        /// <param name="settings">settings to be used when access cache</param>
        public void Store(string key, object data, CacheSettings settings)
        {
            if (IsCacheEnabled())
            {
                var priority = MapPriority(settings.Priority);
                var staticExpiration = DateTime.UtcNow.AddSeconds(settings.Duration);
                var slidingExpiration = new TimeSpan(0, 0, settings.Duration);

                switch (settings.Aging)
                {
                    case AgingMechanism.Static:
                        slidingExpiration = System.Web.Caching.Cache.NoSlidingExpiration;
                        break;

                    case AgingMechanism.Sliding:
                        staticExpiration = System.Web.Caching.Cache.NoAbsoluteExpiration;
                        break;

                    default:
                        slidingExpiration = System.Web.Caching.Cache.NoSlidingExpiration;
                        staticExpiration = System.Web.Caching.Cache.NoAbsoluteExpiration;
                        break;
                }

                HttpContext.Current.Cache.Insert(key, data, null, staticExpiration, slidingExpiration, priority, null);
            }
        }

        private class CachedItem
        {
            public object Item { get; set; }
            public string Region { get; set; }
            public List<string> Tags { get; set; } 
        }
        public void Store(string key, object item, CacheSettings settings, string region, string tag = "")
        {
            var dataWithTags = new CachedItem()
            {
                Item = item,
                Region = region,
                Tags = new List<string>() { tag }
            };
            Store(key, dataWithTags, settings);
        }

        /// <summary>
        /// If found, removes an item with the specified key from cache.
        /// </summary>
        /// <param name="key">Unique item key in cache.</param>
        /// <returns>Returns the object that was removed from cache.</returns>
        public Object Remove(string key)
        {
            Object obj = null;
            if (IsCacheEnabled())
            {
                obj = HttpContext.Current.Cache.Remove(key);
            }

            return obj;
        }

        public Object Remove(string key, string region)
        {
            return Remove(key);
        }


        public void ClearRegion(string regionName)
        {
            IDictionary<string, object> itemList = new Dictionary<string, object>();

            if (IsCacheEnabled())
            {
                IDictionaryEnumerator enumerator = HttpContext.Current.Cache.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var cachedObject = enumerator.Value as CachedItem;
                    if (cachedObject != null)
                    {
                        if (cachedObject.Region == regionName)
                        {
                            itemList.Add(enumerator.Key.ToString(), HttpContext.Current.Cache.Remove(enumerator.Key.ToString()));
                        }
                    }
                }
            }
        }

        #endregion


        public IDictionary<string, object> RemoveByTag(string tag, string region)
        {
            IDictionary<string, object> itemList = new Dictionary<string, object>();

            if (IsCacheEnabled())
            {
                IDictionaryEnumerator enumerator = HttpContext.Current.Cache.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var cachedObject = enumerator.Value as CachedItem;
                    if (cachedObject != null)
                    {
                        if (cachedObject.Tags.Contains(tag))
                        {
                            itemList.Add(enumerator.Key.ToString(), HttpContext.Current.Cache.Remove(enumerator.Key.ToString()));    
                        }
                    }
                }
            }

            return itemList;
        }

        public IDictionary<string, object> FetchByTag(string tag, string region)
        {
            IDictionary<string, object> itemList = new Dictionary<string, object>();

            if (IsCacheEnabled())
            {
                IDictionaryEnumerator enumerator = HttpContext.Current.Cache.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var cachedObject = enumerator.Value as CachedItem;
                    if (cachedObject != null)
                    {
                        if (cachedObject.Tags.Contains(tag))
                        {
                            itemList.Add(enumerator.Key.ToString(), HttpContext.Current.Cache[enumerator.Key.ToString()]);
                        }
                    }
                }
            }

            return itemList;
        }


        /// <summary>
        /// Removes items from the cache based on the specified regular expression pattern.
        /// </summary>
        /// <param name="pattern">Regular expression pattern to search cache keys.</param>
        /// <returns>Collection objects that were removed from cache.</returns>
        private IDictionary<string, object> RemoveByPattern(String pattern)
        {
            IDictionary<string, object> itemList = new Dictionary<string, object>();

            if (IsCacheEnabled())
            {
                IDictionaryEnumerator enumerator = HttpContext.Current.Cache.GetEnumerator();
                Regex regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
                while (enumerator.MoveNext())
                {
                    if (regex.IsMatch(enumerator.Key.ToString()))
                    {
                        itemList.Add(enumerator.Key.ToString(), HttpContext.Current.Cache.Remove(enumerator.Key.ToString()));
                    }
                }
            }

            return itemList;
        }

        /// <summary>
        /// Fetch items from the cache based on the specified regular expression pattern.
        /// </summary>
        /// <param name="pattern">Regular expression pattern to search cache keys.</param>
        /// <returns>Collection objects that were removed from cache.</returns>
        private IDictionary<string, object> FetchByPattern(String pattern)
        {
            IDictionary<string, object> itemList = new Dictionary<string, object>();

            if (IsCacheEnabled())
            {
                IDictionaryEnumerator enumerator = HttpContext.Current.Cache.GetEnumerator();
                Regex regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
                while (enumerator.MoveNext())
                {
                    if (regex.IsMatch(enumerator.Key.ToString()))
                    {
                        itemList.Add(enumerator.Key.ToString(), HttpContext.Current.Cache[enumerator.Key.ToString()]);
                    }
                }
            }

            return itemList;
        }
          /// <summary>
        /// Attempts to retrieve a cached object.
        /// </summary>
        /// <param name="key">Unique item key in cache.</param>
        /// <returns>Returns cached object if found, null otherwise.</returns>
        public object Fetch(string key)
        {
            object item = null;
            if (IsCacheEnabled())
            {
                item = HttpContext.Current.Cache[key];
                var cachedItemTagged = item as CachedItem;
                if (cachedItemTagged  != null)
                {
                    return cachedItemTagged.Item;
                }
            }
            return item;
        }
        
        public object Fetch(string key, string region)
        {
            return Fetch(key);
        }


        public void Store(IDictionary<string, object> items, CacheSettings settings, string region, string tag = "")
        {
            foreach (var item in items)
            {
                Store(item.Key,item.Value,settings, region, tag);
            }
        }

        public void Store(string key, object item, CacheSettings settings, string region, List<string> tags)
        {
            //to be implemented if needed
        }

        public IDictionary<string, object> Remove(List<string> keys, string region)
        {
            return keys.ToDictionary(k => k, k => Remove(k, region));
        }

        public IDictionary<string, object> Fetch(List<string> keys, string region)
        {
            return keys.ToDictionary(k => k, k => Fetch(k, region));
        }

        public IDictionary<string, object> FetchByAllTags(List<string> tags, string region)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, object> FetchByAnyTag(List<string> tags, string region)
        {
            throw new NotImplementedException();
        }

        public List<string> GetCacheItemTags(string itemKey, string region)
        {
            throw new NotImplementedException();
        }
    }    
}