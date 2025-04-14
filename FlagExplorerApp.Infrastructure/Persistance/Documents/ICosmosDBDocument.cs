using Microsoft.Azure.CosmosRepository;

namespace FlagExplorerApp.Infrastructure.Persistance.Documents
{
    internal interface ICosmosDBDocument<TDomain, out TDocument> : ICosmosDBDocument
        where TDomain : class
        where TDocument : ICosmosDBDocument<TDomain, TDocument>
    {
        TDocument PopulateFromEntity(TDomain entity, Func<string, string?> getEtag);
        TDomain ToEntity(TDomain? entity = null);
    }

    internal interface ICosmosDBDocument : IItemWithEtag
    {
        string IItem.PartitionKey => PartitionKey!;
        new string? PartitionKey
        {
            get => Id;
            set => Id = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
