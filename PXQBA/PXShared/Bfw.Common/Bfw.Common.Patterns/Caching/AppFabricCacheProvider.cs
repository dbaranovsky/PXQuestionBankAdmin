using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Bfw.Common.Collections;
using System.Web;
using Microsoft.ApplicationServer.Caching;

using Bfw.Common.Caching;
using System.Collections.ObjectModel;

namespace Bfw.Common.Patterns.Caching
{
    public class AppFabricCacheProvider : ICacheProvider
    {
        /// <summary>
        /// Configuration section that stores all of the DLAP connection information.
        /// </summary>
        private CachingResponseProxySection configuration = null;

        #region Constructor
        public AppFabricCacheProvider(IDataCacheFactoryProvider dataCacheFactoryProvider)
        {
            CacheFactory = dataCacheFactoryProvider.CurrentDataCacheFactory;

            if (CacheFactory != null)
            {
                try
                {
                    if (dataCacheFactoryProvider.ApplicationCacheEnabled)
                    {
                        Cache = CacheFactory.GetCache(dataCacheFactoryProvider.CacheName);
                    }
                    else
                    {
                        Cache = null;
                    }
                }
                catch
                {
                    Cache = null;
                }
                RegionName = dataCacheFactoryProvider.RegionName;
            }
        }
        #endregion

        [ThreadStatic]
        public static bool? IsCacheDisabledByCookieAsync;

        #region ICacheProvider Implementation

        public bool Disabled { get; set; }

        public void Store(string key, object item, CacheSettings settings)
        {
            if (IsCacheEnabled())
            {
                StoreSerialized(key, RegionName, item, settings);
            }
        }

        public void Store(string key, object item, CacheSettings settings, string region, string tag = "")
        {
            if (IsCacheEnabled())
            {
                List<string> tags = null;
                if (!String.IsNullOrEmpty(tag))
                {
                    tags = new List<string> { tag };
                }
                Store(key, item, settings, region, tags);
            }
        }

        public void Store(IDictionary<string, object> items, CacheSettings settings, string region, string tag = "")
        {
            if (IsCacheEnabled())
            {
                foreach (var item in items)
                {
                    //Note: there's no bulk put in AppFabric 1.1. This is here in case one day there is or we move to greener pastures
                    Store(item.Key, item.Value, settings, region, tag);
                }
            }
        }

        public void Store(string key, object item, CacheSettings settings, string region, List<string> tags)
        {
            if (IsCacheEnabled())
            {
                //we store all regions names in a static array as a performance enchancement
                //when we know a region already exists, we know that we dont need to attempt to create it
                if (regionNameList.Contains(region))
                {
                    StoreSerialized(key, region, item, settings, tags);
                }
                else
                {
                    Cache.CreateRegion(region);
                    StoreSerialized(key, region, item, settings, tags);
                    regionNameList.Add(region);
                }
            }
        }

        public object Remove(string key)
        {
            return Remove(key, RegionName);
        }

        public object Remove(string key, string region)
        {
            object obj = null;
            if (IsCacheEnabled())
            {
                obj = FetchSerialized(key, region);
                Cache.Remove(key, region);
            }
            return obj;
        }

        public IDictionary<string, object> Remove(List<string> keys, string region)
        {
            if (!keys.IsNullOrEmpty())
            {
                return keys.ToDictionary(k => k, k => Remove(k, region));
            }
            return null;
        }

        public IDictionary<string, object> RemoveByTag(string tag, string region)
        {
            IDictionary<string, object> objs = null;
            if (Cache != null && IsCacheEnabled())
            {
                objs = FetchSerializedByTag(tag, region);
                foreach (var key in objs.Keys)
                {
                    Cache.Remove(key, region);
                }
            }
            return objs;
        }

        public void ClearRegion(string regionName)
        {
            if (Cache != null && IsCacheEnabled())
            {
                Cache.ClearRegion(regionName);
            }
        }

        public object Fetch(string key)
        {
            return Fetch(key, RegionName);
        }

        public object Fetch(string key, string region)
        {
            object obj = null;
            if (IsCacheEnabled())
            {
                obj = FetchSerialized(key, region);
            }
            return obj;
        }

        public IDictionary<string, object> Fetch(List<string> keys, string region)
        {
            if (IsCacheEnabled())
            {
                var formatter = new BinaryFormatter();
                var results = Cache.BulkGet(keys, region);
                if (!results.IsNullOrEmpty())
                {
                    return results.ToDictionary(r => r.Key, r => { return FetchSerialized(r.Key, region); });
                }
            }
            return null;
        }

        public IDictionary<string, object> FetchByTag(string tag, string region)
        {
            if (IsCacheEnabled())
            {
                return FetchSerializedByTag(tag, region);
            }
            return null;
        }

        public IDictionary<string, object> FetchByAllTags(List<string> tags, string region)
        {
            if (IsCacheEnabled())
            {
                return FetchSerializedByAllTags(tags, region);
            }
            return null;
        }

