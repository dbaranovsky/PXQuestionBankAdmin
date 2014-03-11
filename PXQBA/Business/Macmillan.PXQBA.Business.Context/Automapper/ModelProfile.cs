using AutoMapper;

namespace Macmillan.PXQBA.Business.Automapper
{
    public class ModelProfile : Profile
    {

        protected override void Configure()
        {
            //Mapper.CreateMap<Model, Ticket>()
            //    .ForMember(t => t.Id, opt => opt.Ignore())
            //    .ForMember(t => t.Status, opt => opt.MapFrom(vm => vm.Status))
            //    .ForMember(t => t.ClientTimeZone, opt => opt.Ignore());
        }
    }
}
