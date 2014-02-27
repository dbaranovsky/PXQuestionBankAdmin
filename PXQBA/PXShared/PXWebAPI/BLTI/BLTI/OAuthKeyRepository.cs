
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;

namespace BLTI
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
			var oAuthKeySecretsettings = ConfigurationManager.GetSection("oauthSettings") as NameValueCollection;
			if (oAuthKeySecretsettings == null) return;
			foreach (var key in oAuthKeySecretsettings.AllKeys)
			{
				OAuthKeys.Add(key, oAuthKeySecretsettings[key]);
			}
		}

		public string GetSecret(string key)
		{
			return OAuthKeys.ContainsKey(key) ? OAuthKeys[key] : null;
		}
	}
}
