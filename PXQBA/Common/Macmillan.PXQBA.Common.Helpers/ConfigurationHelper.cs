using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Macmillan.PXQBA.Common.Helpers.Constants;

namespace Macmillan.PXQBA.Common.Helpers
{
    public static class ConfigurationHelper
    {
        /// <summary>
        /// Gets Mars login url from config
        /// </summary>
        /// <returns>Mars login url</returns>
        public static string GetMarsLoginUrl()
        {
            var url = ConfigurationManager.AppSettings[ConfigurationKeys.MarsPathLoginUrl];
            if (!string.IsNullOrEmpty(url))
            {
                return url;
            }
            return "http://dev.activation.macmillanhighered.com/account/logon?target={0}";
        }

        /// <summary>
        /// Gets BH default password from config
        /// </summary>
        /// <returns>Default password</returns>
        public static string GetBrainhoneyDefaultPassword()
        {
            var url = ConfigurationManager.AppSettings[ConfigurationKeys.BrainhoneyDefaultPassword];
            if (!string.IsNullOrEmpty(url))
            {
                return url;
            }
            return "fakepassword";
        }

        /// <summary>
        /// Get question per page parameter from config
        /// </summary>
        /// <returns>Question per page</returns>
        public static int GetQuestionPerPage()
        {
            var questionPerPage = ConfigurationManager.AppSettings[ConfigurationKeys.QuestionPerPage];
            if (!string.IsNullOrEmpty(questionPerPage))
            {
                return Int32.Parse(questionPerPage);
            }

            return 50;
        }

        /// <summary>
        /// Gets converter url for HTS questions
        /// </summary>
        /// <returns>HTS url</returns>
        public static string GetHTSConverterUrl()
        {
            var url = ConfigurationManager.AppSettings[ConfigurationKeys.HtsConverter];
            if (!string.IsNullOrEmpty(url))
            {
                return url;
            }
            return "http://dev.px.bfwpub.com/PxHTS/PxPlayer.ashx";
        }

        /// <summary>
        /// Gets Fma Graph converter url
        /// </summary>
        /// <returns>Fma Graph url</returns>
        public static string GetFmaGraphConverterUrl()
        {
            var url = ConfigurationManager.AppSettings[ConfigurationKeys.FmaGraphConverter];
            if (!string.IsNullOrEmpty(url))
            {
                return url;
            }
            return "http://dev.px.bfwpub.com/PxEG/authormode2.aspx";
        }

        /// <summary>
        /// Gets the list of available question types
        /// </summary>
        /// <returns>Question types list</returns>
        public static IEnumerable<List<string>> GetQuestionTypes()
        {
            var settingString = ConfigurationManager.AppSettings[ConfigurationKeys.QuestionTypes];
            if (!string.IsNullOrEmpty(settingString))
            {
                return settingString.Split('|').Select(type => type.Split(':')).Select(parts => parts.ToList());
            }
            return new List<List<string>>
            {
                new List<string>(){"choice", "Multiple Choice"},
                new List<string>(){"text", "Short Answer"},
                new List<string>(){"essay", "Essay"},
                new List<string>(){"match", "Matching"},
                new List<string>(){"answer", "Multiple Answer"},
                new List<string>(){"HTS","Advanced Question", "custom"},
                new List<string>(){"FMA_GRAPH", "Graph Exercise", "custom"}
            };
        }

        /// <summary>
        /// Get admin userspace
        /// </summary>
        /// <returns>Admin userspace</returns>
        public static string GetAdministratorUserspace()
        {
            var administratorUserspace = ConfigurationManager.AppSettings[ConfigurationKeys.AdministratorUserspace];
            if (!string.IsNullOrEmpty(administratorUserspace))
            {
                return administratorUserspace;
            }
            return "root";
        }

        /// <summary>
        /// Gets admin user id
        /// </summary>
        /// <returns>Admin user id</returns>
        public static string GetAdministratorUserId()
        {
            var administratorUserId = ConfigurationManager.AppSettings[ConfigurationKeys.AdministratorUserId];
            if (!string.IsNullOrEmpty(administratorUserId))
            {
                return administratorUserId;
            }
            return "7";
        }

        /// <summary>
        /// Get admin password
        /// </summary>
        /// <returns>Admin password</returns>
        public static string GetAdministratorPassword()
        {
            var administratorPassword = ConfigurationManager.AppSettings[ConfigurationKeys.AdministratorPassword];
            if (!string.IsNullOrEmpty(administratorPassword))
            {
                return administratorPassword;
            }
            return "Px-Migration-123";
        }

