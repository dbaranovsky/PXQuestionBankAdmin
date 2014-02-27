using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Bfw.Common;
using Bfw.PXWebAPI.Models.Response;
using Newtonsoft.Json;

namespace PXWebAPI.App_Start
{
	/// <summary>
	/// OAuthValidationAttribute
	/// </summary>
	/// 
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class OAuthValidationAttribute : AuthorizeAttribute
	{

		private IPrincipal User { get; set; }

		public override void OnAuthorization(HttpActionContext actionContext)
		{
			User = HttpContext.Current.User;
			base.OnAuthorization(actionContext);
		}

		private void CreatePxWebApiValidConsumer()
		{
			User = new GenericPrincipal(new GenericIdentity("PxWebApiValidConsumer"), new[] { "PxWebApiValidConsumer" });
			HttpContext.Current.User = User;
		}

		protected override bool IsAuthorized(HttpActionContext actionContext)
		{
			string strValidationMessage = null;
			var isAuthorized = HttpContext.Current.User.Identity.IsAuthenticated;

			if (!isAuthorized) isAuthorized = SkipAuthorization(actionContext);

			if (!isAuthorized) isAuthorized = IsOAuthUserValid(actionContext, ref strValidationMessage);

			if (!isAuthorized)
			{
				strValidationMessage = "User is not Authenticated to execute this Action:" + actionContext.ActionDescriptor.ActionName + ". " + strValidationMessage;
				var businessExc = new BusinessException(strValidationMessage, TraceEventType.Warning);

				businessExc.LogBusinessException(strValidationMessage, TraceEventType.Warning, "PXWebAPI");
				throw businessExc;
				//return false;
			}

			CreatePxWebApiValidConsumer();
			return true;
		}

		private static bool SkipAuthorization(HttpActionContext actionContext)
		{
			Contract.Assert(actionContext != null);

			//NOTE: You can skip authorization if your controller action has this attribute:[System.Web.Http.AllowAnonymous]
			var bAllowAnonymous = actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
									  || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();

			if (bAllowAnonymous) return true;

			if (HttpContext.Current.Application["whiteListOfConsumers"] == null) return false;

			var consumerIpAddress = HttpContext.Current.Request.UserHostAddress;
			var whiteListOfConsumers = (Dictionary<string, string>)HttpContext.Current.Application["whiteListOfConsumers"];
			if (whiteListOfConsumers.Any(w => w.Key == consumerIpAddress)) bAllowAnonymous = true;

			return bAllowAnonymous;

		}

		private static bool IsOAuthUserValid(HttpActionContext actionContext, ref string strValidationMessage)
		{
			bool skipOAuth;
			Boolean.TryParse(ConfigurationManager.AppSettings["skipoauth"], out skipOAuth);

			if (skipOAuth) return true;

			var traceEventType = TraceEventType.Warning;

			if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["oauth_traceEventType"])) Enum.TryParse(ConfigurationManager.AppSettings["oauth_traceEventType"], out traceEventType);

			var context = HttpContext.Current;

			object appOAuthKeys;
			string strReturnSignatureInfo;
			string strAuthorizationHeader;

			strValidationMessage = ValidateOAuthKeys(context, out appOAuthKeys, out strReturnSignatureInfo, out strAuthorizationHeader);

			if (!String.IsNullOrEmpty(strValidationMessage))
			{
				CreateAndLogErrOAuthResponse(actionContext, strValidationMessage, traceEventType);
				return false;

			}

			var consumerSecret = "";
			//Oauth authorization header will be in the following format:
			//oauth_consumer_key="key", oauth_nonce="123456789", oauth_signature="tnnArxj06cWHq44gCs1OSKk%2FjLY%3D", 
			//oauth_signature_method="HMAC-SHA1", oauth_timestamp="1318622958" , oauth_version="1.0"

			var oAuthConsumerKeyInRequest = ValidateConsumerKeys(appOAuthKeys, ref strAuthorizationHeader, ref strValidationMessage, ref consumerSecret);

			if (!String.IsNullOrEmpty(strValidationMessage))
			{
				CreateAndLogErrOAuthResponse(actionContext, strValidationMessage, traceEventType);
				return false;
			}

			strValidationMessage = ValidateOAuthSignature(context, strReturnSignatureInfo, consumerSecret, oAuthConsumerKeyInRequest);

			if (!String.IsNullOrEmpty(strValidationMessage))
			{
				CreateAndLogErrOAuthResponse(actionContext, strValidationMessage, traceEventType);
				return false;
			}
			return true;
		}

		private static string ValidateOAuthSignature(HttpContext context, string strReturnSignatureInfo, string consumerSecret, string oAuthConsumerKeyInRequest)
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
				strOAuthBaseValidationMessage = strOAuthBaseValidationMessage + "------Signature: " + strSignature + "------Base Signature: " + strSignatureBase;
			}

			return strOAuthBaseValidationMessage;


		}


		private static string ValidateConsumerKeys(object appOAuthKeys, ref string strAuthorizationHeader, ref string strValidationMessage, ref string consumerSecret)
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

		private static string ValidateOAuthKeys(HttpContext context, out object appOAuthKeys, out string strReturnSignatureInfo, out string strAuthorizationHeader)
		{
			string strValidationMessage = null;
			//check if the application data exists
			appOAuthKeys = context.Application["oauthkeys"];
			if (appOAuthKeys == null) strValidationMessage = "Authentication failed. No 'oauthkeys' in Web.config";

			strReturnSignatureInfo = ConfigurationManager.AppSettings["returnsignatureinfo"];
			if (string.IsNullOrEmpty(strReturnSignatureInfo)) strValidationMessage = "Authentication failed.  No 'returnsignatureinfo' in Web.config";

			//get the key from the request
			strAuthorizationHeader = context.Request.Headers["Authorization"];
			if (string.IsNullOrEmpty(strAuthorizationHeader)) strValidationMessage = "Authentication failed. No 'Authorization' header found";
			return strValidationMessage;
		}


		private static void CreateAndLogErrOAuthResponse(HttpActionContext actionContext, string strValidationMessage, TraceEventType traceEventType)
		{
			var businessExc = new BusinessException(strValidationMessage, traceEventType);

			businessExc.LogBusinessException(strValidationMessage, traceEventType, "PXWebAPI");

			actionContext.Response = new HttpResponseMessage()
			{
				Content =
					new StringContent(
					JsonConvert.SerializeObject(new OAuthResponse
					{
						status_code = -1,
						error_message =
							strValidationMessage,
						results = null
					}))
			};
		}
	}



}