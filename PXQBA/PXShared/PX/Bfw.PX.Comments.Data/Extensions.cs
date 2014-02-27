using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Comments.Data
{
    public static class PxCommentsDataContextExtensions
    {
        public static string Association(this Comment comment)
        {
            return comment.HighlightComments.First().Highlight.description;
        }

        public static HighlightType HighlightType(this Comment comment)
        {
            Highlight hl = comment.HighlightComments.First().Highlight;
            return hl.highlightType;
        }

        public static Highlight ParentHighlight(this Comment comment)
        {
            Highlight hl = comment.HighlightComments.First().Highlight;
            return hl;
        }


    } 
}
