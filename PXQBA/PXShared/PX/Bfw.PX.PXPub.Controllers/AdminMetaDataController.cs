using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Xml.XPath;
using Bfw.Agilix.DataContracts;
using Bfw.Common.DynamicExtention;
using Bfw.Common.Logging;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using Microsoft.Practices.ServiceLocation;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using ContentItem = Bfw.PX.PXPub.Models.ContentItem;
using FacetedSearchQuery = Bfw.PX.PXPub.Models.FacetedSearchQuery;
using FacetField = Bfw.PX.PXPub.Models.FacetField;
using SearchQuery = Bfw.PX.PXPub.Models.SearchQuery;

namespace Bfw.PX.PXPub.Controllers
{


	[PerfTraceFilter]
	public class AdminMetaDataController : Controller
	{

		private const string _constXPathToElasticControl = "./bfw_metadata_admin/metaDataElements/metaDataElement[@name='{0}']/elasticMetaContorl";
		private const string _constXPathToElasticNodeToSave = "./bfw_metadata_admin/metaDataElements/metaDataElement[@name='{0}']/elasticMetaNodeToSave";
		/// <summary>
		/// Access to the current business context information.
		/// </summary>
		/// <value>
		/// The context.
		/// </value>
		protected BizSC.IBusinessContext Context { get; set; }

		protected BizSC.IAdminMetaDataActions AdminMetaDataActions { get; set; }

		/// <summary>
		/// Access to an IContentActions implementation
		/// </summary>
		/// <value>
		/// The content actions.
		/// </value>
		protected BizSC.IContentActions ContentActions { get; set; }


		/// <summary>
		/// The ICourseActions.
		/// </summary>
		protected ICourseActions CourseActions { get; set; }

		/// <summary>
		/// The ICourseActions.
		/// </summary>
		protected ISearchActions SearchActions { get; set; }

		public AdminMetaDataController(BizSC.IBusinessContext context, BizSC.IContentActions contentActions, ICourseActions courseActions, ISearchActions searchActions, IAdminMetaDataActions adminMetaDataActions)
		{
			Context = context;
			ContentActions = contentActions;
			CourseActions = courseActions;
			SearchActions = searchActions;
			AdminMetaDataActions = adminMetaDataActions;
		}


		/// <summary>
		/// Delete ResourceSubType from all resources
		/// </summary>
		/// <returns>List of Resource Sub Types</returns>	
		[HttpPost]
		public ActionResult Delete_ResourceSubType(string deleteValue)
		{
			string oldResourceSubType = deleteValue;

			////do a search 
			//BizDC.SearchQuery query = new BizDC.SearchQuery
			//{
			//    EntityId = Context.Course.ProductCourseId,
			//    ExactQuery = String.Format("/meta-content-type= '{0}'", oldResourceSubType)
			//};
			//BizDC.SearchResultSet searchResultSet = SearchActions.DoSearch(query);
			//AdminMetaDataActions.DeleteResourceSubType(searchResultSet);

			string query = String.Format("/meta-content-type= '{0}' OR /meta-content-type= '{1}'", "instructor_" + oldResourceSubType, "student_" + oldResourceSubType);

			List<BizDC.ContentItem> foundItems = null;

			using (Context.Tracer.DoTrace("QuestionAdminActions.GetCourseQuizzesForSelectedChapters()"))
			{
				ItemSearch itemsSearchQuery = new ItemSearch()
				{
					EntityId = Context.EntityId,
					Query = query
				};

				var items = ContentActions.FindContentItems(itemsSearchQuery);

				if (items != null) foundItems = items.ToList();
			}
			AdminMetaDataActions.DeleteResourceSubType(foundItems);

			return PopulateMetaDataDropDownFor_ResourceSubType();
		}


