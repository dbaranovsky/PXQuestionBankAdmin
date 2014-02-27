
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Xml.XPath;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common;
using Bfw.Common.Caching;
using Bfw.Common.Collections;
using Bfw.Common.Logging;
using Bfw.Common.SSO;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.PX.Biz.Direct.Services;
using PxWebUser;
using Adc = Bfw.Agilix.DataContracts;


namespace Bfw.PX.PXPub.Components
{
	/// <summary>
	/// Base class representing the information we get from the context to be used in business logic.
	/// </summary>
	public abstract class BusinessContextBase : IBusinessContext
	{
		#region IBusinessContext Members
		/// <summary>
		/// a regular expression that makes sure the string only has digits '\d+'
		/// </summary>
		private Regex NumberValidator = new Regex(@"^\d+");

		/// <summary>
		/// Gets or sets the current user.
		/// </summary>
		/// <value>
		/// The current user.
		/// </value>
		public UserInfo CurrentUser { get; set; }

		/// <summary>
		/// Gets or sets the SSO data.
		/// </summary>
		/// <value>
		/// The SSO data.
		/// </value>
		public SSOData SSOData { get; protected set; }

	    private List<Domain> _raUserDomains;  

		/// <summary>
		/// Gets or sets a value indicating whether this instance is anonymous.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is anonymous; otherwise, <c>false</c>.
		/// </value>
		public bool IsAnonymous { get; protected set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is public view.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is public view; otherwise, <c>false</c>.
		/// </value>
		public bool IsPublicView { get; protected set; }

		/// <summary>
		/// Gets or sets the domain. 
		/// </summary>
		/// <value>
		/// The domain.
		/// </value>
		public Domain Domain { get; protected set; }


		/// <summary>
		/// A private variable that stores the Entity Id.
		/// </summary>
		private string _dashboardCourseId = null;

		/// <summary>
		/// Gets or sets the dashboard course id. 
		/// </summary>
		/// <value>
		/// The domain.
		/// </value>
		public string DashboardCourseId
		{
			get
			{
				return _dashboardCourseId;
			}
			set
			{
				_dashboardCourseId = value;
			}

		}


		/// <summary>
		/// A private variable that stores the Entity Id.
		/// </summary>
		private string _entityId = null;

		/// <summary>
		/// Gets or sets the entity id into _entityId.
		/// </summary>
		/// <value>
		/// The entity id.
		/// </value>
		public string EntityId
		{
			get
			{
				if (string.IsNullOrEmpty(_entityId))
				{
					if (Course != null)
					{
						_entityId = CourseId;
					}
					else if (Product != null)
					{
						_entityId = ProductCourseId;
					}
					else
					{
						_entityId = string.Empty;
					}
				}

				return _entityId;
			}
			set
			{
				_entityId = value;
			}
		}

		/// <summary>
		/// Gets or sets the enrollment id.
		/// </summary>
		/// <value>
		/// The enrollment id.
		/// </value>
		public string EnrollmentId { get; set; }       

		/// <summary>
        /// Gets or sets the enrollment status.
        /// </summary>
        public string EnrollmentStatus { get; set; }

		/// <summary>
		/// Gets or sets the environment URL.
		/// </summary>
		/// <value>
		/// The environment URL.
		/// </value>
		public string EnvironmentUrl { get; protected set; }

		/// <summary>
		/// Gets or sets the app domain URL.
		/// </summary>
		/// <value>
		/// The app domain URL.
		/// </value>
		public string AppDomainUrl { get; protected set; }

		/// <summary>
		/// Gets or sets the external resource base URL.
		/// </summary>
		/// <value>
		/// The external resource base URL.
		/// </value>
		public string ExternalResourceBaseUrl { get; protected set; }

		/// <summary>
		/// Gets or sets the dev URL.
		/// </summary>
		/// <value>
		/// The dev URL.
		/// </value>
		public string DevUrl { get; protected set; }


		public static string HTSEquationImageUrl
		{
			get
			{
				return ConfigurationManager.AppSettings["PxHTSEqImageUrl"];
				//TODO: change for different environments
			}
		}
		/// <summary>
		/// A private variable that stores the hts editor url.
		/// </summary>
		private string _htsEditorUrl = null;

		/// <summary>
		/// Gets or sets the HTS Editor URL.
		/// </summary>
		/// <value>
		/// The HTS Editor URL.
		/// </value>
		public string HTSEditorUrl
		{
			get
			{
				if (_htsEditorUrl == null)
				{
					_htsEditorUrl = ConfigurationManager.AppSettings["PxHTSEditorUrl"];
				}
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
				_htsEditorUrl = _htsEditorUrl.Replace("[company]", documentDomain);

				return _htsEditorUrl;
			}
			set
			{
				_htsEditorUrl = value;
			}
		}

		/// <summary>
		/// Gets or sets the proxy URL.
		/// </summary>
		/// <value>
		/// The proxy URL.
		/// </value>
		public string ProxyUrl { get; protected set; }

		/// <summary>
		/// Gets or sets the BrainHoney URL
		/// </summary>
		public string BrainHoneyUrl { get; protected set; }

		/// <summary>
		/// Gets or sets the discussion prefix.
		/// </summary>
		/// <value>
		/// The discussion prefix.
		/// </value>
		public string DiscussionPrefix { get; protected set; }

		/// <summary>
		/// The private variable used to store the product course.
		/// </summary>
		private Course _product = null;

		/// <summary>
		/// Gets or sets the Product Coure into _product.
		/// </summary>
		/// <value>
		/// The product.
		/// </value>
		public Course Product
		{
			get
			{
				if (_product == null)
				{
					_product = GetCourse(ProductCourseId);
				}

				return _product;
			}
			set
			{
				_product = value;
			}
		}

		/// <summary>
		/// The private variable used to store the Course.
		/// </summary>
		private Course _course = null;

		/// <summary>
		/// Gets or sets the course into _course.
		/// </summary>
		/// <value>
		/// The course.
		/// </value>
		public Course Course
		{
			get
			{
				if (_course == null)
				{
					if (CourseIsProductCourse)
					{
						_course = Product;
					}
					else
					{
						_course = GetCourse(CourseId);
					}
				}
				return _course;
			}
			set
			{
				_course = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether [course is product course].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [course is product course]; otherwise, <c>false</c>.
		/// </value>
		public bool CourseIsProductCourse { get { return ProductCourseId == CourseId; } }

		/// <summary>
		/// Gets or sets the access level.
		/// </summary>
		/// <value>
		/// The access level.
		/// </value>
		public AccessLevel AccessLevel { get; set; }

		/// <summary>
		/// A flag which indicates whether a course needs to be browsed in a read only mode
		/// </summary>
		public bool IsCourseReadOnly { get; set; }

		/// <summary>
		/// A flag which indicates whether a course is being browsed in a shared view mode
		/// </summary>
		public bool IsSharedCourse { get; set; }

		/// <summary>
		/// <c>true</c> if the user is logged in as an instructor but in 'student view' mode.
		/// </summary>
		public bool ImpersonateStudent { get; protected set; }

		/// <summary>
		/// Gets or sets the student dashboard id.
		/// </summary>
		public string StudentDashboardId { get; set; }

		/// <summary>
		/// The key for the student view cookie.
		/// </summary>
		public string StudentViewCookieKey
		{
			get
			{
				return "StudentView";
			}
		}

		/// <summary>
		/// The key for the preview as visitor cookie.
		/// </summary>
		public string PreviewAsVisitorCookieKey
		{
			get
			{
				return "PreviewAsVisitor";
			}
		}


		/// <summary>
		/// Gets or sets the type of the access.
		/// </summary>
		/// <value>
		/// The type of the access.
		/// </value>
		public AccessType AccessType { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance can create course.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance can create course; otherwise, <c>false</c>.
		/// </value>
		public bool CanCreateCourse { get; set; }

		/// <summary>
		/// Gets or sets the site ID.
		/// </summary>
		/// <value>
		/// The site ID.
		/// </value>
		public string SiteID { get; set; }

		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		public string URL { get; set; }

		/// <summary>
		/// Gets or sets the RA base URL.
		/// </summary>
		/// <value>
		/// The RA base URL.
		/// </value>
		public string RABaseUrl { get; protected set; }

		/// <summary>
		/// The name of the product the requrest is using.
		/// </summary>
		public virtual string ProductType { get; set; }

		/// <summary>
		/// Gets or sets the bh auth cookie value.
		/// </summary>
		/// <value>
		/// The bh auth cookie value.
		/// </value>
		public string BhAuthCookieValue { get; set; }

		/// <summary>
		/// returns the Product Course ID of the URL passed in
		/// </summary>
		/// <param name="url">the url of the course ex. -standard-/dashboard/eportfolio/</param>
		/// <returns>Product Master Course Id for URL</returns>
		public abstract String GetProductCourseId(String course, String url);

		/// <summary>
		/// Initializes from request.
		/// </summary>
		protected abstract void InitializeFromRequest();

		/// <summary>
		/// Initializes the user.
		/// </summary>
		protected abstract void InitializeUser();

		/// <summary>
		/// Initializes the permissions.
		/// </summary>
		protected abstract void InitializePermissions();


		protected void InitializeFromExternalOAuthRequest()
		{

			if (( string.IsNullOrEmpty(HttpContext.Current.Request.Params["oauth_signature"]) &
			 string.IsNullOrEmpty(HttpContext.Current.Request.Params["ext_macmillan_rauid"]) )) return;

			string strValidationMessage = null;
			var traceEventType = TraceEventType.Warning;
			if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["oauth_traceEventType"]))
				TraceEventType.TryParse(ConfigurationManager.AppSettings["oauth_traceEventType"], out traceEventType);

			var oauthSettingsConfigSection = WebConfigurationManager.GetSection("oauthSettings") as NameValueCollection;
			if (oauthSettingsConfigSection == null)
			{
				strValidationMessage = " Section('oauthSettings') is not found in Web.config";
				CreateAndLogErrOAuthResponse(HttpContext.Current, ref strValidationMessage, traceEventType);
				RedirectIfOAuthRequestFailed();
				return;
			}

			if (CheckForBltiRequest(oauthSettingsConfigSection))
			{
				InitializeFromBLTIRequest(oauthSettingsConfigSection, traceEventType, ref strValidationMessage);
				if (!string.IsNullOrEmpty(strValidationMessage))
				{
					CreateAndLogErrOAuthResponse(HttpContext.Current, ref strValidationMessage, traceEventType);
					RedirectIfOAuthRequestFailed();
					return;
				}
			}

			string OAuthRaUserId;
			if (!TryGetOAuthRaUserId(ref strValidationMessage, out OAuthRaUserId))
			{
				CreateAndLogErrOAuthResponse(HttpContext.Current, ref strValidationMessage, traceEventType);
				RedirectIfOAuthRequestFailed();
				return;
			}

			HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(OAuthRaUserId), new string[] { "BltiUser" });
			var principal = HttpContext.Current.User;

			Thread.CurrentPrincipal = principal;

			if (!principal.Identity.IsAuthenticated)
			{
				strValidationMessage = strValidationMessage + String.Format(" RA User {0} is not Authenticated", OAuthRaUserId);
				CreateAndLogErrOAuthResponse(HttpContext.Current, ref strValidationMessage, traceEventType);
				ResetAllCookies();
				RedirectIfOAuthRequestFailed();
				return;
			}

			var authTicket = new FormsAuthenticationTicket(2, OAuthRaUserId, DateTime.Now, DateTime.Now.Add(FormsAuthentication.Timeout), true, "isBltiCookie", FormsAuthentication.FormsCookiePath);
			FormsAuthentication.SetAuthCookie(authTicket.Name, false, FormsAuthentication.FormsCookiePath);
			FormsAuthentication.RenewTicketIfOld(authTicket);

		}

        protected RAg.Net.RAWS.GetCourseSiteID.SiteInfo GetSiteInfo(string url)
        {
            RAg.Net.RAWS.GetCourseSiteID.SiteInfo siteInfo = null;

            using (Tracer.DoTrace("BusinessContextBase.GetSiteInfo"))
            {
                url = AdjustForSubdomain(url);
                object cachedObj = CacheProvider.FetchRASiteInfo(url);


                if (cachedObj == null)
                {
                    var svc = RAServices;

                    using (Tracer.DoTrace("GetAgilixCourseID from CoreServices"))
                    {
                        try
                        {
                            var response = svc.GetSiteListByBaseUrl(url);

                            if (response != null && response.Error != null && response.Error.Code != "-1" && !response.Sites.IsNullOrEmpty())
                            {
                                siteInfo = new RAg.Net.RAWS.GetCourseSiteID.SiteInfo();
                                siteInfo.AgilixSiteID = response.Sites.First().AgilixCourseId;
                                siteInfo.SiteID = response.Sites.First().SiteId;
                                siteInfo.BaseURL = response.Sites.First().BaseUrl;

                                CacheProvider.StoreRASiteInfo(url, siteInfo);
                            }
                            else
                            {
                                throw new Exception(string.Format("GetSiteData Failed for {0} with error: {1}", url, response.Error.Message));
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Logger.Log(string.Format("GetSiteData failed: {0}", ex.Message), LogSeverity.Error);
                            throw ex;
                        }
                    }
                }
                else
                {
                    siteInfo = (RAg.Net.RAWS.GetCourseSiteID.SiteInfo)cachedObj;
                }
            }

            return siteInfo;
        }

		private static void RedirectIfOAuthRequestFailed()
		{
			var routeData = System.Web.Routing.RouteTable.Routes.GetRouteData(new System.Web.HttpContextWrapper(System.Web.HttpContext.Current));
			var baseUrl = ConfigurationManager.AppSettings["InsecureBaseUrl"];
			//if (routeData == null) return;
			var course = routeData.Values["course"];
			var section = routeData.Values["section"];
			var courseIdObj = routeData.Values["courseid"];
			var returnUrl = string.Format("http://{0}{1}/{2}/{3}/{4}", HttpContext.Current.Request.Url.Host, baseUrl, section, course, courseIdObj);
			HttpContext.Current.Response.Redirect(string.Format("{0}?auth=failed", returnUrl));
		}

		private static void ResetAllCookies()
		{
			// clear all cookies:
			var cookies = System.Web.HttpContext.Current.Request.Cookies.AllKeys;
			foreach (var cookie in cookies)
			{
				var requestCookie = System.Web.HttpContext.Current.Request.Cookies[cookie];
				if (requestCookie != null)
				{
					requestCookie.Expires = System.DateTime.Now.AddYears(-1);
					requestCookie.Value = string.Empty;
				}

				var responseCookie = System.Web.HttpContext.Current.Response.Cookies[cookie];
				if (responseCookie == null) continue;
				responseCookie.Expires = System.DateTime.Now.AddYears(-1);
				responseCookie.Value = string.Empty;
			}
		}


		/// <summary>
		/// Allows the implementation to set any necessary.
		/// values
		/// </summary>
		public void Initialize()
		{
			using (Tracer.StartTrace("BusinessContext Initialize"))
			{
				InitializeFromExternalOAuthRequest();
				InitializeFromCookies();
				InitializeFromRequest();
				InitializeDataFromConfig();
				InitializeCourses();
				InitializeDomains();

				// If we don't have either a course or a product course ID, throw an exception.
				if (Course == null && String.IsNullOrEmpty(ProductCourseId))
				{
					var rd = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));

					if (rd.Values.ContainsKey("__px__routename") && rd.Values["__px__routename"].ToString() != "Error404")
					{
						var redirectUrl = UrlHelper.RouteUrl("Error404");
						HttpContext.Current.Response.Redirect(redirectUrl, true);
					}
					else
					{
						return;
					}
				}

				InitializeUser();
				//RedirectForValidEnrollment();
				InitializeUserWebRights();
				InitializePermissions();
				InitializeEnrollments();
				RedirectForUserRights();
				UpdateCookies();
				SetAgilixAuthCookie();
			}
		}

