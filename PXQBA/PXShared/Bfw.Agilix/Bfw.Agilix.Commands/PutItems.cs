using System.Collections.Generic;
using Bfw.Common.Collections;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/PutItems command
    /// </summary>
    public class PutItems : DlapCommand
    {
        /// <summary>
        /// Set the default partition size for breaking up putitems requests
        /// </summary>
        private const int PutItemsPartitionSize = 3;

        #region Propeties

        /// <summary>
        /// The list of items that are going to be added/updated
        /// </summary>
        public List<Item> Items { get; protected set; }

        /// <summary>
        /// Any items that failed to process
        /// </summary>
        public List<ItemStorageFailure> Failures { get; protected set; }


        
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PutItems"/> class.
        /// </summary>
        /// <remarks></remarks>
        public PutItems()
        {
            Items = new List<Item>();
            Failures = new List<ItemStorageFailure>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds an item to send to DLAP.
        /// </summary>
        /// <param name="item">Item to send to DLAP.</param>
        public void Add(IItem item)
        {
            Items.Add(item.AsItem());
        }

        /// <summary>
        /// Adds the specified items to send to DLAP
        /// </summary>
        /// <param name="items">The items to send to DLAP</param>
        /// <remarks></remarks>
        public void Add(IEnumerable<Item> items)
        {
            Items.AddRange(items.Map(item => item.AsItem()));
        }

        /// <summary>
        /// Clears the items to send to DLAP.
        /// </summary>
        /// <remarks>Clears the <see cref="Items" /> collection.</remarks>
        public void Clear()
        {
            Items.Clear();
        }

        /// <summary>
        /// Ensures proper tree formation by marking every item in the children collection
        /// so that their ParentId == root.Id.  This method is applied recursively to each
        /// child.
        /// </summary>
        /// <param name="root">root item in the tree</param>
        /// <param name="children">all immidiate child items in root.Children</param>
        protected void BuildItemTree(Item root, List<Item> children)
        {
            if (children.IsNullOrEmpty())
                return;

            foreach (var child in children)
            {
                child.ParentId = root.Id;
                BuildItemTree(child, child.Children);
            }
        }

        #endregion

        #region override DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/PutItems command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/PutItems</returns>
        /// <remarks></remarks>
        public override DlapRequest ToRequest()
        {
            if (Items.IsNullOrEmpty())
                throw new DlapException("Cannot create a PutItems request if there are not items in the Items collection");

            var request = new DlapRequest()
            {
                Type = DlapRequestType.Post,
                Mode = DlapRequestMode.Batch,
                Parameters = new Dictionary<string, object>() { { "cmd", "putitems" } }
            };

            foreach (var item in Items)
            {
                BuildItemTree(item, item.Children);
                request.AppendData(item.ToEntity());
            }

            return request;
        }


        /// <summary>
        /// Builds a collection of DlapRequests by the http://dev.dlap.bfwpub.com/Docs/Command/PutItems command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/PutItems</returns>
        /// <remarks></remarks>
        public override IEnumerable<DlapRequest> ToRequestAsync()
        {
            if (Items.IsNullOrEmpty())
                throw new DlapException("Cannot create a PutItems request if there are not items in the Items collection");

            var requests = new List<DlapRequest>();


            foreach (var items in Items.Partition(PutItemsPartitionSize))
            {
                var request = new DlapRequest()
                {
                    Type = DlapRequestType.Post,
                    Mode = DlapRequestMode.Batch,
                    Parameters = new Dictionary<string, object>() { { "cmd", "putitems" } }
                };
                foreach (var item in items)
                {
                    BuildItemTree(item, item.Children);
                    request.AppendData(item.ToEntity());
                }
                requests.Add(request);
            }

            return requests;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/PutItems command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        /// <remarks></remarks>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
                throw new DlapException(string.Format("PutItems request failed with response code {0}", response.Code));
            
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
        /// <summary>
        /// Parses the asynchronous response of the http://dev.dlap.bfwpub.com/Docs/Command/PutItems command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        /// <remarks></remarks>
        public override void ParseResponseAsync(IEnumerable<DlapResponse> response)
        {
            foreach (var dlapResponse in response)
            {
                this.ParseResponse(dlapResponse);
            }
        }

        #endregion
    }
}