		/// <summary>
		/// Update ResourceSubType for all resources
		/// </summary>
		/// <returns>List of Resource Sub Types</returns>	
		[HttpPost]
		public ActionResult Update_ResourceSubType(string oldValue, string newValue)
		{
			string oldResourceSubType = oldValue;
			string newResourceSubType = newValue;

			////do a search 
			//BizDC.SearchQuery query = new BizDC.SearchQuery
			//                          {
			//                              EntityId = Context.EntityId,
			//                              ExactQuery = String.Format("/meta-content-type= '{0}' OR /meta-content-type= '{1}'", "instructor_" + oldResourceSubType, "student_" + oldResourceSubType)
			//                          };
			//BizDC.SearchResultSet searchResultSet = SearchActions.DoSearch(query);


			string query = String.Format("/meta-content-type= '{0}' OR /meta-content-type= '{1}'", "instructor_" + oldResourceSubType, "student_" + oldResourceSubType);

			List<BizDC.ContentItem> foundItems = null;

			using (Context.Tracer.DoTrace("QuestionAdminActions.GetCourseQuizzesForSelectedChapters()"))
			{
				ItemSearch itemsSearchQuery = new ItemSearch()
				{
					EntityId = Context.EntityId,
					Query = query
				};

				var items = ContentActions.FindContentItems(itemsSearchQuery);

				if (items != null) foundItems = items.ToList();
			}


			AdminMetaDataActions.UpdateResourceSubType(newResourceSubType, foundItems);


			//AdminMetaDataActions.UpdateResourceSubType(newResourceSubType, searchResultSet);

			return PopulateMetaDataDropDownFor_ResourceSubType();
		}

		/// <summary>
		/// Populate MetaData DropDown For ResourceSubType 
		/// </summary>
		/// <returns>List of Resource Sub Types</returns>	
		[HttpGet]
		public ActionResult PopulateMetaDataDropDownFor_ResourceSubType()
		{
			//do a faceted search for units(chapters)
			SearchQuery query = new SearchQuery
								{
									IsFaceted = true,
									FacetedQuery = new FacetedSearchQuery
												  {
													  Fields = { "meta-content-type_dlap_e" },
													  Limit = -1,
													  MinCount = 1,
												  },
									EntityId = Context.EntityId
								};

			Dictionary<string, string> resourceTypesToDisplay = new Dictionary<string, string>();

			GetResourseSubTypesForProductCourse(query, resourceTypesToDisplay);

			GetResourseSubTypesForSandBoxCourse(query, resourceTypesToDisplay);

			resourceTypesToDisplay = resourceTypesToDisplay.Distinct().OrderBy(r => r.Value).ToDictionary(res => res.Key, res => res.Value);

			return Json(resourceTypesToDisplay, JsonRequestBehavior.AllowGet);
		}

		private void GetResourseSubTypesForSandBoxCourse(SearchQuery query, Dictionary<string, string> resourceTypesToDisplay)
		{
			//GetResourseSubTypesForSandBoxCourse:
			SearchActions.DoProductSearch(false);

			BizDC.SearchQuery bizSearchQuery = query.ToSearchQuery();

			SearchResultSet bizResult = SearchActions.DoSearch(bizSearchQuery);

			FacetedSearchResults searchResultsForSandBox = bizResult.ToFacetedSearchResults();

			List<FacetField> facetFields = searchResultsForSandBox.FacetFields;

			Dictionary<string, string> sandBoxCourseResourceTypes = new Dictionary<string, string>();
			facetFields.ForEach(f => f.FieldValues.ForEach(v => sandBoxCourseResourceTypes.Add(v.Value, v.Value)));

			foreach (var resourceType in sandBoxCourseResourceTypes)
			{
				var key = resourceType.Key;
				var val = resourceType.Value;

				key = key.Replace("student_", "").Replace("instructor_", "").Replace(" ", "").ToLower();
				val = val.Replace("student_", "").Replace("instructor_", "");

				if (key == "" || val == "") continue;
				if (resourceTypesToDisplay.ContainsKey(key) | resourceTypesToDisplay.ContainsValue(val)) continue;

				resourceTypesToDisplay.Add(key, val);
			}
		}

		/// <summary>
		/// Get ResourseSubTypes For ProductCourse
		/// </summary>
		/// <param name="query"></param>
		/// <param name="resourceTypesToDisplay"> </param>
		private static void GetResourseSubTypesForProductCourse(SearchQuery query, Dictionary<string, string> resourceTypesToDisplay)
		{
			//get search controller instance
			SearchController searchController = ServiceLocator.Current.GetInstance(typeof(SearchController)) as SearchController;

			FacetedSearchResults searchResultsForProductCourse = searchController.DoFacetedSearch(query);

			Dictionary<string, string> productCourseResourceTypes = new Dictionary<string, string>();

			searchResultsForProductCourse.FacetFields.ForEach(
				f => f.FieldValues.ForEach(v => productCourseResourceTypes.Add(v.Value, v.Value)));

			foreach (var resourceType in productCourseResourceTypes)
			{
				var key = resourceType.Key.Replace("student_", "").Replace("instructor_", "").Replace(" ", "").ToLower();
				var val = resourceType.Value.Replace("student_", "").Replace("instructor_", "");
				if (resourceTypesToDisplay.ContainsKey(key) | resourceTypesToDisplay.ContainsValue(val))
				{ }
				else
					resourceTypesToDisplay.Add(key, val);
			}

		}


