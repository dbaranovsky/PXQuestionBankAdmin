using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Web.Mvc;
using Bfw.PX.Biz.DataContracts;
using Bdc = Bfw.PX.Biz.DataContracts;
using Adc = Bfw.Agilix.DataContracts;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.Commands;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.Common.Logging;

namespace Bfw.PX.Biz.Direct.Services
{
    /// <summary>
    /// Implements ISearchActions using direct connection to DLAP.
    /// </summary>
    public class SearchActions : ISearchActions
    {
        #region Properties

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected IBusinessContext Context { get; set; }

        /// <summary>
        /// Gets or sets the session manager.
        /// </summary>
        /// <value>
        /// The session manager.
        /// </value>
        protected ISessionManager SessionManager { get; set; }

        protected IContentActions ContentActions { get; set; }

        protected IQuestionActions QuestionActions { get; set; }

        //determines if searches should be done against product course
        bool _doProductSearch = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["FaceplateSearchAgainstProductCourse"]);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchActions"/> class.
        /// </summary>
        /// <param name="ctx">The IBusinessContext implementation.</param>
        /// <param name="sessionManager">The session manager.</param>
        /// <param name="contentActions"></param>
        /// <param name="questionActions"></param>
        public SearchActions(IBusinessContext ctx, ISessionManager sessionManager, IContentActions contentActions, IQuestionActions questionActions)
        {
            Context = ctx;
            SessionManager = sessionManager;
            ContentActions = contentActions;
            QuestionActions = questionActions;
        }

        #endregion

        #region ISearchActions Members

        /// <summary>
        /// Determines whether the course is indexed by performing a generic search.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if any results are returned; otherwise, <c>false</c>.
        /// </returns>
        public Boolean IsIndexed()
        {
            using (Context.Tracer.DoTrace("SearchActions.IsIndexed"))
            {
                Bdc.SearchQuery query = new Bdc.SearchQuery() { IncludeWords = "*" };
                Bdc.SearchResultSet srs = DoSearch(query, null, false);

                int num1 = 0;
                if (srs != null)
                {
                    bool res = int.TryParse(srs.numFound, out num1);
                    if (res) { return (num1 > 0); }
                }
            }

            return false;
        }

        /// <summary>
        /// Lists results that match the specified <see cref="SearchQuery" /> criteria.
        /// </summary>
        /// <returns></returns>
        public Bdc.SearchResultSet DoSearch(Bdc.SearchQuery query)
        {

            return DoSearch(query, null, false);
        }
        /// <summary>
        /// Searches for questions
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IEnumerable<Question> DoQuestionSearch(Bdc.SearchQuery query, string entityId, out int numfound)
        {
            string userId = Context.CurrentUser.Id;

            this.DoProductSearch(false);
            // check if it's meta search
            if (query.IncludeWords.Contains(":"))
            {
                var words = query.IncludeWords.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

                if (words.Length > 1)
                {
                    query.ExactQuery = query.IncludeWords;
                    query.IncludeWords = string.Empty;
                }
            }

            if (!query.ExactQuery.IsNullOrEmpty())
            {
                string queryForModification = "(userCreated:(\"true\") OR publisherEdited:(\"true\"))";
                string queryForUserInfo = string.Format(" AND (createdBy:(\"{0}\")  OR modifiedBy:(\"{0}\"))", userId);
                string queryField = string.Concat(queryForModification, queryForUserInfo);

                switch (query.ExactQuery.Trim().ToUpper())
                {

                    case "IN_USE":
                        query.ExactQuery = string.Concat(queryField, " AND NOT totalUsed:(0)");
                        break;
                    case "NOT_IN_USE":
                        query.ExactQuery = string.Concat(queryField, " AND totalUsed:(0)");
                        break;
                }
            }
            query.EntityId = entityId;
            query.ClassType = "question";
            var results = new SearchResultSet();
            try
            {
                results = DoSearch(query, null, false);    
            }
            catch (Exception)
            {
                //Fix:- If Solr search throws error, show the user
                // as 0 results found.
                results = new SearchResultSet {numFound = "0"};
            }

            
            var questions = results.docs.Map(q => q.ToQuestion()).ToList();

            int.TryParse(results.numFound, out numfound);

            return questions;

        }


