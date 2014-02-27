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

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/PutMessage command
    /// </summary>
    public class PutMessage : DlapCommand
    {
        #region Properties

        /// <summary>
        /// Gets or sets the entity id.
        /// </summary>
        /// <value>The entity id.</value>
        /// <remarks>Id of the entity the message is being sent to.</remarks>
        public string EntityId { get; set; }

        /// <summary>
        /// Gets or sets the discussion id.
        /// </summary>
        /// <value>The discussion id.</value>
        /// <remarks>Id of the discussion the message is a part of.</remarks>
        public string DiscussionId { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        /// <remarks>Contents of the message beign added to the dicsussion.</remarks>
        public Message Message { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Builds the zip file that represents the data to post to the
        /// Agilix server
        /// </summary>
        /// <returns>Stream containing the ZIPed message.</returns>
        private Stream BuildRequestStream()
        {
            var metaFile = new MemoryStream();
            var msgFile = new MemoryStream();
            var zipFile = new MemoryStream();

            var metaFilefSw = new StreamWriter(metaFile);
            metaFilefSw.Write(Message.ToEntity().ToString());
            metaFilefSw.Flush();
            metaFile.Seek(0, SeekOrigin.Begin);

            var msgFileSw = new StreamWriter(msgFile);
            msgFileSw.Write(Message.Body);
            msgFileSw.Flush();
            msgFile.Seek(0, SeekOrigin.Begin);

            var zip = new ZipFile();
            zip.AddEntry("meta.xml", metaFile);
            zip.AddEntry("message.htm", msgFile);

            zip.Save(zipFile);
            zipFile.Flush();
            zipFile.Seek(0, SeekOrigin.Begin);

            return zipFile;
        }

        #endregion

        #region override DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/PutMessage command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/PutMessage</returns>
        /// <remarks></remarks>
        public override DlapRequest ToRequest()
        {
            if (string.IsNullOrEmpty(Message.Id))
            {
                Message.Id = string.Format("{0}.zip", Guid.NewGuid().ToString("N"));
                Message.Version = "0";
            }

            var request = new DlapRequest()
            {
                Type = DlapRequestType.Post,
                Parameters = new Dictionary<string, object>() {
                    { "cmd", "putmessage" },
                    { "entityid", EntityId },
                    { "itemid", DiscussionId },
                    { "messageid", Message.Id },
                    { "version", Message.Version }
                }
            };

            request.AppendData(BuildRequestStream());            

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/PutMessage command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        /// <remarks></remarks>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
                throw new DlapException(string.Format("PutMessage failed with code: {0}", response.Code));

            var msg = new Message();
            msg.ParseEntity(response.ResponseXml.Root);

            Message.Version = msg.Version;
        }

        #endregion
    }
}
