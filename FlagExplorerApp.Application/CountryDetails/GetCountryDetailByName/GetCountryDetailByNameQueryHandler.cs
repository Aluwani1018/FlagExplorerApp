using AutoMapper;
using FlagExplorerApp.Application.CountryDetail;
using FlagExplorerApp.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace FlagExplorerApp.Application.CountryDetails.GetCountryDetailByName;

public class GetCountryDetailByNameQueryHandler : IRequestHandler<GetCountryDetailByNameQuery, CountryDetailDto>
{
    private readonly ICountryDetailRepository _countryDetailRepository;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;

    public GetCountryDetailByNameQueryHandler(IMapper mapper, ICountryDetailRepository countryDetailRepository, IMemoryCache memoryCache)
    {
        _mapper = mapper;
        _countryDetailRepository = countryDetailRepository;
        _memoryCache = memoryCache;
    }

    /// <summary>
    /// Handles the GetCountryDetailByNameQuery request by retrieving the country details
    /// from the repository or cache and mapping them to a CountryDetailDto.
    /// </summary>
    /// <param name="request">The query containing the name of the country to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the mapped CountryDetailDto.</returns>
    public async Task<CountryDetailDto> Handle(GetCountryDetailByNameQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"CountryDetail_{request.Name}";

        //Added memory cache to improve performance, however would have use a distributed cache with redis, had issue with containers in my machine
        if (!_memoryCache.TryGetValue(cacheKey, out CountryDetailDto? cachedCountryDetail))
        {
            var countryDetail = await _countryDetailRepository.FindByNameAsync(request.Name, cancellationToken);

            if (countryDetail == null)
            {
                throw new KeyNotFoundException($"Country with name '{request.Name}' was not found.");
            }

            cachedCountryDetail = _mapper.Map<CountryDetailDto>(countryDetail);

            // Set cache options and store the result in the cache
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(5)
            };

            _memoryCache.Set(cacheKey, cachedCountryDetail, cacheOptions);
        }

        return cachedCountryDetail;
    }
}
