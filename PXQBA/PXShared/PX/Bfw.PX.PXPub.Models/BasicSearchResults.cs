using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Bfw.PX.PXPub.Models
{
    public class BasicSearchResults
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicSearchResults"/> class.
        /// </summary>
        public BasicSearchResults()
        {
            IsIndexed = true;
            Valid = true;
            Results = new Dictionary<string, SearchResults>();
        }

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>
        /// The results.
        /// </value>
        public IDictionary<string, SearchResults> Results { set; get; }

        /// <summary>
        /// Gets or sets the assignments.
        /// </summary>
        /// <value>
        /// The assignments.
        /// </value>
        public SearchResults Assignments { set; get; }

        /// <summary>
        /// Gets or sets the assigned items.
        /// </summary>
        /// <value>
        /// The assignments.
        /// </value>
        public SearchResults AssignedItems { set; get; }

        /// <summary>
        /// Gets or sets the Ebook.
        /// </summary>
        /// <value>
        /// The ebook.
        /// </value>
        public SearchResults Ebook { set; get; }

        /// <summary>
        /// Gets or sets the discussions.
        /// </summary>
        /// <value>
        /// The discussions.
        /// </value>
        public SearchResults Discussions { set; get; }

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        /// <value>
        /// The query.
        /// </value>
        public SearchQuery Query { set; get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is indexed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is indexed; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsIndexed { set; get; }

        /// <summary>
        /// Gets or sets a value indicating whether this query is valid.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this query is valid; otherwise, <c>false</c>.
        /// </value>
        public Boolean Valid { set; get; }

        /// <summary>
        /// Gets or sets the min search categories.
        /// </summary>
        /// <value>
        /// The min search categories.
        /// </value>
        public List<String> MinSearchCategories { set; get; }

        /// <summary>
        /// Gets the total results.
        /// </summary>
        public int TotalResults
        {
            get
            {
                int total = 0;
                bool res = false;
                int num1;

                if (Assignments != null)
                {
                    res = int.TryParse(Assignments.numFound, out num1);
                    if (res) { total += num1; }
                }

                if (Discussions != null)
                {
                    res = int.TryParse(Discussions.numFound, out num1);
                    if (res) { total += num1; }
                }

                if (Results != null)
                {
                    foreach (KeyValuePair<string, SearchResults> kvp in Results)
                    {
                        if (kvp.Value != null)
                        {
                            res = int.TryParse(kvp.Value.numFound, out num1);
                            if (res)
                            {
                                total += num1;
                            }
                        }
                    }
                }

                return total;
            }
        }
    }
}
