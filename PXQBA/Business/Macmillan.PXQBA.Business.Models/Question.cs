using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Dto represent question that is used in react components.
    /// </summary>
    public class Question
    {
        public string Title;

        public string QuestionType;

        public string EBookChapter;

        public string QuestionBank;

        public string QuestionSeq;

        public string EnrollmentId;

        public string ItemId;

        public string FormerId;

        public string Id;

        public string GeneralFeedback;

        public string EntityId;

        public string Text;

        public string PreviewText;

        public string CustomUrl;

        public string HtsPlayerUrl;

        public string Type;

        public string QuestionTypeUrl;

        public  string LevelFormatted;

        public double PointComputed;

        public double PointPossible;

      //  public List<QuestionChoice> Choices;

        public string Answer;

        public List<string> AnswerList;

        public int BankCount;

        public int BankUse;

        public string QuestionXml;

        public string InteractionData;

        public bool IsLast;

        public List<string> QuestionMetaData;

        public string QuestionHtmlInlinePreview;

        /// <summary>
        /// Indicates the question type (multiple choice, matching,...).
        /// </summary>
        public InteractionType InteractionType { get; set; }

        public static string QuestionTypeShortNameFromId(string id)
        {
            return new Dictionary<string, string>()
            {
                { "answer", "A" },
                { "choice", "MC" },
                { "composite", "COMP" },
                { "custom", "CUSTOM" },
                { "hts", "HTS" },
                { "graph", "FMA_GRAPH" },
                { "essay", "E" },
                { "match", "MT" },
                { "text", "TXT" },
                { "bank", "BANK" },
            }[id];
        }
    }
}
