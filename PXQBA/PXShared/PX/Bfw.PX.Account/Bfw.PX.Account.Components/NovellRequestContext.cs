using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Bfw.PX.Biz.Direct.Services;
using Bfw.Common.SSO;
using Bfw.Common.Logging;
using Bfw.Common.Caching;
using Bfw.PX.Account.Abstract;
using RAg.Net.RAWS.GetCourseSiteID;
using RAg.Net.RAWS.GetSiteUserData;

namespace Bfw.PX.Account.Components
{
    /// <summary>
    /// Represents all custom context information for this request
    /// </summary>
    public class NovellRequestContext : IRequestContext
    {
        #region Properties

        public SSOData SSOData { get; protected set; }

        public Uri PublicUrl { get; protected set; }
        public Uri SecureUrl { get; protected set; }
        public Uri TargetUrl { get; protected set; }
        public Bfw.Common.Logging.ILogger Logger { get; set; }
        public Bfw.Common.Logging.ITraceManager Tracer { get; set; }

        public ICacheProvider CacheProvider { get; set; }

        public BFW.RAg.Site SiteInfo { get; protected set; }

        public BFW.RAg.SiteUserData SiteUserInfo { get; protected set; }

        private ISSODataProvider SSODataProvider { get; set; }

        private bool InLocalTestMode = false;

        private string SecurePathElement;
        private string PublicPathElement;
        private int SecurePathElementIndex;
        private const string SiteDataCookieName = "SiteData";
        private const string SiteUserDataCookieName = "SiteUserData";
        private const string CheckSessionCookieName = "SessionChecked";

        #endregion

        public NovellRequestContext(ISSODataProvider ssoDataProvider, ICacheProvider cacheProvider)
        {
            SSODataProvider = ssoDataProvider;
            CacheProvider = cacheProvider;
        }

        #region Methods

