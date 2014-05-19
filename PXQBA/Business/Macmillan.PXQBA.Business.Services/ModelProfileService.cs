using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Commands.Helpers;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Common.Helpers.Constants;
using Macmillan.PXQBA.DataAccess.Data;
using Course = Macmillan.PXQBA.Business.Models.Course;
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

        public Dictionary<string, string> CreateQuestionMetadata(Question question)
        {
            var data = new Dictionary<string, string>();

            data.Add(MetadataFieldNames.InlinePreview, question.Preview);
            data.Add(MetadataFieldNames.DlapType, EnumHelper.GetEnumDescription(question.Type));
            //data.Add(MetadataFieldNames.DlapTitle, question.LocalMetadata.Title);
            data.Add(MetadataFieldNames.Id, question.Id);
            //data.Add(MetadataFieldNames.DlapStatus, EnumHelper.GetEnumDescription(question.LocalMetadata.Status));
            //data.Add(MetadataFieldNames.Chapter, question.LocalMetadata.Chapter);
            //data.Add(MetadataFieldNames.Bank, question.LocalMetadata.Bank);
            //data.Add(MetadataFieldNames.Sequence, question.LocalMetadata.Sequence.ToString());
            //data.Add(MetadataFieldNames.Difficulty, question.LocalMetadata.Difficulty);
            //data.Add(MetadataFieldNames.Keywords, String.Join(", ", question.LocalMetadata.Keywords));
            //data.Add(MetadataFieldNames.SuggestedUse, String.Join(", ", question.LocalMetadata.SuggestedUse));
            //data.Add(MetadataFieldNames.Guidance, question.LocalMetadata.Guidance);
            //data.Add(MetadataFieldNames.LearningObjectives, String.Join(", ", question.LocalMetadata.LearningObjectives.Select(lo => lo.Description)));
            //data.Add(MetadataFieldNames.SharedWith, question.ProductCourses.Count <= 1 ? string.Empty : String.Join("<br />", question.ProductCourses.Select(c => c.Title)));
            data.Add(MetadataFieldNames.QuestionIdDuplicateFrom, question.QuestionIdDuplicateFrom);
            //data.Add(MetadataFieldNames.ProductCourse, String.Join(", ", question.ProductCourses.Select(pc=>pc.Title)));
          

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

        public IEnumerable<CourseMetadataFieldDescriptor> GetCourseMetadataFieldDescriptors(
            Bfw.Agilix.DataContracts.Course src)
        {
            return CourseDataXmlParser.ParseMetaAvailableQuestionData(src.Data);
        }

        public string GetQuestionBankRepositoryCourse(Bfw.Agilix.DataContracts.Course src)
        {
            return CourseDataXmlParser.ParseQuestionBankRepositoryCourse(src.Data);
        }

        public Dictionary<string, IEnumerable<string>> GetQuestionDefaultValues(Bfw.Agilix.DataContracts.Question question)
        {
            return QuestionDataXmlParser.GetDefaultSectionValues(question.MetadataElements);
        }

        public IEnumerable<ProductCourseSection> GetProductCourseSections(Bfw.Agilix.DataContracts.Question question)
        {
            return QuestionDataXmlParser.GetProductCourseSectionValues(question.MetadataElements);
        }

        public QuestionMetadata GetQuestionMetadataForCourse(Question question, string courseId)
        {
            var metadata = new QuestionMetadata();

            metadata.Data.Add(MetadataFieldNames.InlinePreview, question.Preview);
            metadata.Data.Add(MetadataFieldNames.DlapType, EnumHelper.GetEnumDescription(question.Type));
            metadata.Data.Add(MetadataFieldNames.Id, question.Id);
            metadata.Data.Add(MetadataFieldNames.QuestionIdDuplicateFrom, question.QuestionIdDuplicateFrom);
            var courseName = string.Empty;
            if (!string.IsNullOrEmpty(courseId))
            {
                var course = productCourseOperation.GetProductCourse(courseId);
                courseName = course != null ? course.Title : string.Empty;
            }
            metadata.Data.Add(MetadataFieldNames.ProductCourseName, courseName);
            var productCourseSection = !string.IsNullOrEmpty(courseId)
                ? question.ProductCourseSections.FirstOrDefault(p => p.ProductCourseId == courseId)
                : question.ProductCourseSections.FirstOrDefault();
            if (productCourseSection != null)
            {
                foreach (var metadataValue in productCourseSection.ProductCourseValues)
                {
                    metadata.Data.Add(metadataValue.Key, string.Join(", ", metadataValue.Value));
                }
            }
            return metadata;
        }


        public IEnumerable<ProductCourseSection> GetHardCodedSharedProductCourses(ProductCourse productCourse)
        {
            if (productCourse.QuestionId%2 != 0)
            {
                return new List<ProductCourseSection>
                {
                   
                };
            }
            else
            {
                return new List<ProductCourseSection>()
                {
                   // new ProductCourseSection {Id = productCourse.Id.ToString(), Title = productCourse.Title}
                };
            }
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

        public string GetQuizIdForQuestion(string id, string entityId)
        {
            return temporaryQuestionOperation.GetQuizIdForQuestion(id, entityId);
        }
    }
}