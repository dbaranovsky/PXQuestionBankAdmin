using System;

using Bfw.PX.PXPub.Controllers.Mappers;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;


namespace Bfw.PX.PXPub.Controllers.Helpers
{
    public class AssessmentSettingsHelper
    {
        /// <summary>
        /// Access to the current business context information.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Access to an IContentActions implementation
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }

        public AssessmentSettingsHelper(BizSC.IBusinessContext context, BizSC.IContentActions contentActions)
        {
            Context = context;
            ContentActions = contentActions;
        }

        public bool Save(Models.AssessmentSettings settings)
        {
            if (String.IsNullOrEmpty(settings.EntityId) || settings.EntityId.Equals("EntireClass", StringComparison.CurrentCultureIgnoreCase))
            {
                settings.EntityId = Context.CourseId;
            }

            // Load the quiz settings
            BizDC.ContentItem item = ContentActions.GetContent(settings.EntityId, settings.AssessmentId);

            // Map new settings
            settings.MapTo(item);

            // Store back the new settings
            ContentActions.StoreContent(item, settings.EntityId);

            return true;
        }
    }
}
