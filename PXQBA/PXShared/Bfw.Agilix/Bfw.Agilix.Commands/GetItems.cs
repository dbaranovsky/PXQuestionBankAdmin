using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Common.Collections;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Provides and implementation of the GetItem, GetItemList, and GetManifestItem Agilix commands
    /// </summary>
    public class GetItems : DlapCommand
    {
        #region Data Members

        /// <summary>
        /// Parameters for the search results
        /// </summary>
        /// <value>
        /// The search parameters.
        /// </value>
        public ItemSearch SearchParameters { get; set; }

        /// <summary>
        /// List of items found as a result of the request
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public List<Item> Items { get; set; }


        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="GetItems"/> class.
        /// </summary>
        public GetItems()
        {
            SearchParameters = new ItemSearch()
            {
                Depth = 0
            };
        }

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/getitemlist command.
        /// </summary>
        /// <returns>
        /// Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/getitemlist
        /// </returns>
        public override Bfw.Agilix.Dlap.DlapRequest ToRequest()
        {
            if (string.IsNullOrEmpty(SearchParameters.EntityId))
                throw new DlapException("GetItems command requires an Entity Id");

            var request = new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object>() {
                    { "cmd", "getitemlist" },
                    { "entityid", SearchParameters.EntityId }
                }
            };

            string query = SearchParameters.Query;
            if (!string.IsNullOrEmpty(SearchParameters.ItemId))
            {
                request.Parameters["cmd"] = "getitem";
                request.Parameters["itemid"] = SearchParameters.ItemId;

                query = SearchParameters.Query;

                if (!string.IsNullOrEmpty(query))
                {
                    request.Parameters["cmd"] = "getitemlist";
                    request.Parameters.Remove("itemid");
                }
                else if (0 < SearchParameters.Depth)
                {
                    request.Parameters["cmd"] = "getitemlist";
                    request.Parameters.Remove("itemid");
                    var psearch = string.Format("/parent = '{0}'", SearchParameters.ItemId);

                    if (string.IsNullOrEmpty(query))
                    {
                        query = psearch;
                    }
                    else
                    {
                        query = string.Format("{0} AND {1}", query, psearch);
                    }
                }
            }
            if (!string.IsNullOrEmpty(query))
            {
                request.Parameters["query"] = query;
            }
            if (!string.IsNullOrEmpty(SearchParameters.AncestorId))
            {
                request.Parameters["cmd"] = "getmanifestitem";
                request.Parameters["itemid"] = SearchParameters.AncestorId;
            }

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/getitemlist command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        public override void ParseResponse(Bfw.Agilix.Dlap.DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
                throw new DlapException(string.Format("GetItems command failed with code {0}", response.Code));

            Item single = null;
            var results = new List<Item>();

            if (null == response.ResponseXml.Root)
            {
            }
            else if ("items" == response.ResponseXml.Root.Name)
            {
                foreach (var itemElm in response.ResponseXml.Root.Elements("item"))
                {
                    single = new Item();
                    single.ParseEntity(itemElm);
                    single.EntityId = SearchParameters.EntityId;
                    results.Add(single);
                }
            }
            else if (response.ResponseXml.Root.Element("root") != null)
            {
                foreach (var itemElm in response.ResponseXml.Descendants("root"))
                {
                    single = new Item();
                    single.ParseEntity(itemElm);
                    single.EntityId = SearchParameters.EntityId;
                    results.Add(single);
                }
            }
            else if ("item" == response.ResponseXml.Root.Name)
            {
                single = new Item();
                single.ParseEntity(response.ResponseXml.Root);
                single.EntityId = SearchParameters.EntityId;
                if (single.Id.ToLowerInvariant() == SearchParameters.ItemId.ToLowerInvariant())
                {
                    results.Add(single);
                }
            }
            else
            {
                throw new BadDlapResponseException(string.Format("GetItems command expected a response element of 'item' or 'items', but got {0} instead.", response.ResponseXml.Root.Name));
            }

            if (DlapItemType.None != SearchParameters.Type)
            {
                results = FilterByType(results, SearchParameters.Type);
            }

            Items = results;
        }

        /// <summary>
        /// Filters the list of items by type recursively
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private List<Item> FilterByType(List<Item> items, DlapItemType type)
        {
            var results = new List<Item>();

            if (!items.IsNullOrEmpty())
            {
                foreach (var item in items)
                {
                    if (type == item.Type)
                    {
                        results.Add(item);
                    }

                    var children = FilterByType(item.Children, type);
                    if (!children.IsNullOrEmpty())
                        results.AddRange(children);
                }
            }

            return results;
        }
    }
}
