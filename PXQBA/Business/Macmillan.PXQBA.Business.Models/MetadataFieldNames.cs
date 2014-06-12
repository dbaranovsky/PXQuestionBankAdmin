using System.Collections;
using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Models
{
    public static class MetadataFieldNames
    {
        public const string QuestionStatus = "questionstatus";

        public const string InlinePreview = "questionHtmlInlinePreview";
        public const string DlapType = "dlap_q_type";
        public const string DlapTitle = "title";
        public const string Id = "id";
        public const string Chapter = "chapter";
        public const string Bank = "bank";
        public const string Sequence = "sequence";
        public const string Difficulty = "difficulty";
        public const string Keywords = "keywords";
        public const string SuggestedUse = "suggesteduse";
        public const string Guidance = "guidance";
        public const string SharedWith = "sharedWith";
        public const string DuplicateFromShared = "duplicatefromshared";
        public const string RestoredFromVersion = "restoredfromversion";
        public const string IsPublishedFromDraft = "ispublishedfromdraft";
        public const string DuplicateFrom = "duplicatefrom";
        public const string ProductCourse = "productcourseid";
        public const string TargetProductCourse = "targetproductcourse";
        public const string Flag = "flag";
        public const string DraftFrom = "draftfrom";
        public const string ParentProductCourseId = "parentProductCourseId";
        public const string ModifiedBy = "modifiedby";
        public const string ContainsText = "containstext";

        public static IEnumerable<string> GetStaticFieldNames()
        {
            return new List<string>()
            {
                DlapTitle,
                Bank,
                Chapter,
                ProductCourse,
                Sequence,
                Flag,
                ParentProductCourseId
            };
        }
    }
}