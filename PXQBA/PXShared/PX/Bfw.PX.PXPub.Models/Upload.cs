using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
namespace Bfw.PX.PXPub.Models
{
    public class Upload
    {
        /// <summary>
        /// Gets or sets the parent id.
        /// </summary>
        /// <value>
        /// The parent id.
        /// </value>
        public string ParentId { get; set; }

        /// <summary>
        /// Gets or sets the upload file.
        /// </summary>
        /// <value>
        /// The upload file.
        /// </value>
        public HttpPostedFileBase UploadFile { get; set; }

        /// <summary>
        /// Gets or sets the upload title.
        /// </summary>
        /// <value>
        /// The upload title.
        /// </value>
        public string UploadTitle { get; set; }

        /// <summary>
        /// Gets or sets the upload Comment.
        /// </summary>
        /// <value>
        /// The upload comment.
        /// </value>
        public string UploadComment { get; set; }

        /// <summary>
        /// Gets or sets the on complete script.
        /// </summary>
        /// <value>
        /// The on complete script.
        /// </value>
        public string OnCompleteScript { get; set; }

        /// <summary>
        /// Gets or sets the on begin script.
        /// </summary>
        /// <value>
        /// The on begin script.
        /// </value>
        public string OnBeginScript { get;set; }

        /// <summary>
        /// Gets or sets the type of the upload.
        /// </summary>
        /// <value>
        /// The type of the upload.
        /// </value>
        public UploadType UploadType { get;set; }

        /// <summary>
        /// Gets or sets the type of the upload file.
        /// </summary>
        /// <value>
        /// The type of the upload file.
        /// </value>
        public UploadFileType UploadFileType { get;set; }

        /// <summary>
        /// Sets a path in case we need to store this as a resource also acts as a flag.
        /// </summary>
        public string UploadFilePath { get; set; }

        /// <summary>
        /// Sets a path in case we need to store this as a resource also acts as a flag.
        /// </summary>
        public bool RetainOriginalFile { get; set; }



        /// <summary>
        /// Download Only Document Types.
        /// </summary>
        public string DownloadOnlyDocumentTypes { get; set; }

        /// <summary>
        /// Sets a EnrollmentId for reading/writing resources.
        /// </summary>
        public string EnrollmentId { get; set; }

        /// <summary>
        /// Gets or sets the on success action URL.
        /// </summary>
        /// <value>
        /// The on success action URL.
        /// </value>
        public string OnSuccessActionUrl { get; set; }

        /// <summary>
        /// this property when true forces the upload controller to add the file to the resource map
        /// 
        /// </summary>
        public bool AddToResourceMap { get;  set; }

        /// <summary>
        /// Gets or sets the custom params.
        /// </summary>
        /// <value>
        /// The custom params.
        /// </value>
        public IDictionary<string, string> CustomParams { get; set; }

        public EasyXDM EasyXDM { get; set; }
    }
}
