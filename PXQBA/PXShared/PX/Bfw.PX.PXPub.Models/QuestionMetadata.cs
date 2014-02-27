using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Web.Mvc;


namespace Bfw.PX.PXPub.Models
{
    public class QuestionMetadata
    {
        /// <summary>
        /// question id.
        /// </summary>
        public string QuestionId { get; set; }

        /// <summary>
        /// question title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// question entity id.
        /// </summary>
        public string EntityId { get; set; }
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
        /// The Id for the question this question is used in.
        /// </summary>
        public List<SelectListItem> LearningObjectives { get; set; }

        /// <summary>
        /// The Id for the question this question is used in.
        /// </summary>
        public List<SelectListItem> SuggestedUse { get; set; }


        public Question QuestionData { get; set; }

        public QuestionAdminSearchPanel Metadata { get; set; }

        /// <summary>
        /// List of Question card metadata for a question
        /// </summary>
        public List<QuestionCardData> QuestionCardData { get; set; }

        /// <summary>
        /// Old Quiz ID from Question
        /// </summary>
        public string OldQuizId { get; set; }

        public string QuestionBankText { get; set; }
        public string eBookChapterText { get; set; }


        /// <summary>
        /// Question Status can be "deleted", "In progress" , "Available to Instructor"
        /// </summary>
        public string QuestionStatus { get; set; }




        /// <summary>
        /// The core data that represents the assignment
        /// </summary>
        [DataMember]
        public IEnumerable<SelectListItem> SelectedSuggestUse { get; set; }

        /// <summary>
        /// The core data that represents the assignment
        /// </summary>
        [DataMember]
        public IEnumerable<SelectListItem> SelectedLearningObjectives { get; set; }

        public QuestionMetadata()
        {



        }


    }

}
