using Backend.Domain.Models;

namespace Backend.Infrastructure.Interfaces;

public interface IEntityRepository : IRepository<Entity>
{
    Task<Entity?> AddToScenarioAsync(
        Entity entity,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Entity>> GetByScenarioIdAsync(
        Guid scenarioId,
        CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<Guid, int>> GetCountsByScenarioIdsAsync(
        IEnumerable<Guid> scenarioIds,
        CancellationToken cancellationToken);
}
