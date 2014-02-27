using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Bfw.Common.HttpModules.Configuration.ResourceCompression
{
    /// <summary>
    /// Represents a resource build by combining and compressing several files into 
    /// one file.
    /// </summary>
    [ConfigurationCollection(typeof(ResourceElement), AddItemName = "resource")]
    public class ResourceElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ResourceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ResourceElement)element).Path;
        }
    }

    /// <summary>
    /// Resource that can be requested from the ResourceCompressionModule.
    /// </summary>
    public class ResourceElement : ConfigurationElement
    {
        #region Properties

        /// <summary>
        /// The path, full or virtual, of the output resource. This is the URL that
        /// the client will be attempting to load.
        /// </summary>
        [ConfigurationProperty(name: "path", IsRequired = true, IsKey = true)]
        public string Path
        {
            get
            {
                return (string)this["path"];
            }
            set
            {
                this["path"] = value;
            }
        }

        /// <summary>
        /// The type of resource to be constructed.
        /// </summary>
        [ConfigurationProperty(name: "type", IsRequired = true)]
        public ResourceType Type
        {
            get
            {
                return (ResourceType)this["type"];
            }
            set
            {
                this["type"] = value;
            }
        }

        /// <summary>
        /// True if the resource should be cached once compiled. 
        /// The default value is false if it isn't specified.
        /// </summary>
        [ConfigurationProperty(name: "cache", IsRequired = false, DefaultValue = false)]
        public bool Cache
        {
            get
            {
                return (bool)this["cache"];
            }
            set
            {
                this["cache"] = value;
            }
        }

        /// <summary>
        /// True if all files in the resouce definition should be compressed. Individual
        /// files can still override this by specifying cache=false.
        /// Default value is false.
        /// </summary>
        [ConfigurationProperty(name: "compress", IsRequired = false, DefaultValue = false)]
        public bool Compress
        {
            get
            {
                return (bool)this["compress"];
            }
            set
            {
                this["compress"] = value;
            }
        }

        /// <summary>
        /// The collection of files that are part of the resource.
        /// </summary>
        [ConfigurationProperty(name: "files", IsRequired = false)]
        public FileElementCollection Files
        {
            get
            {
                return (FileElementCollection)this["files"];
            }
            set
            {
                this["files"] = value;
            }
        }

        #endregion
    }
}