        /// <summary>
        /// Flag to determines if searches should be done against product course
        /// </summary>
        /// <returns>
        ///   <c>true</c> if searches should be done against product course; otherwise, <c>false</c>.
        /// </returns>
        public void DoProductSearch(bool flag)
        {
            _doProductSearch = flag;
        }

        /// <summary>
        /// Lists results that match the specifed <see cref="SearchQuery"/> criteria.
        /// </summary>
        /// <param name="query">The search criteria.</param>
        /// <param name="urlHelper">A UrlHelper object to resolve internal item urls.</param>
        /// <param name="postProcess">if set to <c>true</c> runs post processing on the initial results.</param>
        /// <returns></returns>
        public Bdc.SearchResultSet DoSearch(Bdc.SearchQuery query, UrlHelper urlHelper, Boolean postProcess)
        {
            Adc.SolrSearchParameters solrParam = new Adc.SolrSearchParameters();
            Bdc.SearchResultSet srs = new Bdc.SearchResultSet();
            List<XDocument> resultsDocs = new List<XDocument>();

            using (Context.Tracer.StartTrace("Search.DoSearch"))
            {
                if (_doProductSearch)
                {
                    query.EntityId = Context.Course.ProductCourseId;
                }
                Bdc.SearchParameters p1 = BuildSearchParameters(query);

                solrParam = TypeConversion.ConvertType<Bdc.SearchParameters, Adc.SolrSearchParameters>(p1, null);

                var cmd = new Search()
                {
                    SearchParameters = solrParam
                };
                //do server-side paging for more than 25 rows
                if (cmd.SearchParameters.Rows > 25)
                {
                    int numRows = cmd.SearchParameters.Rows;
                    cmd.SearchParameters.Rows = 25;
                    for (int i = cmd.SearchParameters.Start; i <= numRows; i += 25)
                    {
                        cmd.SearchParameters.Start = i;
                        ExecuteAsAdmin(cmd);
                        resultsDocs.Add(cmd.SearchResults);

                    }
                }
                else
                {
                    ExecuteAsAdmin(cmd);
                    resultsDocs.Add(cmd.SearchResults);
                }

                srs.Query = query;
                if (!query.MetaCategories.IsNullOrEmpty())
                {
                    srs.metaValue = string.Join("", query.MetaCategories.ToArray());
                }
                foreach (var resultsDoc in resultsDocs)
                {
                    var resultsElement = resultsDoc.Element("results");
                    if (null != resultsElement)
                    {
                        var resultElements = resultsElement.Elements("result");
                        foreach (XElement resultElement in resultElements)
                        {
                            srs.ParseEntity(resultElement);
                        }
                        try
                        {
                            var lstFacetCounts =
                                resultsElement.Elements("lst").First(e => e.Attribute("name").Value == "facet_counts");
                            srs.ParseFacetResults(lstFacetCounts);
                        }
                        catch (Exception)
                        {


                        }


                    }
                }
                
            }

            if (postProcess)
            {
                srs = PostProcess(srs, urlHelper);
            }
            return srs;
        }

        /// <summary>
        /// Executes command as admin if the entity is not equal to context entity
        /// </summary>
        /// <param name="cmd"></param>
        private void ExecuteAsAdmin(Search cmd)
        {
            using (Context.Tracer.DoTrace("SearchActions.ExecuteAsAdmin"))
            {
                if (cmd.SearchParameters.EntityId != Context.EntityId)
                {
                    SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                }
                else
                {
                    SessionManager.CurrentSession.Execute(cmd);
                }
            }
                
        }