        /// <summary>
        /// Get domain id
        /// </summary>
        /// <returns>Domain id</returns>
        public static string GetDomainId()
        {
            var domainId = ConfigurationManager.AppSettings[ConfigurationKeys.DomainId];
            if (!string.IsNullOrEmpty(domainId))
            {
                return domainId;
            }
            return "6650";
        }

        /// <summary>
        /// Get domain userspace
        /// </summary>
        /// <returns>Domain userspace</returns>
        public static string GetDomainUserspace()
        {
            var domainUserspace = ConfigurationManager.AppSettings[ConfigurationKeys.DomainUserspace];
            if (!string.IsNullOrEmpty(domainUserspace))
            {
                return domainUserspace;
            }
            return "bfwproducts";
        }

        /// <summary>
        /// Get discipline course id
        /// </summary>
        /// <returns>Discipline course id</returns>
        public static string GetDisciplineCourseId()
        {
            var disciplineCourseId = ConfigurationManager.AppSettings[ConfigurationKeys.DisciplineCourseId];
            if (!string.IsNullOrEmpty(disciplineCourseId))
            {
                return disciplineCourseId;
            }
            return "6710";
        }


        /// <summary>
        /// Get ActionPlayer url for generatingrun question scenario
        /// </summary>
        /// <returns>Discipline course id</returns>
        public static string GetActionPlayerUrlTemplate()
        {
            var url = ConfigurationManager.AppSettings[ConfigurationKeys.ActionPlayerUrlTemplate];
            if (!string.IsNullOrEmpty(url))
            {
                return url;
            }
            return "http://root.dev.brainhoney.bfwpub.com/BrainHoney/Component/ActivityPlayer?enrollmentid=200117&itemid=AHWDG&ShowHeader=false";
        }

        /// <summary>
        /// Get discipline course id
        /// </summary>
        /// <returns>Discipline course id</returns>
        public static int GetCacheTimeout()
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[ConfigurationKeys.CacheTimeout]))
            {
                int cacheTimeout;
                if (int.TryParse(ConfigurationManager.AppSettings[ConfigurationKeys.CacheTimeout], out cacheTimeout))
                {
                    return cacheTimeout;
                }
            }
            return 60;
        }
		/// <summary>
        /// Get Editor url for question editing
        /// </summary>
        /// <returns>Discipline course id</returns>
        public static string GetEditorUrlTemplate()
        {
            var url = ConfigurationManager.AppSettings[ConfigurationKeys.EditorUrlTemplate];
            if (!string.IsNullOrEmpty(url))
            {
                return url;
            }
            return "http://root.dev.brainhoney.bfwpub.com/BrainHoney/Component/QuestionEditor?enrollmentid={0}&itemid={1}&questionid={2}&showcancel=true";
        }

        public static string GetTemporaryCourseId()
        {
            var courseId = ConfigurationManager.AppSettings[ConfigurationKeys.TemporaryCourseId];
            if (!string.IsNullOrEmpty(courseId))
            {
                return courseId;
            }
            return "200117";
        }

        public static int? GetSolrUpdateTaskId()
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[ConfigurationKeys.SolrUpdateTaskId]))
            {
                int taskId;
                if (int.TryParse(ConfigurationManager.AppSettings[ConfigurationKeys.SolrUpdateTaskId], out taskId))
                {
                    return taskId;
                }
            }
            return null;
        }

        public static string GetHTSEditorUrlTemplate()
        {
            var url = ConfigurationManager.AppSettings[ConfigurationKeys.HTSEditorUrlTemplate];
            if (!string.IsNullOrEmpty(url))
            {
                return url;
            }
            return "http://dev.whfreeman.com/hts?questionId={0}&amp;quizId={1}&amp;entityId={2}&amp;enrollmentId={3}&amp;playerUrl=http%3a%2f%2fdev.px.bfwpub.com%2fPxHTS%2fPxPlayer.ashx&amp;convert=True";
        }

        public static string GetFmaGraphEditorUrl()
        {
            var url = ConfigurationManager.AppSettings[ConfigurationKeys.GraphEditorUrl];
            if (!string.IsNullOrEmpty(url))
            {
                return url;
            }
            return "http://dev.px.bfwpub.com/PxEG/authormode2.aspx";
        }

        public static int GetUsersPerPage()
        {
            var usersPerPage = ConfigurationManager.AppSettings[ConfigurationKeys.UsersPerPage];
            if (!string.IsNullOrEmpty(usersPerPage))
            {
                return Int32.Parse(usersPerPage);
            }

            return 20;
        }
    }
}
