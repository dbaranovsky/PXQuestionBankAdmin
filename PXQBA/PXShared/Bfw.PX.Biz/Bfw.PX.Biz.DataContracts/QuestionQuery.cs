using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Wrapper class for a querying of questions.
    /// </summary>
    public class QuestionQuery
    {

        /// <summary>
        /// Gets or sets the entity id.
        /// </summary>
        /// <value>
        /// The entity id.
        /// </value>
        public string EntityId{get;set;}


        /// <summary>
        /// Gets or sets the item id.
        /// </summary>
        /// <value>
        /// The item id.
        /// </value>
        public string ItemId {get;set;}


        /// <summary>
        /// Gets or sets a value indicating whether [ignore banks].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [ignore banks]; otherwise, <c>false</c>.
        /// </value>
        public bool IgnoreBanks {get;set;}


        /// <summary>
        /// Gets or sets the question entity id.
        /// </summary>
        /// <value>
        /// The question entity id.
        /// </value>
        public string QuestionEntityId {get;set;}

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        /// <value>
        /// The start.
        /// </value>
        public string Start {get;set;}

        /// <summary>
        /// Gets or sets the end.
        /// </summary>
        /// <value>
        /// The end.
        /// </value>
        public string End {get;set;}
    }
}
