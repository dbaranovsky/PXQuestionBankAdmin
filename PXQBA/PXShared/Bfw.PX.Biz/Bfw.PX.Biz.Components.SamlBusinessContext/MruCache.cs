using System.Collections;
using System.Collections.Generic;

namespace Bfw.PX.Biz.Components.SamlBusinessContext
{
    /// <summary>
    /// Most recently used (MRU) cache.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public class MruCache<TKey, TValue>
    {
        private Dictionary<TKey, TValue> mruCache;
        private LinkedList<TKey> mruList;
        private object syncRoot;
        private int capacity;
        private int sizeAfterPurge;

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        /// <param name="sizeAfterPurge">Size to make the cache after purging because it's reached capacity.</param>
        public MruCache(int capacity, int sizeAfterPurge)
        {
            this.mruList = new LinkedList<TKey>();
            this.mruCache = new Dictionary<TKey, TValue>(capacity);
            this.capacity = capacity;
            this.sizeAfterPurge = sizeAfterPurge;
            this.syncRoot = new object();
        }

        /// <summary>
        /// Adds an item if it doesn't already exist.
        /// </summary>
        public void Add(TKey key, TValue value)
        {
            lock (this.syncRoot)
            {
                if (mruCache.ContainsKey(key))
                    return;

                if (mruCache.Count + 1 >= this.capacity)
                {
                    while (mruCache.Count > this.sizeAfterPurge)
                    {
                        var lru = mruList.Last.Value;
                        mruCache.Remove(lru);
                        mruList.RemoveLast();
                    }
                }
                mruCache.Add(key, value);
                mruList.AddFirst(key);
            }
        }

        /// <summary>
        /// Removes an item if it exists.
        /// </summary>
        public void Remove(TKey key)
        {
            lock (this.syncRoot)
            {
                if (!mruCache.ContainsKey(key))
                    return;

                mruCache.Remove(key);
                mruList.Remove(key);
            }
        }

        /// <summary>
        /// Gets an item.  If a matching item doesn't exist null is returned.
        /// </summary>
        public TValue Get(TKey key)
        {
            lock (this.syncRoot)
            {
                if (!mruCache.ContainsKey(key))
                    return default(TValue);

                mruList.Remove(key);
                mruList.AddFirst(key);
                return mruCache[key];
            }
        }

        /// <summary>
        /// Gets whether a key is contained in the cache.
        /// </summary>
        public bool ContainsKey(TKey key)
        {
            lock (this.syncRoot)
                return mruCache.ContainsKey(key);
        }
    }
}