        /// <summary>
        /// Performs post processing tasks on a search result set.
        /// Excludes template items for result set.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <param name="urlHelper">URL helper class to resolve internal references.</param>
        /// <returns></returns>
        public Bdc.SearchResultSet PostProcess(Bdc.SearchResultSet results, UrlHelper urlHelper)
        {
            List<string> excludes = new List<string>();
            using (Context.Tracer.DoTrace("SearchActions.PostProcess"))
            {
                foreach (Bdc.SearchResultDoc doc in results.docs)
                {
                    doc.url = GetSearchResultUrl(doc, urlHelper);
                    if (doc.dlap_id.Contains("TMP_")) excludes.Add(doc.dlap_id);
                }

                //Remove template files from results
                results.docs.RemoveAll(item => excludes.Contains(item.dlap_id));
            }
            return results;
        }

        /// <summary>
        /// Gets the search result URL.
        /// </summary>
        /// <param name="doc">The search result item to update.</param>
        /// <param name="urlHelper">URL helper class to resolve internal references.</param>
        /// <returns></returns>
        private string GetSearchResultUrl(Bdc.SearchResultDoc doc, UrlHelper urlHelper)
        {
            if (urlHelper == null) return string.Empty;
            var dlapType = Convert.ToInt32(doc.dlap_itemtype);
            switch (dlapType)
            {
                case (int)Bdc.DlapType.Assignment:
                    return urlHelper.Action("Index", "Content", new { id = doc.itemid });
                case (int)Bdc.DlapType.AssetLink:
                    return urlHelper.Action("Index", "Content", new { id = doc.itemid });
                case (int)Bdc.DlapType.Comment:
                    return urlHelper.Action("Index", "Content", new { id = doc.itemid });
                case (int)Bdc.DlapType.Discussion:
                    return urlHelper.Action("Index", "Content", new { id = doc.itemid });
                case (int)Bdc.DlapType.Resource:
                    return urlHelper.Action("Index", "Content", new { id = doc.itemid });
                default:
                    break;
            }
            return String.Empty;
        }

