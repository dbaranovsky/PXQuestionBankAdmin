using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using IntelliClass.Utils;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;


namespace BFW
{
    namespace Encryption
    {
        #region BLOCKTEA
        public static class BlockTea
        {
            
            public static char[] ControlCharacters = { '\0', '\t', '\n', '\v', '\f', '\r', '\xa0', '\'', '\"', '!' };

            /// <summary>
            /// encrypts a string using Block Tea
            /// </summary>
            /// <param name="input">plain text string to encrypt</param>
            /// <param name="password">encryption password</param>
            /// <returns>encrypted text</returns>
            public static string Encrypt(string plaintext, string password)
            {
                if (String.IsNullOrEmpty(plaintext)) // nothing to encrypt
                    return "";

                // 'escape' plaintext so chars outside ISO-8859-1 work in single-byte packing, but keep
                // spaces as spaces (not '%20') so encrypted text doesn't grow too int (quick & dirty)

                long[] v = StrToLongs(Escape(plaintext).Replace("%20", " "));  // convert string to array of ints
                long[] k = KeyToLongs(password.PadRight(16).Substring(0, 16));  // simply convert first 16 chars of password as key

                //for (int voy = 0; voy < v.Length; voy++)
                //    HttpContext.Current.Trace.Write("v " + voy + ": " + v[voy]);

                int n = v.Length;
                if (n == 0)
                    return ""; // nothing to encrypt
                if (n == 1)
                    v = new long[2] { v[0], 0 };  // algorithm doesn't work for n<2 so fudge by adding a null

                uint delta = 0x9E3779B9;
                uint sum = 0;
                long z = v[n - 1];
                uint e;
                long y = v[0];
                int mx;
                int q = (int)Math.Floor(6m + 52 / n);

                //for (int voy = 0; voy < k.Length; voy++)
                //    HttpContext.Current.Trace.Write("pw " + voy + ": " + k[voy]);

                //HttpContext.Current.Trace.Write("z = " + z);
                //HttpContext.Current.Trace.Write("y = " + y);
                //HttpContext.Current.Trace.Write("delta = " + delta);
                //HttpContext.Current.Trace.Write("q = " + q);

                while (q-- > 0)
                {  // 6 + 52/n operations gives between 6 & 32 mixes on each word
                    sum += delta;
                    e = sum >> 2 & 3;
                    int p = 0;

                    //for (int voy = 0; voy < v.Length; voy++)
                    //    HttpContext.Current.Trace.Write(voy + ": " + v[voy]);

                    for (; p < n - 1; p++)
                    {
                        y = v[p + 1];
                        //HttpContext.Current.Trace.Write("y = " + y + " | p+1 = " + (p + 1));
                        //HttpContext.Current.Trace.Write("step 1: " + ((uint)z >> 5));
                        //HttpContext.Current.Trace.Write("step 2: " + ((int)y << 2));
                        //HttpContext.Current.Trace.Write("step 3: " + ((uint)z >> 5 ^ (int)y << 2));
                        //HttpContext.Current.Trace.Write("step 4: " + (((int)y >> 3) ^ (int)z << 4));
                        //HttpContext.Current.Trace.Write("step 5: " + ((int)sum ^ (int)y) + " sum = " + sum);
                        ////HttpContext.Current.Trace.Write("step 5b: " + ((y >> 3 ^ z << 4) ^ ((int)sum ^ (int)y)));
                        //HttpContext.Current.Trace.Write("step 6: " + (k[p & 3 ^ e] ^ (int)z));
                        ////mx = (z>>>5 ^ y<<2) + (y>>>3 ^ z<<4) ^ (sum^y) + (k[p&3 ^ e] ^ z)

                        mx = (int)(((uint)z >> 5 ^ (int)y << 2)
                                + ((int)((uint)y >> 3) ^ z << 4)
                                ^ ((int)sum ^ (int)y)
                                + (k[p & 3 ^ e] ^ (int)z));

                        //HttpContext.Current.Trace.Write("v[p] = " + v[p] + " | mx = " + mx + " | z = " + z); // + " | " + ((long)v[p] + (long)mx));
                        z = (v[p] + mx);
                        v[p] = v[p] + mx;
                        //HttpContext.Current.Trace.Write("v[p] = " + v[p] + " | mx = " + mx + " | z = " + z);
                    }
                    //for (int voy = 0; voy < v.Length; voy++)
                    //    HttpContext.Current.Trace.Write(voy + ": " + v[voy]);

                    y = v[0];
                    //mx=(z >>>5 ^ y<<2) + (y>>>3 ^ z<<4) ^ (sum^y) + (k[p&3 ^ e] ^ z)
                    mx = (int)(((uint)z >> 5 ^ (int)y << 2)
                            + ((int)((uint)y >> 3) ^ z << 4)
                            ^ ((int)sum ^ (int)y)
                            + (k[p & 3 ^ e] ^ (int)z));

                    v[n - 1] += mx;
                    z = v[n - 1];
                    //HttpContext.Current.Trace.Write("y = " + y + " | mx = " + mx + " | z = " + z);
                }



                //return EscapeControlCharacters(ciphertext);
                return Escape(LongsToStr(v));
                //return intsToStr(v);
            }

