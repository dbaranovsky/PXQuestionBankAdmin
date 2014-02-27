using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Common.Collections;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Contracts;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.Biz.DataContracts;
using ContentItem = Bfw.PX.Biz.DataContracts.ContentItem;
using Grade = Bfw.PX.Biz.DataContracts.Grade;

namespace Bfw.PX.PXPub.Controllers.Helpers
{
    public class TreeWidgetHelper : ITreeWidgetHelper
    {
        public string DefaultToc { get { return "syllabusfilter"; } }
        public string DefaultContainer { get { return "Launchpad"; } }
        public string DefaultSubContainer { get { return "PX_MULTIPART_LESSONS"; } }

        public List<Models.ContentItem> LoadItem(IContentActions contentActions, IBusinessContext context, 
            IGradeActions gradeActions, string itemId, string level, string widget = "")
        {
            throw new NotImplementedException();
        }

        public List<Models.ContentItem> ProcessStudentGrades(IContentActions contentActions, IBusinessContext context, 
            IGradeActions gradeActions, Models.ContentItem item, ref Models.ContentItem parentItem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Where are these items on the content item actually being used?
        /// </summary>
        /// <param name="item"></param>
        /// <param name="grade"></param>
        public void SetGrade(Models.ContentItem item, Biz.DataContracts.Grade grade)
        {
            //TODO: Merge this with ContentActions.GetGradesPerItem
            if (item == null || grade == null)
                return;

            //What, we only set achieved to -1 if we have max points but don't want to show it?
            if (item.MaxPoints > 0)
            {
                if ((grade.Status & GradeStatus.ShowScore) != GradeStatus.ShowScore
                    || ((grade.Status & GradeStatus.Released) != GradeStatus.Released && item.GradeReleaseDate.ToUniversalTime() > DateTime.UtcNow)
                    || grade.Possible < 1)
                {
                    //if not show score, set score to -1
                    grade.Achieved = -1;
                }
            }

            if ((grade.Status & GradeStatus.Completed) == GradeStatus.Completed
                || (grade.Status & GradeStatus.ShowScore) == GradeStatus.ShowScore)
            {
                //TODO: Don't think we should be setting a "score" on a content item.  Wrap the grade and the content item if need be.
                item.Score = grade.Achieved;
            }

            if((grade.Status & GradeStatus.Completed) == GradeStatus.Completed)
            {  //only mark as submitted if item has been completed
                item.IsUserSubmitted = true;
                   //special case for learning curve:
                if (item.FacetMetadata.ContainsKey("meta-content-type") &&
                    item.FacetMetadata["meta-content-type"].Contains("LearningCurve"))
                {
                    if (grade.Achieved < 1)
                    {
                        item.IsUserSubmitted = false;
                    }
                }
            }
        }

        /// <summary>
        /// Returns a list of ids for the top level item ids for all the items in <paramref name="itemsDue"/>
        /// </summary>
        /// <param name="itemsDue">List of content items in tree widget</param>
        /// <returns>List of item IDS for top level items</returns>
        public List<string> GetSubcontainerItemIds(List<ContentItem> itemsDue, string toc)
        {
            if (itemsDue.IsNullOrEmpty())
            {
                return new List<string>();
            }

            //Doesn't make sense to include subcontainerid that is empty
            List<string> chapters = itemsDue
                .Where(i => i.GetSubContainer(toc) != "PX_MULTIPART_LESSONS" && !string.IsNullOrWhiteSpace(i.GetSubContainer(toc)))
                .GroupBy(i => i.GetSubContainer(toc))
                .Select(i => i.Key).ToList();
            return chapters;
        }

        /// <summary>
        /// Returns list of content items in the current course that have been assigned, have a point value and are 
        /// completable 
        /// </summary>
        /// <param name="contentActions"></param>
        /// <param name="context"></param>
        /// <param name="container">Filter items to a specific toc</param>
        /// <param name="chapterId">Filter items to a specific top level content item</param>
        /// <returns>List of gradeable content for the course</returns>
        public List<ContentItem> GetAssignedContent(IContentActions contentActions, string entityId, string container,
            string chapterId, string toc)
        {
            if (contentActions == null)
            {
                return new List<ContentItem>();
            }

            var itemsDue = contentActions.ListContentWithDueDates(entityId, container, chapterId, toc).ToList();
            itemsDue = itemsDue.Where(t => !t.Type.ToLower().Equals("pxunit") && t.MaxPoints > 0).ToList();
            return itemsDue;
        }

        /// <summary>
        /// Calculates the overall completion percentage of a subcontainer for a particular enrollment
        /// </summary>
        /// <param name="enrollmentGrades">Grades associated with an enrollment</param>
        /// <param name="itemsInSubcontainer">Content items within the subcontainer</param>
        /// <param name="subcontainer">The subcontainer to calculate the completion data for</param>
        public void CalculateSubcontainerCompletionData(IEnumerable<Biz.DataContracts.Grade> enrollmentGrades, 
            IEnumerable<ContentItem> itemsInSubcontainer, Models.ContentItem subcontainer)
        {
            if (subcontainer == null)
                return;

            var stats = GetTotalAndMaxPossibleScore(enrollmentGrades, itemsInSubcontainer);
            if (stats != null)
            {
                CalculateCompletedPercentage(subcontainer, itemsInSubcontainer.Count(), stats.CompletedItems,
                    stats.TotalScore, stats.MaxScore);
            }
        }

        /// <summary>
        /// Calculates and sets the completion percentage for the subcontainer
        /// </summary>
        /// <param name="subcontainer">Subcontainer whos completion percentage is being calculated for</param>
        /// <param name="numItems">Number of assigned items within the subcontainer</param>
        /// <param name="completedItems">Number of completed items within the subcontainer</param>
        /// <param name="totalScore">Total points scored for graded items within the subcontainer</param>
        /// <param name="maxScore">Max possible score for graded items within the subcontainer</param>
        private static void CalculateCompletedPercentage( Models.ContentItem subcontainer, int numItems, 
            int completedItems, double totalScore, double maxScore)
        {
            if (subcontainer == null)
                return;

            var completedPercentage = 0;
            var scoredPercentage = 0;

            if (completedItems > 0 && numItems > 0)
            {
                completedPercentage = Convert.ToInt32(((double) completedItems/(double) numItems)*100);
            }

            if (totalScore > 0 && maxScore > 0)
            {
                scoredPercentage = Convert.ToInt32((totalScore/maxScore)*100);
            }

            subcontainer.StudentCompletedPercentage = completedPercentage;
            subcontainer.StudentScorePercentage = scoredPercentage;
            subcontainer.StudentCompletedItems = completedItems;
            subcontainer.StudentItemsAssigned = numItems;
        }

        /// <summary>
        /// Returns the total score and max possible score for the graded items within a list 
        /// </summary>
        /// <param name="enrollmentGrades">Grades associated with the student whos total points are being retrieved</param>
        /// <param name="assignedItems">Assigned items list we are calculating the total and max points for</param>
        /// <returns>UnitCompletionStats object</returns>
        private static UnitCompletionStats GetTotalAndMaxPossibleScore(IEnumerable<Grade> enrollmentGrades, 
            IEnumerable<ContentItem> itemsInSubcontainer)
        {
            if (enrollmentGrades == null || itemsInSubcontainer == null)
            {
                return null;
            }

            UnitCompletionStats retval = new UnitCompletionStats();

            foreach (var item in itemsInSubcontainer)
            {
                var gradeSubmissions = enrollmentGrades.Where(g => g.GradedItem.Id == item.Id);
                var grade = gradeSubmissions.OrderByDescending(g => g.SubmittedVersion).FirstOrDefault();

                if (grade != null && item.MaxPoints > 0)
                {
                    if ((grade.Status & GradeStatus.Completed) == GradeStatus.Completed ||
                        (grade.Status & GradeStatus.ShowScore) == GradeStatus.ShowScore)
                    {
                        //special case for learning curve:
                        if (item.FacetMetadata.ContainsKey("meta-content-type") &&
                            item.FacetMetadata["meta-content-type"].Contains("LearningCurve"))
                        {
                            if (grade.Achieved <= 0.0)
                            {
                                continue;
                            }
                        }

                        retval.CompletedItems++;

                        if ((grade.Status & GradeStatus.ShowScore) == GradeStatus.ShowScore && grade.Possible > 0)
                        {
                            retval.MaxScore += item.MaxPoints;
                            retval.TotalScore += (grade.Achieved/grade.Possible)*item.MaxPoints;
                        }
                    }
                }
            }
            return retval;
        }

        /// <summary>
        /// Gets the containers items.
        /// </summary>
        /// <param name="contentActions">The content actions.</param>
        /// <param name="context">The context.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="containerId">The container unique identifier.</param>
        /// <param name="subcontainerId">The subcontainer unique identifier.</param>
        /// <param name="itemlevel">The itemlevel.</param>
        /// <returns></returns>
        public List<TreeWidgetViewItem> GetContainersItems(IContentActions contentActions, IBusinessContext context, 
            TreeWidgetSettings settings, string containerId, string subcontainerId, int itemlevel = 0)
        {
            List<Models.ContentItem> children = contentActions.GetContainerItems(settings.UseProductCourse ? context.ProductCourseId : context.EntityId, containerId, subcontainerId, settings.TOC)
                                                               .Map(biz => biz.ToContentItem(contentActions, false)).ToList();

            List<TreeWidgetViewItem> retval = new List<TreeWidgetViewItem>(children.Count);
            List<string> parents = new List<string>(children.Count);
            children = children.OrderBy(child => child.Sequence).ToList();
            foreach (var child in children)
            {
                //TODO: This worries me.  We set the parentid of the child to the parent of the toc.
                string sequence;
                child.ParentId = child.GetSyllabusFilterFromCategory(out sequence, settings.TOC);
                child.Sequence = sequence;
                child.UserAccess = context.AccessLevel;

                parents.Add(child.ParentId);

                //CHANGE!
                retval.Add(new TreeWidgetViewItem(child, settings, itemlevel));
            }

            //Set if the item is the parent to any other items
            foreach (var viewitem in retval)
            {
                if (parents.Contains(viewitem.Item.Id))
                    viewitem.HasChildren = true;
            }

            return retval;
        }

        /// <summary>
        /// Returns the template id for a unit if one exists in dlap. This is used in creating assignment units
        /// </summary>
        /// <returns>Unit template id if it exists.  Empty string if not</returns>
        public string GetUnitTemplateId(IContentActions contentActions)
        {
            string retval = string.Empty;
            var unitTemplate = contentActions.GetAllTemplates().FirstOrDefault(
                        t => String.Equals(t.Subtype, typeof (PxUnit).Name, StringComparison.CurrentCultureIgnoreCase));
            if (unitTemplate != null)
            {
                retval = unitTemplate.Id;
            }
            return retval;
        }

        internal class UnitCompletionStats
        {
            public int CompletedItems { get; set; }
            public double MaxScore { get; set; }
            public double TotalScore { get; set; }
        }
    }
}