		/// <summary>
		/// MetaDataIndex Displays the  MetaDataLayout 
		/// </summary>
		/// <param name="id"> </param>
		/// <returns>The Metadata Layout settings</returns>\	
		[HttpGet]
		public ViewResult MetaDataIndex(string id)
		{
			if (id == null) throw new ArgumentNullException("id");
			var contentItem = GetContentItem(id);
			// Render the MetadataLayout
			return View(contentItem);
		}

		/// <summary>
		/// MetaDataIndex Displays the  MetaDataLayout 
		/// </summary>
		/// <param name="contentItemId"> </param>
		/// <returns>The Metadata Layout settings</returns>
		[HttpPost]
		//[AjaxMethod]
		[ActionName("MetaDataIndex")]
		public ActionResult SaveMetaData(string id)
		{
			if (id == null) throw new ArgumentNullException("id");

			var contentItem = GetContentItem(id);

			string guid = GenerateUniqueId();

			foreach (MetaDataElement metaDataElement in contentItem.AdminMetaData.MetaData.Elements)
			{
				var formValue = Request.Params[metaDataElement.Name];
				XElement xEl = contentItem.ItemDataXml.XPathSelectElement(metaDataElement.XPath);

				if (RemoveItemDataElement_ToContinueBeforeSave(xEl, metaDataElement, formValue, guid)) continue;

				var nodeXml = CreateItemDataElement_BeforeSave(contentItem, metaDataElement, ref xEl);

				if (!xEl.HasElements)
					xEl.SetValue(formValue);
				else
				{
					xEl.SetAttributeValue("stamp", guid);
					string xChildElName = metaDataElement.XElasticMetaNodeToSave.FirstAttribute.Value;

					for (int i = xEl.Descendants(xChildElName).Count() - 1; i > -1; i--)
					{
						XElement dXElement = xEl.Descendants(xChildElName).ToArray()[i];

						if (dXElement.Attribute("stamp") != null && dXElement.Attribute("stamp").Value == guid) continue;
						dXElement.Remove();

					}

					string[] separator = new[] { "|," };
					string[] values = formValue.Split(separator, StringSplitOptions.RemoveEmptyEntries);

					foreach (var val in values)
					{
						var valueToSave = val.Replace("|", "");
						XElement xChildEl = metaDataElement.XElasticMetaNodeToSave.Descendants(xChildElName).FirstOrDefault();
						if (xChildEl == null) continue;

						xChildEl.SetAttributeValue("stamp", guid);
						xChildEl.SetValue(valueToSave);
						xEl.Add(xChildEl);
					}

				}
			}


			XElement metaDataXml = contentItem.ItemDataXml.XPathSelectElement("./bfw_metadata_admin");
			if (metaDataXml != null)
				metaDataXml.Remove();
			ContentActions.SaveRowItem(Context.EntityId, contentItem.ItemDataXml, id);
			contentItem = GetContentItem(id);

			return View("MetaDataIndex", contentItem);

		}

		private static string CreateItemDataElement_BeforeSave(ContentItem contentItem, MetaDataElement metaDataElement,
															   ref XElement xEl)
		{
			SetXPathNodeToSave(metaDataElement.Name, contentItem, metaDataElement);
			string nodeXml = metaDataElement.XElasticMetaNodeToSave.FirstNode.ToString();
			if (xEl == null || !xEl.HasElements)
			{
				xEl = XElement.Parse(nodeXml);
				contentItem.ItemDataXml.Add(xEl);
			}
			return nodeXml;
		}

		private static bool RemoveItemDataElement_ToContinueBeforeSave(XElement xEl, MetaDataElement metaDataElement, string formValue, string guid)
		{
			//if this is not static (ReadOnly) element:
			if (formValue == null)
			{
				if (!metaDataElement.Action.ToLower().Contains("static") & xEl != null)
				{
					if (xEl.HasElements)
					{
						if (xEl.Attribute("stamp") == null || ( xEl.Attribute("stamp") != null && xEl.Attribute("stamp").Value != guid ))
							xEl.RemoveNodes();
					}
					else
						xEl.Value = "";
					if (xEl.HasAttributes) xEl.RemoveAttributes();
				}

				return true;
			}
			return false;
		}

