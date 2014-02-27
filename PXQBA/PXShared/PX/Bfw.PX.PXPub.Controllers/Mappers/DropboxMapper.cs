using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Controllers.Helpers;
using Bfw.PX.PXPub.Models;
using Microsoft.Practices.ServiceLocation;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class DropboxMapper
    {
        /// <summary>
        ///Convert to a writing assignment.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="biz">The biz.</param>
        /// <param name="content">The content.</param>
        public static void ToDropbox(this Dropbox model, Bfw.PX.Biz.ServiceContracts.IBusinessContext context, BizSC.IContentActions content, BizDC.ContentItem biz, bool loadGrades = false)
        {
            if (biz.AssignmentSettings != null)
            {
                model.IsAssignable = true;
                model.DropBoxType = (DropboxType)Enum.Parse(typeof(DropboxType), biz.AssignmentSettings.DropBoxType.ToString());
                model.DueDate = biz.AssignmentSettings.DueDate;
                model.PossibleScore = biz.AssignmentSettings.Points;
                model.Category = biz.AssignmentSettings.Category;                
            }

            model.DefaultPoints = biz.DefaultPoints;

            if (!biz.Properties.IsNullOrEmpty() && biz.Properties.ContainsKey("bfw_submission_date"))
            {
                model.SubmittedDate = biz.Properties["bfw_submission_date"].As<DateTime>();
            }

            if (!biz.Resources.IsNullOrEmpty())
            {
                foreach (var res in biz.Resources)
                {
                    if (res.ContentType == "text/html")
                    {
                        using (var sw = new System.IO.StreamReader(res.GetStream()))
                            model.Description = sw.ReadToEnd();
                    }
                }
            }

            // Load grades
            if (loadGrades)
            {
                content.GetGradesPerItem(new List<BizDC.ContentItem>() { biz }, context.CourseId);
            }
        }

        /// <summary>
        ///Convert to a writing assignment.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static Dropbox ToDropbox(this BizDC.ContentItem biz, Bfw.PX.Biz.ServiceContracts.IBusinessContext context, BizSC.IContentActions content, bool loadGrades = false)
        {
            var model = new Dropbox()
            {
                Id = biz.Id,
                Title = biz.Title,
                ParentId = biz.ParentId,
                Sequence = biz.Sequence,
                SubType = biz.Subtype,
                DefaultPoints = biz.DefaultPoints
            };

            try
            {
                model.IsAllowSubmission = FindIsAllowSubmission(biz);
                model.ToDropbox(context, content, biz, loadGrades);
                model.SetVisibility(biz);
            }
            catch { }

            return model;
        }

        /// <summary>
        /// Validate DueDate For Dropbox
        /// </summary>
        /// <returns></returns>
        private static bool FindIsAllowSubmission(this BizDC.ContentItem biz)
        {
            bool result = false;
            if (biz.AssignmentSettings != null)
            {
                DateTime duedate = biz.AssignmentSettings.DueDate;
                if (biz.AssignmentSettings.AllowLateSubmission && !biz.AssignmentSettings.IsAllowLateGracePeriod)
                {
                    result = true;
                }
                else if (biz.AssignmentSettings.AllowLateSubmission && biz.AssignmentSettings.IsAllowLateGracePeriod)
                {
                    duedate = AssignmentHelper.GetGraceDueDate(biz.AssignmentSettings);
                    if (duedate > DateTime.Now.GetCourseDateTime())
                    {
                        result = true;
                    }
                }
                else if (duedate > DateTime.Now.GetCourseDateTime())
                {
                    result = true;
                }
            }
            return result;
        }

    }
}