            /// <summary>
            /// decrypts an encrypted text that was encrypted using Block Tea
            /// </summary>
            /// <param name="ciphertext">encrypted text to decrypt</param>
            /// <param name="password">encryption password</param>
            /// <returns>decrypted plain text</returns>
            public static string Decrypt(string ciphertext, string password)
            {
                if (String.IsNullOrEmpty(ciphertext)) // nothing to decrypt
                    return "";
                //int[] v = StrToints(UnescapeControlCharacters(ciphertext));
                long[] v = StrToLongs(Unescape(ciphertext));
                long[] k = KeyToLongs(password.PadRight(16).Substring(0, 16));
                int n = v.Length;

                uint delta = 0x9E3779B9;
                long z = v[n - 1];
                long e;
                long y = v[0];
                int mx;
                int q = (int)Math.Floor(6m + 52 / n);
                int p;
                long sum = q * delta;

                //for (int voy = 0; voy < v.Length; voy++)
                //    HttpContext.Current.Trace.Write(voy + ": " + v[voy]);

                while (sum != 0)
                {
                    e = sum >> 2 & 3;
                    p = n - 1;

                    //HttpContext.Current.Trace.Write("e = " + e + " | sum = " + sum);

                    for (; p > 0; p--)
                    {
                        z = v[p - 1];
                        mx = (int)(((uint)z >> 5 ^ (int)y << 2)
                                + ((int)((uint)y >> 3) ^ z << 4)
                                ^ ((int)sum ^ (int)y)
                                + (k[p & 3 ^ e] ^ (int)z));

                        //HttpContext.Current.Trace.Write("z = " + z + " | mx = " + mx + " | v[p] = " + v[p] + " | " + ((long)v[p] - mx));
                        v[p] = v[p] - mx;
                        y = (long)v[p];
                        //HttpContext.Current.Trace.Write("y = " + y + " | v[p] = " + v[p]);
                    }
                    z = v[n - 1];
                    mx = (int)(((uint)z >> 5 ^ (int)y << 2)
                            + ((int)((uint)y >> 3) ^ z << 4)
                            ^ ((int)sum ^ (int)y)
                            + (k[p & 3 ^ e] ^ (int)z));
                    //HttpContext.Current.Trace.Write("z = " + z + " | mx = " + mx + " | v[p] = " + v[p] + " | " + ((long)v[p] - mx));
                    v[p] = v[p] - mx;
                    y = (long)v[p];
                    sum -= delta;
                    //HttpContext.Current.Trace.Write("y = " + y + " | v[p] = " + v[p]);
                }

                string plaintext = LongsToStr(v);
                if (plaintext.IndexOf("\x00") != -1)
                {
                    // strip trailing null chars resulting from filling 4-char blocks
                    plaintext = plaintext.Substring(0, plaintext.IndexOf("\x00"));
                }
                return Unescape(plaintext);                
            }

            public static string Escape(string str)
            {
                //return Microsoft.JScript.GlobalObject.escape(str);
                return System.Uri.EscapeUriString(str);
            }

            public static string Unescape(string str)
            {
                //return Microsoft.JScript.GlobalObject.unescape(str);
                return System.Uri.UnescapeDataString(str);
            }

            /// <summary>
            /// Convert 16 chars of key to array(4) of ints
            /// </summary>
            /// <param name="k"></param>
            /// <returns></returns>
            public static int[] KeyToInts(string k)
            {
                int[] l = new int[4];
                for (int i = 0; i < 4; i++)
                {
                    l[i] = (int)k[i * 4]
                          + ((int)k[i * 4 + 1] << 8)
                          + ((int)k[i * 4 + 2] << 16)
                          + ((int)k[i * 4 + 3] << 24);
                }
                return l;  // note running off the end of the string is ok 'cos Xor '^' treats NaN as 0
            }

