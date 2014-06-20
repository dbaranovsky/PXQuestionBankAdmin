using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Common.Helpers.Constants
{
    /// <summary>
    /// Represents configuration file key values
    /// </summary>
    class ConfigurationKeys
    {
        /// <summary>
        /// Represents the key for Mars login page
        /// </summary>
        public const string MarsPathLoginUrl = "MarsPathLogin";

        /// <summary>
        /// Key for brain honey default password
        /// </summary>
        public const string BrainhoneyDefaultPassword = "BrainhoneyDefaultPassword";

        /// <summary>
        /// Key for number of question on the page in question list
        /// </summary>
        public const string QuestionPerPage = "QuestionPerPage";

        /// <summary>
        /// Key for HTS converter page
        /// </summary>
        public const string HtsConverter = "HtsConverter";

        /// <summary>
        /// Key for Graph converter page
        /// </summary>
        public const string FmaGraphConverter = "FmaGraphConverter";

        /// <summary>
        /// Key for question types
        /// </summary>
        public const string QuestionTypes = "QuestionTypes";

        public const string TemporaryCourseId = "TemporaryCourseId";


        /// <summary>
        /// Key for administrator userspace
        /// </summary>
        public const string AdministratorUserspace = "AdministratorUserspace";

        /// <summary>
        /// Key for administrator user id
        /// </summary>
        public const string AdministratorUserId = "AdministratorUserId";

        /// <summary>
        /// Key for administrator password
        /// </summary>
        public const string AdministratorPassword = "AdministratorPassword";

        /// <summary>
        /// Key for domain id
        /// </summary>
        public const string DomainId = "DomainId";

        /// <summary>
        /// Key for domain userspace
        /// </summary>
        public const string DomainUserspace = "DomainUserspace";

        /// <summary>
        /// Key for entity id
        /// </summary>
        public const string DisciplineCourseId = "DisciplineCourseId";

        /// <summary>
        /// Key for external Action Player in BrainHoney
        /// </summary>
        public static string ActionPlayerUrlTemplate = "ActionPlayerUrl";

        /// <summary>
        /// Represents the cache timeout in minutes
        /// </summary>
        public const string CacheTimeout = "cacheTimeoutInMinutes";

		/// <summary>
        /// Key for external Editor in BrainHoney
        /// </summary>
        public static string EditorUrlTemplate = "EditorUrl";

        /// <summary>
        /// Task id to update solr with latest data 
        /// </summary>
        public static string SolrUpdateTaskId = "SolrUpdateTaskId";

        /// <summary>
        /// Key for Advanced Question editor
        /// </summary>
        public static string HTSEditorUrlTemplate = "HTSEditorUrl";

        /// <summary>
        /// Server access point URL where from comes html with Graph Editor
        /// </summary>
        public static string GraphEditorUrl = "GraphEditorUrl";

        /// <summary>
        /// Key for number of question on the page in question list
        /// </summary>
        public const string UsersPerPage = "UsersPerPage";
    }
}
