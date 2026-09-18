using Backend.Domain.Models;

namespace Backend.Api.Services.Interfaces;

public interface IEntityService
{
    Task<IReadOnlyCollection<Entity>?> GetByScenarioIdAsync(
        Guid scenarioId,
        CancellationToken cancellationToken);

    Task<Entity?> GetByIdAsync(Guid entityId, CancellationToken cancellationToken);

    Task<Entity?> CreateAsync(
        Guid scenarioId,
        EntityType type,
        TaskForce taskForce,
        string name,
        double latitude,
        double longitude,
        CancellationToken cancellationToken);

}
