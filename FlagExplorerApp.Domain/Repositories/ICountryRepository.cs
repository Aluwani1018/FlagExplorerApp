using FlagExplorerApp.Domain.Entities;
using FlagExplorerApp.Domain.Repositories.Documents;

namespace FlagExplorerApp.Domain.Repositories;

public interface ICountryRepository : ICosmosDBRepository<Country , ICountryDocument>
{
}
