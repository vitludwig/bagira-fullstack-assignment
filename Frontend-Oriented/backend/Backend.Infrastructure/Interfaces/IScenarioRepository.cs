using Backend.Domain.Models;
using Backend.Infrastructure.Models;

namespace Backend.Infrastructure.Interfaces;

public interface IScenarioRepository : IRepository<Scenario>
{
    Task<PagedResult<ScenarioListRecord>> GetAllAsync(
        GridRequest request,
        CancellationToken cancellationToken);
}
