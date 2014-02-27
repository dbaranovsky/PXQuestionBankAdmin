using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;

namespace Bfw.PXWebAPI.Models
{
    public class OAuthKeyRepository
    {
        public Dictionary<string, string> OAuthKeys
        {
            get;
            set;
        }
        public OAuthKeyRepository()
        {
            OAuthKeys = new Dictionary<string, string>();
            var oAuthKeySecretsettings = ConfigurationManager.GetSection("oauthkeysecretSettings") as NameValueCollection;
            if (oAuthKeySecretsettings != null)
            {
                foreach (var key in oAuthKeySecretsettings.AllKeys)
                {
                    OAuthKeys.Add(key, oAuthKeySecretsettings[key]);
                }
            }
        }

        public string GetSecret(string key)
        {
            return OAuthKeys.ContainsKey(key) ? OAuthKeys[key] : null;
        }
    }
}
