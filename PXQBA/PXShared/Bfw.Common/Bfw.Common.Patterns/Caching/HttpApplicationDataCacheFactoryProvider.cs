using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Configuration;
using Bfw.Common.Caching;
using Microsoft.ApplicationServer.Caching;

namespace Bfw.Common.Patterns.Caching
{
    public class HttpApplicationDataCacheFactoryProvider : IDataCacheFactoryProvider
    {
        public HttpApplicationDataCacheFactoryProvider()
        {
            var factoryName = ConfigurationManager.AppSettings["DataCacheFactoryName"];
            var cacheName = ConfigurationManager.AppSettings["DataCacheName"];
            var regionName = ConfigurationManager.AppSettings["CacheRegionName"];

            var cacheConfiguration = ConfigurationManager.GetSection("pxCacheManager") as CachingResponseProxySection;


            var factory = System.Web.HttpContext.Current.Application[factoryName] as DataCacheFactory;

            if (factory == null)
            {
                try
                {
                    if (cacheConfiguration.ObjectCacheEnabled)
                    {
                        factory = new DataCacheFactory();
                    }
                }
                catch
                {
                    //not alot we can do if there is a comms issue
                }

                if (factory != null)
                {
                    System.Web.HttpContext.Current.Application[factoryName] = factory;

                    try
                    {
                        if (cacheConfiguration.ObjectCacheEnabled)
                        {
                            factory.GetCache(cacheName).CreateRegion(regionName);
                        }
                    }
                    catch
                    {
                        //region already exists, swallow exception
                    }
                }
            }
            else
            {
                try
                {
                    if (cacheConfiguration.ObjectCacheEnabled)
                    {
                        var defaultCache = factory.GetDefaultCache();
                    }
                }
                catch (DataCacheException ex)
                {
                    //probably a coms issue, we should null out the factory so that it will attempt to reconnect in the future
                    System.Web.HttpContext.Current.Application[factoryName] = null;
                }
            }

            CurrentDataCacheFactory = factory;
            CacheName = cacheName;
            RegionName = regionName;
            ApplicationCacheEnabled = cacheConfiguration.ObjectCacheEnabled;
        }

        public DataCacheFactory CurrentDataCacheFactory
        {
            get;
            protected set;
        }

        public string CacheName
        {
            get;
            protected set;
        }

        public string RegionName
        {
            get;
            protected set;
        }

        public bool ApplicationCacheEnabled
        {
            get;
            protected set;
        }
    }
}