		private static bool CheckForOAuthRequest(NameValueCollection oauthSettingsConfigSection)
		{
			var checkForOAuthRequest = oauthSettingsConfigSection["checkForOAuthRequest"];
			return !string.IsNullOrEmpty(checkForOAuthRequest) && bool.Parse(checkForOAuthRequest);
		}


		private static bool CheckForBltiRequest(NameValueCollection oauthSettingsConfigSection)
		{
			var checkForBltiRequest = oauthSettingsConfigSection["checkForBltiRequest"];
			return !string.IsNullOrEmpty(checkForBltiRequest) && bool.Parse(checkForBltiRequest);
		}

		private static bool ValidateOAuthSettings(NameValueCollection oauthSettingsConfigSection, ref string strValidationMessage, out string key, out string middlewarekey)
		{
			key = oauthSettingsConfigSection["key"];
			middlewarekey = oauthSettingsConfigSection["middlewarekey"];

			if (key == null) strValidationMessage = "No key found in Configuration Section 'oauthkeysecretSettings' Web.config";
			if (middlewarekey == null) strValidationMessage = "No middlewarekey found in Configuration Section 'oauthkeysecretSettings' Web.config";

			return string.IsNullOrEmpty(strValidationMessage);
		}



		protected void InitializeFromBLTIRequest(NameValueCollection oauthSettingsConfigSection, TraceEventType traceEventType, ref string strValidationMessage)
		{
			if (strValidationMessage == null) strValidationMessage = "";

			var context = HttpContext.Current;

			var bltiObj = new BLTI.BLTI(context.Request);

			string strSignatureBase;
			string strSignatureGenerated;

			string strKeySecret;
			string strMiddleWareKey;

			if (!ValidateOAuthSettings(oauthSettingsConfigSection, ref strValidationMessage, out strKeySecret, out strMiddleWareKey))
			{
				CreateAndLogErrOAuthResponse(context, ref strValidationMessage, traceEventType);
				return;
			}

			if (!ValidateOAuthRequest(bltiObj, ref strValidationMessage))
			{
				CreateAndLogErrOAuthResponse(context, ref strValidationMessage, traceEventType);
				return;
			}


			if (!bltiObj.ValidateRequestUsingBltiHttpRequest(strKeySecret, out strValidationMessage, out strSignatureBase,
														out strSignatureGenerated))
			{
				strValidationMessage = strValidationMessage + String.Format(" Signature from Request '{0}' does not match to Generated Signature {1}",
														   strSignatureBase, strSignatureGenerated);
				CreateAndLogErrOAuthResponse(context, ref strValidationMessage, traceEventType);
			}

		}

