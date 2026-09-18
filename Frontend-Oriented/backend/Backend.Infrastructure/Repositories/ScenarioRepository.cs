using Backend.Domain.Models;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Interfaces;
using Backend.Infrastructure.Models;
using Gridify;
using Gridify.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

public class ScenarioRepository(BackendDbContext dbContext) : IScenarioRepository
{
    private static readonly IGridifyMapper<ScenarioListRecord> GridMapper = new GridifyMapper<ScenarioListRecord>()
        .AddMap("id", record => record.Scenario.Id)
        .AddMap("name", record => record.Scenario.Name)
        .AddMap("description", record => record.Scenario.Description!)
        .AddMap("updatedAt", record => record.Scenario.UpdatedAt)
        .AddMap("entityCount", record => record.EntityCount);

    public async Task<PagedResult<ScenarioListRecord>> GetAllAsync(
        GridRequest request,
        CancellationToken cancellationToken)
    {
        var source = dbContext.Scenarios
            .AsNoTracking()
            .Select(scenario => new ScenarioListRecord
            {
                Scenario = scenario,
                EntityCount = dbContext.Entities.Count(entity => entity.ScenarioId == scenario.Id)
            });
        var result = await source.GridifyAsync(new GridifyQuery
        {
            Page = request.Page,
            PageSize = request.PageSize,
            Filter = request.Filter,
            OrderBy = $"{request.OrderBy},id asc"
        }, cancellationToken, GridMapper);

        return new PagedResult<ScenarioListRecord>
        {
            Items = result.Data.ToList(),
            TotalCount = result.Count
        };
    }

    public Task<Scenario?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Scenarios
            .AsNoTracking()
            .SingleOrDefaultAsync(scenario => scenario.Id == id, cancellationToken);
    }

    public async Task<Scenario> AddAsync(Scenario scenario, CancellationToken cancellationToken)
    {
        if (scenario.Id == Guid.Empty)
            scenario.Id = Guid.NewGuid();

        var now = DateTime.UtcNow;
        scenario.CreatedAt = now;
        scenario.UpdatedAt = now;

        dbContext.Scenarios.Add(scenario);
        await dbContext.SaveChangesAsync(cancellationToken);
        return scenario;
    }
}
