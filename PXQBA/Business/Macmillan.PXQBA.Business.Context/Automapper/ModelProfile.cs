using AutoMapper;
using Bfw.Agilix.DataContracts;
using Macmillan.PXQBA.Common.Helpers;
using Question = Macmillan.PXQBA.Business.Models.Question;

namespace Macmillan.PXQBA.Business.Automapper
{
    public class ModelProfile : Profile
    {

        protected override void Configure()
        {
            Mapper.CreateMap<Bfw.Agilix.DataContracts.Question, Question>()
                .ForMember(dto => dto.Title, opt => opt.MapFrom(q => q.Title))
                .ForMember(dto => dto.EBookChapter, opt => opt.MapFrom(q => q.eBookChapter))
                .ForMember(dto => dto.QuestionBank, opt => opt.MapFrom(q => q.QuestionBank))
                .ForMember(dto => dto.QuestionSeq, opt => opt.Ignore())
                .ForMember(dto => dto.QuestionType, opt => opt.Ignore())
                .ForMember(dto => dto.QuestionHtmlInlinePreview, opt => opt.MapFrom( q => QuestionPreviewHelper.GetQuestionHtmlPreview(q)));
        }
    }
}
