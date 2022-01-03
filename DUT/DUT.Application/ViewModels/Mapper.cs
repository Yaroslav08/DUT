using AutoMapper;
using DUT.Application.ViewModels.Group;

namespace DUT.Application.ViewModels
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Domain.Models.Group, GroupViewModel>()
                .ForMember(x => x.Name, s => s.MapFrom(x => $"{x.Name} ({x.StartStudy.Year})"));
        }
    }
}
