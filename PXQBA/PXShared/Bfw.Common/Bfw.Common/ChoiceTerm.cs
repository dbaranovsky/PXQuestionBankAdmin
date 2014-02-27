using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.HtsConversion
{
    /// <summary>
    /// Class to match choice with term.
    /// </summary>
    public class ChoiceTerm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChoiceTerm"/> class.
        /// </summary>
        /// <param name="choice">The choice.</param>
        /// <param name="term">The term.</param>
        public ChoiceTerm(string choice, string term)
        {
            this.choice = choice;
            this.term = term;
        }

        /// <summary>
        /// The choice.
        /// </summary>
        private string choice = string.Empty;

        /// <summary>
        /// The term.
        /// </summary>
        private string term = string.Empty;

        /// <summary>
        /// Gets the choice.
        /// </summary>
        public string Choice
        {
            get
            {
                return this.choice;
            }
        }

        /// <summary>
        /// Gets the term.
        /// </summary>
        public string Term
        {
            get
            {
                return this.term;
            }
        }

        /// <summary>
        /// For the 'choice', find the 'term' from list of 'choiceAndTerms', using that term find the index in 'terms'
        /// </summary>
        /// <param name="choiceAndTerms">List of mapping between choices and terms.</param>
        /// <param name="choice">Choice for which we need index of term.</param>
        /// <param name="terms">List of terms.</param>
        /// <returns></returns>
        public static int GetTermIndex(List<ChoiceTerm> choiceAndTerms, string choice, List<string> terms)
        {
            int index = -2;

            for (int i = 0; i < choiceAndTerms.Count; i++)
            {
                if (choiceAndTerms[i].choice == choice)
                {
                    string term = choiceAndTerms[i].term;
                    index = terms.IndexOf(term);
                    break;
                }
            }

            index++; // increment index by 1 because we want index to start from 1 instead of zero
            return index;
        }
    }
}
