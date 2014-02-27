using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.IO;

using Ionic.Zip;

using Bfw.Common;
using Bfw.Common.Collections;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;
using System.Windows.Documents;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/PutAnnouncement command
    /// </summary>
    /// 
    [Obsolete("PutAnnouncment Command is out of date and no longer matches DLAP documentation", false)]
    public class PutAnnoucement : DlapCommand
    {
        #region Properties

        public string DomainId { get; set; }

        public string Path { get; set; }

        public Announcement Announcement { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Writes a ZIP files containing he metadata XML as well as the announcement body (XAML) to
        /// a stream.
        /// </summary>
        /// <returns>Stream containing the ZIPed contents of the announcement</returns>
        private Stream BuildRequestStream()
        {
            // Create the meta file
            var metaFile = new MemoryStream();
            var metaFileSw = new StreamWriter(metaFile);
            var element = Announcement.ToEntity();
            metaFileSw.Write(element.ToString());
            metaFileSw.Flush();
            metaFile.Seek(0, SeekOrigin.Begin);

            var zip = new ZipFile();
            zip.AddEntry("meta.xml", metaFile);           

            var zipFile = new MemoryStream();
            zip.Save(zipFile);
            zipFile.Flush();
            zipFile.Seek(0, SeekOrigin.Begin);

            return zipFile;
        }

        /// <summary>
        /// Returns the complete contents of the stream.
        /// </summary>
        /// <param name="s">Stream to read contents of.</param>
        /// <returns>String containing all data from <see cref="s" />.</returns>
        public static string FromStream(Stream s)
        {
            var sr = new StreamReader(s);
            var str = sr.ReadToEnd();
            s.Seek(0, SeekOrigin.Begin);
            return str;
        }

        #endregion

        #region override DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/PutAnnouncement command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/PutAnnouncement</returns>
        /// <remarks></remarks>
        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Post,
                Parameters = new Dictionary<string, object>()
                {
                    { "cmd", "putannouncement" },
                    { "entityid", DomainId },
                    { "path", Path }
                }
            };

            request.AppendData(BuildRequestStream());            

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/PutAnnouncement command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        /// <remarks></remarks>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
                throw new DlapException(string.Format("PutAnnouncement failed with code: {0}.\nResponse was:\n{1}", response.Code, response.Message));

            var announcement = new Announcement();
            announcement.ParseEntity(response.ResponseXml.Root);

            Announcement.Version = announcement.Version;
        }

        #endregion        
    }
}
