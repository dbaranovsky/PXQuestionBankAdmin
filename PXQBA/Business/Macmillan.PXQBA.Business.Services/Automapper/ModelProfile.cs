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
using Macmillan.PXQBA.Web.ViewModels.TiteList;
using Course = Macmillan.PXQBA.Business.Models.Course;
using LearningObjective = Macmillan.PXQBA.Business.Models.LearningObjective;
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
                .ForMember(dest => dest.LearningObjectives, opt => opt.MapFrom(src => src.LearningObjectives))
                .ForMember(dest => dest.QuestionCardLayout,
                    opt => opt.MapFrom(src => modelProfileService.GetQuestionCardLayout(src)))
                .ForMember(dest => dest.FieldDescriptors,
                    opt => opt.MapFrom(src => modelProfileService.GetCourseMetadataFieldDescriptors(src)))
                .ForMember(dest => dest.QuestionRepositoryCourseId, 
                    opt => opt.MapFrom(src => modelProfileService.GetQuestionBankRepositoryCourse(src)));


            Mapper.CreateMap<CourseMetadataFieldDescriptor, QuestionMetaField>()
                .ForMember(dest => dest.FriendlyName, opt => opt.MapFrom(src => src.Friendlyname))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.TypeDescriptor,
                    opt => opt.MapFrom(src => Mapper.Map<MetaFieldTypeDescriptor>(src)));

            Mapper.CreateMap<CourseMetadataFieldDescriptor, MetaFieldTypeDescriptor>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.AvailableChoice,
                    opt => opt.MapFrom(src => src.CourseMetadataFieldValues.Select(i => new AvailableChoiceItem(i.Text)).ToList()));
 

            Mapper.CreateMap<Bfw.Agilix.DataContracts.LearningObjective, LearningObjective>()
                .ForMember(dest => dest.Guid, opt => opt.MapFrom(src => src.Guid))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Title));
           
            Mapper.CreateMap<Bfw.Agilix.DataContracts.Question, Question>()
               .ForMember(dto => dto.Id, opt => opt.MapFrom(q => q.Id))
               .ForMember(dto => dto.Status, opt => opt.MapFrom(q => q.QuestionStatus))
               .ForMember(dto => dto.DefaultValues, opt => opt.MapFrom(q => modelProfileService.GetQuestionDefaultValues(q)))
               .ForMember(dto => dto.ProductCourseSections, opt => opt.MapFrom(q => modelProfileService.GetProductCourseSections(q)))
               .ForMember(dto => dto.Preview, opt => opt.MapFrom(q => CustomQuestionHelper.GetQuestionHtmlPreview(q)));

            Mapper.CreateMap<Bfw.Agilix.DataContracts.QuestionChoice, QuestionChoice>();

            Mapper.CreateMap<Question, Bfw.Agilix.DataContracts.Question>()
               .ForMember(dto => dto.Id, opt => opt.MapFrom(q => q.Id))
               .ForMember(dto => dto.QuestionStatus, opt => opt.MapFrom(q => q.Status))
               .ForMember(dto => dto.MetadataElements, opt => opt.MapFrom(q => modelProfileService.GetXmlMetadataElements(q)))
               .ForMember(dto => dto.Body, opt => opt.Condition(cont => cont.DestinationValue == null))
               .ForMember(dto => dto.Answer, opt => opt.Condition(cont => cont.DestinationValue == null))
               .ForMember(dto => dto.AnswerList, opt => opt.Condition(cont => cont.DestinationValue == null))
               .ForMember(dto => dto.Choices, opt => opt.Condition(cont => cont.DestinationValue == null))
               .ForMember(dto => dto.InteractionData, opt => opt.MapFrom(src => src.InteractionData))
               .ForMember(dto => dto.InteractionType, opt =>opt.Condition(cont => cont.DestinationValue == null))
               .ForMember(dto => dto.CustomUrl, opt =>opt.Condition(cont => cont.DestinationValue == null));

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
                .ForMember(dest => dest.LocalValues, opt => opt.MapFrom(src => src.ProductCourseSections))
                .ForMember(dest => dest.SharedQuestionDuplicateFrom, opt => opt.MapFrom(src => src.ProductCourseSections));

            Mapper.CreateMap<List<ProductCourseSection>, Dictionary<string, List<string>>>().ConvertUsing(new ProductSectionToLocalValuesConverter());

            Mapper.CreateMap<List<ProductCourseSection>, SharedQuestionDuplicateFromViewModel>()
                .ConvertUsing(new ProductSectionToSharedQuestionDuplicateConverter(modelProfileService));
                
            Mapper.CreateMap<QuestionViewModel, Question>()
                .ForMember(dest => dest.ProductCourseSections, opt => opt.MapFrom(src => modelProfileService.GetProductCourseSections(src)));
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

    public class ProductSectionToLocalValuesConverter : ITypeConverter<List<ProductCourseSection>, Dictionary<string, List<string>>>
    {
        public Dictionary<string, List<string>> Convert(ResolutionContext context)
        {
            var values = new Dictionary<string, List<string>>();
            if (context.Options.Items.Any())
            {
                var course = (Course) context.Options.Items.First().Value;
                var productCourseId = course.ProductCourseId;
                var section = ((List<ProductCourseSection>)context.SourceValue).FirstOrDefault(s => s.ProductCourseId == productCourseId);
                if (section != null)
                {
                    values = section.ProductCourseValues;
                }
                foreach (var courseMetadataFieldDescriptor in course.FieldDescriptors.Where(courseMetadataFieldDescriptor => !values.ContainsKey(courseMetadataFieldDescriptor.Name)))
                {
                    values.Add(courseMetadataFieldDescriptor.Name, new List<string>());
                }
            }
            return values;
        }
    }

    public class ProductSectionToSharedQuestionDuplicateConverter : ITypeConverter<List<ProductCourseSection>, SharedQuestionDuplicateFromViewModel>
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
                var productCourseId = course.ProductCourseId;
                var section = ((List<ProductCourseSection>)context.SourceValue).FirstOrDefault(s => s.ProductCourseId == productCourseId);
                return modelProfileService.GetSourceQuestionSharedWith(section, course);
            }
            return null;
        }
    }


    //public class ValueResolver : IValueResolver
    //{
    //    public ResolutionResult Resolve(ResolutionResult source)
    //    {
    //        return source;
    //    }
    //}

    //public class ValueResolver1 : ValueResolver <Question, QuestionMetadata>
    //{
    //    protected override QuestionMetadata ResolveCore(Question source)
    //    {
            
    //        throw new NotImplementedException();
    //    }
    //}
}
