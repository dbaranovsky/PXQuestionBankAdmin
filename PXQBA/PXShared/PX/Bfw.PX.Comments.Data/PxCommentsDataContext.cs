using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Comments.Data
{
    public enum HighlightType
    {
        None,
        GeneralContent,
        WritingAssignment,
        PeerReview
    }

    public partial class Highlight
    {
        public bool isUserHighlight { set; get; }
    }

    public partial class PxCommentsDataContext
    {
       

        public List<Comment> GetComments(int highlightId)
        {
            var src = from hc in this.HighlightComments where hc.FK_HighlightId == highlightId select hc;

            List<Comment> commentList = new List<Comment>();

            foreach (HighlightComment h in src)
            {
                commentList.Add(h.Comment);
            }

            return commentList;
        }

        public List<Comment> GetComments(string itemId, string secondaryId, string peerReviewId, string commenterId, HighlightType hType)
        {
            var comments = from h in this.Highlights
                           from hc in this.HighlightComments
                           where hc.FK_HighlightId == h.highlightId
                           from c in this.Comments
                           where hc.FK_CommentId == c.commentId 
                           && h.itemId == itemId
                           && ((string.IsNullOrEmpty(secondaryId)) || h.secondaryId == secondaryId)
                           && ((string.IsNullOrEmpty(peerReviewId)) || h.reviewId == peerReviewId)
                           && ((string.IsNullOrEmpty(commenterId)) || c.userId == Convert.ToInt32(commenterId))
                           && ((hType == HighlightType.None) || h.highlightType == hType)                                                      
                           select c;

            if (comments.Any()) return comments.ToList();

            return new List<Comment>();

        }

        public int GetCommentsCount(string itemId, string secondaryId, string peerReviewId, string commenterId, HighlightType hType)
        {
            var comments = from h in this.Highlights
                           from hc in this.HighlightComments
                           where hc.FK_HighlightId == h.highlightId
                           from c in this.Comments
                           where hc.FK_CommentId == c.commentId
                           && h.itemId == itemId
                           && ((string.IsNullOrEmpty(secondaryId)) || h.secondaryId == secondaryId)
                           && ((string.IsNullOrEmpty(peerReviewId)) || h.reviewId == peerReviewId)
                           && ((string.IsNullOrEmpty(commenterId)) || c.userId == Convert.ToInt32(commenterId))
                           && h.highlightType == hType
                           select c;

            if (comments.Any()) return comments.Count();

            return 0;

        }

        public List<Highlight> GetHighlights(int courseId, string itemId, string secondaryId, string peerReviewId, string commenterId, int hType, bool currentUserView, string currentUserId)
        {
            HighlightType hlType = (Enum.IsDefined(typeof(HighlightType), hType))
                ? (HighlightType)hType
                : HighlightType.GeneralContent;

            if (commenterId == null) commenterId = String.Empty;
            
            //commenterId = "";
            var highlightSrc = from h in this.Highlights
                               where h.itemId == itemId
                               && h.courseId == courseId select h;
            
            if(hlType != HighlightType.None)
                highlightSrc = highlightSrc.Where(h => h.highlightType == hlType);

            if(!String.IsNullOrEmpty(secondaryId))
                highlightSrc = highlightSrc.Where(h => h.secondaryId == secondaryId);

            if (!String.IsNullOrEmpty(peerReviewId))
                highlightSrc = highlightSrc.Where(h => h.reviewId == peerReviewId);

            if(!String.IsNullOrEmpty(commenterId)) 
                highlightSrc = highlightSrc.Where(h => h.userId == Convert.ToInt32(commenterId));
            else if (currentUserView) 
                highlightSrc = highlightSrc.Where(h => h.userId == Convert.ToInt32(currentUserId));
            else
                highlightSrc = highlightSrc.Where(h => (h.userId == Convert.ToInt32(currentUserId)) || (h.isShared == true));
            

            if (highlightSrc.Any()) return highlightSrc.ToList();

            return new List<Highlight>();

        }


    } 
}
