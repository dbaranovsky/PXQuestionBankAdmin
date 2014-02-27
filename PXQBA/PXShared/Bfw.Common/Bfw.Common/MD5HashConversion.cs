using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Common
{
    /// <summary>
    /// Provides helper functions for MD5 hash conversion
    /// </summary>
    public class MD5HashConversion
    {
        /// <summary>
        /// Computes the MD5 Hash given an input string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ConvertToMD5Hash(string input)
        {
            //compute MD5 hash from input
            var md5 = System.Security.Cryptography.MD5.Create();
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);

            //convert the byte array into Hexadecimal string
            var sb = new StringBuilder();
            foreach (var hashByte in hash)
            {
                sb.Append(hashByte.ToString("X2"));
            }

            return sb.ToString();
        }

    }
}
