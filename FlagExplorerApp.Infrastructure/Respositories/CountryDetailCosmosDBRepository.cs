using FlagExplorerApp.Domain.Entities;
using FlagExplorerApp.Domain.Repositories.Documents;
using FlagExplorerApp.Domain.Repositories;
using FlagExplorerApp.Infrastructure.Persistance.Documents;
using FlagExplorerApp.Infrastructure.Persistance;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Options;

namespace FlagExplorerApp.Infrastructure.Respositories;

internal class CountryDetailCosmosDBRepository : CosmosDBRepositoryBase<CountryDetail, CountryDetailDocument, ICountryDetailDocument>, ICountryDetailRepository
{
    public CountryDetailCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
        Microsoft.Azure.CosmosRepository.IRepository<CountryDetailDocument> cosmosRepository,
        ICosmosContainerProvider<CountryDetailDocument> containerProvider,
        IOptionsMonitor<RepositoryOptions> optionsMonitor) : base(unitOfWork, cosmosRepository, "name", containerProvider, optionsMonitor)
    {
    }

    public async Task<CountryDetail?> FindByNameAsync(string name, CancellationToken cancellationToken = default) => await base.FindByIdAsync(id: name, cancellationToken: cancellationToken);
}