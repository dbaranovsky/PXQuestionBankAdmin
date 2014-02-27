using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using BFW.Encryption;

namespace BFW
{
    public static class ItemServer
    {
        #region Configuration Settings
        private static string _BaseUrl;
        public static string BaseUrl
        {
            get 
            {
                if (String.IsNullOrEmpty(_BaseUrl))
                {
                    if (HttpContext.Current.Request.UserHostAddress == "::1")
                        _BaseUrl = ConfigurationManager.AppSettings["BaseUrlLocalHost"];
                    else
                        _BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];

                    if (String.IsNullOrEmpty(_BaseUrl))
                        throw new HttpException("Missing Configuration Setting for BaseUrl");
                }
                return _BaseUrl;
            }
        }

        /// <summary>
        /// file system path for base question data directory
        /// </summary>
        private static string _QuestionDataPath;
        public static string QuestionDataPath
        {
            get
            {
                if (String.IsNullOrEmpty(_QuestionDataPath))
                {
                    _QuestionDataPath = ConfigurationManager.AppSettings["QuestionDataPath"];
                    if (String.IsNullOrEmpty(_QuestionDataPath))
                        throw new HttpException("Missing Configuration Setting for QuestionDataPath");
                }
                return _QuestionDataPath;
            }
        }

        /// <summary>
        /// file system path for base results data directory
        /// </summary>
        private static string _ResultsDataPath;
        public static string ResultsDataPath
        {
            get
            {
                if (String.IsNullOrEmpty(_ResultsDataPath))
                {
                    _ResultsDataPath = ConfigurationManager.AppSettings["ResultsDataPath"];
                    if (String.IsNullOrEmpty(_ResultsDataPath))
                        throw new HttpException("Missing Configuration Setting for ResultsDataPath");
                }
                return _ResultsDataPath;
            }
        }

        /// <summary>
        /// absoulte URL of the player server
        /// </summary>
        private static string _PlayerServerUrl;
        public static string PlayerServerUrl
        {
            get
            {
                if (String.IsNullOrEmpty(_PlayerSaveServer))
                {
                    _PlayerServerUrl = ConfigurationManager.AppSettings["PlayerServerUrl"];
                    if (String.IsNullOrEmpty(_PlayerServerUrl))
                        throw new HttpException("Missing Configuration Setting for PlayerServerUrl");
                }
                return _PlayerServerUrl;
            }
        }
        
        /// <summary>
        /// relative URL of the player save server
        /// </summary>
        private static string _PlayerSaveServer;
        public static string PlayerSaveServer
        {
            get
            {
                if (String.IsNullOrEmpty(_PlayerSaveServer))
                {
                    _PlayerSaveServer = ConfigurationManager.AppSettings["PlayerSaveServer"];
                    if (String.IsNullOrEmpty(_PlayerSaveServer))
                        throw new HttpException("Missing Configuration Setting for PlayerSaveServer");
                }
                return _PlayerSaveServer;
            }
        }

        /// <summary>
        /// relative URL of the player restore server
        /// </summary>
        private static string _PlayerRestoreServer;
        public static string PlayerRestoreServer
        {
            get
            {
                if (String.IsNullOrEmpty(_PlayerRestoreServer))
                {
                    _PlayerRestoreServer = ConfigurationManager.AppSettings["PlayerRestoreServer"];
                    if (String.IsNullOrEmpty(_PlayerRestoreServer))
                        throw new HttpException("Missing Configuration Setting for PlayerRestoreServer");
                }
                return _PlayerRestoreServer;
            }
        }

        private static bool? _IsPlayerDebug;
        public static bool IsPlayerDebug
        {
            get
            {
                if (!_IsPlayerDebug.HasValue)
                    _IsPlayerDebug = Convert.ToBoolean(ConfigurationManager.AppSettings["IsPlayerDebug"]);
                return _IsPlayerDebug.Value;
            }
        }
        private static bool? _UseEncryption;
        public static bool UseEncryption
        {
            get
            {
                if (!_UseEncryption.HasValue)
                    _UseEncryption = Convert.ToBoolean(ConfigurationManager.AppSettings["UseEncryption"]);
                return _UseEncryption.Value;
            }
        }
        private static bool? _UseEncryptedXml;
        public static bool UseEncryptedXml
        {
            get
            {
                if (!_UseEncryptedXml.HasValue)
                    _UseEncryptedXml = Convert.ToBoolean(ConfigurationManager.AppSettings["UseEncryptedXml"]);
                return _UseEncryptedXml.Value;
            }
        }
        private static string _EncryptionKey;
        public static string EncryptionKey
        {
            get
            {
                if (String.IsNullOrEmpty(_EncryptionKey))
                {
                    _EncryptionKey = ConfigurationManager.AppSettings["EncryptionKey"];
                    if (String.IsNullOrEmpty(_EncryptionKey))
                        throw new HttpException("Missing Configuration Setting for EncryptionKey");
                }
                return _EncryptionKey;
            }
        }
        #endregion







        public static string Decrypt(string cipherText)
        {
            return Decrypt(cipherText, false);
        }

        public static string Decrypt(string cipherText, bool isForce)
        {
            if (UseEncryption || isForce)
                return Encryption.BlockTea.Decrypt(cipherText, EncryptionKey);
            else
                return cipherText;
        }

        public static string Encrypt(string plainText)
        {
            return Encrypt(plainText, false);
        }

        public static string Encrypt(string plainText, bool isForce)
        {
            if (UseEncryption || isForce)
                return Encryption.BlockTea.Encrypt(plainText, EncryptionKey);
            else
                return plainText;
        }
    }

    public enum QuestionMode : int
    {
        Undefined = -1,
        Preview = 0,
        Delivery = 1,
        Review = 2
    }
}