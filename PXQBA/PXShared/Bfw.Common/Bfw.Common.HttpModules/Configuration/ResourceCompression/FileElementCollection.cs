using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Bfw.Common.HttpModules.Configuration.ResourceCompression
{
    /// <summary>
    /// Collection of files that are part of a resource.
    /// </summary>
    [ConfigurationCollection(typeof(FileElement), AddItemName = "file")]
    public class FileElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new FileElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FileElement)element).Path;
        }
    }

    /// <summary>
    /// Specific file that is part of a resource.
    /// </summary>
    public class FileElement : ConfigurationElement
    {
        #region Properties

        /// <summary>
        /// Virtual or absolute path to the script to process.
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
        /// True if the file should be compressed. If not set, the value is null, which means
        /// whether the resource is compressed or not depends on the value of the compress attribute
        /// on the resource element.
        /// </summary>
        [ConfigurationProperty(name: "compress", IsRequired = false, DefaultValue = null)]
        public bool? Compress
        {
            get
            {

                return (bool?)this["compress"];
            }
            set
            {
                this["compress"] = value;
            }
        }

        #endregion
    }
}
