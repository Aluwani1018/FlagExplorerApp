namespace FlagExplorerApp.Domain.Common.Interfaces;

public interface ICosmosDBUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
