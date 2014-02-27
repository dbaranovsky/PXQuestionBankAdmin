using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Web.Mvc;

using Bdc = Bfw.PX.Biz.DataContracts;
using Adc = Bfw.Agilix.DataContracts;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.Commands;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.Common.Logging;

namespace Bfw.PX.Biz.Direct.Services
{
    /// <summary>
    /// Implements IResourceMapActions using direct connection to DLAP.
    /// </summary>
    public class ResourceMapActions : IResourceMapActions
    {
        #region Properties

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected IBusinessContext Context { get; set; }

        /// <summary>
        /// Gets or sets the session manager.
        /// </summary>
        /// <value>
        /// The session manager.
        /// </value>
        protected ISessionManager SessionManager { get; set; }

        /// <summary>
        /// The IContentActions implementation to use.
        /// </summary>
        protected IContentActions ContentActions { get; set; }

        /// <summary>
        /// Gets or sets the path to the course resource mapping file.
        /// </summary>
        protected string ResourceMapPath { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceMapActions"/> class.
        /// </summary>
        /// <param name="ctxt">The IBusinessContext implementation.</param>
        /// <param name="contentActions">The IContentActions implementation.</param>
        public ResourceMapActions(IBusinessContext ctxt, IContentActions contentActions)
        {
            Context = ctxt;
            ContentActions = contentActions;
            string entityId = Context.EntityId;
            string enrollmentId = Context.EnrollmentId;

            if (Context.Course != null)
            {               
                ResourceMapPath = string.Format("Templates/Data/XmlResources/Settings/ResourceMaps/{0}/{1}.pxres", entityId, enrollmentId);
            }

        }
       
        #endregion

        #region IResourceMapActions Members

        /// <summary>
        /// Stores the currently logged in user's resource mappings
        /// </summary>
        /// <param name="resourceMap"></param>
        [Obsolete("This method depends on an IContentActions object.", false)]
        public void StoreMaps(List<Bdc.ResourceMap> mapList)
        {
            string entityId = Context.EntityId;
            string enrollmentId = Context.EnrollmentId;            
            StoreMaps(mapList, entityId, enrollmentId);
        }

        public void StoreMaps(List<Bdc.ResourceMap> mapList, string entityId, string enrollmentId)
        {
            if (mapList.IsNullOrEmpty()) return;
            using (Context.Tracer.DoTrace("ResourceMapActions.StoreMaps(mapList)"))
            {
                var xDoc = new XmlDocument();
                XmlNode xRootNode = xDoc.CreateNode(XmlNodeType.Element, "ResourceMaps", "");
                foreach (var map in mapList)
                {
                    XmlNode xNode = xDoc.CreateNode(XmlNodeType.Element, "Map", "");

                    //if inserting a new map record create a new guid
                    if (string.IsNullOrEmpty(map.Id)) map.Id = Guid.NewGuid().ToString("N");

                    XmlAttribute attribId = xDoc.CreateAttribute("Id");
                    attribId.Value = map.Id;
                    xNode.Attributes.Append(attribId);

                    XmlAttribute attribItemId = xDoc.CreateAttribute("ItemId");
                    attribItemId.Value = map.ItemId;
                    xNode.Attributes.Append(attribItemId);

                    XmlAttribute attribMapType = xDoc.CreateAttribute("MapType");
                    attribMapType.Value = map.MapType;
                    xNode.Attributes.Append(attribMapType);

                    XmlAttribute attribMapDesc = xDoc.CreateAttribute("Desc");
                    attribMapDesc.Value = map.Description;
                    xNode.Attributes.Append(attribMapDesc);

                    XmlNode xAssociatedItemsNode = xDoc.CreateNode(XmlNodeType.Element, "AssociatedItems", "");
                    xNode.AppendChild(xAssociatedItemsNode);

                    foreach (string s in map.AssociatedItems)
                    {
                        XmlNode xAssociatedItemNode = xDoc.CreateNode(XmlNodeType.Element, "AssociatedItem", "");
                        XmlAttribute attribAssociatedId = xDoc.CreateAttribute("Id");
                        attribAssociatedId.Value = s;
                        xAssociatedItemNode.Attributes.Append(attribAssociatedId);
                        xAssociatedItemsNode.AppendChild(xAssociatedItemNode);
                    }

                    xRootNode.AppendChild(xNode);
                }

                xDoc.AppendChild(xRootNode);


                var resource = new Bdc.XmlResource
                {
                    Status = Bdc.ResourceStatus.Normal,
                    Url = String.Format("Templates/Data/XmlResources/Settings/ResourceMaps/{0}/{1}.pxres", entityId, enrollmentId),
                    EntityId = enrollmentId,
                    Title = "ResourceMap_" + enrollmentId,
                    Body = xDoc.InnerXml
                };

                resource.ExtendedProperties.Add("ResourceMap_", entityId);
                ContentActions.StoreResources(new List<Bdc.Resource> { resource });
            }
        }

        /// <summary>
        /// Helper method to merge 2 comma-delimited strings of resource IDs excluding duplicates.
        /// </summary>
        /// <param name="origIds">The original IDs.</param>
        /// <param name="newIds">The new IDs.</param>
        /// <returns></returns>
        private List<string> MergedResourceIds(string origIds, string newIds)
        {
            List<string> result = new List<string>();
            using (Context.Tracer.DoTrace("ResourceMapActions.MergedResourceIds(origIds={0},newIds={1})", origIds, newIds))
            {


                var itemIds = newIds.Split(',').ToList();
                foreach (string s in itemIds)
                {
                    if (!String.IsNullOrEmpty(s))
                    {
                        if (!origIds.Contains(s)) origIds = origIds + ',' + s;
                    }
                }

                itemIds = origIds.Split(',').ToList();
                foreach (string s in itemIds)
                {
                    if (!String.IsNullOrEmpty(s))
                    {
                        result.Add(s);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Lists the currently logged in user's resource mappings.
        /// </summary>
        /// <returns>Collection of resourceMap objects.</returns>
        [Obsolete("This method depends on an IContentActions object.", false)]
        public IEnumerable<Bdc.ResourceMap> ListMaps()
        {
            string entityId = Context.EntityId;
            string enrollmentId = Context.EnrollmentId;           
            var resourceMapList = new List<Bdc.ResourceMap>();

            using (Context.Tracer.DoTrace("ResourceMapActions.ListMaps"))
            {
                var resourceMapFile = ContentActions.ListResources(enrollmentId, ResourceMapPath, "");


                if (!resourceMapFile.Any())
                {
                    var xDoc = new XmlDocument();
                    XmlNode xRootNode = xDoc.CreateNode(XmlNodeType.Element, "ResourceMaps", "");
                    xDoc.AppendChild(xRootNode);
                    var resource = new Bdc.XmlResource
                    {
                        Status = Bdc.ResourceStatus.Normal,
                        Url = ResourceMapPath,
                        EntityId = enrollmentId,
                        Title = "ResourceMap_" + enrollmentId,
                        Body = xDoc.InnerXml
                    };

                    resource.ExtendedProperties.Add("ResourceMap_", entityId);
                    ContentActions.StoreResources(new List<Bdc.Resource> { resource });

                    resourceMapFile = ContentActions.ListResources(enrollmentId, ResourceMapPath, "");
                }

                if (resourceMapFile.Any())
                {
                    var xDoc = new XmlDocument();

                    var stream = resourceMapFile.First().GetStream();
                    string data = "";
                    using (var sw = new System.IO.StreamReader(stream))
                    {
                        data = sw.ReadToEnd();
                    }

                    xDoc.LoadXml(data); //Created the Parent Node.
                    XmlNode rootNode = xDoc.SelectSingleNode("ResourceMaps");
                    if (rootNode == null) return resourceMapList;

                    foreach (var node in rootNode.ChildNodes)
                    {
                        XmlNode item = (XmlNode)node;
                        string mapId = (item.Attributes["Id"] != null) ? item.Attributes["Id"].Value : "";
                        string itemId = (item.Attributes["ItemId"] != null) ? item.Attributes["ItemId"].Value : "";
                        string mapDesc = (item.Attributes["Desc"] != null) ? item.Attributes["Desc"].Value : "";
                        string associatedResourceIds = (item.Attributes["AssociatedItems"] != null) ? item.Attributes["AssociatedItems"].Value : "";

                        List<string> associatedIds = new List<string>();
                        XmlNode associatedItemsNode = item.SelectSingleNode("AssociatedItems");
                        if (associatedItemsNode != null)
                        {
                            foreach (var childNode in associatedItemsNode.ChildNodes)
                            {
                                XmlNode associatedItem = (XmlNode)childNode;
                                string associatedId = (associatedItem.Attributes["Id"] != null) ? associatedItem.Attributes["Id"].Value : "";
                                if (!String.IsNullOrEmpty(associatedId)) associatedIds.Add(associatedId);
                            }
                        }

                        var resourceMap = new Bdc.ResourceMap() { Id = mapId, ItemId = itemId, AssociatedItems = associatedIds, Description = mapDesc };

                        resourceMapList.Add(resourceMap);
                    }
                }
            }
            return resourceMapList;
        }

        /// <summary>
        /// Lists the currently logged in user's resource mappings.
        /// </summary>
        /// <returns>Collection of resourceMap objects.</returns>
        public IEnumerable<Bdc.ResourceMap> ListMaps(string enrollmentId, string entityId = null)
        {
            if (entityId == null) entityId = Context.EntityId;
            var resourceMapList = new List<Bdc.ResourceMap>();

            using (Context.Tracer.DoTrace("ResourceMapActions.ListMaps(enrollmentId={0})", enrollmentId))
            {
                var resourceMapPath = string.Format("Templates/Data/XmlResources/Settings/ResourceMaps/{0}/{1}.pxres", entityId, enrollmentId);
                var resourceMapFile = ContentActions.ListResources(enrollmentId, resourceMapPath, "");

                if (!resourceMapFile.Any())
                {
                    var xDoc = new XmlDocument();
                    XmlNode xRootNode = xDoc.CreateNode(XmlNodeType.Element, "ResourceMaps", "");
                    xDoc.AppendChild(xRootNode);
                    var resource = new Bdc.XmlResource
                    {
                        Status = Bdc.ResourceStatus.Normal,
                        Url = resourceMapPath,
                        EntityId = enrollmentId,
                        Title = "ResourceMap_" + enrollmentId,
                        Body = xDoc.InnerXml
                    };

                    resource.ExtendedProperties.Add("ResourceMap_", Context.EntityId);
                    ContentActions.StoreResources(new List<Bdc.Resource> { resource });

                    resourceMapFile = ContentActions.ListResources(enrollmentId, resourceMapPath, "");
                }

                if (resourceMapFile.Any())
                {
                    var xDoc = new XmlDocument();

                    var stream = resourceMapFile.First().GetStream();
                    string data = "";
                    using (var sw = new System.IO.StreamReader(stream))
                    {
                        data = sw.ReadToEnd();
                    }

                    xDoc.LoadXml(data); //Created the Parent Node.
                    XmlNode rootNode = xDoc.SelectSingleNode("ResourceMaps");
                    if (rootNode == null) return resourceMapList;

                    foreach (var node in rootNode.ChildNodes)
                    {
                        XmlNode item = (XmlNode)node;
                        string mapId = (item.Attributes["Id"] != null) ? item.Attributes["Id"].Value : "";
                        string itemId = (item.Attributes["ItemId"] != null) ? item.Attributes["ItemId"].Value : "";
                        string mapDesc = (item.Attributes["Desc"] != null) ? item.Attributes["Desc"].Value : "";
                        string associatedResourceIds = (item.Attributes["AssociatedItems"] != null) ? item.Attributes["AssociatedItems"].Value : "";

                        List<string> associatedIds = new List<string>();
                        XmlNode associatedItemsNode = item.SelectSingleNode("AssociatedItems");
                        if (associatedItemsNode != null)
                        {
                            foreach (var childNode in associatedItemsNode.ChildNodes)
                            {
                                XmlNode associatedItem = (XmlNode)childNode;
                                string associatedId = (associatedItem.Attributes["Id"] != null) ? associatedItem.Attributes["Id"].Value : "";
                                if (!String.IsNullOrEmpty(associatedId)) associatedIds.Add(associatedId);
                            }
                        }

                        var resourceMap = new Bdc.ResourceMap() { Id = mapId, ItemId = itemId, AssociatedItems = associatedIds, Description = mapDesc };

                        resourceMapList.Add(resourceMap);
                    }
                }
            }
            return resourceMapList;
        }



        /// <summary>
        /// This method will return a comma-delimited of resource IDs for a specific item.
        /// </summary>
        /// <param name="itemId">ID of the item to get resources for.</param>
        /// <returns></returns>
        public string GetResourceIdsForItem(string itemId)
        {
            string resourceIds = "";

            using (Context.Tracer.DoTrace("ResourceMapActions.GetResourceIdsForItem(itemId={0})", itemId))
            {
                var resourceMapList = ListMaps();

                if (resourceMapList.Any())
                {
                    var resourceMap = resourceMapList.Where(r => r.ItemId == itemId);
                    if (resourceMap.Any()) resourceIds = String.Join(",", resourceMap.First().AssociatedItems.ToArray());
                }
            }
            return resourceIds;
        }

        /// <summary>
        /// This method will return a list of resources for a specific item.
        /// </summary>
        /// <param name="itemId">ID of the item to get resources for.</param>
        /// <returns>Collection of resource objects.</returns>
        [Obsolete("This method depends on an IContentActions object.", false)]
        public IEnumerable<Bdc.Resource> GetResourcesForItem(string itemId)
        {
            using (Context.Tracer.DoTrace("ResourceMapActions.GetResourcesForItem(itemId={0})", itemId))
            {
                string resourceIds = "";
                string enrollmentId = Context.EnrollmentId;

                var resourceMapList = ListMaps();

                if (resourceMapList.Any())
                {
                    var resourceMap = resourceMapList.Where(r => r.ItemId == itemId);
                    if (resourceMap.Any()) resourceIds = String.Join(",", resourceMap.First().AssociatedItems.ToArray());
                }
                return ContentActions.ListResources(resourceIds, enrollmentId);
            }
        }

        /// <summary>
        ///  This method will return a list of resources for a specific item with enrollment id
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public IEnumerable<Bdc.Resource> GetResourcesForItem(string itemId, string enrollmentId, string entityId = null)
        {
            using (Context.Tracer.DoTrace("ResourceMapActions.GetResourcesForItem(itemId={0},enrollmentId={1})", itemId, enrollmentId))
            {
                string resourceIds = "";

                var resourceMapList = ListMaps(enrollmentId, entityId);

                if (resourceMapList.Any())
                {
                    var resourceMap = resourceMapList.Where(r => r.ItemId == itemId);
                    if (resourceMap.Any()) resourceIds = String.Join(",", resourceMap.First().AssociatedItems.ToArray());
                }
                return ContentActions.ListResources(resourceIds, enrollmentId);
            }
        }


        /// <summary>
        /// Gets the set of all mappings for the specified resource.
        /// </summary>
        /// <param name="resourceId">The resource ID.</param>
        /// <returns>Collection of resourceMap objects.</returns>
        public IEnumerable<Bdc.ResourceMap> GetMapsForResource(string resourceId)
        {
            using (Context.Tracer.DoTrace("ResourceMapActions.GetMapsForResource(resourceId={0})", resourceId))
            {
                var resourceMapList = ListMaps();

                if (resourceMapList.Any())
                {
                    resourceMapList = resourceMapList.Where(r => r.AssociatedItems.Contains(resourceId));
                }
                return resourceMapList;
            }
        }

        /// <summary>
        /// Get a string defining associations with resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns>Comma-delimited list of ids associated with specified resource.</returns>
        public String GetAssociationsForResource(Bdc.Resource resource)
        {
            using (Context.Tracer.StartTrace("ResourceMapActions.GetAssociationsForResource"))
            {
                string strAssoc = String.Empty;
                string resourceId = this.GetResourceId(resource);
                var mapList = this.GetMapsForResource(resourceId);
                if (mapList.Any())
                {
                    foreach (Bdc.ResourceMap map in mapList)
                    {
                        strAssoc += String.IsNullOrEmpty(strAssoc) ? "" : ",";
                        strAssoc += map.Description;
                    }
                }
                return String.IsNullOrEmpty(strAssoc) ? "None" : strAssoc;
            }
        }

        /// <summary>
        /// This method will return a list of resources for the current enrolled user.
        /// </summary>
        [Obsolete("This method depends on an IContentActions object.", false)]
        public IEnumerable<Bdc.Resource> GetResourcesForEnrollment()
        {
            using (Context.Tracer.StartTrace("ResourceMapActions.GetResourcesForEnrollment"))
            {
                string resourceIds = "";
                var resourceMapList = ListMaps();

                if (resourceMapList.Any())
                {
                    foreach (Bdc.ResourceMap map in resourceMapList)
                    {
                        resourceIds = String.Join(",", map.AssociatedItems.ToArray());
                    }
                }
                return ContentActions.ListResources(resourceIds, Context.EnrollmentId);
            }
        }

        /// <summary>
        /// This method will delete any mapping for specified item ID.
        /// </summary>
        /// <param name="itemId"></param>
        public void DeleteMap(string itemId)
        {
            using (Context.Tracer.DoTrace("ResourceMapActions.DeleteMap(itemId={0})", itemId))
            {
                var mapList = ListMaps().ToList();
                if (mapList.Any())
                {
                    mapList.RemoveAll(r => r.ItemId == itemId);
                    StoreMaps(mapList);
                }
            }
        }

        /// <summary>
        /// This method will add a resource to a mapped item ID.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="itemId">The item ID.</param>
        /// <param name="mapDescription">The mapping description.</param>
        public void AddResourceMap(Bdc.Resource resource, string itemId, string mapDescription)
        {
            using (Context.Tracer.DoTrace("ResourceMapActions.AddResourceMap(itemId={0})", itemId))
            {
                var resourceIds = GetResourceId(resource);

                var mapList = ListMaps().ToList();
                var resourceMap = mapList.Where(r => r.ItemId == itemId);

                if (resourceMap.Any())
                {
                    string originalIds = String.Join(",", resourceMap.First().AssociatedItems.ToArray());
                    resourceMap.First().AssociatedItems = MergedResourceIds(originalIds, resourceIds);
                }
                else
                {
                    Bdc.ResourceMap map = new Bdc.ResourceMap();
                    map.ItemId = itemId;
                    map.AssociatedItems = MergedResourceIds(resourceIds, "");
                    map.MapType = "Map";
                    map.Description = mapDescription;

                    mapList.Add(map);
                }

                StoreMaps(mapList);
            }
        }

        public void AddResourceMap(Bdc.Resource resource, string itemId, string mapDescription, string applicableEntityId)
        {
            using (Context.Tracer.DoTrace("ResourceMapActions.AddResourceMap(itemId={0})", itemId))
            {
                var resourceIds = GetResourceId(resource);

                var mapList = ListMaps().ToList();
                var resourceMap = mapList.Where(r => r.ItemId == itemId);

                if (resourceMap.Any())
                {
                    string originalIds = String.Join(",", resourceMap.First().AssociatedItems.ToArray());
                    resourceMap.First().AssociatedItems = MergedResourceIds(originalIds, resourceIds);
                }
                else
                {
                    Bdc.ResourceMap map = new Bdc.ResourceMap();
                    map.ItemId = itemId;
                    map.AssociatedItems = MergedResourceIds(resourceIds, "");
                    map.MapType = "Map";
                    map.Description = mapDescription;

                    mapList.Add(map);
                }

                StoreMaps(mapList, applicableEntityId, resource.EntityId);
            }
        }

        /// <summary>
        /// Returns true if the item is mapped, false otherwise.
        /// </summary>
        /// <param name="itemId">The item ID.</param>
        /// <returns>
        ///   <c>true</c> if the specified item ID is mapped; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMapped(string itemId)
        {
            bool isMapped = !String.IsNullOrEmpty(GetResourceIdsForItem(itemId));
            return isMapped;
        }

        /// <summary>
        /// This method will add resources to a mapped item ID.
        /// </summary>
        /// <param name="resourceIds">The resource IDs.</param>
        /// <param name="itemId">ID of the item to map to.</param>
        /// <param name="mapDescription">The mapping description.</param>
        public void AddResourceMap(string resourceIds, string itemId, string mapDescription)
        {
            using (Context.Tracer.DoTrace("ResourceMapActions.AddResourceMap(resourceIds={0},itemId={1})", resourceIds, itemId))
            {
                var mapList = ListMaps().ToList();
                var resourceMap = mapList.Where(r => r.ItemId == itemId);

                if (resourceMap.Any())
                {
                    string originalIds = String.Join(",", resourceMap.First().AssociatedItems.ToArray());
                    resourceMap.First().AssociatedItems = MergedResourceIds(originalIds, resourceIds);
                    resourceMap.First().Description = mapDescription;
                }
                else
                {
                    Bdc.ResourceMap map = new Bdc.ResourceMap();
                    map.ItemId = itemId;
                    map.AssociatedItems = MergedResourceIds(resourceIds, "");
                    map.MapType = "Map";
                    map.Description = mapDescription;

                    mapList.Add(map);
                }

                StoreMaps(mapList);
            }
        }

        /// <summary>
        /// Helper method to find the ID of a resource from its path.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns></returns>
        public string GetResourceId(Bdc.Resource resource)
        {
            using (Context.Tracer.StartTrace("ResourceMapActions.GetResourceId"))
            {
                string resId = String.Empty;
                if (!String.IsNullOrEmpty(resource.Url))
                {
                    resId = System.IO.Path.GetFileNameWithoutExtension(resource.Url).Replace(".", "");
                }

                return resId;
            }
        }

        /// <summary>
        /// Gets the resource link.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns>Url string pointing to specified resource.</returns>
        public string GetResourceLink(Bdc.Resource resource)
        {
            using (Context.Tracer.StartTrace("ResourceMapActions.GetResourceLink"))
            {
                string resourceDocUrl = "";

                string resourceTitle = String.Empty;
                resourceTitle = !String.IsNullOrEmpty(resource.Name) ? resource.Name : "Document Title";

                string resourceLink = "<a href=" + resourceDocUrl + " class='fne-link'>" + resourceTitle + "</a>";

                return resourceTitle;
            }
        }

        /// <summary>
        /// Gets the content of the resource from its stream.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns></returns>
        public string GetResourceContent(Bdc.Resource resource)
        {
            using (Context.Tracer.StartTrace("ResourceMapActions.GetResourceContent"))
            {
                return resource.GetStream().AsString();
            }
        }

        #endregion
    }
}