using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using AutoMapper;
using Bfw.Agilix.DataContracts;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Web.ViewModels;
using Macmillan.PXQBA.Web.ViewModels.MetadataConfig;
using Macmillan.PXQBA.Web.ViewModels.TiteList;
using Macmillan.PXQBA.Web.ViewModels.User;
using Macmillan.PXQBA.Web.ViewModels.Versions;
using Course = Macmillan.PXQBA.Business.Models.Course;
using Question = Macmillan.PXQBA.Business.Models.Question;
using QuestionChoice = Macmillan.PXQBA.Business.Models.QuestionChoice;

namespace Macmillan.PXQBA.Business.Services.Automapper
{
    public class ModelProfile : Profile
    {
        private readonly IModelProfileService modelProfileService;

        public ModelProfile(IModelProfileService modelProfileService)
        {
            this.modelProfileService = modelProfileService;
        }

        protected override void Configure()
        {
            Mapper.CreateMap<Bfw.Agilix.DataContracts.Course, Course>()
                .ForMember(dest => dest.ProductCourseId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.FieldDescriptors,
                    opt => opt.MapFrom(src => src.QuestionCardData))
                .ForMember(dest => dest.QuestionRepositoryCourseId, 
                    opt => opt.MapFrom(src => src.QuestionBankRepositoryCourse));

            Mapper.CreateMap<QuestionCardData, CourseMetadataFieldDescriptor>()
                 .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.QuestionCardDataName))
                 .ForMember(dest => dest.Type, opt => opt.MapFrom(src => modelProfileService.GetMetadataFieldType(src.Type)))
                 .ForMember(dest => dest.CourseMetadataFieldValues, opt => opt.MapFrom(src => src.QuestionValues));

            Mapper.CreateMap<QuestionCardDataValue, CourseMetadataFieldValue>();

