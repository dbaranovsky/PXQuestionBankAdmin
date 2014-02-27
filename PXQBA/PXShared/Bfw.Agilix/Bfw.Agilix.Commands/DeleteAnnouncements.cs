using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using System.Xml.Linq;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/DeleteAnnouncements command.
    /// </summary>
    public class DeleteAnnouncements : DlapCommand
    {
        #region Properties

        /// <summary>
        /// Announcements to delete.
        /// </summary>
        public IEnumerable<Announcement> Announcements { get; set; }

        /// <summary>
        /// Any failures that occured.
        /// </summary>
        public List<ItemStorageFailure> Failures { get; protected set; }

        #endregion        

        public DeleteAnnouncements()
        {
            Announcements = new List<Announcement>();
            Failures = new List<ItemStorageFailure>();
        }
        #region Methods

        /// <summary>
        /// Builds the XML request to POST to DLAP.
        /// </summary>
        /// <returns>XML fragment to act as the request POST body.</returns>
        private XElement BuildRequestsElement()
        {
            XElement requestsElement = new XElement("requests");

            foreach (var announcement in Announcements)
            {
                XElement announcementElement = new XElement("announcement");
                announcementElement.SetAttributeValue("entityid", announcement.EntityId);
                announcementElement.SetAttributeValue("path", announcement.Path);
                requestsElement.Add(announcementElement);
            }

            return requestsElement;
        }

        #endregion

        #region Overrides from DlapCommand

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/DeleteAnnouncements command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse.</param>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
            {
                throw new DlapException(string.Format("DeleteAnnouncements request failed with response code {0}", response.Code));
            }

            int index = 0;
            string message = string.Empty;

            foreach (var resp in response.Batch)
            {
                if (DlapResponseCode.OK != resp.Code)
                {
                    message = (!string.IsNullOrEmpty(resp.Message) ? resp.Message : resp.Code.ToString());
                    Failures.Add(new ItemStorageFailure()
                    {
                        Reason = message
                    });
                }
                ++index;
            }
        }

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/DeleteAnnouncements command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/DeleteAnnouncements.</returns>
        /// <remarks></remarks>
        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Post,
                Parameters = new Dictionary<string, object>()
                {
                    { "cmd", "deleteannouncements" }
                }
            };

            request.AppendData(BuildRequestsElement());

            return request;
        }

        #endregion
    }
}
