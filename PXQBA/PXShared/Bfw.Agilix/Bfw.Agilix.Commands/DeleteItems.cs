using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Bfw.Common.Collections;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Implementation of the http://dev.dlap.bfwpub.com/Docs/Command/DeleteItems DLAP command.
    /// </summary>
    public class DeleteItems : DlapCommand
    {
        #region Properties

        /// <summary>
        /// The list of items that are going to be deleted.
        /// </summary>
        public List<Item> Items { get; set; }

        /// <summary>
        /// Any items that failed to process.
        /// </summary>
        public List<ItemStorageFailure> Failures { get; protected set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a default DeleteItems Command.
        /// </summary>
        public DeleteItems()
        {
            Items = new List<Item>();
            Failures = new List<ItemStorageFailure>();
        }

        #endregion

        #region Overrides from DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/ command.
        /// </summary>
        /// <returns>
        /// Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/.
        /// </returns>
        public override DlapRequest ToRequest()
        {
            if (Items.IsNullOrEmpty())
            {
                throw new DlapException("Cannot create a DeleteItems request if there are not items in the Items collection");
            }

            var request = new DlapRequest()
            {
                Type = DlapRequestType.Post,
                Mode = DlapRequestMode.Batch,
                Parameters = new Dictionary<string, object>() { { "cmd", "deleteitems" } }
            };

            foreach (var item in Items)
            {
                request.AppendData(item.ToEntity());
            }

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/ command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse.</param>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
            {
                throw new DlapException(string.Format("DeleteItems request failed with response code {0}", response.Code));
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
        }

        #endregion
    }
}
