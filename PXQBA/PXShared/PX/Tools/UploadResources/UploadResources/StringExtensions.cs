using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Xsl;

namespace UploadResources.Extensions
{
    public static class StringExtensions
    {
        #region Checks for Valid URL


        public static bool IsValidUrl(this string text)
        {
            var rx = new Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
            return rx.IsMatch(text);
        }

        #endregion

        #region Encrypt & Decrypt Functions


        public static string Encrypt(this string value)
        {
            const string sPassKey = "16";
            byte[] sOriginalArray = Encoding.UTF8.GetBytes(value);

            var MD5Hash = new MD5CryptoServiceProvider();
            byte[] sPassKeyArray = MD5Hash.ComputeHash(Encoding.UTF8.GetBytes(sPassKey));
            MD5Hash.Clear();

            var tripleDesCsp = new TripleDESCryptoServiceProvider
            {
                Key = sPassKeyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = tripleDesCsp.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(sOriginalArray, 0, sOriginalArray.Length);
            tripleDesCsp.Clear();

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }


        /// <summary>
        /// Decryptes a string using a key. Decoding is done using MD5 encryption.
        /// </summary>
        /// <param name="encryptedValue">the string to process</param>
        /// <returns>A string containing the decrypted account number</returns>
        public static string Decrypt(this string encryptedValue)
        {
            if (encryptedValue == null || encryptedValue.Length <= 0)
                return "";

            const string sPassKey = "16";
            encryptedValue = encryptedValue.Replace(" ", "+");
            byte[] sOriginalArray = Convert.FromBase64String(encryptedValue);
            var MD5Hash = new MD5CryptoServiceProvider();
            byte[] sPassKeyArray = MD5Hash.ComputeHash(Encoding.UTF8.GetBytes(sPassKey));
            MD5Hash.Clear();
            var tripleDesCsp = new TripleDESCryptoServiceProvider
            {
                Key = sPassKeyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = tripleDesCsp.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(sOriginalArray, 0, sOriginalArray.Length);
            tripleDesCsp.Clear();

            return Encoding.UTF8.GetString(resultArray);
        }

        #endregion

        #region Return Right/Left Characters

        /// <summary>
        /// Returns the last few characters of the string with a length
        /// specified by the given parameter. If the string'value length is less than the 
        /// given length the complete string is returned. If length is zero or 
        /// less an empty string is returned
        /// </summary>
        /// <param name="value">the string to process</param>
        /// <param name="length">Number of characters to return</param>
        /// <returns>string</returns>
        public static string Right(this string value, int length)
        {
            length = Math.Max(length, 0);

            if (value.Length > length)
            {
                return value.Substring(value.Length - length, length);
            }
            return value;
        }

        /// <summary>
        /// Returns the first few characters of the string with a length
        /// specified by the given parameter. If the string'value length is less than the 
        /// given length the complete string is returned. If length is zero or 
        /// less an empty string is returned
        /// </summary>
        /// <param name="value">the string to process</param>
        /// <param name="length">Number of characters to return</param>
        /// <returns>string</returns>
        public static string Left(this string value, int length)
        {
            length = Math.Max(length, 0);

            if (value.Length > length)
            {
                return value.Substring(0, length);
            }
            return value;
        }

        #endregion

        #region Strip strings

        /// <summary>
        /// Strip a string of the specified character.
        /// </summary>
        /// <param name="value">the string to process</param>
        /// <param name="character">character to remove from the string</param>
        /// <example>
        /// string value = "abcde"; 
        /// value = value.Strip('b');  //value becomes 'acde;
        /// </example>
        /// <returns></returns>
        public static string Strip(this string value, char character)
        {
            value = value.Replace(character.ToString(), "");

            return value;
        }

        /// <summary>
        /// Strip a string of the specified characters.
        /// </summary>
        /// <param name="value">the string to process</param>
        /// <param name="characters">list of characters to remove from the string</param>
        /// <example>
        /// string value = "abcde"; 
        /// value = value.Strip('a', 'd');  //value becomes 'bce;
        /// </example>
        /// <returns></returns>
        public static string Strip(this string value, params char[] characters)
        {
            foreach (char character in characters)
            {
                value = value.Replace(character.ToString(), "");
            }

            return value;
        }

        /// <summary>
        /// Strip a string of the specified substring.
        /// </summary>
        /// <param name="value">the string to process</param>
        /// <param name="substring">substring to remove</param>
        /// <example>
        /// string value = "abcde";
        /// 
        /// value = value.Strip("bcd");  //value becomes 'ae;
        /// </example>
        /// <returns></returns>
        public static string Strip(this string value, string substring)
        {
            value = value.Replace(substring, "");

            return value;
        }

        /// <summary>
        /// Strip unwanted characters and replace them with empty string
        /// </summary>
        /// <param name="data">the string to strip characters from.</param>
        /// <param name="textToStrip">Characters to strip. Can contain Regular expressions</param>
        /// <returns>The stripped text if there are matching string.</returns>
        /// <remarks>If error occurred, original text will be the output.</remarks>
        public static string StripUnwantedText(this string data, string textToStrip)
        {
            string stripText = data;

            if (string.IsNullOrEmpty(data)) return stripText;

            try
            {
                stripText = Regex.Replace(data, textToStrip, string.Empty);
            }
            catch (System.Exception ex)
            {
                return ex.Message;
            }
            return stripText;
        }

        /// <summary>
        /// Strips unwanted characters on the specified string
        /// </summary>
        /// <param name="data">the string to strip characters from.</param>
        /// <param name="textToStrip">Characters to strip. Can contain Regular expressions</param>
        /// <param name="textToReplace">the characters to replace the stripped text</param>
        /// <returns>The stripped text if there are matching string.</returns>
        /// <remarks>If error occurred, original text will be the output.</remarks>
        public static string StripUnwantedText(this string data, string textToStrip, string textToReplace)
        {
            string stripText = data;

            if (string.IsNullOrEmpty(data)) return stripText;

            try
            {
                stripText = Regex.Replace(data, textToStrip, textToReplace);
            }
            catch (System.Exception ex)
            {
                return ex.Message;
            }
            return stripText;
        }

        #endregion

        #region Checks String for Not Null or Empty

        /// <summary>
        /// Returns true when a given string is not null or empty
        /// </summary>
        /// <param name="value">the string to process</param>
        /// <returns>bool</returns>
        public static bool IsNotNullOrEmpty(this string value)
        {
            return !String.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Returns true when a given string is null or empty
        /// </summary>
        /// <param name="value">the string to validate</param>
        /// <returns>bool</returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return String.IsNullOrEmpty(value);
        }

        #endregion

        #region Check for Valid Email Address

        /// <summary>
        /// Checks whether a string is an valid e-mail address
        /// </summary>
        /// <param name="value">the string that is processed</param>
        /// <returns>bool</returns>
        public static bool IsValidEmailAddress(this string value)
        {
            var regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            return regex.IsMatch(value);
        }

        #endregion

        #region Check for Characters in a given string

        /// <summary>
        /// Checks if a given string contains any of the characters in the passed
        /// array of characters
        /// </summary>
        /// <param name="value">the string to be processed</param>
        /// <param name="characters">the array of characters to check for</param>
        /// <returns>bool</returns>
        public static bool ContainsAny(this string value, char[] characters)
        {
            foreach (char character in characters)
            {
                if (value.Contains(character.ToString()))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Truncate string and replace with "..."

        /// <summary>
        /// Truncates the string to a specified length and replace the truncated to a ...
        /// </summary>
        /// <param name="value">string that will be truncated</param>
        /// <param name="maxLength">total length of characters to maintain before the truncate happens</param>
        /// <returns>truncated string</returns>
        public static string Truncate(this string value, int maxLength)
        {
            // replaces the truncated string to a ...
            const string suffix = "...";
            string truncatedString = value;

            if (maxLength <= 0) return truncatedString;
            int strLength = maxLength - suffix.Length;

            if (strLength <= 0) return truncatedString;

            if (value == null || value.Length <= maxLength) return truncatedString;

            truncatedString = value.Substring(0, strLength);
            truncatedString = truncatedString.TrimEnd();
            truncatedString += suffix;
            return truncatedString;
        }

        #endregion

        #region Checks for spaces in a string

        /// <summary>
        /// Checks if a string contains no spaces
        /// </summary>
        /// <param name="value">the string to be processed</param>
        /// <returns>bool</returns>
        public static bool ContainsNoSpaces(this string value)
        {
            var regex = new Regex(@"^[a-zA-Z0-9]+$");
            return regex.IsMatch(value);
        }

        #endregion

        #region Checks for valid IP Address

        /// <summary>
        /// Checks to see if string is a valid IP address
        /// </summary>
        /// <param name="value">the string to be processed</param>
        /// <returns>bool</returns>
        public static bool IsValidIPAddress(this string value)
        {
            return Regex.IsMatch(value,
                                  @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b");
        }

        #endregion

        #region Checks for valid numeric string

        /// <summary>
        /// Checks to see if string is numeric 
        /// </summary>
        /// <param name="value"></param>
        /// <returns>bool</returns>
        public static bool IsNumeric(this string value)
        {
            var regex = new Regex(@"[0-9]");
            return regex.IsMatch(value);
        }

        #endregion

        #region Checks for strong password

        /// <summary>
        /// Validates whether a string is compliant with a strong password policy
        /// </summary>
        /// <param name="password">the string to be processed</param>
        /// <returns>bool</returns>
        public static bool IsStrongPassword(this string password)
        {
            bool isStrong = Regex.IsMatch(password, @"[\d]");
            if (isStrong) isStrong = Regex.IsMatch(password, @"[a-z]");
            if (isStrong) isStrong = Regex.IsMatch(password, @"[A-Z]");
            if (isStrong) isStrong = Regex.IsMatch(password, @"[\password~!@#\$%\^&\*\(\)\{\}\|\[\]\\:;'?,.`+=<>\/]");
            if (isStrong) isStrong = password.Length > 7;
            return isStrong;
        }

        #endregion

        #region Parse the XML string

        /// <summary>
        /// Transforms the XML string with the appropriate XSL and returns the transformed string
        /// </summary>
        /// <param name="xmlData">XML string that needs to be transformed</param>
        /// <param name="xsl">XSL string to be used for transformation</param>
        /// <returns>Transformed data</returns>
        public static string TransformXML(this string xmlData, string xsl)
        {
            var transformedData = new StringBuilder();
            XmlReader xmlReader = XmlReader.Create(xmlData);
            var xslStream = new MemoryStream(Encoding.Default.GetBytes(xsl));
            var xslReader = new XmlTextReader(xslStream);

            var xslTransform = new XslCompiledTransform();

            xslTransform.Load(xslReader);

            var xmlwriter = new XmlTextWriter(new StringWriter(transformedData));

            xslTransform.Transform(xmlReader, xmlwriter);

            //Dispose all resources
            xslStream.Dispose();

            xmlwriter.Close();

            xmlReader.Close();

            xslReader.Close();

            return transformedData.ToString();
        }

        #endregion

        #region Proper case

        private static readonly string[] _prefixes = { "mc" };
        private static readonly Regex _properNameRx = new Regex(@"\b(\w+)\b");

        /// <summary>
        /// Converts the given string into ProperCase.
        /// </summary>
        /// <param name="original">The original string, f.e. "JoHN F kenNeDy"</param>
        /// <returns>The string converted into <b>ProperCase</b>, f.e. "John F Kennedy"</returns>
        public static string ToProperCase(this string original)
        {
            if (IsNullOrEmpty(original))
                return original;

            // Make proper case on a word-by-word basis
            string result = _properNameRx.Replace(original.ToLower(), HandleWord);
            return result;
        }

        private static string ProperCase(string word)
        {
            if (string.IsNullOrEmpty(word))
                return String.Empty;

            if (word.Length > 1)
                return Char.ToUpper(word[0]) + word.Substring(1);

            return word.ToUpper();
        }

        private static string HandleWord(Match match)
        {
            string word = match.Groups[1].Value;

            foreach (string prefix in _prefixes)
            {
                if (word.StartsWith(prefix))
                    return ProperCase(prefix) + ProperCase(word.Substring(prefix.Length));
            }

            return ProperCase(word);
        }

        #endregion

        #region Trim

        /// <summary>
        /// Removes all occurrences of white space characters from the beginning and end of the given string.
        /// </summary>
        /// <param name="value">A string.</param>
        /// <param name="characters">An array of Unicode characters to be removed or <see langword="null" />.</param>
        /// <returns>An empty string if <paramref name="value" /> is null or empty. Otherwise, the trimmed string.</returns>
        public static string Trim(this string value, params char[] characters)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return (characters.Length == 0) ? value.Trim() : value.Trim(characters);
        }

        #endregion

        #region Card Number Masking

        /// <summary>
        /// Returns a card number representation appropiate for display
        /// </summary>
        /// <param name="cardNumber">The card number</param>
        /// <returns>A display card number</returns>
        public static string MaskNumberForDisplay(this string cardNumber)
        {
            return GetMaskedCardNumber(cardNumber, 4);
        }

        /// <summary>
        /// Get the Masked Number
        /// </summary>
        /// <param name="cardNumber">Card Number to be masked</param>
        /// <param name="length">length of the number to be displayed</param>
        /// <returns></returns>
        private static string GetMaskedCardNumber(string cardNumber, int length)
        {
            if (cardNumber.Length > length)
                cardNumber = cardNumber.Substring(cardNumber.Length - length).PadLeft(cardNumber.Length, 'X');

            if (cardNumber.Length == 16)
                cardNumber = string.Format("{0}-{1}-{2}-{3}", cardNumber.Substring(0, 4),
                                            cardNumber.Substring(4, 4), cardNumber.Substring(8, 4),
                                            cardNumber.Substring(12, 4));

            return cardNumber;
        }

        #endregion

        #region Word Count

        /// <summary>
        /// Count all words in a given string
        /// </summary>
        /// <param name="value">string to begin with</param>
        /// <returns>int</returns>
        public static int WordCount(this string value)
        {
            int count = 0;
            try
            {
                // Exclude whitespaces, Tabs and line breaks
                var re = new Regex(@"[^\s]+");
                MatchCollection matches = re.Matches(value);
                count = matches.Count;
            }
            catch
            {
            }
            return count;
        }

        #endregion

        #region Strip Special Characters from String

        /// <summary>
        /// Strip Special Characters from String
        /// </summary>
        /// <param name="name">string to begin with</param>
        /// <returns>string</returns>
        public static string ToDotNetIdentifier(this string name)
        {
            //Compliant with item 2.4.2 of the C# specification
            var regex = new Regex(@"[^\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Nl}\p{Mn}\p{Mc}\p{Cf}\p{Pc}\p{Lm}]");
            string ret = regex.Replace(name, "");
            //The identifier must start with a character or a "_"
            if (!char.IsLetter(ret, 0) && !Microsoft.CSharp.CSharpCodeProvider.CreateProvider("C#").IsValidIdentifier(ret))
                ret = string.Concat("_", ret);
            return ret;
        }

        #endregion

        #region Phone Number Masking
        public static string ToPhoneNumber(this string phone)
        {
            //check if parameter is blank or null and return an empty string
            if (phone.IsNullOrEmpty()) return "";

            var strTemp = ""; //Temporary string to hold our working text

            // Build the output string formatted to our liking!
            // (xxx) xxx-xxxx

            strTemp = "(";
            strTemp = strTemp + Left(phone, 3);    // Area code
            strTemp = strTemp + ") ";                  // ") "
            strTemp = strTemp + phone.Substring(3, 3);   // Exchange
            strTemp = strTemp + "-";                   // "-"

            strTemp = strTemp + phone.Substring(6, 4);    // 4 digit part    



            if (phone.Length > 10)
            {
                strTemp = strTemp + " ext. ";
                strTemp = strTemp + phone.Substring(10);
            }


            return strTemp;
        }
        #endregion
    }
}