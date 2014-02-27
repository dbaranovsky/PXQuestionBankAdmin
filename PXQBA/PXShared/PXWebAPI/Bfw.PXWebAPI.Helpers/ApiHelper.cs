using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Collections;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using UserType = Bfw.PXWebAPI.Models.UserType;

namespace Bfw.PXWebAPI.Helpers
{
	/// <summary>
	/// Helper Functions
	/// </summary>
	public class ApiHelper
	{
		/// <summary>
		/// GetPostParameters
		/// </summary>
		/// <param name="httpRequest"></param>
		/// <returns></returns>
		public static Dictionary<string, string> GetPostParameters(HttpRequest httpRequest)
		{
			var retParams = new Dictionary<string, string>();
			foreach (string name in httpRequest.Form)
			{
				if (retParams.ContainsKey(name))
					retParams[name] = httpRequest.Form[name];
				else
					retParams.Add(name, httpRequest.Form[name]);
			}
			return retParams;
		}


		/// <summary>
		/// Initialize PxBusinessContext
		/// </summary>
		/// <param name="businessContext"></param>
		/// <param name="apiCourseActions"></param>
		/// <param name="ApiUserActions"> </param>
		/// <param name="courseid"></param>
		/// <param name="userRole"></param>
		public static void InitializeInstructorPxBusinessContext(IBusinessContext businessContext, IApiCourseActions apiCourseActions, IApiUserActions ApiUserActions,
													   string courseid, string userRole)
		{
			InitializePxBusinessContext(businessContext, apiCourseActions, courseid, userRole);

			var userid = businessContext.Course.CourseOwner;

			if (string.IsNullOrEmpty(userid)) return;

			businessContext.CurrentUser = new UserInfo { Id = userid };

			businessContext.CurrentUser = businessContext.GetNewUserData();

			var enrolments = ApiUserActions.GetUserEnrollments(userid, courseid);

			if (!enrolments.Any(e => e.Course != null && e.Course.Id == courseid)) return;

			var enrolment = enrolments.First(e => e.Course != null && e.Course.Id == courseid);

			businessContext.EnrollmentId = enrolment.Id;
		}


		/// <summary>
		/// Initialize PxBusinessContext
		/// </summary>
		/// <param name="businessContext"></param>
		/// <param name="apiCourseActions"></param>
		/// <param name="courseid"></param>
		/// <param name="userRole"></param>
		public static void InitializePxBusinessContext(IBusinessContext businessContext, IApiCourseActions apiCourseActions, string courseid, string userRole)
		{
			Bfw.PX.Biz.DataContracts.Course course = Bfw.PX.Biz.Services.Mappers.BizEntityExtensions.ToCourse(apiCourseActions.GetCourseByCourseId(courseid));

			businessContext.Course = course;
			businessContext.CourseId = courseid;
			businessContext.ProductCourseId = course.ProductCourseId;

			businessContext.InitializeDomains();

			var typeOfUser = (UserType)Enum.Parse(typeof(UserType), userRole);
			businessContext.AccessLevel = PX.Biz.ServiceContracts.AccessLevel.Student;
			if (typeOfUser == UserType.Instructor) businessContext.AccessLevel = PX.Biz.ServiceContracts.AccessLevel.Instructor;
			
		}


		/// <summary>
		/// GetPostParameters
		/// </summary>
		/// <param name="key"> </param>
		/// <returns></returns>
		public static string GetFormRequestParameter(string key)
		{

			return HttpContext.Current.Request.Form[key] ?? "";

		}

		/// <summary>
		/// GetCategoryName
		/// </summary>
		/// <param name="courseId"></param>
		/// <param name="categoryId"></param>
		/// <param name="sessionManager"></param>
		/// <param name="apiGradeBookActions"> </param>
		/// <returns></returns>
		public static string GetCategoryName(string courseId, string categoryId, ISessionManager sessionManager, IApiGradeBookActions apiGradeBookActions)
		{
			var gbWeights = apiGradeBookActions.GetGradeBookWeights(courseId);
			if (gbWeights != null)
			{
				var gwCategories = gbWeights.GradeWeightCategories;
				if (!gwCategories.IsNullOrEmpty())
				{
					return ( from swCategory in gwCategories where swCategory.Id.Equals(categoryId) select swCategory.Text ).FirstOrDefault();
				}

			}
			return null;
		}


