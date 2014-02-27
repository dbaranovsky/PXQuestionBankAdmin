using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Used to identified what Dlap primitive the item element represents.
    /// </summary>
    public enum DlapType
    {
        Resource = 0,
        Assignment,
        Assessment,
        Discussion,
        Folder,
        CustomActivity,
        AssetLink,
        RssFeed,
        Survey,
        Shortcut,
        Homework,
        Custom,
        HtmlDocument,
        Comment = 999
    }

    /// <summary>
    /// Represents a single search result item.
    /// (See http://gls.agilix.com/Docs/Command/Search)
    /// </summary>
    public class SearchResultDoc
    {
        /// <summary>
        /// ID of the entity the result item belongs to.
        /// </summary>
        public string entityid { get; set; }

        /// <summary>
        /// The class or type that this result document is. Possible values are item and question.
        /// </summary>
        public string doc_class { get; set; }

        /// <summary>
        /// ID of the search result item.
        /// </summary>
        public string itemid { get; set; }

        /// <summary>
        /// Score of the item (only if question).
        /// </summary>
        public string score { get; set; }

        /// <summary>
        /// The class or type that this result document is. Possible values are item and question.
        /// </summary>
        public string dlap_class { get; set; }

        /// <summary>
        /// Indicates if result item is hidden from students.
        /// </summary>
        public string dlap_hiddenfromstudent { get; set; }

        /// <summary>
        /// Unique ID of the result item.
        /// </summary>
        public string dlap_id { get; set; }
        
        /// <summary>
        /// Indicates the <see cref="DlapItemType" /> of the result item.
        /// </summary>         
        public string dlap_itemtype { get; set; }

        /// <summary>
        /// Indicates the <see cref="DlapContentType" /> of the result item.
        /// </summary>         
        public string dlap_contenttype { get; set; }

        /// <summary>
        /// Title text for the result item.
        /// </summary>
        public string dlap_title { get; set; }

        /// <summary>
        /// SubTitle text for the result item.
        /// </summary>
        public string dlap_subtitle { get; set; }

        /// <summary>
        /// URL pointing to the result content.
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// Indexed content body returned from search, including title and other indexed fields.
        /// </summary>
        public string dlap_html_text { get; set; }

        /// <summary>
        /// Indexed content body returned from search.
        /// </summary>
        public string dlap_text { get; set; }

        /// <summary>
        /// Gets the CSS class to associate with the search result based off the result item type.
        /// </summary>
        public string CssClass
        {
            get
            {
                DlapType type = (DlapType)(Convert.ToInt32(this.dlap_itemtype));
                return type.ToString();
                
            }
        }

        /// <summary>
        /// Collection of elements in item that begin with "meta-" tag (and are indexed by solr)
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; }


        #region Question Fields - Fields that only exist for questions
        /// <summary>
        /// Contains the interaction type (choice, match, answer, etc
        /// </summary>
        public string dlap_q_type { get; set; }

        /// <summary>
        /// Contains the score value, if any.
        /// </summary>
        public double dlap_q_score { get; set; }

        /// <summary>
        /// Contains each of the interaction's flags names (ShowWorkspace, MaintainOrder, Inline, etc.)
        /// </summary>
        public string dlap_q_flags { get; set; }

        /// <summary>
        /// If the interaction type is text, contains the texttype value (Normal, IgnoreCase, Numeric, etc.)
        /// </summary>
        public string dlap_q_texttype { get; set; }

        /// <summary>
        /// Contains learning objective IDs listed beneath the learningobjectives element. ID values are 32-digit string values with no hyphens; for example: E6E51A041F224259AE129334C6804D1C
        /// </summary>
        public string dlap_objectives { get; set; }

        /// <summary>
        /// Contains the text of the learning objectives whose IDs are listed beneath the question's learningobjectives element. The text comes from the corresponding objective element in the Course Data of the course that contains the question.
        /// </summary>
        public string dlap_objective_text { get; set; }
        #endregion

        //*end question fields
        public SearchResultDoc()
        {
            this.Metadata = new Dictionary<string, string>();
        }
    }
}