		private static bool ValidateOAuthRequest(BLTI.BLTI bltiObj, ref string strValidationMessage)
		{
			if (strValidationMessage == null) strValidationMessage = "";

			var isOAuthRequestValid = true;
			if (!bltiObj.BltiLaunchParameters.ContainsKey("oauth_consumer_key"))
			{
				strValidationMessage = strValidationMessage + " No 'oauth_consumer_key' found in the request";
				isOAuthRequestValid = false;
			}
			if (!bltiObj.BltiLaunchParameters.ContainsKey("oauth_version"))
			{
				strValidationMessage = strValidationMessage + " No 'oauth_version' found in the request";
				isOAuthRequestValid = false;
			}
			if (!bltiObj.BltiLaunchParameters.ContainsKey("oauth_signature_method"))
			{
				strValidationMessage = strValidationMessage + "No 'oauth_signature_method' found in the request";
				isOAuthRequestValid = false;
			}
			if (!bltiObj.BltiLaunchParameters.ContainsKey("oauth_signature"))
			{
				strValidationMessage = strValidationMessage + " No 'oauth_signature' found in the request";
				isOAuthRequestValid = false;
			}
			if (!bltiObj.BltiLaunchParameters.ContainsKey("oauth_timestamp"))
			{
				strValidationMessage = strValidationMessage + " No 'oauth_timestamp' found in the request";
				isOAuthRequestValid = false;
			}
			if (!bltiObj.BltiLaunchParameters.ContainsKey("oauth_nonce"))
			{
				strValidationMessage = strValidationMessage + " No 'oauth_nonce' found in the request";
				isOAuthRequestValid = false;
			}

			return isOAuthRequestValid;
		}

		private static bool TryGetOAuthRaUserId(ref string strValidationMessage, out string bltiRaUserId)
		{
			if (!string.IsNullOrEmpty(HttpContext.Current.Request.Params["ext_macmillan_rauid"]))
			{
				bltiRaUserId = HttpContext.Current.Request.Params["ext_macmillan_rauid"];
				return true;
			}

			strValidationMessage = strValidationMessage + " No RA UserId found in BLTI Request";
			bltiRaUserId = null;
			return false;
		}


		protected void InitializeFromBaseOAuthRequest(NameValueCollection oauthSettingsConfigSection, TraceEventType traceEventType, ref string strValidationMessage)
		{
			if (strValidationMessage == null) strValidationMessage = "";
			HttpContext context = HttpContext.Current;

			object appOAuthKeys;
			string strReturnSignatureInfo;
			string strAuthorizationHeader;

			strValidationMessage = strValidationMessage + " " + ValidateOAuthKeys(context, oauthSettingsConfigSection, out appOAuthKeys, out strReturnSignatureInfo, out strAuthorizationHeader);

			if (!String.IsNullOrEmpty(strValidationMessage))
			{
				CreateAndLogErrOAuthResponse(context, ref strValidationMessage, traceEventType);
				return;

			}

			var consumerSecret = "";

			//Oauth authorization header will be in the following format:
			//oauth_consumer_key="key", oauth_nonce="123456789", oauth_signature="tnnArxj06cWHq44gCs1OSKk%2FjLY%3D", 
			//oauth_signature_method="HMAC-SHA1", oauth_timestamp="1318622958" , oauth_version="1.0"

			var oAuthConsumerKeyInRequest = ValidateConsumerKeys(appOAuthKeys, ref strAuthorizationHeader, ref strValidationMessage, ref consumerSecret);

			if (!String.IsNullOrEmpty(strValidationMessage))
			{
				CreateAndLogErrOAuthResponse(context, ref strValidationMessage, traceEventType);
				return;

			}

			strValidationMessage = ValidateOAuthSignature(context, strReturnSignatureInfo, consumerSecret, oAuthConsumerKeyInRequest);
			if (!String.IsNullOrEmpty(strValidationMessage))
			{
				CreateAndLogErrOAuthResponse(context, ref strValidationMessage, traceEventType);
			}

		}



		protected static string ValidateOAuthSignature(HttpContext context, string strReturnSignatureInfo, string consumerSecret, string oAuthConsumerKeyInRequest)
		{
			var objOAuthBase = new BLTI.OAuthBase();

			string strSignatureBase;
			string strSignature;
			string strOAuthBaseValidationMessage;
			var isOauthRequestValid = objOAuthBase.ValidateSignature(context.Request, oAuthConsumerKeyInRequest, consumerSecret,
															 out strOAuthBaseValidationMessage,
															 out strSignatureBase,
															 out strSignature);

			if (!isOauthRequestValid & ( !string.IsNullOrEmpty(strReturnSignatureInfo) && Convert.ToBoolean(strReturnSignatureInfo) ))
			{
				strOAuthBaseValidationMessage = strOAuthBaseValidationMessage + "------Siganture: " + strSignature + "------Base Siganture: " + strSignatureBase;
			}

			return strOAuthBaseValidationMessage;


		}


		protected static string ValidateConsumerKeys(object appOAuthKeys, ref string strAuthorizationHeader, ref string strValidationMessage, ref string consumerSecret)
		{
			const string oauthHeaderPrefix = "Oauth ";
			strAuthorizationHeader = strAuthorizationHeader.Trim().Substring(oauthHeaderPrefix.Length);
			var splitOauthParams = strAuthorizationHeader.Split(',');
			var oAuthParams = new Dictionary<string, string>();
			foreach (var splitOauthParam in splitOauthParams)
			{
				var firstIndexofEqualTo = splitOauthParam.IndexOf('=');
				var key = HttpUtility.UrlDecode(splitOauthParam.Substring(0, firstIndexofEqualTo).Trim());
				var value = HttpUtility.UrlDecode(splitOauthParam.Substring(firstIndexofEqualTo + 1).Trim().Trim('"'));
				oAuthParams.Add(key, value);
			}

			var oAuthConsumerKeyInRequest = oAuthParams["oauth_consumer_key"];
			if (string.IsNullOrEmpty(oAuthConsumerKeyInRequest)) strValidationMessage = "Authentication failed. No 'oauth_consumer_key' found in request";

			var oAuthRepo = (BLTI.OAuthKeyRepository)appOAuthKeys;
			if (oAuthRepo != null) consumerSecret = oAuthRepo.GetSecret(oAuthConsumerKeyInRequest);
			if (string.IsNullOrEmpty(consumerSecret)) strValidationMessage = "Authentication failed. No secret found for the key in request";
			return oAuthConsumerKeyInRequest;
		}

		protected static string ValidateOAuthKeys(HttpContext context, NameValueCollection oauthSettingsConfigSection, out object appOAuthKeys, out string strReturnSignatureInfo, out string strAuthorizationHeader)
		{
			string strValidationMessage = null;
			//check if the application data exists
			appOAuthKeys = context.Application["oauthkeys"];
			if (appOAuthKeys == null) strValidationMessage = "Authentication failed. No 'oauthkeys' in Web.config";

			strReturnSignatureInfo = oauthSettingsConfigSection["returnsignatureinfo"];
			if (string.IsNullOrEmpty(strReturnSignatureInfo)) strValidationMessage = "Authentication failed.  No 'returnsignatureinfo' in Web.config";

			//get the key from the request
			strAuthorizationHeader = context.Request.Headers["Authorization"];
			if (string.IsNullOrEmpty(strAuthorizationHeader)) strValidationMessage = "Authentication failed. No 'Authorization' header found";
			return strValidationMessage;
		}


		private static void CreateAndLogErrOAuthResponse(HttpContext context, ref string strValidationMessage, TraceEventType traceEventType)
		{
			var businessExc = new BusinessException(strValidationMessage, traceEventType);

			businessExc.LogBusinessException(strValidationMessage, traceEventType, "BLTI_OAuth");
		}

