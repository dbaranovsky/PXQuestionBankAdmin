using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Bfw.Common.Caching
{
    /// <summary>
    /// Custom configuration section used to setup access to DLAP and BrainHoney.
    /// </summary>
    public class CachingResponseProxySection : ConfigurationSection
    {
        #region Properties

        /// <summary>
        /// Gets or sets the cache location.
        /// </summary>
        /// <value>The cache location.</value>
        /// <remarks>Local path to where cached files are stored.</remarks>
        [ConfigurationProperty("cacheLocation", IsRequired = true)]
        public String CacheLocation
        {
            get
            {
                return (String)this["cacheLocation"];
            }
            set
            {
                this["cacheLocation"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the cache duration.
        /// </summary>
        /// <value>The cache duration.</value>
        /// <remarks>Duration, in minutes, that cached file is valid for.</remarks>
        [ConfigurationProperty("cacheDuration", IsRequired = true, DefaultValue="60")]
        public int CacheDuration
        {
            get
            {
                return Math.Abs((int)this["cacheDuration"]);
            }
            set
            {
                this["cacheDuration"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the cache meta file extension.
        /// </summary>
        /// <value>The cache meta file extension.</value>
        /// <remarks>Required, Default "meta". Extension used for metadata of cached files. Do NOT include the dot.</remarks>
        [ConfigurationProperty("cacheMetaExtension", IsRequired = true, DefaultValue = "meta")]
        public String CacheMetaExtension
        {
            get
            {
                return (String)this["cacheMetaExtension"];
            }
            set
            {
                this["cacheMetaExtension"] = value;
            }
        }   

        /// <summary>
        /// Gets or sets the cache content file extension.
        /// </summary>
        /// <value>The cache content file extension.</value>
        /// <remarks>Required, Default "cont". Extension used for content of cached files. Do NOT include the dot.</remarks>
        [ConfigurationProperty("cacheContentExtension", IsRequired = true, DefaultValue = "cont")]
        public String CacheContentExtension
        {
            get
            {
                return (String)this["cacheContentExtension"];
            }
            set
            {
                this["cacheContentExtension"] = value;
            }
        }

        /// <summary>
        /// Turns the proxy cache on and off.
        /// </summary>
        /// <value>True if proxy cache is on, false otherwise.</value>
        /// <remarks>Required, Default is true.</remarks>
        [ConfigurationProperty("proxyCacheEnabled", IsRequired = true, DefaultValue=true)]
        public Boolean ProxyCacheEnabled
        {
            get
            {
                return (Boolean)this["proxyCacheEnabled"];
            }
            set
            {
                this["proxyCacheEnabled"] = value;
            }
        }

        /// <summary>
        /// Turns the proxy cache on and off.
        /// </summary>
        /// <value>True if proxy cache is on, false otherwise.</value>
        /// <remarks>Required, Default is true.</remarks>
        [ConfigurationProperty("objectCacheEnabled", IsRequired = true, DefaultValue = true)]
        public Boolean ObjectCacheEnabled
        {
            get
            {
                return (Boolean)this["objectCacheEnabled"];
            }
            set
            {
                this["objectCacheEnabled"] = value;
            }
        }

        #endregion
    }
}