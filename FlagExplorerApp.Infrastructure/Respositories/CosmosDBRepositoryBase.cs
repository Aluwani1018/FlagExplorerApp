using System.Linq.Expressions;
using System.Net;
using FlagExplorerApp.Domain.Common.Interfaces;
using FlagExplorerApp.Domain.Repositories;
using FlagExplorerApp.Infrastructure.Persistance;
using FlagExplorerApp.Infrastructure.Persistance.Documents;
using FlagExplorerApp.Infrastructure.Persistance.Documents.Extensions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Extensions;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Options;

namespace FlagExplorerApp.Infrastructure.Respositories;

internal abstract class CosmosDBRepositoryBase<TDomain, TDocument, TDocumentInterface> : ICosmosDBRepository<TDomain, TDocumentInterface>
        where TDomain : class
        where TDocument : ICosmosDBDocument<TDomain, TDocument>, TDocumentInterface, new()
{
    private readonly Dictionary<string, string?> _eTags = new Dictionary<string, string?>();
    private readonly string _documentType;
    private readonly CosmosDBUnitOfWork _unitOfWork;
    private readonly Microsoft.Azure.CosmosRepository.IRepository<TDocument> _cosmosRepository;
    private readonly string _idFieldName;
    private readonly ICosmosContainerProvider<TDocument> _containerProvider;
    private readonly IOptionsMonitor<RepositoryOptions> _optionsMonitor;

    protected CosmosDBRepositoryBase(CosmosDBUnitOfWork unitOfWork,
        Microsoft.Azure.CosmosRepository.IRepository<TDocument> cosmosRepository,
        string idFieldName,
        ICosmosContainerProvider<TDocument> containerProvider,
        IOptionsMonitor<RepositoryOptions> optionsMonitor)
    {
        _unitOfWork = unitOfWork;
        _cosmosRepository = cosmosRepository;
        _idFieldName = idFieldName;
        _containerProvider = containerProvider;
        _optionsMonitor = optionsMonitor;
        _documentType = typeof(TDocument).GetNameForDocument();
    }

    public ICosmosDBUnitOfWork UnitOfWork => _unitOfWork;

    public void Add(TDomain entity)
    {
        _unitOfWork.Track(entity);
        _unitOfWork.Enqueue(async cancellationToken =>
        {
            var document = new TDocument().PopulateFromEntity(entity, _ => null);
            await _cosmosRepository.CreateAsync(document, cancellationToken: cancellationToken);
        });
    }

    public void Update(TDomain entity)
    {
        _unitOfWork.Enqueue(async cancellationToken =>
        {
            var document = new TDocument().PopulateFromEntity(entity, _eTags.GetValueOrDefault);
            await _cosmosRepository.UpdateAsync(document, cancellationToken: cancellationToken);
        });
    }

    public void Remove(TDomain entity)
    {
        _unitOfWork.Enqueue(async cancellationToken =>
        {
            var document = new TDocument().PopulateFromEntity(entity, _eTags.GetValueOrDefault);
            await _cosmosRepository.DeleteAsync(document, cancellationToken: cancellationToken);
        });
    }

    public async Task<TDomain?> FindAsync(
        Expression<Func<TDocumentInterface, bool>> filterExpression,
        CancellationToken cancellationToken = default)
    {
        var documents = await _cosmosRepository.GetAsync(AdaptFilterPredicate(filterExpression), cancellationToken).ToListAsync();

        if (!documents.Any())
        {
            return default;
        }
        var entity = LoadAndTrackDocument(documents.First());

        return entity;
    }

    public async Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default)
    {
        var documents = await _cosmosRepository.GetAsync(_ => true, cancellationToken);
        var results = LoadAndTrackDocuments(documents).ToList();

        return results;
    }

    protected async Task<TDomain?> FindByIdAsync(
        string id,
        string? partitionKey = default,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var document = await _cosmosRepository.GetAsync(id, partitionKey, cancellationToken: cancellationToken);
            var entity = LoadAndTrackDocument(document);

            return entity;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<List<TDomain>> FindAllAsync(
        Expression<Func<TDocumentInterface, bool>> filterExpression,
        CancellationToken cancellationToken = default)
    {
        var documents = await _cosmosRepository.GetAsync(AdaptFilterPredicate(filterExpression), cancellationToken);
        var results = LoadAndTrackDocuments(documents).ToList();

        return results;
    }

    public async Task<List<TDomain>> FindByIdsAsync(
        IEnumerable<string> ids,
        CancellationToken cancellationToken = default)
    {
        var queryDefinition = new QueryDefinition($"SELECT * from c WHERE ARRAY_CONTAINS(@ids, c.{_idFieldName})")
            .WithParameter("@ids", ids);

        return await FindAllAsync(queryDefinition);
    }

    public async Task<TDomain?> FindAsync(
        Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>> queryOptions,
        CancellationToken cancellationToken = default)
    {
        var queryable = await CreateQuery(queryOptions);
        var documents = await ProcessResults(queryable, cancellationToken);

        if (!documents.Any())
        {
            return default;
        }
        var entity = LoadAndTrackDocument(documents.First());

        return entity;
    }

    public async Task<List<TDomain>> FindAllAsync(
        Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>> queryOptions,
        CancellationToken cancellationToken = default)
    {
        var queryable = await CreateQuery(queryOptions);
        var documents = await ProcessResults(queryable, cancellationToken);
        var results = LoadAndTrackDocuments(documents).ToList();

        return results;
    }

    protected async Task<List<TDomain>> FindAllAsync(
        QueryDefinition queryDefinition,
        CancellationToken cancellationToken = default)
    {
        var documents = await _cosmosRepository.GetByQueryAsync(queryDefinition, cancellationToken);
        var results = LoadAndTrackDocuments(documents).ToList();

        return results;
    }

    protected async Task<TDomain?> FindAsync(
        QueryDefinition queryDefinition,
        CancellationToken cancellationToken = default)
    {
        var documents = await _cosmosRepository.GetByQueryAsync(queryDefinition, cancellationToken);

        if (!documents.Any())
        {
            return default;
        }
        var entity = LoadAndTrackDocument(documents.First());

        return entity;
    }

    protected async Task<IQueryable<TDocumentInterface>> CreateQuery(
        Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>>? queryOptions = default,
        QueryRequestOptions? requestOptions = default)
    {
        var container = await _containerProvider.GetContainerAsync();
        var queryable = (IQueryable<TDocumentInterface>)container.GetItemLinqQueryable<TDocumentInterface>(requestOptions: requestOptions, linqSerializerOptions: _optionsMonitor.CurrentValue.SerializationOptions);
        queryable = queryOptions == null ? queryable : queryOptions(queryable);
        //Filter by document type
        queryable = queryable.Where(d => ((IItem)d!).Type == _documentType);

        return queryable;
    }

    protected async Task<List<TProjection>> ProcessResults<TProjection>(
        IQueryable<TProjection> query,
        CancellationToken cancellationToken = default)
    {
        var results = new List<TProjection>();

        using var feedIterator = query.ToFeedIterator();

        while (feedIterator.HasMoreResults)
        {
            results.AddRange(await feedIterator.ReadNextAsync(cancellationToken));
        }

        return results;
    }

    protected async Task<List<TDocument>> ProcessResults(
        IQueryable<TDocumentInterface> query,
        CancellationToken cancellationToken = default)
    {
        var results = new List<TDocument>();

        using var feedIterator = query
            .Select(x => (TDocument?)x) // Explicitly cast to nullable type
            .ToFeedIterator();

        while (feedIterator.HasMoreResults)
        {
            var response = await feedIterator.ReadNextAsync(cancellationToken);
            results.AddRange(response.Where(x => x != null)!); // Filter out nulls and ensure non-nullable
        }

        return results;
    }

    /// <summary>
    /// Adapts a <typeparamref name="TDocumentInterface"/> predicate to a <typeparamref name="TDocument"/> predicate.
    /// </summary>
    private static Expression<Func<TDocument, bool>> AdaptFilterPredicate(Expression<Func<TDocumentInterface, bool>> expression)
    {
        var beforeParameter = expression.Parameters.Single();
        var afterParameter = Expression.Parameter(typeof(TDocument), beforeParameter.Name);
        var visitor = new SubstitutionExpressionVisitor(beforeParameter, afterParameter);
        return Expression.Lambda<Func<TDocument, bool>>(visitor.Visit(expression.Body)!, afterParameter);
    }

    public TDomain LoadAndTrackDocument(TDocument document)
    {
        var entity = document.ToEntity();

        _unitOfWork.Track(entity);
        _eTags[document.Id] = document.Etag;

        return entity;
    }

    public IEnumerable<TDomain> LoadAndTrackDocuments(IEnumerable<TDocument> documents)
    {
        foreach (var document in documents)
        {
            yield return LoadAndTrackDocument(document);
        }
    }

    private class SubstitutionExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _before;
        private readonly Expression _after;

        public SubstitutionExpressionVisitor(Expression before, Expression after)
        {
            _before = before;
            _after = after;
        }

        public override Expression? Visit(Expression? node)
        {
            return node == _before ? _after : base.Visit(node);
        }
    }
}

