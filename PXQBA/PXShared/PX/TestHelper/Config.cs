using System.Configuration;

namespace TestHelper
{
    /// <summary>
    /// For manipulating the config file.
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// Clears out the previous appSettings config key/value.  And inserts the new one provided in the input parameters.
        /// </summary>
        /// <param name="settingsKey">appSettings key</param>
        /// <param name="settingsValue">appSettings value.  If value is null - then this method removes the key entirely
        /// from the config to permit testing of missing keys.</param>
        /// <returns>Returns a reference to the config file.</returns>
        public static Configuration SetTestAppSettingsConfiguration(string settingsKey, string settingsValue)
        {
            // loads from execution enviornment - Test folder
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            // first remove this app key 
            config.AppSettings.Settings.Remove(settingsKey);

            if (settingsValue != null)
            {
                // arrange the key/values to what you want for the test
                config.AppSettings.Settings.Add(settingsKey, settingsValue);
            }

            // save config settings in test folder only if needed as it could impact other tests 
            config.Save(ConfigurationSaveMode.Modified);

            // reloads from file store from the test folder *test.dll.config file
            ConfigurationManager.RefreshSection("appSettings");

            return config;
        }

    }
}
