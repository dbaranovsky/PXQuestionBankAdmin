using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Helpers;
using Bfw.PX.PXPub.Models;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class TemplateMapper
    {
        /// <summary>
        /// Converts to template.
        /// </summary>
        /// <param name="contentItem">The content item.</param>
        /// <returns></returns>
        public static Template ToTemplate(this ContentItem contentItem)
        {
            return new Template
            {
                Id = contentItem.Id,
                Title = contentItem.Title,
                Description = contentItem.Description,
                ParentId = contentItem.ParentId,
                TemplateParentId = contentItem.TemplateParentId,
                Policies = contentItem.Policies ?? new string[] {}
            };
        }

        /// <summary>
        /// Convertes to template.
        /// </summary>
        /// <param name="contentItem">The content item.</param>
        /// <param name="contentActions">The content actions.</param>
        /// <returns></returns>
        public static Template ToTemplate(this Bfw.PX.Biz.DataContracts.ContentItem contentItem, IContentActions contentActions)
        {
            var modelTemplate = new Template
            {
                Id = contentItem.Id,
                Title = contentItem.Title,
                Description = contentItem.Description,
                ParentId = contentItem.ParentId
            };
            if (contentActions != null)
            {
                modelTemplate.Policies = AssignmentHelper.AssignmentPolicyFromSettings(contentItem, contentActions);
            }

            if (contentItem.Properties.ContainsKey("templateparent"))
            {
                modelTemplate.TemplateParentId = contentItem.Properties["templateparent"].Value.ToString();
            }

            return modelTemplate;
        }
    }
}
