using System.Configuration;

namespace Bfw.Common.HttpModules.Configuration.ResourceCompression
{
	/// <summary>
	/// Represents the custom configuration section for settings up
	/// resources for compression and combining.
	/// </summary>
	public class ResourceCompressionConfigurationSection : ConfigurationSection
	{
		/// <summary>
		/// The report element allows us to configure the analysis reports.
		/// </summary>
		[ConfigurationProperty(name: "report", IsRequired = false)]
		public ReportElement Report
		{
			get
			{
				return (ReportElement)this["report"];
			}
			set
			{
				this["report"] = value;
			}
		}

		/// <summary>
		/// Resources configure what logical resources the module can combine from
		/// separate files and how they should be treated.
		/// </summary>
		[ConfigurationProperty(name: "resources", IsRequired = false)]
		public ResourceElementCollection Resources
		{
			get
			{
				return (ResourceElementCollection)this["resources"];
			}
			set
			{
				this["resources"] = value;
			}
		}


		/// <summary>
		/// Resources configure what logical resources the module can combine from
		/// separate files and how they should be treated.
		/// </summary>
		[ConfigurationProperty(name: "domains", IsRequired = false)]
		public DomainElementCollection Domains
		{
			get
			{
				return (DomainElementCollection)this["domains"];
			}
			set
			{
				this["domains"] = value;
			}
		}


		/// <summary>
		/// Cache duration allows us to set, in seconds, the amount of time the resources should be
		/// cached for.  The default is for six months.
		/// </summary>
		[ConfigurationProperty(name: "cacheduration", IsRequired = false, DefaultValue = 15552000.0)]
		public double CacheDuration
		{
			get
			{
				return (double)this["cacheduration"];
			}
			set
			{
				this["cacheduration"] = value;
			}
		}

		/// <summary>
		/// Stores the path to the file that contains the current asset version.
		/// </summary>
		[ConfigurationProperty(name: "versionfile", IsRequired = false, DefaultValue = "")]
		public string VersionFile
		{
			get
			{
				return (string)this["versionfile"];
			}
			set
			{
				this["versionfile"] = value;
			}
		}
	}
}
