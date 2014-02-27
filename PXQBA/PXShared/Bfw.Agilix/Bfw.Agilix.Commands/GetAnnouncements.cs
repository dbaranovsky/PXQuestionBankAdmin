using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.Windows.Documents;
using System.IO;

using Ionic.Zip;

using Bfw.Common;
using Bfw.Common.Xaml;
using Bfw.Common.Collections;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/GetAnnouncement and 
    /// http://dev.dlap.bfwpub.com/Docs/Command/GetAnnouncementList commands.
    /// </summary>
    public class GetAnnouncements : DlapCommand
    {
        #region Data Members

        /// <summary>
        /// Parameters that determine what announcements are loaded from DLAP.
        /// </summary>
        public AnnouncementSearch SearchParameters { get; set; }

        /// <summary>
        /// List of announcments that were loaded based on SearchParameters.
        /// </summary>
        public List<Announcement> Announcements { get; protected set; }

        #endregion

        #region Overrides from DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/ command.
        /// If SearchParameters.Path is set, then GetAnnouncment is used. Otherwise, GetAnnouncementList is used.
        /// </summary>
        /// </summary>
        /// <returns>
        /// Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/.
        /// </returns>
        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object>() 
                {
                    { "cmd", "GetAnnouncementList" },
                    { "entityid", SearchParameters.EntityId}
                }
            };

            if (!string.IsNullOrEmpty(SearchParameters.Path))
            {
                request.Parameters["cmd"] = "getannouncement";
                request.Parameters["entityid"] = SearchParameters.EntityId;
                request.Parameters["path"] = SearchParameters.Path;                
            }

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/ command.  It is either 
        /// an XML response from GetAnnouncmentList or a zip response stream from GetAnnouncement.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse.</param>
        public override void ParseResponse(DlapResponse response)
        {
            if (null != response.ResponseXml)
            {
                Announcements = ParseXmlResponse(response);
            }
            else
            {
                Announcements = new List<Announcement>() { ParseStreamResponse(response) };
            }
        }

        #endregion

        #region Parsing Helper Methods

        /// <summary>
        /// Processes the ZIP file containing the Announcement meta data and content.
        /// </summary>
        /// <param name="response">The response with the stream to parse.</param>
        /// <returns>An <see cref="Announcement"/> object.</returns>
        private Announcement ParseStreamResponse(DlapResponse response)
        {
            var announcement = new Announcement();

            if (null != response.ResponseStream)
            {
                int bsize = 4096;
                byte[] buff = new byte[bsize];
                int read = 0;
                using (var zipFile = new MemoryStream())
                {

                    while ((read = response.ResponseStream.Read(buff, 0, bsize)) > 0)
                    {
                        zipFile.Write(buff, 0, read);
                    }

                    zipFile.Seek(0, SeekOrigin.Begin);
                    var zip = ZipFile.Read(zipFile);
                    var metaFile = new MemoryStream();
                    var meta = zip["meta.xml"];

                    meta.Extract(metaFile);
                    metaFile.Seek(0, SeekOrigin.Begin);                    

                    var announcementEl = XElement.Load(new XmlTextReader(metaFile, XmlNodeType.Element, null));
                    announcement.ParseEntity(announcementEl);
                    announcement.Path = SearchParameters.Path;
                    metaFile.Close();
                }
            }

            return announcement;
        }

        /// <summary>
        /// Parses out the list of items as expected by the GetAnnouncementList DLAP request.
        /// </summary>
        /// <param name="response">The response with the XML to parse.</param>
        /// <returns>An <see cref="Announcement"/> object.</returns>
        private List<Announcement> ParseXmlResponse(DlapResponse response)
        {
            var listElm = response.ResponseXml.Root;
            var list = new List<Announcement>();

            if (null != listElm && "announcements" == listElm.Name)
            {
                Announcement single = null;
                foreach (var aElm in listElm.Elements("announcement"))
                {
                    single = new Announcement();
                    single.ParseEntity(aElm);

                    list.Add(single);
                }
            }
            else
            {
                throw new BadDlapResponseException("GetAnnouncements expected an announcements element, but none was found");
            }

            return list;
        }

        #endregion
    }
}