		/// <summary>
		/// Content items sort.
		/// </summary>
		/// <param name="l1">The l1.</param>
		/// <param name="l2">The l2.</param>
		/// <returns></returns>
		public static int ContentItemSort(PX.PXPub.Models.ContentItem l1, PX.PXPub.Models.ContentItem l2)
		{
			if (l1.DueDate == l2.DueDate)
			{
				try { return System.String.CompareOrdinal(l1.Title, l2.Title); }
				catch { return 0; }
			}

			var dueDate = l1.DueDate;

			return dueDate.CompareTo(l2.DueDate);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="entityId"></param>
		/// <param name="queryParams"></param>
		/// <param name="userId"></param>
		/// <param name="op"></param>
		/// <returns></returns>
		public static List<Bfw.Agilix.DataContracts.Item> DoItemSearch(ISession session, string entityId, Dictionary<string, string> queryParams, string userId, string op = "OR")
		{
			var queryCmd = new GetItems()
			{
				SearchParameters = BuildItemSearchQuery(entityId, queryParams, userId, op)
			};
			var batch = new Batch();
			batch.Add(queryCmd);

			session.ExecuteAsAdmin(batch);

			var items = queryCmd.Items;
			if (!queryCmd.Items.IsNullOrEmpty())
			{
				var tempChildren = queryCmd.Items.ToList();
				tempChildren.RemoveAll(i => i.Id.ToLowerInvariant().StartsWith("shortcut__"));
				items = tempChildren.ToList();
			}

			return items;
		}

		/// <summary>
		/// Build query for custom Agilix get items command
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="queryParams"></param>
		/// <param name="userId"></param>
		/// <param name="op">logical operator to use in query</param>
		/// <returns></returns>
		public static Bfw.Agilix.DataContracts.ItemSearch BuildItemSearchQuery(string entityId, Dictionary<string, string> queryParams, string userId, string op)
		{
			var search = new Bfw.Agilix.DataContracts.ItemSearch()
			{
				EntityId = entityId,
				Query = ""
			};
			var queryExpressions = queryParams.Keys.Select(key => string.Format(@"/{0}='{1}'", key, queryParams[key])).ToList();
			search.Query = string.Join(" " + op + " ", queryExpressions);
			return search;
		}



		/// <summary>
		/// Builds query for the agilix ListChildren command.
		/// </summary>
		/// <param name="entityId">The entity id.</param>
		/// <param name="parentId">The parent id.</param>
		/// <param name="depth">The depth.</param>
		/// <param name="categoryId">The category id.</param>
		/// <param name="userId">The user id.</param>
		/// <param name="isCourseSync"> </param>
		/// <returns></returns>
		public static Bfw.Agilix.DataContracts.ItemSearch BuildListChildrenQuery(string entityId, string parentId, int depth, string categoryId, string userId, bool isCourseSync = false)
		{
			var search = new Bfw.Agilix.DataContracts.ItemSearch()
			{
				EntityId = entityId,
				ItemId = parentId,
				Depth = depth
			};
			if (isCourseSync == true)
			{

				search.Query = string.Format("/parent='{0}'", parentId);
			}
			else if (categoryId == Constants.USE_AGILIX_PARENT)
			{
				// Here we don't do any special query; we just increase the
				// depth by one to make sure that the children are loaded.
				depth += 1;
			}
			else if (categoryId == System.Configuration.ConfigurationManager.AppSettings["MyMaterials"])
			{
				search.Query = string.Format(@"/bfw_tocs/my_materials@parentid='{0}'", categoryId + "_" + userId);

			}
			else if (categoryId == System.Configuration.ConfigurationManager.AppSettings["MyQuizes"])
			{
				//search.Query = string.Format(@"/bfw_tocs/my_materials@parentid='{0}'", System.Configuration.ConfigurationManager.AppSettings["MyMaterials"] + "_" + userId) + "AND /type='Assessment'  or /type='Homework'";
				search.Query = string.Format(@"/bfw_tocs/syllabusfilter@parentid<>'{0}'", "") + "AND /type='Assessment'  or /type='Homework'";
			}
			else if (!string.IsNullOrEmpty(categoryId))
			{
				search.Query = string.Format(@"/bfw_tocs/{0}@parentid='{1}'", categoryId, parentId);
			}
			else
			{

				search.Query = string.Format(@"/bfw_tocs/bfw_toc_contents@parentid='{0}'", parentId);
			}

			return search;
		}

		/// <summary>
		/// Gets the student items 
		/// </summary>
		/// <param name="iContentActions">content actions</param>
		/// <param name="enrollmentId">enrollment id</param>
		/// <param name="categoryId">category id</param>
		/// <returns></returns>
		public static IEnumerable<Bfw.Agilix.DataContracts.Item> GetStudentItems(IContentActions iContentActions, string enrollmentId, string categoryId)
		{
			var studentResourceDoc = iContentActions.GetResource(enrollmentId, iContentActions.GetStudentResourceId(enrollmentId));
			var allStudentItems = new List<Bfw.Agilix.DataContracts.Item>();

			if (!categoryId.IsNullOrEmpty())
			{
				studentResourceDoc = iContentActions.GetResource(categoryId, iContentActions.GetStudentResourceId(categoryId));
			}

			if (studentResourceDoc != null)
			{
				var stream = studentResourceDoc.GetStream();
				XDocument doc = stream != null && stream.Length > 0
									? XDocument.Load(stream)
									: iContentActions.GetEmptyStudentDoc();

				foreach (var itemElement in doc.Root.Elements("item"))
				{
					var item = new Bfw.Agilix.DataContracts.Item();
					item.ParseEntity(itemElement);
					allStudentItems.Add(item);
				}
			}

			return allStudentItems;
		}
	}
}
