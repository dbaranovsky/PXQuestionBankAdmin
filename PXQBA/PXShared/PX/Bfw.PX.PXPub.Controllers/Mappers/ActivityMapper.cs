using System.Configuration;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class ActivityMapper
    {

        /// <summary>
        /// Maps ContentItem to Activity
        /// </summary>
        /// <param name="biz">Content Item</param>
        /// <param name="context">Context Service</param>
        public static Models.Activity ToActivity(this BizDC.ContentItem biz, BizSC.IBusinessContext context, BizSC.IContentActions contentActions)
        {
            var model = new Models.Activity();
            var modelItem = biz.ToContentItem(contentActions);
            if (biz != null)
            {
                model.isAssigned = biz.AssignmentSettings != null && biz.AssignmentSettings.meta_bfw_Assigned;
                model.DueDate = biz.AssignmentSettings != null ? biz.AssignmentSettings.DueDate : model.DueDate;
                model.Id = biz.Id;
                model.Title = biz.Title;
                model.UserAccess = context.AccessLevel;
                model.Sequence = biz.Sequence;
                model.Href = modelItem.Url;
                if (biz.FacetMetadata.ContainsKey("meta-topic") && !string.IsNullOrEmpty(biz.FacetMetadata["meta-topic"]))
                {
                    model.MetaTopic = biz.FacetMetadata["meta-topic"];

                }
                else if (biz.FacetMetadata.ContainsKey("meta-topics/meta-topic") && !string.IsNullOrEmpty(biz.FacetMetadata["meta-topics/meta-topic"]))
                {
                    model.MetaTopic = biz.FacetMetadata["meta-topics/meta-topic"];

                }
                else
                {
                    model.MetaTopic = "Uncategorized";
                }

                if (context.Course.EnableArgaUrlMapping && ExternalDomainMapper.IsEnable())
                {
                    ExternalDomainMapper.MapUrlToPxUrl(model);
                }
                else
                {
                    string learningCurveDomain = ConfigurationManager.AppSettings["learningCurveDomain"];
                    if (!string.IsNullOrEmpty(model.Href) && !string.IsNullOrEmpty(learningCurveDomain) && learningCurveDomain != "http://learningcurve.bfwpub.com")
                    {
                        model.Href = model.Href.Replace("http://learningcurve.bfwpub.com", learningCurveDomain);
                    }
                }
            }

            return model;
        }
    }
}
