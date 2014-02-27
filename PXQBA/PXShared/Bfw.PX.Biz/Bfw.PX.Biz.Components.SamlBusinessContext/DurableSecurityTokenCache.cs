using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel;
using System.IdentityModel.Tokens;

using Microsoft.IdentityModel;
using Microsoft.IdentityModel.Tokens;

namespace Bfw.PX.Biz.Components.SamlBusinessContext
{

    /// <summary>
    /// Two level durable security token cache (level 1: in memory MRU, level 2: out of process cache).
    /// </summary>
    public class DurableSecurityTokenCache : SecurityTokenCache
    {
        private readonly MruCache<SecurityTokenCacheKey, SecurityToken> mruCache;

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="durableCache">The durable second level cache (should be out of process ie sql server, azure table, app fabric, etc).</param>
        /// <param name="mruCapacity">Capacity of the internal first level cache (in-memory MRU cache).</param>
        public DurableSecurityTokenCache(int mruCapacity)
        {
            this.mruCache = new MruCache<SecurityTokenCacheKey, SecurityToken>(mruCapacity, mruCapacity / 4);
        }

        public override bool TryAddEntry(object key, SecurityToken value)
        {
            var cacheKey = (SecurityTokenCacheKey)key;

            // add the entry to the mru cache.
            this.mruCache.Add(cacheKey, value);

            // add the entry to the durable cache.
            var keyString = GetKeyString(cacheKey);
            var buffer = this.GetSerializer().Serialize((SessionSecurityToken)value);

            return true;
        }

        public override bool TryGetEntry(object key, out SecurityToken value)
        {
            var cacheKey = (SecurityTokenCacheKey)key;

            // attempt to retrieve the entry from the mru cache.
            value = this.mruCache.Get(cacheKey);
            if (value != null)
                return true;

            return false;
        }

        public override bool TryRemoveEntry(object key)
        {
            var cacheKey = (SecurityTokenCacheKey)key;

            // remove the entry from the mru cache.
            this.mruCache.Remove(cacheKey);

            return true;
        }

        public override bool TryReplaceEntry(object key, SecurityToken newValue)
        {
            var cacheKey = (SecurityTokenCacheKey)key;

            // remove the entry in the mru cache.
            this.mruCache.Remove(cacheKey);

            // add the new value.
            return this.TryAddEntry(key, newValue);
        }

        public override bool TryGetAllEntries(object key, out IList<SecurityToken> tokens)
        {
            // not implemented... haven't been able to find how/when this method is used.
            tokens = new List<SecurityToken>();
            return true;
            //throw new NotImplementedException();
        }

        public override bool TryRemoveAllEntries(object key)
        {
            // not implemented... haven't been able to find how/when this method is used.
            return true;
            //throw new NotImplementedException();
        }

        public override void ClearEntries()
        {
            // not implemented... haven't been able to find how/when this method is used.
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the string representation of the specified SecurityTokenCacheKey.
        /// </summary>
        private string GetKeyString(SecurityTokenCacheKey key)
        {
            return string.Format("{0}; {1}; {2}", key.ContextId, key.KeyGeneration, key.EndpointId);
        }

        /// <summary>
        /// Gets a new instance of the token serializer.
        /// </summary>
        private SessionSecurityTokenCookieSerializer GetSerializer()
        {
            return new SessionSecurityTokenCookieSerializer();  // may need to do something about handling bootstrap tokens.
        }
    }
}
