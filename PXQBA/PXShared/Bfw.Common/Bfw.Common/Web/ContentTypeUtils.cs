namespace Bfw.Common.Web
{
    /// <summary>
    /// Utilities related to working with HTTP content types.
    /// </summary>
    public static class ContentTypeUtils
    {
        /// <summary>
        /// Gets a content type from a file name, using the Windows Registry.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static string GetContentType(string fileName)
        {
            string contentType = "application/octetstream";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();

            Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (registryKey != null && registryKey.GetValue("Content Type") != null)
            {
                contentType = registryKey.GetValue("Content Type").ToString();
            }

            return contentType;
        }
    }
}
