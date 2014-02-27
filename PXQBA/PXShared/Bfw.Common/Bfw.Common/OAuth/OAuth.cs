using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace Bfw.Common.OAuth
{
    public class OAuth
    {
        private const string AlphaNumeric = Upper + Lower + Digit;
        private const string Digit = "1234567890";
        private const string Lower = "abcdefghijklmnopqrstuvwxyz";
        private const string Unreserved = AlphaNumeric + "-._~";
        private const string Upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static NameValueCollection GeneratePXAuthHeader(Dictionary<string, string> postData, Uri url, string httpMethod, out string signatureBase)
        {
            var oAuth = new OAuthBase();
            string key = ConfigurationManager.AppSettings["pxapi_key"];
            Dictionary<string, string> data;
            if (postData == null)
            {
                data = new Dictionary<string, string>();
            }
            else
            {
                data = new Dictionary<string, string>(postData);
            }
            data.Add(OAuthBase.OAuthConsumerKeyKey, key);
            data.Add(OAuthBase.OAuthVersionKey, "1.0");
            data.Add(OAuthBase.OAuthSignatureMethodKey, "HMAC-SHA1");

            string secret = ConfigurationManager.AppSettings["pxapi_secret"];
            var strTimeStamp = oAuth.GenerateTimeStamp();
            var strNonce = oAuth.GenerateNonce();
            string strBaseSignature;
            string strSignature = oAuth.GenerateSignature(data, url, key, secret, null, null,
                                   httpMethod, strTimeStamp, strNonce,
                                   OAuthBase.SignatureTypes.HMACSHA1, out strBaseSignature);
            signatureBase = strBaseSignature;
            var nvc = HttpUtility.ParseQueryString(string.Empty);
            var authorizationHeaderParams = new StringBuilder("OAuth ");
            authorizationHeaderParams.AppendFormat("{0}=\"{1}\", ", OAuthBase.OAuthConsumerKeyKey, UrlEncodeRelaxed(key));
            authorizationHeaderParams.AppendFormat("{0}=\"{1}\", ", OAuthBase.OAuthNonceKey, UrlEncodeRelaxed(strNonce));
            authorizationHeaderParams.AppendFormat("{0}=\"{1}\", ", OAuthBase.OAuthSignatureKey, UrlEncodeRelaxed(strSignature.Trim()));
            authorizationHeaderParams.AppendFormat("{0}=\"{1}\", ", OAuthBase.OAuthSignatureMethodKey, "HMAC-SHA1");
            authorizationHeaderParams.AppendFormat("{0}=\"{1}\", ", OAuthBase.OAuthTimestampKey, UrlEncodeRelaxed(strTimeStamp));
            //oath_token - not used
            authorizationHeaderParams.AppendFormat("{0}=\"{1}\", ", OAuthBase.OAuthVersionKey, "1.0");
            nvc.Add("Authorization", authorizationHeaderParams.ToString().Trim(", ".ToCharArray()));
            return nvc;
        }

        public static NameValueCollection GeneratePXAuthHeader(Dictionary<string, string> postData, Uri url, string httpMethod)
        {
            string signatureBase;
            return GeneratePXAuthHeader(postData, url, httpMethod, out signatureBase);
        }

        /// <summary>
        /// The set of characters that are unreserved in RFC 2396 but are NOT unreserved in RFC 3986.
        /// </summary>
        /// <seealso cref="http://stackoverflow.com/questions/846487/how-to-get-uri-escapedatastring-to-comply-with-rfc-3986" />
        private static readonly string[] UriRfc3986CharsToEscape = new[] { "!", "*", "'", "(", ")" };

        private static readonly string[] UriRfc3968EscapedHex = new[] { "%21", "%2A", "%27", "%28", "%29" };

        /// <summary>
        /// URL encodes a string based on section 5.1 of the OAuth spec.
        /// Namely, percent encoding with [RFC3986], avoiding unreserved characters,
        /// upper-casing hexadecimal characters, and UTF-8 encoding for text value pairs.
        /// </summary>
        /// <param name="value">The value to escape.</param>
        /// <returns>The escaped value.</returns>
        /// <seealso cref="http://oauth.net/core/1.0#encoding_parameters" />
        /// <seealso cref="https://raw.github.com/restsharp/RestSharp/master/RestSharp/Authenticators/OAuth/OAuthTools.cs" />
        public static string UrlEncodeRelaxed(string value)
        {
            StringBuilder escaped = new StringBuilder(Uri.EscapeDataString(value));

            for (int i = 0; i < UriRfc3986CharsToEscape.Length; i++)
            {
                string t = UriRfc3986CharsToEscape[i];
                escaped.Replace(t, UriRfc3968EscapedHex[i]);
            }

            return escaped.ToString();
        }

        /// <summary>
        /// URL encodes a string based on section 5.1 of the OAuth spec.
        /// Namely, percent encoding with [RFC3986], avoiding unreserved characters,
        /// upper-casing hexadecimal characters, and UTF-8 encoding for text value pairs.
        /// </summary>
        /// <param name="value"></param>
        /// <seealso cref="http://oauth.net/core/1.0#encoding_parameters" />
        /// /// <seealso cref="https://raw.github.com/restsharp/RestSharp/master/RestSharp/Authenticators/OAuth/OAuthTools.cs" />
        public static string UrlEncodeStrict(string value)
        {
            // [JD]: We need to escape the apostrophe as well or the signature will fail
            var original = value;
            var ret = original.Where(
                c => !Unreserved.Contains(c) && c != '%').Aggregate(
                    value, (current, c) => current.Replace(
                        c.ToString(), PercentEncode(c.ToString())
                ));

            return ret.Replace("%%", "%25%"); // Revisit to encode actual %'s
        }

        private static string PercentEncode(string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                // [DC]: Support proper encoding of special characters (\n\r\t\b)
                if ((b > 7 && b < 11) || b == 13)
                {
                    sb.Append(String.Format("%0{0:X}", (object)b));
                }
                else
                {
                    sb.Append(String.Format("%{0:X}", (object)b));
                }
            }
            return sb.ToString();
        }

    }
}