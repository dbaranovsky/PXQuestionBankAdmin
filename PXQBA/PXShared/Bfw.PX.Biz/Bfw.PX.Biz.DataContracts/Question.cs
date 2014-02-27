using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Represents a Question business object. (See http://gls.agilix.com/Docs/Schema/Question).
    /// </summary>
    public class Question
    {
        public Question()
        {
            AssignedQuizes = new List<ContentItem>();
            SearchableMetaData = new Dictionary<string, string>();
            LearningObjectives = new Dictionary<string, string>();
            SuggestedUse = new Dictionary<string, string>();
            Interaction = new QuestionInteraction();
        }
        /// <summary>
        /// ID of the question.
        /// </summary>
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
        /// ID of the entity that owns the question.
        /// </summary>
        public string EntityId { get; set; }

        /// <summary>
        /// HTML body of text.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Url to webserver or path to resource that contains the definition for a custom question.
        /// </summary>
        public string CustomUrl { get; set; }

        /// <summary>
        /// The points possible for this question. If omitted, uses the assessment default score.
        /// </summary>
        public double Points { get; set; }

        /// <summary>
        /// Indicates the question type (multiple choice, matching,...).
        /// </summary>
        public InteractionType InteractionType { get; set; }

        /// <summary>
        /// If this is a question bank, then this number represents how many
        /// questions from the bank are to be used.  -1 indicates 'all'.
        /// </summary>
        public int BankUse { get; set; }

        /// <summary>
        /// If this is a question bank, then this number represents how
        /// many questions the bank contains.
        /// </summary>
        public int BankCount { get; set; }

        /// <summary>
        /// Collection of choices or possible answers.
        /// </summary>
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
        public IList<string> AnswerList { get; set; }

        /// <summary>
        /// Gets or sets the raw question XML.
        /// </summary>
        public string QuestionXml { get; set; }

        /// <summary>
        /// Gets or sets the interaction data.
        /// </summary>
        public string InteractionData { get; set; }

        /// <summary>
        /// Contains question type, properties, and choices 
        /// </summary>
        public QuestionInteraction Interaction;

        /// <summary>
        /// Collection of assigned quiz IDs.
        /// </summary>
        public IList<ContentItem> AssignedQuizes { get; set; }

        /// <summary>
        /// Chapter to which quiz belongs
        /// </summary>
        public string AssignedChapter { get; set; }

        /// <summary>
        /// Collection of question meta data values.
        /// </summary>
        public List<string> QuestionMetaData { get; set; }

        /// <summary>
        /// Gets or sets the searchable meta data.
        /// </summary>
        /// <value>
        /// The searchable meta data.
        /// </value>
        public Dictionary<string, string> SearchableMetaData { get; set; }

        /// <summary>
        /// Gets or sets the learning curve question settings.
        /// </summary>
        /// <value>
        /// The learning curve question settings.
        /// </value>
        public List<LearningCurveQuestionSettings> LearningCurveQuestionSettings { get; set; }

        /// <summary>
        /// Collection of included assessment groups.
        /// </summary>
        public List<string> AssessmentGroups { get; set; }

        /// <summary>
        /// Gets or sets the associated question analysis record.
        /// </summary>
        public QuestionAnalysis Analysis { get; set; }

        /// <summary>
        /// Gets or sets the associated question, if this is a question pool
        /// </summary>
        public IList<Question> Questions { get; set; }

        public bool AdminFlag
        {
            get
            {
                bool returnValue = false;
                if (SearchableMetaData != null && SearchableMetaData.ContainsKey("adminflag"))
                    if (SearchableMetaData["adminflag"].ToLower() == "true")
                        returnValue = true;
                return returnValue;
            }
            set
            {
                string flagValue = value.ToString().ToLower();
                if (SearchableMetaData == null)
                {
                    SearchableMetaData = new Dictionary<string, string>();
                    SearchableMetaData.Add("adminflag", flagValue);
                }
                else if (SearchableMetaData.ContainsKey("adminflag"))
                    SearchableMetaData["adminflag"] = flagValue;
                else
                    SearchableMetaData.Add("adminflag", flagValue); 
            }
        }

        /// <summary>
        /// Does this question have content?
        /// </summary>
        /// 
        [Obsolete("No longer necessary because we can't accurately tell what questions are blank", false)]
        public bool IsEmpty
        {
            get
            {
                return String.IsNullOrEmpty(Body) && String.IsNullOrEmpty(QuestionXml);
            }
        }

        /// <summary>
        /// Excercise Number for the  question.
        /// </summary>
        public string ExcerciseNo { get; set; }

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
        /// question version information
        /// </summary>
        public string QuestionVersion { get; set; }
        public DateTime ModifiedDate { get; set; }

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
}
