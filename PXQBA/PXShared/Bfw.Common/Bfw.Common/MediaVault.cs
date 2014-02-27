using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Configuration;

namespace Bfw.Common
{
    /// <summary>
    /// This class has helper funtions to add media valut hash code to the url
    /// </summary>
   public class MediaVault
    {
        /// <summary>
        /// appends end time and media vault hash url in the format url?s=endTime&h=hashCode
        /// </summary>
        /// <param name="url"></param>
        /// <returns>url which has appended end time and media vault hash url in the format url?s=endTime&h=hashCode</returns>
        public static string GetMediaValutUrlWithHash(string url)
        {
            string limelightSharedSecret = ConfigurationManager.AppSettings["LimelightSharedSecret"];
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(limelightSharedSecret))
            {
                url = string.Empty;
                return url;
            }

            int lifetime = MediaVaultLifetime();
            url = AddParamterToUrl(url, "e", lifetime.ToString());

            string source = limelightSharedSecret + url;
            MD5 md5Hash = MD5.Create();
            string hash = GetMd5Hash(md5Hash, source);

            url = AddParamterToUrl(url, "h", hash);

            return url;
        }

        private static string AddParamterToUrl(string url, string paramName, string paramValue)
        {
            if (string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }

            if (url.Contains("?") && url.Contains("=")) // case where ur is "abc.com?a=34" or "abc.com?a=34&b=35"
            {
                url = string.Format("{0}&{1}={2}", url, paramName, paramValue);
            }
            else if (url.Contains("?")) // case where ur is "abc.com?"
            {
                url = string.Format("{0}{1}={2}", url, paramName, paramValue);
            }
            else
            {
                url = string.Format("{0}?{1}={2}", url, paramName, paramValue); // case where ur is "abc.com"
            }

            return url;
        }

       private static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        /// <summary>
        /// get life time from config file appsetting "MediaVaultLifeTime" and adds Seconds since the Unix epoch to it
        /// </summary>
        /// <returns></returns>
       private static int MediaVaultLifetime()
        {
            string seconds = ConfigurationManager.AppSettings["MediaVaultLifeTime"];
            int time = 0;
            if (!int.TryParse(seconds, out time))
            {
                time = 86400;
            }

            time = time + SecondsSinceUnixEpoch();
            return time;
        }

        /// <summary>
        /// Gets Seconds since the Unix epoch
        /// </summary>
        /// <returns></returns>
       private static int SecondsSinceUnixEpoch()
        {

            int seconds = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

            //TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1).ToUniversalTime());
            //int seconds = (int)t.TotalSeconds;

            return seconds;
        }
    }
}