        public void Init()
        {
            ExtractSSOData();
            string target = "";

            using (Tracer.DoTrace("Parse Target URL"))
            {
                if (System.Web.HttpContext.Current.Request.QueryString["target"] != null)
                {
                    target = System.Web.HttpContext.Current.Request.QueryString["target"];
                }
                if (!string.IsNullOrEmpty(target))
                {
                    if (!(target.IndexOf("http://") > -1) && !(target.IndexOf("https://") > -1))
                    {
                        target = "http://" + target;
                    }
                    Uri outUrl = null;
                    if (Uri.TryCreate(target, UriKind.Absolute, out outUrl))
                    {
                        TargetUrl = outUrl;
                    }
                    else
                    {
                        TargetUrl = null;
                    }
                }
            }

            System.Configuration.Configuration rootWebConfig1 =
                System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(System.Web.HttpContext.Current.Request.ApplicationPath);
            System.Configuration.KeyValueConfigurationElement ConfigInLocalTestMode =
                rootWebConfig1.AppSettings.Settings["NovellSSOInLocalTestMode"];
            if (ConfigInLocalTestMode != null)
            {
                if (ConfigInLocalTestMode.Value.ToLower() == "true")
                {
                    InLocalTestMode = true;
                }
            }
            // Construct Secure/Public URLs for current request
            System.Configuration.KeyValueConfigurationElement ConfigSecurePathElement =
                rootWebConfig1.AppSettings.Settings["NovellSSOSecurePathElement"];
            System.Configuration.KeyValueConfigurationElement ConfigPublicPathElement =
                rootWebConfig1.AppSettings.Settings["NovellSSOPublicPathElement"];
            System.Configuration.KeyValueConfigurationElement ConfigSecurePathElementIndex =
                rootWebConfig1.AppSettings.Settings["NovellSSOSecurePathElementIndex"];
            int trySecurePathElementIndex = -1;
            if (Int32.TryParse(ConfigSecurePathElementIndex.Value, out trySecurePathElementIndex))
            {
                using (Tracer.DoTrace("Determine Secure Path Element"))
                {
                    SecurePathElementIndex = trySecurePathElementIndex;
                    if (ConfigSecurePathElement != null && ConfigPublicPathElement != null)
                    {
                        SecurePathElement = ConfigSecurePathElement.Value;
                        PublicPathElement = ConfigPublicPathElement.Value;
                        StringBuilder SecureUrlString = new StringBuilder();
                        StringBuilder PublicUrlString = new StringBuilder();
                        bool LocalTestModeIsSecure = false;
                        for (int i = 0; i < System.Web.HttpContext.Current.Request.Url.Segments.Length; i++)
                        {
                            if (i == SecurePathElementIndex)
                            {
                                if (PublicPathElement == "" && System.Web.HttpContext.Current.Request.Url.Segments[i] != SecurePathElement)
                                {
                                    SecureUrlString.Append(SecurePathElement);
                                    //no need to add empty element to PublicUrl
                                    SecureUrlString.Append(System.Web.HttpContext.Current.Request.Url.Segments[i]);
                                    PublicUrlString.Append(System.Web.HttpContext.Current.Request.Url.Segments[i]);
                                }
                                else if (PublicPathElement == "/" && System.Web.HttpContext.Current.Request.Url.Segments[i] != SecurePathElement)
                                {
                                    SecureUrlString.Append(SecurePathElement);
                                    //don't add extra / in PublicUrl.
                                    SecureUrlString.Append(System.Web.HttpContext.Current.Request.Url.Segments[i]);
                                    PublicUrlString.Append(System.Web.HttpContext.Current.Request.Url.Segments[i]);
                                }
                                else
                                {
                                    SecureUrlString.Append(SecurePathElement);
                                    PublicUrlString.Append(PublicPathElement);
                                }
                                if (System.Web.HttpContext.Current.Request.Url.Segments[i] == SecurePathElement)
                                {
                                    LocalTestModeIsSecure = true;
                                }
                            }
                            else
                            {
                                SecureUrlString.Append(System.Web.HttpContext.Current.Request.Url.Segments[i]);
                                PublicUrlString.Append(System.Web.HttpContext.Current.Request.Url.Segments[i]);
                            }
                        }
                        if (InLocalTestMode && SSOData == null)
                        {
                            SSOData = new SSOData();
                            SSOData.IsProtected = LocalTestModeIsSecure;
                            SSOData.AuthSession = "IDQFAKE=FAKE";
                            SSOData.BrainHoneyAuth = "";
                            SSOData.DlapAuth = "";
                        }
                        if (SSOData != null)
                        {
                            if (SSOData.IsProtected)
                            {
                                string holdPubUrl = PublicUrlString.ToString();
                                PublicUrlString = new StringBuilder(System.Web.HttpContext.Current.Request.Url.OriginalString.Replace(SecureUrlString.ToString(), PublicUrlString.ToString()));
                                SecureUrlString = new StringBuilder(System.Web.HttpContext.Current.Request.Url.OriginalString.Replace(holdPubUrl, SecureUrlString.ToString()));
                            }
                            else
                            {
                                string holdPubUrl = PublicUrlString.ToString();
                                PublicUrlString = new StringBuilder(System.Web.HttpContext.Current.Request.Url.OriginalString.Replace(SecureUrlString.ToString(), PublicUrlString.ToString()));
                                SecureUrlString = new StringBuilder(System.Web.HttpContext.Current.Request.Url.OriginalString.Replace(holdPubUrl, SecureUrlString.ToString()));
                            }
                        }
                        PublicUrl = new Uri(PublicUrlString.ToString());
                        SecureUrl = new Uri(SecureUrlString.ToString());
                    }
                }
            }
            StringBuilder debugMsg = new StringBuilder(string.Format("RequestContext\nRequest Url: {0}\nPublicUrl: {1}\nSecureUrl: {2}", HttpContext.Current.Request.Url.ToString(), PublicUrl.ToString(), SecureUrl.ToString()));
            StringBuilder redirectUrl = new StringBuilder();
            if (SSOData != null)
            {
                using (Tracer.DoTrace("SSOData not null"))
                {
                    if (SSOData.AuthSession != null)
                    {
                        using (Tracer.DoTrace("AuthSession Not Null"))
                        {
                            var checkForEqualsChar = (SSOData.AuthSession.IndexOf("=") > 0);
                            var checkIsEmpty = string.IsNullOrEmpty(SSOData.AuthSession);
                            var checkLength = SSOData.AuthSession.Length;
                            var checkEqualsPos = SSOData.AuthSession.IndexOf("=") + 1;
                            var checkUidValid = !string.IsNullOrEmpty(SSOData.UserId);
                            var sessionCookieChecked = false;
                            System.Web.HttpCookie checkSessionCookie = System.Web.HttpContext.Current.Request.Cookies.Get(CheckSessionCookieName);
                            if (checkSessionCookie != null)
                            {
                                if (checkSessionCookie.Value == "1")
                                {
                                    sessionCookieChecked = true;
                                }
                            }
                            debugMsg.Append(string.Format("\n - SSOData.IsProtected = {0}\n - SSOData.AuthSession value = {1}\n - checkIsEmpty = {2}\n - checkLength = {3}\n - checkForEqualsChar = {4}\n - checkEqualsPos = {5}\n - checkUidValid = {6}", SSOData.IsProtected, SSOData.AuthSession, checkIsEmpty, checkLength, checkForEqualsChar, checkEqualsPos, checkUidValid));
                            if (!SSOData.IsProtected && !sessionCookieChecked)
                            {
                                SetDomainCookie(CheckSessionCookieName, "1");
                                debugMsg.Append("\n - case 1.3a setting CheckSessionCookie and redirect to SecureUrl to check session");
                                redirectUrl = new StringBuilder(SecureUrl.ToString());
                            }
                            else if (SSOData.IsProtected && !checkUidValid && (checkIsEmpty || !checkForEqualsChar || (checkLength == checkEqualsPos)))
                            {
                                debugMsg.Append("\n - case 1.1 redirecting to PublicUrl");
                                redirectUrl = new StringBuilder(PublicUrl.ToString());
                            }
                            else if (!SSOData.IsProtected && checkUidValid && !checkIsEmpty && checkForEqualsChar && (checkLength > checkEqualsPos))
                            {
                                debugMsg.Append("\n - case 1.2 redirecting to SecureUrl");
                                redirectUrl = new StringBuilder(SecureUrl.ToString());
                            }
                            else if (!SSOData.IsProtected && !checkUidValid && (checkIsEmpty || !checkForEqualsChar || (checkLength == checkEqualsPos)))
                            {
                                if (!sessionCookieChecked)
                                {
                                    SetDomainCookie(CheckSessionCookieName, "1");
                                    debugMsg.Append("\n - case 1.3a setting CheckSessionCookie and redirect to SecureUrl to check session");
                                    redirectUrl = new StringBuilder(SecureUrl.ToString());
                                }
                                else
                                {
                                    debugMsg.Append("\n - case 1.3b no redirect to SecureUrl");
                                }
                            }
                            else if (SSOData.IsProtected && checkUidValid && !checkIsEmpty && checkForEqualsChar && (checkLength > checkEqualsPos))
                            {
                                debugMsg.Append("\n - case 1.4 no redirect to PublicUrl");
                            }
                            else
                            {
                                debugMsg.Append("\n - case 1.0, no redirect");
                            }
                        }
                    }
                    else
                    {
                        if (SSOData.IsProtected)
                        {
                            debugMsg.Append("\n - case 2.1 redirecting to PublicUrl");
                            redirectUrl = new StringBuilder(PublicUrl.ToString());
                        }
                        else
                        {
                            debugMsg.Append("\n - case 2.0, no redirect");
                        }
                    }
                }
            }
            else
            {
                debugMsg.Append("\n - SSOData is null");
            }
            debugMsg.Append(string.Format("\n - redirect URL: {0}", redirectUrl.ToString()));
            using (Tracer.DoTrace("Output case message"))
            {
                Debug(debugMsg.ToString());
            }

            if (!string.IsNullOrEmpty(redirectUrl.ToString()))
            {
                using (Tracer.DoTrace("Redirecting"))
                {
                    System.Web.HttpContext.Current.Response.Redirect(redirectUrl.ToString());
                }
            }

            System.Web.HttpCookie SiteDataCookie = null;
            using (Tracer.DoTrace("Dumping Request cookies"))
            {
                //             RaSiteInfo = new BFW.RAg.Site();
                SiteInfo = null;
                SiteUserInfo = null;
                // get RaSiteInfo from cookie
                SiteDataCookie = System.Web.HttpContext.Current.Request.Cookies.Get(SiteDataCookieName);
                var cookies = System.Web.HttpContext.Current.Request.Cookies;
                var sb = new StringBuilder();
                sb.AppendLine("RequestContext Cookies");
                for (int i = 0; i < cookies.Count; ++i)
                {
                    sb.AppendFormat("{{name: {0}, value: {1}, path: {2} }}\n", cookies[i].Name, HttpUtility.UrlDecode(cookies[i].Value), cookies[i].Path);
                }
                Debug(sb.ToString());
            }

            using (Tracer.DoTrace("Pre Deserialize cookie"))
            {
                if (SiteDataCookie != null)
                {
                    Debug(string.Format("RequestContext got SiteDataCookie with value {0}", SiteDataCookie.Value));
                    try
                    {
                        var jser = new JavaScriptSerializer();
                        using(Tracer.DoTrace("Deserialize Cookie"))
                        {
                            SiteInfo = jser.Deserialize<BFW.RAg.Site>(HttpUtility.UrlDecode(SiteDataCookie.Value));
                        }
                        Debug("RequestContext SiteInfo deserialized");
                    }
                    catch (Exception ex)
                    {
                        Debug(string.Format("RequestContext SiteInfo could not deserialize: {0}", ex.Message));
                        throw new Exception("Could not deserialize SiteData cookie JSON object", ex);
                    }
                }
            }

            // check that RaSiteInfo from cookie matches TargetUrl or current Url
            if (SiteInfo != null)
            {
                if (TargetUrl != null)
                {
                    Debug(string.Format("RequestContext TargetUrl = {0}", TargetUrl));
                    if (!(TargetUrl.ToString().IndexOf(SiteInfo.BaseURL) > -1))
                    {
                        Debug(string.Format("RequestContext could not find baseurl {0} in TargetUrl", SiteInfo.BaseURL));
                        SiteInfo = null;
                    }
                }
                else
                {
                    var raw = System.Web.HttpContext.Current.Request.Url;
                    var pubUrl = MakePublicUrl(raw);

                    if (!(pubUrl.IndexOf(SiteInfo.BaseURL) > -1))
                    {
                        Debug(string.Format("RequestContext could not find baseurl {0} in pubUrl {1} based on uri {2}", SiteInfo.BaseURL, pubUrl, raw));
                        SiteInfo = null;
                    }
                }
            }
            if (SiteInfo != null)
            {
                using (Tracer.DoTrace("SiteInfo not null"))
                {
                    // get RaSiteUserData from cookie
                    System.Web.HttpCookie SiteUserDataCookie = System.Web.HttpContext.Current.Request.Cookies.Get(SiteUserDataCookieName);
                    if (SiteUserDataCookie != null)
                    {
                        Debug(string.Format("RequestContext SiteUserData Cookie value = {0}", SiteDataCookie.Value));
                        try
                        {
                            var jser = new JavaScriptSerializer();
                            SiteUserInfo = jser.Deserialize<BFW.RAg.SiteUserData>(HttpUtility.UrlDecode(SiteUserDataCookie.Value));
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Could not deserialize SiteUserData cookie JSON object", ex);
                        }
                        // check that RaSiteUserData from cookie matches current RaSiteInfo

                        if (SiteUserInfo != null)
                        {
                            if (InLocalTestMode)
                            {
                                SSOData.UserId = Convert.ToString(SiteUserInfo.UserID);
                                SSOData.User = new SSOUser();
                                SSOData.User.CustomerID = "1";
                                SSOData.User.FirstName = "Fake";
                                SSOData.User.LastName = "User";
                                SSOData.User.Email = "fake@bfwpub.com";
                            }

                            if (SiteInfo.SiteID != SiteUserInfo.SiteID)
                            {
                                SiteUserInfo = null;
                            }
                        }
                    }
                    else
                    {
                        Debug("RequestContext SiteUserData Cookie value is null");
                    }
                }
            }
            else
            {
                Debug("RequestContext SiteInfo cookie is null");
            }
        }

