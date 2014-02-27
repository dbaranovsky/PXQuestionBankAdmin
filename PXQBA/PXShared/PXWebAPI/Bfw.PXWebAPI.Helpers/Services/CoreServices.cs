using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using Bfw.Common;
using Bfw.PXWebAPI.Models.CoreServices;

namespace Bfw.PXWebAPI.Helpers.Services
{
    public class CoreServices
    {
        /// <summary>
        /// Gets the packages for a RA user
        /// </summary>
        /// <param name="raUserId">RA user id</param>
        /// <param name="raEmail">RA email</param>
        /// <returns>List of PackageSiteInfo Objects</returns>
        public PackageSiteInfoListResponse GetUserPacakges(string raUserId, string raEmail)
        {
            string[] requestSettings = GetRequestSettings("GetUserPackages");
            string settingsUrl = requestSettings[0];
            string settingsMethod = requestSettings[1];
            string baseAddress = settingsUrl + "UserId=" + raUserId + "&" + "UserEmail=" + raEmail;
            string responseOb = SendWebRequest(baseAddress, settingsMethod, "Get", null);
            return DeserializeResponse<PackageSiteInfoListResponse>(responseOb);
        }

        /// <summary>
        /// Checks and creates Agilix domain for an institution
        /// </summary>
        /// <param name="institutionId">Institution id</param>
        /// <param name="domainName">Domain name</param>
        /// <returns>Institution Object</returns>
        public InstitutionInfoResponse CheckAndCreateAgilixDomain(string institutionId, string domainName)
        {
            string[] requestSettings = GetRequestSettings("CheckAndCreateAgilixDomain");
            string settingsUrl = requestSettings[0];
            string settingsMethod = requestSettings[1];
            string baseAddress = settingsUrl + "InstitutionId=" + institutionId + "&" + "domainName=" + domainName;
            string responseOb = SendWebRequest(baseAddress, settingsMethod, "Post", null);
            return DeserializeResponse<InstitutionInfoResponse>(responseOb);
        }

        /// <summary>
        /// Creates a new RA user for integration testing purposes
        /// </summary>
        /// <returns>Registered User object</returns>
        public RegisterUserInfoResponse RegisterUser(string email, string password, string passwordHint, string firstName, string lastName)
        {
            string[] requestSettings = GetRequestSettings("RegisterUser");
            string settingsUrl = requestSettings[0];
            string settingsMethod = requestSettings[1];
            string baseAddress = settingsUrl + 
                                 String.Format("email={0}&password={1}&passwordHint={2}&firstName={3}&lastName={4}&mailPreferences=HTML&optInEmail=0&remoteIPAddr=0&instructorEmail=&baseUrl=&completedEula=true",
                                 email, password, passwordHint, firstName, lastName);
            string response = SendWebRequest(baseAddress, settingsMethod, "Post", null);
            return DeserializeResponse<RegisterUserInfoResponse>(response);
        }
        
        /// <summary>
        /// For Access Code redemption
        /// </summary>
        public CheckUserAssignmentResponse CheckUserAssignment(string raUserId, string accessId)
        {
            string[] requestSettings = GetRequestSettings("CheckUserAssignment");
            string settingsUrl = requestSettings[0];
            string settingsMethod = requestSettings[1];
            string baseAddress = String.Format("{0}UserId={1}&AccessID={2}&PackageID=6476", settingsUrl, raUserId, accessId);
            string response = SendWebRequest(baseAddress, settingsMethod, "Get", null);
            return DeserializeResponse<CheckUserAssignmentResponse>(response);
        }

        private string GetToken(string remoteMethod)
        {
            string vi = ConfigurationManager.AppSettings["iv"];
            string key = ConfigurationManager.AppSettings["key"];
            ResourceAccessAES aes = new ResourceAccessAES(Encoding.ASCII.GetBytes(key), Encoding.ASCII.GetBytes(vi),CipherMode.ECB,PaddingMode.Zeros);

            DateTime dt = DateTime.Now.ToUniversalTime();
            string token = aes.EncryptToString(string.Format("{0}|001|{1}", remoteMethod, dt));
            return token;
        }
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

            request.Headers.Add("X-AppId", "7");
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

        private T DeserializeResponse<T>(string response) where T : class, IError, new()
        {
            T responseObject = null;
            var xmlSerializer = new XmlSerializer(typeof(T));
            try
            {
                responseObject = xmlSerializer.Deserialize(new StringReader(response)) as T;
            }
            catch (Exception ex)
            {
                responseObject = new T();
                responseObject.Error = new Error();
                responseObject.Error.Code = "-1";
                responseObject.Error.Message = ex.Message;
            }
            return responseObject;
        }

        private string[] GetRequestSettings(string serviceName)
        {
            string settings = ConfigurationManager.AppSettings[serviceName];
            return settings.Split('|');
        }
    }
}
