
using AutoMapper;
using FlagExplorerApp.Application.Common.Mappings;
using DomainCountryDetail = FlagExplorerApp.Domain.Entities.CountryDetail;

namespace FlagExplorerApp.Application.CountryDetail
{
    public record class CountryDetailDto : IMapFrom<DomainCountryDetail>
    {
        public required string Name { get; set; }
        public int Population { get; set; }
        public string? Capital { get; set; }
        public string? Flag { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<DomainCountryDetail, CountryDetailDto>()
                    .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Name))
                    .ForMember(d => d.Population, opt => opt.MapFrom(src => src.Population))
                      .ForMember(d => d.Capital, opt => opt.MapFrom(src => src.Capital))
                    .ForMember(d => d.Flag, opt => opt.MapFrom(src => src.Flag));
        }
    }
}
