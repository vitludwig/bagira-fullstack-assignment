using Backend.Api.Services.Models;
using Backend.Domain.Models;
using Backend.Infrastructure.Models;

namespace Backend.Api.Services.Interfaces;

public interface IScenarioService
{
    Task<PagedResult<ScenarioSummary>> GetAllAsync(
        int page, int pageSize, string? search, string sortBy, string sortDirection,
        CancellationToken cancellationToken);
    Task<Scenario?> GetByIdAsync(Guid scenarioId, CancellationToken cancellationToken);
    Task<Scenario> CreateAsync(string name, string? description, CancellationToken cancellationToken);
}
