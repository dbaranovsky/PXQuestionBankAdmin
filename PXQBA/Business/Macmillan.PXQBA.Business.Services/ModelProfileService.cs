using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using AutoMapper;
using Bfw.Agilix.DataContracts;
using Bfw.Common;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Commands.Helpers;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Business.QuestionParserModule.DataContracts;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Common.Helpers.Constants;
using Macmillan.PXQBA.Web.ViewModels;
using Macmillan.PXQBA.Web.ViewModels.MetadataConfig;
using Macmillan.PXQBA.Web.ViewModels.TiteList;
using Macmillan.PXQBA.Web.ViewModels.User;
using Macmillan.PXQBA.Web.ViewModels.Versions;
using Course = Macmillan.PXQBA.Business.Models.Course;
using Question = Macmillan.PXQBA.Business.Models.Question;
using QuestionChoice = Macmillan.PXQBA.Business.Models.QuestionChoice;

namespace Macmillan.PXQBA.Business.Services
{
    public class ModelProfileService : IModelProfileService
    {
        private readonly IProductCourseOperation productCourseOperation;

        private readonly IQuestionCommands questionCommands;

        private readonly IUserOperation userOperation;

        private readonly INoteCommands noteCommands;

        public ModelProfileService(IProductCourseOperation productCourseOperation, IQuestionCommands questionCommands, IUserOperation userOperation, INoteCommands noteCommands)
        {
            this.productCourseOperation = productCourseOperation;
            this.questionCommands = questionCommands;
            this.userOperation = userOperation;
            this.noteCommands = noteCommands;
        }

        public QuestionMetadataSection GetQuestionDefaultValues(Bfw.Agilix.DataContracts.Question question)
        {
            return QuestionDataXmlParser.GetDefaultSectionValues(question.MetadataElements);
        }

        public List<QuestionMetadataSection> GetProductCourseSections(Bfw.Agilix.DataContracts.Question question)
        {
            return QuestionDataXmlParser.GetProductCourseSectionValues(question.MetadataElements);
        }

