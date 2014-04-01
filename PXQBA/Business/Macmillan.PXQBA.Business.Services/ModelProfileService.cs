using System;
using System.Collections.Generic;
using System.Linq;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Common.Helpers.Constants;

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
            // TODO: This is hardcoded to a single product course in question. This is not correct.
            var productCourse = question.ProductCourses.FirstOrDefault(p => p.ProductCourseDlapId == Constants.ProductCourseId);

            var data = new Dictionary<string, string>();
            data.Add(MetadataFieldNames.InlinePreview, question.Preview);
            data.Add(MetadataFieldNames.DlapType, SerachCommandXmlParserHelper.GetQuestionFullType(question.Type, availableQuestionTypes));
            data.Add(MetadataFieldNames.DlapTitle, question.DlapId);
            data.Add(MetadataFieldNames.Id, question.Id.ToString());
            data.Add(MetadataFieldNames.DlapStatus, EnumHelper.GetEnumDescription((QuestionStatus)Enum.Parse(typeof(QuestionStatus), question.Status.ToString())));
            if (productCourse != null)
            {
                data.Add(MetadataFieldNames.Chapter, productCourse.Chapter);
                data.Add(MetadataFieldNames.Bank, productCourse.Bank);
                data.Add(MetadataFieldNames.Sequence, productCourse.Sequence);
            }

            return data;
        }
    }
}