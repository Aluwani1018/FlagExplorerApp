using AutoMapper;
using FlagExplorerApp.Application.Country;
using FlagExplorerApp.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

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
    ///  Handles the GetCountriesQuery request by retrieving a list of countries from the repository
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<CountryDto>> Handle(GetCountriesQuery request, CancellationToken cancellationToken)
    {
        // Check for cancellation before executing operations
        cancellationToken.ThrowIfCancellationRequested();

        var countries = await _countryRepository.FindAllAsync(cancellationToken);

        // Validate the result to prevent unexpected null values
        if (countries == null || !countries.Any())
        {
            return new List<CountryDto>();
        }

        // Map the Country entities to CountryDto objects
        var countryDtos = _mapper.Map<List<CountryDto>>(countries);

        return countryDtos;
    }
}
