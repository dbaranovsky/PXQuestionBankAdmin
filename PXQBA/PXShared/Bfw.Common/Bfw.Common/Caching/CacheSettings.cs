using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Common.Caching
{
    /// <summary>
    /// Contains settings for items in the cache.
    /// </summary>
    public class CacheSettings
    {
        /// <summary>
        /// Type of invalidation supported for this cached item.
        /// </summary>
        public AgingMechanism Aging { get; set; }

        /// <summary>
        /// Span of time, in seconds, that the item is cached for. How this property is
        /// used depends on the AgingMechanism.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Relative priority of the item in the cache.
        /// </summary>
        public CachePriority Priority { get; set; }
    }

    /// <summary>
    /// Agining mechanisms supported by the system.
    /// </summary>
    public enum AgingMechanism
    {
        /// <summary>
        /// Item will expire from the cache at a set interval.
        /// </summary>
        Static = 0,

        /// <summary>
        /// Item will have its expiration reset each time it is accessed.
        /// </summary>
        Sliding
    }

    /// <summary>
    /// Determines how important the item is to remain in the cache, this 
    /// determines which items are removed during scavenging.
    /// </summary>
    public enum CachePriority
    {
        /// <summary>
        /// Normal cache priority.
        /// </summary>
        Normal = 0,
        /// <summary>
        /// High cache priority means the item will not be removed from the cache 
        /// during scavenging until all lower priority items are removed.
        /// </summary>
        High,
        /// <summary>
        /// Low cache priority means that the item will be removed during the first
        /// round of scavenging.
        /// </summary>
        Low
    }
}
