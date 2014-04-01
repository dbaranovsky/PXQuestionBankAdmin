using System;
using System.Collections.Generic;
using System.Linq;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;

namespace Macmillan.PXQBA.Business.Services
{
    public class ModelProfileService : IModelProfileService
    {
        private Dictionary<string, string> availableQuestionTypes;
        public ModelProfileService()
        {
            this.availableQuestionTypes = ConfigurationHelper.GetQuestionTypes();
        }

        /// <summary>
        /// A helper method to generate a dictionary object Interation Types
        /// </summary>
        private static readonly Dictionary<string, InteractionType> interactionTypes = new Dictionary<string, InteractionType>() {
            { "A", InteractionType.Answer },
            { "MC", InteractionType.Choice },
            { "COMP", InteractionType.Composite },
            { "CUSTOM", InteractionType.Custom },
            { "HTS", InteractionType.Custom },
            { "E", InteractionType.Essay },
            { "MT", InteractionType.Match },
            { "TXT", InteractionType.Text },
            { "BANK", InteractionType.Bank },
            {"1", InteractionType.NotBank},
            {"2", InteractionType.Bank},
        };

        public InteractionType CreateInteractionType(string questionType)
        {
            if (interactionTypes.ContainsKey(questionType))
            {
                return interactionTypes[questionType];
            }
            return InteractionType.Custom;
        }

        public Dictionary<string, string> CreateQuestionMetadata(DataAccess.Data.Question question)
        {
            var data = new Dictionary<string, string>();
            data.Add("questionHtmlInlinePreview", question.Preview);
            data.Add("dlap_q_type", SerachCommandXmlParserHelper.GetQuestionFullType(question.Type, availableQuestionTypes));
            data.Add("dlap_title", question.DlapId);
            data.Add("id", question.Id.ToString());
            // TODO: This is hardcoded to a single product course in question. This is not correct.
            data.Add("chapter", question.ProductCourses.First().Chapter);
            data.Add("bank", question.ProductCourses.First().Bank);
            data.Add("seq", question.ProductCourses.First().Sequence);
            data.Add("dlap_q_status", EnumHelper.GetEnumDescription((QuestionStatus)Enum.Parse(typeof(QuestionStatus), question.Status)));

            return data;
        }
    }
}