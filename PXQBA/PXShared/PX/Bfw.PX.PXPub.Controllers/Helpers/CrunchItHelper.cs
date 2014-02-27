using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Security.Cryptography;
using System.Web;

namespace Bfw.PX.PXPub.Controllers.Helpers
{
    public class CrunchItHelper
    {
        public string Base64ForUrlEncode(string value)
        {
            byte[] encbuff = Encoding.UTF8.GetBytes(value);
            return HttpServerUtility.UrlTokenEncode(encbuff);
        }

        public long GetExpirationToken(long minutes)
        {
            DateTime dateInFuture = DateTime.Now.AddMinutes(minutes);
            TimeSpan span = (dateInFuture - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());

            return (long)span.TotalSeconds;
        }

        public string SignWithHMACSHA256(string value)
        {
            var key = ConfigurationManager.AppSettings.Get("CrunchIt3PrivateKey");
            byte[] keyBytes = Convert.FromBase64String(key);

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] valueBytes = encoding.GetBytes(value);

            HMACSHA256 hmacsha256 = new HMACSHA256(keyBytes);

            byte[] hashValue = hmacsha256.ComputeHash(valueBytes);

            return Convert.ToBase64String(hashValue).Replace("+", "-").Replace("/", "_"); ;
        }

        private static string ByteToString(byte[] buff)
        {
            return BitConverter.ToString(buff).Replace("-", "").ToLower();

            string sbinary = string.Empty;

            for (int i = 0; i < buff.Length; i++)
            {
                sbinary += buff[i].ToString("X2"); // hex format
            }

            return (sbinary);
        }
    }
}
