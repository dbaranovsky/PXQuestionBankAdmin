using System;
using System.Collections.Generic;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.Agilix.Commands
{
    public class GetItemLinks : DlapCommand
    {
        /// <summary>
        /// ID of the course that owns the item.
        /// </summary>
        public string EntityId { get; set; }

        /// <summary>
        /// ID of the item. Omit itemid to return the list of courses that link to (are derivatives of) entityid.
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// Indicates whether or not to include items chained to the specified item through a derivative course relationship. If omitted, the default is true
        /// </summary>
        public bool? IncludeDerivatives { get; set; }

        /// <summary>
        /// Indicates whether or not to include items chained to the specified item through item links. If omitted, the default is true
        /// </summary>
        public bool? IncludeItemLinks { get; set; }

        /// <summary>
        /// Indicates whether or not to recursive through the item graph including all items that indirectly link to the specified item. If omitted, the default is true
        /// </summary>
        public bool? Recurse { get; set; }

        /// <summary>
        /// List of item links found as a result of the request
        /// </summary>
        public List<ItemLink> ItemLinks { get; set; }

        public override DlapRequest ToRequest()
        {
            if (String.IsNullOrEmpty(EntityId))
                throw new DlapException("GetItemLinks command requires an Entity Id");
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object>() {
                    { "cmd", "getitemlinks" },
                    { "entityid", EntityId }
                }
            };
            if (!String.IsNullOrEmpty(ItemId))
                request.Parameters.Add("itemid", ItemId);
            if (IncludeDerivatives.HasValue)
                request.Parameters.Add("derivatives", IncludeDerivatives.Value);
            if (IncludeItemLinks.HasValue)
                request.Parameters.Add("links", IncludeItemLinks.Value);
            if (Recurse.HasValue)
                request.Parameters.Add("recurse", Recurse.Value);
            return request;
        }

        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
                throw new DlapException(String.Format("GetItemLinks command failed with code {0}", response.Code));
            ItemLinks = new List<ItemLink>();
            var root = response.ResponseXml.Root;
            if (root != null)
            {
                var elements = root.Elements("item");
                foreach (var element in elements)
                {
                    var itemLink = new ItemLink();
                    itemLink.ParseEntity(element);
                    ItemLinks.Add(itemLink);
                }
            }
        }
    }
}
