using System.Collections.Concurrent;
using Backend.Domain.Models;
using Backend.Infrastructure.Interfaces;
using Backend.Infrastructure.Models;
using Gridify;

namespace Backend.Infrastructure.Repositories;

public class ScenarioRepository : IScenarioRepository
{
    private static readonly IGridifyMapper<ScenarioListRecord> GridMapper = new GridifyMapper<ScenarioListRecord>()
        .AddMap("name", record => record.Scenario.Name)
        .AddMap("description", record => record.Scenario.Description!)
        .AddMap("updatedAt", record => record.Scenario.UpdatedAt)
        .AddMap("entityCount", record => record.EntityCount);
    internal static readonly object StoreLock = new();
    internal static readonly ConcurrentDictionary<Guid, Scenario> Store = new();

    public Task<PagedResult<ScenarioListRecord>> GetAllAsync(
        GridRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entityCounts = EntityRepository.Store.Values
            .GroupBy(entity => entity.ScenarioId)
            .ToDictionary(group => group.Key, group => group.Count());
        var source = Store.Values
            .Select(scenario => new ScenarioListRecord
            {
                Scenario = scenario,
                EntityCount = entityCounts.GetValueOrDefault(scenario.Id)
            })
            .AsQueryable();
        var result = source.Gridify(new GridifyQuery
        {
            Page = request.Page,
            PageSize = request.PageSize,
            Filter = request.Filter,
            OrderBy = request.OrderBy
        }, GridMapper);

        return Task.FromResult(new PagedResult<ScenarioListRecord>
        {
            Items = result.Data.ToList(),
            TotalCount = result.Count
        });
    }

    public Task<Scenario?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Store.TryGetValue(id, out var scenario);
        return Task.FromResult(scenario);
    }

    public Task<Scenario> AddAsync(Scenario scenario, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (scenario.Id == Guid.Empty)
            scenario.Id = Guid.NewGuid();

        var now = DateTime.UtcNow;
        scenario.CreatedAt = now;
        scenario.UpdatedAt = now;

        Store[scenario.Id] = scenario;
        return Task.FromResult(scenario);
    }
}
