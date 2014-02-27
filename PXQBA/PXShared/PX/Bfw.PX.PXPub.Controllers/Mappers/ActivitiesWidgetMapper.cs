using System;
using System.Collections.Generic;
using System.Linq;
using Bfw.Common.Collections;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class ActivitiesWidgetMapper
    {

        /// Update List of Activities with submissions
        /// </summary>
        /// <param name="activityList">List of Activity</param>
        /// <param name="content">Context Service</param>
        /// <param name="gradeActions">GradeActions Service</param>
        private static List<Activity> UpdateSubmissionStatus(this List<Activity> activityList, BizSC.IBusinessContext context, IGradeActions gradeActions, IContentActions contentActions, List<BizDC.ContentItem> c)
        {
            if (!activityList.IsNullOrEmpty())
            {
                List<string> activityIds = activityList.Map(a => a.Id).ToList();

                List<BizDC.Grade> grades = null;
                try
                {
                    grades = gradeActions.GetGradesByEnrollment(context.EnrollmentId, activityIds).ToList();
                }
                catch (Exception) { }


                if (grades != null)
                {
                    foreach (Activity activity in activityList)
                    {
                        var isSubmitted = grades.Where(i => i.GradedItem.Id == activity.Id).ToList();
                        //var hasLetterGrade = grades.Where(i=> i.Letter != null).ToList();
                        if (isSubmitted.Count == 1 && isSubmitted[0].Letter != null && isSubmitted[0].Letter == "A")
                        {
                            activity.IsUserSubmitted = true;
                        }
                    }
                }
            }

            return activityList;
        }



        /// Convert Activity to GroupActivities
        /// </summary>
        /// <param name="activityList">List of Activity</param>
        private static Dictionary<string, List<Activity>> ToGroupActivities(this List<Activity> activityList)
        {
            Dictionary<string, List<Activity>> groupedActivities = new Dictionary<string, List<Activity>>();

            if (!activityList.IsNullOrEmpty())
            {
                foreach (Activity item in activityList)
                {
                    string metaTopic = item.MetaTopic;

                    if (!groupedActivities.ContainsKey(metaTopic))
                    {
                        List<Activity> similarItems = new List<Activity>();
                        similarItems.Add(item);
                        groupedActivities.Add(metaTopic, similarItems);
                    }
                    else
                    {
                        groupedActivities[metaTopic].Add(item);
                    }
                }
            }

            return groupedActivities;
        }



        /// Convert ActivitiesWidgetResults to ActivitiesWidget
        /// </summary>
        /// <param name="biz">ActivitiesWidgetResults</param>
        /// <param name="content">Context Service</param>        
        /// <param name="gradeActions">GradeActions Service</param>
        public static ActivitiesWidget ToActivitiesWidget(this BizDC.ActivitiesWidgetResults biz, BizSC.IBusinessContext context, IGradeActions gradeActions, IContentActions contentActions, bool sort = false)
        {
            ActivitiesWidget model = new ActivitiesWidget();

            if (biz != null)
            {
                List<Activity> activities = new List<Activity>();

                var activityModels = biz.Activities.Map(a => a.ToActivity(context, contentActions));
                var sortedActivityModels = activityModels.OrderBy(i => i.MetaTopic);

                activities = sortedActivityModels.ToList();

                if (context.AccessLevel == AccessLevel.Student)
                {

                    activities.UpdateSubmissionStatus(context, gradeActions, contentActions, biz.Activities);
                }


                Dictionary<string, List<Activity>> d = new Dictionary<string, List<Activity>>();

                if (sort)
                {
                    var results = activities.SortByDueDate();
                    d.Add("sorted", results);
                    model.GroupedActivities = d;



                }
                else
                {


                    model.GroupedActivities = activities.ToGroupActivities();
                    model.GroupedActivities = model.GroupedActivities.SortItemsBySequence();
                    model.GroupedActivities = model.GroupedActivities.SortGroupedActivitiesTopicTitle();

                }


            }

            model.EnrollmentId = context.EnrollmentId;
            model.Userspace = context.Domain.Userspace.ToLowerInvariant();
            model.UserAccess = context.AccessLevel;

            return model;
        }

        /// <summary>
        /// Convert Activity to GroupActivities
        /// </summary>
        /// <param name="activityList">List of Activity</param>
        public static Dictionary<string, List<Activity>> SortGroupedActivitiesTopicTitle(this Dictionary<string, List<Activity>> activityList)
        {

            var sortedGroupedActivities = activityList.OrderBy(f => //order by chapter numbers
                             {
                                 var chapterNum = 9999;
                                 if (f.Key.Contains("Chapter"))
                                 {
                                     try
                                     {
                                         var matches = System.Text.RegularExpressions.Regex.Matches(f.Key, @"(\d+)");
                                         var result = matches[0].Groups[0].Value;
                                         Int32.TryParse(result, out chapterNum);
                                     }
                                     catch (Exception ex)
                                     {

                                     }

                                 }
                                 return chapterNum;
                             }).ToDictionary(f => f.Key, f => f.Value);


            return sortedGroupedActivities;

        }

        public static List<Activity> SortByDueDate(this List<Activity> activities)
        {
            activities.ForEach(a => { a.MetaTopic = (a.isAssigned ? "Assigned" : "Unassigned"); });
            var sortedActivites = activities.OrderBy(i => i.MetaTopic).ThenBy(i => i.DueDate).ThenBy(i => i.Sequence).ToList();

            return sortedActivites;

        }

        public static Dictionary<string, List<Activity>> SortItemsBySequence(this Dictionary<string, List<Activity>> activitys)
        {
            Dictionary<string, List<Activity>> groupedActivities = new Dictionary<string, List<Activity>>();

            foreach (KeyValuePair<string, List<Activity>> i in activitys)
            {

                groupedActivities.Add(i.Key, i.Value.OrderBy(a => a.Sequence).ToList());
            }


            return groupedActivities;

        }




    }
}
