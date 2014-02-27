using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]

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

        #region Methods


        #endregion

    
    }
}