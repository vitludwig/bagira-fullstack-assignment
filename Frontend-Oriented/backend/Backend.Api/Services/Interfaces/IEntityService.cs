using Backend.Domain.Models;
using Backend.Infrastructure.Models;

namespace Backend.Api.Services.Interfaces;

public interface IEntityService
{
    Task<PagedResult<Entity>?> GetByScenarioIdAsync(
        Guid scenarioId, int page, int pageSize, string? search,
        EntityType? type, TaskForce? taskForce, string sortBy, string sortDirection,
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
