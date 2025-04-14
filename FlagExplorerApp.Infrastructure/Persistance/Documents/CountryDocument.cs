using FlagExplorerApp.Domain.Entities;
using FlagExplorerApp.Domain.Repositories.Documents;
using FlagExplorerApp.Infrastructure.Persistance.Documents.Extensions;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

namespace FlagExplorerApp.Infrastructure.Persistance.Documents;

internal class CountryDocument : ICountryDocument, ICosmosDBDocument<Country, CountryDocument>
{
    private string? _type;
    [JsonProperty("_etag")]
    protected string? _etag;
    [JsonProperty("type")]
    string IItem.Type
    {
        get => _type ??= GetType().GetNameForDocument();
        set => _type = value;
    }
    string? IItemWithEtag.Etag => _etag;
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Flag { get; set; } = default!;

    public CountryDocument PopulateFromEntity(Country entity, Func<string, string?> getEtag)
    {
        Id = entity.Id;
        Name = entity.Name;
        Flag = entity.Flag;

        _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

        return this;
    }

    public Country ToEntity(Country? entity = null)
    {
        entity ??= new Country
        {
            Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null"),
            Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null")
        };

        entity.Flag = Flag;

        return entity;
    }
}
