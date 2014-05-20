using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AutoMapper;
using Bfw.Agilix.DataContracts;
using Bfw.Common;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Commands.Helpers;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Common.Helpers.Constants;
using Macmillan.PXQBA.DataAccess.Data;
using Macmillan.PXQBA.Web.ViewModels;
using Course = Macmillan.PXQBA.Business.Models.Course;
using LearningObjective = Macmillan.PXQBA.Business.Models.LearningObjective;
using Question = Macmillan.PXQBA.Business.Models.Question;

namespace Macmillan.PXQBA.Business.Services
{
    public class ModelProfileService : IModelProfileService
    {
        private readonly IProductCourseOperation productCourseOperation;

        private Dictionary<string, string> availableQuestionTypes;
        private IQuestionCommands questionCommands;

        private ITemporaryQuestionOperation temporaryQuestionOperation;
        public ModelProfileService(IProductCourseOperation productCourseOperation, IQuestionCommands questionCommands, ITemporaryQuestionOperation temporaryQuestionOperation)
        {
            this.productCourseOperation = productCourseOperation;
            this.questionCommands = questionCommands;
            this.temporaryQuestionOperation = temporaryQuestionOperation;
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
            var questioncardLayout = src.Data.Element("questioncardlayout");
            if (questioncardLayout != null)
            {
                var scriptElement = questioncardLayout.Element("script");
                if (scriptElement != null)
                {
                    if (scriptElement.Element("div") != null)
                    {
                        return scriptElement.Element("div").ToString();
                    }
                }
            }
            return string.Empty;
        }

        public List<CourseMetadataFieldDescriptor> GetCourseMetadataFieldDescriptors(
            Bfw.Agilix.DataContracts.Course src)
        {
            return CourseDataXmlParser.ParseMetaAvailableQuestionData(src.Data);
        }

        public string GetQuestionBankRepositoryCourse(Bfw.Agilix.DataContracts.Course src)
        {
            return CourseDataXmlParser.ParseQuestionBankRepositoryCourse(src.Data);
        }

        public Dictionary<string, List<string>> GetQuestionDefaultValues(Bfw.Agilix.DataContracts.Question question)
        {
            return QuestionDataXmlParser.GetDefaultSectionValues(question.MetadataElements);
        }

        public List<ProductCourseSection> GetProductCourseSections(Bfw.Agilix.DataContracts.Question question)
        {
            return QuestionDataXmlParser.GetProductCourseSectionValues(question.MetadataElements);
        }

        public List<ProductCourseSection> GetProductCourseSections(QuestionViewModel viewModel)
        {
            var currentProductCourseId = viewModel.LocalValues[MetadataFieldNames.ProductCourse].First();
            var sections = new List<ProductCourseSection>()
                           {
                               new ProductCourseSection()
                               {
                                   ProductCourseId = currentProductCourseId,
                                   ProductCourseValues = viewModel.LocalValues
                               }
                           };
            var question = questionCommands.GetQuestion(viewModel.EntityId, viewModel.Id);
            sections.AddRange(question.ProductCourseSections.Where(s => s.ProductCourseId != currentProductCourseId));
            return sections;
        }

        public QuestionMetadata GetQuestionMetadataForCourse(Question question, Course course = null)
        {
            var metadata = new QuestionMetadata();

            long status;
            if (long.TryParse(question.Status, out status))
            {
                metadata.Data.Add(MetadataFieldNames.DlapStatus, ((QuestionStatus) status).GetDescription());
            }

            metadata.Data.Add(MetadataFieldNames.InlinePreview, question.Preview);
            metadata.Data.Add(MetadataFieldNames.DlapType, EnumHelper.GetEnumDescription(question.Type));
            metadata.Data.Add(MetadataFieldNames.Id, question.Id);
            metadata.Data.Add(MetadataFieldNames.QuestionIdDuplicateFrom, question.QuestionIdDuplicateFrom);
            var courseName = course != null ? course.Title : string.Empty;
            metadata.Data.Add(MetadataFieldNames.ProductCourse, courseName);
            var productCourseSection = course != null
                ? question.ProductCourseSections.FirstOrDefault(p => p.ProductCourseId == course.ProductCourseId)
                : question.ProductCourseSections.FirstOrDefault();
            if (productCourseSection != null)
            {
                foreach (var metadataValue in productCourseSection.ProductCourseValues)
                {
                    if (!metadata.Data.ContainsKey(metadataValue.Key))
                    {
                        metadata.Data.Add(metadataValue.Key, string.Join(", ", metadataValue.Value));
                    }
                }
            }
            return metadata;
        }

        public Dictionary<string, XElement> GetXmlMetadataElements(Question question)
        {
            return QuestionDataXmlParser.ToXmlElements(question);
        }


        public string GetHardCodedQuestionDuplicate()
        {
            return "67CE313C-ACEE-9747-4EA3-AFF66696C1DE";
        }

        public Question GetHardCodedSourceQuestion(int sharedFrom)
        {
            if (sharedFrom % 2 == 0)
            {
                return null;
            }
            return questionCommands.GetQuestion("", GetHardCodedQuestionDuplicate());
        }
       
    }
}