        private void ExtractSSOData()
        {
            using (Tracer.DoTrace("ExtractSSOData"))
            {
                SSOData = SSODataProvider.GetData(System.Web.HttpContext.Current.Request);
            }
        }

        public void RedirectToTarget()
        {
            if (TargetUrl != null)
            {
                System.Web.HttpContext.Current.Response.Redirect(TargetUrl.ToString());
            }
        }

        /// <summary>
        /// Loads data from either a cookie or RA service(s) using the current request URL
        /// </summary>
        public void InitSiteData()
        {
            using (Tracer.DoTrace("InitSiteData"))
            {
                if (PublicUrl != null)
                {
                    Debug(string.Format("InitSiteData with PublicUrl = {0}", PublicUrl));
                    InitSiteData(PublicUrl);
                }
                else
                {
                    var rawUrl = System.Web.HttpContext.Current.Request.RawUrl;
                    Debug(string.Format("InitSiteData with RawUrl = {0}", rawUrl));
                    InitSiteData(new Uri(rawUrl));
                }
            }
        }

        /// <summary>
        /// Loads data from either a cookie or RA service(s) using the input URL
        /// </summary>
        /// <param name="Url"></param>
        public void InitSiteData(Uri Url)
        {
            using (Tracer.DoTrace("InitSiteData(url={0})", Url))
            {
                if (Url != null)
                {
                    // set up Site data
                    if (SiteInfo != null)
                    {
                        if (!(Url.ToString().IndexOf(SiteInfo.BaseURL) > -1))
                        {
                            GetSiteData(Url);
                            SiteUserInfo = null;
                        }
                    }
                    else
                    {
                        GetSiteData(Url);
                        SiteUserInfo = null;
                    }

                    // set up Site User data
                    string checkCase = "0";
                    if (SSOData == null)
                    {
                        checkCase = "1";
                        SiteUserInfo = null;
                        ClearSiteUserDataCookie();
                    }
                    else if (string.IsNullOrEmpty(SSOData.UserId))
                    {
                        checkCase = "2";
                        SiteUserInfo = null;
                        ClearSiteUserDataCookie();
                    }
                    else if (SiteInfo == null)
                    {
                        checkCase = "3";
                        SiteUserInfo = null;
                        ClearSiteUserDataCookie();
                    }
                    else if (SiteUserInfo == null)
                    {
                        checkCase = "4";
                        GetSiteUserData();
                    }
                    else if (SiteUserInfo.SiteID != SiteInfo.SiteID)
                    {
                        checkCase = "5";
                        SiteUserInfo = null;
                        GetSiteUserData();
                    }
                    else if (Convert.ToString(SiteUserInfo.UserID) != SSOData.UserId)
                    {
                        checkCase = "6";
                        SiteUserInfo = null;
                        GetSiteUserData();
                    }
                    else
                    {
                        checkCase = "7";
                        //                     RaSiteUserData = null;
                        //                     GetSiteUserData();
                    }
                    Debug(string.Format("InitSiteData ran through check case #{0}", checkCase));
                }
            }
        }

