namespace FlagExplorerApp.Domain.Repositories.Documents;

public interface ICountryDetailDocument
{
    string Id { get; }
    string Name { get; set; }
    int Population { get; set; }
    string? Capital { get; set; }
    string? Flag { get; set; }
}
