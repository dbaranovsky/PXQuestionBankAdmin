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
    /// Provides and implementation of the Getforummessages Agilix commands
    /// </summary>
    public class GetMessages : DlapCommand
    {
        /// <summary>
        /// Gets or sets the search parameters.
        /// </summary>
        /// <value>
        /// The search parameters.
        /// </value>
        public MessageSearch SearchParameters { get; set; }

        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        /// <value>
        /// The messages.
        /// </value>
        public List<Message> Messages { get; protected set; }

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/getforummessages command.
        /// </summary>
        /// <returns>
        /// Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/getforummessages
        /// </returns>
        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object>() {
                    { "cmd", "getforummessages" },
                    { "entityid", SearchParameters.EntityId },
                    { "itemid", SearchParameters.DiscussionId }
                }
            };

            if (!string.IsNullOrEmpty(SearchParameters.MessageId))
            {
                request.Parameters["cmd"] = "getmessage";
                request.Parameters["messageid"] = SearchParameters.MessageId;
            }

            if (string.IsNullOrEmpty(SearchParameters.EntityId))
                throw new DlapException("GetMessages requires an enrollment id");

            if (string.IsNullOrEmpty(SearchParameters.DiscussionId))
                throw new DlapException("GetMessages requires a discussion item id");

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/getforummessages command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        public override void ParseResponse(DlapResponse response)
        {
            if (null != response.ResponseXml)
            {
                Messages = ParseXmlResponse(response.ResponseXml.Root);
            }
            else
            {
                Messages = new List<Message>() { ParseStreamResponse(response.ResponseStream) };
            }
        }

        /// <summary>
        /// Parses the stream response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        private Message ParseStreamResponse(Stream response)
        {
            var message = new Message();

            if (null != response)
            {
                int bsize = 4096;
                byte[] buff = new byte[bsize];
                int read = 0;
                using (var zipFile = new MemoryStream())
                {

                    while ((read = response.Read(buff, 0, bsize)) > 0)
                    {
                        zipFile.Write(buff, 0, read);
                    }

                    zipFile.Seek(0, SeekOrigin.Begin);

                    if (zipFile.Length > 0)
                    {
                        var zip = ZipFile.Read(zipFile);
                        var metaFile = new MemoryStream();
                        var notesFile = new MemoryStream();
                        var meta = zip["meta.xml"];
                        var notes = zip["message.htm"];

                        meta.Extract(metaFile);
                        notes.Extract(notesFile);

                        metaFile.Seek(0, SeekOrigin.Begin);
                        notesFile.Seek(0, SeekOrigin.Begin);

                        XElement element = null;

                        if (metaFile.Length > 0)
                            element = XElement.Load(new XmlTextReader(metaFile, XmlNodeType.Element, null));

                        StreamReader html = null;
                        if (notesFile.Length > 0)
                            html = new StreamReader(notesFile);

                        if (null != element)
                            message.ParseEntity(element);

                        if (null != html)
                            message.Body = html.ReadToEnd();

                        metaFile.Close();
                        notesFile.Close();
                    }
                }
            }

            return message;
        }

        /// <summary>
        /// Parses the XML response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        private List<Message> ParseXmlResponse(XElement response)
        {
            List<Message> list = null;

            if (null != response)
            {
                list = new List<Message>();

                var listElm = response.Descendants("message");

                if (null != listElm)
                {
                    Message single = null;
                    foreach (var aElm in listElm)
                    {
                        single = new Message();
                        single.ParseEntity(aElm);

                        list.Add(single);
                    }
                }
            }

            return list;
        }
    }
}
