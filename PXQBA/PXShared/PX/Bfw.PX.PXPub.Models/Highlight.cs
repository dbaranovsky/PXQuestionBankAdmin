using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;

using Bfw.Common;
using Bfw.Common.Collections;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Contains all data required to display a resource
    /// </summary>
    public class HighlightModel
    {
        public int HighlightId { get; set; }
        public string HighlightText { get; set; }
        public string CommentText { get; set; }
        public string CommentLink { get; set; }
        public string ItemId { get; set; }
        public string SecondaryId { get; set; }
        public string PeerReviewId { get; set; }
        public int HighlightType { get; set; }
        public string HighlightDescription { get; set; }
        public string Url { get; set; }
    }
}
