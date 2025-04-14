using AutoMapper;
using FlagExplorerApp.Application.CountryDetail;
using FlagExplorerApp.Domain.Repositories;
using MediatR;

namespace FlagExplorerApp.Application.CountryDetails.GetCountryDetailByName;

public class GetCountryDetailByNameQueryHandler : IRequestHandler<GetCountryDetailByNameQuery, CountryDetailDto>
{
    private readonly ICountryDetailRepository _countryDetailRepository;
    private readonly IMapper _mapper;

    public GetCountryDetailByNameQueryHandler(IMapper mapper, ICountryDetailRepository countryDetailRepository)
    {
           _mapper = mapper;
           _countryDetailRepository = countryDetailRepository;
    }

    /// <summary>
    /// Handles the GetCountryDetailByNameQuery request by retrieving the country details
    /// from the repository and mapping them to a CountryDetailDto.
    /// </summary>
    /// <param name="request">The query containing the name of the country to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the mapped CountryDetailDto.</returns>
    public async Task<CountryDetailDto> Handle(GetCountryDetailByNameQuery request, CancellationToken cancellationToken)
    {
        var countryDetail = await _countryDetailRepository.FindByNameAsync(request.Name, cancellationToken);

        if (countryDetail == null)
        {
            throw new KeyNotFoundException($"Country with name '{request.Name}' was not found.");
        }

        return _mapper.Map<CountryDetailDto>(countryDetail);
    }
}
