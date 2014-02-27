using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.DataContracts;
using System.Xml.Linq;

namespace Bfw.Agilix.Commands.Tests
{
    /// <summary>
    /// Summary description for SearchTests
    /// </summary>
    [TestClass]
    public class SearchTests
    {
        public SearchTests()
        {

        }

        [TestCategory("SearchTests"), TestMethod]
        public void SearchTests_HasSearchParameters_RequestOk()
        {
            SolrSearchParameters parameters = new SolrSearchParameters();
            parameters.EntityId = "someEntity";
            parameters.Query = "meta-creationdate:(\"January 1, 2010\") ";
            parameters.Fields = "someFields";
            parameters.Hl = false;
            parameters.HlFields = "someHlFields";
            parameters.Rows = 1;
            parameters.Start = 1;
            parameters.Facet = true;
            parameters.FacetFields = "someFacetFields";
            parameters.FacetLimit = new FacetParam<int> { Value = 1 };
            parameters.FacetMinCount = new FacetParam<int> { Value = 1,FieldNamePrefix = "fn" };

            Search search = new Search { SearchParameters = parameters };
            DlapRequest request = search.ToRequest();
            Assert.AreEqual(request.Parameters["cmd"], "search");

        }

        [TestCategory("SearchTests"), TestMethod]
        public void SearchTests_HasSearchParametersNoQuery_RequestOk()
        {
            SolrSearchParameters parameters = new SolrSearchParameters();
            parameters.EntityId = "someEntity";
            parameters.Fields = "someFields";
            parameters.Hl = false;
            parameters.HlFields = "someHlFields";
            parameters.Start = 1;
            parameters.Facet = true;
            parameters.FacetFields = "someFacetFields";
            parameters.FacetLimit = new FacetParam<int> { Value = 1 };
            parameters.FacetMinCount = new FacetParam<int> { Value = 1, FieldNamePrefix = "fn" };

            Search search = new Search { SearchParameters = parameters };
            DlapRequest request = search.ToRequest();
            Assert.AreEqual(request.Parameters["cmd"], "search");

        }

        [TestCategory("SearchTests"), TestMethod]
        public void SearchTests_HasSearchParametersNoQueryNoEntityId_RequestOk()
        {
            SolrSearchParameters parameters = new SolrSearchParameters();
            parameters.Fields = "someFields";
            parameters.Hl = false;
            parameters.HlFields = "someHlFields";
            parameters.Start = 1;
            parameters.Facet = true;
            parameters.FacetFields = "someFacetFields";
            parameters.FacetLimit = new FacetParam<int> { Value = 1 };
            parameters.FacetMinCount = new FacetParam<int> { Value = 1, FieldNamePrefix = "fn" };

            Search search = new Search { SearchParameters = parameters };
            DlapRequest request = search.ToRequest();
            Assert.AreEqual(request.Parameters["cmd"], "search");

        }

        [TestCategory("SearchTests"), TestMethod]
        public void SearchTests_SearchResult_ResponseOk()
        {

            string responseString = "<response code=\"OK\"><results_>"+
                                    "<result numFound=\"1\" start=\"0\" maxScore=\"0.4641946\">"+
                                    "<doc entityid=\"268973\" class=\"item\" itemid=\"190563C32FA94A2996C78653E6F2551B\">"+
                                    "<float name=\"score\">0.4641946</float><str name=\"dlap_class\">item</str>"+
                                    "<str name=\"dlap_id\">268973|I|190563C32FA94A2996C78653E6F2551B</str><str name=\"dlap_title\">About the administrator dashboard</str><str name=\"meta-abstract\">A document about using the administrator dashboard to evaluate teacher performance.</str>"+
                                    "<str name=\"meta-author\">Jeff Gammon</str></doc><doc><float name=\"score\">0.3755317</float> <str name=\"dlap_class\">item</str><str name=\"dlap_id\">268973|I|SODGD</str><str name=\"dlap_title\">About the instructor dashboard</str>"+
                                    "<str name=\"meta-abstract\">Demonstrates how instructors can use the instructor dashboard to evaluate student performance.</str>         <str name=\"meta-author\">Bernd Helzer</str>       </doc>     </result>     <lst name=\"highlighting\">       "+
                                    "<lst name=\"268973|I|190563C32FA94A2996C78653E6F2551B\">         <arr>           <str name=\"meta-abstract\">A document about using the administrator <em>dashboard</em> to evaluate teacher performance.</str>         </arr>       </lst>"+
                                    "<lst name=\"268973|I|SODGD\">         <arr>           <str name=\"meta-abstract\">Demonstrates how instructors can use the instructor <em>dashboard</em> to evaluate student performance."+
                                    "</str></arr></lst></lst></results_> </response>";

            DlapResponse response = new DlapResponse(XDocument.Parse(responseString));
            Search search = new Search();
            search.ParseResponse(response);
        }

    }
}
