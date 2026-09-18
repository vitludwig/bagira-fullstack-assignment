using Backend.Api.Services.Models;
using Backend.Domain.Models;

namespace Backend.Api.Services.Interfaces;

public interface IScenarioService
{
    Task<IReadOnlyCollection<ScenarioSummary>> GetAllAsync(CancellationToken cancellationToken);
    Task<Scenario?> GetByIdAsync(Guid scenarioId, CancellationToken cancellationToken);
    Task<Scenario> CreateAsync(string name, string? description, CancellationToken cancellationToken);
}