        /// <summary>
        /// Clears and re-inits data
        /// </summary>
        public void ReInitSiteData()
        {
            using (Tracer.DoTrace("ReInitSiteData"))
            {
                if (PublicUrl != null)
                {
                    ReInitSiteData(PublicUrl);
                }
                else
                {
                    ReInitSiteData(new Uri(System.Web.HttpContext.Current.Request.RawUrl));
                }
            }
        }

        /// <summary>
        /// Clears and re-inits data
        /// </summary>
        /// <param name="Url"></param>
        public void ReInitSiteData(Uri Url)
        {
            using (Tracer.DoTrace("ReInitSiteData(url)"))
            {
                SiteInfo = null;
                SetCookie(SiteDataCookieName, "");
                SiteUserInfo = null;
                SetCookie(SiteUserDataCookieName, "");
                InitSiteData(Url);
            }
        }

        /// <summary>
        /// Loads site data from RA service(s) using the input URL
        /// </summary>
        private void GetSiteData(Uri Url)
        {
            using (Tracer.DoTrace("GetSiteData(url={0})", Url))
            {
                ClearSiteDataCookie();
                RAg.Net.RAWS.GetCourseSiteID.SiteInfo RawsSiteInfo = null;
                var raws = new RAg.Net.RAWS.GetCourseSiteID.RAGetAgilixCourseIDSoapClient();
                var url = AdjustForSubdomain(Url.ToString());

                using (Tracer.DoTrace("GetAgilixCourseID from RAWS"))
                {
                    try
                    {
                        RawsSiteInfo = raws.GetAgilixCourseID(url);
                    }
                    catch (System.Exception ex)
                    {
                        Debug(string.Format("GetSiteData failed: {0}", ex.Message));
                    }
                }

                if (RawsSiteInfo != null)
                {
                    string cookieVal = "";
                    SiteInfo = new BFW.RAg.Site();
                    if (String.IsNullOrEmpty(RawsSiteInfo.BaseURL))
                    {
                        SiteInfo.BaseURL = "";
                    }
                    else
                    {
                        SiteInfo.BaseURL = RawsSiteInfo.BaseURL;
                    }
                    int intVal = 0;
                    SiteInfo.AgilixCourseId = 0;
                    if (Int32.TryParse(RawsSiteInfo.AgilixSiteID, out intVal))
                    {
                        Debug(string.Format("Setting SiteInfo.AgilixSiteID = {0}", intVal));
                        SiteInfo.AgilixCourseId = intVal;
                    }
                    else
                    {
                        Debug(string.Format("Could not parse value {0} to be SiteInfo.AgilixSiteID", RawsSiteInfo.AgilixSiteID));
                    }
                    SiteInfo.SiteID = 0;
                    if (Int32.TryParse(RawsSiteInfo.SiteID, out intVal))
                    {
                        SiteInfo.SiteID = intVal;
                    }
                    try
                    {
                        var jser = new JavaScriptSerializer();
                        cookieVal = jser.Serialize(SiteInfo);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Could not serialize SiteData cookie JSON object", ex);
                    }
                    SetCookie(SiteDataCookieName, cookieVal);
                }
                else
                {
                    Debug("RawSiteInfo is NULL");
                }
            }
        }

