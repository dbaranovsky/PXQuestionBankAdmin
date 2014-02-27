using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Xml.Serialization;
using Bfw.Common.Pagination;


namespace Bfw.PX.PXPub.Models
{
	[Serializable]
	[XmlRoot("root")]
	public class QuestionAdminSearchPanel : IPageble
	{
		/// <summary>
		/// 
		/// </summary>
		[XmlIgnore]
		public Quiz Quiz { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[XmlElement]
		public Pagination Pagination { get; set; }

		/// <summary>
		/// Gets or sets the question types.
		/// </summary>
		/// <value>
		/// The question types.
		/// </value>
		[XmlAttribute]
		public string SearchKeyword { get; set; }

		[XmlIgnore]
		public IEnumerable<SelectListItem> FormatSelectList { get; set; }
		[XmlIgnore]
		public IEnumerable<SelectListItem> StatusSelectList { get; set; }
		[XmlIgnore]
		public IEnumerable<SelectListItem> ChapterSelectList { get; set; }
		[XmlIgnore]
		public IEnumerable<SelectListItem> QuizSelectList { get; set; }

		[XmlArray]
		public List<string> QuizSelectedValues { get; set; }

		[XmlArray]
		public List<string> ChapterSelectedValues { get; set; }

		[XmlArray]
		public List<string> StatusSelectedValues { get; set; }

		[XmlArray]
		public List<string> FormatSelectedValues { get; set; }

		[XmlIgnore]
		public Question SelectedQuestion { get; set; }

		[XmlElement]
		public string SelectedQuestionId { get; set; }

		[XmlElement]
		public string SelectedQuizId { get; set; }

		[XmlElement]
		public string SelectedQuestionXml { get; set; }

		public object SelectedData { get; set; }

		[XmlElement]
		public int NextPageStartSearchQuestion { get; set; }

		[XmlElement]
		public int NextPageStartSearchQuiz { get; set; }

        [XmlElement]
        public Boolean BulkEdit { get; set; }

        [XmlElement]
        public string BulkEditStatus { get; set; }

        [XmlArray]
        public List<string> BulkEditSelectedQuestions { get; set; }

        [XmlArray]
        public List<string> FlagSelectedValues { get; set; }

         [XmlArray]
        public List<string> SelectedCustomQuestions { get; set; }

         public QuestionAdminSearchPanel()
         {
             this.ChapterSelectedValues = new List<string>() { "0" };
             this.QuizSelectedValues = new List<string>() { "0" };
             this.FormatSelectedValues = new List<string>() { "0" };
         }
	}


	
}

    public class QuestionEditor
    {
        public string QuestionId { get; set; }

        public string QuizId { get; set; }

        public string EnrollmentId { get; set; }

        public string EntityId { get; set; }

        public string QuestionType { get; set; }

        public string HtsPlayerUrl { get; set; }

        public string CustomUrl { get; set; }

        public bool ShowAdvanced { get; set; }

        public bool ShowSave { get; set; }

        public bool ShowCancel { get; set; }

        public bool ShowProperties { get; set; }

        public bool ShowFeedback { get; set; }

        public bool IsAdvancedConvert { get; set; }

        public string QuestionText { get; set; }
        public string CustomXML { get; set; }


    }

