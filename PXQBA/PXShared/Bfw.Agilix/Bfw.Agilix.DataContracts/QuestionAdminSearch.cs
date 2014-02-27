using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.Agilix.DataContracts
{
	/// <summary>
	/// Parameters for getting questions from DLAP
	/// </summary>
	[DataContract]
	public class QuestionAdminSearch: QuestionSearch
	{

		/// <summary>
		/// Query Filter for getting questions from DLAP
		/// Example: query=/interaction@type='choice'
		/// </summary>
		public string QuestionsFilter { get; set; }


		/// <summary>
		/// Number of Questions to Return
		/// </summary>
		public int Count  { get; set; }


        /// <summary>
        /// version of the item / question
        /// </summary>
        public bool version { get; set; }
	}
}