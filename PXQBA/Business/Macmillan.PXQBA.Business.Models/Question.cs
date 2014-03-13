using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Dto represent question that is used in react components.
    /// </summary>
    public class Question
    {
        public string title;

        public string questionType;

        public string eBookChapter;

        public string questionBank;

        public string questionSeq;

        public string EnrollmentId;

        public string ItemId;

        public string FormerId;

        public string Id;

        public string Title;

        public string GeneralFeedback;

        public string EntityId;

        public string Text;

        public string PreviewText;

        public string CustomUrl;

        public string HtsPlayerUrl;

        public string Type;

        public string QuestionType;

        public string QuestionTypeUrl;

        public  string LevelFormatted;

        public double PointComputed;

        public double PointPossible;

        public List<QuestionChoice> Choices;

        public string Answer;

        public List<string> AnswerList;

        public int BankCount;

        public int BankUse;

        public string QuestionXml;

        public string InteractionData;

        public bool IsLast;

        public List<string> QuestionMetaData;


    }
}
