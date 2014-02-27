using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Configuration;

using ICSharpCode.SharpZipLib.GZip;

using Bfw.Common.HttpModules.Configuration.ResourceCompression;

namespace Bfw.Common.HttpModules.ResourceCompression
{
    /// <summary>
    /// 
    /// </summary>
    public class ResourceCompressionModule : IHttpModule
    {
        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += Application_BeginRequest;
        }

        private void Application_BeginRequest(Object source, EventArgs e)
        {
            HttpApplication application = (HttpApplication)source;
            HttpContext context = application.Context;
            double cacheDuration = ResourceEngine.CacheDuration;
            
            var ifModifiedSince = application.Request.Headers["If-Modified-Since"];
            DateTime modifiedDate;
            DateTime.TryParse(ifModifiedSince, out modifiedDate);

            var version = ResourceEngine.GetVersion(context.Request.Url);
            if (version == ResourceEngine.Version && ifModifiedSince != null)
            {
                application.Response.StatusCode = 304;
                application.Response.SuppressContent = true;
                application.Response.End();
                return;
            }
            var resource = ResourceEngine.Resource(context.Request.Url);

            if (resource != null)
            {
                if (resource.Cache)
                {
                    application.Response.ContentType = resource.ContentType;
                    application.Response.Cache.SetAllowResponseInBrowserHistory(true);
                    application.Response.Cache.SetExpires(DateTime.UtcNow.AddSeconds(cacheDuration));
                    application.Response.Cache.SetMaxAge(new TimeSpan(365, 0, 0, 0));
                    application.Response.Cache.SetLastModified(resource.LastModified);
                    application.Response.Cache.SetCacheability(HttpCacheability.Public);
                }

                if (resource.LastModified.ToString() == modifiedDate.ToString())
                {
                    application.Response.StatusCode = 304;
                    application.Response.SuppressContent = true;
                }
                else
                {
                    application.Response.Write(resource.Content);
                    application.Response.End();
                }

            }
            else if (context.Request.Url.AbsolutePath.ToLowerInvariant().Contains(".less"))
            {
                try
                {
                    var filePath = application.Server.MapPath(ResourceEngine.UnversionPath(context.Request.Url.AbsolutePath));
                    var css = System.IO.File.ReadAllText(filePath);

                    css = dotless.Core.Less.Parse(css);
                    application.Response.ContentType = "text/css";

                    if (!ResourceEngine.IsDebug())
                    {
                        css = Yahoo.Yui.Compressor.CssCompressor.Compress(css);
                        application.Response.Cache.SetAllowResponseInBrowserHistory(true);
                        application.Response.Cache.SetExpires(DateTime.UtcNow.AddSeconds(cacheDuration));
                        application.Response.Cache.SetLastModified(System.IO.File.GetLastWriteTime(filePath));
                    }

                    application.Response.Write(css);
                    application.Response.End();
                }
                catch { /* swallow */ }
            }
            else if (!ResourceEngine.IsDebug() && context.Request.Url.AbsolutePath.ToLowerInvariant().EndsWith(".css"))
            {
                try
                {
                    var filePath = application.Server.MapPath(ResourceEngine.UnversionPath(context.Request.Url.AbsolutePath));
                    var css = System.IO.File.ReadAllText(filePath);

                    css = Yahoo.Yui.Compressor.CssCompressor.Compress(css);
                    application.Response.ContentType = "text/css";
                    application.Response.Cache.SetAllowResponseInBrowserHistory(true);
                    application.Response.Cache.SetExpires(DateTime.UtcNow.AddSeconds(cacheDuration));
                    application.Response.Cache.SetLastModified(System.IO.File.GetLastWriteTime(filePath));
                    application.Response.Write(css);
                    application.Response.End();
                }
                catch { /* swallow */ }
            }
            else if (!ResourceEngine.IsDebug() &&
                context.Request.Url.AbsolutePath.ToLowerInvariant().EndsWith(".js"))
            {
                try
                {
                    //TODO: check version
                    var filePath =
                        application.Server.MapPath(ResourceEngine.UnversionPath(context.Request.Url.AbsolutePath));
                    var lastFileMod = System.IO.File.GetLastWriteTime(filePath);
                    if (lastFileMod.ToString() == modifiedDate.ToString())
                    {
                        application.Response.StatusCode = 304;
                        application.Response.SuppressContent = true;
                    }
                    else
                    {
                        var js = System.IO.File.ReadAllText(filePath);
                        var compressor = new Yahoo.Yui.Compressor.JavaScriptCompressor(js, false);
                        js = compressor.Compress();

                        application.Response.ContentType = "text/javascript";
                        application.Response.Cache.SetAllowResponseInBrowserHistory(true);
                        application.Response.Cache.SetExpires(DateTime.UtcNow.AddSeconds(cacheDuration));
                        application.Response.Cache.SetLastModified(System.IO.File.GetLastWriteTime(filePath));

                        application.Response.Write(js);
                        application.Response.End();
                    }
                }
                catch { /* swallow */ }
            }
        }
    }
}