		private void SetAgilixAuthCookie()
		{
			if (this.CurrentUser == null)
				return;
			if (String.IsNullOrWhiteSpace(this.CurrentUser.Id))
				return;

			if (this.Course.ObtainHBAuthToken)
			{
				var request = new DlapRequest()
				{
					Type = DlapRequestType.Get,
					Parameters = new Dictionary<string, object>()
				};
				request.Parameters.Add("cmd", "getcookie");
				var response = SessionManager.StartNewSession(null, null, false, this.CurrentUser.Id).Send(request);
				BhAuthCookieValue = response.DlapAuthCookie;
			}
		}

		/// <summary>
		/// Make necessary changes to cookies based on status gathered from the initialization phase.
		/// </summary>
		private void UpdateCookies()
		{
			// If we get to a product course page, turn off student view. This just saves a world of trouble
			// that can be caused by creating a course as a student.
			if (CourseIsProductCourse)
			{
				HttpContextWrapper.Response.Cookies.Add(new HttpCookie(StudentViewCookieKey) { Expires = DateTime.Now.AddDays(-1) });
			}
		}

		/// <summary>
		/// Initialize the use web rights 
		/// </summary>
		private void InitializeUserWebRights()
		{
			if (CurrentUser != null)
			{
				//Added Web User Rights for this Course:
                CurrentUser.WebRights = new PxWebUserRights(CurrentUser.Username, ProductCourseId);
			}
		}

		/// <summary>
		/// Set various things depending on request cookie values.
		/// </summary>
		private void InitializeFromCookies()
		{
			var studentViewCookie = HttpContext.Current.Request.Cookies[StudentViewCookieKey];
            ImpersonateStudent = studentViewCookie != null && !studentViewCookie.Value.IsNullOrEmpty() && bool.Parse(studentViewCookie.Value);
		}

		/// <summary>
		/// Produces a new Item Id. The Id must be unique per domain.
		/// </summary>
		/// <returns>
		/// item id
		/// </returns>
		public string NewItemId()
		{
			return Guid.NewGuid().ToString("N");
		}

		/// <summary>
		/// Determines the correct sequence string for an item that is between min and max.
		/// </summary>
		/// <param name="min">item sequenced "above" the item</param>
		/// <param name="max">item sequenced "below" the item</param>
		/// <returns></returns>
		public string Sequence(string min, string max)
		{
			return Bfw.Common.Tumbler.GetTumbler(min, max);
		}

		/// <summary>
		/// Clear the course cache and get the new course definition.
		/// </summary>
		public void RefreshCourse()
		{
			if (Course != null)
			{
				CacheProvider.InvalidateCourse(Course.Id);
			}

			// If we set course to null, it will be retrieved the next time
			// something attempts to access it.
			_course = null;
		}

		public virtual UserInfo GetNewUserData()
		{
			return CurrentUser;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the logger.
		/// </summary>
		/// <value>
		/// The logger.
		/// </value>
		public ILogger Logger { get; protected set; }

		/// <summary>
		/// Gets or sets the tracer.
		/// </summary>
		/// <value>
		/// The tracer.
		/// </value>
		public ITraceManager Tracer { get; protected set; }

		/// <summary>
		/// Gets or sets the cache provider.
		/// </summary>
		/// <value>
		/// The cache provider.
		/// </value>
		public ICacheProvider CacheProvider { get; protected set; }

        /// <summary>
        /// RA services
        /// </summary>
        public IRAServices RAServices { get; protected set; }

		/// <summary>
		/// Gets or sets the session manager.
		/// </summary>
		/// <value>
		/// The session manager.
		/// </value>
		protected ISessionManager SessionManager { get; set; }

		/// <summary>
		/// Gets or sets the RA session.
		/// </summary>
		/// <value>
		/// The RA session.
		/// </value>
		protected BFW.RAg.Session RASession { get; set; }

		/// <summary>
		/// Gets or sets the product course id.
		/// </summary>
		/// <value>
		/// The product course id.
		/// </value>
		public string ProductCourseId { get; set; }

		/// <summary>
		/// Gets or sets the course id.
		/// </summary>
		/// <value>
		/// The course id.
		/// </value>
		public string CourseId { get; set; }

		/// <summary>
		/// Gets or sets the RA password.
		/// </summary>
		/// <value>
		/// The RA password.
		/// </value>
		protected string BrainhoneyDefaultPassword { get; set; }

		/// <summary>
		/// Gets or sets the admin id.
		/// </summary>
		/// <value>
		/// The admin id.
		/// </value>
		protected string AdminId { get; set; }

		/// <summary>
		/// Gets or sets the admin password.
		/// </summary>
		/// <value>
		/// The admin password.
		/// </value>
		protected string AdminPassword { get; set; }

		/// <summary>
		/// Gets or sets the admin domain prefix.
		/// </summary>
		/// <value>
		/// The admin domain prefix.
		/// </value>
		protected string AdminDomainPrefix { get; set; }

		/// <summary>
		/// Gets or sets the student permission flags.
		/// </summary>
		/// <value>
		/// The student permission flags.
		/// </value>
		protected string StudentPermissionFlags { get; set; }

		/// <summary>
		/// Gets or sets the brain honey auth cookie.
		/// </summary>
		/// <value>
		/// The brain honey auth cookie.
		/// </value>
		protected string BrainHoneyAuthCookie { get; set; }

		/// <summary>
		/// Gets or sets the admin dlap connection.
		/// </summary>
		/// <value>
		/// The admin dlap connection.
		/// </value>
		protected DlapConnection AdminDlapConnection { get; set; }

		/// <summary>
		/// Store the RA user locally
		/// </summary>
		private bool GetAgilixUsersFromRa
		{
			get
			{
				var result = false;
				var configValue = ConfigurationManager.AppSettings["GetAgilixUsersFromRa"];

				if (!string.IsNullOrEmpty(configValue))
				{
					result = configValue.Equals("true", StringComparison.CurrentCultureIgnoreCase);
				}

				return result;
			}
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Based on current context state, and the current request, the courses the
		/// context is dealing with are populated.
		/// </summary>
		protected void InitializeCourses()
		{
			using (Tracer.StartTrace("BusinessContext InitializeCourses"))
			{
				var rd = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));
				if (rd == null) return;

				CourseId = null;
				if (rd.Values["courseid"] != null && rd.Values["courseid"] != System.Web.Mvc.UrlParameter.Optional)
				{
					String courseId = rd.Values["courseid"].ToString();
					// only set the course number if it is a valid course id
					// should contain at least one digit
					if (NumberValidator.IsMatch(courseId))
					{
						CourseId = courseId;
						Logger.Log(String.Format("CourseId {0} found in URL", CourseId), LogSeverity.Debug);
					}
				}

				// If the above failed to set the course ID, then use the product course ID.
				if (String.IsNullOrEmpty(CourseId))
				{
					Logger.Log(string.Format("CourseId not found.  Using ProductCourseId {0} for URL {1}", ProductCourseId, HttpContext.Current.Request.RawUrl), LogSeverity.Debug);
					CourseId = ProductCourseId;
				}

				if (Course != null && Course.Properties.ContainsKey("bfw_product_type") == true)
				{
					ProductType = Course.Properties["bfw_product_type"].Value.ToString();
				}
				else
				{
					if (rd.Values["section"] != null && rd.Values["section"] != System.Web.Mvc.UrlParameter.Optional)
					{
						if (!string.IsNullOrEmpty(rd.Values["section"].ToString()))
						{
							ProductType = rd.Values["section"].ToString();
						}
					}
				}
				if (Course != null)
				{
					HttpContext.Current.Items["CourseTimeZone"] = Course.CourseTimeZone;
				}
				EntityId = CourseId;

				if (Course != null)
				{
					DashboardCourseId = Course.DashboardCourseId;
				}
			}
		}

		/// <summary>
		/// Gets an existing user.
		/// </summary>
		/// <param name="connection">The connection.</param>
		/// <param name="domainId">The domain id.</param>
		/// <param name="referenceId">The reference id.</param>
		/// <returns></returns>
		protected UserInfo ExistingUser(DlapConnection connection, string domainId, string referenceId)
		{
			UserInfo user = null;

			using (Tracer.StartTrace(String.Format("BusinessContext ExistingUser, parameters {0}, {1}", domainId, referenceId)))
			{
				user = CacheProvider.FetchUserByReference(domainId, referenceId);
				if (user != null)
				{
			        Logger.Log(
			            string.Format("User with reference id {0} and domain {1} loaded from cache", referenceId, domainId),
			            LogSeverity.Debug);
				}
				else
				{
			        GetUsers search = null;

			        search = new GetUsers
					{
						SearchParameters = new Bfw.Agilix.DataContracts.UserSearch()
						{
							ExternalId = referenceId,
							DomainId = domainId
						}
					};

					var request = search.ToRequest();
					var response = connection.Send(request);

					search.ParseResponse(response);

					if (!search.Users.IsNullOrEmpty())
					{
						var agxUser = search.Users.First();
						user = agxUser.ToUserInfo();
						CacheProvider.StoreUser(user);
					}
				    else
				    {//if we don't find a user, store a blank user so that we don't keep hitting DLAP looking for one
				        user = new UserInfo()
				        {
				            Username = referenceId,
                            ReferenceId = referenceId,
				            DomainId = domainId
				        };
                        CacheProvider.StoreUser(user);
				}
			}
			}

			return user;
		}

