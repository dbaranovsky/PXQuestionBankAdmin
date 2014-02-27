using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using BLTI;


namespace Bfw.BLTIProvider
{
	public partial class OAuth : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}
		protected const string OAuthConsumerKeyKey = "oauth_consumer_key";
		protected const string OAuthCallbackKey = "oauth_callback";
		protected const string OAuthVersionKey = "oauth_version";
		protected const string OAuthSignatureMethodKey = "oauth_signature_method";
		protected const string OAuthSignatureKey = "oauth_signature";
		protected const string OAuthTimestampKey = "oauth_timestamp";
		protected const string OAuthNonceKey = "oauth_nonce";
		protected const string OAuthTokenKey = "oauth_token";
		protected const string OAuthTokenSecretKey = "oauth_token_secret";
		public const string HeaderAuthorizationKey = "Authorization";
		protected void Button1_Click(object sender, EventArgs e)
		{
			BLTI.OAuthBase bObj = new BLTI.OAuthBase() { };
			var postdata = new StringBuilder();
			//Mandatory Params
			var oAuthParams = new Dictionary<string, string>();
			oAuthParams.Add(OAuthConsumerKeyKey, txtKey.Text.Trim());
			oAuthParams.Add(OAuthVersionKey, "1.0");
			bool isOauthCallbackChecked = false;
			if (chkOAuthCallback.Checked)
			{
				isOauthCallbackChecked = true;
				oAuthParams.Add(OAuthCallbackKey, "about:blank");
			}
			oAuthParams.Add(OAuthSignatureMethodKey, "HMAC-SHA1");
			if (txtHttpMethod.Text.Trim().Equals("POST"))
			{
				if (!txtRequestBody.Text.Trim().Equals(""))
				{
					string strRequestBody = txtRequestBody.Text.Trim();
					string[] postparamssplit = strRequestBody.Split('&');
					foreach (var s in postparamssplit)
					{
						string[] pparams = s.Split('=');
						oAuthParams.Add(pparams[0], pparams[1]);
					}
				}
			}
			var strBaseSignature = "";
			var strValidationMessage = "";
			var strSignature = "";
			var strTimeStamp = bObj.GenerateTimeStamp();
			var strNonce = bObj.GenerateNonce();
			Uri uri = new Uri(txtLaunchURL.Text.Trim());
			strSignature = bObj.GenerateSignature(oAuthParams, uri, txtKey.Text.Trim(), txtSecret.Text.Trim(), null, null,
								   txtHttpMethod.Text.Trim(), strTimeStamp, strNonce,
								   OAuthBase.SignatureTypes.HMACSHA1, out strBaseSignature);
			if (!strSignature.Trim().Equals(""))
			{
				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(txtLaunchURL.Text.Trim());
				req.Method = txtHttpMethod.Text.Trim();
				//Oauth authorization header will be in the following format
				//OAuth oauth_consumer_key="key", oauth_nonce="123456789", oauth_signature="tnnArxj06cWHq44gCs1OSKk%2FjLY%3D", oauth_signature_method="HMAC-SHA1", oauth_timestamp="1318622958" , oauth_version="1.0"
				StringBuilder sbOauth = new StringBuilder();
				sbOauth.Append("Oauth ");
				sbOauth.Append(HttpUtility.UrlEncode("realm") + "=\"" + HttpUtility.UrlEncode(uri.ToString()) + "\", ");
				sbOauth.Append(HttpUtility.UrlEncode(OAuthConsumerKeyKey) + "=\"" + HttpUtility.UrlEncode(txtKey.Text.Trim()) + "\", ");
				if (isOauthCallbackChecked)
					sbOauth.Append(HttpUtility.UrlEncode(OAuthCallbackKey) + "=\"" + HttpUtility.UrlEncode("about:blank") + "\", ");
				sbOauth.Append(HttpUtility.UrlEncode(OAuthVersionKey) + "=\"" + HttpUtility.UrlEncode("1.0") + "\", ");
				sbOauth.Append(HttpUtility.UrlEncode(OAuthSignatureMethodKey) + "=\"" + HttpUtility.UrlEncode("HMAC-SHA1") + "\", ");
				sbOauth.Append(HttpUtility.UrlEncode(OAuthSignatureKey) + "=\"" + HttpUtility.UrlEncode(strSignature.Trim()) + "\", ");
				sbOauth.Append(HttpUtility.UrlEncode(OAuthTimestampKey) + "=\"" + HttpUtility.UrlEncode(strTimeStamp) + "\", ");
				sbOauth.Append(HttpUtility.UrlEncode(OAuthNonceKey) + "=\"" + HttpUtility.UrlEncode(strNonce) + "\"");
				req.Headers.Add(HeaderAuthorizationKey, sbOauth.ToString().Trim());

				req.Headers.Add("Accept-Encoding", "gzip,deflate");

				// req.Headers.Add("Content-Length", );
				if (req.Method.Trim().Equals("POST"))
				{
					req.ContentType = "application/x-www-form-urlencoded";
					var strPostData = txtRequestBody.Text.Trim(); //postdata.ToString().Trim('&');
					byte[] postData = Encoding.UTF8.GetBytes(strPostData);
					req.ContentLength = txtRequestBody.Text.Trim().Length;
					req.GetRequestStream();
					Stream newStream = req.GetRequestStream();
					newStream.Write(postData, 0, postData.Length);

					// Close the Stream object.
					newStream.Close();

				}

				HttpWebResponse loWebResponse = (HttpWebResponse)req.GetResponse();
				Encoding enc = Encoding.GetEncoding(1252);  // Windows default Code Page
				StreamReader loResponseStream =
				   new StreamReader(loWebResponse.GetResponseStream(), enc);

				string lcHtml = loResponseStream.ReadToEnd();

				loWebResponse.Close();
				loResponseStream.Close();
				ResponseResult.Text = lcHtml;
			}
			else
			{
				lblBaseSiganture.Text = "";
				lblSignatureGenerated.Text = strValidationMessage;
			}
		}
	}
}