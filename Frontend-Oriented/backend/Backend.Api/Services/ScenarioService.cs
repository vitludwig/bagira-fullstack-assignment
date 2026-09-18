using Backend.Api.Services.Interfaces;
using Backend.Api.Services.Models;
using Backend.Domain.Models;
using Backend.Infrastructure.Interfaces;

namespace Backend.Api.Services;

public class ScenarioService : IScenarioService
{
    private readonly IScenarioRepository _scenarioRepository;
    private readonly IEntityRepository _entityRepository;

    public ScenarioService(
        IScenarioRepository scenarioRepository,
        IEntityRepository entityRepository)
    {
        _scenarioRepository = scenarioRepository;
        _entityRepository = entityRepository;
    }

    public async Task<IReadOnlyCollection<ScenarioSummary>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var scenarios = await _scenarioRepository.GetAllAsync(cancellationToken);
        var counts = await _entityRepository.GetCountsByScenarioIdsAsync(
            scenarios.Select(scenario => scenario.Id),
            cancellationToken);

        return scenarios
            .Select(scenario => new ScenarioSummary
            {
                Scenario = scenario,
                EntityCount = counts.GetValueOrDefault(scenario.Id)
            })
            .ToList();
    }

    public Task<Scenario?> GetByIdAsync(Guid scenarioId, CancellationToken cancellationToken)
    {
        return _scenarioRepository.GetByIdAsync(scenarioId, cancellationToken);
    }

    public Task<Scenario> CreateAsync(
        string name,
        string? description,
        CancellationToken cancellationToken)
    {
        var scenario = new Scenario
        {
            Name = name.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim()
        };

        return _scenarioRepository.AddAsync(scenario, cancellationToken);
    }
}