		/// <summary>
		/// Defines the Admin type.
		/// </summary>
		public enum AdminAccessType
		{
			RootAdmin,
			UserAdmin
		}

		/// <summary>
		/// Sets up a DlapConnection for admin access.
		/// </summary>
		/// <param name="accessType">Type of the access.</param>
		/// <returns></returns>
		protected DlapConnection AdminConnection(AdminAccessType accessType)
		{
			DlapConnection connection = null;

			if (AdminDlapConnection == null)
			{
				string userspace = null;
				string userId = null;
				string password = null;

				using (Tracer.StartTrace("BusinessContext AdminConnection"))
				{
					switch (accessType)
					{
						case AdminAccessType.RootAdmin:
							userspace = ConfigurationManager.AppSettings.Get("AdministratorUserspace");
							userId = ConfigurationManager.AppSettings.Get("AdministratorUserId");
							password = ConfigurationManager.AppSettings.Get("AdministratorPwd");
							break;
						case AdminAccessType.UserAdmin:
							userspace = ConfigurationManager.AppSettings.Get("UserAdministratorUserspace");
							userId = ConfigurationManager.AppSettings.Get("UserAdministratorUserId");
							password = ConfigurationManager.AppSettings.Get("UserAdministratorPwd");
							break;
						default:
							throw new ArgumentException("Unknown AdminAccessType", "AdminAccesType");
					}

					var config = ConfigurationManager.GetSection("agilixSessionManager") as Bfw.Agilix.Dlap.Configuration.SessionManagerSection;

					connection = ConnectionFactory.GetDlapConnection(config.Connection.Url);
					connection.Tracer = Tracer;
					connection.Logger = Logger;
					connection.TrustHeaderKey = config.Connection.SecretKey;
					connection.TrustHeaderUsername = userId;

				}
				AdminDlapConnection = connection;
			}
			else
			{
				Logger.Log("Already have AdminConnection", LogSeverity.Debug);
				connection = AdminDlapConnection;
			}

			return connection;
		}

		/// <summary>
		/// Finds list of enrollments for the user and course.
		/// </summary>
		/// <param name="userId">ID of the user.</param>
		/// <param name="courseId">ID of the course.</param>
		/// <returns>List of enrollments the user has for this course.</returns>
		public Enrollment FindEnrollment(string userId, string courseId)
		{
			var adminDomainPrefix = AdminDomainPrefix;
			Enrollment enrollment = null;

			using (Tracer.StartTrace("BusinessContext FindEnrollments"))
			{
				if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(courseId))
				{
					enrollment = CacheProvider.FetchEnrollmentByCourse(userId, courseId);
                    if (enrollment != null && IsValidEnrollment(enrollment, AccessLevel))
					{
						Logger.Log(string.Format("Enrollment loaded from cache, user {0} - course {1}", userId, courseId), LogSeverity.Debug);
					}
					else
					{
						var criteria = new Agilix.DataContracts.EntitySearch() { CourseId = courseId, UserId = userId, AllStatus = true };
						var cmd = new GetEntityEnrollmentList() { SearchParameters = criteria };

						var request = cmd.ToRequest();
						var response = AdminConnection(AdminAccessType.RootAdmin).Send(request);
						cmd.ParseResponse(response);

                        enrollment = GetCorrectEnrollment(cmd);
						CacheProvider.StoreEnrollment(enrollment);
					}
				}
			}

			return enrollment;
		}

        /// <summary>
        /// Check if the enrollment is valid by checking its flags and user access level.
        /// ie: student should have enrollment with flag 'Participate'.
        /// </summary>
        /// <param name="enrollment"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private bool IsValidEnrollment(Enrollment enrollment, AccessLevel level)
        {
            if (null == enrollment || enrollment.Flags.IsNullOrEmpty())
                return false;
            //If this is student, the enrollment should have student flags. Otherwise, return false;
            else if (level != Biz.ServiceContracts.AccessLevel.Instructor)
                return enrollment.Flags.Contains(DlapRights.Participate.ToString());
            else
                return !enrollment.Flags.Contains(DlapRights.Participate.ToString());
        }
		/// <summary>
		/// Figure out which mode (student/instructor) we're in, and return the correllative enrollment.
		/// </summary>
		/// <param name="executedCommand"></param>
		/// <returns></returns>
		private Enrollment GetCorrectEnrollment(GetEntityEnrollmentList executedCommand)
		{
			Enrollment enrollment = null;

			if (!executedCommand.Enrollments.IsNullOrEmpty())
			{
				if (executedCommand.Enrollments.Count > 1)
				{
					// If there's more than one enrollment, figure out which one is the 
					// instructor enrollment and which one is the student enrollment.
					Agilix.DataContracts.Enrollment instructorEnrollment = executedCommand.Enrollments.First();
					Agilix.DataContracts.Enrollment studentEnrollment = executedCommand.Enrollments.First();
					foreach (var e in executedCommand.Enrollments)
					{
						if (( e.Flags & DlapRights.Participate ) == DlapRights.Participate)
						{
							studentEnrollment = e;
						}
						else
						{
							instructorEnrollment = e;
						}
					}
					if (studentEnrollment == null)
					{
						throw new Exception(String.Format("Could not find a student enrollment for user {0} course {1}.", executedCommand.SearchParameters.UserId, executedCommand.SearchParameters.CourseId));
					}
					if (instructorEnrollment == null)
					{
						throw new Exception(String.Format("Could not find an instructor enrollment for user {0} course {1}.", executedCommand.SearchParameters.UserId, executedCommand.SearchParameters.CourseId));
					}

					// If we're in student view mode, and we've the student enrollment, and it is not valid,
					// validate it. Likewise, if we're an instructor, and NOT in student view, make sure the
					// student enrollment is invalid.
					if (( AccessLevel == Biz.ServiceContracts.AccessLevel.Student ) && ImpersonateStudent)
					{
						if (studentEnrollment.Status == "10")
						{
							studentEnrollment.Status = "1";
							var cmd = new UpdateEnrollments() { Enrollments = new List<Agilix.DataContracts.Enrollment>() { studentEnrollment } };
							cmd.ParseResponse(AdminDlapConnection.Send(cmd.ToRequest()));
						}
					}
					if (AccessLevel == Biz.ServiceContracts.AccessLevel.Instructor)
					{
						if (studentEnrollment.Status == "1")
						{
							studentEnrollment.Status = "10";
							var cmd = new UpdateEnrollments() { Enrollments = new List<Agilix.DataContracts.Enrollment>() { studentEnrollment } };
							cmd.ParseResponse(AdminDlapConnection.Send(cmd.ToRequest()));
						}
					}

					enrollment = ImpersonateStudent ? studentEnrollment.ToEnrollment() : instructorEnrollment.ToEnrollment();
				}
				else
				{
					// If there's just one enrollment for this user, use that.
					enrollment = executedCommand.Enrollments.First().ToEnrollment();
				}

				CacheProvider.StoreEnrollment(enrollment);
			}

			return enrollment;
		}

		/// <summary>
		/// Returns a list of all the courses that the current RA user is logged into, across domains.
		/// </summary>
		/// <param name="isSingleDomain"></param>
		/// <returns></returns>
		public IEnumerable<Course> FindAllEnrolledCoursesForRAUser(bool isSingleDomain)
		{

			string d = Domain.Id;
			var courses = new List<Course>();
			using (Tracer.StartTrace("FindAllEnrolledCoursesForRAUser"))
			{
				var batch = new Batch();
				if (isSingleDomain)
				{

					batch.Add(new GetUserEnrollmentList()
					{
						SearchParameters = new Bfw.Agilix.DataContracts.EntitySearch()
						{
							UserId = SSOData.UserId
						}
					});

					batch.Add(new GetCourse()
					{
						SearchParameters = new Agilix.DataContracts.CourseSearch()
						{
							DomainId = Domain.Id
						}
					});

				}
				else
				{
					foreach (var agilixUser in SSOData.User.AgilixUsers)
					{
						batch.Add(new GetUserEnrollmentList()
									   {
										   SearchParameters = new Bfw.Agilix.DataContracts.EntitySearch()
																  {
																	  UserId = agilixUser.ID
																  }
									   });

						batch.Add(new GetCourse()
									  {
										  SearchParameters = new Agilix.DataContracts.CourseSearch()
																 {
																	 DomainId = agilixUser.DomainID
																 }
									  });
					}
				}

				Execute(AdminConnection(AdminAccessType.RootAdmin), batch);

				var enrollmentCourseIds = new Dictionary<string, string>();
				for (int ord = 0; ord < batch.Commands.Count() - 1; ord += 2)
				{
					batch.CommandAs<GetUserEnrollmentList>(ord).Enrollments.Map(ue => ue.Course.Id).Reduce((c, dict) => { dict[c] = c; return dict; }, enrollmentCourseIds);
					courses.AddRange(batch.CommandAs<GetCourse>(ord + 1).Courses.Filter(c => enrollmentCourseIds.ContainsKey(c.Id)).Map(c => c.ToCourse()));
				}
			}

			return courses.Distinct((a, b) => a.Id == b.Id);
		}

