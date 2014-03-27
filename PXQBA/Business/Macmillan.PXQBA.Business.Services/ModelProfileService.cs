using System.Collections.Generic;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Services
{
    public class ModelProfileService : IModelProfileService
    {
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
    }
}