        /// <summary>
        /// Builds up a SOLR syntax string based on a <see cref="SearchQuery" /> object.
        /// </summary>
        /// <param name="query">The query to build the query string from.</param>
        /// <returns></returns>
        public Bdc.SearchParameters BuildSearchParameters(Bdc.SearchQuery query)
        {
            String searchAllField = "text";
            String tempString = String.Empty;
            List<string> searchFields = new List<string>();
            Bdc.SearchParameters p1 = new Bdc.SearchParameters();
            using (Context.Tracer.DoTrace("SearchActions.BuildSearchParameters"))
            {
                p1.EntityId = !string.IsNullOrWhiteSpace(query.EntityId) ? query.EntityId : Context.EntityId;
                p1.Query = "";
                p1.Start = (query.Start > 0) ? query.Start : 0;
                if (!query.IsFaceted)
                {
                    p1.Rows = (query.Rows > 0) ? query.Rows : 5;
                }
                else
                { //if facet query, don't return any rows by default
                    p1.Rows = (query.Rows > 0) ? query.Rows : 0;
                }

                if (String.IsNullOrEmpty(query.IncludeFields) && String.IsNullOrEmpty(query.MetaIncludeFields))
                {
                    searchFields.Add("dlap_" + searchAllField); ;
                }
                else
                {
                    if (!string.IsNullOrEmpty(query.IncludeFields))
                    {
                        query.IncludeFields.Split(',').ToList().ForEach(type => searchFields.Add("dlap_" + type));
                    }
                    if (!string.IsNullOrEmpty(query.MetaIncludeFields))
                    {
                        query.MetaIncludeFields.Split(',').ToList().ForEach(f => searchFields.Add(f));
                    }

                }

                if (!String.IsNullOrEmpty(query.IncludeWords))
                {
                    if (query.IncludeWords == "*")
                    {
                        p1.Query = "*:*";
                        return p1;
                    }

                    tempString = String.Empty;
                    foreach (string field in searchFields)
                    {
                        if (!String.IsNullOrEmpty(tempString)) tempString += " OR ";
                        var searchString = query.IncludeWords.ToSearchString();
                        if (!String.IsNullOrEmpty(searchString)) tempString += field + ":" + searchString;
                    }
                    if (!String.IsNullOrEmpty(p1.Query)) p1.Query += " AND ";
                    p1.Query += tempString;
                    tempString = String.Empty;
                }

                if (!String.IsNullOrEmpty(query.ClassType))
                {
                    tempString = query.ClassType;
                    if (!String.IsNullOrEmpty(p1.Query)) p1.Query += " AND ";
                    p1.Query += "(" + "dlap_class:" + tempString + ")";
                    tempString = String.Empty;
                }

                if (!query.MetaCategories.IsNullOrEmpty())
                {
                    tempString = String.Empty;
                    List<string> catList = query.MetaCategories.ToList();
                    foreach (string cat in catList)
                    {
                        if (!String.IsNullOrEmpty(tempString)) tempString += " OR ";
                        tempString += "meta-bfw_searchcategory:" + cat;
                    }
                    if (!String.IsNullOrEmpty(p1.Query)) p1.Query += " AND ";
                    p1.Query += "(" + tempString + ")";
                    tempString = String.Empty;
                }

                if (!String.IsNullOrEmpty(query.ContentTypes))
                {
                    if (!String.IsNullOrEmpty(p1.Query)) p1.Query += " AND ";
                    var types = query.ContentTypes.Split(',').ToList();
                    var dType = (Bdc.DlapType)Enum.Parse(typeof(Bdc.DlapType), types[0], true);
                    p1.Query += "(dlap_itemtype:" + ((int)dType).ToString();
                    foreach (string type in types)
                    {
                        dType = (Bdc.DlapType)Enum.Parse(typeof(Bdc.DlapType), type, true);
                        p1.Query += " OR dlap_itemtype:" + ((int)dType).ToString();
                    }
                    p1.Query += ")";
                }

                if (!String.IsNullOrEmpty(query.ExcludeWords.ToSearchString()))
                {
                    var words = query.ExcludeWords.Split(' ').ToList();
                    if (String.IsNullOrEmpty(p1.Query))
                        p1.Query = "NOT " + words[0];

                    foreach (string word in words)
                    {
                        p1.Query += " AND NOT " + word;
                    }
                }

                if (query.IncludeAssigned == true)
                {
                    if (!String.IsNullOrEmpty(p1.Query)) p1.Query += " AND meta-bfw_assigned:true";
                }

                if (!String.IsNullOrEmpty(query.ExactPhrase.ToSearchString()))
                {
                    string phrase = string.Format("\"{0}\"", query.ExactPhrase);
                    if (searchFields.Count > 0)
                    {
                        tempString = String.Empty;
                        foreach (string field in searchFields)
                        {
                            if (!String.IsNullOrEmpty(tempString)) tempString += " OR ";
                            tempString += field + ":" + phrase;
                        }
                        if (!String.IsNullOrEmpty(p1.Query)) p1.Query += " AND ";
                        p1.Query += tempString;
                        tempString = String.Empty;
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(p1.Query))
                            p1.Query = "\"" + query.ExactPhrase + "\"";
                        else
                            p1.Query += String.Format(" AND \"{0}\"", query.ExactPhrase);
                    }

                }

                if (!String.IsNullOrEmpty(query.ExactQuery.ToSearchString()))
                {
                    if (String.IsNullOrEmpty(p1.Query))
                        p1.Query = query.ExactQuery;
                    else
                        p1.Query += String.Format(" AND {0}", query.ExactQuery);
                }
                if (query.IsFaceted && query.FacetedQuery != null)
                {
                    p1.FacetFields = string.Join("|", query.FacetedQuery.Fields);
                    p1.FacetLimit = new FacetParam<int>() { Value = query.FacetedQuery.Limit };
                    p1.FacetMinCount = new FacetParam<int>() { Value = query.FacetedQuery.MinCount };
                    p1.Facet = true;
                }
            }
            return p1;
        }

        #endregion
    }
}