            Mapper.CreateMap<Course, Bfw.Agilix.DataContracts.Course>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ProductCourseId, opt => opt.Ignore())
                .ForMember(dest => dest.Title, opt => opt.Ignore())
                .ForMember(dest => dest.QuestionCardData, opt => opt.MapFrom(src => src.FieldDescriptors));

            Mapper.CreateMap<CourseMetadataFieldDescriptor, QuestionCardData>()
                 .ForMember(dest => dest.QuestionCardDataName, opt => opt.MapFrom(src => src.Name))
                 .ForMember(dest => dest.Type, opt => opt.MapFrom(src => modelProfileService.MetadataFieldTypeToString(src.Type)))
                 .ForMember(dest => dest.QuestionValues, opt => opt.MapFrom(src => src.CourseMetadataFieldValues));

            Mapper.CreateMap<CourseMetadataFieldDescriptor, QuestionMetaField>()
                .ForMember(dest => dest.FriendlyName, opt => opt.MapFrom(src => src.FriendlyName))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.TypeDescriptor,
                    opt => opt.MapFrom(src => Mapper.Map<MetaFieldTypeDescriptor>(src)));

            Mapper.CreateMap<CourseMetadataFieldDescriptor, MetaFieldTypeDescriptor>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.AvailableChoice,
                    opt => opt.MapFrom(src => src.CourseMetadataFieldValues.Select(i => new AvailableChoiceItem(i.Text)).ToList()));

            Mapper.CreateMap<Bfw.Agilix.DataContracts.Question, Question>()
                .ForMember(dto => dto.Id, opt => opt.MapFrom(q => q.Id))
                .ForMember(dto => dto.Status, opt => opt.MapFrom(q => q.QuestionStatus))
                .ForMember(dto => dto.DefaultSection, opt => opt.MapFrom(q => modelProfileService.GetQuestionDefaultValues(q)))
                .ForMember(dto => dto.ProductCourseSections, opt => opt.MapFrom(q => modelProfileService.GetProductCourseSections(q)))
                .ForMember(dto => dto.Version, opt => opt.MapFrom(q => modelProfileService.GetNumericVersion(q.QuestionVersion)))
                .ForMember(dto => dto.Preview, opt => opt.MapFrom(q => CustomQuestionHelper.GetQuestionHtmlPreview(q)))
                .ForMember(dto => dto.ModifiedBy, opt => opt.MapFrom(q => modelProfileService.GetModifiedBy(q)))
                .ForMember(dto => dto.DuplicateFromShared, opt => opt.MapFrom(q => modelProfileService.GetDuplicateFromShared(q)))
                .ForMember(dto => dto.DuplicateFrom, opt => opt.MapFrom(q => modelProfileService.GetDuplicateFrom(q)))
                .ForMember(dto => dto.DraftFrom, opt => opt.MapFrom(q => modelProfileService.GetDraftFrom(q)))
                .ForMember(dto => dto.RestoredFromVersion, opt => opt.MapFrom(q => modelProfileService.GetRestoredFromVersion(q)))
                .ForMember(dto => dto.IsPublishedFromDraft, opt => opt.MapFrom(q => modelProfileService.GetPublishedFromDraft(q)));

            Mapper.CreateMap<Bfw.Agilix.DataContracts.QuestionChoice, QuestionChoice>();

            Mapper.CreateMap<Question, Bfw.Agilix.DataContracts.Question>()
               .ForMember(dto => dto.Id, opt => opt.MapFrom(q => q.Id))
               .ForMember(dto => dto.QuestionStatus, opt => opt.MapFrom(q => q.Status))
               .ForMember(dto => dto.MetadataElements, opt => opt.MapFrom(q => modelProfileService.GetXmlMetadataElements(q)))
               .ForMember(dto => dto.Body, opt => opt.Condition(cont => cont.SourceValue != null))
               .ForMember(dto => dto.Answer, opt => opt.Condition(cont => cont.SourceValue != null))
               .ForMember(dto => dto.AnswerList, opt => opt.Condition(cont => cont.SourceValue != null))
               .ForMember(dto => dto.Choices, opt => opt.Condition(cont => cont.SourceValue != null))
               .ForMember(dto => dto.InteractionData, opt => opt.Condition(q => q.CustomUrl == QuestionTypeHelper.GraphType))
               .ForMember(dto => dto.InteractionType, opt => opt.Condition(cont => cont.SourceValue != null))
               .ForMember(dto => dto.CustomUrl, opt => opt.Condition(cont => cont.SourceValue != null))
               .ForMember(dto => dto.QuestionVersion, opt => opt.MapFrom(q => q.Version))
               .ForMember(dto => dto.ModifiedDate, opt =>opt.Ignore());

            Mapper.CreateMap<QuestionChoice, Bfw.Agilix.DataContracts.QuestionChoice>();

            Mapper.CreateMap<Question, QuestionMetadata>().ConvertUsing(new QuestionToQuestionMetadataConverter(modelProfileService));

            Mapper.CreateMap<AgilixUser, UserInfo>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.FirstName, opt => opt.MapFrom(s => s.FirstName))
                .ForMember(d => d.LastName, opt => opt.MapFrom(s => s.LastName))
                .ForMember(d => d.Username, opt => opt.MapFrom(s => s.Credentials != null ? s.Credentials.Username : s.UserName))
                .ForMember(d => d.Password, opt => opt.MapFrom(s => s.Credentials != null ? s.Credentials.Password : ""))
                .ForMember(d => d.ReferenceId, opt => opt.MapFrom(s => s.Reference))
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.DomainId, opt => opt.MapFrom(s => s.Domain != null ? s.Domain.Id : ""))
                .ForMember(d => d.DomainName, opt => opt.MapFrom(s => s.Domain != null ? s.Domain.Name : ""))
                .ForMember(d => d.LastLogin, opt => opt.MapFrom(s => s.LastLogin));

            Mapper.CreateMap<Course, TitleViewModel>()
                .ForMember(vm => vm.Id, opt => opt.MapFrom(c => c.ProductCourseId))
                .ForMember(vm => vm.Title, opt => opt.MapFrom(c => c.Title))
                .ForMember(vm => vm.Chapters, opt => opt.MapFrom(c => modelProfileService.GetChaptersViewModel(c)));

            Mapper.CreateMap<CourseMetadataFieldValue, ChapterViewModel>()
                .ForMember(vm => vm.Id, opt => opt.MapFrom(c => c.Text))
                .ForMember(vm => vm.Title, opt => opt.MapFrom(c => c.Text));

            Mapper.CreateMap<Question, QuestionViewModel>()
                .ForMember(dest => dest.ProductCourses, opt => opt.MapFrom(src => modelProfileService.GetTitleNames(src.ProductCourseSections.Select(p => p.ProductCourseId))))
                .ForMember(dest => dest.DefaultSection, opt => opt.MapFrom(src => modelProfileService.GetDefaultSectionForViewModel(src)))
                .ForMember(dest => dest.LocalSection, opt => opt.MapFrom(src => src.ProductCourseSections))
                .ForMember(dest => dest.SharedQuestionDuplicateFrom, opt => opt.MapFrom(src => src));

            Mapper.CreateMap<List<QuestionMetadataSection>, QuestionMetadataSection>().ConvertUsing(new ProductSectionToLocalValuesConverter());

            Mapper.CreateMap<Question, SharedQuestionDuplicateFromViewModel>()
                .ConvertUsing(new ProductSectionToSharedQuestionDuplicateConverter(modelProfileService));
                
            Mapper.CreateMap<QuestionViewModel, Question>()
                 .ForMember(dest => dest.ProductCourseSections, opt => opt.MapFrom(src => modelProfileService.GetProductCourseSections(src)))
                .ForMember(dest => dest.CustomUrl, opt => opt.MapFrom(src => src.QuestionType));

            Mapper.CreateMap<Course, ProductCourseViewModel>()
                .ForMember(vm => vm.Id, opt => opt.MapFrom(c => c.ProductCourseId))
                .ForMember(vm => vm.Title, opt => opt.MapFrom(c => c.Title));

            Mapper.CreateMap<IEnumerable<Question>, QuestionHistoryViewModel>()
                .ForMember(dest => dest.Versions, opt => opt.MapFrom(src =>src));

            Mapper.CreateMap<Question, QuestionVersionViewModel>()
                .ForMember(dest => dest.Version, opt => opt.MapFrom(src => src.Version.ToString()))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => modelProfileService.GetModifierName(src.ModifiedBy)))
                .ForMember(dest => dest.DuplicateFrom, opt => opt.MapFrom(src => modelProfileService.GetDuplicateFromQuestion(src.EntityId, src.DuplicateFrom)))
                .ForMember(dest => dest.RestoredFromVersion, opt => opt.MapFrom(src => modelProfileService.GetQuestionVersion(src.EntityId, src.Id, src.RestoredFromVersion)))
                .ForMember(dest => dest.QuestionPreview, opt => opt.MapFrom(src => src.Preview)); 

            Mapper.CreateMap<Question, DuplicateFromViewModel>().ConvertUsing(new QuestionToDuplicateFromConverter());

            Mapper.CreateMap<Question, RestoredFromVersionViewModel>()
                .ForMember(dest => dest.Version, opt => opt.MapFrom(src => src.Version.ToString()))
                .ForMember(dest => dest.VersionAuthor, opt => opt.MapFrom(src => modelProfileService.GetModifierName(src.ModifiedBy)))
                .ForMember(dest => dest.VersionDate, opt => opt.MapFrom(src => src.ModifiedDate));



            Mapper.CreateMap<Course, MetadataConfigViewModel>()
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.ProductCourseId))
                .ForMember(dest => dest.Banks, opt => opt.MapFrom(src => modelProfileService.GetCourseBanks(src)))
                .ForMember(dest => dest.Chapters, opt => opt.MapFrom(src => modelProfileService.GetCourseChapters(src)))
                .ForMember(dest => dest.Fields, opt => opt.MapFrom(src => src.FieldDescriptors.Where(f => f.Name != MetadataFieldNames.Bank && f.Name != MetadataFieldNames.Chapter)));

            Mapper.CreateMap<CourseMetadataFieldDescriptor, ProductCourseSpecificMetadataFieldViewModel>()
                .ForMember(dest => dest.InternalName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.FriendlyName))
                .ForMember(dest => dest.FieldType, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.ValuesOptions, opt => opt.MapFrom(src => modelProfileService.GetMetadataFieldValues(src)))
                .ForMember(dest => dest.DisplayOptions, opt => opt.MapFrom(src => src));

            Mapper.CreateMap<CourseMetadataFieldDescriptor, MetadataFieldDisplayOptionsViewModel>();

            Mapper.CreateMap<MetadataConfigViewModel, Course>()
                .ForMember(dest => dest.ProductCourseId, opt => opt.MapFrom(src => src.CourseId))
                .ForMember(dest => dest.FieldDescriptors, opt => opt.MapFrom(src => modelProfileService.GetCourseFieldDescriptors(src)));

            Mapper.CreateMap<CourseMetadataFieldValue, QuestionCardDataValue>();

            Mapper.CreateMap<Role, RoleViewModel>()
                .ForMember(dest => dest.ActiveCapabiltiesCount, opt => opt.MapFrom(src => src.CapabilitiesCount))
                .ForMember(dest => dest.CapabilityGroups, opt => opt.MapFrom(src => modelProfileService.GetCapabilityGroups(src.Capabilities)));

            Mapper.CreateMap<RoleViewModel, Role>()
                .ForMember(dest => dest.CanEdit, opt => opt.UseValue(true))
                .ForMember(dest => dest.Capabilities, opt => opt.MapFrom(src => modelProfileService.GetActiveRoleCapabilities(src)));

            Mapper.CreateMap<QBAUser, UserViewModel>()
               .ForMember(dest => dest.ProductCoursesCount, opt => opt.MapFrom(src => src.ProductCoursesCount));
        }
    }

    public class QuestionToQuestionMetadataConverter : ITypeConverter<Question, QuestionMetadata>
    {
        private readonly IModelProfileService modelProfileService;

        public QuestionToQuestionMetadataConverter(IModelProfileService modelProfileService)
        {
            this.modelProfileService = modelProfileService;
        }
        public QuestionMetadata Convert(ResolutionContext context)
        {
            if (context.Options.Items.Any())
            {
                return modelProfileService.GetQuestionMetadataForCourse((Question)context.SourceValue,
                    (Course)context.Options.Items.First().Value);
            }
            return modelProfileService.GetQuestionMetadataForCourse((Question)context.SourceValue);
        }
    }

    public class QuestionToDuplicateFromConverter : ITypeConverter<Question, DuplicateFromViewModel>
    {

        public DuplicateFromViewModel Convert(ResolutionContext context)
        {
            var model = new DuplicateFromViewModel();
            if (context.Options.Items.Any())
            {
                var productCourseId = (string) context.Options.Items.First().Value;
                var question = (Question) context.SourceValue;
                if (question != null)
                {
                    model.Id = question.Id;
                    var section =
                        question.ProductCourseSections.FirstOrDefault(s => s.ProductCourseId == productCourseId);
                    if (section == null)
                    {
                        section = question.DefaultSection;
                    }
                    model.Bank = section.Bank;
                    model.Chapter = section.Chapter;
                    model.Title = section.Title;
                }
            }
            return model;
        }
    }

    public class ProductSectionToLocalValuesConverter : ITypeConverter<List<QuestionMetadataSection>, QuestionMetadataSection>
    {
        public QuestionMetadataSection Convert(ResolutionContext context)
        {
            var section = new QuestionMetadataSection();
            if (context.Options.Items.Any())
            {
                var course = (Course) context.Options.Items.First().Value;
                var productCourseId = course.ProductCourseId;
                var dynamicFields =
                    course.FieldDescriptors.Where(f => !MetadataFieldNames.GetStaticFieldNames().Contains(f.Name));
                section = ((List<QuestionMetadataSection>)context.SourceValue).FirstOrDefault(s => s.ProductCourseId == productCourseId);
                if (section != null)
                {
                    foreach (var courseMetadataFieldDescriptor in dynamicFields.Where(courseMetadataFieldDescriptor => !section.DynamicValues.ContainsKey(courseMetadataFieldDescriptor.Name)))
                    {
                        section.DynamicValues.Add(courseMetadataFieldDescriptor.Name, new List<string>());
                    }
                }
            }
            return section;
        }
    }

    public class ProductSectionToSharedQuestionDuplicateConverter : ITypeConverter<Question, SharedQuestionDuplicateFromViewModel>
    {
        private readonly IModelProfileService modelProfileService;
        public ProductSectionToSharedQuestionDuplicateConverter(IModelProfileService modelProfileService)
        {
            this.modelProfileService = modelProfileService;
        }
        public SharedQuestionDuplicateFromViewModel Convert(ResolutionContext context)
        {
            if (context.Options.Items.Any())
            {
                var course = (Course)context.Options.Items.First().Value;
                var question = ((Question)context.SourceValue);
                return modelProfileService.GetSourceQuestionSharedFrom(question.DuplicateFromShared, course);
            }
            return null;
        }
    }
}