        /// <summary>
        /// Loads user's site data from RA service(s) using the input URL
        /// </summary>
        private void GetSiteUserData()
        {
            using (Tracer.DoTrace("GetSiteUserData"))
            {
                ClearSiteUserDataCookie();
                if (SiteInfo != null && SSOData != null)
                {
                    if (!string.IsNullOrEmpty(SSOData.UserId))
                    {                        
                        Debug(string.Format("Calling RAWS GetSiteUserData with SSOData.UserId: {0}", SSOData.UserId));
                        RAg.Net.RAWS.GetSiteUserData.SiteUserData RawsSiteUserData = null;

                        var exp = "/(?<courseid>[0-9]+)/?";
                        var match = System.Text.RegularExpressions.Regex.Match(SecureUrl.ToString(), exp);
                        var foundCourseId = string.Empty;
                        if (match.Success)
                        {
                            foundCourseId = match.Groups["courseid"].Value;
                        }


                        using (Tracer.DoTrace("GetSiteUserData from RAWS"))
                        {
                            object cachedObject = null;

                            if (!string.IsNullOrEmpty(foundCourseId))
                            {
                                cachedObject = CacheProvider.FetchRASiteUserData(SSOData.UserId, foundCourseId);
                            }

                            if (cachedObject == null)
                            {
                                using (Tracer.DoTrace("From Service {0}:{1}", SSOData.UserId, foundCourseId))
                                {
                                    try
                                    {
                                        var rawsSiteUserDataService = new RAg.Net.RAWS.GetSiteUserData.RAGetSiteUserDataSoapClient();
                                        RawsSiteUserData = rawsSiteUserDataService.GetSiteUserData(SiteInfo.SiteID, Convert.ToInt32(SSOData.UserId), System.Web.HttpContext.Current.Request.UserHostAddress);
                                        CacheProvider.StoreRASiteUserData(SSOData.UserId, foundCourseId, RawsSiteUserData);
                                      
                                    }
                                    catch (System.Exception ex)
                                    {
                                        Debug(string.Format("GetSiteUserData failed: {0}", ex.Message));
                                    }
                                }
                            }
                            else
                            {
                                using (Tracer.DoTrace("From Cache {0}:{1}", SSOData.UserId, foundCourseId))
                                {
                                    RawsSiteUserData = cachedObject as RAg.Net.RAWS.GetSiteUserData.SiteUserData;
                                }
                            }
                        }

                        if (RawsSiteUserData != null)
                        {
                            SiteUserInfo = new BFW.RAg.SiteUserData();
                            int intVal = 0;
                            SiteUserInfo.SiteID = SiteInfo.SiteID;
                            SiteUserInfo.UserID = 0;
                            if (Int32.TryParse(RawsSiteUserData.UserID, out intVal))
                            {
                                SiteUserInfo.UserID = intVal;
                            }
                            SiteUserInfo.LevelOfAccess = 10;
                            if (Int32.TryParse(RawsSiteUserData.LevelOfAccess, out intVal))
                            {
                                SiteUserInfo.LevelOfAccess = intVal;
                            }
                            SiteUserInfo.InstructorEmail = RawsSiteUserData.InstructorEmail;
                            SiteUserInfo.Expiration = new DateTime();
                            DateTime dateVal = new DateTime();
                            if (DateTime.TryParse(RawsSiteUserData.Expiration, out dateVal))
                            {
                                SiteUserInfo.Expiration = dateVal;
                            }
                            string cookieVal = "";
                            try
                            {
                                var jser = new JavaScriptSerializer();
                                cookieVal = jser.Serialize(SiteUserInfo);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Could not serialize SiteUserData cookie JSON object", ex);
                            }
                            SetCookie(SiteUserDataCookieName, cookieVal);
                            if (!SSOData.IsProtected && !string.IsNullOrEmpty(SSOData.UserId))
                            {
                                System.Web.HttpContext.Current.Response.Redirect(SecureUrl.ToString());
                            }
                            else if (SSOData.IsProtected && string.IsNullOrEmpty(SSOData.UserId))
                            {
                                System.Web.HttpContext.Current.Response.Redirect(PublicUrl.ToString());
                            }
                        }
                        else
                        {
                            Debug("RawsSiteUserData is null");
                        }
                    }
                    else
                    {
                        Debug("SSOData.UserId is null");
                    }
                }
            }
        }

