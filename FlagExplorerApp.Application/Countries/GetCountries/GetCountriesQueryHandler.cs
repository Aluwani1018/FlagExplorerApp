using AutoMapper;
using FlagExplorerApp.Application.Country;
using FlagExplorerApp.Domain.Repositories;
using MediatR;

namespace FlagExplorerApp.Application.Countries.GetCountries;

public class GetCountriesQueryHandler : IRequestHandler<GetCountriesQuery, List<CountryDto>>
{
    private readonly ICountryRepository _countryRepository;
    private readonly IMapper _mapper;

    public GetCountriesQueryHandler(ICountryRepository countryRepository, IMapper mapper)
    {
        _countryRepository = countryRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the GetCountriesQuery request.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<CountryDto>> Handle(GetCountriesQuery request, CancellationToken cancellationToken)
    {
        // Retrieve all countries from the repository
        var countries = await _countryRepository.FindAllAsync(cancellationToken);

        // Map the Country entities to CountryDto objects
        var countryDtos = _mapper.Map<List<CountryDto>>(countries);

        return countryDtos;
    }
}
