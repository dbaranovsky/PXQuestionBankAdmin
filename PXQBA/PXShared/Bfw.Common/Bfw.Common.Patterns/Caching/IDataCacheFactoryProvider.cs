using System;
using Microsoft.ApplicationServer.Caching;

namespace Bfw.Common.Patterns.Caching
{
    /// <summary>
    /// Simple interface to allow for injection of DataCacheFactory.
    /// </summary>
    public interface IDataCacheFactoryProvider
    {
        DataCacheFactory CurrentDataCacheFactory { get; }

        string CacheName { get; }

        string RegionName { get; }

        bool ApplicationCacheEnabled { get; }
    }
}
