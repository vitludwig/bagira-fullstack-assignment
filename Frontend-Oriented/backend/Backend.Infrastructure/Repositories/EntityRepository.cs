using System.Collections.Concurrent;
using Backend.Domain.Models;
using Backend.Infrastructure.Interfaces;
using Backend.Infrastructure.Models;
using Gridify;

namespace Backend.Infrastructure.Repositories;

public class EntityRepository : IEntityRepository
{
    private static readonly IGridifyMapper<Entity> GridMapper = new GridifyMapper<Entity>()
        .AddMap("name", entity => entity.Name)
        .AddMap("type", entity => entity.Type)
        .AddMap("taskForce", entity => entity.TaskForce)
        .AddMap("latitude", entity => entity.Latitude)
        .AddMap("longitude", entity => entity.Longitude)
        .AddMap("updatedAt", entity => entity.UpdatedAt);
    internal static readonly ConcurrentDictionary<Guid, Entity> Store = new();

    public Task<PagedResult<Entity>> GetByScenarioIdAsync(
        Guid scenarioId,
        GridRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = Store.Values
            .Where(entity => entity.ScenarioId == scenarioId)
            .AsQueryable()
            .Gridify(new GridifyQuery
            {
                Page = request.Page,
                PageSize = request.PageSize,
                Filter = request.Filter,
                OrderBy = request.OrderBy
            }, GridMapper);

        return Task.FromResult(new PagedResult<Entity>
        {
            Items = result.Data.ToList(),
            TotalCount = result.Count
        });
    }

    public Task<Entity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Store.TryGetValue(id, out var entity);
        return Task.FromResult(entity);
    }

    public Task<Entity> AddAsync(Entity entity, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (entity.Id == Guid.Empty)
            entity.Id = Guid.NewGuid();

        var now = DateTime.UtcNow;
        entity.CreatedAt = now;
        entity.UpdatedAt = now;

        Store[entity.Id] = entity;
        return Task.FromResult(entity);
    }

    public Task<Entity?> AddToScenarioAsync(
        Entity entity,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (ScenarioRepository.StoreLock)
        {
            if (!ScenarioRepository.Store.ContainsKey(entity.ScenarioId))
                return Task.FromResult<Entity?>(null);

            if (entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();

            var now = DateTime.UtcNow;
            entity.CreatedAt = now;
            entity.UpdatedAt = now;

            Store[entity.Id] = entity;
            return Task.FromResult<Entity?>(entity);
        }
    }

}
