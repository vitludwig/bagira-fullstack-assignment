using Backend.Api.Services.Interfaces;
using Backend.Api.Services.Models;
using Backend.Domain.Models;
using Backend.Infrastructure.Interfaces;
using Backend.Infrastructure.Models;
using Backend.Infrastructure.Utilities;

namespace Backend.Api.Services;

public class ScenarioService : IScenarioService
{
    private static readonly HashSet<string> SortFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "name", "updatedAt", "entityCount"
    };
    private readonly IScenarioRepository _scenarioRepository;

    public ScenarioService(IScenarioRepository scenarioRepository)
    {
        _scenarioRepository = scenarioRepository;
    }

    public async Task<PagedResult<ScenarioSummary>> GetAllAsync(
        int page,
        int pageSize,
        string? search,
        string sortBy,
        string sortDirection,
        CancellationToken cancellationToken)
    {
        var normalizedSortBy = SortFields.Contains(sortBy) ? sortBy : "updatedAt";
        var direction = sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc";
        var filter = string.IsNullOrWhiteSpace(search)
            ? null
            : GridifyFilterBuilder.Or(
                GridifyFilterBuilder.Contains("name", search.Trim()),
                GridifyFilterBuilder.Contains("description", search.Trim()));
        var result = await _scenarioRepository.GetAllAsync(new GridRequest
        {
            Page = page,
            PageSize = pageSize,
            Filter = filter,
            OrderBy = $"{normalizedSortBy} {direction}"
        }, cancellationToken);

        return new PagedResult<ScenarioSummary>
        {
            Items = result.Items.Select(record => new ScenarioSummary
            {
                Scenario = record.Scenario,
                EntityCount = record.EntityCount
            }).ToList(),
            TotalCount = result.TotalCount
        };
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
