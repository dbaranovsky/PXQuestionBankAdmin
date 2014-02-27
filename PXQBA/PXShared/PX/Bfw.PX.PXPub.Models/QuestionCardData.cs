using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Web.Mvc;

namespace Bfw.PX.PXPub.Models
{

    public class QuestionCardData
    {
        #region Properties
        /// <summary>
        /// The core data that represents the assignment
        /// </summary>
        [DataMember]
        public string QuestionCardDataName { get; set; }

        /// <summary>
        /// The core data that represents the assignment
        /// </summary>
        [DataMember]
        public bool Filterable { get; set; }
        /// <summary>
        /// The core data that represents the assignment
        /// </summary>
        [DataMember]
        public string FriendlyName { get; set; }
        /// <summary>
        /// The core data that represents the assignment
        /// </summary>
        [DataMember]
        public string SearchTerm { get; protected set; }

        /// <summary>
        /// The core data that represents the assignment
        /// </summary>
        [DataMember]
        public List<string> QuestionValues { get; set; }

        /// <summary>
        /// The core data that represents the assignment
        /// </summary>
        [DataMember]
        public IEnumerable<SelectListItem> SelectedQuestionValues { get; set; }


        #endregion

        #region Constructors

        /// <summary>
        /// Question Card Constructor
        /// </summary>
        public QuestionCardData()
        {
            QuestionValues = new List<string>();

        }

        #endregion

    
    }
}