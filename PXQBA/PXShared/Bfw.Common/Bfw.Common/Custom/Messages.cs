using System;

namespace Bfw.Common.Custom
{
    /// <summary>
    /// Contains user-friendly messages to be displayed by the PX application
    /// </summary>
    public class Messages
    {
        private Messages() { }

        #region Uploading
        public const string UPLOAD_FAILURE = "It looks like you've uploaded a document type we don’t yet support, or there has been an error during the uploading process. If you're uploading content that can be pasted into the “Compose” window, please do that. Alternately, you can save your document as a supported file type or as an earlier version of your file type, and upload that.";
        public const string UPLOAD_SIZE = "Upload cannot be completed. File size cannot exceed {0} Megabytes.";
        #endregion

        #region Export
        public const string EPORTFOLIO_EXPORTED = "Your exported e-Portfolio is being processed. This may take a few minutes. When it is done it will be available to download from your Student e-Portfolio Dashboard.";
        #endregion
    }
}
