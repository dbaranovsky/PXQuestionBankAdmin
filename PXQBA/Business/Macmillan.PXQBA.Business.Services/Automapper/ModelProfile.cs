using System.Data.Entity.ModelConfiguration.Conventions;
using AutoMapper;
using Bfw.Agilix.DataContracts;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;
using Course = Macmillan.PXQBA.Business.Models.Course;
using Question = Macmillan.PXQBA.Business.Models.Question;

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
                .ForMember(dest => dest.ProductCourseId, opt => opt.MapFrom(q => q.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(q => q.Title))
                .ForMember(dest => dest.LearningObjectives, opt => opt.Ignore())
                .ForMember(dest => dest.QuestionCardLayout, opt => opt.Ignore());

            Mapper.CreateMap<Bfw.Agilix.DataContracts.Question, Question>()
                .ForMember(dto => dto.Id, opt => opt.MapFrom(q => q.Id))
                //.ForMember(dto => dto.EntityId, opt => opt.MapFrom(q => q.EntityId))
                .ForMember(dto => dto.Title, opt => opt.MapFrom(q => q.Title))
                .ForMember(dto => dto.Chapter, opt => opt.MapFrom(q => q.eBookChapter))
                .ForMember(dto => dto.Bank, opt => opt.MapFrom(q => q.QuestionBank))
                .ForMember(dto => dto.Sequence, opt => opt.Ignore())
                .ForMember(dto => dto.Type, opt => opt.Ignore())
                .ForMember(dto => dto.Preview, opt => opt.MapFrom( q => QuestionPreviewHelper.GetQuestionHtmlPreview(q)));

            Mapper.CreateMap<Question, Bfw.Agilix.DataContracts.Question>()
                .ForMember(dto => dto.Id, opt => opt.MapFrom(q => q.Id))
                //.ForMember(dto => dto.EntityId, opt => opt.MapFrom(q => q.EntityId))
                .ForMember(dto => dto.Title, opt => opt.MapFrom(q => q.Title))
                .ForMember(dto => dto.eBookChapter, opt => opt.MapFrom(q => q.Chapter))
                .ForMember(dto => dto.QuestionBank, opt => opt.MapFrom(q => q.Bank));

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

            Mapper.CreateMap<string, InteractionType>().ConvertUsing(modelProfileService.CreateInteractionType);


            #region UI models to dummy models

            Mapper.CreateMap<DataAccess.Data.ProductCourse, Question>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Question.DlapId))
                    .ForMember(dest => dest.Keywords, opt => opt.MapFrom(src => src.Keywords.Split('|')))
                    .ForMember(dest => dest.SuggestedUse, opt => opt.MapFrom(src => src.SuggestedUse.Split('|')))
                    .ForMember(dest => dest.LearningObjectives, opt => opt.MapFrom(src => modelProfileService.GetLOByGuid(src.ProductCourseDlapId, src.LearningObjectives)))
                    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Question.Type))
                    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Question.Status))
                    .ForMember(dest => dest.Preview, opt => opt.MapFrom(src => src.Question.Preview));

            Mapper.CreateMap<Question, DataAccess.Data.ProductCourse>()
                  .ForMember(dest => dest.Id, opt => opt.Ignore())
                  .ForMember(dest => dest.Keywords, opt => opt.MapFrom(src => src.Keywords != null ? string.Join("|", src.Keywords) : null))
                  .ForMember(dest => dest.SuggestedUse, opt => opt.MapFrom(src => src.SuggestedUse != null ? string.Join("|", src.SuggestedUse) : null))
                  .ForMember(dest => dest.LearningObjectives, opt => opt.MapFrom(src => modelProfileService.SetLearningObjectives(src.LearningObjectives)));

            Mapper.CreateMap<Question, DataAccess.Data.Question>()
                  .ForMember(dest => dest.DlapId, opt => opt.MapFrom(src => src.Id))
                  .ForMember(dest => dest.Id, opt => opt.Ignore());

            Mapper.CreateMap<Question, QuestionMetadata>()
             .ForMember(dest => dest.Data, opt => opt.MapFrom(src => modelProfileService.CreateQuestionMetadata(src)));

            Mapper.CreateMap<Note, DataAccess.Data.Note>()
                  .ForMember(dest => dest.QuestionId, opt => opt.Ignore());
            Mapper.CreateMap<DataAccess.Data.Note, Note>()
                .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.Question.DlapId));

            Mapper.CreateMap<Question, QuestionViewModel>();

            #endregion
        }
    }
}
