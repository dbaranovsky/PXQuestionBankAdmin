using System;
using System.Linq;
using System.Configuration;
using Bfw.Common;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class ExternalContentMapper
    {
        public static ExternalContent ToExternalContent(this BizDC.ContentItem biz, BizSC.IBusinessContext context)
        {
            if (string.IsNullOrWhiteSpace(biz.Href))
                throw new MappingDataMissingException("Cannot map ExternalContent: Missing href");
            if (context == null)
                throw new MappingDataMissingException("Cannot map ExternalContent: Missing Context");

            var model = new ExternalContent();
            
            model.ToBaseContentItem(biz);

            model.IsProductCourse = context.CourseIsProductCourse;

            model.Url = biz.Href;

            if (biz.AssignmentSettings != null)
            {
                model.DueDate = biz.AssignmentSettings.DueDate;
            }
            if (biz.FacetMetadata.ContainsKey("meta-content-type"))
            {
                var metaContentType = biz.FacetMetadata["meta-content-type"];
                if (!String.IsNullOrEmpty(metaContentType))
                {
                    if (metaContentType.IndexOf("SolutionMaster", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        string solutionMasterUrl = ConfigurationManager.AppSettings["SolutionMasterUrl"];
                        var textbook = GetTextbookNameFromUrl(model.Url);
                        model.Url = String.Format("{0}/{1}/?uid={2}&rau={2}", solutionMasterUrl, textbook,
                            context.CurrentUser.Username);
                    }
                    else if (metaContentType.Contains("LearningCurve"))
                    {
                        model.TrackMinutesSpent = false;
                    }
                }
            }

            if (model.Sco)
            {
                if (context.Course.EnableArgaUrlMapping && ExternalDomainMapper.IsEnable())
                {
                    ExternalDomainMapper.MapUrlToPxUrl(model);
                }
                else
                {
                    string learningCurveDomain = ConfigurationManager.AppSettings["learningCurveDomain"];
                    if (!string.IsNullOrEmpty(learningCurveDomain) &&
                        learningCurveDomain != "http://learningcurve.bfwpub.com")
                    {
                        model.Url = model.Url.Replace("http://learningcurve.bfwpub.com", learningCurveDomain);
                    }
                }
                var appRoot = GetAppRoot(context);
                var argaUrl = GetArgaUrl(context);
                // Get the number of milliseconds elapsed since 1 January 1970 00:00:00 UTC.
                long unixCurrentTime = Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds);
                var utcDueDate = context.Course == null
                    ? model.DueDate
                    : DateTimeConversion.ConvertToUtcTime(model.DueDate, context.Course.CourseTimeZone);
                long unixDueDate = model.DueDate.Year == DateTime.MinValue.Year ? 0 : Convert.ToInt64((utcDueDate - new DateTime(1970, 1, 1)).TotalMilliseconds);
                var q = string.Format("{0}enrollmentid={1}&itemid={2}&approot={3}&ARGA_server={4}&arga_api={5}&startTime={6}&dueDate={7}&track={8}{9}", model.Url.Contains("?") ? "&" : "?", context.EnrollmentId, model.Id, appRoot, argaUrl.Replace("/ARGA_wrapper.js", ""), argaUrl, unixCurrentTime, unixDueDate, (!model.TrackMinutesSpent).ToString().ToLower(), "&reportingMode=arga&disableArgaAlert=true&platform=px");

                model.Url += q;
            }


            return model;
        }

        private static string GetArgaUrl(BizSC.IBusinessContext context)
        {
            var argaUrl = ConfigurationManager.AppSettings["arga"];

            if (string.IsNullOrEmpty(argaUrl))
            {
                argaUrl = "/BFWglobal/js/ARGA/ARGA_wrapper.js";
            }
            else
            {
                string urlStr = context.URL.Contains(Uri.SchemeDelimiter) ? context.URL : string.Concat(Uri.UriSchemeHttp, Uri.SchemeDelimiter, context.URL);
                Uri uri = new Uri(urlStr);
                string domain = uri.Host;
                argaUrl = argaUrl.Replace("[domain]", domain);

            }

            return argaUrl;
        }
        private static string GetAppRoot(BizSC.IBusinessContext context)
		{
			const string default_appRoot = "/brainhoney";
			var appRoot = ConfigurationManager.AppSettings["scorm_approot"];

			if (string.IsNullOrEmpty(appRoot)) 
			{
				appRoot = default_appRoot;
			}
		    else
			{
                appRoot = GetAppRootFromContextDomain(context, appRoot);
				if (string.IsNullOrEmpty(appRoot)) appRoot = default_appRoot;
			}
			
			return appRoot;
		}

        private static string GetAppRootFromContextDomain(BizSC.IBusinessContext context, string appRoot)
		{
            if (context == null || context.Domain == null ||
                string.IsNullOrEmpty(context.Domain.Userspace)) return null;

            appRoot = appRoot.Replace("[domain]", context.Domain.Userspace.ToLowerInvariant());

			string documentDomain;
			var requestUrl = System.Web.HttpContext.Current.Request.Url;
			var host = requestUrl.Host;
			if (!host.Contains("."))
			{
				documentDomain = host; // this will handle the case for "localhost"
			}
			else
			{
				var splitParts = host.Split('.');
				var length = splitParts.Length;
				documentDomain = string.Format("{0}.{1}", splitParts[length - 2], splitParts[length - 1]);
			}

			appRoot = appRoot.Replace("[company]", documentDomain);

			if (appRoot.Contains("localhost")) appRoot = appRoot.Replace("localhost", "bedfordstmartins.com");
			
			return  appRoot;
		}

        private static string GetTextbookNameFromUrl(string url)
        {
            if (!String.IsNullOrEmpty(url))
            {
                var parts = url.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < parts.Length; i++)
                {
                    var part = parts[i];
                    if (part.Equals("SolutionMaster", StringComparison.OrdinalIgnoreCase))
                    {
                        var position = i - 2;
                        if (position > 0)
                        {
                            var textbook = parts[position];
                            return textbook;
                        }
                    }
                }
            }
            return String.Empty;
        }
    }
}
