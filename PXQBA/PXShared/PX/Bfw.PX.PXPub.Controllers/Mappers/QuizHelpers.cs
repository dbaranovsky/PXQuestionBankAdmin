using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class QuizHelpers
    {

        /// <summary>
        /// A helper method to generate a dictionary object Interation Types
        /// </summary>
        private static readonly Dictionary<string, BizDC.InteractionType> TypeDescriptionMap = new Dictionary<string, BizDC.InteractionType>() {
            { "A", BizDC.InteractionType.Answer },
            { "MC", BizDC.InteractionType.Choice },
            { "COMP", BizDC.InteractionType.Composite },
            { "CUSTOM", BizDC.InteractionType.Custom },
            { "HTS", BizDC.InteractionType.Custom },
            { "E", BizDC.InteractionType.Essay },
            { "MT", BizDC.InteractionType.Match },
            { "TXT", BizDC.InteractionType.Text },
            { "BANK", BizDC.InteractionType.Bank },
            {"1", BizDC.InteractionType.NotBank},
            {"2", BizDC.InteractionType.Bank},
        };

        /// <summary>
        /// A helper method to get the description for an Interaction type.
        /// </summary>
        /// <param name="interactionType">Type of the interaction.</param>
        /// <returns></returns>
        internal static string DescriptionForInteractionType(Biz.DataContracts.InteractionType interactionType)
        {
            foreach (var kvp in TypeDescriptionMap)
            {
                if (kvp.Value == interactionType)
                {
                    return kvp.Key;
                }
            }

            throw new ArgumentOutOfRangeException("interactionType");
        }

        /// <summary>
        /// Returns an Interaction Type based on a description.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        internal static Biz.DataContracts.InteractionType InteractionTypeForDescription(string description)
        {
            try
            {
                return TypeDescriptionMap[description];
            }
            catch
            {
                throw new ArgumentOutOfRangeException("description");
            }
        }
    }
}
