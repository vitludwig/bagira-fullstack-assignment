using System.Collections.Concurrent;
using Backend.Domain.Models;
using Backend.Infrastructure.Interfaces;

namespace Backend.Infrastructure.Repositories;

public class EntityRepository : IEntityRepository
{
    internal static readonly ConcurrentDictionary<Guid, Entity> Store = new();

    public Task<Entity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Store.TryGetValue(id, out var entity);
        return Task.FromResult(entity);
    }

    public Task<IReadOnlyCollection<Entity>> GetAllAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyCollection<Entity>>(Store.Values.ToList());
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

    public Task<IReadOnlyCollection<Entity>> GetByScenarioIdAsync(
        Guid scenarioId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entities = Store.Values
            .Where(e => e.ScenarioId == scenarioId)
            .ToList();
        return Task.FromResult<IReadOnlyCollection<Entity>>(entities);
    }

    public Task<IReadOnlyDictionary<Guid, int>> GetCountsByScenarioIdsAsync(
        IEnumerable<Guid> scenarioIds,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var scenarioIdSet = scenarioIds.ToHashSet();
        var counts = Store.Values
            .Where(entity => scenarioIdSet.Contains(entity.ScenarioId))
            .GroupBy(entity => entity.ScenarioId)
            .ToDictionary(group => group.Key, group => group.Count());

        return Task.FromResult<IReadOnlyDictionary<Guid, int>>(counts);
    }
}
