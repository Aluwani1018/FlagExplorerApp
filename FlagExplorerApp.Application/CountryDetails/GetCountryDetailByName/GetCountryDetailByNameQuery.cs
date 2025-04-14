using FlagExplorerApp.Application.Common.Interfaces;
using FlagExplorerApp.Application.CountryDetail;
using MediatR;

namespace FlagExplorerApp.Application.CountryDetails.GetCountryDetailByName;

public class GetCountryDetailByNameQuery : IRequest<CountryDetailDto>, IQuery
{
    public GetCountryDetailByNameQuery(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
}