            /// <summary>
            /// Convert 16 chars of key to array(4) of ints
            /// </summary>
            /// <param name="k"></param>
            /// <returns></returns>
            public static long[] KeyToLongs(string k)
            {
                long[] l = new long[4];
                for (int i = 0; i < 4; i++)
                {
                    l[i] = (int)k[i * 4]
                          + ((int)k[i * 4 + 1] << 8)
                          + ((int)k[i * 4 + 2] << 16)
                          + ((int)k[i * 4 + 3] << 24);
                }
                return l;  // note running off the end of the string is ok 'cos Xor '^' treats NaN as 0
            }

            /// <summary>
            /// convert string to array of ints, each containing 4 chars
            /// note chars must be within ISO-8859-1 (with Unicode code-point < 256) to fit 4/int
            /// </summary>
            /// <param name="str">string</param>
            /// <returns>array of ints</returns>
            public static int[] StrToInts(string str)
            {
                int[] l = new int[(int)Math.Ceiling(((double)str.Length) / 4d)];
                for (int i = 0; i < l.Length; i++)
                {
                    // note little-endian encoding - endianness is irrelevant as int as 
                    // it is the same in intsToStr() 
                    for (int j = 0; j < 4; j++)
                    {
                        int index = i * 4 + j;
                        if (index < str.Length)
                            l[i] += (int)(str[index]) << j * 8;
                    }
                }
                return l;  // note running off the end of the string generates nulls since 
            }              // bitwise operators treat NaN as 0

            /// <summary>
            /// convert array of ints back to string
            /// </summary>
            /// <param name="l">array of ints</param>
            /// <returns>string</returns>
            public static string IntsToStr(int[] l)
            {
                StringBuilder str = new StringBuilder();

                for (int i = 0; i < l.Length; i++)
                {
                    for (int j = 0; j < 4; j++)
                        str.Append((char)(l[i] >> (8 * j) & 0xFF));
                }

                return str.ToString();
            }

            /// <summary>
            /// convert string to array of ints, each containing 4 chars
            /// note chars must be within ISO-8859-1 (with Unicode code-point < 256) to fit 4/int
            /// </summary>
            /// <param name="str">string</param>
            /// <returns>array of ints</returns>
            public static long[] StrToLongs(string str)
            {
                // note chars must be within ISO-8859-1 (with Unicode code-point < 256) to fit 4/long
                long[] l = new long[(int)Math.Ceiling(((double)str.Length) / 4d)];

                //HttpContext.Current.Trace.Write("" + (str.Length / 4));
                //HttpContext.Current.Trace.Write("strlen = " + str.Length);
                //HttpContext.Current.Trace.Write("llen = " + l.Length);
                
                for (int i = 0; i < l.Length; i++)
                {
                    // note little-endian encoding - endianness is irrelevant as int as 
                    // it is the same in intsToStr() 
                    for (int j = 0; j < 4; j++)
                    {
                        int index = i * 4 + j;
                        if (index < str.Length)
                            l[i] += (int)(str[index]) << j * 8;
                    }
                }
                return l;  // note running off the end of the string generates nulls since 
            }              // bitwise operators treat NaN as 0

            /// <summary>
            /// convert array of ints back to string
            /// </summary>
            /// <param name="l">array of ints</param>
            /// <returns>string</returns>
            public static string LongsToStr(long[] l)
            {
                StringBuilder str = new StringBuilder();

                for (int i = 0; i < l.Length; i++)
                {
                    for (int j = 0; j < 4; j++)
                        str.Append((char)(l[i] >> (8 * j) & 0xFF));
                }

                return str.ToString();
            }

            /// <summary>
            /// escape control chars etc which might cause problems with encrypted texts
            /// </summary>
            /// <param name="str"></param>
            /// <returns></returns>
            public static string EscapeControlCharacters(string str)
            {
                string escaped = str;
                foreach (char c in ControlCharacters)
                    escaped.Replace(c.ToString(), "!" + (int)c + "!");
                return escaped;
            }

            /// <summary>
            /// unescape potentially problematic nulls and control characters
            /// </summary>
            /// <param name="str"></param>
            /// <returns></returns>
            public static string UnescapeControlCharacters(string str)
            {
                string unescaped = str;
                foreach (char c in ControlCharacters)
                    unescaped.Replace("!" + (int)c + "!", c.ToString());
                return unescaped;
            }
        }

        #endregion

    }
}