        private void ClearSiteDataCookie()
        {
            SetCookie(SiteDataCookieName, "");
        }

        private void ClearSiteUserDataCookie()
        {
            SetCookie(SiteUserDataCookieName, "");
        }

        private string MakePublicUrl(Uri uri)
        {
            var pubUrl = new StringBuilder();
            var segment = string.Empty;

            using (Tracer.DoTrace("MakePublicUrl"))
            {
                for (int i = 0; i < uri.Segments.Length; ++i)
                {
                    segment = uri.Segments[i];
                    if (i != SecurePathElementIndex)
                    {
                        pubUrl.Append(segment);
                    }
                    else if (segment.ToLower() != SecurePathElement.ToLower())
                    {
                        pubUrl.Append(segment);
                    }
                }

                if (uri.IsAbsoluteUri)
                {
                    pubUrl.Insert(0, uri.Host);
                }
            }

            return pubUrl.ToString();
        }

        /// <summary>
        /// Sets cookie value at multiple paths.  
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        private void SetCookie(string Name, string Value)
        {
            using (Tracer.DoTrace("SetCookie"))
            {
                string cookiePathPublic = "/";
                string cookiePathSecure = "/";
                if (System.Web.HttpContext.Current.Request.Url.Host != "localhost")
                {
                    if (SiteInfo != null)
                    {
                        if (SSOData != null)
                        {
                            Uri BaseURLUri = new Uri("http://" + SiteInfo.BaseURL, UriKind.Absolute);
                            StringBuilder testBaseUrlSecureString = new StringBuilder();
                            StringBuilder testBaseUrlPublicString = new StringBuilder();
                            StringBuilder testMsg = new StringBuilder();
                            if (BaseURLUri.IsBaseOf(System.Web.HttpContext.Current.Request.Url))
                            {
                                testMsg.Append("1:");
                                for (int i = 0; i < BaseURLUri.Segments.Length; i++)
                                {
                                    testMsg.Append("i=" + Convert.ToString(i) + ":");
                                    if (i == SecurePathElementIndex)
                                    {
                                        testMsg.Append("1:");
                                        if ((PublicPathElement == "" || PublicPathElement == "/") && BaseURLUri.Segments[i] != SecurePathElement)
                                        {
                                            testMsg.Append("1:");
                                            testBaseUrlSecureString.Append(SecurePathElement);
                                            testBaseUrlSecureString.Append(BaseURLUri.Segments[i]);
                                            testBaseUrlPublicString.Append(BaseURLUri.Segments[i]);
                                        }
                                        else if (PublicPathElement != "" && BaseURLUri.Segments[i] != SecurePathElement)
                                        {
                                            testMsg.Append("2:");
                                            testBaseUrlSecureString.Append(SecurePathElement);
                                            testBaseUrlPublicString.Append(BaseURLUri.Segments[i]);
                                        }
                                        else
                                        {
                                            testMsg.Append("3:");
                                            testBaseUrlSecureString.Append(BaseURLUri.Segments[i]);
                                        }
                                    }
                                    else
                                    {
                                        testMsg.Append("2:");
                                        testBaseUrlSecureString.Append(BaseURLUri.Segments[i]);
                                        testBaseUrlPublicString.Append(BaseURLUri.Segments[i]);
                                    }
                                }
                            }
                            else
                            {
                                testMsg.Append("2:");
                                for (int i = 0; i <= SecurePathElementIndex && i < System.Web.HttpContext.Current.Request.Url.Segments.Length; i++)
                                {
                                    testMsg.Append("i=" + Convert.ToString(i) + ":");
                                    if (i == SecurePathElementIndex)
                                    {
                                        testMsg.Append("1:");
                                        if ((PublicPathElement == "" || PublicPathElement == "/") && System.Web.HttpContext.Current.Request.Url.Segments[i] != SecurePathElement)
                                        {
                                            testMsg.Append("1:");
                                            testBaseUrlSecureString.Append(SecurePathElement);
                                            testBaseUrlSecureString.Append(System.Web.HttpContext.Current.Request.Url.Segments[i]);
                                            testBaseUrlPublicString.Append(System.Web.HttpContext.Current.Request.Url.Segments[i]);
                                        }
                                        else if (PublicPathElement != "" && System.Web.HttpContext.Current.Request.Url.Segments[i] != SecurePathElement)
                                        {
                                            testMsg.Append("2:");
                                            testBaseUrlSecureString.Append(SecurePathElement);
                                            testBaseUrlPublicString.Append(System.Web.HttpContext.Current.Request.Url.Segments[i]);
                                        }
                                        else
                                        {
                                            testMsg.Append("3:");
                                            testBaseUrlSecureString.Append(System.Web.HttpContext.Current.Request.Url.Segments[i]);
                                        }
                                    }
                                    else
                                    {
                                        testMsg.Append("2:");
                                        testBaseUrlSecureString.Append(System.Web.HttpContext.Current.Request.Url.Segments[i]);
                                        testBaseUrlPublicString.Append(System.Web.HttpContext.Current.Request.Url.Segments[i]);
                                    }
                                }
                            }
                            cookiePathPublic = testBaseUrlPublicString.ToString();
                            cookiePathSecure = testBaseUrlSecureString.ToString();
                            testMsg.Append(cookiePathPublic + ":");
                            testMsg.Append(cookiePathSecure + "!!");
                            Debug(string.Format("RequestContext testMsg: ", testMsg.ToString()));
                        }
                    }
                }

                //read cookie expiration from web.config
                System.Configuration.Configuration rootWebConfig1 =
                System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(System.Web.HttpContext.Current.Request.ApplicationPath);
                System.Configuration.KeyValueConfigurationElement NovellSSO_CookieExpiration = rootWebConfig1.AppSettings.Settings["NovellSSO_CookieExpiration"];

                //added expiration to try and work around the FF bug with Save and Quit
                //see: https://bugzilla.mozilla.org/show_bug.cgi?id=443354
                DateTime cookieExpirTime = DateTime.Now;

                if (NovellSSO_CookieExpiration != null)
                {
                    cookieExpirTime = DateTime.Now.AddMinutes(Int32.Parse(NovellSSO_CookieExpiration.Value));
                } 
                else
                {
                    cookieExpirTime = DateTime.Now.AddMinutes(60);
                }

                
	                System.Web.HttpCookie ThePublicCookie = new System.Web.HttpCookie(Name);
	                ThePublicCookie.Value = HttpUtility.UrlEncode(Value);
	                ThePublicCookie.Path = cookiePathPublic;
	                ThePublicCookie.Expires = cookieExpirTime;
	                System.Web.HttpContext.Current.Response.Cookies.Add(ThePublicCookie);
	
	                System.Web.HttpCookie TheSecureCookie = new System.Web.HttpCookie(Name);
	                TheSecureCookie.Value = HttpUtility.UrlEncode(Value);
	                TheSecureCookie.Path = cookiePathSecure;
	                TheSecureCookie.Expires = cookieExpirTime;
	                System.Web.HttpContext.Current.Response.Cookies.Add(TheSecureCookie);
                
            }
        }

