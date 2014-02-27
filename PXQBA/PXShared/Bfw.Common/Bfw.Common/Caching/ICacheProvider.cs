using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Bfw.Common.Caching
{
    /// <summary>
    /// Defines capabilities for enterprise caching.
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// Flag to disable caching (overwrites the config setting)
        /// </summary>
        bool Disabled { get; set; }

        /// <summary>
        /// Stores item in cache with the specified unique item key.
        /// </summary>
        /// <param name="key">Unique item key in cache.</param>
        /// <param name="item">Object to store in cache.</param>
        /// <param name="settings">Cache Settings</param>
        void Store(string key, object item, CacheSettings settings);

        /// <summary>
        /// Stores item in cache with the specified unique item key in a specific region.
        /// </summary>
        /// <param name="key">Unique item key in cache.</param>
        /// <param name="item">Object to store in cache.</param>
        /// <param name="settings">Cache Settings</param>
        /// <param name="region">Specific region in cache</param>
        /// <param name="tag">tag to use for AppFabric cache provider</param>
        void Store(string key, object item, CacheSettings settings, string region, string tag = "");

        /// <summary>
        /// Stores item in cache with the specified unique item key.
        /// </summary>
        /// <param name="items">Dictionary of items and keys to store in cache.</param>
        /// <param name="settings">Cache Settings</param>
        void Store(IDictionary<string, object> items, CacheSettings settings, string region, string tag = "");

        /// <summary>
        /// Stores item in cache with the specified unique item key in a specific region and specific tags.
        /// </summary>
        /// <param name="key">Unique item key in cache.</param>
        /// <param name="item">Object to store in cache.</param>
        /// <param name="settings">Cache Settings</param>
        /// <param name="region">Specific region in cache</param>
        /// <param name="tags">list of tags to use for AppFabric cache provider</param>
        void Store(string key, object item, CacheSettings settings, string region, List<string> tags);

        /// <summary>
        /// If found, removes an item with the specified key from cache.
        /// </summary>
        /// <param name="key">Unique item key in cache.</param>
        object Remove(string key);

        /// <summary>
        /// If found, removes an item with the specified key from cache.
        /// </summary>
        /// <param name="keys">Unique item key in cache.</param>
        IDictionary<string, object> Remove(List<string> keys, string region);

        /// <summary>
        /// If found, removes an item with the specified key from cache for a specific region.
        /// </summary>
        /// <param name="key">Unique item key in cache.</param>
        /// <param name="region">Specific region in cache</param>
        object Remove(string key, string region);

        /// <summary>
        /// Clears an entire region in the cache
        /// </summary>
        /// <param name="regionName"></param>
        void ClearRegion(string regionName);

        /// <summary>
        /// If found, removes all items with the specified tag from cache for a specific region.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        IDictionary<string, object> RemoveByTag(string tag, string region);

        /// <summary>
        /// Attempts to retrieve a cached object.
        /// </summary>
        /// <param name="key">Unique item key in cache.</param>
        /// <returns>Returns cached object if found, null otherwise.</returns>
        object Fetch(string key);

        /// <summary>
        /// Attempts to retrieve a cached object from a specific region.
        /// </summary>
        /// <param name="key">Unique item key in cache.</param>
        /// <param name="region">Specific region in cache</param>
        /// <returns>Returns cached object if found, null otherwise.</returns>
        object Fetch(string key, string region);

        /// <summary>
        /// Attempts to retrieve a cached object from a specific region.
        /// </summary>
        /// <param name="keys">Unique item key in cache.</param>
        /// <param name="region">Specific region in cache</param>
        /// <returns>Returns cached object if found, null otherwise.</returns>
        IDictionary<string, object> Fetch(List<string> keys, string region);


        /// <summary>
        /// Attempts to retrieve all objects with a specific tag from a region
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        IDictionary<string, object> FetchByTag(string tag, string region);

        /// <summary>
        /// Attempts to retrieve all objects with the specificied tags from a region
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        IDictionary<string, object> FetchByAllTags(List<string> tags, string region);

        /// <summary>
        /// Attempts to retrieve all objects with any of the specificied tags from a region
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        IDictionary<string, object> FetchByAnyTag(List<string> tags, string region);

        /// <summary>
        /// Gets a list of all tags stored with the item
        /// </summary>
        /// <param name="key"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        List<string> GetCacheItemTags(string itemKey, string region);

        /// <summary>
        /// Returns true if the cache is disabled
        /// </summary>
        /// <returns></returns>
        bool IsCacheDisabledByCookie();
    }
}