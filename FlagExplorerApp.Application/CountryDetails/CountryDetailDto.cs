
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
            profile.CreateMap<DomainCountryDetail, CountryDetailDto>();
        }
    }
}
