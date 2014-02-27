using System.Configuration;

namespace Bfw.PX.PXPub.Controllers.Configurations
{

    public class ExternalDomainMapping : ConfigurationSection
    {
        #region Properties

        [ConfigurationProperty("enable", IsRequired = false)]
        public bool Enable
        {
            get
            {
                return this["enable"] != null && bool.Parse(this["enable"].ToString());
            }
            set
            {
                this["enable"] = value;
            }
        }

        [ConfigurationProperty("mapping", IsRequired = true, IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(ExternalDomainMappingCollection))]
        public ExternalDomainMappingCollection Mappings
        {
            get
            {
                return (ExternalDomainMappingCollection)base["mapping"];
            }
        }

        #endregion

        public class ExternalDomainMappingCollection : ConfigurationElementCollection
        {
            protected override ConfigurationElement CreateNewElement()
            {
                return new ExternalDomainMap();
            }

            protected override object GetElementKey(ConfigurationElement element)
            {
                return ((ExternalDomainMap)element).MapFrom; ;
            }
        }
        public class ExternalDomainMap : ConfigurationElement
        {
            #region Properties
            [ConfigurationProperty("from", IsRequired = true, IsKey=true)]
            public string MapFrom
            {
                get
                {
                    return base["from"].ToString();
                }
                set
                {
                    base["from"] = value;
                }
            }

            [ConfigurationProperty("to", IsRequired = true)]
            public string MapTo
            {
                get
                {
                    return base["to"].ToString();
                }
                set
                {
                    base["to"] = value;
                }
            }
            #endregion
        }
    }


}
