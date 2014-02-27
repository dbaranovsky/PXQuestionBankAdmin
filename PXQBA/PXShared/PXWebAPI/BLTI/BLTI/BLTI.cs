using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace BLTI
{
	public class BLTI
	{
		public const string BltiLtiMessageTypeKey = "lti_message_type";
		public const string BltiLtiMessageTypeValue = "basic-lti-launch-request";
		public const string BltiLtiVersionKey = "lti_version";
		public const string BltiLtiVersionValue = "LTI-1p0";
		public const string BltiResourceLinkIdKey = "resource_link_id";
		public const int TimeStampThreshold = 300;

		public HttpRequest BltiHttpRequest
		{
			get;
			set;
		}

		public Dictionary<string, string> BltiLaunchParameters
		{
			get;
			set;
		}

		public BLTI()
		{
			BltiLaunchParameters = new Dictionary<string, string>();
		}

		public BLTI(HttpRequest request)
		{
			BltiHttpRequest = request;
			BltiLaunchParameters = new Dictionary<string, string>();

			////Consumer Key
			////OAuth Version
			////OAuth Signature Method
			////OAuth Timestamp		
			////OAuth Nonce
			////OAuthSignatureKey) 
			////OAuthConsumerKeyKey

			foreach (var objKey in BltiHttpRequest.Params.Keys.Cast<object>().Where(key => key != null))
			{
				var key = objKey.ToString();
				if (key == BltiLtiMessageTypeKey || key == BltiLtiVersionKey || key == BltiResourceLinkIdKey ||
					key == OAuthBase.OAuthConsumerKeyKey || key == OAuthBase.OAuthVersionKey ||
					key == OAuthBase.OAuthSignatureMethodKey || key == OAuthBase.OAuthTimestampKey ||
					key == OAuthBase.OAuthNonceKey || key == OAuthBase.OAuthSignatureKey)
				{
					BltiLaunchParameters.Add(key, BltiHttpRequest.Params[key]);
				}
			}
		}

		/// <summary>
		/// Validates if the request is a valid BLTI request. This method will check to see if LTI message type, LTI Version and Resournce Link Id are provided
		/// </summary>
		/// <returns>True or False</returns>
		private bool IsBasicLtiRequest()
		{

			if (BltiLaunchParameters.Count == 0)
				return false;

			if (!BltiLaunchParameters.ContainsKey(BltiLtiMessageTypeKey))
				return false;

			if (!BltiLaunchParameters[BltiLtiMessageTypeKey].Equals(BltiLtiMessageTypeValue))
				return false;

			if (!BltiLaunchParameters.ContainsKey(BltiLtiVersionKey))
				return false;

			if (!BltiLaunchParameters[BltiLtiVersionKey].Equals(BltiLtiVersionValue))
				return false;

			if (!BltiLaunchParameters.ContainsKey(BltiResourceLinkIdKey))
				return false;

			if (String.IsNullOrEmpty(BltiLaunchParameters[BltiResourceLinkIdKey]))
				return false;

			return true;
		}

		/// <summary>
		/// Validates the request using BLTIHttpRequest. Checks to see if its a valid BLTI launch and checks the signature in the request if it matches with the signature generated using the BltiHttpRequest's data
		/// </summary>
		/// <param name="strSecret">Secret that will be used to generate the signature</param>
		/// <param name="strSignatureBase">out parameter that gives the base signature that is used to build the actual signature. Useful for debugging purposes</param>
		/// <param name="strValidationMessage">out parameter to return the validation message</param>
		/// <param name="strSignatureGenerated">out parameter to return the signature that was generated</param>
		/// <param name="intTimeStampThreshold">Optional parameter: Timestamp threshold (in secs) to compare the current timestamp and the timestamp sent in the request</param>
		/// <returns>True or False</returns>
		public bool ValidateRequestUsingBltiHttpRequest(string strSecret, out string strValidationMessage, out string strSignatureBase, out string strSignatureGenerated, int intTimeStampThreshold = TimeStampThreshold)
		{
			strSignatureBase = "";
			strSignatureGenerated = "";
			//If a httprequest was not assigned to the BLTI object then return with the error message
			if (BltiHttpRequest == null)
			{
				strValidationMessage = "Error: No HTTPRequest object set to the BLTI object!";
				return false;
			}
			Uri uri = BltiHttpRequest.Url;
			//Get the post parameters from the HTTP requst object
			AddPostandHeaderOauthParameters(BltiHttpRequest);
			return Validate(uri, strSecret, BltiHttpRequest.HttpMethod, intTimeStampThreshold, out strValidationMessage, out strSignatureBase, out strSignatureGenerated);
		}

		/// <summary>
		/// Validates the request using the BLTILaunchParameters. Checks to see if its a valid BLTI launch and checks the signature in the request if it matches with the signature generated using the BltiHttpRequest's data
		/// </summary>
		/// <param name="uri">URI of the BLTI launch request</param>
		/// <param name="strSecret">Secret that will be used to generate the signature</param>
		/// <param name="strSignatureBase">out parameter that gives the base signature that is used to build the actual signature. Useful for debugging purposes</param>
		/// <param name="strValidationMessage">out parameter to return the validation message</param>
		/// <param name="strSignatureGenerated">out parameter to return the signature that was generated</param>
		/// <param name="intTimeStampThreshold">Optional parameter: Timestamp threshold (in secs) to compare the current timestamp and the timestamp sent in the request</param>
		/// <returns>True or False</returns>
		public bool ValidateRequestUsingBltiLaunchParameters(Uri uri, string strSecret, string strHttpMethod, out string strValidationMessage, out string strSignatureBase, out string strSignatureGenerated, int intTimeStampThreshold = TimeStampThreshold, bool validateOnlyOAuth = false)
		{
			return Validate(uri, strSecret, strHttpMethod, intTimeStampThreshold, out strValidationMessage, out strSignatureBase, out strSignatureGenerated);
		}



		/// <summary>
		/// The core validation process. Checks to see if its a valid BLTI launch and checks the signature in the request if it matches with the signature generated using the BLTI launch data
		/// </summary>
		/// <param name="uri">URI of the BLTI launch request</param>
		/// <param name="strSecret">Secret that will be used to generate the signature</param>
		/// <param name="strSignatureBase">out parameter that gives the base signature that is used to build the actual signature. Useful for debugging purposes</param>
		/// <param name="strValidationMessage">out parameter to return the validation message</param>
		/// <param name="strSignatureGenerated">out parameter to return the signature that was generated</param>
		/// <param name="intTimeStampThreshold">Timestamp threshold (in secs) to compare the current timestamp and the timestamp sent in the request</param>
		/// <returns>True or False</returns>
		private bool Validate(Uri uri, string strSecret, string strHttpMethod, int intTimeStampThreshold, out string strValidationMessage, out string strSignatureBase, out string strSignatureGenerated)
		{
			strSignatureBase = "";
			strSignatureGenerated = "";
			//If there were no parameters in the request then return with error message
			if (BltiLaunchParameters.Count == 0)
			{
				strValidationMessage = "Error: No data to validate!";
				return false;
			}

			//Check to see if all the basic OAuth parameters are set to generate the signature.
			//This includes checking if its a basic LTI call.
			if (!BasicParametersToGenerateSignatureExist(out strValidationMessage))
				return false;

			//Check if signature was sent in the request
			if (!BltiLaunchParameters.ContainsKey(OAuthBase.OAuthSignatureKey) || String.IsNullOrEmpty(BltiLaunchParameters[OAuthBase.OAuthSignatureKey]))
			{
				strValidationMessage = "Error: No signature was provided!";
				return false;
			}
			//string signatureSentInRequest = BltiLaunchParameters[OAuthBase.OAuthSignatureKey];
			string strConsumerKey = BltiLaunchParameters[OAuthBase.OAuthConsumerKeyKey];

			//OAuth authentication
			OAuthBase oBase = new OAuthBase();
			if (oBase.ValidateSignature(BltiLaunchParameters, uri, strConsumerKey, strSecret, strHttpMethod, intTimeStampThreshold, out strSignatureBase, out strValidationMessage, out strSignatureGenerated))
				return true;

			return false;
		}
		/// <summary>
		/// Populates BltiLaunchParameters with the POST parameters in httpRequest
		/// </summary>
		/// <param name="httpRequest">The launch URL</param>
		private void AddPostandHeaderOauthParameters(HttpRequest httpRequest)
		{
			foreach (string name in httpRequest.Form)
			{
				if (BltiLaunchParameters.ContainsKey(name))
					BltiLaunchParameters[name] = httpRequest.Form[name];
				else
					BltiLaunchParameters.Add(name, httpRequest.Form[name]);
			}
			var headers = httpRequest.Headers;
			if (headers != null)
				for (var i = 0; i < headers.Count; i++)
				{
					//add only if its a oauth parameter
					if (!OAuthBase.IsOauthParameter(headers.GetKey(i))) continue;
					if (BltiLaunchParameters.ContainsKey(headers.GetKey(i)))
						BltiLaunchParameters[headers.GetKey(i)] = headers.Get(i);
					else
						BltiLaunchParameters.Add(headers.GetKey(i), headers.Get(i));
				}
		}

		/// <summary>
		/// The core siganture generation process using the OAuth library.
		/// </summary>
		/// <param name="strLaunchUrl">The launch URL</param>
		/// <param name="strSecret">Secret that will be used to generate the signature</param>
		/// <param name="strNonce">Nonce that will be used to generate the signature</param>
		/// <param name="strTimeStamp">Timestamp that will be used to generate the signature</param>
		/// <param name="strSignatureBase">out parameter that gives the base signature that is used to build the actual signature. Useful for debugging purposes</param>
		/// <param name="strValidationMessage">out parameter to return the validation message</param>
		/// <param name="strSignatureGenerated">out parameter to return the signature that was generated</param>
		/// <returns>True or False</returns>
		public bool GenerateSignature(string strLaunchUrl, string strSecret, string strNonce, string strTimeStamp, out string strSignatureBase, out string strSignatureGenerated, out string strValidationMessage)
		{
			strSignatureBase = "";
			strSignatureGenerated = "";
			try
			{
				OAuthBase oBase = new OAuthBase();
				//If a nonce and timestamp are sent, then always replace them with the new values.
				if (BltiLaunchParameters.ContainsKey(OAuthBase.OAuthNonceKey))
					BltiLaunchParameters[OAuthBase.OAuthNonceKey] = strNonce;
				else
					BltiLaunchParameters.Add(OAuthBase.OAuthNonceKey, strNonce);

				if (BltiLaunchParameters.ContainsKey(OAuthBase.OAuthTimestampKey))
					BltiLaunchParameters[OAuthBase.OAuthTimestampKey] = strTimeStamp;
				else
					BltiLaunchParameters.Add(OAuthBase.OAuthTimestampKey, strTimeStamp);
				if (!BasicParametersToGenerateSignatureExist(out strValidationMessage))
					return false;
				Uri uri = new Uri(strLaunchUrl);
				string strConsumerKey = BltiLaunchParameters[OAuthBase.OAuthConsumerKeyKey];
				strSignatureGenerated = oBase.GenerateSignature(BltiLaunchParameters, uri, strConsumerKey, strSecret, null, null, "POST", null, null, OAuthBase.SignatureTypes.HMACSHA1, out strSignatureBase);
				strValidationMessage = "OK: Signature successfully generated!";
				return true;
			}
			catch (Exception)
			{
				strValidationMessage = "Error: Unknown error!";
				return false;
			}
		}

		/// <summary>
		/// Generates signature and adds the signatue to the BltiLaunchParameters.
		/// </summary>
		/// <param name="strLaunchUrl">The launch URL</param>
		/// <param name="strSecret">Secret that will be used to generate the signature</param>
		/// <param name="strNonce">Nonce that will be used to generate the signature</param>
		/// <param name="strTimeStamp">Timestamp that will be used to generate the signature</param>
		/// <param name="strSignatureBase">out parameter that gives the base signature that is used to build the actual signature. Useful for debugging purposes</param>
		/// <param name="strValidationMessage">out parameter to return the validation message</param>
		/// <param name="strSignatureGenerated">out parameter to return the signature that was generated</param>
		/// <returns>True or False</returns>
		public bool GenerateSignatureAndAddToBltiParameters(string strLaunchUrl, string strSecret, string strNonce, string strTimeStamp, out string strSignatureBase, out string strSignatureGenerated, out string strValidationMessage)
		{
			strSignatureBase = "";
			strSignatureGenerated = "";
			try
			{
				bool blnSignatureGenerated = GenerateSignature(strLaunchUrl, strSecret, strNonce, strTimeStamp,
														 out strSignatureBase, out strSignatureGenerated, out strValidationMessage);
				if (blnSignatureGenerated)
				{
					if (BltiLaunchParameters.ContainsKey(OAuthBase.OAuthSignatureKey))
						BltiLaunchParameters[OAuthBase.OAuthSignatureKey] = strSignatureGenerated;
					else
						BltiLaunchParameters.Add(OAuthBase.OAuthSignatureKey, strSignatureGenerated);
				}
				return blnSignatureGenerated;
			}
			catch (Exception)
			{
				strValidationMessage = "Error: Unknown error!";
				return false;
			}

		}

		/// <summary>
		/// Generates signature. Also generates the nonce and timestamp to be used for signature generation.
		/// </summary>
		/// <param name="strLaunchUrl">The launch URL</param>
		/// <param name="strSecret">Secret that will be used to generate the signature</param>
		/// <param name="strSignatureBase">out parameter that gives the base signature that is used to build the actual signature. Useful for debugging purposes</param>
		/// <param name="strValidationMessage">out parameter to return the validation message</param>
		/// <param name="strSignatureGenerated">out parameter to return the signature that was generated</param>
		/// <returns>True or False</returns>
		public bool GenerateSignature(string strLaunchUrl, string strSecret, out string strSignatureBase, out string strSignatureGenerated, out string strValidationMessage)
		{
			OAuthBase oBase = new OAuthBase();
			string strNonce = oBase.GenerateNonce();
			string strTimeStamp = oBase.GenerateTimeStamp();
			return GenerateSignature(strLaunchUrl, strSecret, strNonce, strTimeStamp,
														  out strSignatureBase, out strSignatureGenerated, out strValidationMessage);
		}

		/// <summary>
		/// Generates signature and adds the signatue to the BltiLaunchParameters. Also generates the nonce and timestamp to be used for signature generation.
		/// </summary>
		/// <param name="strLaunchUrl">The launch URL</param>
		/// <param name="strSecret">Secret that will be used to generate the signature</param>
		/// <param name="strSignatureBase">out parameter that gives the base signature that is used to build the actual signature. Useful for debugging purposes</param>
		/// <param name="strValidationMessage">out parameter to return the validation message</param>
		/// <param name="strSignatureGenerated">out parameter to return the signature that was generated</param>
		/// <returns>True or False</returns>
		public bool GenerateSignatureAndAddToBltiParameters(string strLaunchUrl, string strSecret, out string strSignatureBase, out string strSignatureGenerated, out string strValidationMessage)
		{
			OAuthBase oBase = new OAuthBase();
			string strNonce = oBase.GenerateNonce();
			string strTimeStamp = oBase.GenerateTimeStamp();
			return GenerateSignatureAndAddToBltiParameters(strLaunchUrl, strSecret, strNonce, strTimeStamp,
																	 out strSignatureBase, out strSignatureGenerated, out strValidationMessage);
		}

		/// <summary>
		/// This method will check if basic lti and oauth parameters exist to generate a oauth signature.Parameters that are required : oauth_consumer_key,oauth_version,oauth_signature_method,oauth_timestamp,oauth_nonce
		/// </summary>
		/// <param name="strValidationMessage">out parameter to return the validation message</param>
		/// <returns>True or False</returns>
		private bool BasicParametersToGenerateSignatureExist(out string strValidationMessage)
		{
			//Check to see if this is a valid basic LTI request
			strValidationMessage = "";
			if (IsBasicLtiRequest())
			{
				return true;
			}
			strValidationMessage = "Error: Invalid BLTI request!";
			return false;
		}
	}
}
