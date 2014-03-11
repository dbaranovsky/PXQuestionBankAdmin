using AutoMapper;
using Bfw.Agilix.DataContracts;
using Question = Macmillan.PXQBA.Business.Models.Question;

namespace Macmillan.PXQBA.Business.Automapper
{
    public class ModelProfile : Profile
    {

        protected override void Configure()
        {
            Mapper.CreateMap<Bfw.Agilix.DataContracts.Question, Question>()
                .ForMember(dto => dto.title, opt => opt.MapFrom(q => q.Title))
                .ForMember(dto => dto.eBookChapter, opt => opt.MapFrom(q => q.eBookChapter))
                .ForMember(dto => dto.questionBank, opt => opt.MapFrom(q => q.QuestionBank))
                .ForMember(dto => dto.questionSeq, opt => opt.Ignore())
                .ForMember(dto => dto.questionType, opt => opt.Ignore());
        }
    }
}