		/// <summary>
		/// Returns list of all courses that the user is enrolled in.
		/// </summary>
		/// <param name="userId">ID of the user to search for.</param>
		/// <param name="domainId">Domain in which to search.</param>
		/// <returns></returns>
		public IEnumerable<Course> FindCoursesByUserEnrollment(string userId, string domainId)
		{
			return FindCoursesByUserEnrollment(userId, domainId, null);
		}

		public IEnumerable<Course> FindCoursesByUserEnrollmentBatch(List<UserInfo> userInfoList, string productCourseId, bool titleAndIdOnly = false)
		{
			List<Course> cacheCoursesByUserEnrollment = new List<Course>();
            cacheCoursesByUserEnrollment = CacheProvider.FetchCourseList(CurrentUser.Username); //***LMS - ref id
			var domainCourses = new List<Course>();
			if (cacheCoursesByUserEnrollment.IsNullOrEmpty())
			{
				var fullcourses = new List<Course>();
				var fullEnrollments = new List<Enrollment>();

				Batch batch = new Batch();

				if (userInfoList.Count > 1)
				{
					batch.RunAsync = true;
				}

				if (!userInfoList.IsNullOrEmpty())
				{
					foreach (UserInfo userInfo in userInfoList)
					{
						batch.Add(new GetUserEnrollmentList()
						{
							SearchParameters = new Bfw.Agilix.DataContracts.EntitySearch()
							{
								UserId = userInfo.Id,
								Query = "/meta-product-course-id=" + productCourseId + " AND /creationdate>=" + DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd")
							}
						});
					}

					SessionManager.CurrentSession.ExecuteAsAdmin(batch);

					var userEnrollments = new List<Adc.Enrollment>();
					for (int ord = 0; ord < batch.Commands.Count(); ord++)
					{
						userEnrollments.AddRange(batch.CommandAs<GetUserEnrollmentList>(ord).Enrollments);
					}

					if (!userEnrollments.IsNullOrEmpty())
					{
						var courses = userEnrollments.Map(ue => ue.ToEnrollment().Course);

						if (!courses.IsNullOrEmpty())
						{
							foreach (Course course in courses)
							{
								if (!domainCourses.Exists(i => i.Id == course.Id))
								{
									domainCourses.Add(course);
								}
							}

							if (titleAndIdOnly)
							{
								return domainCourses;
							}
						}
						var courseBatch = new Batch();
						courseBatch.RunAsync = true;

						//build up your batch call
						foreach (Course course in domainCourses)
						{
							string courseId = course.Id;
							GetCourse cmdGetCourse = new GetCourse()
							{
								SearchParameters = new Bfw.Agilix.DataContracts.CourseSearch()
								{
									CourseId = courseId
								}
							};

							courseBatch.Add(courseId, cmdGetCourse);
						}

						if (!courseBatch.Commands.IsNullOrEmpty())
						{
							SessionManager.CurrentSession.ExecuteAsAdmin(courseBatch);
							var fullCourses = new List<Biz.DataContracts.Course>();
							foreach (Course course in domainCourses)
							{
								string courseId = course.Id;
								var cmdCourse = courseBatch.CommandAs<GetCourse>(courseId);
								if (!cmdCourse.Courses.IsNullOrEmpty())
								{
									var c = cmdCourse.Courses.First().ToCourse();
									if (c.ProductCourseId == productCourseId)
									{
										fullCourses.Add(c);
									}
								}
							}

							domainCourses = fullCourses;
						}

					}
				}

                CacheProvider.StoreCourseList(domainCourses, CurrentUser.Username);
			}
			else
			{
				domainCourses = cacheCoursesByUserEnrollment;
			}
			return domainCourses;
		}

		/// <summary>
		/// Returns list of all courses that the user is enrolled in.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="domainId"></param>
		/// <param name="parentId">If this value is not null, then limit courses to only those with this parent ID.</param>
		/// <returns>
		/// Course
		/// </returns>
		public IEnumerable<Course> FindCoursesByUserEnrollment(string userId, string domainId, string parentId)
		{
			using (Tracer.StartTrace("BusinessContext FindDerivativeEnrollments"))
			{
				// Gets all courses.
				var courseCmd = new GetCourse() { SearchParameters = new Bfw.Agilix.DataContracts.CourseSearch() { DomainId = domainId } };
				courseCmd.ParseResponse(AdminConnection(AdminAccessType.RootAdmin).Send(courseCmd.ToRequest()));

				var courses = courseCmd.Courses;

				// If we were asked to, restrict courses to only those with a given parent ID.
				if (!String.IsNullOrEmpty(parentId))
				{
					courses = courses.Filter(c => c.ParentId == parentId);
				}

				// Now find user enrollments.
				var enrollmentsCmd = new GetUserEnrollmentList()
				{
					SearchParameters = new Bfw.Agilix.DataContracts.EntitySearch()
					{
						UserId = userId
					}
				};

				SessionManager.CurrentSession.ExecuteAsAdmin(enrollmentsCmd);

				var userEnrollments = enrollmentsCmd.Enrollments;
				var enrollmentCourseIds = userEnrollments.Map(ue => ue.Course.Id);

				courses = courses.Filter(c => enrollmentCourseIds.Contains(c.Id));
				return courses.Map(c => c.ToCourse());
			}
		}

		/// <summary>
		/// Gets a course by ID.
		/// </summary>
		/// <param name="id">The ID.</param>
		/// <returns>Course</returns>
		public Course GetCourse(string id)
		{
			Course c = null;

			using (Tracer.StartTrace("BusinessContext GetCourse"))
			{
				if (!string.IsNullOrEmpty(id))
				{
					c = CacheProvider.FetchCourse(id);
					if (c != null)
					{
						Logger.Log(string.Format("Course {0} loaded from cache", id), LogSeverity.Debug);
					}
					else
					{
						var connection = AdminConnection(AdminAccessType.RootAdmin);
						var courses = new GetCourse()
						{
							SearchParameters = new Bfw.Agilix.DataContracts.CourseSearch()
							{
								CourseId = id
							}
						};

						var request = courses.ToRequest();
						var response = connection.Send(request);

						courses.ParseResponse(response);

						if (!courses.Courses.IsNullOrEmpty())
						{
							c = courses.Courses.First().ToCourse();
							CacheProvider.StoreCourse(c);
						}
					}
				}
			}

			return c;
		}

		/// <summary>
		/// The private variable that holds HttpContextWrapper.
		/// </summary>
		private HttpContextWrapper _httpContextWrapper;

		/// <summary>
		/// Gets the HTTP context wrapper into _httpContextWapper.
		/// </summary>
		private HttpContextWrapper HttpContextWrapper
		{
			get
			{
				return _httpContextWrapper != null ?
					_httpContextWrapper :
					_httpContextWrapper = new HttpContextWrapper(System.Web.HttpContext.Current);
			}
		}

		/// <summary>
		/// A private variable that holds the UrlHelper.
		/// </summary>
		private UrlHelper _urlHelper;

		/// <summary>
		/// Gets the URL helper from _urlHelper.
		/// </summary>
		private UrlHelper UrlHelper
		{
			get
			{
				return _urlHelper != null ?
					_urlHelper :
					_urlHelper = new UrlHelper(new RequestContext(HttpContextWrapper, RouteTable.Routes.GetRouteData(HttpContextWrapper)));
			}
		}