		private ContentItem GetContentItem(string contentItemId)
		{
			Biz.DataContracts.ContentItem bizContentItem = ContentActions.GetContent(Context.EntityId, contentItemId);
			Models.ContentItem contentItem = bizContentItem.ToContentItem(ContentActions);
			const string defaultMetadataTemplete = "PX_default_metadata_defination";

			XElement metaDataXml = contentItem.ItemDataXml.XPathSelectElement("./bfw_metadata_admin");
			if (metaDataXml != null)
				metaDataXml.Remove();

			var contentTemplate = ContentActions.FindTemplateForType(contentItem.Type);
			if (contentTemplate != null)
			{
				metaDataXml = contentTemplate.ItemDataXml.XPathSelectElement("./bfw_metadata_admin");
				if (metaDataXml != null)
				{
					contentItem.AdminMetaData = ContentItemMapper.GetAdminMetaData(contentTemplate);
					contentItem.ItemDataXml.Add(metaDataXml);
				}
			}
			if (contentItem.AdminMetaData == null)
			{
				var bizDefinationItem = ContentActions.GetContent(Context.EntityId, defaultMetadataTemplete);
				if (bizDefinationItem != null)
				{
					contentItem.AdminMetaData = ContentItemMapper.GetAdminMetaData(bizDefinationItem);
					metaDataXml = bizDefinationItem.ItemDataXml.XPathSelectElement("./bfw_metadata_admin");
					contentItem.ItemDataXml.Add(metaDataXml);
				}
			}
			return contentItem;
		}


		/// <summary>
		/// Displays MetaData as Static Text
		/// </summary>
		public PartialViewResult MetaDataStaticText()
		{
			// Render the  Metadata Element
			return PartialView(GetDynamicObject());
		}

		private MetaDataElement GetDynamicObject()
		{

			ContentItem contentItem = RouteData.Values["contentItem"] as ContentItem;
			MetaDataElement metaDataElement = RouteData.Values["metaDataElement"] as MetaDataElement;

			if (metaDataElement != null && metaDataElement.XPath != null && contentItem != null && contentItem.ItemDataXml != null)
			{
				string elementName = metaDataElement.Name;

				SetXPathElements(elementName, contentItem, metaDataElement);

				if (metaDataElement.XElasticData != null)
				{
					if (elementName == "ResourceUserGroup") ResourceUserGroup(metaDataElement);
					if (elementName == "ResourceSubType") ResourceSubType(metaDataElement);

				}

				SetElasticObjects(metaDataElement);
			}

			return metaDataElement;

		}



		private static void ResourceUserGroup(MetaDataElement metaDataElement)
		{
			if (metaDataElement.XElasticControl == null) return;

			string rawValue = metaDataElement.XElasticData.Value;
			var resourceSubType = ResourceSubType(metaDataElement);

			var nodes = metaDataElement.XElasticControl.XPathSelectElements(".//input");

			foreach (var xRadioInput in nodes)
			{
				string radioButtonValue = xRadioInput.Attribute("value").Value;
				if (rawValue.Contains(radioButtonValue)) xRadioInput.SetAttributeValue("checked", "checked");

				xRadioInput.Attribute("value").Value += resourceSubType;
			}
		}

		private static void SetXPathElements(string elementName, ContentItem contentItem, MetaDataElement metaDataElement)
		{
			SetXPathControl(elementName, contentItem, metaDataElement);

			SetXPathData(contentItem, metaDataElement);
		}

		private static void SetXPathControl(string elementName, ContentItem contentItem, MetaDataElement metaDataElement)
		{
			XElement xEl = contentItem.ItemDataXml.XPathSelectElement(String.Format(_constXPathToElasticControl, elementName));
			if (xEl != null)
			{
				string s = xEl.ToString();
				s = s.Replace("-", "_");
				metaDataElement.XElasticControl = XElement.Parse(s);
			}
		}

		private static void SetXPathNodeToSave(string elementName, ContentItem contentItem, MetaDataElement metaDataElement)
		{
			XElement xEl = contentItem.ItemDataXml.XPathSelectElement(String.Format(_constXPathToElasticNodeToSave, elementName));
			if (xEl != null)
			{
				string s = xEl.ToString();
				metaDataElement.XElasticMetaNodeToSave = XElement.Parse(s);
			}
		}


		private static void SetXPathData(ContentItem contentItem, MetaDataElement metaDataElement)
		{
			XElement xEl = contentItem.ItemDataXml.XPathSelectElement(metaDataElement.XPath);
			if (xEl != null)
			{
				string s = xEl.ToString();
				s = s.Replace("-", "_");
				metaDataElement.XElasticData = XElement.Parse(s);
			}
		}

