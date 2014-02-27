using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.Dlap;

namespace Bfw.Agilix.Commands
{
    public class GetDomainList : DlapCommand
    {
        #region Data Members

        /// <summary>
        /// List of domain retrieved.
        /// </summary>
        public IEnumerable<Domain> Domains { get; set; }

        public Domain SearchParameters { get; set; }

        #endregion

        public override DlapRequest ToRequest()
        {
            var parameters = new Dictionary<string, object>();
            parameters["cmd"] = "getdomainlist";

            if (!String.IsNullOrEmpty(SearchParameters.Id))
            {
                parameters.Add("domainid", SearchParameters.Id);
            }
            if (!String.IsNullOrEmpty(SearchParameters.Name))
            {
                parameters.Add("name", SearchParameters.Name);
            }
            if (!String.IsNullOrEmpty(SearchParameters.Userspace))
            {
                parameters.Add("userspace", SearchParameters.Userspace);
            }
            if (!String.IsNullOrEmpty(SearchParameters.Reference))
            {
                parameters.Add("reference", SearchParameters.Reference);
            }

            return new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = parameters
            };

           
        }

        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
            {
                throw new DlapException(string.Format("GetDomainList command failed with code {0}", response.Code));
            }

            var data = new List<Domain>();
            Domain dom = null;
            var domains = response.ResponseXml.Descendants("domain");

            foreach (var domain in domains)
            {
                dom = new Domain();
                dom.ParseEntity(domain);

                data.Add(dom);
            }

            Domains = data;
        }
    }
}
