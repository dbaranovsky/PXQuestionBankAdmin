using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Collections;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.PXWebAPI.Helpers;
using Bfw.PXWebAPI.Mappers;
using Bfw.PXWebAPI.Models;
using Bfw.PXWebAPI.Models.Response;
using Ac = Bfw.Agilix.Commands;
using Adc = Bfw.Agilix.DataContracts;

namespace Bfw.PXWebAPI.Controllers
{
	/// <summary>
	/// GradesController
	/// </summary>
	public class GradesController : ApiController
	{

		protected ISessionManager SessionManager { get; set; }

		protected IBusinessContext Context { get; set; }

		protected Helpers.IApiGradeBookActions ApiGradeBookActions { get; set; }

		protected Helpers.IApiGradeActions ApiGradeActions { get; set; }

		protected Helpers.IApiItemActions ApiItemActions { get; set; }

		/// <summary>
		/// GradesController
		/// </summary>
		/// <param name="sessionManager"></param>
		/// <param name="context"> </param>
		/// <param name="apiGradeBookActions"> </param>
		/// <param name="apiGradeActions"> </param>
		/// <param name="apiItemActions"> </param>
		public GradesController(ISessionManager sessionManager, PX.Biz.ServiceContracts.IBusinessContext context, Helpers.IApiGradeBookActions apiGradeBookActions,
																												  Helpers.IApiGradeActions apiGradeActions,
																												  Helpers.IApiItemActions apiItemActions)
		{
			SessionManager = sessionManager;
			Context = context;
			ApiGradeBookActions = apiGradeBookActions; //new ApiGradeBookActions(sessionManager, context);
			ApiGradeActions = apiGradeActions; //new ApiGradeActions(sessionManager, context);
			ApiItemActions = apiItemActions; //new ApiItemActions(sessionManager, context);
		}

		/// <summary>
		/// Get Grades by EntityId and following optional POST Parameters: 
		/// - lastupdated  
		/// - "|" separated list of itemids 
		/// - "|" separated list of userids 
        /// Test URL: http://dev.px.bfwpub.com/api/grades/details/112284
		/// POST: userids=99630  itemids=bsi__48454DA4__B0D2__45F8__9895__FCFEBB049B3F
        /// Grade Status: http://dev.dlap.bfwpub.com/js/docs/#!/Enum/GradeStatus
		/// </summary>
		/// <param name="id">The course id</param>
		/// <returns>GradesItemListResponse</returns>
		[HttpPost]
		[ActionName(Helper.DETAILS)]
		public GradesItemListResponse Details(string id)
		{
			var response = new GradesItemListResponse();
			var gradeItemList = new List<GradesItem>();

			var lastUpdated = ApiHelper.GetFormRequestParameter(Helper.LASTUPDATED);
            
            var toc = ApiHelper.GetFormRequestParameter(Helper.TOC);
            if (toc.IsNullOrEmpty())
            {
                toc = Helper.DEFAULT_TOC;
            }

			var inputItemIds = ApiHelper.GetFormRequestParameter(Helper.ITEMIDS);
            var itemIdList = inputItemIds.Split('|');
            var userIds = ApiHelper.GetFormRequestParameter(Helper.USERIDS);
			var userIdList = userIds.Split('|');
		    
            var itemids = new List<string>();
		    if (itemIdList.Any())
		    {
                itemids = inputItemIds.Length > 1950 ? new List<string> { "*" } : new List<string>(itemIdList);
		    }

			foreach (var cmd in userIdList.Select(userid => new Ac.GetGrades
			{
				SearchParameters = new Adc.GradeSearch
				{
					UserId = userid,
					EntityId = id,
                    ItemIds = itemids
				}
			}))
			{
				SessionManager.CurrentSession.Execute(cmd);

                if (!cmd.Enrollments.IsNullOrEmpty())
                {
                    foreach (var enrollment in cmd.Enrollments)
                    {
                        if (!enrollment.ItemGrades.IsNullOrEmpty())
                        {
                            foreach (var grade in enrollment.ItemGrades)
                            {
                                if (!lastUpdated.IsNullOrEmpty())
                                {
                                    if (grade.ScoredDate < Convert.ToDateTime(lastUpdated))
                                    {
                                        continue;
                                    }
                                }

                                var gradeItem = gradeItemList.FirstOrDefault(i => i.ItemId.Trim().Equals(grade.Item.Id.Trim()));

                                if (gradeItem == null)
                                {
                                    var items = ApiItemActions.GetItems(id, grade.Item.Id);
                                    if (items.IsNullOrEmpty())
                                    {
                                        continue;
                                    }

                                    var item = items.First();
                                    var pxItem = item.ToContentItem();
                                    gradeItem = pxItem.ToGradesItem(toc);
                                    gradeItem.Category = ApiHelper.GetCategoryName(id, item.Category, SessionManager, ApiGradeBookActions);

                                    gradeItemList.Add(gradeItem);
                                }

                                var userGrade = grade.ToUserGrade();
                                userGrade.UserId = enrollment.User.Id;                                

                                gradeItem.UserGrades.Add(userGrade);
                            }
                        }
                    }
                }
			}

            var result = (from g in gradeItemList join i in itemIdList on g.ItemId equals i select g).ToList();
            if (result.IsNullOrEmpty())
            {
                response.error_message = Helper.NO_RESULTS;
            }
            else
			{
                response.results = result;
			}

			return response;
		}

		/// <summary>
		/// Get Grades By EnrollmentId
		/// Test URL: 
		/// new route: http://dev.px.bfwpub.com/api/Grades/GetGradesByEnrollment/114699/
		/// existing route: http://dev.px.bfwpub.com/api/Grades/GradesByEnrollment/114501/
		/// </summary>
		/// <param name="id"></param>
		/// <returns>GradesItemListResponse</returns>
		[HttpGet]
		[ActionName("GradesByEnrollment")]
		public GradesItemListResponse GetGradesByEnrollment(string id)
		{
			var response = new GradesItemListResponse();

			var gradeActions = ApiGradeActions;
			Adc.Enrollment enrollment;
			var enrlmntgrades = gradeActions.GetGradesByEnrollment(id, null, out enrollment);
			var courseid = enrollment.Course.Id;
			var gradesitemlist = new List<GradesItem>();
			var itemActions = ApiItemActions;

			if (enrlmntgrades == null) return null;
			//go through each grade in the enrollment grades
			foreach (var grade in enrlmntgrades)
			{
				var gradesitem =
					gradesitemlist.FirstOrDefault(gitem => gitem.ItemId.Trim().Equals(grade.Item.Id.Trim()));

				//If this item is not in the list of items to be returned, 
				//add it to the list
				if (gradesitem == null)
				{
					var itms = itemActions.GetItems(courseid, grade.Item.Id);
					if (itms.IsNullOrEmpty()) continue;
					var itm = itms.First();
					var pxitem = itm.ToContentItem();
					gradesitem = pxitem.ToGradesItem();
					gradesitem.Category = Bfw.PXWebAPI.Helpers.ApiHelper.GetCategoryName(courseid, itm.Category, SessionManager, ApiGradeBookActions);
					gradesitemlist.Add(gradesitem);
				}
				var usergrade = grade.ToUserGrade();
				//Assign the userid
				usergrade.UserId = enrollment.User.Id;
				//Add the grade item result to the results collection.
				gradesitem.UserGrades.Add(usergrade);
			}
			response.results = gradesitemlist;
			return response;

		}
	}
}