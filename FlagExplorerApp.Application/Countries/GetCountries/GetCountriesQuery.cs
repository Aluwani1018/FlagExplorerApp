using FlagExplorerApp.Application.Common.Interfaces;
using FlagExplorerApp.Application.Country;
using MediatR;

namespace FlagExplorerApp.Application.Countries.GetCountries;

/// <summary>
/// Query to get a list of countries.
/// </summary>
public class GetCountriesQuery : IRequest<List<CountryDto>>, IQuery
{
    public GetCountriesQuery()
    {
        
    }
}
