namespace FlagExplorerApp.Domain.Entities;

public record class CountryDetail : BaseEntity
{
    public required string Name { get; set; }
    public int Population { get; set; }
    public string? Capital { get; set; }
    public string? Flag { get; set; }
}
