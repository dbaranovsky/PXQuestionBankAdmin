using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using Bfw.Common.Database;
using Bfw.Common.Collections;
using Bfw.Common;

namespace Bfw.PX.PXPub.Components
{
    /// <summary>
    /// The purpose of this module is to perform url redirects based on the values in the PresentationAlias table
    /// </summary>
    public class AliasRedirectModule : IHttpModule
    {
        #region Properties

        #endregion

        #region Constructors

        #endregion

        #region AliasRedirectModule Members

        /// <summary>
        ///  Initializes the module and handles the requests
        /// </summary>
        /// <param name="app"></param>
        public void Init(System.Web.HttpApplication app)
        {
            app.BeginRequest += new EventHandler(OnBeginRequest);
        }

        /// <summary>
        /// performs the actual url redirection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBeginRequest(Object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = application.Context;

            var hostname = context.Request.Url.Host;

            var absolutepath = context.Request.Url.AbsolutePath;
            var spltHost = hostname.Split('.');
            var splHostLen = spltHost.Length;

            //lookup url contains the last two parts of the hostname and the rest of the url             
            var lookupUrl = string.Format("{0}.{1}{2}", spltHost[splHostLen - 2], spltHost[splHostLen - 1], absolutepath);

            //only check the database if it contains url like eportfolio/public
            if (lookupUrl.ToLower().Contains("eportfolio/public")) 
            {
                //convert the lookup url to MD5 hash for efficient search
                var lookupHash = MD5HashConversion.ConvertToMD5Hash(lookupUrl);

                //get the home page url based on the hashed lookup url            
                var homepageUrl = GetHomePageUrl(lookupHash);
                if (!homepageUrl.IsNullOrEmpty())
                {
                    context.Response.StatusCode = 302; // temporary redirect            
                    context.Response.AddHeader("Location", homepageUrl);
                    context.Response.End();
                }
            }                          
          
        }

        /// <summary>
        /// Looksup the provided lookup hash in the PresentationAlias table and returns the HomePage url
        /// </summary>
        /// <param name="lookupHash"></param>
        /// <returns></returns>
        private string GetHomePageUrl(string lookupHash)
        {
            var db = new DatabaseManager("PXData");

            try
            {
                db.StartSession();
                var searchType = "aliashash";
                var records = db.Query("SelectPresentationAlias @0, @1", searchType, lookupHash);

                if (!records.IsNullOrEmpty())
                {
                    return records.First().String("HomeUrl");                    
                }
                
            }
            finally
            {
                db.EndSession();
            }

            return string.Empty;
        }

        /// <summary>
        /// Disposes the resources used by the module
        /// </summary>
        public void Dispose()
        {

        }

        #endregion

    }
}
