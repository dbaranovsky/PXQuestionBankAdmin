using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;
using System.Configuration;
using System.Xml.Serialization;
using Bfw.Common;
using Bfw.PX.Biz.DataContracts;
using Bfw.Common.Collections;
using Bfw.PX.Biz.ServiceContracts;
using BizDC = Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.Direct.Services
{
	public class RAServices: IRAServices
	{
		#region RA ServiceMethods

		/// <summary>
		/// Gets the RA Users from a list of DLAP users
		/// </summary>
		/// <param name="dlapUsersList">List of UserInfo objects</param>
		/// <returns>List of UserInfo Objects</returns>
        public UserProfileResponse GetUserProfile(IEnumerable<UserInfo> dlapUsersList)
        {
            String[] ids = dlapUsersList.Map(user => user.Username).ToArray();
            return GetUserProfile(ids);
        }

        /// <summary>
        /// Gets the RA Users from a list of user reference ids
        /// </summary>
        /// <param name="user_reference_ids">String Array of user reference ids</param>
        /// <returns>List of UserInfo Objects</returns>
        public UserProfileResponse GetUserProfile(String[] user_reference_ids)
		{
            string dlapUsers = String.Join(",", user_reference_ids);
            string settings = ConfigurationManager.AppSettings["GetUserProfile"];
			string[] requestSettings = settings.Split('|');
			string settingsUrl = requestSettings[0];
			string settingsMethod = requestSettings[1];
			string baseAddress = string.Format("{0}{1}", settingsUrl, dlapUsers);
            string response = SendWebRequest(baseAddress, settingsMethod, "GetProfUser", null);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserProfileResponse));
            try
            {
                UserProfileResponse responseObj =
                    (UserProfileResponse)xmlSerializer.Deserialize(new StringReader(response));
                return responseObj;
            }
            catch (Exception ex)
            {
                UserProfileResponse responseObj = new UserProfileResponse();
                responseObj.Error = new Error();  
                responseObj.Error.Code = "-1";
                responseObj.Error.Message = ex.Message;
				return responseObj;
            }
		}

        public UserAuthResponse AuthenticateUser(string user, string password)
        {
            string url = ConfigurationManager.AppSettings["AuthenticateUser"];
            url = string.Format("{0}?userEmail={1}&password={2}", url, user, password);

            string response = SendWebRequest(url, "GET", "AuthenticateUser", null);
            UserAuthResponse responseObj = new UserAuthResponse();
            

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserAuthResponse));
            try
            {
                responseObj = (UserAuthResponse)xmlSerializer.Deserialize(new StringReader(response));
            }
            catch (Exception ex)
            {
                responseObj.Error = new Error();
                responseObj.Error.Code = "-1";
                responseObj.Error.Message = ex.Message;
            }

            return responseObj;
        }

        public RAAccessInfo GetAccessLevelByBaseUrl(string raUserId, string baseUrl)
        {
            RAAccessInfo responseObj = new RAAccessInfo
            {
                Error = new Error
                {
                    Code = "-1",
                    Message = "An Error Occurred"
                }
            };

            string url = ConfigurationManager.AppSettings["GetAccessLevelByBaseUrl"];
            url = string.Format("{0}?RAId={1}&baseUrl={2}", url, raUserId, baseUrl.Replace("http://", "").Replace("https://", ""));

            string response = SendWebRequest(url, "GET", "GetAccessLevelByBaseUrl", null);

            var serializer = new XmlSerializer(typeof(RAAccessInfo));
            try
            {
                responseObj = (RAAccessInfo)serializer.Deserialize(new StringReader(response));
            }
            catch (Exception ex)
            {
                responseObj.Error.Code = "-1";
                responseObj.Error.Message = ex.Message;
            }

            return responseObj;
        }

        public RASiteInfo GetSiteListByBaseUrl(string baseUrl)
        {
            RASiteInfo responseObj = new RASiteInfo
            {
                Error = new Error
                {
                    Code = "-1",
                    Message = "An Error Occurred"
                }
            };

            baseUrl = baseUrl.TrimEnd('/');

            string url = ConfigurationManager.AppSettings["GetSiteListByBaseUrl"];
            url = string.Format("{0}?baseUrl={1}", url, baseUrl.Replace("http://", "").Replace("https://", ""));

            string response = SendWebRequest(url, "GET", "GetSiteListByBaseUrl", null);

            var serializer = new XmlSerializer(typeof(RASiteInfo));
            try
            {
                responseObj = (RASiteInfo)serializer.Deserialize(new StringReader(response));
            }
            catch (Exception ex)
            {
                responseObj.Error.Code = "-1";
                responseObj.Error.Message = ex.Message;
            }

            return responseObj;
        }
        
		/// <summary>
		/// Sets a user profile image for a specific user and returns the RAWebServiceResponse object
		/// </summary>
		/// <param name="userRefId"> </param>
		/// <param name="fileType">File Extention</param>
		/// <param name="file">Posted file Stream object</param>
		/// <returns>RAWebServiceResponse Object</returns>
		public UserProfileResponse SetUserProfileImage(string userRefId, string fileType, Stream file)
		{
            string settings = ConfigurationManager.AppSettings["SetUserProfileImage"];
			string[] requestSettings = settings.Split('|');
			string settingsUrl = requestSettings[0];
			string settingsMethod = requestSettings[1];
            string baseAddress = string.Format("{0}userId={1}&imageObjectType={2}&requestType=SET", settingsUrl, userRefId, fileType);
            string response = SendWebRequest(baseAddress, settingsMethod, "SetProfUserImage", GetBytes(file));
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserProfileResponse));
            try
            {
                UserProfileResponse responseObj =
                    (UserProfileResponse) xmlSerializer.Deserialize(new StringReader(response));
                return responseObj;
            } catch (Exception ex)
            {
                UserProfileResponse responseObj = new UserProfileResponse();
                responseObj.Error.Code = "99999";
                responseObj.Error.Message = ex.Message;
                return responseObj;
            }
		}

		/// <summary>
		/// Sets or updates a users email, first and last names by user ID
		/// </summary>
		/// <param name="userId">User ID</param>
		/// <param name="userRefId"> </param>
		/// <param name="email">E-Mail</param>
		/// <param name="firstName">First Name</param>
		/// <param name="lastName">Last Name</param>
		/// <returns>RAWebServiceResponse Object</returns>
		public UserProfileResponse SetUserProfileInfo(string userId, string userRefId, string email, string firstName, string lastName)
		{
			string settings = ConfigurationManager.AppSettings["SetUserEmailandName"];
			string[] requestSettings = settings.Split('|');
			string settingsUrl = requestSettings[0];
			string settingsMethod = requestSettings[1];
			string baseAddress = string.Format("{0}userId={1}&email={2}&firstName={3}&lastName={4}", settingsUrl, userRefId, email, firstName, lastName);
            string response = SendWebRequest(baseAddress, settingsMethod, "SetProfUserEmailandName", null);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserProfileResponse));
            try
            {
                UserProfileResponse responseObj =
                    (UserProfileResponse)xmlSerializer.Deserialize(new StringReader(response));
                return responseObj;
            }
            catch (Exception ex)
            {
                UserProfileResponse responseObj = new UserProfileResponse();
                //UserInfo ui = new UserInfo();
                responseObj.Error = new Error();
                responseObj.Error.Code = "-1";
                responseObj.Error.Message = ex.Message;
                return responseObj;
            }
		}
		#endregion

		#region Internal Helpers

		/// <summary>
		/// Generically sends requests to RA REST services
		/// </summary>
		/// <param name="baseAddress">Url with parameters</param>
		/// <param name="requestMethod">They type of request being made GET or POST</param>
		/// <param name="remoteMethod">The name of the remote method call</param>
		/// <param name="bytearray">An array of bytes if there is binary content to transmit or null</param>
		/// <returns>XML string</returns>
		private string SendWebRequest(string baseAddress, string requestMethod, string remoteMethod, byte[] bytearray)
		{
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(baseAddress);
		    request.KeepAlive = false;
		    request.Method = requestMethod;
			request.ContentType = "text/plain";

			request.Headers.Add("X-AppId", "1");
			request.Headers.Add("X-ATK", GetToken(remoteMethod));

			if (bytearray != null)
			{
				Stream serverStream = request.GetRequestStream();
				serverStream.Write(bytearray, 0, bytearray.Length);
				serverStream.Close();
			}
			else
			{
                request.ContentLength = request.ContentLength == -1 ? 0 : request.ContentLength;
			}
			StringBuilder msg = new StringBuilder();
			try
			{
				using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
				{
					int statusCode = (int)response.StatusCode;
					StreamReader reader = new StreamReader(response.GetResponseStream());
					Char[] read = new Char[256];
					int count = reader.Read(read, 0, 256);

					while (count > 0)
					{
						msg.Append(new string(read, 0, count));
						count = reader.Read(read, 0, 256);
					}

				}
				return msg.ToString();
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}

		private string GetToken(string remoteMethod)
		{
			string vi = ConfigurationManager.AppSettings["iv"];
			string key = ConfigurationManager.AppSettings["key"];
			ResourceAccessAES aes = new ResourceAccessAES(Encoding.ASCII.GetBytes(key), Encoding.ASCII.GetBytes(vi));
			DateTime dt = DateTime.Now.ToUniversalTime();
			string token = aes.EncryptToString(string.Format("{0}|001|{1}", remoteMethod, dt));
			return token;
		}

		private byte[] GetBytes(Stream file)
		{
			byte[] bytesOut = new byte[file.Length]; //new byte[16 * 1024]; // 

			int count = 0;
			while (count < file.Length)
			{
				bytesOut[count++] = Convert.ToByte(file.ReadByte());
			}
			return bytesOut;
		}
		#endregion
	}
}
