using System.Linq.Expressions;
using FlagExplorerApp.Domain.Common.Interfaces;

namespace FlagExplorerApp.Domain.Repositories;

public interface ICosmosDBRepository<TDomain, TDocumentInterface> : IRepository<TDomain>
{
    ICosmosDBUnitOfWork UnitOfWork { get; }
    Task<TDomain?> FindAsync(Expression<Func<TDocumentInterface, bool>> filterExpression, CancellationToken cancellationToken = default);
    Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default);
    Task<List<TDomain>> FindAllAsync(Expression<Func<TDocumentInterface, bool>> filterExpression, CancellationToken cancellationToken = default);
    Task<List<TDomain>> FindByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
    Task<TDomain?> FindAsync(Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<TDomain>> FindAllAsync(Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>> queryOptions, CancellationToken cancellationToken = default);
}