		/// <summary>
		/// If this is not a product course check whether the current user has an enrollment.
		/// </summary>
		protected void RedirectForValidEnrollment()
		{
			using (Tracer.StartTrace("RedirectForValidEnrollment"))
			{
				var redirectUrl = UrlHelper.RouteUrl("CourseSectionDefault", new { courseid = ProductCourseId });
				if (!CourseIsProductCourse && IsRedirectPage(UrlHelper.RequestContext.RouteData.Values) && !IsPublicView)
				{
					if (SSOData != null && !string.IsNullOrEmpty(SSOData.UserId))
					{
						UserInfo cacheUser = null;
						Enrollment cacheEnrollment = null;
						var batch = new Batch();

						cacheUser = CacheProvider.FetchUserByReference(Domain.Id, SSOData.UserId);
						if (cacheUser != null)
						{
							Logger.Log(string.Format("User with reference id {0} and domain {1} loaded from cache", SSOData.UserId, Domain.Id), LogSeverity.Debug);
							cacheEnrollment = CacheProvider.FetchEnrollmentByCourse(cacheUser.Id, CourseId);
						}

						var userCmd = new GetUsers()
						{
							SearchParameters = new Bfw.Agilix.DataContracts.UserSearch()
							{
								ExternalId = SSOData.UserId, // change for LMS? 
								DomainId = Domain.Id
							}
						};

						var enrollmentsCmd = new GetEntityEnrollmentList()
						{
							SearchParameters = new Bfw.Agilix.DataContracts.EntitySearch()
							{
								UserId = string.Format("{0}/{1}", Domain.Id, SSOData.UserId),
								CourseId = CourseId,
								AllStatus = true
							}
						};

						if (cacheUser == null)
						{
							batch.Add(userCmd);
						}

						if (cacheEnrollment == null)
						{
							batch.Add(enrollmentsCmd);
						}

						if (batch.Commands.ToList().Count > 0)
						{
							try
							{
								var connection = AdminConnection(AdminAccessType.RootAdmin);
								var request = batch.ToRequest();
								var response = connection.Send(request);
								batch.ParseResponse(response);

								if (!userCmd.Users.IsNullOrEmpty())
								{
									cacheUser = userCmd.Users.First().ToUserInfo();
									CacheProvider.StoreUser(cacheUser);
								}

								if (!enrollmentsCmd.Enrollments.IsNullOrEmpty())
								{
									cacheEnrollment = GetCorrectEnrollment(enrollmentsCmd);
								}
							}
							catch (Exception ex)
							{
								throw new Exception(string.Format("Could not get enrollments for {0}/{1}", Domain.Id, SSOData.UserId));
							}
						}

						if (cacheUser == null)
						{
							HttpContext.Current.Response.Redirect(redirectUrl, true);
						}
						else
						{
							CurrentUser = cacheUser;
							if (cacheEnrollment != null)
								EnrollmentId = cacheEnrollment.Id;
						}
					}
				}
			}
		}

		/// <summary>
		/// Determines whether [is redirect page] [the specified route vals].
		/// </summary>
		/// <param name="routeVals">The route vals.</param>
		/// <returns>
		///   <c>true</c> if [is redirect page] [the specified route vals]; otherwise, <c>false</c>.
		/// </returns>
		protected bool IsRedirectPage(RouteValueDictionary routeVals)
		{
			// We only want to redirect when the requested route is the home route for the main course, and the course id has not been specified.
			return
				routeVals != null &&
				routeVals["controller"] != null &&
				routeVals["controller"].Equals("Home") &&
				!routeVals["action"].Equals("SelectCourse") &&
				!routeVals["action"].Equals("Ping");
		}

		/// <summary>
		/// Redirects for user rights.
		/// </summary>
		protected void RedirectForUserRights()
		{
			var routeVals = UrlHelper.RequestContext.RouteData.Values;
			if (CurrentUser != null && IsRedirectPage(routeVals) && ( routeVals["courseid"] == null || routeVals["courseid"].Equals("") ))
			{
				var response = HttpContext.Current.Response;
				var derivativeEnrolledCourses =
					!String.IsNullOrEmpty(CurrentUser.Id) ?
					FindCoursesByUserEnrollment(CurrentUser.Id, ProductCourseId) :
					new List<Course>();

				// These redirect requirements doumented in 2011-05-04 email from ccrume.
				// For adopters:
				// 0 derivative enrollments = product course with create option.
				// 1 derivative enrollment = redirect user to it.
				// 1+ derivative enrollments = user chooses.
				if (AccessType == AccessType.Adopter)
				{
					if (derivativeEnrolledCourses.Count() == 1)
					{
						response.Redirect(UrlHelper.RouteUrl("CourseSectionDefault", new { courseid = derivativeEnrolledCourses.First().Id }), true);
					}
					else if (derivativeEnrolledCourses.Count() > 1)
					{
						response.Redirect(UrlHelper.RouteUrl("SelectCourse", new { courseid = ProductCourseId }), true);
					}
				}

				// Premium Students (as long as we're not in student view):
				// 0 derivative enrollments = TBD, maybe just page to contact TechSupport. Stub it for now.
				// 1 derivative enrollment = redirect user to it.
				// 1+ derivative enrollments = user chooses.
				if (AccessLevel == AccessLevel.Student && !ImpersonateStudent && this.Course != null && this.Course.CourseType != CourseType.PersonalEportfolioProductMaster.ToString() && this.Course.CourseType != CourseType.Eportfolio.ToString())
				{
					if (derivativeEnrolledCourses.Count() == 0 || derivativeEnrolledCourses.Count() > 1)
					{
						response.Redirect(UrlHelper.RouteUrl("SelectCourse", new { courseid = ProductCourseId }), true);
					}
					else if (derivativeEnrolledCourses.Count() == 1)
					{
						response.Redirect(UrlHelper.RouteUrl("CourseSectionDefault", new { courseid = derivativeEnrolledCourses.First().Id }), true);
					}
				}
			}
		}

		/// <summary>
		/// Gets the HTS URL.
		/// </summary>
		/// <returns>String</returns>
		protected string GetHtsUrl()
		{
			var htsUrl = string.Empty;
			var request = new DlapRequest()
			{
				Type = DlapRequestType.Get,
				Parameters = new Dictionary<string, object>() {
                    { "cmd", "getdomain" },
                    { "domainid", Domain.Id }
                }
			};

			var connection = AdminConnection(AdminAccessType.RootAdmin);
			var response = connection.Send(request);
			var htsElm = response.ResponseXml.XPathSelectElement("//customquestion[@name='HTS']");

			if (htsElm != null)
			{
				htsUrl = htsElm.Attribute("location").Value;
			}

			return htsUrl;
		}

		/// <summary>
		/// What kind of questions are allowed.  Map from question type name to description.
		/// </summary>
		/// <returns>A dictionary of question type name to question type description.</returns>
		public Dictionary<string, string> GetQuestionTypes()
		{
			try
			{
				var questionTypes = new Dictionary<string, string>();
				var settingString = ConfigurationManager.AppSettings["QuestionTypes"];
				foreach (var type in settingString.Split('|'))
				{
					var parts = type.Split(':');
					questionTypes.Add(parts[0], parts[1]);
				}
				return questionTypes;
			}
			catch (Exception ex)
			{
				throw new Exception("Error trying to find or parse the QuestionTypes config parameter.", ex);
			}
		}


		/// <summary>
		/// What kind of documents are allowed for download only.
		/// </summary>
		/// <returns>A list of string of documents type.</returns>
		public List<string> GetDownloadOnlyDocuments()
		{
			try
			{
				var downloadType = new List<string>();
				var settingString = Course.DownloadOnlyDocumentTypes;
				foreach (var type in settingString.Split('|'))
				{
					downloadType.Add(type);
				}
				return downloadType;
			}
			catch (Exception ex)
			{
				throw new Exception("Error trying to find or parse the QuestionTypes config parameter.", ex);
			}
		}


		/// <summary>
		/// Initializes the domains.
		/// </summary>
		public virtual void InitializeDomains()
		{
			using (Tracer.DoTrace("BusinessContext InitializeDomains"))
			{
				// If we don't have either a course or a product course ID, throw an exception.
				if (Course == null && String.IsNullOrEmpty(ProductCourseId))
				{
					//throw new Exception("Can not load derivative or product course");
					return;
				}

				// If the course is null, use the product course to set the domain.
				var course = Course;

				if (course == null)
				{
					Course prodCourse = null;
					prodCourse = CacheProvider.FetchCourse(ProductCourseId);
					if (prodCourse != null)
					{
						course = prodCourse;
						Logger.Log(string.Format("Product Course {0} loaded from cache", ProductCourseId), LogSeverity.Debug);
					}
					else
					{
						var courseCmd = new GetCourse() { SearchParameters = new Bfw.Agilix.DataContracts.CourseSearch() { CourseId = ProductCourseId } };
						courseCmd.ParseResponse(AdminConnection(AdminAccessType.RootAdmin).Send(courseCmd.ToRequest()));
						course = courseCmd.Courses.First().ToCourse();
						CacheProvider.StoreCourse(course);
					}
				}

				Domain = GetDomain(course.Domain.Id);
			}
		}

		/// <summary>
		/// Reads any necessary data from the web.config file.
		/// </summary>
		protected void InitializeDataFromConfig()
		{
			using (Tracer.StartTrace("BusinessContext InitializeDataFromConfig"))
			{
				BrainhoneyDefaultPassword = ConfigurationManager.AppSettings["BrainhoneyDefaultPassword"];
				AdminId = ConfigurationManager.AppSettings.Get("UserAdministratorId");
				AdminPassword = ConfigurationManager.AppSettings.Get("UserAdministratorPwd");
				AdminDomainPrefix = ConfigurationManager.AppSettings.Get("UserAdministratorUserspace");
				EnvironmentUrl = ConfigurationManager.AppSettings["EnvironmentUrl"];
				DevUrl = ConfigurationManager.AppSettings["DevUrl"];
				HTSEditorUrl = ConfigurationManager.AppSettings["PxHTSEditorUrl"];
				ProxyUrl = ConfigurationManager.AppSettings["ProxyUrl"];
				BrainHoneyUrl = ConfigurationManager.AppSettings["BrainHoneyUrl"];
				AppDomainUrl = ConfigurationManager.AppSettings["AppDomainUrl"];
				ExternalResourceBaseUrl = ConfigurationManager.AppSettings["ExternalResourceBaseUrl"];
				BrainHoneyAuthCookie = ConfigurationManager.AppSettings["BrainHoneyAuthCookie"];
				StudentPermissionFlags = ConfigurationManager.AppSettings["StudentPermissionFlags"];
			}
		}

