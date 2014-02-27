using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using Bfw.Common.HttpModules.Configuration.ResourceCompression;
using dotless.Core;
using Yahoo.Yui.Compressor;

namespace Bfw.Common.HttpModules.ResourceCompression
{
	/// <summary>
	/// Responsible for processing resource files.
	/// </summary>
	public class ResourceEngine
	{

		#region Constants

		/// <summary>
		/// Key used to cache the configuration of the resources.
		/// </summary>
		private const string CONFIG_KEY = "RESOURCE_COMPRESSION_MODULE_CONFIG";

		/// <summary>
		/// Expected name of the configuration section.
		/// </summary>
		private const string CONFIG_SECTION_NAME = "bfw.common.httpmodules.resourcecompression";

		#endregion

		#region Properties

		/// <summary>
		/// HttpServerUtility instance used for mapping paths.
		/// </summary>
		protected static HttpServerUtility Server { get { return HttpContext.Current.Server; } }

		/// <summary>
		/// Cache instance used to store the configuration and other cached resources.
		/// </summary>
		protected static Cache Cache { get { return HttpContext.Current.Cache; } }

		/// <summary>
		/// Stores the data from the configuration sections.
		/// </summary>
		private static ResourceCompressionConfigurationSection _config;

		/// <summary>
		/// Stores the data from the configuration section.
		/// </summary>
		protected static ResourceCompressionConfigurationSection Configuration
		{
			get
			{
				if (_config == null)
				{
					_config = ConfigurationManager.GetSection(CONFIG_SECTION_NAME) as Configuration.ResourceCompression.ResourceCompressionConfigurationSection;
				}

				return _config;
			}
		}

		/// <summary>
		/// Gets the cache duration as set in the configuration file.
		/// </summary>
		public static double CacheDuration
		{
			get
			{
				return Configuration.CacheDuration;
			}
		}

