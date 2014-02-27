// -----------------------------------------------------------------------
// <copyright file="ILoginServices.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Bfw.PX.Biz.Direct.Services
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Xml.Serialization;
    using Bfw.PX.Biz.DataContracts;

    /// <summary>
    /// ILogin Service method(s) invoked and response results are returned from here
    /// </summary>
    public class ILoginServices
    {
        /// <summary>
        /// Searches school based on city, region code
        /// </summary>
        /// <param name="instituteType">Onyx Institution Type (0=Both; 1=Colleges; 2=High Schools)</param>
        /// <param name="countryCode">Onyx Country Code (either ‘US’ or ‘CA’ for Canada)</param>
        /// <param name="regionCode">Onyx Region Code (for State or Province)</param>
        /// <param name="city">City (case-insensitive “contains” search, i.e., WHERE [DatabaseCity] LIKE %InputCity%)</param>
        /// <returns>List of school</returns>
        public SchoolRespose OnyxSchoolCitySearch(string instituteType, string countryCode, string regionCode, string city)
        {
            string settings = ConfigurationManager.AppSettings["OnyxSchoolCitySearch"];
            string[] requestSettings = settings.Split('|');
            string settingsUrl = requestSettings[0];
            string settingsMethod = requestSettings[1];
            string baseAddress = string.Format("{0}InstitutionType={1}&CountryCode={2}&RegionCode={3}&City={4}", settingsUrl, instituteType, countryCode, regionCode, city);
            string response = this.SendWebRequest(baseAddress, settingsMethod, "OnyxSchoolCitySearch", null);
            return CreateSchoolResponse(ref response);
        }

        /// <summary>
        /// Searches schools based on radius
        /// </summary>
        /// <param name="miles">specify 0 for an Exact Search, or specify 5, 10, or 25 for a Radius Search</param>
        /// <param name="postalCode">specify either a 5-digit US zip code (i.e., ‘07076’) or a 6- alphanumeric Canadian postal code without spaces in the form A#A#A# (i.e., ‘H1Y2G7’)</param>
        /// <param name="instituteType">Onyx Institution Type – specify 0=Both; 1=Colleges; 2=High Schools</param>
        /// <param name="includeDistance">specify 0 or 1, where 0=sort by school name; or 1=sort by distance from radius.  If 1 is specified, the distance from the radius is also included in the company name node.  This parameter is ignored if Miles is set to 0</param>
        /// <returns>List of school</returns>
        public SchoolRespose OnyxSchoolRadiusSearch(string miles, string postalCode, string instituteType, string includeDistance)
        {
            string settings = ConfigurationManager.AppSettings["OnyxSchoolRadiusSearch"];
            string[] requestSettings = settings.Split('|');
            string settingsUrl = requestSettings[0];
            string settingsMethod = requestSettings[1];
            string baseAddress = string.Format("{0}PostalCode={1}&Miles={2}&InstitutionType={3}&IncludeDistance={4}", settingsUrl, postalCode, miles, instituteType, includeDistance);
            string response = this.SendWebRequest(baseAddress, settingsMethod, "OnyxSchoolRadiusSearch", null);
            return CreateSchoolResponse(ref response);
        }

        /// <summary>
        /// Method is used to prepare School Response from XML string
        /// </summary>
        /// <param name="response">Response string received from Onyx Web service</param>
        /// <returns>School Response</returns>
        private static SchoolRespose CreateSchoolResponse(ref string response)
        {
            response = response.Replace("&lt;", "<").Replace("&gt;", ">");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SchoolRespose));
            try
            {
                SchoolRespose responseObj =
                    (SchoolRespose)xmlSerializer.Deserialize(new StringReader(response));
                return responseObj;
            }
            catch (Exception ex)
            {
                SchoolRespose responseObj = new SchoolRespose();
                responseObj.Error = ex.Message;
                return responseObj;
            }
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
                    char[] read = new char[256];
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
    }
}