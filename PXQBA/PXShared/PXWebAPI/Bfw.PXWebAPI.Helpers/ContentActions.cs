using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap.Session;
using Bfw.PXWebAPI.Mappers;
using Adc = Bfw.Agilix.DataContracts;
using Bfw.PXWebAPI.Models;

namespace Bfw.PXWebAPI.Helpers
{
    public class ContentActions
    {

        #region Properties
        /// <summary>
        /// Extension used by all PX Resource files.
        /// </summary>
        public const string PXRES_EXTENSION = "pxres";

        protected ISessionManager SessionManager { get; set; }

        #endregion

         #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentActions"/> class.
        /// </summary>
        /// <param name="sessionManager">The session manager.</param>
        public ContentActions(ISessionManager sessionManager)
        {
            SessionManager = sessionManager;
        }
        #endregion

        /// <summary>
        /// Returns the matching resource.
        /// </summary>
        /// <param name="entityId">Id of the entity to load resource from.</param>
        /// <param name="resourceUri">Uri to the resource.</param>
        /// <returns></returns>
        public Resource GetResource(string entityId, string resourceUri)
        {
            Resource result = null;
            result = LoadResource(entityId, resourceUri);
            return result;
        }

        /// <summary>
        /// Loads the resource.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="resourceUri">The resource URI.</param>
        /// <returns></returns>
        private Resource LoadResource(string entityId, string resourceUri, string query = "")
        {
            Resource resource = null;


            if (IsResourceUri(resourceUri))
            {
                var cmd = new GetResource() { EntityId = entityId, ResourcePath = resourceUri };
                SessionManager.CurrentSession.Execute(cmd);

                if (null != cmd.Resource)
                {
                    resource = cmd.Resource.ToResource();

                    if (resource.Extension == PXRES_EXTENSION)
                    {
                        //if (!ParsePxResourceFile(resource, query))
                        //{
                        //    resource = null;
                        //}

                        XmlResource xmlRes = cmd.Resource.ToXmlResource();
                        return xmlRes;
                    }

                }
            }


            return resource;
        }

        /// <summary>
        /// Determines whether [is resource URI] [the specified resource URI].
        /// </summary>
        /// <param name="resourceUri">The resource URI.</param>
        /// <returns>
        ///   <c>true</c> if [is resource URI] [the specified resource URI]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsResourceUri(string resourceUri)
        {
            bool result = false;
            Uri uri = null;

            result = Uri.TryCreate(resourceUri, UriKind.RelativeOrAbsolute, out uri) && !uri.IsAbsoluteUri;

            return result;
        }
    }
}
