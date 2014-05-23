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
using Macmillan.PXQBA.Web.ViewModels.TiteList;
using Course = Macmillan.PXQBA.Business.Models.Course;
using LearningObjective = Macmillan.PXQBA.Business.Models.LearningObjective;
using Question = Macmillan.PXQBA.Business.Models.Question;

namespace Macmillan.PXQBA.Business.Services
{
    public class ModelProfileService : IModelProfileService
    {
        private readonly IProductCourseOperation productCourseOperation;

        private readonly IQuestionCommands questionCommands;

        public ModelProfileService(IProductCourseOperation productCourseOperation, IQuestionCommands questionCommands)
        {
            this.productCourseOperation = productCourseOperation;
            this.questionCommands = questionCommands;
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

            int status;
            if (int.TryParse(question.Status, out status))
            {
                metadata.Data.Add(MetadataFieldNames.QuestionStatus, ((QuestionStatus) status).GetDescription());
            }

            metadata.Data.Add(MetadataFieldNames.InlinePreview, question.Preview);
            metadata.Data.Add(MetadataFieldNames.DlapType, QuestionTypeHelper.GetDisplayName(question.InteractionType, question.CustomUrl));
            metadata.Data.Add(MetadataFieldNames.Id, question.Id);
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

        public IEnumerable<ChapterViewModel> GetChaptersViewModel(Course course)
        {
            IEnumerable<ChapterViewModel> chapters = course.GetChaptersList().Select(Mapper.Map<ChapterViewModel>).ToList();

            var questionCounts = questionCommands.GetQuestionCountByChapters(course.QuestionRepositoryCourseId, course.ProductCourseId);

            foreach (var questionCount in questionCounts)
            {
                var chapter = chapters.SingleOrDefault(ch => ch.Title == questionCount.Key);
                if (chapter != null)
                {
                    chapter.QuestionsCount = questionCount.Value;
                }
            }

            return chapters;
        }

        public IEnumerable<string> GetTitleNames(IEnumerable<string> titleIds)
        {
            return productCourseOperation.GetCoursesByCourseIds(titleIds).Select(c => c.Title);
        }

        public SharedQuestionDuplicateFromViewModel GetSourceQuestionSharedWith(ProductCourseSection section, Course course)
        {
            if (section != null)
            {
                if (section.ProductCourseValues.Any() &&
                    section.ProductCourseValues.ContainsKey(MetadataFieldNames.QuestionIdDuplicateFromShared))
                {
                    var value = section.ProductCourseValues[MetadataFieldNames.QuestionIdDuplicateFromShared].FirstOrDefault();
                    if (value != null)
                    {
                        var sharedWith =
                            string.Join(", ", productCourseOperation.GetCoursesByCourseIds(
                                questionCommands.GetQuestion(course.QuestionRepositoryCourseId, value)
                                    .ProductCourseSections.Select(s => s.ProductCourseId)).Select(c => c.Title));
                        return new SharedQuestionDuplicateFromViewModel
                               {
                                   QuestionId = value,
                                   ShareWith = sharedWith
                               };
                    }
                }
            }
            return null;
        }
    }
}