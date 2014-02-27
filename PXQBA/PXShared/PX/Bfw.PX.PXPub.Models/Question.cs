using System.Collections.Generic;
using System.Text.RegularExpressions;
using Bfw.Common;
using System;
using System.Linq;

namespace Bfw.PX.PXPub.Models
{
    public class Question
    {
        /// <summary>
        /// Constuctor
        /// </summary>
        public Question()
        {
            AssignedQuizes = new List<ContentItem>();
            Interaction = new QuestionInteraction();
        }
        /// <summary>
        /// Gets or sets the enrollment id.
        /// </summary>
        /// <value>
        /// The enrollment id.
        /// </value>
        public string EnrollmentId { get; set; }

        /// <summary>
        /// Gets or sets the item id.
        /// </summary>
        /// <value>
        /// The item id.
        /// </value>
        public string ItemId { get; set; }

        /// <summary>
        /// Gets or sets the former id.
        /// </summary>
        /// <value>
        /// The former id.
        /// </value>
        public string FormerId { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// The question's title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// the Question's Feedback
        /// </summary>
        public string GeneralFeedback { get; set; }

        /// <summary>
        /// Gets or sets the entity id.
        /// </summary>
        /// <value>
        /// The entity id.
        /// </value>
        public string EntityId { get; set; }

        /// <summary>
        /// The display text of the question
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text { get; set; }

        /// <summary>
        /// The dislpay text of the question, when it is to be displayed in a smaller 
        /// area. This removes HTML formatting and truncates the string, adding an ellipsis.
        /// </summary>
        public string PreviewText
        {
            get
            {
                return Regex.Replace(Regex.Replace(Text, "<[^>]*>", "").Truncate("...", 0, 150), "[\\s\\r\\n]", " ").Trim();
            }
        }

        /// <summary>
        /// Gets or sets the custom URL.
        /// </summary>
        /// <value>
        /// The custom URL.
        /// </value>
        public string CustomUrl { get; set; }

        /// <summary>
        /// Gets or sets the HTS URL.
        /// </summary>
        /// <value>
        /// The HTS URL.
        /// </value>
        public string HtsPlayerUrl { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type
        {
            get;
            set;
        }


        /// <summary>
        /// Gets the type of the question.
        /// </summary>
        /// <value>
        /// The type of the question.
        /// </value>
        public static string QuestionType(string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                switch (type.ToUpper())
                {
                    case "MC": return "Multiple Choice";
                    case "BANK": return "Question Pool";
                    case "A": return "Multiple Answer";
                    case "COMP": return "Composite";
                    case "E": return "Essay";
                    case "MT": return "Matching";
                    case "TXT": return "Short Answer";
                    case "HTS": return "Custom";
                    case "GRAPH": return "Custom";
                    case "CUSTOM": return "Advanced"; /*PX-1085*/
                    default: return type;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public static string QuestionTypeUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                switch (url.ToUpper())
                {
                    case "HTS": return "Advanced";
                    case "FMA_GRAPH": return "Graphing";
                    default: return url;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public static string LevelFormatted(string level)
        {
            var formatted = String.Empty;
            if (!String.IsNullOrEmpty(level))
            {
                switch (level)
                {
                    case "1":
                        formatted = "1 (Easy)";
                        break;
                    case "2":
                        formatted = "2 (Medium)";
                        break;
                    case "3":
                        formatted = "3 (Hard)";
                        break;
                }
            }
            return formatted;
        }

        /// <summary>
        /// Gets or sets the points.
        /// </summary>
        /// <value>
        /// The points.
        /// </value>
        public double Points { get; set; }

        /// <summary>
        /// Gets or sets the point computed.
        /// </summary>
        /// <value>
        /// The points.
        /// </value>
        public double PointComputed { get; set; }

        /// <summary>
        /// Gets or sets the point possible.
        /// </summary>
        /// <value>
        /// The points.
        /// </value>
        public double PointPossible { get; set; }

        /// <summary>
        /// Gets or sets the choices.
        /// </summary>
        /// <value>
        /// The choices.
        /// </value>
        public IList<QuestionChoice> Choices { get; set; }

        /// <summary>
        /// Gets the answer.
        /// </summary>
        /// <value>
        /// The answer.
        /// </value>
        public string Answer 
        {
            get
            {
                string result = string.Empty;

                if (AnswerList != null && AnswerList.Count > 0)
                {
                    result = AnswerList.First();
                }

                return result;
            }
        }

        /// <summary>
        /// Gets or sets the answer list.
        /// </summary>
        /// <value>
        /// The answer list.
        /// </value>
        public IList<string> AnswerList { get; set; }

        /// <summary>
        /// Gets or sets the bank count.
        /// </summary>
        /// <value>
        /// The bank count.
        /// </value>
        public int BankCount { get; set; }

        /// <summary>
        /// Gets or sets the bank use.
        /// </summary>
        /// <value>
        /// The bank use.
        /// </value>
        public int BankUse { get; set; }

        /// <summary>
        /// Gets or sets the question XML.
        /// </summary>
        /// <value>
        /// The question XML.
        /// </value>
        public string QuestionXml { get; set; }

        /// <summary>
        /// Gets or sets the interaction data.
        /// </summary>
        /// <value>
        /// The interaction data.
        /// </value>
        public string InteractionData { get; set; }

        /// <summary>
        /// Contains question type, properties, and choices 
        /// </summary>
        public QuestionInteraction Interaction;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is last.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is last; otherwise, <c>false</c>.
        /// </value>
        public bool IsLast { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance should be converted to an advanced question.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance should be converted; otherwise, <c>false</c>.
        /// </value>
        public bool IsAdvancedConvert { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is HTS.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is HTS; otherwise, <c>false</c>.
        /// </value>
        public bool IsHts { get; set; }

        /// <summary>
        /// Gets or sets the assigned quizes.
        /// </summary>
        /// <value>
        /// The assigned quizes.
        /// </value>
        public IList<ContentItem> AssignedQuizes { get; set; }

        /// <summary>
        /// Questions the type short name from id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets or sets the question meta data.
        /// </summary>
        /// <value>
        /// The question meta data.
        /// </value>
        public List<string> QuestionMetaData { get; set; }

        /// <summary>
        /// Gets or sets all the qustions if this is a question pool
        /// </summary>
        public IList<Question> Questions { get; set; }
        /// <summary>
        /// Gets or sets the searchable meta data.
        /// </summary>
        /// <value>
        /// The searchable meta data.
        /// </value>
        public Dictionary<string, string> SearchableMetaData { get; set; }

        /// <summary>
        /// Gets or sets the assessment groups.
        /// </summary>
        /// <value>
        /// The assessment groups.
        /// </value>
        public List<string> AssessmentGroups { get; set; }

        /// <summary>
        /// Gets or sets the attempts.
        /// </summary>
        /// <value>
        /// The attempts.
        /// </value>
        public string Attempts { get; set; }

        /// <summary>
        /// Gets or sets the time limit.
        /// </summary>
        /// <value>
        /// The time limit.
        /// </value>
        public string TimeLimit { get; set; }

        /// <summary>
        /// Gets or sets the scrambled.
        /// </summary>
        /// <value>
        /// The scrambled.
        /// </value>
        public string Scrambled { get; set; }

        /// <summary>
        /// Gets or sets the review.
        /// </summary>
        /// <value>
        /// The review.
        /// </value>
        public string Review { get; set; }

        /// <summary>
        /// Gets or sets the hints.
        /// </summary>
        /// <value>
        /// The hints.
        /// </value>
        public string Hints { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        public string Score { get; set; }

        /// <summary>
        /// Gets or sets the analysis.
        /// </summary>
        /// <value>
        /// The analysis.
        /// </value>
        public QuestionAnalysis Analysis { get; set; }

        /// <summary>
        /// Gets or sets for new question.
        /// </summary>
        public bool IsNewQuestion { get; set; }

        /// <summary>
        /// The review settings specific to a homework question
        /// </summary>
        public ReviewSettings ReviewSettings { get; set; }

        /// <summary>
        /// The quiz type to which the question belongs to
        /// </summary>
        public QuizType QuizType { get; set; }

    	public bool AdminFlag { get; set; }
    	

        /// <summary>
        /// Gets or sets the learning curve question settings.
        /// </summary>
        /// <value>
        /// The learning curve question settings.
        /// </value>
        public IList<LearningCurveQuestionSettings> LearningCurveQuestionSettings { get; set; }

        /// <summary>
        /// Bloom's taxonomy 
        /// </summary>
        public string LearningCurve_Blooms { get; set; }

        /// <summary>
        /// Related content
        /// </summary>
        public RelatedContent RelatedContent { get; set; }

        /// <summary>
        /// Excercise Number for the  question.
        /// </summary>
        public string ExcerciseNo {get;set;}

        /// <summary>
        /// Question Bank for the  question.
        /// </summary>
        public string QuestionBank { get; set; }

        /// <summary>
        /// eBook Chapter for the  question.
        /// </summary>
        public string eBookChapter { get; set; }

        /// <summary>
        /// Difficulty for the  question.
        /// </summary>
        public string Difficulty { get; set; }

        /// <summary>
        /// Congnitive Level for the  question.
        /// </summary>
        public string CongnitiveLevel { get; set; }

        /// <summary>
        /// Blooms Domain for the  question.
        /// </summary>
        public string BloomsDomain { get; set; }

        /// <summary>
        /// Guidance for the  question.
        /// </summary>
        public string Guidance { get; set; }

        /// <summary>
        /// Free Response Question for the  question.
        /// </summary>
        public string FreeResponseQuestion { get; set; }
        
        /// <summary>
        /// The Id for the question this question is used in.
        /// </summary>
        public string UsedIn { get; set; }
      
        /// <summary>
        /// Chapter to which quiz belongs
        /// </summary>
        public string AssignedChapter { get; set; }

        /// <summary>
        /// Dictionary of learning objectives for question
        /// </summary>
        public Dictionary<string, string> LearningObjectives { get; set; }

        /// <summary>
        /// Dictionarty of Suggested Uses for a question
        /// </summary>
        public Dictionary<string, string> SuggestedUse { get; set; }

        /// <summary>
        /// The Id for the question this question is used in.
        /// </summary>
        public string QuestionBankText { get; set; }

        /// <summary>
        /// Chapter to which quiz belongs
        /// </summary>
        public string EbookSectionText { get; set; }

        /// <summary>
        /// Dictionary of Selected learning objectives from question
        /// </summary>
        public Dictionary<string, string> SelectedLearningObjectives { get; set; }

        /// <summary>
        /// Dictionarty of Selected Suggested Uses from question
        /// </summary>
        public Dictionary<string, string> SelectedSuggestedUse { get; set; }


        /// <summary>
        /// Question status e.g. Deleted , In progress etc
        /// </summary>
        public string QuestionStatus { get; set; }
    }

    /// <summary>
    /// Represents the review settings for a homework question
    /// </summary>
    public class ReviewSettings
    {
        public ReviewSetting ShowScoreAfter { get; set; }
        public ReviewSetting ShowQuestionsAnswers { get; set; }
        public ReviewSetting ShowRightWrong { get; set; }
        public ReviewSetting ShowAnswers { get; set; }
        public ReviewSetting ShowFeedbackAndRemarks { get; set; }
        public ReviewSetting ShowSolutions { get; set; }
    }

}
