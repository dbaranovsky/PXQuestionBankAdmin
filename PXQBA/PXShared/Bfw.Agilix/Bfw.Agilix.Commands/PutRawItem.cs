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
    /// Alternative implementation of http://dev.dlap.bfwpub.com/Docs/Command/PutItems command
    /// that does no processing or validation of the item XML being sent. It simply sends the exact XML
    /// given.
    /// </summary>
    public class PutRawItem : DlapCommand
    {
        #region Properties

        /// <summary>
        /// Gets or sets the item doc.
        /// </summary>
        /// <value>The item doc.</value>
        /// <remarks></remarks>
        public XDocument ItemDoc { get; set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="PutRawItem"/> is success.
        /// </summary>
        /// <remarks></remarks>
        public bool Success { get; private set; }

        /// <summary>
        /// The list of items that are going to be added/updated
        /// </summary>
        public List<Item> Items { get; protected set; }

        /// <summary>
        /// Any items that failed to process
        /// </summary>
        public List<ItemStorageFailure> Failures { get; protected set; }

        #endregion

        #region override DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/PutItems command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/PutItems</returns>
        /// <remarks></remarks>
        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Post,
                Parameters = new Dictionary<string, object>()
                {
                    { "cmd", "putitems" }
                }
            };

            var requestDoc = new XDocument();

            if (ItemDoc.Root != null && ItemDoc.Root.Name != "requests")
            {
                var rootElem = new XElement("requests");
                rootElem.Add(ItemDoc.Root);
                requestDoc.Add(rootElem);
            }
            else
            {
                requestDoc = ItemDoc;
            }
                
            request.AppendData(requestDoc.Root);

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/PutItems command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        /// <remarks></remarks>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
            {
                throw new DlapException(string.Format("PutRawItem failed with code: {0}.\nResponse was:\n{1}", response.Code, response.Message));
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
                        ItemId = Items[index].Id,
                        EntityId = Items[index].EntityId,
                        Reason = message
                    });
                }
                ++index;
            }

            Success = true;
        }

        #endregion
    }
}
