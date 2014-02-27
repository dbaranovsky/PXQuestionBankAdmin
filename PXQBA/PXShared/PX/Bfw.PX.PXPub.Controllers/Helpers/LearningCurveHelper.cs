using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Models;
using Microsoft.Practices.ServiceLocation;

namespace Bfw.PX.PXPub.Controllers.Helpers
{
    public static class LearningCurveHelper
    {
        /// <summary>
        /// Constructs the url to the learningCurve player based on the model.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string GetPlayerUrl(LearningCurveActivity model, string mode)
        {
            StringBuilder sb = new StringBuilder();
            string domain = WebConfigurationManager.AppSettings["learningCurveDomain"];
            var appRoot = GetAppRoot();
            string targetScore = model.TargetScore ?? "";
            string questionId = model.DefaultQuestionId ?? string.Empty;
            string studentView = "";
            if (model.UserAccess == AccessLevel.Student)
            {
                studentView = "&view=student";
            }
            string debugMode = "";
            var environment = GetEnvironment(domain);
            if (environment.Equals("dev", StringComparison.CurrentCultureIgnoreCase))
            {
                debugMode = "&test_mode=true";
            }
            string showReport = "", reportType = "";

            if (!string.IsNullOrEmpty(mode) && mode.Equals("results", StringComparison.CurrentCultureIgnoreCase))
            {
                showReport = "&show_report=true";
                reportType = "&report_type=showAll";
            }

            sb.AppendFormat("{0}/index.php?st={1}&enrollmentid={2}&itemid={3}&approot={4}&platform=px&reportingmode=arga{5}{6}{7}{8}&qid={9}&disableArgaAlert=true",
                                domain, targetScore, model.EnrollmentId, model.Id, appRoot, studentView, debugMode, showReport, reportType, questionId);
            return sb.ToString();
        }

        private static string GetEnvironment(string domain)
        {
            var match = Regex.Match(domain, @"^(?:http://)(\w*)-?(?:learningcurve.bfwpub.com)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var value = match.Groups[1].Value;
                return value.Equals("stg", StringComparison.CurrentCultureIgnoreCase) ? "qa" : value;
            }
            return "";
        }

        private static string GetAppRoot()
        {
            var context = ServiceLocator.Current.GetInstance<IBusinessContext>();
            var appRoot = WebConfigurationManager.AppSettings["scorm_approot"];
            appRoot = appRoot.Replace("[domain]", context.Domain.Userspace);
            Uri requestUrl = System.Web.HttpContext.Current.Request.Url;
            string host = requestUrl.Host;
            var requestDomain = "";
            if (!host.Contains("."))
            {
                requestDomain = host; // this will handle the case for "localhost"
            }
            else
            {
                string[] splitParts = host.Split('.');
                int length = splitParts.Length;
                requestDomain = string.Format("{0}.{1}", splitParts[length - 2], splitParts[length - 1]);
            }
            if (requestDomain.Contains("localhost"))
            {
                requestDomain = requestDomain.Replace("localhost", "worthpublishers.com");
            }
            appRoot = appRoot.Replace("[company]", requestDomain);
            return appRoot;
        }
    }
}
