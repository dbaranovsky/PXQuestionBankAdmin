// -----------------------------------------------------------------------
// <copyright file="DisplayItem.cs" company="Macmillan">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
namespace Bfw.PX.PXPub.Models
{

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DisplayItem
    {
        public string Id { get; set; }
        public ContentViewMode Mode { get; set; }
        public bool? IncludeToc { get; set; }
        public bool? IncludeDiscussion { get; set; }
        public bool? IncludeNavigation { get; set; }
        public bool? ReadOnly { get; set; }
        public Guid? HasParentLesson { get; set; }
        public bool? IsStudentView { get; set; }
        public bool? IsBeingEdited { get; set; }
        public string CommentId { get; set; }
        public bool? IsStudentUpdated { get; set; }        
        public bool GetChildrenGrades { get; set; }
        public string Category { get; set; }
        public string GroupId { get; set; }
        public bool IsStart { get; set; }
        public bool RenderFNE { get; set; }
        public string ExternalUrl { get; set; }
        public bool RenderDialog { get; set; }
    }
}
