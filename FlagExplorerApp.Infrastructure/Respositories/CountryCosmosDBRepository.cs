using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlagExplorerApp.Domain.Entities;
using FlagExplorerApp.Domain.Repositories;
using FlagExplorerApp.Domain.Repositories.Documents;
using FlagExplorerApp.Infrastructure.Persistance;
using FlagExplorerApp.Infrastructure.Persistance.Documents;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Options;

namespace FlagExplorerApp.Infrastructure.Respositories
{
    internal class CountryCosmosDBRepository : CosmosDBRepositoryBase<Country, CountryDocument, ICountryDocument>, ICountryRepository
    {
        public CountryCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            Microsoft.Azure.CosmosRepository.IRepository<CountryDocument> cosmosRepository,
            ICosmosContainerProvider<CountryDocument> containerProvider,
            IOptionsMonitor<RepositoryOptions> optionsMonitor) : base(unitOfWork, cosmosRepository, "id", containerProvider, optionsMonitor)
        {
        }

        public async Task<Country?> FindByIdAsync(string id, CancellationToken cancellationToken = default) => await base.FindByIdAsync(id: id, cancellationToken: cancellationToken);
    }
}
