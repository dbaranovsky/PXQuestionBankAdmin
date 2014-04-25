using System;
using System.Collections.Generic;
using System.Linq;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Common.Helpers.Constants;

namespace Macmillan.PXQBA.Business.Services
{
    public class ModelProfileService : IModelProfileService
    {
        private readonly IProductCourseOperation productCourseOperation;

        private Dictionary<string, string> availableQuestionTypes;
        private IQuestionCommands questionCommands;
        public ModelProfileService(IProductCourseOperation productCourseOperation, IQuestionCommands questionCommands)
        {
            this.productCourseOperation = productCourseOperation;
            this.questionCommands = questionCommands;
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

        public Dictionary<string, string> CreateQuestionMetadata(Question question)
        {
            var data = new Dictionary<string, string>();

            data.Add(MetadataFieldNames.InlinePreview, question.Preview);
            data.Add(MetadataFieldNames.DlapType, EnumHelper.GetEnumDescription(question.Type));
            data.Add(MetadataFieldNames.DlapTitle, question.Title);
            data.Add(MetadataFieldNames.Id, question.Id);
            data.Add(MetadataFieldNames.DlapStatus, EnumHelper.GetEnumDescription(question.Status));
            data.Add(MetadataFieldNames.Chapter, question.Chapter);
            data.Add(MetadataFieldNames.Bank, question.Bank);
            data.Add(MetadataFieldNames.Sequence, question.Sequence.ToString());
            data.Add(MetadataFieldNames.Difficulty, question.Difficulty);
            data.Add(MetadataFieldNames.Keywords, String.Join(", ", question.Keywords));
            data.Add(MetadataFieldNames.SuggestedUse, String.Join(", ", question.SuggestedUse));
            data.Add(MetadataFieldNames.Guidance, question.Guidance);
            data.Add(MetadataFieldNames.LearningObjectives, String.Join(", ", question.LearningObjectives.Select(lo => lo.Description)));

            return data;
        }

        public string SetLearningObjectives(IEnumerable<LearningObjective> learningObjectives)
        {
            return (learningObjectives != null) ? string.Join("|", learningObjectives.Select(lo => lo.Guid)) : null;
        }

        public IEnumerable<LearningObjective> GetLOByGuid(string productCourseId, string learningObjectiveGuids)
        {
            if (learningObjectiveGuids == null)
            {
                return new List<LearningObjective>();
            }
            var guids = learningObjectiveGuids.Split('|');
            return productCourseOperation.GetProductCourse(productCourseId).LearningObjectives.Where(lo => guids.Contains(lo.Guid));
        }

        public string GetQuestionCardLayout(Bfw.Agilix.DataContracts.Course src)
        {
            if (src.Data.Element("questioncardlayout") != null)
            {
                return src.Data.Element("questioncardlayout").ToString();
            }
            return string.Empty;
        }

        public IEnumerable<Chapter> GetHardCodedQuestionChapters()
        {
            return new List<Chapter>()
            {
                new Chapter()
                {
                    Title = "Chapter 1",
                    QuestionsCount = 2
                },
                new Chapter
                {
                    Title = "Chapter 2",
                    QuestionsCount = 3
                },
                new Chapter
                {
                    Title = "Chapter 3",
                    QuestionsCount = 3
                }
            };
        }
    }
}