        public List<QuestionMetadataSection> GetProductCourseSections(QuestionViewModel viewModel)
        {
            var currentProductCourseId = viewModel.LocalSection.ProductCourseId;
            var sections = new List<QuestionMetadataSection>()
                           {
                               viewModel.LocalSection
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

            string currentCourseId = String.Empty;
            if (course != null)
            {
                currentCourseId = course.ProductCourseId;
            }
            var productCourses = GetTitleNames(question.ProductCourseSections.Where(p => p.ProductCourseId != currentCourseId).Select(p => p.ProductCourseId));
            metadata.Data.Add(MetadataFieldNames.DraftFrom, question.DraftFrom);

            metadata.Data.Add(MetadataFieldNames.SharedWith, string.Join("<br>", productCourses));
            
            var courseName = course != null ? course.Title : string.Empty;
            metadata.Data.Add(MetadataFieldNames.ProductCourse, courseName);
            var productCourseSection = course != null
                ? question.ProductCourseSections.FirstOrDefault(p => p.ProductCourseId == course.ProductCourseId)
                : question.ProductCourseSections.FirstOrDefault();
            if (productCourseSection != null)
            {
                metadata.Data.Add(MetadataFieldNames.DlapTitle, productCourseSection.Title);
                metadata.Data.Add(MetadataFieldNames.Chapter, productCourseSection.Chapter);
                metadata.Data.Add(MetadataFieldNames.Bank, productCourseSection.Bank);
                metadata.Data.Add(MetadataFieldNames.Sequence, productCourseSection.Sequence);
                metadata.Data.Add(MetadataFieldNames.Flag, productCourseSection.Flag);
                metadata.Data.Add(MetadataFieldNames.Notes, string.Join("<br>", noteCommands.GetQuestionNotes(question.Id).Select(x=> "- "+x.Text)));

                foreach (var metadataValue in productCourseSection.DynamicValues)
                {
                    var values = metadataValue.Value;
                    if (course != null)
                    {
                        var courseDescriptors = course.FieldDescriptors.Where(x => x.Name == metadataValue.Key);
                        if (courseDescriptors.Any() && courseDescriptors.First().Type == MetadataFieldType.ItemLink && values.Any())
                        {
                            var itemLinkValues = values.Select(value => courseDescriptors.First().CourseMetadataFieldValues.Where(x => String.Format(CourseDataXmlHelper.ItemLinkPatterm, x.Id) == value));
                            if (itemLinkValues.Any())
                            {
                                values = itemLinkValues.Where(x => x.Any()).Select(x => x.First().Text).ToList();
                            }
                        }
                    }

                    if (!metadata.Data.ContainsKey(metadataValue.Key))
                    {
                        metadata.Data.Add(metadataValue.Key, string.Join(", ", values));
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

            var facetCounts = questionCommands.GetFacetedResults(course.QuestionRepositoryCourseId, course.ProductCourseId, MetadataFieldNames.Chapter);
            foreach (var chapterViewModel in chapters)
            {
                var facetCount = facetCounts.FirstOrDefault(f => f.FacetedFieldValue == chapterViewModel.Title);
                if (facetCount != null)
                {
                    chapterViewModel.QuestionsCount = facetCount.FacetedCount;
                }
            }

            //var questionCounts = questionCommands.GetQuestionCountByChapters(course.QuestionRepositoryCourseId, course.ProductCourseId, chapters.Select(c => c.Title));

            //foreach (var questionCount in questionCounts)
            //{
            //    var chapter = chapters.SingleOrDefault(ch => ch.Title == questionCount.Key);
            //    if (chapter != null)
            //    {
            //        chapter.QuestionsCount = questionCount.Value;
            //    }
            //}

            return chapters;
        }

        public IEnumerable<string> GetTitleNames(IEnumerable<string> titleIds)
        {
            return productCourseOperation.GetCoursesByCourseIds(titleIds).Select(c => c.Title);
        }

        public SharedQuestionDuplicateFromViewModel GetSourceQuestionSharedFrom(string questionIdDuplicateFrom, Course course)
        {
            if (!string.IsNullOrEmpty(questionIdDuplicateFrom))
            {
                if (!string.IsNullOrEmpty(questionIdDuplicateFrom))
                {
                        var sharedWith =
                            string.Join(", ", productCourseOperation.GetCoursesByCourseIds(questionCommands.GetQuestion(course.QuestionRepositoryCourseId, questionIdDuplicateFrom)
                                    .ProductCourseSections.Select(s => s.ProductCourseId)).Select(c => c.Title));
                        return new SharedQuestionDuplicateFromViewModel
                               {
                                   QuestionId = questionIdDuplicateFrom,
                                   SharedWith = sharedWith
                               };
                }
            }
            return null;
        }

        public QuestionMetadataSection GetDefaultSectionForViewModel(Question question)
        {
            var section = question.DefaultSection;
            var parentProductCourseId =
                !string.IsNullOrEmpty(question.ProductCourseSections.First().ParentProductCourseId)
                    ? question.ProductCourseSections.First().ParentProductCourseId
                    : question.ProductCourseSections.First().ProductCourseId;
            var course = productCourseOperation.GetProductCourse(parentProductCourseId);
            var dynamicsFields = course.FieldDescriptors.Where(f => !MetadataFieldNames.GetStaticFieldNames().Contains(f.Name));
            foreach (var courseMetadataFieldDescriptor in dynamicsFields.Where(courseMetadataFieldDescriptor => !section.DynamicValues.ContainsKey(courseMetadataFieldDescriptor.Name)))
            {
                section.DynamicValues.Add(courseMetadataFieldDescriptor.Name, new List<string>());
            }
            return section;
        }

        public string GetModifierName(string modifiedByUserId)
        {
            if (!string.IsNullOrEmpty(modifiedByUserId))
            {
                var user = userOperation.GetUser(modifiedByUserId);
                if (user != null)
                {
                    return string.Format("{0} {1}", user.FirstName, user.LastName);
                }
            }
            return "(Unknown)";
        }

        public Question GetDuplicateFromQuestion(string repositoryCourseId, string duplicateFrom)
        {
            return questionCommands.GetQuestion(repositoryCourseId, duplicateFrom);
        }

        public string GetDuplicateFromShared(Bfw.Agilix.DataContracts.Question question)
        {
            return QuestionDataXmlParser.GetMetadataField(question.MetadataElements, MetadataFieldNames.DuplicateFromShared);
        }

        public string GetDuplicateFrom(Bfw.Agilix.DataContracts.Question question)
        {
            return QuestionDataXmlParser.GetMetadataField(question.MetadataElements, MetadataFieldNames.DuplicateFrom);
        }

        public string GetDraftFrom(Bfw.Agilix.DataContracts.Question question)
        {
            return QuestionDataXmlParser.GetMetadataField(question.MetadataElements, MetadataFieldNames.DraftFrom);
        }

        public string GetRestoredFromVersion(Bfw.Agilix.DataContracts.Question question)
        {
            return QuestionDataXmlParser.GetMetadataField(question.MetadataElements, MetadataFieldNames.RestoredFromVersion);
        }

        public bool GetPublishedFromDraft(Bfw.Agilix.DataContracts.Question question)
        {
            var isPublishedFromDraft = QuestionDataXmlParser.GetMetadataField(question.MetadataElements, MetadataFieldNames.IsPublishedFromDraft);
            bool result;
            bool.TryParse(isPublishedFromDraft, out result);
            return result;
        }

        public string GetModifiedBy(Bfw.Agilix.DataContracts.Question question)
        {
            return QuestionDataXmlParser.GetMetadataField(question.MetadataElements, MetadataFieldNames.ModifiedBy);
        }

        public int GetNumericVersion(string questionVersion)
        {
            var version = 0;
            int.TryParse(questionVersion, out version);
            return version;
        }

        public string GetCourseBanks(Course course)
        {
            return GetMetaFieldValuesToString(course, MetadataFieldNames.Bank);
        }

        public string GetCourseChapters(Course course)
        {
            return GetMetaFieldValuesToString(course, MetadataFieldNames.Chapter);
        }

        public IEnumerable<AvailableChoiceItem> GetMetadataFieldValues(CourseMetadataFieldDescriptor field)
        {
            return field.CourseMetadataFieldValues.OrderBy(v => v.Sequence).Select(v => new AvailableChoiceItem(string.IsNullOrEmpty(v.Id)? v.Text: v.Id, v.Text));
        }

        public IEnumerable<CourseMetadataFieldDescriptor> GetCourseFieldDescriptors(MetadataConfigViewModel metadataConfigViewModel)
        {
            var fieldDescriptors = new List<CourseMetadataFieldDescriptor>();
            fieldDescriptors.Add(GetFieldDescriptorWithSplitedValues(metadataConfigViewModel.Banks, MetadataFieldNames.Bank));
            fieldDescriptors.Add(GetFieldDescriptorWithSplitedValues(metadataConfigViewModel.Chapters, MetadataFieldNames.Chapter));

            foreach (var courseMetadataFieldDescriptor in CourseExtensions.GetPredefinedCourseFields())
            {
                if (fieldDescriptors.All(d => d.Name != courseMetadataFieldDescriptor.Name))
                {
                    fieldDescriptors.Add(courseMetadataFieldDescriptor);
                }
            }

            if (metadataConfigViewModel.Fields == null)
            {
                return fieldDescriptors;
            }

            foreach (var field in metadataConfigViewModel.Fields)
            {
                if (fieldDescriptors.All(d => d.Name != field.InternalName))
                {
                    fieldDescriptors.Add(new CourseMetadataFieldDescriptor()
                    {
                        Filterable = field.DisplayOptions.Filterable,
                        DisplayInBanks = field.DisplayOptions.DisplayInBanks,
                        ShowFilterInBanks = field.DisplayOptions.ShowFilterInBanks,
                        MatchInBanks = field.DisplayOptions.MatchInBanks,
                        DisplayInCurrentQuiz = field.DisplayOptions.DisplayInCurrentQuiz,
                        DisplayInInstructorQuiz = field.DisplayOptions.DisplayInInstructorQuiz,
                        DisplayInResources = field.DisplayOptions.DisplayInResources,
                        ShowFilterInResources = field.DisplayOptions.ShowFilterInResources,
                        MatchInResources = field.DisplayOptions.MatchInResources,
                        FriendlyName = field.FieldName,
                        Name = field.InternalName,
                        Type = field.FieldType,
                        CourseMetadataFieldValues =
                            field.ValuesOptions == null
                                ? null
                                : field.ValuesOptions.Where(vo=>(!String.IsNullOrEmpty(vo.Value))&&(!String.IsNullOrEmpty(vo.Text))).Select((v, i) => new CourseMetadataFieldValue()
                                {
                                    Id = field.FieldType == MetadataFieldType.ItemLink ? v.Value : null,
                                    Text = v.Text,
                                    Sequence = i
                                })
                    });
                }
            }
            return fieldDescriptors;
        }

        public MetadataFieldType GetMetadataFieldType(string type)
        {
            if (type == "single-select")
            {
                return MetadataFieldType.SingleSelect;
            }

            if (type == "multi-select")
            {
                return MetadataFieldType.MultiSelect;
            }
            if (type == "multi-line")
            {
                return MetadataFieldType.MultilineText;
            }
            if (type == "keywords")
            {
                return MetadataFieldType.Keywords;
            }
            if (type == "item-link")
            {
                return MetadataFieldType.ItemLink;
            }

            return MetadataFieldType.Text;
        }

        public string MetadataFieldTypeToString(MetadataFieldType type)
        {
            if (type == MetadataFieldType.MultiSelect)
            {
                return "multi-select";
            }
            if (type == MetadataFieldType.SingleSelect)
            {
                return "single-select";
            }
            if (type == MetadataFieldType.MultilineText)
            {
                return "multi-line";
            }
            if (type == MetadataFieldType.Keywords)
            {
                return "keywords";
            }
            if (type == MetadataFieldType.ItemLink)
            {
                return "item-link";
            }
            return "text";
        }

        public Question GetQuestionVersion(string entityId, string id, string version)
        {
            if (!string.IsNullOrEmpty(version))
            {
                return questionCommands.GetQuestion(entityId, id, version);
            }
            return null;
        }

        public IEnumerable<Capability> GetActiveRoleCapabilities(RoleViewModel src)
        {
            return src.CapabilityGroups.SelectMany(c => c.Capabilities.Where(cap => cap.IsActive)).Select(c => (Capability)Enum.Parse(typeof(Capability), c.Id.ToString()));
        }

        public IEnumerable<CapabilityGroupViewModel> GetCapabilityGroups(IList<Capability> capabilities)
        {
            var groups = CapabilityHelper.GetCapabilityGroups().ToList();
            return groups.Select(keyValuePair => new CapabilityGroupViewModel()
            {
                Name = keyValuePair.Key, Capabilities = keyValuePair.Value.Select(v => new CapabilityViewModel
                {
                    Id = (int) v, Name = EnumHelper.GetEnumDescription(v), IsActive = capabilities.Contains(v)
                })
            }).ToList();
        }

        public IEnumerable<CourseMetadataFieldDescriptor> MapFieldsWithItemLinks(List<QuestionCardData> questionCardData, XElement courseData)
        {
            var fields = new List<CourseMetadataFieldDescriptor>();
            if (questionCardData != null)
            {
                fields = Mapper.Map<IEnumerable<CourseMetadataFieldDescriptor>>(questionCardData).ToList();
            }
           
            fields.AddRange(CourseDataXmlHelper.GetItemLinksDescriptors(courseData));
            return fields;
        }

        public string GetTypeFromParsedType(ParsedQuestionType type)
        {
            switch (type)
            {
                 case ParsedQuestionType.MultipleChoice:
                    return "choice";
                 case ParsedQuestionType.MultipleAnswer:
                    return "answer";
                 case ParsedQuestionType.Essay:
                    return "essay";
                 case ParsedQuestionType.ShortAnswer:
                    return "text";
                 case ParsedQuestionType.Matching:
                    return "match";
                default:
                    return "choice";
            }
        }

        public Question GetQuestionFromParsedQuestion(ParsedQuestion parsedQuestion, Course course)
        {
            var question = new Question
            {
                Id = parsedQuestion.Id,
                Body = parsedQuestion.Text,
                GeneralFeedback = parsedQuestion.Feedback,
                Score = parsedQuestion.Points.HasValue ? parsedQuestion.Points.Value : 0,
                AnswerList = parsedQuestion.Choices.Where(c => c.IsCorrect).Select(c => c.Id).ToList(),
                InteractionType = GetTypeFromParsedType(parsedQuestion.Type),
                Choices = parsedQuestion.Choices.Select(GetQuestionChoice).ToList(),
                EntityId = course.QuestionRepositoryCourseId,
                Status = ((int)QuestionStatus.InProgress).ToString()
            };
            question.ProductCourseSections.Add(new QuestionMetadataSection
            {
                ProductCourseId = course.ProductCourseId,
                Title = parsedQuestion.Title,
                DynamicValues = parsedQuestion.MetadataSection
            });
            return question;
        }

        public Resource GetResourceFromParsedResource(ParsedResource parsedResource, string courseId)
        {
            var resource = new Resource()
                           {
                               EntityId = courseId,
                               Url = ConfigurationHelper.GetBrainhoneyCourseImageFolder()+parsedResource.FullPath,
                               ResourceStream = new MemoryStream(parsedResource.BinData),
                               CreationDate = DateTime.Now,
                               ModifiedDate = DateTime.Now
                           };

            return resource;
        }

        public IEnumerable<CourseMetadataFieldDescriptor> ChooseFieldsForMetadataConfig(IList<CourseMetadataFieldDescriptor> fieldDescriptors)
        {
            return fieldDescriptors.Where(f => !CourseExtensions.GetPredefinedCourseFields().Select(d => d.Name).Contains(f.Name));
        }

        private QuestionChoice GetQuestionChoice(ParsedQuestionChoice parsedChoice)
        {
            return new QuestionChoice()
            {
                Id = parsedChoice.Id,
                Text = parsedChoice.Text,
                Answer = parsedChoice.Answer,
                Feedback = parsedChoice.Feedback
            };
        }

        private CourseMetadataFieldDescriptor GetFieldDescriptorWithSplitedValues(string concatedValues, string internalName)
        {
            return new CourseMetadataFieldDescriptor
            {
                Type = MetadataFieldType.SingleSelect,
                FriendlyName = internalName[0].ToString().ToUpper() + internalName.Substring(1),
                Name = internalName,
                Filterable = true,
                DisplayInBanks = true,
                ShowFilterInBanks = true,
                MatchInBanks = true,
                DisplayInCurrentQuiz = true,
                DisplayInInstructorQuiz = true,
                DisplayInResources = true,
                ShowFilterInResources = true,
                MatchInResources = true,
                CourseMetadataFieldValues = string.IsNullOrEmpty(concatedValues) ? null :
                    concatedValues.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                        .Select((v, i) => new CourseMetadataFieldValue() { Sequence = i, Text = v })
            };
        }

        private string GetMetaFieldValuesToString(Course course, string fieldName)
        {
            var field = course.FieldDescriptors.FirstOrDefault(f => f.Name == fieldName);
            if (field != null)
            {
                return string.Join("\n", field.CourseMetadataFieldValues.OrderBy(f => f.Sequence).Select(f => f.Text));
            }
            return string.Empty;
        }
    }
}