        /// <summary>
        /// Sets cookie value at current domain.  
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        private void SetDomainCookie(string Name, string Value)
        {
            using (Tracer.DoTrace("SetDomainCookie"))
            {
                Debug(string.Format("SetDomainCookie: {0} = {1}", Name, Value));


                //read cookie expiration from web.config
                System.Configuration.Configuration rootWebConfig1 =
                System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(System.Web.HttpContext.Current.Request.ApplicationPath);
                System.Configuration.KeyValueConfigurationElement NovellSSO_CookieExpiration = rootWebConfig1.AppSettings.Settings["NovellSSO_CookieExpiration"];

                //added expiration to try and work around the FF bug with Save and Quit
                //see: https://bugzilla.mozilla.org/show_bug.cgi?id=443354

                DateTime cookieExpirTime = DateTime.Now;
                if (NovellSSO_CookieExpiration != null)
                {
                    cookieExpirTime = DateTime.Now.AddMinutes(Int32.Parse(NovellSSO_CookieExpiration.Value));
                }
                else
                {
                    cookieExpirTime = DateTime.Now.AddMinutes(60);
                }

                System.Web.HttpCookie TheCookie = new System.Web.HttpCookie(Name);
                TheCookie.Value = HttpUtility.UrlEncode(Value);
                TheCookie.Path = "/";
                TheCookie.Expires = cookieExpirTime;
                System.Web.HttpContext.Current.Response.Cookies.Add(TheCookie);
            }
        }

        private void Debug(string message)
        {
            using (Tracer.DoTrace("Debug"))
            {
                if (Logger != null)
                {
                    //Logger.Log(message, Bfw.Common.Logging.LogSeverity.Information);
                }
            }
        }

        /// <summary>
        /// Adds the missing www subdomain to the URL
        /// </summary>  
        private string AdjustForSubdomain(String url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                if (!url.Contains("://"))
                    url = "http://" + url;

                Uri uri = new Uri(url);
                string host = uri.Host;

                string subdomain = host.Split('.').FirstOrDefault();
                subdomain = subdomain.ToLower();

                string allowedSubdomain = System.Configuration.ConfigurationManager.AppSettings["AllowedSubdomains"];
                List<string> allowedSubdomains = allowedSubdomain.Split(',').ToList();

                if (!string.IsNullOrEmpty(subdomain) && !allowedSubdomains.Contains(subdomain))
                    host = "www." + host;

                if (uri.Host != host)
                    url = url.Replace(uri.Host, host);
            }
            return url;
        }

        #endregion
    }
}
