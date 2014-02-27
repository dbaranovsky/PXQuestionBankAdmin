using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;
using System.Xml.Linq;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Provides an implementation of the http://dev.dlap.bfwpub.com/Docs/Command/Search command
    /// </summary>
    public class Search : DlapCommand
    {
        #region Properties

        /// <summary>
        /// Search filters to apply.
        /// </summary>
        public SolrSearchParameters SearchParameters { get; set; }

        /// <summary>
        /// Content returned based on the <see cref="SearchParameters" />.
        /// </summary>
        public XDocument SearchResults { get; set; }

        #endregion

        #region override DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/Search command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/Search </returns>
        /// <remarks></remarks>
        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object>() { { "cmd", "search" } }
            };

            ProcessFacetedParameters(request);

            if (!string.IsNullOrEmpty(SearchParameters.EntityId)) { request.Parameters["entityid"] = SearchParameters.EntityId; }
            if (!string.IsNullOrEmpty(SearchParameters.Query)) { request.Parameters["query"] = SearchParameters.Query; }
            if (!string.IsNullOrEmpty(SearchParameters.Fields)) { request.Parameters["fields"] = SearchParameters.Fields; }
            if (!SearchParameters.Hl) { request.Parameters["hl"] = SearchParameters.Hl; }
            if (!string.IsNullOrEmpty(SearchParameters.HlFields)) { request.Parameters["hl.fields"] = SearchParameters.HlFields; }
            if (SearchParameters.Rows > 0 || SearchParameters.Facet) { request.Parameters["rows"] = SearchParameters.Rows; }
            if (SearchParameters.Start > 0) { request.Parameters["start"] = SearchParameters.Start; }


            return request;
        }

        private void ProcessFacetedParameters(DlapRequest request)
        {
//Faceted Search
            if (SearchParameters.Facet)
            {
                string facetQuery = (string.IsNullOrEmpty(SearchParameters.Query)) ? "true" : SearchParameters.Query;

                Dictionary<string, string> facetParams = new Dictionary<string, string>();

                facetParams["facet"] = "true";
                facetParams["facet.method"] = "enum"; //TODO: enum vs fc, do performance testing
                if (!string.IsNullOrEmpty(SearchParameters.FacetFields))
                {
                    facetParams["facet.fields"] = SearchParameters.FacetFields;
                }
                

                CreateFacetParamString<int>(facetParams, "facet.limit", SearchParameters.FacetLimit);
                CreateFacetParamString<string>(facetParams, "facet.method", SearchParameters.FacetMethod);
                CreateFacetParamString<int>(facetParams, "facet.mincount", SearchParameters.FacetMinCount);
                CreateFacetParamString<bool>(facetParams, "facet.missing", SearchParameters.FacetMissing);
                CreateFacetParamString<int>(facetParams, "facet.offset", SearchParameters.FacetOffset);
                CreateFacetParamString<string>(facetParams, "facet.sort", SearchParameters.FacetSort);

                facetParams.ToList().ForEach(p=> request.Parameters.Add(p.Key,p.Value));

                if (string.IsNullOrWhiteSpace(SearchParameters.Query))
                {
                    if (String.IsNullOrWhiteSpace(SearchParameters.EntityId))
                    {
                        SearchParameters.Query = "*:*"; //query is a required field, fill with return all if missing    
                    }
                    else
                    {
                        SearchParameters.Query = "dlap_entityid:" + SearchParameters.EntityId; //query is a required field, fill with return all if missing    
                    }
                    
                }
            }
        }

        private void CreateFacetParamString<T>(Dictionary<string, string> facetParams, string paramName, FacetParam<T> param)
        {
            if(param == null || param.Value == null || string.IsNullOrWhiteSpace(param.Value.ToString()))
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(param.FieldNamePrefix))
            {
                facetParams[paramName] = param.Value.ToString();
            }
            else
            {
                facetParams[string.Format("f.{0}.{1}", param.FieldNamePrefix, paramName)] = param.Value.ToString();
            }
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/Search command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        /// <remarks></remarks>
        public override void ParseResponse(DlapResponse response)
        {
            if (null != response.ResponseXml)
            {
                SearchResults = response.ResponseXml;
            }
        }

        #endregion
    }
}
