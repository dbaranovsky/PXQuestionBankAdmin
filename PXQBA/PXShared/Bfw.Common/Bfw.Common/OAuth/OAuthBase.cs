using System;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Bfw.Common.OAuth
{
    public class OAuthBase
    {

        /// <summary>
        /// Provides a predefined set of algorithms that are supported officially by the protocol
        /// </summary>
        public enum SignatureTypes
        {
            HMACSHA1,
            PLAINTEXT,
            RSASHA1
        }
        public HttpRequest Http_Request
        {
            get;
            set;
        }

        /// <summary>
        /// Provides an internal structure to sort the request parameter
        /// </summary>
        public class RequestParameter
        {
            private string name = null;
            private string value = null;

            public RequestParameter(string name, string value)
            {
                this.name = name;
                this.value = value;
            }

            public string Name
            {
                get { return name; }
            }

            public string Value
            {
                get { return value; }
            }
        }

        public class RequestParameters : ICollection<RequestParameter>
        {
            public List<RequestParameter> parameters
            {
                get;
                set;
            }

            public RequestParameters()
            {
                parameters = new List<RequestParameter>();
            }

            public void Add(RequestParameter item)
            {
                //Check to see if a parameter with the same name exists.
                //Add only if the entry doesn't exist.
                //If it exits, then replace the value
                foreach (RequestParameter rp in parameters)
                {
                    if (rp.Name.Trim().Equals(item.Name.Trim()))
                    {
                        parameters.Remove(rp);
                        break;
                    }
                }
                parameters.Add(item);
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(RequestParameter item)
            {
                return parameters.Contains(item);
            }

            public void CopyTo(RequestParameter[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public int Count
            {
                get { return parameters.Count; }
            }

            public bool IsReadOnly
            {
                get { throw new NotImplementedException(); }
            }

            public bool Remove(RequestParameter item)
            {
                return parameters.Remove(item);
            }

            public IEnumerator<RequestParameter> GetEnumerator()
            {
                return parameters.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Comparer class used to perform the sorting of the query parameters
        /// </summary>
        protected class RequestParameterComparer : IComparer<RequestParameter>
        {

            #region IComparer<RequestParameter> Members

            public int Compare(RequestParameter x, RequestParameter y)
            {
                if (x.Name == y.Name)
                {
                    return string.Compare(x.Value, y.Value);
                }
                else
                {
                    return string.Compare(x.Name, y.Name);
                }
            }

            #endregion
        }

        public const string OAuthVersion = "1.0";
        public const string OAuthParameterPrefix = "oauth_";

        //
        // List of know and used oauth parameters' names
        //        
        public const string OAuthConsumerKeyKey = "oauth_consumer_key";
        public const string OAuthCallbackKey = "oauth_callback";
        public const string OAuthVersionKey = "oauth_version";
        public const string OAuthSignatureMethodKey = "oauth_signature_method";
        public const string OAuthSignatureKey = "oauth_signature";
        public const string OAuthTimestampKey = "oauth_timestamp";
        public const string OAuthNonceKey = "oauth_nonce";
        public const string OAuthTokenKey = "oauth_token";
        public const string OAuthTokenSecretKey = "oauth_token_secret";

        public const string HMACSHA1SignatureType = "HMAC-SHA1";
        public const string PlainTextSignatureType = "PLAINTEXT";
        public const string RSASHA1SignatureType = "RSA-SHA1";
        public const int TimeStampThreshold = 300;

        public const string OauthHeaderPrefix = "Oauth ";



        protected Random random = new Random();

        protected string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        /// <summary>
        /// Helper function to compute a hash value
        /// </summary>
        /// <param name="hashAlgorithm">The hashing algoirhtm used. If that algorithm needs some initialization, like HMAC and its derivatives, they should be initialized prior to passing it to this function</param>
        /// <param name="data">The data to hash</param>
        /// <returns>a Base64 string of the hash value</returns>
        private string ComputeHash(HashAlgorithm hashAlgorithm, string data)
        {
            if (hashAlgorithm == null)
            {
                throw new ArgumentNullException("hashAlgorithm");
            }

            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException("data");
            }

            byte[] dataBuffer = Encoding.ASCII.GetBytes(data);
            byte[] hashBytes = hashAlgorithm.ComputeHash(dataBuffer);

            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Internal function to cut out all non oauth query string parameters (all parameters not begining with "oauth_")
        /// </summary>
        /// <param name="parameters">The query string part of the Url</param>
        /// <returns>A list of RequestParameter each containing the parameter name and value</returns>
        private RequestParameters GetRequestParameters(string parameters)
        {
            if (parameters.StartsWith("?"))
            {
                parameters = parameters.Remove(0, 1);
            }

            RequestParameters result = new RequestParameters();

            if (!string.IsNullOrEmpty(parameters))
            {
                string[] p = parameters.Split('&');
                foreach (string s in p)
                {
                    if (!string.IsNullOrEmpty(s) && !s.StartsWith(OAuthParameterPrefix))
                    {
                        if (s.IndexOf('=') > -1)
                        {
                            string[] temp = s.Split('=');
                            result.Add(new RequestParameter(temp[0], temp[1]));
                        }
                        else
                        {
                            result.Add(new RequestParameter(s, string.Empty));
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// This is a different Url Encode implementation since the default .NET one outputs the percent encoding in lower case.
        /// While this is not a problem with the percent encoding spec, it is used in upper case throughout OAuth
        /// </summary>
        /// <param name="value">The value to Url encode</param>
        /// <returns>Returns a Url encoded string</returns>
        protected string UrlEncode(string value)
        {
            StringBuilder result = new StringBuilder();

            foreach (char symbol in value)
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append('%' + String.Format("{0:X2}", (int)symbol));
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Normalizes the request parameters according to the spec
        /// </summary>
        /// <param name="parameters">The list of parameters already sorted</param>
        /// <returns>a string representing the normalized parameters</returns>
        protected string NormalizeRequestParameters(RequestParameters parameters)
        {
            StringBuilder sb = new StringBuilder();
            RequestParameter p = null;
            for (int i = 0; i < parameters.Count; i++)
            {
                p = parameters.parameters[i];
                //URL Encode the key and value of each parameter. This is required by OAuth
                sb.AppendFormat("{0}={1}", UrlEncode(p.Name), UrlEncode(p.Value));

                if (i < parameters.Count - 1)
                {
                    sb.Append("&");
                }
            }

            return sb.ToString();
        }

        private string GenerateSignatureBase(Dictionary<string, string> signatureParams, Uri url, string consumerKey, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce, string signatureType)
        {
            if (token == null)
            {
                token = string.Empty;
            }

            if (tokenSecret == null)
            {
                tokenSecret = string.Empty;
            }

            if (timeStamp == null)
            {
                timeStamp = string.Empty;
            }

            if (nonce == null)
            {
                nonce = string.Empty;
            }

            if (string.IsNullOrEmpty(consumerKey))
            {
                throw new ArgumentNullException("consumerKey");
            }

            if (string.IsNullOrEmpty(httpMethod))
            {
                throw new ArgumentNullException("httpMethod");
            }

            if (string.IsNullOrEmpty(signatureType))
            {
                throw new ArgumentNullException("signatureType");
            }

            string normalizedUrl = null;
            string normalizedRequestParameters = null;

            RequestParameters parameters = GetRequestParameters(url.Query);
            foreach (KeyValuePair<string, string> qp in signatureParams)
            {
                //add all lti parameters except for the siganture parameter
                if (!qp.Key.Equals(OAuthSignatureKey))
                    parameters.Add(new RequestParameter(qp.Key, qp.Value));
            }
            //Add timestamp and nonce
            //will overwrite if already added
            if (timeStamp != string.Empty)
                parameters.Add(new RequestParameter(OAuthTimestampKey, timeStamp));
            if (nonce != string.Empty)
                parameters.Add(new RequestParameter(OAuthNonceKey, nonce));

            if (!string.IsNullOrEmpty(token))
            {
                parameters.Add(new RequestParameter(OAuthTokenKey, token));
            }

            parameters.parameters.Sort(new RequestParameterComparer());

            normalizedUrl = string.Format("{0}://{1}", url.Scheme, url.Host);
            if (!((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)))
            {
                normalizedUrl += ":" + url.Port;
            }
            normalizedUrl += url.AbsolutePath;
            normalizedRequestParameters = NormalizeRequestParameters(parameters);

            StringBuilder signatureBase = new StringBuilder();
            signatureBase.AppendFormat("{0}&", httpMethod.ToUpper());
            signatureBase.AppendFormat("{0}&", UrlEncode(normalizedUrl));
            signatureBase.AppendFormat("{0}", UrlEncode(normalizedRequestParameters));

            return signatureBase.ToString();
        }


        /// <summary>
        /// Generate the signature value based on the given signature base and hash algorithm
        /// </summary>
        /// <param name="signatureBase">The signature based as produced by the GenerateSignatureBase method or by any other means</param>
        /// <param name="hash">The hash algorithm used to perform the hashing. If the hashing algorithm requires initialization or a key it should be set prior to calling this method</param>
        /// <returns>A base64 string of the hash value</returns>
        public string GenerateSignatureUsingHash(string signatureBase, HashAlgorithm hash)
        {
            return ComputeHash(hash, signatureBase);
        }

        /// <summary>
        /// Generates a signature using the HMAC-SHA1 algorithm
        /// </summary>		
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="consumerSecret">The consumer seceret</param>
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <returns>A base64 string of the hash value</returns>
        public string GenerateSignature(Dictionary<string, string> signatureParams, Uri url, string consumerKey, string consumerSecret, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce, out string signatureBase)
        {
            return GenerateSignature(signatureParams, url, consumerKey, consumerSecret, token, tokenSecret, httpMethod, timeStamp, nonce, SignatureTypes.HMACSHA1, out signatureBase);
        }

        /// <summary>
        /// Generates a signature using the specified signatureType 
        /// </summary>		
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="consumerSecret">The consumer seceret</param>
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="signatureType">The type of signature to use</param>
        /// <returns>A base64 string of the hash value</returns>
        public string GenerateSignature(Dictionary<string, string> signatureParams, Uri url, string consumerKey, string consumerSecret, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce, SignatureTypes signatureType, out string signatureBase)
        {
            signatureBase = "";
            switch (signatureType)
            {
                case SignatureTypes.PLAINTEXT:
                    return HttpUtility.UrlEncode(string.Format("{0}&{1}", consumerSecret, tokenSecret));
                case SignatureTypes.HMACSHA1:
                    signatureBase = GenerateSignatureBase(signatureParams, url, consumerKey, token, tokenSecret, httpMethod, timeStamp, nonce, HMACSHA1SignatureType);
                    HMACSHA1 hmacsha1 = new HMACSHA1();
                    hmacsha1.Key = Encoding.ASCII.GetBytes(string.Format("{0}&{1}", UrlEncode(consumerSecret), string.IsNullOrEmpty(tokenSecret) ? "" : UrlEncode(tokenSecret)));
                    return GenerateSignatureUsingHash(signatureBase, hmacsha1);
                case SignatureTypes.RSASHA1:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentException("Unknown signature type", "signatureType");
            }
        }

        /// <summary>
        /// Validates a signature
        /// </summary>
        /// <param name="signatureParams">signature parameters in a dictionary of string key and string value</param>
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="consumerSecret">The consumer seceret</param>
        /// <param name="strSignatureSentInRequest">The signature that was sent in the request that needs to be validated</param>
        /// <param name="strSignatureBase">out parameter that gives the base signature that is used to build the actual signature. Useful for debugging purposes</param>
        /// <param name="strValidationMessage">out parameter to return the validation message</param>
        /// <param name="strSignatureGenerated">out parameter to return the signature that was generated</param>
        /// <param name="intTimeStampThreshold">Timestamp threshold (in secs) to compare the current timestamp and the timestamp sent in the request</param>
        /// <returns>True or False</returns>
        public bool ValidateSignature(Dictionary<string, string> signatureParams, Uri url, string consumerKey, string consumerSecret, string strHttpMethod, int intTimeStampThreshold, out string strSignatureBase, out string strValidationMessage, out string strSignatureGenerated)
        {
            strSignatureGenerated = "";
            strSignatureBase = "";
            if (!IsBasicOauthRequest(signatureParams, out strValidationMessage))
                return false;
            //Check timestamp
            if (!CheckTimeStamp(signatureParams[OAuthTimestampKey], intTimeStampThreshold))
            {
                strValidationMessage = "Error: Timestamp expired!";
                return false;
            }
            var strSignatureSentInRequest = signatureParams[OAuthSignatureKey];
            //Generate the signature
            strSignatureGenerated = GenerateSignature(signatureParams, url, consumerKey, consumerSecret, null, null, strHttpMethod, null, null, SignatureTypes.HMACSHA1, out strSignatureBase);
            //Check the siganture
            if (strSignatureGenerated.Equals(strSignatureSentInRequest))
            {
                strValidationMessage = "OK: validation passed!";
                return true;
            }

            strValidationMessage = "Error: Signatures did not match!";
            return false;
        }

        /// <summary>
        /// Validates a signature
        /// </summary>
        /// <param name="signatureParams">signature parameters in a dictionary of string key and string value</param>
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="consumerSecret">The consumer seceret</param>
        /// <param name="strSignatureSentInRequest">The signature that was sent in the request that needs to be validated</param>
        /// <param name="strSignatureBase">out parameter that gives the base signature that is used to build the actual signature. Useful for debugging purposes</param>
        /// <param name="strValidationMessage">out parameter to return the validation message</param>
        /// <param name="strSignatureGenerated">out parameter to return the signature that was generated</param>
        /// <param name="intTimeStampThreshold">Timestamp threshold (in secs) to compare the current timestamp and the timestamp sent in the request</param>
        /// <returns>True or False</returns>
        public bool ValidateSignature(HttpRequest httpRequest, string consumerKey, string consumerSecret, out string strSignatureBase, out string strValidationMessage, out string strSignatureGenerated, int intTimeStampThreshold = TimeStampThreshold)
        {
            strSignatureGenerated = "";
            strSignatureBase = "";
            //If a httprequest was not assigned to the BLTI object then return with the error message
            if (httpRequest == null)
            {
                strValidationMessage = "Error: No HTTPRequest object set!";
                return false;
            }
            Uri uri = httpRequest.Url;
            //Get the post parameters from the HTTP requst object
            var signParameters = ReturnPostandHeaderParameters(httpRequest);
            return ValidateSignature(signParameters, uri, consumerKey, consumerSecret, httpRequest.HttpMethod, intTimeStampThreshold, out strValidationMessage, out strSignatureBase, out strSignatureGenerated);
        }

        /// <summary>
        /// Populates BltiLaunchParameters with the POST parameters in httpRequest
        /// </summary>
        /// <param name="httpRequest">The launch URL</param>
        private Dictionary<string, string> ReturnPostandHeaderParameters(HttpRequest httpRequest)
        {
            var retParams = new Dictionary<string, string>();
            foreach (string name in httpRequest.Form)
            {
                if (retParams.ContainsKey(name))
                    retParams[name] = httpRequest.Form[name];
                else
                    retParams.Add(name, httpRequest.Form[name]);
            }
            var strAuthorizationHeader = httpRequest.Headers["Authorization"];
            var headers = httpRequest.Headers;
            //Oauth authorization header will be in the following format
            //OAuth oauth_consumer_key="key", oauth_nonce="123456789", oauth_signature="tnnArxj06cWHq44gCs1OSKk%2FjLY%3D", oauth_signature_method="HMAC-SHA1", oauth_timestamp="1318622958" , oauth_version="1.0"
            strAuthorizationHeader = strAuthorizationHeader.Trim().Substring(OauthHeaderPrefix.Length);
            var splitOauthParams = strAuthorizationHeader.Split(',');
            var oAuthParams = new Dictionary<string, string>();
            foreach (var splitOauthParam in splitOauthParams)
            {
                var firstIndexofEqualTo = splitOauthParam.IndexOf('=');
                var key = HttpUtility.UrlDecode(splitOauthParam.Substring(0, firstIndexofEqualTo).Trim());
                var value = HttpUtility.UrlDecode(splitOauthParam.Substring(firstIndexofEqualTo + 1).Trim().Trim('"'));
                oAuthParams.Add(key, value);
            }
            foreach (KeyValuePair<string, string> name in oAuthParams)
            {
                if (retParams.ContainsKey(name.Key))
                    retParams[name.Key] = name.Value;
                else
                    retParams.Add(name.Key, name.Value);
            }
            return retParams;
        }

        public static bool IsOauthParameter(string paramName)
        {

            if (paramName.Equals(OAuthCallbackKey))
                return true;
            if (paramName.Equals(OAuthConsumerKeyKey))
                return true;
            if (paramName.Equals(OAuthNonceKey))
                return true;
            if (paramName.Equals(OAuthSignatureKey))
                return true;
            if (paramName.Equals(OAuthSignatureMethodKey))
                return true;
            if (paramName.Equals(OAuthTimestampKey))
                return true;
            if (paramName.Equals(OAuthTokenKey))
                return true;
            if (paramName.Equals(OAuthTokenSecretKey))
                return true;
            if (paramName.Equals(OAuthVersionKey))
                return true;
            return false;
        }

        /// <summary>
        /// Validates if the request is a valid OAuth request. 
        /// </summary>
        /// <returns>True or False</returns>
        private bool IsBasicOauthRequest(Dictionary<string, string> signatureParams, out string strValidationMessage)
        {

            //Consumer Key
            if (!signatureParams.ContainsKey(OAuthBase.OAuthConsumerKeyKey) || String.IsNullOrEmpty(signatureParams[OAuthBase.OAuthConsumerKeyKey]))
            {
                strValidationMessage = "Error: No consumer key was provided!";
                return false;
            }

            //OAuth Version
            if (!signatureParams.ContainsKey(OAuthBase.OAuthVersionKey) || String.IsNullOrEmpty(signatureParams[OAuthBase.OAuthVersionKey]))
            {
                strValidationMessage = "Error: No OAuth version was provided!";
                return false;
            }

            //OAuth Signature Method
            if (!signatureParams.ContainsKey(OAuthBase.OAuthSignatureMethodKey) || String.IsNullOrEmpty(signatureParams[OAuthBase.OAuthSignatureMethodKey]))
            {
                strValidationMessage = "Error: No OAuth signature method was provided!";
                return false;
            }

            //OAuth Timestamp
            if (!signatureParams.ContainsKey(OAuthBase.OAuthTimestampKey) || String.IsNullOrEmpty(signatureParams[OAuthBase.OAuthTimestampKey]))
            {
                strValidationMessage = "Error: No OAuth timestamp was provided!";
                return false;
            }

            //OAuth Nonce
            if (!signatureParams.ContainsKey(OAuthBase.OAuthNonceKey) || String.IsNullOrEmpty(signatureParams[OAuthBase.OAuthNonceKey]))
            {
                strValidationMessage = "Error: No OAuth nonce was provided!";
                return false;
            }

            strValidationMessage = "OK: Success!";
            return true;
        }


        /// <summary>
        /// Validates the timestamp
        /// </summary>
        /// <param name="strTimeStamp">Timestamp string that needs to be validated</param>
        /// <param name="intTimeStampThreshold">Timestamp threshold (in secs) to compare the current timestamp and the timestamp sent in the request</param>
        /// <returns>True or False</returns>
        private bool CheckTimeStamp(string strTimeStamp, int intTimeStampThreshold)
        {
            //Get the current timestamp and check against the timestamp sent in the request
            //if the difference is over the desired time, then the timestamp is expired
            //Currently we check for 300 seconds (5mins)
            int currTimeStamp = Convert.ToInt32(GenerateTimeStamp());
            if ((currTimeStamp - Convert.ToInt32(strTimeStamp)) > intTimeStampThreshold)
                return false;
            return true;
        }

        /// <summary>
        /// Generate the timestamp for the signature        
        /// </summary>
        /// <returns></returns>
        public virtual string GenerateTimeStamp()
        {
            // Default implementation of UNIX time of the current UTC time
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// Generate a nonce
        /// </summary>
        /// <returns></returns>
        public virtual string GenerateNonce()
        {
            // Just a simple implementation of a random number between 123400 and 9999999
            return random.Next(123400, 9999999).ToString();
        }

    }
}