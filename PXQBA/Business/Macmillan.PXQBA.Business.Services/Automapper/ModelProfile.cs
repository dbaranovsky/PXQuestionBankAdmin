﻿using System.Data.Entity.ModelConfiguration.Conventions;
using AutoMapper;
using Bfw.Agilix.DataContracts;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Business.Models.Web;
using Macmillan.PXQBA.Common.Helpers;
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
            Mapper.CreateMap<Bfw.Agilix.DataContracts.Question, Question>()
                .ForMember(dto => dto.Id, opt => opt.MapFrom(q => q.Id))
                .ForMember(dto => dto.EntityId, opt => opt.MapFrom(q => q.EntityId))
                .ForMember(dto => dto.Title, opt => opt.MapFrom(q => q.Title))
                .ForMember(dto => dto.EBookChapter, opt => opt.MapFrom(q => q.eBookChapter))
                .ForMember(dto => dto.QuestionBank, opt => opt.MapFrom(q => q.QuestionBank))
                .ForMember(dto => dto.QuestionSeq, opt => opt.Ignore())
                .ForMember(dto => dto.QuestionType, opt => opt.Ignore())
                .ForMember(dto => dto.QuestionHtmlInlinePreview, opt => opt.MapFrom( q => QuestionPreviewHelper.GetQuestionHtmlPreview(q)));

            Mapper.CreateMap<Question, Bfw.Agilix.DataContracts.Question>()
                .ForMember(dto => dto.Id, opt => opt.MapFrom(q => q.Id))
                .ForMember(dto => dto.EntityId, opt => opt.MapFrom(q => q.EntityId))
                .ForMember(dto => dto.Title, opt => opt.MapFrom(q => q.Title))
                .ForMember(dto => dto.eBookChapter, opt => opt.MapFrom(q => q.EBookChapter))
                .ForMember(dto => dto.QuestionBank, opt => opt.MapFrom(q => q.QuestionBank));

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

            Mapper.CreateMap<Item, ContentItem>();

            Mapper.CreateMap<string, InteractionType>().ConvertUsing(modelProfileService.CreateInteractionType);


            #region UI models to dummy models

            Mapper.CreateMap<Question, DataAccess.Data.Question>()
                .ForMember(dest => dest.Id, d => d.Ignore())
                .ForMember(dest => dest.DlapId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.QuestionType))
                .ForMember(dest => dest.Preview, opt => opt.MapFrom(src => src.QuestionHtmlInlinePreview))
                .ForMember(dest => dest.InteractionType, opt => opt.MapFrom(src => ((int) src.InteractionType).ToString()));

            Mapper.CreateMap<DataAccess.Data.Question, Question>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DlapId))
               .ForMember(dest => dest.QuestionType, opt => opt.MapFrom(src => src.Type))
               .ForMember(dest => dest.QuestionHtmlInlinePreview, opt => opt.MapFrom(src => src.Preview))
               .ForMember(dest => dest.InteractionType, opt => opt.MapFrom(src => (InteractionType)int.Parse(src.InteractionType)));

            Mapper.CreateMap<DataAccess.Data.Question, QuestionMetadata>()
             .ForMember(dest => dest.Data, opt => opt.MapFrom(src => modelProfileService.CreateQuestionMetadata(src)));

            #endregion
        }
    }
}