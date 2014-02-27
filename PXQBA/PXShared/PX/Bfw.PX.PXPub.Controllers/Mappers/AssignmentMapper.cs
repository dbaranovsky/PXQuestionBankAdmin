using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class AssignmentMapper
    {
        /// <summary>
        /// Maps an Assignment to a generic ContentItem.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="courseId">The course id.</param>
        /// <returns></returns>
        public static BizDC.ContentItem ToContentItem(this Assignment model, string courseId)
        {
            var biz = model.ToBaseContentItem(courseId);

            if (null != model)
            {
                biz.Type = "Assignment";
                biz.Subtype = model.SubType;
                biz.AssignmentSettings = new BizDC.AssignmentSettings
                {
                    IsAssignable = true,
                    DropBoxType = (BizDC.DropBoxType)model.DropBoxType,
                    DueDate = model.DueDate,
                    DueDateTZ = model.DueDateTZ,
                    Points = model.PossibleScore,
                    Category = model.Category
                };
                biz.Href = string.Format("Templates/Data/{0}/index.html", biz.Id);
                biz.AssignmentSettings.DropBoxType = (BizDC.DropBoxType)model.DropBoxType;

                var rez = new BizDC.Resource()
                {
                    ContentType = "text/html",
                    Extension = "html",
                    Status = BizDC.ResourceStatus.Normal,
                    Url = biz.Href,
                    EntityId = courseId
                };

                var sw = new System.IO.StreamWriter(rez.GetStream());
                sw.Write(System.Web.HttpUtility.HtmlDecode(model.Description));
                sw.Flush();

                biz.Resources = new List<BizDC.Resource>() { rez };
            }

            return biz;
        }

        public static Assignment ToAssignment(this BizDC.ContentItem biz, BizSC.IContentActions contentActions)
        {
            if (null == biz)
                return null;
            Assignment model = new Assignment();
            model.ToBaseContentItem(biz);
            model.Id = biz.Id;
            model.Type = biz.Type;
            model.SubType = biz.Subtype;
            if (biz.AssignmentSettings != null)
            {
                model.IsAssignable = true;
                model.DueDate = biz.AssignmentSettings.DueDate;
                model.DueDateTZ = biz.AssignmentSettings.DueDateTZ;
                model.DropBoxType = (DropboxType)Enum.Parse(typeof(DropboxType), biz.AssignmentSettings.DropBoxType.ToString());
                model.PossibleScore = model.MaxPoints = biz.AssignmentSettings.Points;
                model.Category = biz.AssignmentSettings.Category;
                model.IsAllowLateSubmission = biz.AssignmentSettings.AllowLateSubmission;
            }
            model.DefaultPoints = biz.DefaultPoints;
            if (!biz.Properties.IsNullOrEmpty() && biz.Properties.ContainsKey("bfw_submission_date"))
            {
                model.SubmittedDate = biz.Properties["bfw_submission_date"].As<DateTime>();
            }

            return model;
        }
    }
}
