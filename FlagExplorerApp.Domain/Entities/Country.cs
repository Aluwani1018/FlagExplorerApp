namespace FlagExplorerApp.Domain.Entities;

public record class Country : BaseEntity
{
    public required string Name { get; set; }
    public string? Flag { get; set; }
}

