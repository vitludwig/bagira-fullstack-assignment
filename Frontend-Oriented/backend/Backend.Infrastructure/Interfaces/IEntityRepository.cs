using Backend.Domain.Models;
using Backend.Infrastructure.Models;

namespace Backend.Infrastructure.Interfaces;

public interface IEntityRepository : IRepository<Entity>
{
    Task<PagedResult<Entity>> GetByScenarioIdAsync(
        Guid scenarioId,
        GridRequest request,
        CancellationToken cancellationToken);

    Task<Entity?> AddToScenarioAsync(
        Entity entity,
        CancellationToken cancellationToken);

}