		private static void SetElasticObjects(MetaDataElement metaDataElement)
		{
			if (metaDataElement.XElasticControl != null) metaDataElement.ElasticControl = metaDataElement.XElasticControl.ToElastic();

			if (metaDataElement.XElasticData != null) metaDataElement.ElasticData = metaDataElement.XElasticData.ToElastic();
		}

		private static string ResourceSubType(MetaDataElement metaDataElement)
		{
			string resource = metaDataElement.XElasticData.Value;
			int i = resource.IndexOf("_");
			resource = resource.Substring(i + 1);
			metaDataElement.XElasticData.Value = resource;
			return resource;
		}


		/// <summary>
		/// Displays MetaData as Input Text
		/// </summary>

		public PartialViewResult MetaDataInputText()
		{
			// Render the  Metadata Element
			return PartialView(GetDynamicObject());
		}

		/// <summary>
		/// Displays MetaData as DropDownList
		/// </summary>

		public PartialViewResult MetaDataDropDownList()
		{
			// Render the  Metadata Element
			return PartialView(GetDynamicObject());
		}


		public PartialViewResult MetaDataResourceSubType()
		{
			// Render the  Metadata Element
			return PartialView(GetDynamicObject());
		}


		/// <summary>
		/// Displays MetaData as Static Text
		/// </summary>

		public PartialViewResult MetaDataRadioButtonList()
		{
			// Render the  Metadata Element
			return PartialView(GetDynamicObject());
		}


		/// <summary>
		/// Display MetaDataChaptersAssociations
		/// </summary>
		/// <returns></returns>
		public PartialViewResult MetaDataChaptersAssociations()
		{
			// Render the  Metadata Element
			return PartialView(GetDynamicObject());
		}


		[HttpGet]
		[ActionName("AssociatedChaptersDropDown")]
		public ActionResult AssociatedChaptersDropDown(object data)
		{
			if (data == null) return null;
			IEnumerable<SelectListItem> chapters = null;

			string[] associationsTypes = data as string[];

			if (associationsTypes != null && associationsTypes[0] != null)
			{
				MetaDataAssociationType associationsType;
				Enum.TryParse(associationsTypes[0], true, out associationsType);

				//GET Chapters
				chapters = PopulateChaptersList(Context.EntityId, associationsType);

				ViewData["accociationsType"] = associationsType;
			}

			return PartialView(chapters);
		}


		/// <summary>
		/// Populate Chapters List
		/// </summary>
		/// <param name="entityId"> </param>
		/// <param name="associationsType"> </param>
		/// <returns></returns>
		private IEnumerable<SelectListItem> PopulateChaptersList(string entityId, MetaDataAssociationType associationsType)
		{
			const string containerName = "LaunchPad";
			const string subContainerId = "";
			const string parentId = "PX_LOR";
			const string tempCategory = "";

			if (associationsType == MetaDataAssociationType.none) return null;
			List<Biz.DataContracts.ContentItem> chapters;

			//if (associationsType == MetaDataAssociationType.ebookfilter)
			//{
			//    using (Context.Tracer.StartTrace("AdminMetaDataActions.GetCourseChapters"))
			//    {
			//        var SearchParameters = ItemQueryHelper.BuildListChildrenQuery(entityId, parentId, 1, tempCategory, Context.CurrentUser.Id);
			//        chapters = ContentActions.FindContentItems(SearchParameters).ToList();
			//    }
			//}
			//else
			//{

			using (Context.Tracer.StartTrace("AdminMetaDataActions.GetCourseChapters"))
			{
				chapters = CourseActions.LoadContainerData(Context.Course, containerName, subContainerId).ToList();
			}

			//}

			IEnumerable<SelectListItem> chaptersSelectList = chapters.Select(item => new SelectListItem
																							{
																								Text = item.Title + " ",
																								Value = item.Title + "|"
																								//item.Id
																							});

			chaptersSelectList.ToList().Add(new SelectListItem { Text = "Select", Value = "0", Selected = true });

			return chaptersSelectList;
		}

		private string GenerateUniqueId1()
		{
			using (var rng = new RNGCryptoServiceProvider())
			{
				// change the size of the array depending on your requirements
				var rndBytes = new byte[8];
				rng.GetBytes(rndBytes);
				return BitConverter.ToString(rndBytes).Replace("-", "");
			}
		}

		private string GenerateUniqueId()
		{
			return Guid.NewGuid().ToString("N");
		}
	}
}
