using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Agilix.Dlap.Session
{
    /// <summary>
    /// <para>The structure of the DLAP API is command based. That is, each command has a specific request and
    /// response format.</para>
    /// <para>The DlapCommand class represents this abstraction. Subclasses are responsible for generating the
    /// <see cref="DlapRequest" /> and parsing the resulting <see cref="DlapResponse" /> for the command the represent.</para>
    /// </summary>
    public abstract class DlapCommand : IDlapResponseParser, IDlapRequestTransformer
    {
        #region Properties

        /// <summary>
        /// Order of the command when inside a batch
        /// </summary>
        public int BatchOrder { get; set; }

        /// <summary>
        /// If true the command will be run asynchronously.
        /// </summary>
        public bool RunAsync { get; set; }

        #endregion

        #region IDlapRequestTransformer Members

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/ command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/.</returns>
        public abstract DlapRequest ToRequest();

        #endregion

        #region IDlapResponseParser Members

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/ command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse" /> to parse.</param>
        public abstract void ParseResponse(DlapResponse response);

        #endregion

        #region Methods

        /// <summary>
        /// Allows commands to return one or more DlapRequests that can be 
        /// run asynchronously. This is used primary by batch commands in order
        /// to get better throughput.
        /// 
        /// The default implementation simply returns a collection containing a single
        /// element, which is the result of calling ToRequest().
        /// </summary>
        /// <returns>Collection of DlapRequests.</returns>
        public virtual IEnumerable<DlapRequest> ToRequestAsync()
        {
            return new List<DlapRequest> { ToRequest() };
        }

        public virtual void ParseResponseAsync(IEnumerable<DlapResponse> responses)
        {
            foreach (var response in responses)
            {
                ParseResponse(response);
            }
        }

        #endregion
    }
}
