using System;
using System.IO;
using System.Net;
using System.Text;
using BLTI;

namespace Bfw.BLTIProvider
{
	public partial class BltiLaunch : System.Web.UI.Page
	{
		protected const string OAuthConsumerKeyKey = "oauth_consumer_key";
		protected const string OAuthCallbackKey = "oauth_callback";
		protected const string OAuthVersionKey = "oauth_version";
		protected const string OAuthSignatureMethodKey = "oauth_signature_method";
		protected const string OAuthSignatureKey = "oauth_signature";
		protected const string OAuthTimestampKey = "oauth_timestamp";
		protected const string OAuthNonceKey = "oauth_nonce";
		protected const string OAuthTokenKey = "oauth_token";
		protected const string OAuthTokenSecretKey = "oauth_token_secret";
		protected const string BltiMessageTypeKey = "lti_message_type";
		protected const string BltiVersionKey = "lti_version";
		protected const string ContextIdKey = "context_id";
		protected const string ContextTitleKey = "context_title";
		protected const string ContextLabelKey = "context_label";
		protected const string ResourceLinkIdKey = "resource_link_id";
		protected const string ResourceLinkTitleKey = "resource_link_title";
		protected const string ResourceLinkDescriptionKey = "resource_link_description";
		protected const string UserIdKey = "user_id";
		protected const string RolesKey = "roles";
		protected const string LisPersonNameFullKey = "lis_person_name_full";
		protected const string LisPersonContactEmailPrimaryKey = "lis_person_contact_email_primary";
		protected const string LisPersonSourceIdKey = "lis_person_sourcedid";
		protected const string LaunchPresentationReturnUrlKey = "launch_presentation_return_url";
		protected const string ToolConsumerInstanceGuidKey = "tool_consumer_instance_guid";
		protected const string ToolConsumerInstanceNameKey = "tool_consumer_instance_name";
		protected const string ToolConsumerInstanceContactEmailKey = "tool_consumer_instance_contact_email";
		protected const string ToolConsumerInstanceDescriptionKey = "tool_consumer_instance_description";
		protected const string BookIdKey = "bookId";
		protected const string BaseUrlKey = "baseURL";
		protected const string UidKey = "uid";

		protected const string PxUserIdKey = "ext_macmillan_pxuid";
		protected const string RaUserIdKey = "ext_macmillan_rauid";


		protected void Page_Load(object sender, EventArgs e)
		{

		}


