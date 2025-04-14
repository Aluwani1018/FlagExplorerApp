namespace FlagExplorerApp.Domain.Repositories.Documents;

public interface ICountryDocument
{
    string Id { get; }
    string Name { get; set; }
    string? Flag { get; set; }
}
