using System.Configuration;

namespace Bfw.Common.HttpModules.Configuration.ResourceCompression
{
	/// <summary>
	/// Represents a domains collection 
	/// one file.
	/// </summary>
	[ConfigurationCollection(typeof(ResourceElement), AddItemName = "domain")]
	public class DomainElementCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new DomainElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ( (DomainElement)element ).DomainPrefix;
		}


		/// <summary>
		/// Flag to enable CDN (Content Delivery Network)
		/// </summary>
		[ConfigurationProperty(name: "enableCDN", IsRequired = false, DefaultValue = false)]
		public bool EnableCDN
		{
			get
			{
				return (bool)this["enableCDN"];
			}
			set
			{
				this["enableCDN"] = value;
			}
		}

	}

	/// <summary>
	/// Domain that can be requested from the ResourceCompressionModule.
	/// </summary>
	public class DomainElement : ConfigurationElement
	{
		#region Properties

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
		/// Domain property
		/// </summary>
		[ConfigurationProperty(name: "domainPrefix", IsRequired = true)]
		public string DomainPrefix
		{
			get
			{
				return (string)this["domainPrefix"];
			}
			set
			{
				this["domainPrefix"] = value;
			}
		}


		#endregion
	}
}