		protected void Button1_Click(object sender, EventArgs e)
		{
			BLTI.BLTI bObj = new BLTI.BLTI { };
			var postdata = new StringBuilder(); //replace <value>

			//Mandatory Params

			bObj.BltiLaunchParameters.Add(RaUserIdKey, txtRaUserId.Text.Trim());
			bObj.BltiLaunchParameters.Add(PxUserIdKey, txtPxUserId.Text.Trim());

			postdata.Append("&ext_macmillan_rauid=" + txtRaUserId.Text.Trim());
			postdata.Append("&ext_macmillan_pxuid=" + txtPxUserId.Text.Trim());

			bObj.BltiLaunchParameters.Add(OAuthConsumerKeyKey, txtKey.Text.Trim());
			bObj.BltiLaunchParameters.Add(OAuthVersionKey, "1.0");


			if (chkOAuthCallback.Checked)
			{
				bObj.BltiLaunchParameters.Add(OAuthCallbackKey, "about:blank");
				postdata.Append("&" + OAuthCallbackKey + "=" + "about:blank");
			}

			bObj.BltiLaunchParameters.Add(OAuthSignatureMethodKey, "HMAC-SHA1");
			postdata.Append("&" + OAuthSignatureMethodKey + "=" + "HMAC-SHA1");

			if (chkLTIMessage.Checked)
			{
				bObj.BltiLaunchParameters.Add(BltiMessageTypeKey, "basic-lti-launch-request");
				postdata.Append("&" + BltiMessageTypeKey + "=basic-lti-launch-request");
			}
			if (chkLTIVersion.Checked)
			{
				bObj.BltiLaunchParameters.Add(BltiVersionKey, "LTI-1p0");
				postdata.Append("&" + BltiVersionKey + "=LTI-1p0");
			}
			if (chkResourceLinkId.Checked)
			{
				bObj.BltiLaunchParameters.Add(ResourceLinkIdKey, txtResourceLinkId.Text.Trim());
				postdata.Append("&" + ResourceLinkIdKey + "=" + txtResourceLinkId.Text.Trim());
			}
			if (chkContextId.Checked)
			{
				bObj.BltiLaunchParameters.Add(ContextIdKey, txtContextId.Text.Trim());
				postdata.Append("&" + ContextIdKey + "=" + txtContextId.Text.Trim());
			}
			if (chkContextTitle.Checked)
			{
				bObj.BltiLaunchParameters.Add(ContextTitleKey, txtContextTitle.Text.Trim());
				postdata.Append("&" + ContextTitleKey + "=" + txtContextTitle.Text.Trim());
			}
			if (chkContextLabel.Checked)
			{
				bObj.BltiLaunchParameters.Add(ContextLabelKey, txtContextLabel.Text.Trim());
				postdata.Append("&" + ContextLabelKey + "=" + txtContextLabel.Text.Trim());
			}
			if (chkToolConsumerGUID.Checked)
			{
				bObj.BltiLaunchParameters.Add(ToolConsumerInstanceGuidKey, txtToolConsumerGUID.Text.Trim());
				postdata.Append("&" + ToolConsumerInstanceGuidKey + "=" + txtToolConsumerGUID.Text.Trim());
			}

			//Opional - If the use selected the checkbox
			if (chkResournceLinkTitle.Checked)
			{
				bObj.BltiLaunchParameters.Add(ResourceLinkTitleKey, txtResourceLinkTitle.Text.Trim());
				postdata.Append("&" + ResourceLinkTitleKey + "=" + txtResourceLinkTitle.Text.Trim());
			}
			if (chkResourceLinkDescription.Checked)
			{
				bObj.BltiLaunchParameters.Add(ResourceLinkDescriptionKey, txtResourceLinkDescription.Text.Trim());
				postdata.Append("&" + ResourceLinkDescriptionKey + "=" + txtResourceLinkDescription.Text.Trim());
			}
			if (chkUserId.Checked)
			{
				bObj.BltiLaunchParameters.Add(UserIdKey, txtUserId.Text.Trim());
				postdata.Append("&" + UserIdKey + "=" + txtUserId.Text.Trim());
			}
			if (chkRoles.Checked)
			{
				bObj.BltiLaunchParameters.Add(RolesKey, txtRoles.Text.Trim());
				postdata.Append("&" + RolesKey + "=" + txtRoles.Text.Trim());
			}
			if (chkLisPersonNameFull.Checked)
			{
				bObj.BltiLaunchParameters.Add(LisPersonNameFullKey, txtLisPersonNameFull.Text.Trim());
				postdata.Append("&" + LisPersonNameFullKey + "=" + txtLisPersonNameFull.Text.Trim());
			}
			if (chkLisPersonEmailPrimary.Checked)
			{
				bObj.BltiLaunchParameters.Add(LisPersonContactEmailPrimaryKey, txtLisPersonEmailPrimary.Text.Trim());
				postdata.Append("&" + LisPersonContactEmailPrimaryKey + "=" + txtLisPersonEmailPrimary.Text.Trim());
			}
			if (chkLisPersonSourceId.Checked)
			{
				bObj.BltiLaunchParameters.Add(LisPersonSourceIdKey, txtLisPersonSourceId.Text.Trim());
				postdata.Append("&" + LisPersonSourceIdKey + "=" + txtLisPersonSourceId.Text.Trim());
			}
			if (chkLaunchPresentReturnURL.Checked)
			{
				bObj.BltiLaunchParameters.Add(LaunchPresentationReturnUrlKey, txtLaunchPresentReturnURL.Text.Trim());
				postdata.Append("&" + LaunchPresentationReturnUrlKey + "=" + txtLaunchPresentReturnURL.Text.Trim());
			}
			if (chkToolConsumerName.Checked)
			{
				bObj.BltiLaunchParameters.Add(ToolConsumerInstanceNameKey, txtToolConsumerName.Text.Trim());
				postdata.Append("&" + ToolConsumerInstanceNameKey + "=" + txtToolConsumerName.Text.Trim());
			}
			if (chkToolConsumerContactEmail.Checked)
			{
				bObj.BltiLaunchParameters.Add(ToolConsumerInstanceContactEmailKey,
											  txtToolConsumerContactEmail.Text.Trim());
				postdata.Append("&" + ToolConsumerInstanceContactEmailKey + "=" +
								txtToolConsumerContactEmail.Text.Trim());
			}
			if (chkToolConsumerDescription.Checked)
			{
				bObj.BltiLaunchParameters.Add(ToolConsumerInstanceDescriptionKey, txtToolConsumerDescription.Text.Trim());
				postdata.Append("&" + ToolConsumerInstanceDescriptionKey + "=" + txtToolConsumerDescription.Text.Trim());
			}
			if (chkBookId.Checked)
			{
				bObj.BltiLaunchParameters.Add(BookIdKey, txtBookId.Text.Trim());
				postdata.Append("&" + BookIdKey + "=" + txtBookId.Text.Trim());
			}
			if (chkBaseURL.Checked)
			{
				bObj.BltiLaunchParameters.Add(BaseUrlKey, txtBaseURL.Text.Trim());
				postdata.Append("&" + BaseUrlKey + "=" + txtBaseURL.Text.Trim());
			}
			if (chkUID.Checked)
			{
				bObj.BltiLaunchParameters.Add(UidKey, txtUID.Text.Trim());
				postdata.Append("&" + UidKey + "=" + txtUID.Text.Trim());
			}
			string strBaseSignature = "";
			string strValidationMessage = "";
			string strSignature = "";
			OAuthBase ob = new OAuthBase();

			var strTimeStamp = ob.GenerateTimeStamp();

			var strNonce = ob.GenerateNonce();

			var blnSignatureGenerated = bObj.GenerateSignature(txtLaunchURL.Text.Trim(), txtSecret.Text.Trim(),
																strNonce, strTimeStamp, out strBaseSignature,
																out strSignature, out strValidationMessage);

			var url = new Uri(txtLaunchURL.Text.Trim());

			string strBaseSignature1 = "";
			string strValidationMessage1 = "";
			string strSignature1 = "";

			if (!bObj.BltiLaunchParameters.ContainsKey(OAuthConsumerKeyKey)) bObj.BltiLaunchParameters.Add(OAuthConsumerKeyKey, txtKey.Text.Trim());
			if (!bObj.BltiLaunchParameters.ContainsKey(OAuthVersionKey)) bObj.BltiLaunchParameters.Add(OAuthVersionKey, "1.0");
			if (!bObj.BltiLaunchParameters.ContainsKey(OAuthSignatureMethodKey)) bObj.BltiLaunchParameters.Add(OAuthSignatureMethodKey, "HMAC-SHA1");
			if (!bObj.BltiLaunchParameters.ContainsKey(OAuthSignatureKey)) bObj.BltiLaunchParameters.Add(OAuthSignatureKey, strSignature.Trim());
			if (!bObj.BltiLaunchParameters.ContainsKey(OAuthTimestampKey)) bObj.BltiLaunchParameters.Add(OAuthTimestampKey, strTimeStamp);
			if (!bObj.BltiLaunchParameters.ContainsKey(OAuthNonceKey)) bObj.BltiLaunchParameters.Add(OAuthNonceKey, strNonce);

			var isValid = bObj.ValidateRequestUsingBltiLaunchParameters(url, txtSecret.Text.Trim(), "POST",
																		out strValidationMessage1, out strBaseSignature1,
																		out strSignature1);


			lblBaseSiganture.Text = strBaseSignature;
			lblSignatureGenerated.Text = strSignature;

			if (blnSignatureGenerated)
			{
				//if signature was generated then assign the blti/oauth params and redirect to the launch URL
				//lblBaseSiganture.Text = "Base Signature: " + strBaseSignature + "<br/> Nonce:" +
				//                        bObj.BltiLaunchParameters[OAuthBase.OAuthNonceKey] + "<br/> TimeStamp: " +
				//                        bObj.BltiLaunchParameters[OAuthBase.OAuthTimestampKey];
				//lblSignatureGenerated.Text = "Signature Generated on our end: " + strSignature;

				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(txtLaunchURL.Text.Trim());
				req.Method = "POST";
				req.ContentType = "application/x-www-form-urlencoded";
				//req.Headers.Add(OAuthConsumerKeyKey,txtKey.Text.Trim());
				postdata.Append("&" + OAuthConsumerKeyKey + "=" + txtKey.Text.Trim());
				// req.Headers.Add(OAuthCallbackKey, "about:blank");
				// req.Headers.Add(OAuthVersionKey, "1.0");
				postdata.Append("&" + OAuthVersionKey + "=" + "1.0");
				// req.Headers.Add(OAuthSignatureMethodKey, "HMAC-SHA1");
				postdata.Append("&" + OAuthSignatureMethodKey + "=" + "HMAC-SHA1");
				// req.Headers.Add(OAuthSignatureKey, strSignature.Trim());
				postdata.Append("&" + OAuthSignatureKey + "=" + strSignature.Trim());
				// req.Headers.Add(OAuthTimestampKey, strTimeStamp);
				postdata.Append("&" + OAuthTimestampKey + "=" + strTimeStamp);
				//  req.Headers.Add(OAuthNonceKey, strNonce);
				postdata.Append("&" + OAuthNonceKey + "=" + strNonce);
				var strPostData = postdata.ToString().Trim('&');

				byte[] postData = Encoding.UTF8.GetBytes(strPostData);
				if (req.Method == "POST")
				{
					if (postData != null)
					{
						req.ContentLength = postData.Length;
						using (var dataStream = req.GetRequestStream())
						{
							dataStream.Write(postData, 0, postData.Length);
						}
					}
				}
				HttpWebResponse loWebResponse = (HttpWebResponse)req.GetResponse();
				Encoding enc = Encoding.GetEncoding(1252); // Windows default Code Page
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