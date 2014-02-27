using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Bfw.PXWebAPI.Models.Response;
using Newtonsoft.Json;
using BLTI1 = BLTI.BLTI;
using Bfw.PXWebAPI.Models;

namespace PXWebAPI
{
    public class OAuthValidationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            try
            {
                var strSkipOAuth = ConfigurationManager.AppSettings["skipoauth"];
                var strReturnSignatureInfo = ConfigurationManager.AppSettings["returnsignatureinfo"];
                var blnSkipOAuth = !string.IsNullOrEmpty(strSkipOAuth) && Convert.ToBoolean(strSkipOAuth);
                var blnReturnSignatureInfo = !string.IsNullOrEmpty(strReturnSignatureInfo) && Convert.ToBoolean(strReturnSignatureInfo);
                if (!blnSkipOAuth)
                {
                    HttpContext context = HttpContext.Current;
                    var bObj = new BLTI.OAuthBase();
                    var strValidationMessage = "";
                    var consumerSecret = "";

                    # region "Get the secret for the key"

                    //check if the application data exists
                    //To Do: Clean up
                    var appOAuthKeys = context.Application["oauthkeys"];
                    string oAuthConsumerKeyInRequest = "";
                    if (appOAuthKeys == null)
                    {
                        strValidationMessage = "Authentication failed. No keys found";
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
                        return;
                    }
                    else
                    {
                        //get the key from the request
                        var strAuthorizationHeader = context.Request.Headers["Authorization"];
                       
                        if (string.IsNullOrEmpty(strAuthorizationHeader))
                        {
                            //If no Authorization header found
                            strValidationMessage = "Authentication failed. No authorization header found";
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
                            return;
                        }
                        const string oauthHeaderPrefix = "Oauth ";
                        //Oauth authorization header will be in the following format
                        //OAuth oauth_consumer_key="key", oauth_nonce="123456789", oauth_signature="tnnArxj06cWHq44gCs1OSKk%2FjLY%3D", oauth_signature_method="HMAC-SHA1", oauth_timestamp="1318622958" , oauth_version="1.0"
                        strAuthorizationHeader = strAuthorizationHeader.Trim().Substring(oauthHeaderPrefix.Length);
                        var splitOauthParams = strAuthorizationHeader.Split(',');
                        var oAuthParams = new Dictionary<string, string>();
                        foreach (var splitOauthParam in splitOauthParams)
                        {
                            var firstIndexofEqualTo = splitOauthParam.IndexOf('=');
                            var key = HttpUtility.UrlDecode(splitOauthParam.Substring(0,firstIndexofEqualTo).Trim());
                            var value = HttpUtility.UrlDecode(splitOauthParam.Substring(firstIndexofEqualTo+1).Trim().Trim('"'));
                            oAuthParams.Add(key,value);
                        }
                        oAuthConsumerKeyInRequest = oAuthParams["oauth_consumer_key"];
                        if (string.IsNullOrEmpty(oAuthConsumerKeyInRequest))
                        {
                            //If no key found in the request
                            strValidationMessage = "Authentication failed. No key found in request";
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
                            return;
                        }
                        else
                        {
                            OAuthKeyRepository oAuthRepo = (OAuthKeyRepository) appOAuthKeys;
                            consumerSecret = oAuthRepo.GetSecret(oAuthConsumerKeyInRequest);
                            if (string.IsNullOrEmpty(consumerSecret))
                            {
                                //if no secret found for the key
                                strValidationMessage = "Authentication failed. No secret found for the key in request";
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
                                return;
                            }
                        }
                    }

                    #endregion

                    var strSignatureBase = "";
                    var strSignature = "";
                    var isOauthRequestValid = bObj.ValidateSignature(context.Request, oAuthConsumerKeyInRequest, consumerSecret,
                                                                                       out strValidationMessage,
                                                                                       out strSignatureBase,
                                                                                       out strSignature);

                    if (!isOauthRequestValid)
                    {
                        if (blnReturnSignatureInfo)
                        strValidationMessage = strValidationMessage + "------Siganture: " + strSignature +
                                               "------Base Siganture: " + strSignatureBase;

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
                        return;
                    }
                }
                else
                {
                    return;
                }

            }
            catch (Exception)
            {
                actionContext.Response = new HttpResponseMessage()
                                             {
                                                 Content = new StringContent(JsonConvert.SerializeObject(new OAuthResponse { status_code = -1, error_message = "Authentication failed for some unknown reasons", results = null }))
                                             };
                return;

            }
        }
    }



}