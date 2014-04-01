using System.Collections.Generic;
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
            data.Add("id", question.DlapId);
            data.Add("chapter", question.EBookChapter);
            data.Add("bank", question.QuestionBank);
            data.Add("seq", question.Sequency);
            data.Add("dlap_q_status", question.Status);

            return data;
        }
    }
}