        public IDictionary<string, object> FetchByAnyTag(List<string> tags, string region)
        {
            if (IsCacheEnabled())
            {
                return FetchSerializedByAnyTag(tags, region);
            }
            return null;
        }

        public List<string> GetCacheItemTags(string itemKey, string region)
        {
            var item = Cache.GetCacheItem(itemKey, region);
            if (item != null)
            {
                var tags = new List<string>();
                foreach (var t in item.Tags)
                {
                    tags.Add(t.ToString());
                }
                return tags;
            }
            return null;
        }

        public virtual bool IsCacheDisabledByCookie()
        {
            if (!IsCacheDisabledByCookieAsync.HasValue)
            {
                return (HttpContext.Current.Request["cachestat"] == "off");
            }
            else
            {
                return IsCacheDisabledByCookieAsync.Value;
            }
        }
        #endregion

        #region Protected Members
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

        protected DataCacheFactory CacheFactory { get; set; }

        protected DataCache Cache { get; set; }

        protected string RegionName { get; set; }

        protected static List<string> regionNameList = new List<string>();

        protected IDictionary<string, object> FetchSerializedByTag(string tag, string region)
        {
            return FetchSerializedByTagsCore(() => Cache.GetObjectsByTag(new DataCacheTag(tag), region), region);
        }

        protected IDictionary<string, object> FetchSerializedByAllTags(List<string> tags, string region)
        {
            var allTags = tags.Select(t => new DataCacheTag(t));
            return FetchSerializedByTagsCore(() => Cache.GetObjectsByAllTags(allTags, region), region);
        }

        protected IDictionary<string, object> FetchSerializedByAnyTag(List<string> tags, string region)
        {
            var allTags = tags.Select(t => new DataCacheTag(t));
            return FetchSerializedByTagsCore(() => Cache.GetObjectsByAnyTag(allTags, region), region);
        }

        protected object FetchSerialized(string key, string region)
        {
            object obj = null;
            try
            {
                var bytes = Cache.Get(key, region) as byte[];

                if (bytes != null)
                {
                    var formatter = new BinaryFormatter();
                    var data = new MemoryStream(bytes);

                    obj = formatter.Deserialize(data);
                }
            }
            catch (DataCacheException ex)
            {
                if (ex.ErrorCode == 5)
                {
                    //error is that region does not exist, so attempt to create it
                    try
                    {
                        Cache.CreateRegion(region);
                    }
                    catch
                    {
                        //swallow any other exception
                    }
                }
            }
            catch
            {
                //swallow any other exception
            }
            return obj;
        }

        protected void StoreSerialized(string key, string region, object item, CacheSettings settings, List<string> tags = null)
        {
            try
            {
                var timeout = new TimeSpan(0, 0, settings.Duration);
                var formatter = new BinaryFormatter();
                var data = new MemoryStream();

                formatter.Serialize(data, item);

                data.Seek(0, SeekOrigin.Begin);

                var bytes = data.GetBuffer();

                if (!tags.IsNullOrEmpty())
                {
                    var dataCacheTags = tags.Select(t => new DataCacheTag(t)).ToList();
                    Cache.Put(key, bytes, timeout, dataCacheTags, region);
                }
                else
                {
                    Cache.Put(key, bytes, timeout, region);
                }

            }
            catch (DataCacheException ex)
            {
                if (ex.ErrorCode == 5)
                {
                    //error is that region does not exist, so attempt to create it
                    try
                    {
                        Cache.CreateRegion(region);
                    }
                    catch
                    {
                        //swallow any other exception
                    }
                }
            }
            catch
            {
                throw new Exception("Error deserializing data");
            }
        }

        protected virtual bool IsDebug()
        {
            bool debug = true;

            var compilation = ConfigurationManager.GetSection("system.web/compilation") as System.Web.Configuration.CompilationSection;

            if (compilation != null)
            {
                debug = compilation.Debug;
            }

            return debug;
        }

        protected bool IsCacheEnabled()
        {
            return (!IsCacheDisabledByCookie() && !IsDebug() && Configuration.ObjectCacheEnabled && Cache != null);
        }
        #endregion

        #region Private Members
        private IDictionary<string, object> FetchSerializedByTagsCore(Func<IEnumerable<KeyValuePair<string, object>>> action, string region)
        {
            var objs = new Dictionary<string, object>();
            try
            {
                var objects = action();
                if (objects != null)
                {
                    var formatter = new BinaryFormatter();
                    foreach (var keyValuePair in objects)
                    {
                        var bytes = keyValuePair.Value as byte[];
                        var data = new MemoryStream(bytes);
                        var obj = formatter.Deserialize(data);
                        objs.Add(keyValuePair.Key, obj);
                    }
                }
            }
            catch (DataCacheException ex)
            {
                if (ex.ErrorCode == 5)
                {
                    //error is that region does not exist, so attempt to create it
                    try
                    {
                        Cache.CreateRegion(region);
                    }
                    catch
                    {
                        //swallow any other exception
                    }
                }
            }
            catch
            {
                //swallow any other exception
            }
            return objs;
        }
        #endregion
    }
}