		/// <summary>
		/// Sets up the context with the correct enrollments for the user, if possible.
		/// </summary>
		protected void InitializeEnrollments()
		{
			using (Tracer.StartTrace("BusinessContext InitializeEnrollments"))
			{
				if (CurrentUser != null && Course != null && String.IsNullOrEmpty(EnrollmentId))
				{

					if (Course.ReadOnly == CourseReadOnly.Yes.ToString() ||
						( Course.ReadOnly == CourseReadOnly.IfNotOwner.ToString() && Course.CourseOwner != CurrentUser.Id ))
					{
						IsCourseReadOnly = true;
					}
					else
					{
						IsCourseReadOnly = false;
					}

					if (AccessLevel == Biz.ServiceContracts.AccessLevel.Instructor && !Course.CourseOwner.IsNullOrEmpty() && !Course.CourseOwner.Equals(CurrentUser.Id) && !Course.CourseType.Equals("EportfolioTemplate") && Course.CourseType.ToLower() == "eportfolio")
					{
						IsSharedCourse = true;
						IsCourseReadOnly = true;
					}

					Enrollment enrollment = null;
					if (( IsSharedCourse || IsCourseReadOnly ) && !string.IsNullOrEmpty(Course.CourseOwner))
					{
						enrollment = FindEnrollment(Course.CourseOwner, CourseId);
					}
					else
					{
						enrollment = FindEnrollment(CurrentUser.Id, CourseId);
					}

					if (enrollment != null)
					{
						EnrollmentId = enrollment.Id;
                        EnrollmentStatus = enrollment.Status;
					}

				}
			}
		}

		/// <summary>
		/// Given a domain id, set the current user object to be the user with the current ref ID, but
		/// in the given domain.
		/// </summary>
		/// <param name="domainId">The id of the domain the user is in.</param>
		public virtual void UpdateCurrentUser(string domainId)
		{
			UserInfo user = null;
			Logger.Log(String.Format("Updating current user to user with reference {0} in domain {1}.", SSOData.UserId, domainId), LogSeverity.Debug);

            user = CacheProvider.FetchUserByReference(domainId, SSOData.UserId);//change for LMS
			if (user == null)
			{
				var userCmd = new GetUsers() { SearchParameters = new Bfw.Agilix.DataContracts.UserSearch() { DomainId = domainId, ExternalId = SSOData.UserId } }; //change for LMS

				SessionManager.CurrentSession.ExecuteAsAdmin(userCmd);
				CurrentUser = userCmd.Users.FirstOrDefault().ToUserInfo();
				CacheProvider.StoreUser(CurrentUser);
			}
			else
			{
				CurrentUser = user;
				Logger.Log(string.Format("User with reference id {0} and domain {1} loaded from cache", SSOData.UserId, Domain.Id), LogSeverity.Debug);
			}
		}

		/// <summary>
		/// Returns a list of ids for all domains this RA user belongs to.
		/// </summary>
		/// <returns>List of Domain</returns>
		public IEnumerable<Domain> GetRaUserDomains()
		{
		    if (_raUserDomains == null)
		    {
		        if (GetAgilixUsersFromRa)
		        {
		            return SSOData.User.AgilixUsers.Map(u => GetDomain(u.DomainID));
		        }
		        else
		        {
		            string referenceId = SSOData.UserId;
		            IEnumerable<Adc.AgilixUser> users = CacheProvider.FetchUsersByReference(referenceId);
		            if (users == null)
		            {
		                var cmd = new GetUsers()
		                {
		                    SearchParameters = new Bfw.Agilix.DataContracts.UserSearch() {ExternalId = referenceId}
		                };
		                    //***change for LMS

		                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
		                CacheProvider.StoreUsersByReference(referenceId, cmd.Users);
		                users = cmd.Users;
		            }

		            _raUserDomains = users.Map(u => GetDomain(u.Domain.Id)).Distinct((a, b) => a.Id == b.Id).ToList();
		        }
		    }
		    return _raUserDomains;
		}

		/// <summary>
		/// Gets the domain.
		/// </summary>
		/// <param name="domainId">The domain ID.</param>
		/// <returns>Domain</returns>
		public Domain GetDomain(string domainId)
		{
			Domain domain = CacheProvider.FetchDomain(domainId);

            if (domain == null)
			{
				var cmd = new GetDomain()
				{
					DomainId = domainId
				};

				cmd.ParseResponse(AdminConnection(AdminAccessType.RootAdmin).Send(cmd.ToRequest()));
				domain = cmd.Domain.ToDomain();
			    CacheProvider.StoreDomain(domainId, domain);
			}
			
			return domain;
		}

		private void Execute(DlapConnection connection, DlapCommand cmd)
		{
			var request = cmd.ToRequest();
			var response = connection.Send(request);
			cmd.ParseResponse(response);
		}
		/// <summary>
		/// If there is an entry in the cache for this domain/referenceid string, return the
		/// resulting userid. Otherwise, return empty string.
		/// </summary>
		/// <param name="domainUserReferenceId">The domain/referenceid expected in the cache.</param>
		/// <returns></returns>
		protected string CacheUserId(string domainUserReferenceId)
		{
			string id = string.Empty;
            var cached = CacheProvider.FetchUserIdByReferenceId(domainUserReferenceId);

			if (cached != null)
			{
			    id = cached;
				Logger.Log(string.Format("BusinessContext has cached userid for {0}", domainUserReferenceId), LogSeverity.Debug);
			}

		    return id;
		}

		protected void CacheUserId(string domainUserReferenceId, string userId)
		{
            CacheProvider.StoreUserIdByReferenceId(domainUserReferenceId, userId);
		}

		/// <summary>
		/// this method will return urls like  http://bfwusers.bhqa.whfreeman.com
		/// format: [protocol of current request]://[courseDomain].[SamlDoamin].[last 2 parts of current request domain]
		/// </summary>
		/// <returns></returns>
		public string GetSamlAuthenticationBHComponentUrl()
		{
			string host = HttpContext.Current.Request.Url.Host;
			//host = "qa.whfreeman.com";

			if (host.Contains("localhost"))
			{
				return string.Empty;
			}

			string url = string.Empty;
			string domainUserSpace = this.Domain.Userspace;
			string samlSubdomain = ConfigurationManager.AppSettings["SAMLsubDomain"];
			string localDomain = host;

			if (host.Contains("."))
			{
				var splitParts = host.Split('.');
				int length = splitParts.Length;
				localDomain = string.Format("{0}.{1}", splitParts[length - 2], splitParts[length - 1]);
			}

			url = string.Format("{0}://{1}.{2}.{3}", HttpContext.Current.Request.Url.Scheme, domainUserSpace, samlSubdomain, localDomain);

			return url.ToLower();
		}

		/// <summary>
		/// Sets the IsPublicView property of the BusinessContext given then userid
		/// </summary>
		protected void SetPublicView(string currentUserId)
		{
			//set the IsPublicView property for presentation courses
			if (Course != null && Course.CourseType == CourseType.PersonalEportfolioPresentation.ToString()
					   && currentUserId != Course.CourseOwner && Course.Properties.ContainsKey("bfw_shared"))
			{
				var bfw_shared = Course.Properties["bfw_shared"].Value.ToString().ToLowerInvariant();
				IsPublicView = bfw_shared == "public";
			}
		}

		protected static string AdjustForLocalhostUrl(String url)
		{
			if (url.ToLowerInvariant().Contains("localhost"))
			{
				string lookupkey = "RaLocalUrlReplacement";

				string replacement = ConfigurationManager.AppSettings[lookupkey];
				return Regex.Replace(url, @"localhost:?\d*", replacement);
			}
			return url;
		}

		/// <summary>
		/// Adds the missing www subdomain to the URL, or replaces the "local" subdomain with the "dev" 
		/// subdomain
		/// </summary>        
		protected static string AdjustForSubdomain(String url)
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

				if (subdomain.ToLowerInvariant() == System.Configuration.ConfigurationManager.AppSettings["LocalSubdomain"])
				{
					host = host.Replace(subdomain, System.Configuration.ConfigurationManager.AppSettings["DevSubdomain"]);
				}

				if (uri.Host != host)
					url = url.Replace(uri.Host, host);
			}
			return url;
		}

		#endregion
	}
}