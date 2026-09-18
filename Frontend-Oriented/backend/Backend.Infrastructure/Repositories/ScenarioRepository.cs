using System.Collections.Concurrent;
using Backend.Domain.Models;
using Backend.Infrastructure.Interfaces;

namespace Backend.Infrastructure.Repositories;

public class ScenarioRepository : IScenarioRepository
{
    internal static readonly object StoreLock = new();
    internal static readonly ConcurrentDictionary<Guid, Scenario> Store = new();

    public Task<Scenario?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Store.TryGetValue(id, out var scenario);
        return Task.FromResult(scenario);
    }

    public Task<IReadOnlyCollection<Scenario>> GetAllAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyCollection<Scenario>>(Store.Values.ToList());
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
