using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Bfw.Common.Patterns.Unity
{
    /// <summary>
    /// Manages lifetime of objects in unity for per request
    /// </summary>
    public class UnityPerWebRequestLifetimeModule : IHttpModule
    {
        private static readonly object Key = new object();
        private HttpContextBase _httpContext;

        /// <summary>
        /// This constructor is needed for Unit Test
        /// </summary>
        /// <param name="httpContext"></param>
        public UnityPerWebRequestLifetimeModule(HttpContextBase httpContext)
        {
            _httpContext = httpContext;
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public UnityPerWebRequestLifetimeModule()
        {
        }

        internal IDictionary<UnityPerWebRequestLifetimeManager, object> Instances
        {
            get
            {
                _httpContext = (HttpContext.Current != null) ? new HttpContextWrapper(HttpContext.Current) : _httpContext;

                return (_httpContext == null) ? null : GetInstances(_httpContext);
            }
        }

        void IHttpModule.Init(HttpApplication context)
        {
            context.EndRequest += (sender, e) => RemoveAllInstances();
        }

        void IHttpModule.Dispose()
        {
        }

        internal static IDictionary<UnityPerWebRequestLifetimeManager, object> GetInstances(HttpContextBase httpContext)
        {
            IDictionary<UnityPerWebRequestLifetimeManager, object> instances;

            if (httpContext.Items.Contains(Key))
            {
                instances = (IDictionary<UnityPerWebRequestLifetimeManager, object>)httpContext.Items[Key];
            }
            else
            {
                lock (httpContext.Items)
                {
                    if (httpContext.Items.Contains(Key))
                    {
                        instances = (IDictionary<UnityPerWebRequestLifetimeManager, object>)httpContext.Items[Key];
                    }
                    else
                    {
                        int concurrencyLevel = Environment.ProcessorCount * 2;
                        int capacity = 100;
                        instances = new System.Collections.Concurrent.ConcurrentDictionary<UnityPerWebRequestLifetimeManager, object>(concurrencyLevel, capacity);
                        httpContext.Items.Add(Key, instances);
                    }
                }
            }

            return instances;
        }

        internal void RemoveAllInstances()
        {
            IDictionary<UnityPerWebRequestLifetimeManager, object> instances = Instances;

            if (instances != null)
            {
                foreach (KeyValuePair<UnityPerWebRequestLifetimeManager, object> entry in instances)
                {
                    IDisposable disposable = entry.Value as IDisposable;

                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }

                instances.Clear();
            }
        }
    }

}
