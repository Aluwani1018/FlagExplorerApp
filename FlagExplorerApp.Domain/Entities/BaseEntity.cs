namespace FlagExplorerApp.Domain.Entities;

public abstract record BaseEntity
{
    public required string Id { get; set; }
}