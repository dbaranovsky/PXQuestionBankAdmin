using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Batches a series of commands so that they can be sent to DLAP in one request.
    /// </summary>
    public class Batch : DlapCommand
    {
        #region Properties

        /// <summary>
        /// The batch order of the next command added to the batch.
        /// </summary>
        protected int Order { get; set; }

        /// <summary>
        /// The commands to be run in the batch indexed by tag.
        /// </summary>
        protected Dictionary<string, DlapCommand> CommandSet { get; set; }

        /// <summary>
        /// Commands in the batch ordered by DlapCommand.BatchOrder ascending.
        /// </summary>
        public IEnumerable<DlapCommand> Commands
        {
            get
            {
                return (from c in CommandSet.Values orderby c.BatchOrder select c).ToList();
            }
        }

        /// <summary>
        /// Provides access to a specific command in the batch using a user friends tag.
        /// </summary>
        /// <param name="tag">Uniquely identifies the command inside of the batch.</param>
        /// <returns>Command represented by tag</returns>
        protected DlapCommand this[string tag]
        {
            get
            {
                return CommandSet[tag];
            }

            set
            {
                var cmd = value;
                if (!CommandSet.ContainsKey(tag))
                {
                    cmd.BatchOrder = Order;
                    ++Order;
                }
                else
                {
                    cmd.BatchOrder = CommandSet[tag].BatchOrder;
                }

                CommandSet[tag] = cmd;
            }
        }

        /// <summary>
        /// Provides access to a specific command in the batch using the command's BatchOrder.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        protected DlapCommand this[int order]
        {
            get
            {
                return (from c in CommandSet.Values where c.BatchOrder == order select c).FirstOrDefault();
            }
        }

        /// <summary>
        /// XML document containing the raw response from DLAP.
        /// </summary>
        public XDocument ResponseXml { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes and empty batch with Order set to zero.
        /// </summary>
        public Batch()
        {
            Order = 0;
            CommandSet = new Dictionary<string, DlapCommand>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a DlapCommand to the batch. DlapCommand.BatchOrder will be set to
        /// Batch.Order and a Guid will be generated as the tag value.
        /// </summary>
        /// <param name="cmd">The DlapCommand to add to the batch.</param>
        /// <returns>The string representing the tag cmd was stored under.</returns>
        public string Add(DlapCommand cmd)
        {
            var tag = Guid.NewGuid().ToString("N");
            Add(tag, cmd);

            return tag;
        }

        /// <summary>
        /// Adds a DlapCommand to the batch. DlapCommand.BatchOrder will be set to
        /// Batch.Order.  The tag provided can be used to access the DlapCommand by
        /// name.
        /// </summary>
        /// <param name="tag">Tag client wants to use to retrieve the DlapCommand.</param>
        /// <param name="cmd">DlapCommand to add to the batch.</param>
        public void Add(string tag, DlapCommand cmd)
        {
            this[tag] = cmd;
        }

        /// <summary>
        /// Gets the DlapCommand stored under the given tag value.
        /// </summary>
        /// <typeparam name="TEntity">Subclass of DlapCommand.</typeparam>
        /// <param name="tag">String that can be used to load the DlapCommand.</param>
        /// <returns>DlapCommand that was stored using tag.</returns>
        public TEntity CommandAs<TEntity>(string tag) where TEntity : DlapCommand
        {
            return this[tag] as TEntity;
        }

        /// <summary>
        /// Gets the DlapCommand stored at the given position.
        /// </summary>
        /// <typeparam name="TEntity">Subclass of DlapCommand.</typeparam>
        /// <param name="order">Order of the DlapCommand in the batch.</param>
        /// <returns>DlapCommand that was located at the specified position.</returns>
        public TEntity CommandAs<TEntity>(int order) where TEntity : DlapCommand
        {
            return this[order] as TEntity;
        }

        #endregion

        #region override DlapCommand

        /// <summary>
        /// Builds a DlapRequest conforming to http://dev.dlap.bfwpub.com/Docs/Concept/CommandUsage.
        /// </summary>
        /// <returns>DlapRequest that executes an entire batch of commands</returns>
        public override DlapRequest ToRequest()
        {
            XDocument batch = new XDocument(new XElement("batch"));
            var cmds = from c in CommandSet.Values
                       orderby c.BatchOrder ascending
                       select c;

            foreach (var cmd in cmds)
            {
                var request = cmd.ToRequest();
                var requestBody = request.GetXmlRequestBody();

                var wrappedRequest = new XElement("request");

                if (requestBody != null)
                {
                    wrappedRequest.Add(requestBody.Root);
                }

                if (request.Parameters != null && request.Parameters.Count > 0)
                {
                    foreach (var parameter in request.Parameters.Where(parameter => parameter.Value!=null))
                    {
                    	wrappedRequest.Add(new XAttribute(parameter.Key, parameter.Value));
                    }
                }

                batch.Root.Add(wrappedRequest);
            }

            var batchRequest = new DlapRequest()
            {
                Mode = DlapRequestMode.Batch,
                Type = DlapRequestType.Post,
                SuppressWrapper = true,
            };

            batchRequest.AppendData(batch.Root);

            return batchRequest;
        }

        /// <summary>
        /// Returns a collection of DlapRequest object build from the CommandSet 
        /// of the batch.
        /// </summary>
        /// <returns>collection of DlapRequests</returns>
        public override IEnumerable<DlapRequest> ToRequestAsync()
        {
            
            var requests = new List<DlapRequest>();
            foreach (var request in CommandSet.Values)
            {
                //TODO: Support async requests in async batches
                //if(request.RunAsync)
                //    requests.AddRange(request.ToRequestAsync());
                //else
                requests.Add(request.ToRequest());
            }
              
            return requests;
        }

        /// <summary>
        /// Calls DlapCommand.ParseResponse for each command in the batch passing it
        /// the corresponding DlapResponse.
        /// </summary>
        /// <param name="response">DlapResponse received from DLAP.</param>
        public override void ParseResponse(DlapResponse response)
        {
            ResponseXml = response.ResponseXml;

            int order = 0;
            foreach (var resp in response.Batch)
            {
                this[order].ParseResponse(resp);
                ++order;
            }
        }

        /// <summary>
        /// Parses the asynchronous response of each command from the batch.
        /// </summary>
        /// <param name="responses">Set of responses from the batch.</param>
        public override void ParseResponseAsync(IEnumerable<DlapResponse> responses)
        {
            int order = 0;
            foreach (var resp in responses)
            {
                this[order].ParseResponse(resp);
                ++order;
            }
        }

        #endregion
    }
}
