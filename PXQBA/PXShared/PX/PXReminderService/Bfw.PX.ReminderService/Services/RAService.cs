using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Bfw.Common;

namespace Bfw.PX.ReminderService
{
    public class RAService : IRAService
    {
        public IEnumerable<KeyValuePair<string, string>> GetCourseList(IEnumerable<string> courses)
        {
            var result = new List<KeyValuePair<string, string>>();

            if (courses.Count() > 0)
            {
                try
                {
                    string url = String.Format("{0}{1}", ConfigurationManager.AppSettings.Get("GetCourseList"), string.Join(",", courses.ToArray()));
                    string response = SendWebRequest(url, "GET", "GetGenericCourseList", null);

                    XDocument document = XDocument.Parse(response);

                    if (document.Descendants("Courses") != null && document.Descendants("Courses").Descendants("Course") != null)
                    {
                        foreach (var course in document.Descendants("Courses").Descendants("Course"))
                        { 
                            result.Add(new KeyValuePair<string,string>(course.Element("AgilixID").Value, course.Element("BaseUrl").Value));
                        }
                    }
                }
                catch (Exception ex)
                {
                    
                }
            }

            return result;
        }

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
