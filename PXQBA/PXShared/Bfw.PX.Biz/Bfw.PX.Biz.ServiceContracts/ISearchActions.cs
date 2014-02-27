using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Web.Mvc;

using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// Provides all business logic functions in regards to search.
    /// </summary>
    public interface ISearchActions
    {
        /// <summary>
        /// Lists results that match the specifed <see cref="SearchQuery" /> criteria.
        /// </summary>
        /// <returns></returns>
        SearchResultSet DoSearch(SearchQuery param);

        /// <summary>
        /// Lists results that match the specifed <see cref="SearchQuery"/> criteria.
        /// </summary>
        /// <param name="param">The search criteria.</param>
        /// <param name="urlHelper">A UrlHelper object to resolve internal item urls.</param>
        /// <param name="postProcess">if set to <c>true</c> runs post processing on the initial results.</param>
        /// <returns></returns>
        SearchResultSet DoSearch(SearchQuery param, UrlHelper urlHelper, Boolean postProcess);


        /// <summary>
        /// Lists results that match the specifed <see cref="SearchQuery"/> criteria for questions
        /// </summary>
        /// <param name="param">The search criteria.</param>
        /// <param name="entityId">Course to search</param>
        /// <returns></returns>
        IEnumerable<Question> DoQuestionSearch(SearchQuery param, string entityId, out int numfound);
        /// <summary>
        /// Determines whether the course is indexed by performing a generic search.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if any results are returned; otherwise, <c>false</c>.
        /// </returns>
        Boolean IsIndexed();

        /// <summary>
        /// Flag to determines if searches should be done against product course
        /// </summary>
        /// <returns>
        ///   <c>true</c> if searches should be done against product course; otherwise, <c>false</c>.
        /// </returns>
        void DoProductSearch(bool flag);    
    }
}