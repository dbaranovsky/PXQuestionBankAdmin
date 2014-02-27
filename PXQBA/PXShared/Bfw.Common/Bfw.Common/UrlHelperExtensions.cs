using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Configuration;
using Bfw.Common.HttpModules.ResourceCompression;


namespace Bfw.Common
{
    /// <summary>
    /// Static methods for URL. 
    /// </summary>

    public static class UrlHelperExtensions
    {
        /// <summary>
        /// ActionCache: UrlHelperExtensions Method
        /// <param name="urlHelper"></param>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <returns>String</returns>
        /// </summary>
        public static string ActionCache(this UrlHelper urlHelper, string actionName, string controllerName)
        {
            var cache = HttpContext.Current.Cache;
            string cacheKey = string.Format("{0}_{1}_{2}", HttpContext.Current.Request.Url.AbsolutePath, actionName, controllerName);
            object exists = cache[cacheKey];
            string url = string.Empty;

            if (exists != null)
            {
                url = exists.ToString();
            }
            else
            {
                url = urlHelper.Action(actionName, controllerName);
                cache[cacheKey] = url;
            }

            return url;
        }

        /// <summary>
        /// Content Cache
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="contentPath"></param>
        /// <returns>String</returns>
        public static string ContentCache(this UrlHelper urlHelper, string contentPath, bool versionPath = true)
        {
            var cache = HttpContext.Current.Cache;
           
            object exists = cache[contentPath];
            string url = string.Empty;

            if (exists != null)
            {
                url = exists.ToString();
            }
            else
            {
                if (ResourceEngine.IsDebug() || !versionPath)
                {
                    url = urlHelper.Content(contentPath);
                }
                else
                {
                    url = ResourceEngine.VersionPath(urlHelper.Content(contentPath));
                }
               
                cache[contentPath] = url;
            }

            return url;
        }

        /// <summary>
        /// get the domain, which can be used to set js document.domain property, so that iframes which are cross domian can access DOM elements.
        /// the logic used here is to get the hostname for the current request and get last 2 portions for it.
        /// For example if current url is http://www.dev.proxy.whfeeman.com  , then domain will be whfeeman.com
        /// </summary>
        /// <param name="urlHelper">UrlHelper</param>
        /// <returns></returns>
        public static string GetDocumentDomain(this UrlHelper urlHelper)           
        {
            string documentDomain = string.Empty;
            Uri requestUrl = urlHelper.RequestContext.HttpContext.Request.Url;
            string host = requestUrl.Host;
            if (!host.Contains("."))
            {
                documentDomain = host; // this will handle the case for "localhost"
            }
            else
            {
                string[] splitParts = host.Split('.');
                int length = splitParts.Length;
                documentDomain = string.Format("{0}.{1}", splitParts[length - 2], splitParts[length - 1]);
            }
                     
            return documentDomain;
        }

      

        /// <summary>
        /// Gets HTS editor url by reading app setting "PxHtsEditorUrl"
        /// </summary>
        /// <param name="urlHelper">UrlHelper</param>
        /// <returns></returns>
        public static string GetHtsEditorUrl(this UrlHelper urlHelper)
        {
            string htsEditorUrl = ConfigurationManager.AppSettings["PxHtsEditorUrl"];
            if (string.IsNullOrEmpty(htsEditorUrl))
            {
                htsEditorUrl = "";
            }
            else
            {
                string documentDomain = string.Empty;
                Uri requestUrl = System.Web.HttpContext.Current.Request.Url;
                string host = requestUrl.Host;
                if (!host.Contains("."))
                {
                    documentDomain = host; // this will handle the case for "localhost"
                }
                else
                {
                    string[] splitParts = host.Split('.');
                    int length = splitParts.Length;
                    documentDomain = string.Format("{0}.{1}", splitParts[length - 2], splitParts[length - 1]);
                }
                htsEditorUrl = htsEditorUrl.Replace("[company]", documentDomain);                
            }

            return htsEditorUrl;
        }

        /// <summary>
        /// Used to create a hash URL on a component following the schema of #state/componentname/function?args.
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="component">The name of the component that is setting the hash</param>
        /// <param name="function">The name of the function to run after setting the hash.  This should be setup on a js controller/router</param>
        /// <param name="args">Arguments to be passed into the function</param>
        /// <returns>A string formatted to set a hash url on a component buttonclick/link etc.</returns>
        public static string GetComponentHash(this UrlHelper urlHelper, string component, string function = null, object args = null)
        {
            var retval = string.Format("#state/{0}/{1}", component, function);

            if (args != null)
            {
                var argsDict = args as IDictionary<string, object>;
                if (argsDict == null)
                {
                    argsDict = new Dictionary<string, object>();
                    foreach (var prop in args.GetType().GetProperties())
                    {
                        var value = prop.GetValue(args, null);
                        argsDict.Add(prop.Name, value);
                    }
                }
                
                if (argsDict != null && argsDict.Count(k => k.Value != null) > 0)
                {

                    retval += "?" + string.Join("&", argsDict
                        .Where(k => k.Value != null && !String.IsNullOrWhiteSpace(k.Value.ToString()))
                        .Select(k => k.Key + "=" + k.Value.ToString()));
                }
            }

            return retval;
        }
        /// <summary>
        /// Generates an HTML anchor link for a hash URL following the schema of #state/componentname/function?args.
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="title">Title for the anchor tag</param>
        /// <param name="component">The name of the component that is setting the hash</param>
        /// <param name="function">The name of the function to run after setting the hash.  This should be setup on a js controller/router</param>
        /// <param name="args">Arguments to be passed into the function</param>
        /// <param name="htmlAttr">Html attributes to apply to anchor tag</param>
        /// <returns></returns>
        public static string GetComponentLink(this UrlHelper urlHelper, string title, string component, string function = null,
                                              object args = null, object htmlAttr = null)
        {
            var htmlAttrString = string.Empty;
            var htmlAttrDict = htmlAttr as IDictionary<string, object>;

            if (htmlAttrDict == null && htmlAttr != null)
            {
                htmlAttrDict = new Dictionary<string, object>();
                foreach (var prop in htmlAttr.GetType().GetProperties())
                {
                    var value = prop.GetValue(htmlAttr, null);
                    htmlAttrDict.Add(prop.Name, value);
                }
                
            }
            if (htmlAttrDict != null)
            {
                htmlAttrString = string.Join(" ", htmlAttrDict.Select(k => k.Key + "='" + k.Value.ToString() + "'"));
            }
            var retval = String.Format("<a href='{0}' {1} >{2}</a>", urlHelper.GetComponentHash(component, function, args), htmlAttrString, title);
            return retval;
        }

       
    }
}