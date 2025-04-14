using AutoMapper;
using FlagExplorerApp.Application.Common.Mappings;
using DomainCountry = FlagExplorerApp.Domain.Entities.Country;

namespace FlagExplorerApp.Application.Country;

public record class CountryDto : IMapFrom<DomainCountry>
{
    public required string Name { get; set; }
    public string? Flag { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<DomainCountry, CountryDto>()
                   .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Name))
                   .ForMember(d => d.Flag, opt => opt.MapFrom(src => src.Flag));
    }
}
