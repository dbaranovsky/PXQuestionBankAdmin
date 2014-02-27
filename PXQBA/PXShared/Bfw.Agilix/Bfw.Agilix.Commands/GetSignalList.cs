using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

using Bfw.Common;
using Bfw.Common.Collections;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/GetSignalList command, which gets 
    /// the list of events that have occured in DLAP since the last time it was called.
    /// </summary>
    public class GetSignalList : DlapCommand
    {
        #region Properties

        /// <summary>
        /// Gets or sets the search parameter.
        /// </summary>
        /// <value>The search parameter.</value>
        /// <remarks>Allows signals to be filtered.</remarks>
        public SignalSearch SearchParameter { get; set; }

        /// <summary>
        /// Gets or sets the signal list.
        /// </summary>
        /// <value>The signal list.</value>
        /// <remarks>List of signals that match the <see cref="SearchParameter" /></remarks>
        public List<Signal> SignalList { get; protected set; }

        #endregion

        #region override DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/GetSignalList command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/GetSignalList</returns>
        /// <remarks>Builds request</remarks>
        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object>() {
                    { "cmd", "GetSignalList" },
                    { "lastsignalid", SearchParameter.LastSignalId },
                    { "type", SearchParameter.SignalType}
                }
            };

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/GetSignalList command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        /// <remarks>Populates the <see cref="SignalList" /> property.</remarks>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
            {
                throw new DlapException(string.Format("GetDomainList command failed with code {0}", response.Code));
            }

            var data = new List<Signal>();

            if (response.ResponseXml != null)
            {
                Signal sig = null;
                var signals = response.ResponseXml.Descendants("signal");

                foreach (var signal in signals)
                {
                    sig = new Signal();
                    sig.ParseEntity(signal);

                    data.Add(sig);
                }
            }

            SignalList = data;
        }

        #endregion
    }
}
