using FlagExplorerApp.Domain.Entities;
using FlagExplorerApp.Domain.Repositories.Documents;

namespace FlagExplorerApp.Domain.Repositories;

public interface ICountryDetailRepository : ICosmosDBRepository<CountryDetail, ICountryDetailDocument>
{
    Task<CountryDetail?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
}