		protected static string _version = string.Empty;
		protected static Regex NegativeVersionValidation = new Regex(@"[^a-zA-Z0-9]", RegexOptions.CultureInvariant);
		/// <summary>
		/// Gets the current version from the version file.
		/// </summary>
		public static string Version
		{
			get
			{
				if (string.IsNullOrEmpty(_version))
				{
					var file = Configuration.VersionFile;
					if (!string.IsNullOrEmpty(file))
					{
						file = Server.MapPath(VirtualPathUtility.ToAbsolute(file));
						if (System.IO.File.Exists(file))
						{
							String fileContents = System.IO.File.ReadAllText(file);
							_version = NegativeVersionValidation.Replace(fileContents, "");
						}
					}
				}

				return _version;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Returns the processed resource.
		/// </summary>
		/// <param name="uri">Raw URI to the resource requested by the user.</param>
		/// <returns>Content of the resource.</returns>
		public static Resource Resource(Uri uri)
		{
			var path = UnversionPath(uri.AbsolutePath.ToLowerInvariant());
			return Resource(path);
		}

		/// <summary>
		/// Returns the processed resource.
		/// </summary>
		/// <param name="path">Absolute path to the resource.</param>
		/// <returns>Content of the resource.</returns>
		public static Resource Resource(string path)
		{
			Resource resource = null;
			var resources = AllResources();
			var key = path.ToLowerInvariant();

			if (resources.ContainsKey(key))
			{
				resource = resources[key];
				ProcessResource(resource);
			}

			return resource;
		}

		/// <summary>
		/// Processes the given resource based on its Type.
		/// </summary>
		/// <param name="resource">Resource to process.</param>
		private static void ProcessResource(Resource resource)
		{
			if (resource.Cache && !string.IsNullOrWhiteSpace(resource.Content))
			{
				return;
			}

			switch (resource.Type)
			{
				case ResourceType.js:
					ProcessJs(resource);
					break;

				case ResourceType.css:
					ProcessCss(resource);
					break;
			}
		}

		/// <summary>
		/// Processes Javascript resources. Each file listed in the 
		/// Resource's configuration is loaded, optionally compressed,
		/// and then combined with the other files to create a single
		/// resource.
		/// </summary>
		/// <param name="resource">Javascript resource to process.</param>
		private static void ProcessJs(Resource resource)
		{
			var processed = new StringBuilder();

		    resource.LastModified = DateTime.MinValue;
			foreach (var file in resource.Files)
			{
				var text = System.IO.File.ReadAllText(file.Path);
                var lastMod = System.IO.File.GetLastWriteTime(file.Path);
			    if (lastMod > resource.LastModified)
			        resource.LastModified = lastMod;
				var errorReporter = new ScriptErrorReporter();
				if (file.Compress)
				{
					var c = new JavaScriptCompressor(text, true, Encoding.UTF8, System.Globalization.CultureInfo.CurrentCulture, true, errorReporter);
					text = c.Compress();
				}
				processed.AppendLine(String.Format("/** {0} **/", file.Path.Split('\\', '/').LastOrDefault() ?? "NA"));
				processed.AppendLine(text);
				processed.AppendLine(";");
			}

			resource.Content = processed.ToString();
		}

		/// <summary>
		/// Processes CSS resources.  Each file in the resource is 
		/// loaded, optionally compressed, and then combined with the 
		/// other files to create a single resource. If a file has a .less extension
		/// it will be parsed as a less file prior to being compressed.
		/// </summary>
		/// <param name="resource">CSS resource.</param>
		private static void ProcessCss(Resource resource)
		{
			var processed = new StringBuilder();

		    resource.LastModified = DateTime.MinValue;
			foreach (var file in resource.Files)
			{
				var text = System.IO.File.ReadAllText(file.Path);
				var compress = file.Compress;

                var lastMod = System.IO.File.GetLastWriteTime(file.Path);
                if (lastMod > resource.LastModified)
                    resource.LastModified = lastMod;

				if (System.IO.Path.GetExtension(file.Path).ToLowerInvariant() == ".less")
				{
					text = Less.Parse(text);
				}

				if (string.IsNullOrEmpty(text))
				{
					text = string.Format("/* WARNING: the file {0} is either empty, or the less syntax is invalid */", file.Path);
					compress = false;
				}

                //replace images location from local to brainhoney location
                if (!string.IsNullOrEmpty(text))
                {
                    var BrainHoneyLocation = ConfigurationManager.AppSettings["BHLoc"];
                    text = text.Replace("style/images/", BrainHoneyLocation + "images/");
                }

				if (compress)
				{
					text = CssCompressor.Compress(text, 80, CssCompressionType.StockYuiCompressor, true);
				}

				processed.AppendLine(text);
			}

			resource.Content = processed.ToString();
		}

		/// <summary>
		/// Gets a dictionary of resources that the module is aware of.
		/// </summary>
		/// <param name="server">HttpServerUtility that provides ability to find the physical file paths.</param>
		/// <param name="cache">Cache that may store the resource dictionary.</param>
		/// <returns>Resource dictionary.</returns>
		private static IDictionary<string, Resource> AllResources()
		{
			IDictionary<string, Resource> resources = Cache[CONFIG_KEY] as IDictionary<string, Resource>;

			if (resources == null)
			{
				resources = new Dictionary<string, Resource>();
				var config = Configuration;

				foreach (ResourceElement resourceElm in config.Resources)
				{

					var resource = new Resource()
					{
						Compress = resourceElm.Compress,
						Cache = resourceElm.Cache,
						Type = resourceElm.Type,
						Path = VirtualPathUtility.ToAbsolute(resourceElm.Path).ToLowerInvariant(),
						Files = new List<File>()
					};

					resource = GetResourceCdnPath(resource, config, resourceElm);

					foreach (FileElement fileElm in resourceElm.Files)
					{
						resource.Files.Add(new File()
						{
							Path = Server.MapPath(fileElm.Path),
							ServerPath = VirtualPathUtility.ToAbsolute(fileElm.Path),
							Compress = fileElm.Compress.HasValue ? fileElm.Compress.Value : resource.Compress
						});
					}

					resources[resource.Path] = resource;

					if (_isCdnEnabled)
					{
						resources[resource.CdnPath] = resource;
					}
					else
					{
						resources[resource.Path] = resource;
					}
				}

				Cache.Add(CONFIG_KEY, resources, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
			}

			return resources;
		}

		private static bool _isCdnEnabled = false;

		private static Resource GetResourceCdnPath(Resource resource, ResourceCompressionConfigurationSection config,
											   ResourceElement resourceElm)
		{
			var Domains = config.Domains;

			if (!Domains.EnableCDN) return resource;

			_isCdnEnabled = true;

			string appHostName = System.Web.HttpContext.Current.Request.Url.DnsSafeHost.ToLower();
			string appPath = String.Format("{0}/{1}", appHostName, System.Web.HttpContext.Current.Request.ApplicationPath);

			var resourcePath = resourceElm.Path.ToLowerInvariant().Replace("~/", "/");

			var elm = resourceElm;

			foreach (var domain in Domains.Cast<DomainElement>().Where(domain => elm.Type == domain.Type))
			{
				resource.CdnPath = String.Format("http://{0}{1}{2}", domain.DomainPrefix, appPath, resourcePath).ToLowerInvariant();
				return resource;
			}

			return resource;
		}

		/// <summary>
		/// Renders the correct include HTML needed to add the resource to the page.
		/// If the compilation.debug config setting is true, then the original files will
		/// be referenced instead of the processed resource.
		/// </summary>
		/// <param name="resource"></param>
		/// <returns></returns>
		public static IHtmlString IncludesFor(string path, string baseUrl = "")
		{
			var includes = string.Empty;
			var resource = Resource(VirtualPathUtility.ToAbsolute(path));
			var debug = IsDebug();

			switch (resource.Type)
			{
				case ResourceType.js:
					includes = IncludesForJs(resource, debug, baseUrl);
					break;

				case ResourceType.css:
					includes = IncludesForCss(resource, debug, baseUrl);
					break;
			}

			return new HtmlString(includes);
		}

		/// <summary>
		/// Returns an array of URLs that can be used to access the resource.
		/// If the application is in debug mode then the URLs represent each file
		/// in the resource. Otherwise, the array returned only contains one item
		/// which represents the resource itself.
		/// </summary>
		/// <param name="path">Virtualpath to the resource</param>
		/// <returns></returns>
		public static IList<string> UrlsFor(string path)
		{
			var urls = new List<string>();
			var resource = Resource(VirtualPathUtility.ToAbsolute(path));
			var debug = IsDebug();

			if (debug)
			{
				if (!resource.Files.IsNullOrEmpty())
				{
					foreach (var file in resource.Files)
					{
						urls.Add(file.ServerPath);
					}
				}
			}
			else
			{
				urls.Add(resource.Path);
			}

			return urls;
		}

	

	    private static IHtmlString GetJsonForPath(string path, string[] paths, bool? isRenderAllFiles = null, bool? isVersion = null)
	    {
	        var json = new StringBuilder();
	        var pathList = new List<string>() {path};
	        pathList.AddRange(paths);
	        var filePaths = new List<string>();
	        var debug = IsDebug();
            if (isRenderAllFiles == null)
            {
                isRenderAllFiles = debug;
            }
	        if (isVersion == null)
	        {
	            isVersion = !debug;
	        }
	        foreach (var resourcePath in pathList)
	        {
	            var resource = Resource(VirtualPathUtility.ToAbsolute(resourcePath));
	            if (resource == null)
	                continue;
                if (isRenderAllFiles.Value)
	            {
	                if (!resource.Files.IsNullOrEmpty())
	                {
	                    var files = resource.Files;

	                    foreach (var file in files)
	                    {
	                        if (isVersion.Value)
	                        {
                                filePaths.Add(VersionPath(file.ServerPath));
	                        }
	                        else
	                        {
                                filePaths.Add(file.ServerPath);
	                        }
	                    }
	                }
	            }

	            else
	            {
                    if (isVersion.Value)
                    {
                        filePaths.Add(VersionPath(resource.Path));
                    }
                    else
                    {
                        filePaths.Add(resource.Path);
                    }
	            }
	        }

	        int i = 1;
	        foreach (var filePath in filePaths)
	        {
	            json.AppendFormat("'{0}'{1}", filePath, i < filePaths.Count ? "," : "");
	            ++i;
	        }


	        return new HtmlString(string.Format("[{0}]", json));
	    }


    /// <summary>
	    /// Returns a string containing a JSON formatted array where each element is 
	    /// a path to a resource, or part of a resource.
	    /// 
	    /// If IsDebug returns false, then the array contains a single element which is
	    /// the logical path to the combined resource.  Otherwise, the array contains an
	    /// element for each original file that makes up the resource.
	    /// </summary>
	    /// <param name="path">Path of the resource to generate the JSON for.</param>
	    /// <param name="forceRenderAllFiles">Overrides the debug flag to force generate a list of files</param>
	    /// <returns>String containing a JSON formatted array with the resource paths.</returns>
	    public static IHtmlString JsonFor(string path, params string[] paths)
	    {
	        return GetJsonForPath(path, paths);
	    }
        /// <summary>
        /// Returns a string containing a JSON formatted array where each element is 
        /// a path to a resource, or part of a resource.
        /// 
        /// This method does NOT concatonate all JS files
        /// </summary>
        /// <param name="path"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static IHtmlString JsonForAllFiles(string path, params string[] paths)
        {
           return GetJsonForPath(path, paths, true, !IsDebug());
            
        }

	    /// <summary>
		/// Renders script tags necessary to include the correct scripts for the
		/// resource.
		/// </summary>
		/// <param name="resource">Resource to get the includes for.</param>
		/// <param name="isDebug">Defaults to false. If true then includes for the original scripts that make up the
		/// resource are returned. Otherwise, just the include to the combined resource is returned.</param>
		/// <returns>All script tags necessary to get all parts of the resource.</returns>
		private static string IncludesForJs(Resource resource, bool isDebug = false, string baseUrl = "")
		{
			var includes = new StringBuilder();

			if (_isCdnEnabled)
			{
				includes.Append(ScriptTag(resource.CdnPath));
			}
			else if (isDebug)
			{
				foreach (var file in resource.Files)
				{
					includes.AppendLine(ScriptTag(file.ServerPath));
				}
			}
			else
			{
				includes.Append(ScriptTag(resource.Path));
			}

			return includes.ToString();
		}

		/// <summary>
		/// Renders link tags necessary to include the correct css for the
		/// resource.
		/// </summary>
		/// <param name="resource">Resource to get the includes for.</param>
		/// <param name="isDebug">Defaults to false. If true then includes for the original css that make up the
		/// resource are returned. Otherwise, just the include to the combined resource is returned.</param>
		/// <returns>All link tags necessary to get all parts of the resource.</returns>
		private static string IncludesForCss(Resource resource, bool isDebug = false, string baseUrl = "")
		{
			var includes = new StringBuilder();

			if (_isCdnEnabled)
			{
				includes.Append(LinkTag(resource.CdnPath));
			}
			else if (isDebug)
			{
				foreach (var file in resource.Files)
				{
				    var styleControllerBaseUrl = "style";
				    if (baseUrl.IsNullOrEmpty())
				    {
                        baseUrl = HttpContext.Current.Request.Url.AbsolutePath;
				    }
				   
				    if (baseUrl.EndsWith("/"))
				    {
				        baseUrl = baseUrl.TrimEnd('/');
				    }
				    styleControllerBaseUrl = baseUrl + "/style";
				    
				 
                    includes.AppendLine(LinkTag(file.ServerPath.Replace("/Content", styleControllerBaseUrl)));
				}
			}
			else
			{
				includes.Append(LinkTag(resource.Path));
			}

			return includes.ToString();
		}

		/// <summary>
		/// Renders a script tag using the given src url.
		/// </summary>
		/// <param name="url">URL to the script.</param>
		/// <returns>string containing the script tag.</returns>
		private static string ScriptTag(string url)
		{
			var href = url;

			if (!IsDebug())
			{
				href = VersionPath(url);
			}

			return string.Format("<script type=\"text/javascript\" language=\"javascript\" src=\"{0}\"></script>", href);
		}

     

		/// <summary>
		/// Renders a link tag given the source url.
		/// </summary>
		/// <param name="url">Url to the css file to render a link tag for.</param>
		/// <returns>string containing the link tag.</returns>
		public static string LinkTag(string url)
		{
			var href = url;

			if (!IsDebug())
			{
				href = VersionPath(url);
			}

			return string.Format("<link type=\"text/css\" rel=\"stylesheet\" href=\"{0}\" />", href);
		}

		/// <summary>
		/// Checks the compilation section in the web.config and returns true if the debug 
		/// attribute is set.
		/// </summary>
		/// <returns>True if the compilation.debug flag is true, false otherwise.</returns>
		public static bool IsDebug()
		{
			bool debug = true;

			var compilation = ConfigurationManager.GetSection("system.web/compilation") as System.Web.Configuration.CompilationSection;

			if (compilation != null)
			{
				debug = compilation.Debug;
			}

			return debug;
		}

		/// <summary>
		/// Takes in a path and versions it.
		/// </summary>
		/// <param name="path">unversioned path.</param>
		/// <returns>versioned path.</returns>
		protected static Regex VersionPlacementLocation = new Regex(@"(\.\w+)$", RegexOptions.CultureInvariant);
		public static string VersionPath(string path)
		{
			String[] pathAndQuery = path.Split('?');
			String url = pathAndQuery[0];

			String query = null;
			if (pathAndQuery.Length == 2 && !String.IsNullOrEmpty(pathAndQuery[1]))
				query = pathAndQuery[1];

			String versionedUrl = VersionPlacementLocation.Replace(url, String.Format("__{0}$1", Version));

			if (null == query)
				return versionedUrl;
			else
				return String.Join("?", versionedUrl, query);
		}

        /// <summary>
        /// Get the version for a given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
	    public static string GetVersion(Uri path)
        {
            var match = Regex.IsMatch(path.AbsolutePath, @"(.*__)(.*)(\..*)");
            if (match)
            {
                return Regex.Replace(path.AbsolutePath, @"(.*__)(.*)(\..*)", "$2");
            }
            else
            {
                return null;
            }
        }

		/// <summary>
		/// Takes in a path that contains a version and unversions it.
		/// </summary>
		/// <param name="path">versioned path.</param>
		/// <returns>unversioned path.</returns>
		public static string UnversionPath(string path)
		{
			return System.Text.RegularExpressions.Regex.Replace(path, @"__.*\.", ".");
		}

		#endregion
	}
}
